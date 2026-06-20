using System.Collections.Generic;
using System.Linq;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Formatting
{
    public static class AddressFormatting
    {
        public static void AddIfNotWhiteSpace(ICollection<string> lines, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                lines.Add(value.Trim());
            }
        }

        public static string FormatLines(
            ICollection<string> lines,
            string countryName,
            AddressFormatOptions? options)
        {
            var effectiveOptions = options ?? new AddressFormatOptions();
            var formattedLines = lines.ToList();

            if (effectiveOptions.IncludeCountry)
            {
                AddIfNotWhiteSpace(formattedLines, countryName);
            }

            if (effectiveOptions.Style == AddressFormatStyle.SingleLine)
            {
                return string.Join(effectiveOptions.SingleLineSeparator, formattedLines);
            }

            return string.Join("\n", formattedLines);
        }

        public static string JoinParts(string separator, params string?[] parts)
        {
            return string.Join(
                separator,
                parts
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(part => part!.Trim()));
        }

        public static string GetCountryLine(CountryAlpha2Code countryCode)
        {
            return CountryNameRegistry.TryGetEnglishShortName(countryCode, out var countryName)
                ? countryName!
                : countryCode.Value;
        }
    }
}
