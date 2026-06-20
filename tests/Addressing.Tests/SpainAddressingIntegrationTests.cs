using ISOCodex.Addressing.Spain;
using ISOCodex.Countries;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class SpainAddressingIntegrationTests
{
    private readonly SpanishAddressValidator _validator = new();

    [Fact]
    public void Validate_WithPostalCodeWhitespace_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode(" 28013 ")));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("00000")]
    [InlineData("53000")]
    public void Validate_WithInvalidPostalPrefix_ReturnsProvinceUnknownIssue(string postalCode)
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode(postalCode), stateOrProvince: null));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.ProvinceUnknown");
    }

    [Theory]
    [InlineData("Gipuzkoa")]
    [InlineData("Guipúzcoa")]
    public void Validate_WithProvinceAlias_ReturnsValidResult(string province)
    {
        var result = _validator.Validate(CreateAddress(
            city: "Donostia",
            stateOrProvince: province,
            postalCode: new PostalCode("20001")));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithDefaultPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(new Address(
            "Calle Mayor 1",
            null,
            "Madrid",
            "Madrid",
            default,
            CountryAlpha2Code.Parse("ES")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithWrongCountryCode_ReturnsCountryCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(countryCode: CountryAlpha2Code.Parse("FR")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Fact]
    public void AddSpainAddressing_RegistersSpanishValidatorWithoutStartupActions()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddSpainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();

        var address = new Address(
            "Calle Mayor 1",
            null,
            "Madrid",
            "Madrid",
            new PostalCode("28013"),
            CountryAlpha2Code.Parse("ES"));

        var result = factory.GetValidator(CountryAlpha2Code.Parse("ES")).Validate(address);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void AddSpainAddressing_ValidatorReturnsStructuredIssues()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddSpainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();

        var address = new Address(
            "Calle Mayor 1",
            null,
            "Madrid",
            "Barcelona",
            new PostalCode("28013"),
            CountryAlpha2Code.Parse("ES"));

        var result = factory.GetValidator(CountryAlpha2Code.Parse("ES")).Validate(address);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.ProvinceMismatch");
    }

    [Fact]
    public void AddSpainAddressing_RegistersSpanishFormatter()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddSpainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();

        var address = new Address(
            "Calle Mayor 1",
            null,
            "Madrid",
            "Madrid",
            new PostalCode("28013"),
            CountryAlpha2Code.Parse("ES"));

        Assert.Equal(
            "Calle Mayor 1\n28013 Madrid\nSpain",
            formatter.Format(address));
    }

    [Fact]
    public void AddSpainAddressing_RegistersProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddSpainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();

        var profile = profileProvider.GetProfile(CountryAlpha2Code.Parse("ES"));

        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("Provincia", profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea).Label);
        Assert.Contains(
            profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea).Options,
            option => option.Value == "Madrid");
    }

    [Fact]
    public void Formatter_WithLine2_IncludesLine2()
    {
        var formatter = new SpanishAddressFormatter();

        Assert.Equal(
            "Calle Mayor 1\n3 C\n28013 Madrid\nSpain",
            formatter.Format(CreateAddress(line2: "3 C")));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new SpanishAddressFormatter();

        Assert.Equal(
            "Calle Mayor 1\n28013 Madrid",
            formatter.Format(
                CreateAddress(),
                new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "Calle Mayor 1",
        string? line2 = null,
        string city = "Madrid",
        string? stateOrProvince = "Madrid",
        PostalCode postalCode = default,
        CountryAlpha2Code countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("28013") : postalCode,
            countryCode.Equals(default) ? CountryAlpha2Code.Parse("ES") : countryCode);
    }
}
