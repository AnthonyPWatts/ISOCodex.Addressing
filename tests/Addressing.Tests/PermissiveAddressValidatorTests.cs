using ISOCodex.Addressing.Validation.Validators;
using ISOCodex.Countries;

namespace ISOCodex.Addressing.Tests;

public class PermissiveAddressValidatorTests
{
    [Fact]
    public void Validate_WithAddress_ReturnsSuccess()
    {
        var validator = new PermissiveAddressValidator();
        var address = new Address(
            "1 Rue de Rivoli",
            null,
            "Paris",
            null,
            new PostalCode("75001"),
            CountryAlpha2Code.Parse("FR"));

        var result = validator.Validate(address);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithNullAddress_ReturnsRequiredIssue()
    {
        var validator = new PermissiveAddressValidator();

        var result = validator.Validate(null);

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, issue => issue.Code == "Address.Required");
    }
}
