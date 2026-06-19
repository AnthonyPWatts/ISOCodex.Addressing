using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Mexico;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class MexicoAddressingIntegrationTests
{
    private readonly MexicoAddressValidator _validator = new();

    [Fact]
    public void AddMexicoAddressing_RegistersValidatorFormatterAndProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddMexicoAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
        var address = CreateAddress();

        Assert.True(validatorFactory.GetValidator(CountryCode.MX).Validate(address).IsValid);
        Assert.Equal("Palacio Nacional\n06066 Ciudad de México, CMX\nMexico", formatter.Format(address));

        var profile = profileProvider.GetProfile(CountryCode.MX);
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("Postal code", profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
        var administrativeArea = profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea);
        Assert.True(administrativeArea.IsRequired);
        Assert.Equal(AddressFieldInputKind.Select, administrativeArea.InputKind);
        Assert.Contains(administrativeArea.Options, option => option.Value == "CMX");
    }

    [Fact]
    public void Validate_WithValidAddress_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: "Ciudad de México"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithLeadingZeroPostalCode_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("06066")));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("6066")));

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
        var result = _validator.Validate(CreateAddress(countryCode: CountryCode.BR));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Theory]
    [InlineData(null, "Address.StateOrProvince.Required")]
    [InlineData("XXX", "Address.StateOrProvince.Invalid")]
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
        var formatter = new MexicoAddressFormatter();

        Assert.Equal(
            "Palacio Nacional\nCentro\n06066 Ciudad de México, CMX\nMexico",
            formatter.Format(CreateAddress(line2: "Centro")));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new MexicoAddressFormatter();

        Assert.Equal(
            "Palacio Nacional\n06066 Ciudad de México, CMX",
            formatter.Format(CreateAddress(), new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "Palacio Nacional",
        string? line2 = null,
        string city = "Ciudad de México",
        string? stateOrProvince = "CMX",
        PostalCode postalCode = default,
        CountryCode countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("06066") : postalCode,
            countryCode.Equals(default) ? CountryCode.MX : countryCode);
    }

    private static Address CreateAddressWithDefaultPostalCode()
    {
        return new Address("Palacio Nacional", null, "Ciudad de México", "CMX", default, CountryCode.MX);
    }
}
