# ISOCodex.Addressing.UnitedStates

United States-specific extension package for `ISOCodex.Addressing`.

## Installation

```bash
dotnet add package ISOCodex.Addressing.UnitedStates
```

## Registration

```csharp
services.AddAddressing();
services.AddUnitedStatesAddressing();
```

## What it provides

- `CountryCode.US` validator registration.
- United States address formatter.
- United States address profile metadata for forms.
- ZIP and ZIP+4 validation.
- USPS state, territory, possession, and military-code validation.

Validation is structural. It does not call external services and does not prove that an address exists or is deliverable. It does not cross-check city, state, and ZIP combinations.

## Example

```csharp
var address = new Address(
    "1600 Pennsylvania Avenue NW",
    null,
    "Washington",
    "DC",
    new PostalCode("20500"),
    CountryCode.US);
```

Formatted output:

```text
1600 Pennsylvania Avenue NW
Washington, DC 20500
United States
```
