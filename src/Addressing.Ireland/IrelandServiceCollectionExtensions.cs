using ISOCodex.Addressing.Profiles;
using ISOCodex.Countries;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Ireland
{
    public static class IrelandServiceCollectionExtensions
    {
        public static IServiceCollection AddIrelandAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("IE"),
                () => new IrelandAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("IE"),
                () => new IrelandAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("IE"),
                CreateIrelandAddressProfile);

            return services;
        }

        private static AddressProfile CreateIrelandAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("IE"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10, "1 College Green"),
                    Field(AddressField.AddressLine2, "Address line 2", false, 20),
                    Field(AddressField.Locality, "Town / City", true, 30, "Dublin"),
                    Field(AddressField.AdministrativeArea, "County", false, 40, "Dublin"),
                    Field(AddressField.PostalCode, "Eircode", true, 50, "D02 X285"),
                    Field(AddressField.Country, "Country", true, 60, "Ireland")
                },
                examplePostalCode: "D02 X285",
                exampleFormattedAddress: "1 College Green\nDublin\nD02 X285\nIreland");
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
