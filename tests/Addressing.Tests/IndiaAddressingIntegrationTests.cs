using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.India;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class IndiaAddressingIntegrationTests
{
    private readonly IndiaAddressValidator _validator = new();

    [Fact]
    public void AddIndiaAddressing_RegistersValidatorFormatterAndProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddIndiaAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
        var address = CreateAddress();

        Assert.True(validatorFactory.GetValidator(CountryAlpha2Code.Parse("IN")).Validate(address).IsValid);
        Assert.Equal("Rashtrapati Bhavan\nNew Delhi 110004\nDL\nIndia", formatter.Format(address));

        var profile = profileProvider.GetProfile(CountryAlpha2Code.Parse("IN"));
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("PIN code", profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
        var administrativeArea = profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea);
        Assert.True(administrativeArea.IsRequired);
        Assert.Equal(AddressFieldInputKind.Select, administrativeArea.InputKind);
        Assert.Contains(administrativeArea.Options, option => option.Value == "DL");
    }

    [Fact]
    public void Validate_WithValidAddress_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: "Delhi"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("11004")));

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
        var result = _validator.Validate(CreateAddress(countryCode: CountryAlpha2Code.Parse("BR")));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Theory]
    [InlineData(null, "Address.StateOrProvince.Required")]
    [InlineData("XX", "Address.StateOrProvince.Invalid")]
    public void Validate_WithMissingOrInvalidState_ReturnsStateIssue(
        string? stateOrProvince,
        string expectedIssueCode)
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: stateOrProvince));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == expectedIssueCode);
    }

    [Fact]
    public void Formatter_WithLine2_IncludesLine2()
    {
        var formatter = new IndiaAddressFormatter();

        Assert.Equal(
            "Rashtrapati Bhavan\nGate 1\nNew Delhi 110004\nDL\nIndia",
            formatter.Format(CreateAddress(line2: "Gate 1")));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new IndiaAddressFormatter();

        Assert.Equal(
            "Rashtrapati Bhavan\nNew Delhi 110004\nDL",
            formatter.Format(CreateAddress(), new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "Rashtrapati Bhavan",
        string? line2 = null,
        string city = "New Delhi",
        string? stateOrProvince = "DL",
        PostalCode postalCode = default,
        CountryAlpha2Code countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("110004") : postalCode,
            countryCode.Equals(default) ? CountryAlpha2Code.Parse("IN") : countryCode);
    }

    private static Address CreateAddressWithDefaultPostalCode()
    {
        return new Address(
            "Rashtrapati Bhavan",
            null,
            "New Delhi",
            "DL",
            default,
            CountryAlpha2Code.Parse("IN"));
    }
}
