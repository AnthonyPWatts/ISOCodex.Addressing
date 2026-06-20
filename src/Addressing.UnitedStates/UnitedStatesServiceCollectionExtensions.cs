using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.UnitedStates
{
    public static class UnitedStatesServiceCollectionExtensions
    {
        private static readonly AddressFieldOption[] UnitedStatesAdministrativeAreas =
        {
            Option("AL", "Alabama"), Option("AK", "Alaska"), Option("AZ", "Arizona"),
            Option("AR", "Arkansas"), Option("CA", "California"), Option("CO", "Colorado"),
            Option("CT", "Connecticut"), Option("DE", "Delaware"), Option("FL", "Florida"),
            Option("GA", "Georgia"), Option("HI", "Hawaii"), Option("ID", "Idaho"),
            Option("IL", "Illinois"), Option("IN", "Indiana"), Option("IA", "Iowa"),
            Option("KS", "Kansas"), Option("KY", "Kentucky"), Option("LA", "Louisiana"),
            Option("ME", "Maine"), Option("MD", "Maryland"), Option("MA", "Massachusetts"),
            Option("MI", "Michigan"), Option("MN", "Minnesota"), Option("MS", "Mississippi"),
            Option("MO", "Missouri"), Option("MT", "Montana"), Option("NE", "Nebraska"),
            Option("NV", "Nevada"), Option("NH", "New Hampshire"), Option("NJ", "New Jersey"),
            Option("NM", "New Mexico"), Option("NY", "New York"), Option("NC", "North Carolina"),
            Option("ND", "North Dakota"), Option("OH", "Ohio"), Option("OK", "Oklahoma"),
            Option("OR", "Oregon"), Option("PA", "Pennsylvania"), Option("RI", "Rhode Island"),
            Option("SC", "South Carolina"), Option("SD", "South Dakota"), Option("TN", "Tennessee"),
            Option("TX", "Texas"), Option("UT", "Utah"), Option("VT", "Vermont"),
            Option("VA", "Virginia"), Option("WA", "Washington"), Option("WV", "West Virginia"),
            Option("WI", "Wisconsin"), Option("WY", "Wyoming"), Option("DC", "District of Columbia"),
            Option("AS", "American Samoa"), Option("FM", "Federated States of Micronesia"),
            Option("GU", "Guam"), Option("MH", "Marshall Islands"),
            Option("MP", "Northern Mariana Islands"), Option("PR", "Puerto Rico"),
            Option("PW", "Palau"), Option("VI", "U.S. Virgin Islands"),
            Option("AA", "Armed Forces Americas"), Option("AE", "Armed Forces Europe"),
            Option("AP", "Armed Forces Pacific")
        };

        public static IServiceCollection AddUnitedStatesAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("US"),
                () => new USAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("US"),
                () => new USAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("US"),
                CreateUnitedStatesAddressProfile);

            return services;
        }

        private static AddressProfile CreateUnitedStatesAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("US"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Street address", true, 10, "1600 Pennsylvania Avenue NW"),
                    Field(AddressField.AddressLine2, "Apartment, suite, unit, building", false, 20, "Apt 4B"),
                    Field(AddressField.Locality, "City", true, 30, "Washington"),
                    Field(
                        AddressField.AdministrativeArea,
                        "State",
                        true,
                        40,
                        "DC",
                        AddressFieldInputKind.Select,
                        UnitedStatesAdministrativeAreas),
                    Field(AddressField.PostalCode, "ZIP code", true, 50, "20500"),
                    Field(AddressField.Country, "Country", true, 60, "United States")
                },
                examplePostalCode: "20500",
                exampleFormattedAddress: "1600 Pennsylvania Avenue NW\nWashington, DC 20500\nUnited States");
        }

        private static AddressFieldProfile Field(
            AddressField field,
            string label,
            bool isRequired,
            int displayOrder,
            string? placeholder = null)
        {
            return new AddressFieldProfile(field, label, isRequired, displayOrder, placeholder);
        }

        private static AddressFieldProfile Field(
            AddressField field,
            string label,
            bool isRequired,
            int displayOrder,
            string? placeholder,
            AddressFieldInputKind inputKind,
            AddressFieldOption[] options)
        {
            return new AddressFieldProfile(
                field,
                label,
                isRequired,
                displayOrder,
                placeholder,
                inputKind: inputKind,
                options: options);
        }

        private static AddressFieldOption Option(string value, string label)
        {
            return new AddressFieldOption(value, label);
        }
    }
}
