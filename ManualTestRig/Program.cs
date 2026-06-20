using ISOCodex.Addressing;
using ISOCodex.Countries;
using ISOCodex.Addressing.GreatBritain;
using ISOCodex.Addressing.Spain;
using ISOCodex.Addressing.UnitedStates;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = BuildAddressingServiceProvider();

var addressValidatorFactory =
    serviceProvider.GetRequiredService<IAddressValidatorFactory>();

var spanishAddress = new Address(
    "123 Main St",
    null,
    "Madrid",
    null,
    new PostalCode("28001"),
    CountryAlpha2Code.Parse("ES"));

addressValidatorFactory.GetValidator(spanishAddress.CountryCode).Validate(spanishAddress);
Console.WriteLine("Spanish address is valid!");

var ukAddress = new Address(
    "10 Downing St",
    null,
    "London",
    null,
    new PostalCode("SW1A 2AA"),
    CountryAlpha2Code.Parse("GB"));

addressValidatorFactory.GetValidator(ukAddress.CountryCode).Validate(ukAddress);
Console.WriteLine("UK address is valid!");

return 0;

static IServiceProvider BuildAddressingServiceProvider()
{
    var services = new ServiceCollection();

    services
        .AddAddressing()
        .AddGreatBritainAddressing()
        .AddUnitedStatesAddressing()
        .AddSpainAddressing();

    return services.BuildServiceProvider();
}
