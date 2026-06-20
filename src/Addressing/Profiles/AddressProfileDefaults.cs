using ISOCodex.Countries;

namespace ISOCodex.Addressing.Profiles
{
    internal static class AddressProfileDefaults
    {
        public static AddressProfile CreateGenericFallbackProfile()
        {
            return new AddressProfile(
                CountryAlpha2Code.Parse("GB"),
                new[]
                {
                    Field(AddressField.AddressLine1, "Address line 1", true, 10),
                    Field(AddressField.AddressLine2, "Address line 2", false, 20),
                    Field(AddressField.AddressLine3, "Address line 3", false, 30),
                    Field(AddressField.Locality, "Town / city / locality", false, 40),
                    Field(AddressField.AdministrativeArea, "State / province / region", false, 50),
                    Field(AddressField.PostalCode, "Postal code", false, 60),
                    Field(AddressField.Country, "Country", true, 70)
                },
                AddressProfileSource.GenericFallback);
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
