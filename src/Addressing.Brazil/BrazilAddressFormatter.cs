using System.Collections.Generic;
using ISOCodex.Addressing.Formatting;

namespace ISOCodex.Addressing.Brazil
{
    public sealed class BrazilAddressFormatter : ICountryAddressFormatter
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
                AddressFormatting.JoinParts(" - ", address.City, address.StateOrProvince));
            AddressFormatting.AddIfNotWhiteSpace(lines, address.PostalCode.Code);

            return AddressFormatting.FormatLines(lines, AddressFormatting.GetCountryLine(address.CountryCode), options);
        }
    }
}
