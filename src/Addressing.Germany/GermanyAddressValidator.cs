using System.Collections.Generic;
using ISOCodex.Countries;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.Germany
{
    public class GermanyAddressValidator : IAddressValidator
    {
        private static readonly Regex PostalCodeRegex =
            new Regex(@"^\d{5}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();
            AddressValidationIssues.AddCommonIssues(issues, address, CountryAlpha2Code.Parse("DE"), "German");

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
                    "PostalCode must be a 5-digit number for German addresses.",
                    nameof(Address.PostalCode)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
