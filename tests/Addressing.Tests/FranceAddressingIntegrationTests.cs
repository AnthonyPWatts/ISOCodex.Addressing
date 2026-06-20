using System.Linq;
using ISOCodex.Countries;
using System.Reflection;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.France;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class FranceAddressingIntegrationTests
{
    private readonly FranceAddressValidator _validator = new();

    [Fact]
    public void Validate_WithValidPostalCode_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithOverseasPostalCode_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("97100")));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("7500")]
    [InlineData("750011")]
    [InlineData("ABCDE")]
    public void Validate_WithInvalidPostalCode_ReturnsPostalCodeIssue(string postalCode)
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode(postalCode)));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithBlankPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(CreateAddressWithDefaultPostalCode());

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithBlankCity_ReturnsCityIssue()
    {
        var result = _validator.Validate(CreateInvalidAddress(city: " "));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.City.Required");
    }

    [Fact]
    public void Validate_WithBlankLine1_ReturnsLine1Issue()
    {
        var result = _validator.Validate(CreateInvalidAddress(line1: " "));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.Line1.Required");
    }

    [Fact]
    public void Validate_WithWrongCountryCode_ReturnsCountryCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(countryCode: CountryAlpha2Code.Parse("IE")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Fact]
    public void AddFranceAddressing_RegistersValidatorFormatterAndProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddFranceAddressing();

        using var serviceProvider = services.BuildServiceProvider();

        var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
        var address = CreateAddress();

        Assert.True(validatorFactory.GetValidator(CountryAlpha2Code.Parse("FR")).Validate(address).IsValid);
        Assert.Equal(
            "10 Rue de Rivoli\n75001 Paris\nFrance",
            formatter.Format(address));

        var profile = profileProvider.GetProfile(CountryAlpha2Code.Parse("FR"));
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("Postal code", profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
        Assert.Equal("City / Commune", profile.Fields.Single(field => field.Field == AddressField.Locality).Label);
    }

    [Fact]
    public void Formatter_WithLine2_IncludesLine2()
    {
        var formatter = new FranceAddressFormatter();
        var address = CreateAddress(line2: "Batiment A");

        Assert.Equal(
            "10 Rue de Rivoli\nBatiment A\n75001 Paris\nFrance",
            formatter.Format(address));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new FranceAddressFormatter();

        Assert.Equal(
            "10 Rue de Rivoli\n75001 Paris",
            formatter.Format(
                CreateAddress(),
                new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "10 Rue de Rivoli",
        string? line2 = null,
        string city = "Paris",
        PostalCode postalCode = default,
        CountryAlpha2Code countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            null,
            postalCode.Equals(default) ? new PostalCode("75001") : postalCode,
            countryCode.Equals(default) ? CountryAlpha2Code.Parse("FR") : countryCode);
    }

    private static Address CreateAddressWithDefaultPostalCode()
    {
        return new Address(
            "10 Rue de Rivoli",
            null,
            "Paris",
            null,
            default,
            CountryAlpha2Code.Parse("FR"));
    }

    private static Address CreateInvalidAddress(string? line1 = null, string? city = null)
    {
        var address = CreateAddress();

        SetProperty(address, nameof(Address.Line1), line1 ?? address.Line1);
        SetProperty(address, nameof(Address.City), city ?? address.City);

        return address;
    }

    private static void SetProperty(Address address, string propertyName, object? value)
    {
        var field = typeof(Address).GetField(
            $"<{propertyName}>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(field);
        field.SetValue(address, value);
    }
}
