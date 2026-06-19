# ISOCodex.Addressing.Mexico

Mexico-specific extension package for `ISOCodex.Addressing`.

## Installation

```bash
dotnet add package ISOCodex.Addressing.Mexico
```

## Registration

```csharp
services.AddAddressing();
services.AddMexicoAddressing();
```

## What it provides

- `CountryCode.MX` validator registration.
- Mexico address formatter.
- Mexico address profile metadata for forms.
- Five-digit postal-code validation, including leading-zero codes.
- State validation against package metadata.

Validation is structural. It does not call external services and does not prove that an address exists or is deliverable.

## Example

```csharp
var address = new Address(
    "Palacio Nacional",
    null,
    "Ciudad de México",
    "CMX",
    new PostalCode("06066"),
    CountryCode.MX);
```

Formatted output:

```text
Palacio Nacional
06066 Ciudad de México, CMX
Mexico
```
