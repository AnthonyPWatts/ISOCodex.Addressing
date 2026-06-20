using ISOCodex.Countries;

namespace ISOCodex.Addressing.Tests;

public class CountryAlpha2CodeTests
{
    [Fact]
    public void Parse_WithValidCode_ReturnsCanonicalValue()
    {
        var result = CountryAlpha2Code.Parse("gb");

        Assert.Equal("GB", result.Value);
    }

    [Fact]
    public void TryParse_WithInvalidCode_ReturnsFalse()
    {
        var success = CountryAlpha2Code.TryParse("ZZZ", out var result);

        Assert.False(success);
        Assert.Equal(default, result);
    }

    [Fact]
    public void IsValidSyntax_WithValidAndInvalidCodes_ReturnsExpectedResult()
    {
        Assert.True(CountryAlpha2Code.IsValidSyntax("US"));
        Assert.False(CountryAlpha2Code.IsValidSyntax("USA"));
    }

    [Fact]
    public void CountryRegistry_DistinguishesCurrentCountriesFromSpecialCodeElements()
    {
        Assert.True(CountryRegistry.TryGetByAlpha2(CountryAlpha2Code.Parse("DE"), out var country));
        Assert.Equal("Germany", country!.EnglishShortName);

        Assert.False(CountryRegistry.TryGetByAlpha2(CountryAlpha2Code.Parse("EU"), out _));
        Assert.True(CountryCodeElementRegistry.TryGetByAlpha2(CountryAlpha2Code.Parse("EU"), out var element));
        Assert.NotEqual(CountryCodeElementKind.CurrentCountry, element!.Kind);
    }
}
