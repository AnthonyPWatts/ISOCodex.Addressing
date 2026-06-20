using System.Collections.Generic;
using ISOCodex.Countries;
using System.Collections.ObjectModel;
using System.Linq;

namespace ISOCodex.Addressing.Profiles
{
    public sealed class AddressProfile
    {
        public AddressProfile(
            CountryAlpha2Code countryCode,
            IEnumerable<AddressFieldProfile> fields,
            AddressProfileSource source = AddressProfileSource.CountrySpecific,
            string? examplePostalCode = null,
            string? exampleFormattedAddress = null)
        {
            CountryCode = countryCode;
            Source = source;
            Fields = new ReadOnlyCollection<AddressFieldProfile>(fields.ToArray());
            ExamplePostalCode = examplePostalCode;
            ExampleFormattedAddress = exampleFormattedAddress;
        }

        public CountryAlpha2Code CountryCode { get; }

        public AddressProfileSource Source { get; }

        public IReadOnlyList<AddressFieldProfile> Fields { get; }

        public string? ExamplePostalCode { get; }

        public string? ExampleFormattedAddress { get; }

        internal AddressProfile ForCountry(CountryAlpha2Code countryCode)
        {
            return new AddressProfile(
                countryCode,
                Fields,
                Source,
                ExamplePostalCode,
                ExampleFormattedAddress);
        }
    }
}
