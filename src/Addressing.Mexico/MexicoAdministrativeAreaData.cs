using System;
using System.Collections.Generic;
using System.Linq;
using ISOCodex.Addressing.Profiles;

namespace ISOCodex.Addressing.Mexico
{
    internal static class MexicoAdministrativeAreaData
    {
        private static readonly AdministrativeArea[] Areas =
        {
            Area("AGU", "Aguascalientes", "Ags."),
            Area("BCN", "Baja California", "B.C."),
            Area("BCS", "Baja California Sur", "B.C.S."),
            Area("CAM", "Campeche", "Camp."),
            Area("CHP", "Chiapas", "Chis."),
            Area("CHH", "Chihuahua", "Chih."),
            Area("CMX", "Ciudad de México", "CDMX"),
            Area("COA", "Coahuila", "Coah.", "Coahuila de Zaragoza"),
            Area("COL", "Colima", "Col."),
            Area("DUR", "Durango", "Dgo."),
            Area("GUA", "Guanajuato", "Gto."),
            Area("GRO", "Guerrero", "Gro."),
            Area("HID", "Hidalgo", "Hgo."),
            Area("JAL", "Jalisco", "Jal."),
            Area("MEX", "México", "Méx.", "Estado de México"),
            Area("MIC", "Michoacán", "Mich."),
            Area("MOR", "Morelos", "Mor."),
            Area("NAY", "Nayarit", "Nay."),
            Area("NLE", "Nuevo León", "N.L."),
            Area("OAX", "Oaxaca", "Oax."),
            Area("PUE", "Puebla", "Pue."),
            Area("QUE", "Querétaro", "Qro."),
            Area("ROO", "Quintana Roo", "Q.R."),
            Area("SLP", "San Luis Potosí", "S.L.P."),
            Area("SIN", "Sinaloa", "Sin."),
            Area("SON", "Sonora", "Son."),
            Area("TAB", "Tabasco", "Tab."),
            Area("TAM", "Tamaulipas", "Tamps."),
            Area("TLA", "Tlaxcala", "Tlax."),
            Area("VER", "Veracruz", "Ver."),
            Area("YUC", "Yucatán", "Yuc."),
            Area("ZAC", "Zacatecas", "Zac.")
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
