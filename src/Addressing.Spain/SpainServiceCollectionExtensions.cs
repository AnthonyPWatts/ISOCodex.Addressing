using ISOCodex.Addressing.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Spain
{
    public static class SpainServiceCollectionExtensions
    {
        public static IServiceCollection AddSpainAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryCode.ES,
                () => new SpanishAddressValidator());

            services.AddAddressFormatter(
                CountryCode.ES,
                () => new SpanishAddressFormatter());

            services.AddAddressProfile(
                CountryCode.ES,
                CreateSpanishAddressProfile);

            return services;
        }

        private static AddressProfile CreateSpanishAddressProfile()
        {
            return new AddressProfile(
                CountryCode.ES,
                new[]
                {
                    Field(AddressField.AddressLine1, "Calle y número", true, 10, "Calle Mayor 10"),
                    Field(AddressField.AddressLine2, "Piso, puerta, escalera", false, 20, "3º C"),
                    Field(AddressField.Locality, "Localidad", true, 30, "Madrid"),
                    Field(
                        AddressField.AdministrativeArea,
                        "Provincia",
                        true,
                        40,
                        "Madrid",
                        AddressFieldInputKind.Select,
                        SpanishProvinceData.AdministrativeAreaOptions),
                    Field(AddressField.PostalCode, "Código postal", true, 50, "28013"),
                    Field(AddressField.Country, "País", true, 60, "España")
                },
                examplePostalCode: "28013",
                exampleFormattedAddress: "Calle Mayor 10\n28013 Madrid\nMadrid\nEspaña");
        }

        private static AddressFieldProfile Field(
            AddressField field,
            string label,
            bool isRequired,
            int displayOrder,
            string? placeholder = null)
        {
            return new AddressFieldProfile(
                field,
                label,
                isRequired,
                displayOrder,
                placeholder);
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
