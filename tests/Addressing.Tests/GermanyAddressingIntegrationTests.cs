using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Germany;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class GermanyAddressingIntegrationTests
{
    private readonly GermanyAddressValidator _validator = new();

    [Fact]
    public void AddGermanyAddressing_RegistersValidatorFormatterAndProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddGermanyAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
        var address = CreateAddress();

        Assert.True(validatorFactory.GetValidator(CountryAlpha2Code.Parse("DE")).Validate(address).IsValid);
        Assert.Equal("Pariser Platz 1\n10117 Berlin\nGermany", formatter.Format(address));

        var profile = profileProvider.GetProfile(CountryAlpha2Code.Parse("DE"));
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("Postcode", profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
        Assert.DoesNotContain(profile.Fields, field => field.Field == AddressField.AdministrativeArea && field.IsRequired);
    }

    [Fact]
    public void Validate_WithValidAddress_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("1011")));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithDefaultPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(CreateAddressWithDefaultPostalCode());

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithWrongCountryCode_ReturnsCountryCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(countryCode: CountryAlpha2Code.Parse("IT")));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Fact]
    public void Validate_WithoutState_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: null));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Formatter_WithLine2_IncludesLine2()
    {
        var formatter = new GermanyAddressFormatter();

        Assert.Equal(
            "Pariser Platz 1\nEtage 2\n10117 Berlin\nGermany",
            formatter.Format(CreateAddress(line2: "Etage 2")));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new GermanyAddressFormatter();

        Assert.Equal(
            "Pariser Platz 1\n10117 Berlin",
            formatter.Format(CreateAddress(), new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "Pariser Platz 1",
        string? line2 = null,
        string city = "Berlin",
        string? stateOrProvince = null,
        PostalCode postalCode = default,
        CountryAlpha2Code countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("10117") : postalCode,
            countryCode.Equals(default) ? CountryAlpha2Code.Parse("DE") : countryCode);
    }

    private static Address CreateAddressWithDefaultPostalCode()
    {
        return new Address("Pariser Platz 1", null, "Berlin", null, default, CountryAlpha2Code.Parse("DE"));
    }
}
