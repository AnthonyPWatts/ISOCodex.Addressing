using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Italy;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class ItalyAddressingIntegrationTests
{
    private readonly ItalyAddressValidator _validator = new();

    [Fact]
    public void AddItalyAddressing_RegistersValidatorFormatterAndProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddItalyAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
        var address = CreateAddress();

        Assert.True(validatorFactory.GetValidator(CountryAlpha2Code.Parse("IT")).Validate(address).IsValid);
        Assert.Equal("Piazza del Colosseo 1\n00184 Roma RM\nItaly", formatter.Format(address));

        var profile = profileProvider.GetProfile(CountryAlpha2Code.Parse("IT"));
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("CAP", profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
        var administrativeArea = profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea);
        Assert.True(administrativeArea.IsRequired);
        Assert.Equal(AddressFieldInputKind.Select, administrativeArea.InputKind);
        Assert.Contains(administrativeArea.Options, option => option.Value == "RM");
    }

    [Fact]
    public void Validate_WithValidAddress_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: "Roma"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("0018")));

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
        var result = _validator.Validate(CreateAddress(countryCode: CountryAlpha2Code.Parse("DE")));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Theory]
    [InlineData(null, "Address.StateOrProvince.Required")]
    [InlineData("XX", "Address.StateOrProvince.Invalid")]
    public void Validate_WithMissingOrInvalidProvince_ReturnsStateIssue(
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
        var formatter = new ItalyAddressFormatter();

        Assert.Equal(
            "Piazza del Colosseo 1\nScala B\n00184 Roma RM\nItaly",
            formatter.Format(CreateAddress(line2: "Scala B")));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new ItalyAddressFormatter();

        Assert.Equal(
            "Piazza del Colosseo 1\n00184 Roma RM",
            formatter.Format(CreateAddress(), new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "Piazza del Colosseo 1",
        string? line2 = null,
        string city = "Roma",
        string? stateOrProvince = "RM",
        PostalCode postalCode = default,
        CountryAlpha2Code countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("00184") : postalCode,
            countryCode.Equals(default) ? CountryAlpha2Code.Parse("IT") : countryCode);
    }

    private static Address CreateAddressWithDefaultPostalCode()
    {
        return new Address("Piazza del Colosseo 1", null, "Roma", "RM", default, CountryAlpha2Code.Parse("IT"));
    }
}
