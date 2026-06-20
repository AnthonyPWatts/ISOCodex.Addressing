using System;
using System.Collections.Concurrent;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Formatting
{
    public sealed class AddressFormatter : IAddressFormatter
    {
        private readonly ConcurrentDictionary<CountryAlpha2Code, ICountryAddressFormatter> _formatters =
            new ConcurrentDictionary<CountryAlpha2Code, ICountryAddressFormatter>();
        private ICountryAddressFormatter? _fallbackFormatter;

        public void RegisterFormatter(CountryAlpha2Code countryCode, ICountryAddressFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            _formatters[countryCode] = formatter;
        }

        public void RegisterFallbackFormatter(ICountryAddressFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            _fallbackFormatter = formatter;
        }

        public string Format(Address address, AddressFormatOptions? options = null)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!_formatters.TryGetValue(address.CountryCode, out var formatter))
            {
                if (_fallbackFormatter == null || !CountryRegistry.TryGetByAlpha2(address.CountryCode, out _))
                {
                    throw new InvalidOperationException(
                        $"No address formatter registered for country code '{address.CountryCode.Value}'.");
                }

                formatter = _fallbackFormatter;
            }

            return formatter.Format(address, options);
        }
    }
}
