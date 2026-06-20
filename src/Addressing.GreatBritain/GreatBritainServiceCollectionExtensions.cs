using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.GreatBritain
{
    public static class GreatBritainServiceCollectionExtensions
    {
        public static IServiceCollection AddGreatBritainAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("GB"),
                () => new GBAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("GB"),
                () => new GBAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("GB"),
                CreateGreatBritainAddressProfile);

            return services;
        }

        private static AddressProfile CreateGreatBritainAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("GB"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10, "10 Downing Street"),
                    Field(AddressField.AddressLine2, "Address line 2", false, 20),
                    Field(AddressField.AddressLine3, "Address line 3", false, 30),
                    Field(AddressField.Locality, "Town or city", true, 40, "London"),
                    Field(AddressField.AdministrativeArea, "County", false, 50),
                    Field(AddressField.PostalCode, "Postcode", true, 60, "SW1A 2AA"),
                    Field(AddressField.Country, "Country", true, 70, "United Kingdom")
                },
                examplePostalCode: "SW1A 2AA",
                exampleFormattedAddress: "10 Downing Street\nLondon\nSW1A 2AA\nUnited Kingdom");
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
