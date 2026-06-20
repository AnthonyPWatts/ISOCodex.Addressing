using ISOCodex.Countries;

namespace ISOCodex.Addressing.Profiles
{
    public interface IAddressProfileProvider
    {
        AddressProfile GetProfile(CountryAlpha2Code countryCode);
    }
}
