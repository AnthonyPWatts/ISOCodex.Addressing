using System.Collections.Generic;
using ISOCodex.Addressing.Formatting;

namespace ISOCodex.Addressing.Ireland
{
    public sealed class IrelandAddressFormatter : ICountryAddressFormatter
    {
        public string Format(Address address, AddressFormatOptions? options = null)
        {
            var lines = new List<string>
            {
                address.Line1
            };

            AddressFormatting.AddIfNotWhiteSpace(lines, address.Line2);
            AddressFormatting.AddIfNotWhiteSpace(lines, address.City);
            AddressFormatting.AddIfNotWhiteSpace(lines, address.PostalCode.Code);

            return AddressFormatting.FormatLines(lines, "Ireland", options);
        }
    }
}
