using System;
using ISOCodex.Countries;
using System.Linq;
using ISOCodex.Addressing.Canada;
using ISOCodex.Addressing.GreatBritain;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Spain;
using ISOCodex.Addressing.UnitedStates;
using Microsoft.Extensions.DependencyInjection;

namespace ISOCodex.Addressing.Tests;

public class AddressProfileProviderTests
{
    [Fact]
    public void GetProfile_WithRegisteredGbProfile_ReturnsCountrySpecificProfile()
    {
        var provider = new DefaultAddressProfileProvider();
        provider.RegisterProfile(
            CountryAlpha2Code.Parse("GB"),
            new AddressProfile(
                CountryAlpha2Code.Parse("GB"),
                new[]
                {
                    new AddressFieldProfile(
                        AddressField.AddressLine1,
                        "Address line 1",
                        true,
                        10)
                }));

        var profile = provider.GetProfile(CountryAlpha2Code.Parse("GB"));

        Assert.Equal(CountryAlpha2Code.Parse("GB"), profile.CountryCode);
        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
    }

    [Fact]
    public void AddAddressing_WithGb_RegistersProfileWithRequiredCoreFields()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("GB"));

        var profile = provider.GetProfile(CountryAlpha2Code.Parse("GB"));

        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        AssertRequired(profile, AddressField.AddressLine1);
        AssertRequired(profile, AddressField.Locality);
        AssertRequired(profile, AddressField.PostalCode);
        AssertRequired(profile, AddressField.Country);
    }

    [Fact]
    public void AddAddressing_WithGb_LabelsPostalCodeAsPostcode()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("GB"));

        var postalCode = provider
            .GetProfile(CountryAlpha2Code.Parse("GB"))
            .Fields
            .Single(field => field.Field == AddressField.PostalCode);

        Assert.Equal("Postcode", postalCode.Label);
    }

    [Fact]
    public void AddAddressing_WithUs_UsesUsFieldLabels()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("US"));
        var profile = provider.GetProfile(CountryAlpha2Code.Parse("US"));

        Assert.Equal(
            "State",
            profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea).Label);
        Assert.Equal(
            "ZIP code",
            profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
    }

    [Fact]
    public void AddAddressing_WithUs_ProvidesStateOptions()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("US"));

        var administrativeArea = GetAdministrativeArea(provider.GetProfile(CountryAlpha2Code.Parse("US")));

        Assert.Equal(AddressFieldInputKind.Select, administrativeArea.InputKind);
        Assert.Contains(
            administrativeArea.Options,
            option => option.Value == "DC" && option.Label == "District of Columbia");
        Assert.Contains(
            administrativeArea.Options,
            option => option.Value == "CA" && option.Label == "California");
    }

    [Fact]
    public void AddAddressing_WithCa_UsesCanadaFieldLabels()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("CA"));
        var profile = provider.GetProfile(CountryAlpha2Code.Parse("CA"));

        Assert.Equal(
            "Province or territory",
            profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea).Label);
        Assert.Equal(
            "Postal code",
            profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
    }

    [Fact]
    public void AddAddressing_WithCa_ProvidesProvinceAndTerritoryOptions()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("CA"));

        var administrativeArea = GetAdministrativeArea(provider.GetProfile(CountryAlpha2Code.Parse("CA")));

        Assert.Equal(AddressFieldInputKind.Select, administrativeArea.InputKind);
        Assert.Equal(13, administrativeArea.Options.Count);
        Assert.Contains(
            administrativeArea.Options,
            option => option.Value == "ON" && option.Label == "Ontario");
        Assert.Contains(
            administrativeArea.Options,
            option => option.Value == "NU" && option.Label == "Nunavut");
    }

    [Fact]
    public void AddAddressing_WithGb_DoesNotProvideCountyOptions()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("GB"));

        var administrativeArea = GetAdministrativeArea(provider.GetProfile(CountryAlpha2Code.Parse("GB")));

        Assert.Equal("County", administrativeArea.Label);
        Assert.False(administrativeArea.IsRequired);
        Assert.Equal(AddressFieldInputKind.Text, administrativeArea.InputKind);
        Assert.Empty(administrativeArea.Options);
    }

    [Fact]
    public void GetProfile_WithFallbackProfile_ReturnsGenericFallbackForRequestedCountry()
    {
        var services = new ServiceCollection();
        services.AddAddressing();
        services.AddGenericAddressingFallbacks();

        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<IAddressProfileProvider>();

        var profile = provider.GetProfile(CountryAlpha2Code.Parse("FR"));

        Assert.Equal(CountryAlpha2Code.Parse("FR"), profile.CountryCode);
        Assert.Equal(AddressProfileSource.GenericFallback, profile.Source);
        AssertRequired(profile, AddressField.AddressLine1);
        AssertRequired(profile, AddressField.Locality);
        AssertRequired(profile, AddressField.PostalCode);
        AssertRequired(profile, AddressField.Country);
    }

    [Theory]
    [InlineData("EU")]
    [InlineData("UK")]
    public void GetProfile_WithFallbackProfile_DoesNotUseFallbackForNonDeliverableCountryCodes(string countryCode)
    {
        var services = new ServiceCollection();
        services.AddAddressing();
        services.AddGenericAddressingFallbacks();

        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<IAddressProfileProvider>();

        var ex = Assert.Throws<InvalidOperationException>(
            () => provider.GetProfile(CountryAlpha2Code.Parse(countryCode)));

        Assert.Contains(countryCode, ex.Message);
    }

    [Fact]
    public void GetProfile_WhenNoProfileRegistered_ThrowsInvalidOperationException()
    {
        var provider = new DefaultAddressProfileProvider();

        var ex = Assert.Throws<InvalidOperationException>(
            () => provider.GetProfile(CountryAlpha2Code.Parse("US")));

        Assert.Contains("US", ex.Message);
    }

    [Fact]
    public void GetProfile_ReturnsFieldsInStableDisplayOrder()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("GB"));
        var displayOrders = provider
            .GetProfile(CountryAlpha2Code.Parse("GB"))
            .Fields
            .Select(field => field.DisplayOrder)
            .ToArray();

        Assert.Equal(displayOrders.OrderBy(order => order), displayOrders);
        Assert.All(displayOrders, order => Assert.True(order > 0));
    }

    [Fact]
    public void AddressProfile_CopiesFieldCollection()
    {
        var fields = new[]
        {
            new AddressFieldProfile(
                AddressField.AddressLine1,
                "Address line 1",
                true,
                10)
        };

        var profile = new AddressProfile(CountryAlpha2Code.Parse("GB"), fields);

        fields[0] = new AddressFieldProfile(
            AddressField.Country,
            "Country",
            true,
            20);

        Assert.Equal(AddressField.AddressLine1, profile.Fields.Single().Field);
        Assert.False(profile.Fields is AddressFieldProfile[]);
    }

    [Fact]
    public void AddAddressing_RegistersAddressProfileProvider()
    {
        var provider = BuildProfileProvider(CountryAlpha2Code.Parse("GB"));

        Assert.Equal(CountryAlpha2Code.Parse("GB"), provider.GetProfile(CountryAlpha2Code.Parse("GB")).CountryCode);
    }

    [Fact]
    public void AddSpainAddressing_RegistersSpanishProfile()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddSpainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<IAddressProfileProvider>();

        var profile = provider.GetProfile(CountryAlpha2Code.Parse("ES"));

        Assert.Equal(AddressProfileSource.CountrySpecific, profile.Source);
        Assert.Equal(
            "Código postal",
            profile.Fields.Single(field => field.Field == AddressField.PostalCode).Label);
    }

    [Fact]
    public void AddSpainAddressing_ProvidesProvinceOptions()
    {
        var services = new ServiceCollection();

        services.AddAddressing();
        services.AddSpainAddressing();

        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<IAddressProfileProvider>();

        var administrativeArea = GetAdministrativeArea(provider.GetProfile(CountryAlpha2Code.Parse("ES")));

        Assert.Equal(AddressFieldInputKind.Select, administrativeArea.InputKind);
        Assert.Contains(
            administrativeArea.Options,
            option => option.Value == "Madrid" && option.Label == "Madrid");
        Assert.Contains(
            administrativeArea.Options,
            option => option.Value == "Barcelona" && option.Label == "Barcelona");
    }

    [Fact]
    public void AddressFieldProfile_CopiesOptionCollection()
    {
        var options = new[]
        {
            new AddressFieldOption("ON", "Ontario")
        };

        var field = new AddressFieldProfile(
            AddressField.AdministrativeArea,
            "Province or territory",
            true,
            10,
            options: options);

        options[0] = new AddressFieldOption("QC", "Quebec");

        Assert.Equal("ON", field.Options.Single().Value);
        Assert.False(field.Options is AddressFieldOption[]);
    }

    private static IAddressProfileProvider BuildProfileProvider(params CountryAlpha2Code[] countries)
    {
        var services = new ServiceCollection();
        services.AddAddressing();

        foreach (var country in countries)
        {
            if (country == CountryAlpha2Code.Parse("GB"))
            {
                services.AddGreatBritainAddressing();
            }
            else if (country == CountryAlpha2Code.Parse("US"))
            {
                services.AddUnitedStatesAddressing();
            }
            else if (country == CountryAlpha2Code.Parse("CA"))
            {
                services.AddCanadaAddressing();
            }
            else
            {
                throw new ArgumentException($"No test country pack registration for {country.Value}.", nameof(countries));
            }
        }

        using var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<IAddressProfileProvider>();
    }

    private static void AssertRequired(AddressProfile profile, AddressField field)
    {
        Assert.Contains(
            profile.Fields,
            fieldProfile => fieldProfile.Field == field && fieldProfile.IsRequired);
    }

    private static AddressFieldProfile GetAdministrativeArea(AddressProfile profile)
    {
        return profile.Fields.Single(field => field.Field == AddressField.AdministrativeArea);
    }
}
