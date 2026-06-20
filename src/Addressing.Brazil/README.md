# ISOCodex.Addressing.Brazil

Brazil-specific extension package for `ISOCodex.Addressing`.

## Installation

```bash
dotnet add package ISOCodex.Addressing.Brazil
```

## Registration

```csharp
services.AddAddressing();
services.AddBrazilAddressing();
```

## What it provides

- `CountryCode.BR` validator registration.
- Brazil address formatter.
- Brazil address profile metadata for forms.
- CEP shape validation with or without a hyphen.
- State UF validation against package metadata.

Validation is structural. It does not call external services and does not prove that an address exists or is deliverable.

## Example

```csharp
var address = new Address(
    "Praça da Sé",
    null,
    "São Paulo",
    "SP",
    new PostalCode("01001-000"),
    CountryCode.BR);
```

Formatted output:

```text
Praça da Sé
São Paulo - SP
01001-000
Brazil
```
