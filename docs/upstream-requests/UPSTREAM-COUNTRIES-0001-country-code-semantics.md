---
status: ConsumedDownstream
consumer: ISOCodex.Addressing
provider: ISOCodex.Countries
classification: DocumentationRequest
created: 2026-06-20
last_reviewed: 2026-06-20
consumer_contact: Addressing agent
provider_contact: Countries agent
related_consumer_issue: null
related_provider_issue: https://github.com/AnthonyPWatts/ISOCodex.Countries/issues/1
related_provider_pr: null
provider_version_required: 1.0.0
---

# Countries country-code semantics documentation

## Summary

`ISOCodex.Addressing` needs clear provider guidance for the difference between alpha-2 syntax validation, current-country registry lookup, and special/non-country code element lookup.

## Consumer Use Case

Addressing generic fallback services should support current countries that do not have a country-specific address package, but they should not present generic postal-address support for known non-country code elements such as `EU`, unknown alpha-2-shaped values, or alias-like values such as `UK`.

## Provider Domain Boundary

Whether an alpha-2-shaped value is a current country, a known special code element, unknown, or invalid is country-code reference-data semantics. That belongs in `ISOCodex.Countries`, not `ISOCodex.Addressing`.

## Current Provider Behaviour

`ISOCodex.Countries` exposes:

- `CountryAlpha2Code.Parse(...)` and `TryParse(...)` for alpha-2 syntax validation.
- `CountryRegistry.TryGetByAlpha2(...)` for current-country lookup.
- `CountryCodeElementRegistry.TryGetByAlpha2(...)` for special or non-country code elements.

The behaviour is suitable for Addressing, but the intended consumer pattern needed explicit provider documentation.

## Why Existing APIs Were Sufficient

No provider API change was required. The integration issue was documentation clarity: consumers need to understand that successfully parsing a `CountryAlpha2Code` is not the same as confirming that the value is a current deliverable country.

## Provider Resolution

The provider documentation request was raised as:

https://github.com/AnthonyPWatts/ISOCodex.Countries/issues/1

It was resolved in `ISOCodex.Countries` `1.0.0`. The package README now documents the distinction between alpha-2 syntax validation, current-country registry lookup, and special code-element lookup, including the recommended `CountryAlpha2Code.TryParse` plus `CountryRegistry.TryGetByAlpha2` pattern for consumers.

## Consumer Resolution

Addressing consumes `ISOCodex.Countries` `1.0.0`.

Addressing keeps its existing behaviour:

- registered country packages use country-specific validators, formatters, and profiles;
- current countries without registered packages use generic fallbacks only when those fallbacks are registered;
- known special/non-country code elements such as `EU` do not use postal-address fallbacks;
- alias-like values such as `UK` are not silently resolved to `GB`.

No temporary workaround remains in Addressing.
