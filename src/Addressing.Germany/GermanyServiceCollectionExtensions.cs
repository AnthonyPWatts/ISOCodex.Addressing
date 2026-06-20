using ISOCodex.Addressing.Formatting;
using ISOCodex.Countries;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Germany
{
    public static class GermanyServiceCollectionExtensions
    {
        public static IServiceCollection AddGermanyAddressing(this IServiceCollection services)
        {
            services.AddAddressValidator(
                CountryAlpha2Code.Parse("DE"),
                () => new GermanyAddressValidator());

            services.AddAddressFormatter(
                CountryAlpha2Code.Parse("DE"),
                () => new GermanyAddressFormatter());

            services.AddAddressProfile(
                CountryAlpha2Code.Parse("DE"),
                CreateGermanyAddressProfile);

            return services;
        }

        private static AddressProfile CreateGermanyAddressProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("DE"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Street address", true, 10, "Pariser Platz 1"),
                    Field(AddressField.AddressLine2, "Additional address line", false, 20),
                    Field(AddressField.PostalCode, "Postcode", true, 30, "10117"),
                    Field(AddressField.Locality, "Town / City", true, 40, "Berlin"),
                    Field(AddressField.Country, "Country", true, 50, "Germany")
                },
                examplePostalCode: "10117",
                exampleFormattedAddress: "Pariser Platz 1\n10117 Berlin\nGermany");
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
    }
}
