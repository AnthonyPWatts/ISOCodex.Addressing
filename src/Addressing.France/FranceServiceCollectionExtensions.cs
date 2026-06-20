using ISOCodex.Addressing.Profiles;
using ISOCodex.Countries;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.France
{
    public static class FranceServiceCollectionExtensions
    {
        public static IServiceCollection AddFranceAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("FR"),
                () => new FranceAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("FR"),
                () => new FranceAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("FR"),
                CreateFranceAddressProfile);

            return services;
        }

        private static AddressProfile CreateFranceAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("FR"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10, "10 Rue de Rivoli"),
                    Field(AddressField.AddressLine2, "Address line 2", false, 20),
                    Field(AddressField.PostalCode, "Postal code", true, 30, "75001"),
                    Field(AddressField.Locality, "City / Commune", true, 40, "Paris"),
                    Field(AddressField.AdministrativeArea, "Region / Department", false, 50),
                    Field(AddressField.Country, "Country", true, 60, "France")
                },
                examplePostalCode: "75001",
                exampleFormattedAddress: "10 Rue de Rivoli\n75001 Paris\nFrance");
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
    }
}
