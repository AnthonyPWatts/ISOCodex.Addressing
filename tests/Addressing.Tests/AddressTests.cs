using ISOCodex.Countries;

namespace ISOCodex.Addressing.Tests;

public class AddressTests
{
    [Fact]
    public void Constructor_MissingLine1_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new Address(
            null!,
            null,
            "London",
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB")));
    }

    [Fact]
    public void Constructor_MissingCity_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new Address(
            "10 Downing St",
            null,
            null!,
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB")));
    }

    [Fact]
    public void Constructor_ValidAddress_ShouldCreateInstance()
    {
        var address = new Address(
            "10 Downing St",
            null,
            "London",
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB"));

        Assert.Equal("10 Downing St", address.Line1);
        Assert.Equal("London", address.City);
    }
}
