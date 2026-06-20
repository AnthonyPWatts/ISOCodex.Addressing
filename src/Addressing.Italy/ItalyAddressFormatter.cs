using System.Collections.Generic;
using ISOCodex.Addressing.Formatting;

namespace ISOCodex.Addressing.Italy
{
    public sealed class ItalyAddressFormatter : ICountryAddressFormatter
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
                AddressFormatting.JoinParts(" ", address.PostalCode.Code, address.City, address.StateOrProvince));

            return AddressFormatting.FormatLines(lines, "Italy", options);
        }
    }
}
