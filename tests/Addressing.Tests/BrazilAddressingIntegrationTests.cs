using ISOCodex.Addressing.Brazil;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class BrazilAddressingIntegrationTests
{
    private readonly BrazilAddressValidator _validator = new();

    [Fact]
    public void AddBrazilAddressing_RegistersValidatorFormatterAndProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddBrazilAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
        var address = CreateAddress();

        Assert.True(validatorFactory.GetValidator(CountryCode.BR).Validate(address).IsValid);
        Assert.Equal("Praça da Sé\nSão Paulo - SP\n01001-000\nBrazil", formatter.Format(address));

        var profile = profileProvider.GetProfile(CountryCode.BR);
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("CEP", profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
        var administrativeArea = profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea);
        Assert.True(administrativeArea.IsRequired);
        Assert.Equal(AddressFieldInputKind.Select, administrativeArea.InputKind);
        Assert.Contains(administrativeArea.Options, option => option.Value == "SP");
    }

    [Theory]
    [InlineData("01001-000")]
    [InlineData("01001000")]
    public void Validate_WithValidCep_ReturnsValidResult(string cep)
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode(cep)));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("01001")));

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
        var result = _validator.Validate(CreateAddress(countryCode: CountryCode.MX));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Theory]
    [InlineData(null, "Address.StateOrProvince.Required")]
    [InlineData("XX", "Address.StateOrProvince.Invalid")]
    public void Validate_WithMissingOrInvalidUf_ReturnsStateIssue(
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
        var formatter = new BrazilAddressFormatter();

        Assert.Equal(
            "Praça da Sé\nApto 10\nSão Paulo - SP\n01001-000\nBrazil",
            formatter.Format(CreateAddress(line2: "Apto 10")));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new BrazilAddressFormatter();

        Assert.Equal(
            "Praça da Sé\nSão Paulo - SP\n01001-000",
            formatter.Format(CreateAddress(), new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "Praça da Sé",
        string? line2 = null,
        string city = "São Paulo",
        string? stateOrProvince = "SP",
        PostalCode postalCode = default,
        CountryCode countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("01001-000") : postalCode,
            countryCode.Equals(default) ? CountryCode.BR : countryCode);
    }

    private static Address CreateAddressWithDefaultPostalCode()
    {
        return new Address("Praça da Sé", null, "São Paulo", "SP", default, CountryCode.BR);
    }
}
