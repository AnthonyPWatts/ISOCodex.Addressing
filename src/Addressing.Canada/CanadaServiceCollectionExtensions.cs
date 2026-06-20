using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Canada
{
    public static class CanadaServiceCollectionExtensions
    {
        private static readonly AddressFieldOption[] CanadaAdministrativeAreas =
        {
            Option("AB", "Alberta"),
            Option("BC", "British Columbia"),
            Option("MB", "Manitoba"),
            Option("NB", "New Brunswick"),
            Option("NL", "Newfoundland and Labrador"),
            Option("NS", "Nova Scotia"),
            Option("NT", "Northwest Territories"),
            Option("NU", "Nunavut"),
            Option("ON", "Ontario"),
            Option("PE", "Prince Edward Island"),
            Option("QC", "Quebec"),
            Option("SK", "Saskatchewan"),
            Option("YT", "Yukon")
        };

        public static IServiceCollection AddCanadaAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("CA"),
                () => new CAAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("CA"),
                () => new CAAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("CA"),
                CreateCanadaAddressProfile);

            return services;
        }

        private static AddressProfile CreateCanadaAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("CA"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Street address", true, 10, "111 Wellington Street"),
                    Field(AddressField.AddressLine2, "Apartment, suite, unit, building", false, 20),
                    Field(AddressField.Locality, "City or town", true, 30, "Ottawa"),
                    Field(
                        AddressField.AdministrativeArea,
                        "Province or territory",
                        true,
                        40,
                        "ON",
                        AddressFieldInputKind.Select,
                        CanadaAdministrativeAreas),
                    Field(AddressField.PostalCode, "Postal code", true, 50, "K1A 0A6"),
                    Field(AddressField.Country, "Country", true, 60, "Canada")
                },
                examplePostalCode: "K1A 0A6",
                exampleFormattedAddress: "111 Wellington Street\nOttawa ON K1A 0A6\nCanada");
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
