using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.Italy
{
    public class ItalyAddressValidator : IAddressValidator
    {
        private static readonly Regex CapRegex =
            new Regex(@"^\d{5}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();
            AddressValidationIssues.AddCommonIssues(issues, address, CountryCode.IT, "Italian");

            if (address == null)
            {
                return new AddressValidationResult(issues);
            }

            if (!AddressValidationIssues.TryGetRequiredPostalCode(issues, address, out var cap))
            {
                return new AddressValidationResult(issues);
            }

            if (!CapRegex.IsMatch(cap))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.Invalid",
                    "PostalCode must be a 5-digit CAP for Italian addresses.",
                    nameof(Address.PostalCode)));
            }

            if (string.IsNullOrWhiteSpace(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Required",
                    "StateOrProvince cannot be null or empty for Italian addresses.",
                    nameof(Address.StateOrProvince)));
            }
            else if (!ItalyProvinceData.IsValid(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Invalid",
                    $"StateOrProvince '{address.StateOrProvince}' is not a valid Italian province.",
                    nameof(Address.StateOrProvince)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
