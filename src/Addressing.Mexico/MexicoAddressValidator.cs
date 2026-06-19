using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.Mexico
{
    public class MexicoAddressValidator : IAddressValidator
    {
        private static readonly Regex PostalCodeRegex =
            new Regex(@"^\d{5}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();
            AddressValidationIssues.AddCommonIssues(issues, address, CountryCode.MX, "Mexican");

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
                    "PostalCode must be a 5-digit number for Mexican addresses.",
                    nameof(Address.PostalCode)));
            }

            if (string.IsNullOrWhiteSpace(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Required",
                    "StateOrProvince cannot be null or empty for Mexican addresses.",
                    nameof(Address.StateOrProvince)));
            }
            else if (!MexicoAdministrativeAreaData.IsValid(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Invalid",
                    $"StateOrProvince '{address.StateOrProvince}' is not a valid Mexican state.",
                    nameof(Address.StateOrProvince)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
