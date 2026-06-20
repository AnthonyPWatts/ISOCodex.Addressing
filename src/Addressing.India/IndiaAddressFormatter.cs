using System.Collections.Generic;
using ISOCodex.Addressing.Formatting;

namespace ISOCodex.Addressing.India
{
    public sealed class IndiaAddressFormatter : ICountryAddressFormatter
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
                AddressFormatting.JoinParts(" ", address.City, address.PostalCode.Code));
            AddressFormatting.AddIfNotWhiteSpace(lines, address.StateOrProvince);

            return AddressFormatting.FormatLines(lines, AddressFormatting.GetCountryLine(address.CountryCode), options);
        }
    }
}
