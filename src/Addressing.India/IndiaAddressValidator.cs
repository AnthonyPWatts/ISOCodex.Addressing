using System.Collections.Generic;
using ISOCodex.Countries;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.India
{
    public class IndiaAddressValidator : IAddressValidator
    {
        private static readonly Regex PinCodeRegex =
            new Regex(@"^\d{6}$", RegexOptions.Compiled);

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();
            AddressValidationIssues.AddCommonIssues(issues, address, CountryAlpha2Code.Parse("IN"), "Indian");

            if (address == null)
            {
                return new AddressValidationResult(issues);
            }

            if (!AddressValidationIssues.TryGetRequiredPostalCode(issues, address, out var pinCode))
            {
                return new AddressValidationResult(issues);
            }

            if (!PinCodeRegex.IsMatch(pinCode))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.Invalid",
                    "PostalCode must be a 6-digit PIN code for Indian addresses.",
                    nameof(Address.PostalCode)));
            }

            if (string.IsNullOrWhiteSpace(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Required",
                    "StateOrProvince cannot be null or empty for Indian addresses.",
                    nameof(Address.StateOrProvince)));
            }
            else if (!IndiaAdministrativeAreaData.IsValid(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Invalid",
                    $"StateOrProvince '{address.StateOrProvince}' is not a valid Indian state or union territory.",
                    nameof(Address.StateOrProvince)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
