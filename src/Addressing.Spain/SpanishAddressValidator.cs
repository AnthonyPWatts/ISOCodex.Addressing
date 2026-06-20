using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.Spain
{
    public class SpanishAddressValidator : IAddressValidator
    {
        private static readonly Regex PostalCodeRegex =
            new Regex(@"^\d{5}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();

            AddressValidationIssues.AddCommonIssues(issues, address, CountryCode.ES, "Spanish");

            if (address == null)
            {
                return new AddressValidationResult(issues);
            }

            if (!AddressValidationIssues.TryGetRequiredPostalCode(issues, address, out var postalCode))
            {
                return new AddressValidationResult(issues);
            }

            if (!PostalCodeRegex.IsMatch(postalCode))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.Invalid",
                    "PostalCode must be a 5-digit number for Spanish addresses.",
                    nameof(Address.PostalCode)));
            }

            if (!string.IsNullOrWhiteSpace(address.StateOrProvince) &&
                !SpanishProvinceData.IsValidProvince(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Invalid",
                    $"StateOrProvince '{address.StateOrProvince}' is not a valid Spanish province.",
                    nameof(Address.StateOrProvince)));
            }

            if (!PostalCodeRegex.IsMatch(postalCode))
            {
                return new AddressValidationResult(issues);
            }

            if (!SpanishProvinceData.TryGetProvinceForPostalCode(postalCode, out var expectedProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.ProvinceUnknown",
                    $"PostalCode '{postalCode}' is not valid for any known Spanish province.",
                    nameof(Address.PostalCode)));
            }

            if (!string.IsNullOrWhiteSpace(address.StateOrProvince) &&
                !string.IsNullOrWhiteSpace(expectedProvince) &&
                !SpanishProvinceData.MatchesProvince(address.StateOrProvince, expectedProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.ProvinceMismatch",
                    $"PostalCode '{postalCode}' does not match the provided StateOrProvince '{address.StateOrProvince}'.",
                    nameof(Address.PostalCode)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
