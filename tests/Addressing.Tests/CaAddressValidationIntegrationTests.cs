using ISOCodex.Addressing.Canada;
using ISOCodex.Countries;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class CaAddressValidationIntegrationTests
{
    [Fact]
    public void AddAddressing_WithCa_AllowsValidationOfCaAddress()
    {
        var services = new ServiceCollection();
        services.AddAddressing();
        services.AddCanadaAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();

        var address = new Address(
            "111 Wellington St",
            null,
            "Ottawa",
            "ON",
            new PostalCode("K1A 0A9"),
            CountryAlpha2Code.Parse("CA"));

        var result = factory.GetValidator(address.CountryCode).Validate(address);

        Assert.True(result.IsValid);
    }
}
