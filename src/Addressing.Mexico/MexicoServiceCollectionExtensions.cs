using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Mexico
{
    public static class MexicoServiceCollectionExtensions
    {
        public static IServiceCollection AddMexicoAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("MX"),
                () => new MexicoAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("MX"),
                () => new MexicoAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("MX"),
                CreateMexicoAddressProfile);

            return services;
        }

        private static AddressProfile CreateMexicoAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("MX"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10, "Palacio Nacional"),
                    Field(AddressField.AddressLine2, "Address line 2 / colonia / interior", false, 20),
                    Field(AddressField.Locality, "City / Locality", true, 30, "Ciudad de México"),
                    Field(
                        AddressField.AdministrativeArea,
                        "State",
                        true,
                        40,
                        "CMX",
                        AddressFieldInputKind.Select,
                        MexicoAdministrativeAreaData.Options),
                    Field(AddressField.PostalCode, "Postal code", true, 50, "06066"),
                    Field(AddressField.Country, "Country", true, 60, "Mexico")
                },
                examplePostalCode: "06066",
                exampleFormattedAddress: "Palacio Nacional\n06066 Ciudad de México, CMX\nMexico");
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
    }
}
