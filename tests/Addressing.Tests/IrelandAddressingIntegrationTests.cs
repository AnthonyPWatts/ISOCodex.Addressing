using System.Linq;
using System.Reflection;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Ireland;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class IrelandAddressingIntegrationTests
{
    private readonly IrelandAddressValidator _validator = new();

    [Theory]
    [InlineData("D02 X285")]
    [InlineData("D02X285")]
    [InlineData("d02 x285")]
    public void Validate_WithValidEircode_ReturnsValidResult(string eircode)
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode(eircode)));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("D02 X28")]
    [InlineData("D02 X2859")]
    [InlineData("D02-285")]
    [InlineData("D0O X285")]
    public void Validate_WithInvalidEircode_ReturnsPostalCodeIssue(string eircode)
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode(eircode)));

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
        var result = _validator.Validate(CreateAddress(countryCode: CountryCode.FR));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Fact]
    public void AddIrelandAddressing_RegistersValidatorFormatterAndProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddIrelandAddressing();

        using var serviceProvider = services.BuildServiceProvider();

        var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
        var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
        var address = CreateAddress();

        Assert.True(validatorFactory.GetValidator(CountryCode.IE).Validate(address).IsValid);
        Assert.Equal(
            "1 College Green\nDublin\nD02 X285\nIreland",
            formatter.Format(address));

        var profile = profileProvider.GetProfile(CountryCode.IE);
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal("Eircode", profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
        Assert.Equal("Town / City", profile.Fields.Single(field => field.Field == AddressField.Locality).Label);
    }

    [Fact]
    public void Formatter_WithLine2_IncludesLine2()
    {
        var formatter = new IrelandAddressFormatter();
        var address = CreateAddress(line2: "Suite 2");

        Assert.Equal(
            "1 College Green\nSuite 2\nDublin\nD02 X285\nIreland",
            formatter.Format(address));
    }

    [Fact]
    public void Formatter_WithoutCountry_OmitsCountry()
    {
        var formatter = new IrelandAddressFormatter();

        Assert.Equal(
            "1 College Green\nDublin\nD02 X285",
            formatter.Format(
                CreateAddress(),
                new AddressFormatOptions { IncludeCountry = false }));
    }

    private static Address CreateAddress(
        string line1 = "1 College Green",
        string? line2 = null,
        string city = "Dublin",
        PostalCode postalCode = default,
        CountryCode countryCode = default)
    {
        return new Address(
            line1,
            line2,
            city,
            null,
            postalCode.Equals(default) ? new PostalCode("D02 X285") : postalCode,
            countryCode.Equals(default) ? CountryCode.IE : countryCode);
    }

    private static Address CreateAddressWithDefaultPostalCode()
    {
        return new Address(
            "1 College Green",
            null,
            "Dublin",
            null,
            default,
            CountryCode.IE);
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
