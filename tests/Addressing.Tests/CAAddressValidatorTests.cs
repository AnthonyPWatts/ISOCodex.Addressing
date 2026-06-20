using ISOCodex.Addressing.Canada;

namespace ISOCodex.Addressing.Tests;

public class CAAddressValidatorTests
{
    private readonly CAAddressValidator _validator = new();

    [Theory]
    [InlineData("K1A 0A9")]
    [InlineData("H0H 0H0")]
    [InlineData("V6B 1A1")]
    [InlineData("X1A 2N1")]
    public void Validate_WithValidPostalCode_ReturnsValidResult(string postalCode)
    {
        var address = CreateAddress(postalCode: new PostalCode(postalCode));

        var result = _validator.Validate(address);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithLowercasePostalCodeWithoutSpace_ReturnsValidResult()
    {
        var address = CreateAddress(postalCode: new PostalCode("k1a0a9"));

        var result = _validator.Validate(address);

        Assert.True(result.IsValid);
        Assert.Equal("k1a0a9", address.PostalCode.Code);
    }

    [Theory]
    [InlineData("D1A 1A1")]
    [InlineData("W1A 1A1")]
    [InlineData("Z1A 1A1")]
    [InlineData("K1O 1A1")]
    public void Validate_WithInvalidPostalCode_ReturnsIssue(string postalCode)
    {
        var result = _validator.Validate(CreateAddress(postalCode: new PostalCode(postalCode)));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithInvalidProvince_ReturnsIssue()
    {
        var address = CreateAddress(stateOrProvince: "XX");

        var result = _validator.Validate(address);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.StateOrProvince.Invalid");
    }

    [Fact]
    public void Validate_WithTrimmedProvince_ReturnsValidResult()
    {
        var result = _validator.Validate(CreateAddress(stateOrProvince: " ON "));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithDefaultPostalCode_ReturnsPostalCodeIssue()
    {
        var result = _validator.Validate(new Address(
            "111 Wellington St",
            null,
            "Ottawa",
            "ON",
            default,
            CountryCode.CA));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.PostalCode.Invalid");
    }

    [Fact]
    public void Validate_WithWrongCountryCode_ReturnsCountryCodeIssue()
    {
        var result = _validator.Validate(CreateAddress(countryCode: CountryCode.US));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.CountryCode.Invalid");
    }

    private static Address CreateAddress(
        string? stateOrProvince = "ON",
        PostalCode postalCode = default,
        CountryCode countryCode = default)
    {
        return new Address(
            "111 Wellington St",
            null,
            "Ottawa",
            stateOrProvince,
            postalCode.Equals(default) ? new PostalCode("K1A 0A9") : postalCode,
            countryCode.Equals(default) ? CountryCode.CA : countryCode);
    }
}
