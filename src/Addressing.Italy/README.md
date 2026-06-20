# ISOCodex.Addressing.Italy

Italy-specific extension package for `ISOCodex.Addressing`.

## Installation

```bash
dotnet add package ISOCodex.Addressing.Italy
```

## Registration

```csharp
services.AddAddressing();
services.AddItalyAddressing();
```

## What it provides

- `CountryCode.IT` validator registration.
- Italy address formatter.
- Italy address profile metadata for forms.
- Five-digit CAP validation.
- Province validation against package metadata.

Validation is structural. It does not call external services and does not prove that an address exists or is deliverable.

## Example

```csharp
var address = new Address(
    "Piazza del Colosseo 1",
    null,
    "Roma",
    "RM",
    new PostalCode("00184"),
    CountryCode.IT);
```

Formatted output:

```text
Piazza del Colosseo 1
00184 Roma RM
Italy
```
