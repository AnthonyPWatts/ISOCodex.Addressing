using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.Brazil
{
    public class BrazilAddressValidator : IAddressValidator
    {
        private static readonly Regex CepRegex =
            new Regex(@"^\d{5}-?\d{3}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();
            AddressValidationIssues.AddCommonIssues(issues, address, CountryCode.BR, "Brazilian");

            if (address == null)
            {
                return new AddressValidationResult(issues);
            }

            if (!AddressValidationIssues.TryGetRequiredPostalCode(issues, address, out var cep))
            {
                return new AddressValidationResult(issues);
            }

            if (!CepRegex.IsMatch(cep))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.Invalid",
                    "PostalCode must be a valid Brazilian CEP (e.g., 01001-000 or 01001000).",
                    nameof(Address.PostalCode)));
            }

            if (string.IsNullOrWhiteSpace(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Required",
                    "StateOrProvince cannot be null or empty for Brazilian addresses.",
                    nameof(Address.StateOrProvince)));
            }
            else if (!BrazilAdministrativeAreaData.IsValid(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Invalid",
                    $"StateOrProvince '{address.StateOrProvince}' is not a valid Brazilian state code.",
                    nameof(Address.StateOrProvince)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
