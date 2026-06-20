# Changelog

## 1.3.0

- Added country packages for India, Brazil, Mexico, Germany, and Italy.
- Enriched existing validators for safer default postal-code handling and improved country-specific postal/admin-area coverage.
- Expanded US postal administrative-area support for additional USPS state, possession, and military codes.
- Tightened Canada postal-code structure validation.
- Tightened Ireland Eircode structural validation.
- Refactored newer validators/formatters to reuse shared validation and formatting helpers.

## 1.2.0

Moves all country-specific behaviour into country packages and adds Ireland and France.

### Includes

- Core package now supports zero countries and provides shared registries, abstractions, and generic fallbacks only.
- Great Britain, United States, and Canada moved from core into country packages.
- Extended consumer-style test rigs for checkout APIs, dynamic profile-driven forms, and bulk CSV imports.
- Release validation updates for the expanded country package family.

## 1.1.0

Adds Ireland and France country packages.

### Includes

- Ireland validation, formatting, DI registration and profile metadata.
- France validation, formatting, DI registration and profile metadata.
- Convenience `CountryCode` constants for all supported ISO 3166-1 alpha-2 codes.
- Release validation and CI packaging updates for the full package family.

## 1.0.0

Initial stable release of ISOCodex.Addressing.

### Includes

- Core address model and value objects.
- Formatting and validation service infrastructure.
- Initial GB, US and CA country support, later moved to country packages.
- Optional Spain country package.
- Profile/form metadata support.
- Framework-neutral validation results suitable for ASP.NET, Blazor, FluentValidation adapters, import pipelines and custom validation layers.
- NuGet packaging and release validation scripts.
