using System;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Validation
{
    internal sealed class AddressValidatorRegistration
    {
        public AddressValidatorRegistration(
            CountryAlpha2Code country,
            Func<IAddressValidator> createValidator)
        {
            Country = country;
            CreateValidator = createValidator;
        }

        public CountryAlpha2Code Country { get; }

        public Func<IAddressValidator> CreateValidator { get; }
    }
}
