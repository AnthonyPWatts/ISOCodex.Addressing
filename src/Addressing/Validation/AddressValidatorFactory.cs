using System;
using System.Collections.Concurrent;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Validation
{
    public class AddressValidatorFactory : IAddressValidatorFactory
    {
        private readonly ConcurrentDictionary<CountryAlpha2Code, IAddressValidator> _validators =
            new ConcurrentDictionary<CountryAlpha2Code, IAddressValidator>();
        private IAddressValidator? _fallbackValidator;

        public void RegisterValidator(CountryAlpha2Code countryCode, IAddressValidator validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            _validators[countryCode] = validator;
        }

        public void RegisterFallbackValidator(IAddressValidator validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            _fallbackValidator = validator;
        }

        public IAddressValidator GetValidator(CountryAlpha2Code countryCode)
        {
            if (_validators.TryGetValue(countryCode, out var validator))
            {
                return validator;
            }

            if (_fallbackValidator != null && CountryRegistry.TryGetByAlpha2(countryCode, out _))
            {
                return _fallbackValidator;
            }

            throw new InvalidOperationException(
                $"No address validator registered for country code '{countryCode.Value}'.");
        }
    }
}
