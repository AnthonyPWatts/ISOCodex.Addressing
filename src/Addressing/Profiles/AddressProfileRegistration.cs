using System;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Profiles
{
    internal sealed class AddressProfileRegistration
    {
        public AddressProfileRegistration(
            CountryAlpha2Code country,
            Func<AddressProfile> createProfile)
        {
            Country = country;
            CreateProfile = createProfile;
        }

        public CountryAlpha2Code Country { get; }

        public Func<AddressProfile> CreateProfile { get; }
    }
}
