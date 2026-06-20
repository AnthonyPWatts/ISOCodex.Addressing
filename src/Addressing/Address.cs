using System;
using ISOCodex.Countries;

namespace ISOCodex.Addressing
{
    public sealed class Address
    {
        public string Line1 { get; }
        public string? Line2 { get; }
        public string City { get; }
        public string? StateOrProvince { get; }
        public PostalCode PostalCode { get; }
        public CountryAlpha2Code CountryCode { get; }

        public Address(
            string line1,
            string? line2,
            string city,
            string? stateOrProvince,
            PostalCode postalCode,
            CountryAlpha2Code countryCode)
        {
            if (string.IsNullOrWhiteSpace(line1))
            {
                throw new ArgumentException("Line1 cannot be null or empty.", nameof(line1));
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("City cannot be null or empty.", nameof(city));
            }

            Line1 = line1;
            Line2 = line2;
            City = city;
            StateOrProvince = stateOrProvince;
            PostalCode = postalCode;
            CountryCode = countryCode;
        }
    }
}
