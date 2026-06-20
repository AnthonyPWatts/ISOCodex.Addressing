using ISOCodex.Addressing.UnitedStates;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Tests;

public class USAddressValidatorTests
{
    private readonly USAddressValidator _validator = new();

    [Fact]
    public void Validate_WithValidAddress_ReturnsValidResult()
    {
        var address = CreateAddress();

        var result = _validator.Validate(address);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithZipPlus4_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode("20500-0001")));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithTrimmedState_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: " DC "));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("FM")]
    [InlineData("MH")]
    [InlineData("PW")]
    [InlineData("AA")]
    [InlineData("AE")]
    [InlineData("AP")]
    public void Validate_WithAdditionalUspsStateCode_ReturnsValidResult(string state)
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: state));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidState_ReturnsIssue()
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: "XX"));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.StateOrProvince.Invalid");
    }

    [Fact]
    public void Validate_WithoutState_ReturnsIssue()
    {
        var address = CreateAddress(stateOrProvince: null);

        var result = _validator.Validate(address);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.StateOrProvince.Required");
    }

    [Fact]
    public void Validate_WithDefaultPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(new Address(
            "1600 Pennsylvania Avenue NW",
            null,
            "Washington",
            "DC",
            default,
            CountryAlpha2Code.Parse("US")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithWrongCountryCode_ReturnsCountryCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(countryCode: CountryAlpha2Code.Parse("CA")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.CountryCode.Invalid");
    }

    private static Address CreateAddress(
        string? stateOrProvince = "DC",
        PostalCode postalCode = default,
        CountryAlpha2Code countryCode = default)
    {
        return new Address(
            "1600 Pennsylvania Avenue NW",
            null,
            "Washington",
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("20500") : postalCode,
            countryCode.Equals(default) ? CountryAlpha2Code.Parse("US") : countryCode);
    }
}
