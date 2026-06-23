# Extended test rig findings

## Summary

- The core APIs are small and easy to compose once the correct country packages are registered.
- Realistic app edges need local mapping helpers for DTO-to-`Address`, validation issue shaping, field-name mapping, and supported-country lists.
- Address construction rejects missing required fields before validators can produce normal structured validation results.

## API friction

- The original multi-country registration split between core built-ins and extension packages has been removed. Core now supports zero countries, and all country-specific behaviour is registered by country packs.
- There is no registered-country catalogue from DI, so the API and import tool keep their own supported-country arrays.
- DTO-to-`Address` mapping needs repeated trimming/null handling because the model stores `PostalCode` and `CountryCode` value objects.
- `Address` throws for blank `Line1`, blank `City`, and blank `PostalCode`, so apps need pre-validation before calling country validators.
- The README mentions persistence-friendly validation shapes, but consumers still need to build their own error DTOs.

## Country-pack consistency

- Extension method naming is consistent: `AddGreatBritainAddressing`, `AddUnitedStatesAddressing`, `AddCanadaAddressing`, `AddSpainAddressing`, `AddFranceAddressing`, `AddIrelandAddressing`.
- Namespace imports are predictable, but consumers must remember the country package namespace as well as the core namespace.
- Spain provides closed administrative-area options; France and Ireland use free text. That difference is useful but needs UI handling.

## Validation behaviour

- Validation issues include `Code`, `Message`, and optional `PropertyName`, which is enough for basic API/UI mapping.
- Missing required fields can be caught by the model constructor before validators run, so issue codes for those are app-authored in the POCs.
- Generic fallback validation is permissive. The import tool treats successful fallback rows as `AcceptedUnverified` to avoid presenting them as country-validated.

## Formatting behaviour

- Multi-line and single-line formatting are easy to call through `IAddressFormatter`.
- Consumers need to know `AddressFormatOptions.Style` and `SingleLineSeparator`; there is no one-call helper such as `FormatSingleLine`.

## Profile/form metadata

- Profile field ordering, labels, required flags, placeholders, and select options are enough to generate a basic form.
- Mapping `AddressField` values back to a posted DTO still requires local glue code.
- Placeholder metadata exists, but help text is mostly empty in the current profiles.
- Spain's select list demonstrates the shape well; France and Ireland intentionally surface free-text administrative areas.

## Documentation gaps

- Multi-country DI examples should show core plus country packages, because core itself intentionally registers no country-specific rules.
- API examples for converting validation issues to `ProblemDetails`, model state, or field dictionaries would reduce repeated consumer code.
- Import/background-processing guidance could clarify when to use generic fallback and how to label unverified rows.

## Possible future package features

These are deliberately parked after the `2.0.1` close-out unless real consumer demand appears. The current package surface is considered sufficient for the next publish.

- `GetSupportedCountries` from a profile provider, validator factory, formatter, or aggregate registry.
- `ValidateAndFormat` helper returning validation result plus formatted output.
- `ToProblemDetails`, `ToModelState`, or `ToErrorDictionary` helpers for validation issues.
- Profile-to-DTO helpers for UI form generation.
- Explicit fallback result metadata so consumers can distinguish country-specific validation from generic acceptance.

## Defects or suspected defects

- The generic fallback profile originally marked locality and postal code as optional even though the core `Address` model requires them. This is fixed for `2.0.1`.
- The constructor-before-validation behaviour is not necessarily a defect, but it complicates structured validation in apps.

## Nice-to-have improvements

- `FormatSingleLine` and `FormatMultiLine` convenience methods.
- More profile help text for fields where placeholders are not enough.
- Published sample addresses per country package for docs and test rigs.
- A sample package consumer app in documentation that mirrors the checkout API registration pattern.
