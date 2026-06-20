using System;
using ISOCodex.Countries;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISOCodex.Addressing.Validation;

namespace ISOCodex.Addressing.UnitedStates
{
    public class USAddressValidator : IAddressValidator
    {
        private static readonly Regex ZipCodeRegex =
            new Regex(@"^\d{5}(-\d{4})?$", RegexOptions.Compiled);

        private static readonly HashSet<string> ValidStates =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AL","AK","AZ","AR","CA","CO","CT","DE","FL","GA","HI","ID","IL","IN","IA","KS",
                "KY","LA","ME","MD","MA","MI","MN","MS","MO","MT","NE","NV","NH","NJ","NM","NY",
                "NC","ND","OH","OK","OR","PA","RI","SC","SD","TN","TX","UT","VT","VA","WA","WV",
                "WI","WY","DC","AS","FM","GU","MH","MP","PR","PW","VI","AA","AE","AP"
            };

        public AddressValidationResult Validate(Address? address)
        {
            var issues = new List<AddressValidationIssue>();
            AddressValidationIssues.AddCommonIssues(issues, address, CountryAlpha2Code.Parse("US"), "US");

            if (address == null)
            {
                return new AddressValidationResult(issues);
            }

            if (!AddressValidationIssues.TryGetRequiredPostalCode(issues, address, out var zipCode))
            {
                return new AddressValidationResult(issues);
            }

            if (!ZipCodeRegex.IsMatch(zipCode))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.PostalCode.Invalid",
                    "PostalCode must be a valid US ZIP code (e.g., 12345 or 12345-6789).",
                    nameof(Address.PostalCode)));
            }

            if (string.IsNullOrWhiteSpace(address.StateOrProvince))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Required",
                    "StateOrProvince cannot be null or empty for US addresses.",
                    nameof(Address.StateOrProvince)));
            }
            else if (!ValidStates.Contains(address.StateOrProvince.Trim()))
            {
                issues.Add(new AddressValidationIssue(
                    "Address.StateOrProvince.Invalid",
                    $"StateOrProvince '{address.StateOrProvince}' is not a valid US state or territory.",
                    nameof(Address.StateOrProvince)));
            }

            return new AddressValidationResult(issues);
        }
    }
}
