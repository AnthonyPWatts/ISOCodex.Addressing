using ISOCodex.Countries;

namespace ISOCodex.Addressing.Validation
{
    public interface IAddressValidatorFactory
    {
        IAddressValidator GetValidator(CountryAlpha2Code countryCode);

        void RegisterValidator(CountryAlpha2Code countryCode, IAddressValidator validator);
    }
}
