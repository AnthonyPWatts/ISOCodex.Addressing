using System;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Formatting
{
    internal sealed class AddressFormatterRegistration
    {
        public AddressFormatterRegistration(
            CountryAlpha2Code country,
            Func<ICountryAddressFormatter> createFormatter)
        {
            Country = country;
            CreateFormatter = createFormatter;
        }

        public CountryAlpha2Code Country { get; }

        public Func<ICountryAddressFormatter> CreateFormatter { get; }
    }
}
