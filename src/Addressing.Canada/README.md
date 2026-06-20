# ISOCodex.Addressing.Canada

Canada-specific extension package for `ISOCodex.Addressing`.

## Installation

```bash
dotnet add package ISOCodex.Addressing.Canada
```

## Registration

```csharp
services.AddAddressing();
services.AddCanadaAddressing();
```

## What it provides

- `CA` validator registration.
- Canada address formatter.
- Canada address profile metadata for forms.
- Canadian postal-code structure validation with internal case/spacing normalisation.
- Province and territory validation when supplied.

Validation is structural. It does not call external services and does not prove that an address exists or is deliverable. It does not cross-check city, province, and postal-code combinations.

## Example

`csharp
using ISOCodex.Countries;

var address = new Address(
    "111 Wellington Street",
    null,
    "Ottawa",
    "ON",
    new PostalCode("K1A 0A6"),
    CountryAlpha2Code.Parse("CA"));
```

Formatted output:

```text
111 Wellington Street
Ottawa ON K1A 0A6
Canada
```
