using System.Collections.Generic;
using ISOCodex.Countries;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.Ireland
{
    public class IrelandAddressValidator : IAddressValidator
    {
        private static readonly Regex EircodeRegex =
            new Regex(@"^[A-NP-Z0-9]{3}\s?[A-NP-Z0-9]{4}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();

            AddressValidationIssues.AddCommonIssues(issues, address, CountryAlpha2Code.Parse("IE"), "Irish");

            if (address == null)
            {
                return new AddressValidationResult(issues);
            }

            if (!AddressValidationIssues.TryGetRequiredPostalCode(issues, address, out var postalCode))
            {
                return new AddressValidationResult(issues);
            }

            if (!EircodeRegex.IsMatch(postalCode.ToUpperInvariant()))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.Invalid",
                    "PostalCode must be a valid Irish Eircode (e.g., D02 X285).",
                    nameof(Address.PostalCode)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
