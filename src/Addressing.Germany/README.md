# ISOCodex.Addressing.Germany

Germany-specific extension package for `ISOCodex.Addressing`.

## Installation

```bash
dotnet add package ISOCodex.Addressing.Germany
```

## Registration

```csharp
services.AddAddressing();
services.AddGermanyAddressing();
```

## What it provides

- `CountryCode.DE` validator registration.
- Germany address formatter.
- Germany address profile metadata for forms.
- Five-digit postcode validation.

Validation is structural. It does not call external services and does not prove that an address exists or is deliverable.

## Example

```csharp
var address = new Address(
    "Pariser Platz 1",
    null,
    "Berlin",
    null,
    new PostalCode("10117"),
    CountryCode.DE);
```

Formatted output:

```text
Pariser Platz 1
10117 Berlin
Germany
```
