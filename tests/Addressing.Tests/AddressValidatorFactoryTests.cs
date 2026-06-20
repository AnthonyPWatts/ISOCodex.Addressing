using ISOCodex.Addressing.Validation;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Tests;

public class AddressValidatorFactoryTests
{
    [Fact]
    public void RegisterValidator_ThenGetValidator_ReturnsRegisteredInstance()
    {
        var factory = new AddressValidatorFactory();
        var validator = new TestAddressValidator();

        factory.RegisterValidator(CountryAlpha2Code.Parse("GB"), validator);

        var result = factory.GetValidator(CountryAlpha2Code.Parse("GB"));

        Assert.Same(validator, result);
    }

    [Fact]
    public void GetValidator_WhenNotRegistered_ThrowsInvalidOperationException()
    {
        var factory = new AddressValidatorFactory();

        var ex = Assert.Throws<InvalidOperationException>(
            () => factory.GetValidator(CountryAlpha2Code.Parse("GB")));

        Assert.Contains("GB", ex.Message);
    }

    [Fact]
    public void GetValidator_WhenFallbackRegistered_ReturnsFallbackForUnregisteredCountry()
    {
        var factory = new AddressValidatorFactory();
        var validator = new TestAddressValidator();
        var country = CountryAlpha2Code.Parse("FR");

        factory.RegisterFallbackValidator(validator);

        var result = factory.GetValidator(country);

        Assert.Same(validator, result);
    }

    [Theory]
    [InlineData("EU")]
    [InlineData("UK")]
    public void GetValidator_WhenFallbackRegistered_DoesNotUseFallbackForNonDeliverableCountryCodes(string countryCode)
    {
        var factory = new AddressValidatorFactory();
        factory.RegisterFallbackValidator(new TestAddressValidator());

        var ex = Assert.Throws<InvalidOperationException>(
            () => factory.GetValidator(CountryAlpha2Code.Parse(countryCode)));

        Assert.Contains(countryCode, ex.Message);
    }

    [Fact]
    public void GetValidator_WhenCountryAndFallbackRegistered_ReturnsCountryValidator()
    {
        var factory = new AddressValidatorFactory();
        var countryValidator = new TestAddressValidator();
        var fallbackValidator = new TestAddressValidator();

        factory.RegisterValidator(CountryAlpha2Code.Parse("GB"), countryValidator);
        factory.RegisterFallbackValidator(fallbackValidator);

        var result = factory.GetValidator(CountryAlpha2Code.Parse("GB"));

        Assert.Same(countryValidator, result);
    }

    private sealed class TestAddressValidator : IAddressValidator
    {
        public AddressValidationResult Validate(Address? address)
        {
            return AddressValidationResult.Success;
        }
    }
}
