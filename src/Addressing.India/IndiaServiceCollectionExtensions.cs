using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.India
{
    public static class IndiaServiceCollectionExtensions
    {
        public static IServiceCollection AddIndiaAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryCode.IN,
                () => new IndiaAddressValidator());

            services.AddAddressFormatter(
                CountryCode.IN,
                () => new IndiaAddressFormatter());

            services.AddAddressProfile(
                CountryCode.IN,
                CreateIndiaAddressProfile);

            return services;
        }

        private static AddressProfile CreateIndiaAddressProfile()
        {
            return new AddressProfile(
                CountryCode.IN,
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10, "Rashtrapati Bhavan"),
                    Field(AddressField.AddressLine2, "Address line 2", false, 20),
                    Field(AddressField.Locality, "City / Town", true, 30, "New Delhi"),
                    Field(
                        AddressField.AdministrativeArea,
                        "State / Union Territory",
                        true,
                        40,
                        "DL",
                        AddressFieldInputKind.Select,
                        IndiaAdministrativeAreaData.Options),
                    Field(AddressField.PostalCode, "PIN code", true, 50, "110004"),
                    Field(AddressField.Country, "Country", true, 60, "India")
                },
                examplePostalCode: "110004",
                exampleFormattedAddress: "Rashtrapati Bhavan\nNew Delhi 110004\nDL\nIndia");
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
