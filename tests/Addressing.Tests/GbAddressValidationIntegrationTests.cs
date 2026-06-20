using ISOCodex.Addressing.GreatBritain;
using ISOCodex.Countries;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class GbAddressValidationIntegrationTests
{
    [Fact]
    public void AddAddressing_WithGb_AllowsValidationOfGbAddress()
    {
        var services = new ServiceCollection();
        services.AddAddressing();
        services.AddGreatBritainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();

        var address = new Address(
            "10 Downing St",
            null,
            "London",
            null,
            new PostalCode("SW1A 2AA"),
            CountryAlpha2Code.Parse("GB"));

        var result = factory.GetValidator(address.CountryCode).Validate(address);

        Assert.True(result.IsValid);
    }
}
