using ISOCodex.Addressing.Canada;
using ISOCodex.Countries;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Formatting.Formatters;
using ISOCodex.Addressing.GreatBritain;
using ISOCodex.Addressing.UnitedStates;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class AddressFormatterTests
{
    [Fact]
    public void Format_WithRegisteredGbFormatter_ReturnsMultiLineAddress()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFormatter(CountryAlpha2Code.Parse("GB"), new GBAddressFormatter());

        var address = new Address(
            "10 Downing Street",
            null,
            "London",
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB"));

        var result = formatter.Format(address);

        Assert.Equal(
            "10 Downing Street\nLondon\nSW1A 2AA\nUnited Kingdom",
            result);
    }

    [Fact]
    public void Format_WithSingleLineOptions_ReturnsSingleLineAddress()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFormatter(CountryAlpha2Code.Parse("US"), new USAddressFormatter());

        var address = new Address(
            "1600 Pennsylvania Avenue NW",
            null,
            "Washington",
            "DC",
            new PostalCode("20500"),
            CountryAlpha2Code.Parse("US"));

        var result = formatter.Format(
            address,
            new AddressFormatOptions
            {
                Style = AddressFormatStyle.SingleLine
            });

        Assert.Equal(
            "1600 Pennsylvania Avenue NW, Washington, DC 20500, United States",
            result);
    }

    [Fact]
    public void Format_WithLine2_IncludesLine2()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFormatter(CountryAlpha2Code.Parse("GB"), new GBAddressFormatter());

        var address = new Address(
            "Buckingham Palace",
            "The Mall",
            "London",
            null,
            new PostalCode("SW1A 1AA"),
            CountryAlpha2Code.Parse("GB"));

        var result = formatter.Format(address);

        Assert.Equal(
            "Buckingham Palace\nThe Mall\nLondon\nSW1A 1AA\nUnited Kingdom",
            result);
    }

    [Fact]
    public void Format_WithWhiteSpaceLine2_SkipsLine2()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFormatter(CountryAlpha2Code.Parse("GB"), new GBAddressFormatter());

        var address = new Address(
            "10 Downing Street",
            "   ",
            "London",
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB"));

        var result = formatter.Format(address);

        Assert.Equal(
            "10 Downing Street\nLondon\nSW1A 2AA\nUnited Kingdom",
            result);
    }

    [Fact]
    public void Format_WithCustomSingleLineSeparator_UsesSeparator()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFormatter(CountryAlpha2Code.Parse("US"), new USAddressFormatter());

        var address = new Address(
            "1600 Pennsylvania Avenue NW",
            null,
            "Washington",
            "DC",
            new PostalCode("20500"),
            CountryAlpha2Code.Parse("US"));

        var result = formatter.Format(
            address,
            new AddressFormatOptions
            {
                Style = AddressFormatStyle.SingleLine,
                SingleLineSeparator = " | "
            });

        Assert.Equal(
            "1600 Pennsylvania Avenue NW | Washington, DC 20500 | United States",
            result);
    }

    [Fact]
    public void Format_WithIncludeCountryFalse_ExcludesCountry()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFormatter(CountryAlpha2Code.Parse("CA"), new CAAddressFormatter());

        var address = new Address(
            "111 Wellington Street",
            null,
            "Ottawa",
            "ON",
            new PostalCode("K1A 0A9"),
            CountryAlpha2Code.Parse("CA"));

        var result = formatter.Format(
            address,
            new AddressFormatOptions
            {
                IncludeCountry = false
            });

        Assert.Equal("111 Wellington Street\nOttawa ON K1A 0A9", result);
    }

    [Fact]
    public void Format_WithNullAddress_ThrowsArgumentNullException()
    {
        var formatter = new AddressFormatter();

        Assert.Throws<ArgumentNullException>(() => formatter.Format(null!));
    }

    [Fact]
    public void Format_WhenNoFormatterRegistered_ThrowsInvalidOperationException()
    {
        var formatter = new AddressFormatter();
        var address = new Address(
            "Calle Mayor 1",
            null,
            "Madrid",
            "Madrid",
            new PostalCode("28013"),
            CountryAlpha2Code.Parse("ES"));

        var ex = Assert.Throws<InvalidOperationException>(() => formatter.Format(address));

        Assert.Contains("ES", ex.Message);
    }

    [Fact]
    public void Format_WhenFallbackFormatterRegistered_FormatsUnregisteredCountry()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFallbackFormatter(new GenericAddressFormatter());

        var address = new Address(
            "1 Rue de Rivoli",
            null,
            "Paris",
            null,
            new PostalCode("75001"),
            CountryAlpha2Code.Parse("FR"));

        var result = formatter.Format(address);

        Assert.Equal("1 Rue de Rivoli\nParis 75001\nFrance", result);
    }

    [Theory]
    [InlineData("EU")]
    [InlineData("UK")]
    public void Format_WhenFallbackFormatterRegistered_DoesNotFormatNonDeliverableCountryCodes(string countryCode)
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFallbackFormatter(new GenericAddressFormatter());

        var address = new Address(
            "1 Example Street",
            null,
            "Example City",
            null,
            new PostalCode("12345"),
            CountryAlpha2Code.Parse(countryCode));

        var ex = Assert.Throws<InvalidOperationException>(() => formatter.Format(address));

        Assert.Contains(countryCode, ex.Message);
    }

    [Fact]
    public void Format_WhenCountryAndFallbackFormatterRegistered_UsesCountryFormatter()
    {
        var formatter = new AddressFormatter();
        formatter.RegisterFormatter(CountryAlpha2Code.Parse("GB"), new GBAddressFormatter());
        formatter.RegisterFallbackFormatter(new GenericAddressFormatter());

        var address = new Address(
            "10 Downing Street",
            null,
            "London",
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB"));

        var result = formatter.Format(address);

        Assert.Equal(
            "10 Downing Street\nLondon\nSW1A 2AA\nUnited Kingdom",
            result);
    }

    [Fact]
    public void AddAddressing_WithGb_RegistersFormatter()
    {
        var services = new ServiceCollection();
        services.AddAddressing();
        services.AddGreatBritainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();

        var address = new Address(
            "10 Downing Street",
            null,
            "London",
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB"));

        Assert.Equal(
            "10 Downing Street\nLondon\nSW1A 2AA\nUnited Kingdom",
            formatter.Format(address));
    }

    [Fact]
    public void AddAddressFormatter_WithCustomFormatter_RegistersFormatter()
    {
        var services = new ServiceCollection();
        services.AddAddressFormatter(CountryAlpha2Code.Parse("ES"), () => new TestCountryAddressFormatter());

        using var serviceProvider = services.BuildServiceProvider();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();

        var address = new Address(
            "Calle Mayor 1",
            null,
            "Madrid",
            "Madrid",
            new PostalCode("28013"),
            CountryAlpha2Code.Parse("ES"));

        Assert.Equal("custom format", formatter.Format(address));
    }

    [Fact]
    public void AddGenericAddressingFallbacks_RegistersFallbackFormatter()
    {
        var services = new ServiceCollection();
        services.AddGenericAddressingFallbacks();

        using var serviceProvider = services.BuildServiceProvider();
        var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();

        var address = new Address(
            "1 Rue de Rivoli",
            null,
            "Paris",
            null,
            new PostalCode("75001"),
            CountryAlpha2Code.Parse("FR"));

        Assert.Equal("1 Rue de Rivoli\nParis 75001\nFrance", formatter.Format(address));
    }

    private sealed class TestCountryAddressFormatter : ICountryAddressFormatter
    {
        public string Format(Address address, AddressFormatOptions? options = null)
        {
            return "custom format";
        }
    }
}
