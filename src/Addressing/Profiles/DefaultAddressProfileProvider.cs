using System;
using System.Collections.Concurrent;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Profiles
{
    public sealed class DefaultAddressProfileProvider : IAddressProfileProvider
    {
        private readonly ConcurrentDictionary<CountryAlpha2Code, AddressProfile> _profiles =
            new ConcurrentDictionary<CountryAlpha2Code, AddressProfile>();
        private AddressProfile? _fallbackProfile;

        public void RegisterProfile(CountryAlpha2Code countryCode, AddressProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            _profiles[countryCode] = profile.ForCountry(countryCode);
        }

        public void RegisterFallbackProfile(AddressProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            _fallbackProfile = profile;
        }

        public AddressProfile GetProfile(CountryAlpha2Code countryCode)
        {
            if (_profiles.TryGetValue(countryCode, out var profile))
            {
                return profile;
            }

            if (_fallbackProfile != null && CountryRegistry.TryGetByAlpha2(countryCode, out _))
            {
                return _fallbackProfile.ForCountry(countryCode);
            }

            throw new InvalidOperationException(
                $"No address profile registered for country code '{countryCode.Value}'.");
        }
    }
}
