using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.France
{
    public class FranceAddressValidator : IAddressValidator
    {
        private static readonly Regex PostalCodeRegex =
            new Regex(@"^\d{5}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();

            AddressValidationIssues.AddCommonIssues(issues, address, CountryCode.FR, "French");

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
                    "PostalCode must be a 5-digit number for French addresses.",
                    nameof(Address.PostalCode)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
