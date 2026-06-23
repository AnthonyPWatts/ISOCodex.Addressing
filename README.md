# ISOCodex.Addressing

`ISOCodex.Addressing` is a .NET library for modelling, formatting, and validating postal addresses against country-specific rules.

## Project status

The current stable release is `2.0.0`. The package family has been exercised through unit tests, package validation, and consumer-style test rigs for checkout APIs, dynamic forms, and bulk imports.

The established country packages remain demand-led: new countries should be added when there is a real consuming use case, sample data, or user need to justify maintaining the rules.

## Projects

- `src/Addressing` - core types, DI registration, registries, generic fallback behaviours, and abstractions
- `src/Addressing.GreatBritain` - Great Britain country package
- `src/Addressing.UnitedStates` - United States country package
- `src/Addressing.Canada` - Canada country package
- `src/Addressing.Spain` - Spain country package
- `src/Addressing.Ireland` - Ireland country package
- `src/Addressing.France` - France country package
- `src/Addressing.India` - India country package
- `src/Addressing.Brazil` - Brazil country package
- `src/Addressing.Mexico` - Mexico country package
- `src/Addressing.Germany` - Germany country package
- `src/Addressing.Italy` - Italy country package
- `tests/Addressing.Tests` - unit and integration-style tests
- `ManualTestRig` - small console app for quick manual smoke testing

## Package identity

- Core package: `ISOCodex.Addressing`
- Great Britain country package: `ISOCodex.Addressing.GreatBritain`
- United States country package: `ISOCodex.Addressing.UnitedStates`
- Canada country package: `ISOCodex.Addressing.Canada`
- Spain country package: `ISOCodex.Addressing.Spain`
- Ireland country package: `ISOCodex.Addressing.Ireland`
- France country package: `ISOCodex.Addressing.France`
- India country package: `ISOCodex.Addressing.India`
- Brazil country package: `ISOCodex.Addressing.Brazil`
- Mexico country package: `ISOCodex.Addressing.Mexico`
- Germany country package: `ISOCodex.Addressing.Germany`
- Italy country package: `ISOCodex.Addressing.Italy`
- Root namespaces: `ISOCodex.Addressing*`

## Installation

```bash
dotnet add package ISOCodex.Addressing
dotnet add package ISOCodex.Addressing.GreatBritain
dotnet add package ISOCodex.Addressing.UnitedStates
dotnet add package ISOCodex.Addressing.Canada
dotnet add package ISOCodex.Addressing.Spain
dotnet add package ISOCodex.Addressing.Ireland
dotnet add package ISOCodex.Addressing.France
dotnet add package ISOCodex.Addressing.India
dotnet add package ISOCodex.Addressing.Brazil
dotnet add package ISOCodex.Addressing.Mexico
dotnet add package ISOCodex.Addressing.Germany
dotnet add package ISOCodex.Addressing.Italy
```

## Quick start

```csharp
using ISOCodex.Addressing;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.GreatBritain;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using ISOCodex.Countries;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services
    .AddAddressing()
    .AddGreatBritainAddressing();

using var serviceProvider = services.BuildServiceProvider();

var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();

var address = new Address(
    line1: "10 Downing Street",
    line2: null,
    city: "London",
    stateOrProvince: null,
    postalCode: new PostalCode("SW1A 2AA"),
    countryCode: CountryAlpha2Code.Parse("GB"));

var validationResult = validatorFactory
    .GetValidator(address.CountryCode)
    .Validate(address);

var formatted = formatter.Format(address);
var profile = profileProvider.GetProfile(address.CountryCode);
```

`formatted` contains a postal-friendly, country-specific layout:

```text
10 Downing Street
London
SW1A 2AA
United Kingdom
```

## Address formatting

Formatting is routed by `Address.CountryCode`. Register the countries your app supports, then ask `IAddressFormatter` to produce display or output text for each address.

Country package formatters handle:

- country-specific line ordering
- optional second address lines
- postal code placement
- English country display names
- multi-line and single-line output

For compact UI, logs, CSV exports, or search results, request a single-line format:

```csharp
var singleLine = formatter.Format(
    address,
    new AddressFormatOptions
    {
        Style = AddressFormatStyle.SingleLine
    });
```

Output:

```text
10 Downing Street, London, SW1A 2AA, United Kingdom
```

If the country is already obvious from surrounding UI, you can omit it:

```csharp
var withoutCountry = formatter.Format(
    address,
    new AddressFormatOptions
    {
        IncludeCountry = false
    });
```

Formatting is presentation only. It does not validate the address, normalize the stored postal code, or prove the address exists. Use the validator for country-specific validation before formatting when correctness matters.

## Address profiles / form metadata

Address profiles expose country-specific metadata that applications can use to build address entry experiences. They describe which conceptual address fields are relevant, required, labelled, ordered, and hinted for a country.

Profiles are metadata only. They do not render UI, validate an address, format an address, autocomplete addresses, geocode, or prove deliverability. They are framework-agnostic, so the same data can be used from ASP.NET, Blazor, React, console tools, APIs, imports, or custom validation pipelines.

For countries with well-defined administrative subdivisions in the package, the administrative-area field can include selectable `Options`. Current country-pack metadata includes options for US states and territories, Canadian provinces and territories, Spanish provinces, Indian states and union territories, Brazilian UF codes, Mexican states, and Italian provinces. GB counties intentionally remain a free-text optional field because county usage is not strict enough to model as a closed validation list.

```csharp
using ISOCodex.Addressing.Profiles;
using ISOCodex.Countries;

var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
var profile = profileProvider.GetProfile(CountryAlpha2Code.Parse("GB"));

foreach (var field in profile.Fields.OrderBy(field => field.DisplayOrder))
{
    Console.WriteLine($"{field.Label}: {(field.IsRequired ? "required" : "optional")}");
}
```

Country packages contribute profiles when their DI extension methods are called, for example `AddGreatBritainAddressing()`, `AddUnitedStatesAddressing()`, `AddCanadaAddressing()`, `AddSpainAddressing()`, `AddIrelandAddressing()`, `AddFranceAddressing()`, `AddIndiaAddressing()`, `AddBrazilAddressing()`, `AddMexicoAddressing()`, `AddGermanyAddressing()`, or `AddItalyAddressing()`.

`AddGenericAddressingFallbacks()` also registers a conservative generic profile for unsupported ISO countries. The returned profile has `Source = AddressProfileSource.GenericFallback`, while country-pack-backed profiles use `AddressProfileSource.CountrySpecific`.

Applications can expose profile metadata to frontends as JSON if desired:

```json
{
  "countryCode": "GB",
  "source": "CountrySpecific",
  "fields": [
    {
      "field": "AddressLine1",
      "label": "Address line 1",
      "isRequired": true,
      "displayOrder": 10,
      "placeholder": "10 Downing Street"
    },
    {
      "field": "PostalCode",
      "label": "Postcode",
      "isRequired": true,
      "displayOrder": 60,
      "placeholder": "SW1A 2AA"
    }
  ]
}
```

For a US profile, the administrative-area field includes select-style options:

```json
{
  "field": "AdministrativeArea",
  "label": "State",
  "inputKind": "Select",
  "options": [
    { "value": "CA", "label": "California" },
    { "value": "DC", "label": "District of Columbia" }
  ]
}
```

## Structured validation results

`Validate(...)` returns structured, form/API-friendly errors instead of throwing for ordinary validation failures:

```csharp
var result = validatorFactory
    .GetValidator(address.CountryCode)
    .Validate(address);

if (!result.IsValid)
{
    foreach (var issue in result.Issues)
    {
        Console.WriteLine($"{issue.PropertyName}: {issue.Message}");
    }
}
```

Each issue includes a stable `Code`, a human-readable `Message`, and an optional `PropertyName`.

`AddressValidationIssue.Code` is intended for programmatic handling and should be treated as the stable machine-readable contract. `Message` is intended for display/logging and may be refined for clarity in future minor or patch releases.

### Using validation results with FluentValidation

`ISOCodex.Addressing` does not depend on FluentValidation. The core package returns structured validation results so applications can adapt address validation into FluentValidation, ASP.NET ModelState, Blazor forms, imports, or their own validation pipeline.

For example, a consuming application can call the address validator from a FluentValidation rule and map each `AddressValidationIssue` to a FluentValidation failure:

```csharp
using FluentValidation;
using ISOCodex.Addressing;
using ISOCodex.Addressing.Validation;

public sealed class Customer
{
    public Address Address { get; init; } = default!;
}

public sealed class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator(IAddressValidatorFactory addressValidatorFactory)
    {
        RuleFor(customer => customer.Address)
            .Custom((address, context) =>
            {
                if (address is null)
                {
                    context.AddFailure("Address", "Address is required.");
                    return;
                }

                var validator = addressValidatorFactory.GetValidator(address.CountryCode);
                var result = validator.Validate(address);

                foreach (var issue in result.Issues)
                {
                    var propertyName = string.IsNullOrWhiteSpace(issue.PropertyName)
                        ? "Address"
                        : $"Address.{issue.PropertyName}";

                    context.AddFailure(propertyName, issue.Message);
                }
            });
    }
}
```

If unsupported countries are possible in your application, register generic fallbacks or handle the explicit `GetValidator(...)` failure in your application validation layer.

Framework-neutral adapters can use the same issue data:

```csharp
using System.Collections.Generic;
using System.Linq;
using ISOCodex.Addressing.Validation;

public static IReadOnlyDictionary<string, string[]> ToErrorDictionary(
    AddressValidationResult result)
{
    return result.Issues
        .GroupBy(issue => issue.PropertyName ?? string.Empty)
        .ToDictionary(
            group => group.Key,
            group => group.Select(issue => issue.Message).ToArray());
}
```

This keeps the core package framework-agnostic. An optional adapter package may be considered later if there is enough demand, but consumers do not need one in order to use the current validators from FluentValidation.

## Unsupported countries and fallback behaviour

`Address.CountryCode` is a `CountryAlpha2Code` from `ISOCodex.Countries`. Construct it from canonical alpha-2 input, or resolve alpha-3, numeric, alias, or display-name input through `ISOCodex.Countries` at your application boundary before constructing an `Address`.

By default, unsupported countries remain explicit: `GetValidator(...)` and `Format(...)` throw when no country-specific service is registered. This helps applications catch missing country packs when strict validation is expected.

For applications that need to save and display structured addresses for countries without a country pack, register generic fallbacks:

```csharp
var services = new ServiceCollection();

services
    .AddAddressing()
    .AddGreatBritainAddressing();
services.AddGenericAddressingFallbacks();
```

With these fallbacks:

- registered country packs are still used first
- unregistered current countries known by `ISOCodex.Countries` use `PermissiveAddressValidator`
- unregistered current countries known by `ISOCodex.Countries` use `GenericAddressFormatter`
- unregistered current countries known by `ISOCodex.Countries` use a generic `AddressProfile`
- special or non-country code elements such as `EU` do not use postal-address fallbacks
- alias-like values such as `UK` are not silently resolved to `GB`
- validation does not prove the address is deliverable

The fallback validator accepts any non-null `Address` instance. It is intended for store-first or validate-later workflows where the consuming application still wants a structured address object.

The fallback formatter emits the available structured fields in a generic order and uses the Countries English short name as the country line:

```text
1 Rue de Rivoli
Paris 75001
France
```

Fallbacks do not make the `Address` model fully freeform. `Address` still requires `Line1`, `City`, `PostalCode`, and `CountryCode`. If an application needs to store addresses that cannot fit that structure, it should keep a separate raw/freeform field in its own persistence model.

The core `Address` model is intentionally a structured postal-address abstraction rather than a fully freeform global address record. It works best where an address can reasonably be represented using line, locality, postal-code and country components. Applications that must preserve arbitrary user-entered or legacy addresses should store the raw original text separately.

## Recommended persistence shape

For relational storage, persist the value objects as strings and keep constraints aligned with the `Address` model rather than with one country's postal rules.

| Column | Suggested type | Required | Notes |
| --- | --- | --- | --- |
| `Line1` | `nvarchar(200)` | Yes | First delivery/address line. |
| `Line2` | `nvarchar(200)` | No | Apartment, suite, building, organization, or other secondary line. |
| `City` | `nvarchar(100)` | Yes | Locality/town/city value used by the current `Address` model. |
| `StateOrProvince` | `nvarchar(100)` | No | Region, province, state, county, department, prefecture, or equivalent. |
| `PostalCode` | `nvarchar(32)` | Yes | Store the user's value; validators may normalize for checking without mutating this value. |
| `CountryCode` | `char(2)` | Yes | ISO 3166-1 alpha-2 code, stored uppercase. |

Recommended constraints:

- require `Line1`, `City`, `PostalCode`, and `CountryCode`
- allow `Line2` and `StateOrProvince` to be null
- constrain `CountryCode` to exactly two uppercase ASCII letters and validate/canonicalise values through `ISOCodex.Countries`
- avoid country-specific postal-code constraints in the database
- use Unicode string columns for human-entered address fields

Example SQL shape:

```sql
Line1 nvarchar(200) not null,
Line2 nvarchar(200) null,
City nvarchar(100) not null,
StateOrProvince nvarchar(100) null,
PostalCode nvarchar(32) not null,
CountryCode char(2) not null
```

These lengths are practical defaults, not package-enforced limits. Applications with legacy imports, unusually long organization names, or strict partner schemas can choose wider columns without changing how the library works.

## Recommended validation state storage

Saving an address and validating an address are separate concerns. Consumers that need imports, review queues, fallback handling, or background revalidation should store validation state alongside the address instead of treating every saved row as verified.

Suggested optional columns:

| Column | Suggested type | Notes |
| --- | --- | --- |
| `ValidationStatus` | `nvarchar(32)` | `NotValidated`, `Valid`, `Invalid`, or `AcceptedUnverified`. |
| `ValidationProfile` | `nvarchar(100)` | The rules used, such as `ISOCodex.Addressing.GB`, `ISOCodex.Addressing.Spain`, or `GenericFallback`. |
| `ValidatedAt` | `datetimeoffset` | When validation last ran. |
| `ValidationIssuesJson` | `nvarchar(max)` | Serialized `AddressValidationIssue` values when validation fails. |

Suggested status meanings:

- `NotValidated` - the address has been saved, but no validation result is recorded
- `Valid` - the address passed the recorded validation profile
- `Invalid` - validation ran and returned one or more issues
- `AcceptedUnverified` - the address was accepted without country-specific proof, commonly through a generic fallback

Example validation issue payload:

```json
[
  {
    "code": "Address.PostalCode.Invalid",
    "propertyName": "PostalCode",
    "message": "PostalCode must be a valid GB postcode (e.g., SW1A 1AA)."
  }
]
```

`ValidationProfile` should be specific enough for the application to understand what the result means later. If validation freshness matters, include a package or rules version in that value, for example `ISOCodex.Addressing.GB@1.0.0`.

This metadata is application state, so it is not part of the `Address` value object. Store it with the owning entity or address record when your workflow needs to distinguish saved, validated, failed, and accepted-unverified addresses.

## JSON serialization guidance

`Address` is a domain model that contains value objects. If you serialize it directly with `System.Text.Json`, `PostalCode` and `CountryCode` are represented by their object properties:

```json
{
  "line1": "10 Downing Street",
  "line2": null,
  "city": "London",
  "stateOrProvince": null,
  "postalCode": { "code": "SW1A 2AA" },
  "countryCode": { "value": "GB" }
}
```

For public APIs or storage contracts that should expose scalar strings, map to an application DTO with `postalCode` and `countryCode` string properties. Use `CountryAlpha2Code.Parse(...)`, `CountryAlpha2Code.TryParse(...)`, or richer `ISOCodex.Countries` registry lookup at the boundary.

## Country packages

- Great Britain (`GB`) via `ISOCodex.Addressing.GreatBritain`
- United States (`US`) via `ISOCodex.Addressing.UnitedStates`
- Canada (`CA`) via `ISOCodex.Addressing.Canada`
- Spain (`ES`) via `ISOCodex.Addressing.Spain`
- Ireland (`IE`) via `ISOCodex.Addressing.Ireland`
- France (`FR`) via `ISOCodex.Addressing.France`
- India (`IN`) via `ISOCodex.Addressing.India`
- Brazil (`BR`) via `ISOCodex.Addressing.Brazil`
- Mexico (`MX`) via `ISOCodex.Addressing.Mexico`
- Germany (`DE`) via `ISOCodex.Addressing.Germany`
- Italy (`IT`) via `ISOCodex.Addressing.Italy`

Each country package transitively depends on the core `ISOCodex.Addressing` package and registers country-specific validation, formatting, and profile metadata through its DI extension method.

```bash
dotnet add package ISOCodex.Addressing.GreatBritain
dotnet add package ISOCodex.Addressing.UnitedStates
dotnet add package ISOCodex.Addressing.Canada
dotnet add package ISOCodex.Addressing.Spain
dotnet add package ISOCodex.Addressing.Ireland
dotnet add package ISOCodex.Addressing.France
dotnet add package ISOCodex.Addressing.India
dotnet add package ISOCodex.Addressing.Brazil
dotnet add package ISOCodex.Addressing.Mexico
dotnet add package ISOCodex.Addressing.Germany
dotnet add package ISOCodex.Addressing.Italy
```

| Country | Extension method | Postal-code scope | Administrative area |
| --- | --- | --- | --- |
| India | `AddIndiaAddressing()` | Six-digit PIN code | Required state or union territory |
| Brazil | `AddBrazilAddressing()` | CEP with or without hyphen | Required UF |
| Mexico | `AddMexicoAddressing()` | Five-digit postal code | Required state |
| Germany | `AddGermanyAddressing()` | Five-digit postcode | Not required |
| Italy | `AddItalyAddressing()` | Five-digit CAP | Required province |

For example:

```csharp
using ISOCodex.Addressing.Brazil;
using ISOCodex.Addressing.Germany;

services.AddAddressing();
services.AddBrazilAddressing();
services.AddGermanyAddressing();
```

### Spain

```csharp
using ISOCodex.Addressing;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Spain;
using ISOCodex.Addressing.Validation;
using ISOCodex.Countries;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddAddressing();
services.AddGreatBritainAddressing();
services.AddSpainAddressing();

using var serviceProvider = services.BuildServiceProvider();

var validatorFactory = serviceProvider.GetRequiredService<IAddressValidatorFactory>();
var formatter = serviceProvider.GetRequiredService<IAddressFormatter>();
var profileProvider = serviceProvider.GetRequiredService<IAddressProfileProvider>();
```

### Ireland

```csharp
using ISOCodex.Addressing;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.Ireland;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using ISOCodex.Countries;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddAddressing();
services.AddIrelandAddressing();

using var serviceProvider = services.BuildServiceProvider();

var address = new Address(
    line1: "1 College Green",
    line2: null,
    city: "Dublin",
    stateOrProvince: null,
    postalCode: new PostalCode("D02 X285"),
    countryCode: CountryAlpha2Code.Parse("IE"));
```

Default formatted Ireland output:

```text
1 College Green
Dublin
D02 X285
Ireland
```

The Ireland validator accepts pragmatic Eircode shapes such as `D02 X285`, `D02X285`, and lowercase equivalents without mutating the stored postal code.

### France

```csharp
using ISOCodex.Addressing;
using ISOCodex.Addressing.Formatting;
using ISOCodex.Addressing.France;
using ISOCodex.Addressing.Profiles;
using ISOCodex.Addressing.Validation;
using ISOCodex.Countries;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddAddressing();
services.AddFranceAddressing();

using var serviceProvider = services.BuildServiceProvider();

var address = new Address(
    line1: "10 Rue de Rivoli",
    line2: null,
    city: "Paris",
    stateOrProvince: null,
    postalCode: new PostalCode("75001"),
    countryCode: CountryAlpha2Code.Parse("FR"));
```

Default formatted France output:

```text
10 Rue de Rivoli
75001 Paris
France
```

The France validator currently applies conservative five-digit postal-code validation and does not attempt overseas territory, CEDEX, special-case, or city/postal-code cross-checking.

## Release focus

Package identity, namespaces, NuGet metadata, and package documentation should stay aligned under the `ISOCodex.Addressing` name.

## Compatibility policy

From `1.0.0`, public types, method signatures, value-object behaviour, and validation issue codes are treated as compatibility-sensitive.

Version `2.0.0` made the breaking move from Addressing-owned country identity to `ISOCodex.Countries`.

Patch and minor releases may add new countries, metadata, helper APIs, validation cases, and documentation. They may also correct country-specific formatting or validation behaviour where the existing behaviour is demonstrably wrong.

Validation issue `Code` values are intended for programmatic handling and should remain stable unless a major version change is made. Human-readable validation messages may be refined for clarity in minor or patch releases.

## License

MIT
