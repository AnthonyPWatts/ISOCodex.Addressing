using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Italy
{
    public static class ItalyServiceCollectionExtensions
    {
        public static IServiceCollection AddItalyAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryCode.IT,
                () => new ItalyAddressValidator());

            services.AddAddressFormatter(
                CountryCode.IT,
                () => new ItalyAddressFormatter());

            services.AddAddressProfile(
                CountryCode.IT,
                CreateItalyAddressProfile);

            return services;
        }

        private static AddressProfile CreateItalyAddressProfile()
        {
            return new AddressProfile(
                CountryCode.IT,
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10, "Piazza del Colosseo 1"),
                    Field(AddressField.AddressLine2, "Address line 2", false, 20),
                    Field(AddressField.Locality, "City / Comune", true, 30, "Roma"),
                    Field(
                        AddressField.AdministrativeArea,
                        "Province",
                        true,
                        40,
                        "RM",
                        AddressFieldInputKind.Select,
                        ItalyProvinceData.Options),
                    Field(AddressField.PostalCode, "CAP", true, 50, "00184"),
                    Field(AddressField.Country, "Country", true, 60, "Italy")
                },
                examplePostalCode: "00184",
                exampleFormattedAddress: "Piazza del Colosseo 1\n00184 Roma RM\nItaly");
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
