using System;
using System.Collections.Generic;
using System.Linq;
using ISOCodex.Addressing.Profiles;

namespace ISOCodex.Addressing.Spain
{
    internal static class SpanishProvinceData
    {
        private static readonly SpanishProvince[] Provinces =
        {
            Province("01", "Álava", "Araba", "Araba/Álava"),
            Province("02", "Albacete"),
            Province("03", "Alicante"),
            Province("04", "Almería"),
            Province("05", "Ávila"),
            Province("06", "Badajoz"),
            Province("07", "Balearic Islands", "Illes Balears", "Islas Baleares"),
            Province("08", "Barcelona"),
            Province("09", "Burgos"),
            Province("10", "Cáceres"),
            Province("11", "Cádiz"),
            Province("12", "Castellón"),
            Province("13", "Ciudad Real"),
            Province("14", "Córdoba"),
            Province("15", "A Coruña", "La Coruña"),
            Province("16", "Cuenca"),
            Province("17", "Girona", "Gerona"),
            Province("18", "Granada"),
            Province("19", "Guadalajara"),
            Province("20", "Guipúzcoa", "Gipuzkoa"),
            Province("21", "Huelva"),
            Province("22", "Huesca"),
            Province("23", "Jaén"),
            Province("24", "León"),
            Province("25", "Lleida", "Lérida"),
            Province("26", "La Rioja"),
            Province("27", "Lugo"),
            Province("28", "Madrid"),
            Province("29", "Málaga"),
            Province("30", "Murcia"),
            Province("31", "Navarra"),
            Province("32", "Ourense", "Orense"),
            Province("33", "Asturias"),
            Province("34", "Palencia"),
            Province("35", "Las Palmas"),
            Province("36", "Pontevedra"),
            Province("37", "Salamanca"),
            Province("38", "Santa Cruz de Tenerife"),
            Province("39", "Cantabria"),
            Province("40", "Segovia"),
            Province("41", "Sevilla"),
            Province("42", "Soria"),
            Province("43", "Tarragona"),
            Province("44", "Teruel"),
            Province("45", "Toledo"),
            Province("46", "Valencia"),
            Province("47", "Valladolid"),
            Province("48", "Vizcaya", "Bizkaia"),
            Province("49", "Zamora"),
            Province("50", "Zaragoza"),
            Province("51", "Ceuta"),
            Province("52", "Melilla")
        };

        private static readonly Dictionary<string, SpanishProvince> ProvincesByPrefix =
            Provinces.ToDictionary(province => province.PostalCodePrefix);

        private static readonly Dictionary<string, SpanishProvince> ProvincesByName =
            Provinces
                .SelectMany(
                    province => province.Names.Select(
                        name => new KeyValuePair<string, SpanishProvince>(name, province)))
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);

        public static AddressFieldOption[] AdministrativeAreaOptions =>
            Provinces
                .Select(province => new AddressFieldOption(province.Name, province.Name))
                .ToArray();

        public static bool IsValidProvince(string value)
        {
            return ProvincesByName.ContainsKey(value.Trim());
        }

        public static bool TryGetProvinceForPostalCode(
            string postalCode,
            out string provinceName)
        {
            provinceName = string.Empty;

            if (!ProvincesByPrefix.TryGetValue(postalCode.Substring(0, 2), out var province))
            {
                return false;
            }

            provinceName = province.Name;
            return true;
        }

        public static bool MatchesProvince(string value, string provinceName)
        {
            return ProvincesByName.TryGetValue(value.Trim(), out var suppliedProvince) &&
                string.Equals(suppliedProvince.Name, provinceName, StringComparison.OrdinalIgnoreCase);
        }

        private static SpanishProvince Province(
            string postalCodePrefix,
            string name,
            params string[] aliases)
        {
            return new SpanishProvince(postalCodePrefix, name, aliases);
        }

        private sealed class SpanishProvince
        {
            public SpanishProvince(
                string postalCodePrefix,
                string name,
                IEnumerable<string> aliases)
            {
                PostalCodePrefix = postalCodePrefix;
                Name = name;
                Names = new[] { name }.Concat(aliases).ToArray();
            }

            public string PostalCodePrefix { get; }

            public string Name { get; }

            public IReadOnlyList<string> Names { get; }
        }
    }
}
