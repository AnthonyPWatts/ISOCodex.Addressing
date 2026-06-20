using ISOCodex.Addressing.GreatBritain;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Tests;

public class GBAddressValidatorTests
{
    private readonly GBAddressValidator _validator = new();

    [Theory]
    [InlineData("SW1A 2AA")]
    [InlineData("GIR 0AA")]
    [InlineData("M1 1AE")]
    [InlineData("B33 8TH")]
    [InlineData("CR2 6XH")]
    [InlineData("DN55 1PT")]
    public void Validate_WithValidPostcode_ReturnsValidResult(string postcode)
    {
        var address = CreateAddress(new PostalCode(postcode));

        var result = _validator.Validate(address);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithLowercasePostcodeWithoutSpace_ReturnsValidResult()
    {
        var address = CreateAddress(new PostalCode("sw1a2aa"));

        var result = _validator.Validate(address);

        Assert.True(result.IsValid);
        Assert.Equal("sw1a2aa", address.PostalCode.Code);
    }

    [Fact]
    public void Validate_WithInvalidPostcode_ReturnsIssue()
    {
        var address = CreateAddress(new PostalCode("BADCODE"));

        var result = _validator.Validate(address);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithInvalidAreaCombination_ReturnsIssue()
    {
        var result = _validator.Validate(CreateAddress(new PostalCode("QQ1 1AA")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithDefaultPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(new Address(
            "10 Downing St",
            null,
            "London",
            null,
            default,
            CountryAlpha2Code.Parse("GB")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithWrongCountryCode_ReturnsCountryCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(new PostalCode("SW1A 2AA"), CountryAlpha2Code.Parse("US")));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.CountryCode.Invalid");
    }

    private static Address CreateAddress(
        PostalCode postalCode,
        CountryAlpha2Code countryCode = default)
    {
        return new Address(
            "10 Downing St",
            null,
            "London",
            null,
            postalCode,
            countryCode.Equals(default) ? CountryAlpha2Code.Parse("GB") : countryCode);
    }
}
