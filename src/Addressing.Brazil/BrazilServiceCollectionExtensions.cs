using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Brazil
{
    public static class BrazilServiceCollectionExtensions
    {
        public static IServiceCollection AddBrazilAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("BR"),
                () => new BrazilAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("BR"),
                () => new BrazilAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("BR"),
                CreateBrazilAddressProfile);

            return services;
        }

        private static AddressProfile CreateBrazilAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("BR"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10, "Praça da Sé"),
                    Field(AddressField.AddressLine2, "Address line 2 / complement", false, 20),
                    Field(AddressField.Locality, "City / Municipality", true, 30, "São Paulo"),
                    Field(
                        AddressField.AdministrativeArea,
                        "State (UF)",
                        true,
                        40,
                        "SP",
                        AddressFieldInputKind.Select,
                        BrazilAdministrativeAreaData.Options),
                    Field(AddressField.PostalCode, "CEP", true, 50, "01001-000"),
                    Field(AddressField.Country, "Country", true, 60, "Brazil")
                },
                examplePostalCode: "01001-000",
                exampleFormattedAddress: "Praça da Sé\nSão Paulo - SP\n01001-000\nBrazil");
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
