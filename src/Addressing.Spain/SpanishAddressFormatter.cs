using System.Collections.Generic;
using ISOCodex.Addressing.Formatting;

namespace ISOCodex.Addressing.Spain
{
    public sealed class SpanishAddressFormatter : ICountryAddressFormatter
    {
        public string Format(Address address, AddressFormatOptions? options = null)
        {
            var lines = new List<string>
            {
                address.Line1
            };

            AddressFormatting.AddIfNotWhiteSpace(lines, address.Line2);
            AddressFormatting.AddIfNotWhiteSpace(
                lines,
                AddressFormatting.JoinParts(" ", address.PostalCode.Code, address.City));

            return AddressFormatting.FormatLines(lines, AddressFormatting.GetCountryLine(address.CountryCode), options);
        }
    }
}
