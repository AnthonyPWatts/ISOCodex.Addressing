using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.Canada
{
    public class CAAddressValidator : IAddressValidator
    {
        private static readonly Regex PostalCodeRegex = new Regex(
            @"^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z] \d[ABCEGHJ-NPRSTV-Z]\d$",
            RegexOptions.Compiled);

        private static readonly HashSet<string> ValidProvinces =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AB", "BC", "MB", "NB", "NL", "NS", "NT", "NU", "ON", "PE", "QC", "SK", "YT"
            };

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();
            AddressValidationIssues.AddCommonIssues(issues, address, CountryCode.CA, "CA");

            if (address == null)
            {
                return new AddressValidationResult(issues);
            }

            if (!AddressValidationIssues.TryGetRequiredPostalCode(issues, address, out var postalCode))
            {
                return new AddressValidationResult(issues);
            }

            var normalizedPostalCode = NormalizePostalCode(postalCode);

            if (!PostalCodeRegex.IsMatch(normalizedPostalCode))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.Invalid",
                    "PostalCode must be a valid CA postal code (e.g., A1A 1A1).",
                    nameof(Address.PostalCode)));
            }

            if (!string.IsNullOrWhiteSpace(address.StateOrProvince)
                && !ValidProvinces.Contains(address.StateOrProvince.Trim()))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Invalid",
                    $"StateOrProvince '{address.StateOrProvince}' is not a valid CA province or territory.",
                    nameof(Address.StateOrProvince)));
            }

            return new AddressValidationResult(issues);
        }

        private static string NormalizePostalCode(string postalCode)
        {
            var compactPostalCode = postalCode
                .Replace(" ", string.Empty)
                .ToUpperInvariant();

            if (compactPostalCode.Length != 6)
            {
                return compactPostalCode;
            }

            return compactPostalCode.Insert(3, " ");
        }
    }
}
