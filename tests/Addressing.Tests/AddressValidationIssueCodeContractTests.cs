using System.Linq;
using ISOCodex.Countries;
using ISOCodex.Addressing.GreatBritain;
using ISOCodex.Addressing.Spain;
using ISOCodex.Addressing.UnitedStates;
using ISOCodex.Addressing.Validation.Validators;

namespace ISOCodex.Addressing.Tests;

public class AddressValidationIssueCodeContractTests
{
    [Fact]
    public void BuiltInValidator_WithNullAddress_ReturnsStableCode()
    {
        var result = new GBAddressValidator().Validate(null);

        AssertCodes(result.Issues.Select(issue => issue.Code),
            "Address.Required");
    }

    [Fact]
    public void BuiltInValidator_WithInvalidPostalCode_ReturnsStableCode()
    {
        var result = new GBAddressValidator().Validate(
            new Address(
                "10 Downing Street",
                null,
                "London",
                null,
                new PostalCode("BADCODE"),
                CountryAlpha2Code.Parse("GB")));

        AssertCodes(result.Issues.Select(issue => issue.Code),
            "Address.PostalCode.Invalid");
    }

    [Fact]
    public void BuiltInValidator_WithCountryMismatch_ReturnsStableCode()
    {
        var result = new GBAddressValidator().Validate(
            new Address(
                "1600 Pennsylvania Avenue NW",
                null,
                "Washington",
                "DC",
                new PostalCode("20500"),
                CountryAlpha2Code.Parse("US")));

        Assert.Contains(
            result.Issues,
            issue => issue.Code == "Address.CountryCode.Invalid");
    }

    [Fact]
    public void UsValidator_WithInvalidState_ReturnsStableCode()
    {
        var result = new USAddressValidator().Validate(
            new Address(
                "1600 Pennsylvania Avenue NW",
                null,
                "Washington",
                "XX",
                new PostalCode("20500"),
                CountryAlpha2Code.Parse("US")));

        AssertCodes(result.Issues.Select(issue => issue.Code),
            "Address.StateOrProvince.Invalid");
    }

    [Fact]
    public void PermissiveFallbackValidator_WithNullAddress_ReturnsStableCode()
    {
        var result = new PermissiveAddressValidator().Validate(null);

        AssertCodes(result.Issues.Select(issue => issue.Code),
            "Address.Required");
    }

    [Fact]
    public void SpainValidator_WithInvalidAddress_ReturnsStableCodes()
    {
        var result = new SpanishAddressValidator().Validate(
            new Address(
                "Calle Mayor 1",
                null,
                "Madrid",
                "Barcelona",
                new PostalCode("28013"),
                CountryAlpha2Code.Parse("ES")));

        AssertCodes(result.Issues.Select(issue => issue.Code),
            "Address.PostalCode.ProvinceMismatch");
    }

    private static void AssertCodes(
        IEnumerable<string> actualCodes,
        params string[] expectedCodes)
    {
        Assert.Equal(expectedCodes.OrderBy(code => code), actualCodes.OrderBy(code => code));
    }
}
