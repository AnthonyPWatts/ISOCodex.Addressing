using System;
using System.Collections.Generic;
using System.Linq;
using ISOCodex.Addressing.Profiles;

namespace ISOCodex.Addressing.India
{
    internal static class IndiaAdministrativeAreaData
    {
        private static readonly AdministrativeArea[] Areas =
        {
            Area("AP", "Andhra Pradesh"),
            Area("AR", "Arunachal Pradesh"),
            Area("AS", "Assam"),
            Area("BR", "Bihar"),
            Area("CT", "Chhattisgarh"),
            Area("GA", "Goa"),
            Area("GJ", "Gujarat"),
            Area("HR", "Haryana"),
            Area("HP", "Himachal Pradesh"),
            Area("JH", "Jharkhand"),
            Area("KA", "Karnataka"),
            Area("KL", "Kerala"),
            Area("MP", "Madhya Pradesh"),
            Area("MH", "Maharashtra"),
            Area("MN", "Manipur"),
            Area("ML", "Meghalaya"),
            Area("MZ", "Mizoram"),
            Area("NL", "Nagaland"),
            Area("OD", "Odisha", "OR"),
            Area("PB", "Punjab"),
            Area("RJ", "Rajasthan"),
            Area("SK", "Sikkim"),
            Area("TN", "Tamil Nadu"),
            Area("TG", "Telangana"),
            Area("TR", "Tripura"),
            Area("UP", "Uttar Pradesh"),
            Area("UT", "Uttarakhand"),
            Area("WB", "West Bengal"),
            Area("AN", "Andaman and Nicobar Islands", "Andaman & Nicobar"),
            Area("CH", "Chandigarh"),
            Area("DN", "Dadra and Nagar Haveli and Daman and Diu", "Dadra & Nagar Haveli & Daman & Diu", "DH"),
            Area("DL", "Delhi"),
            Area("JK", "Jammu and Kashmir", "Jammu & Kashmir"),
            Area("LA", "Ladakh"),
            Area("LD", "Lakshadweep"),
            Area("PY", "Puducherry")
        };

        private static readonly Dictionary<string, AdministrativeArea> AreasByValue =
            Areas
                .SelectMany(
                    area => area.AcceptedValues.Select(
                        value => new KeyValuePair<string, AdministrativeArea>(value, area)))
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);

        public static AddressFieldOption[] Options =>
            Areas
                .Select(area => new AddressFieldOption(area.Code, area.Name))
                .ToArray();

        public static bool IsValid(string value)
        {
            return AreasByValue.ContainsKey(value.Trim());
        }

        private static AdministrativeArea Area(
            string code,
            string name,
            params string[] aliases)
        {
            return new AdministrativeArea(code, name, aliases);
        }

        private sealed class AdministrativeArea
        {
            public AdministrativeArea(
                string code,
                string name,
                IEnumerable<string> aliases)
            {
                Code = code;
                Name = name;
                AcceptedValues = new[] { code, name }.Concat(aliases).ToArray();
            }

            public string Code { get; }

            public string Name { get; }

            public IReadOnlyList<string> AcceptedValues { get; }
        }
    }
}
