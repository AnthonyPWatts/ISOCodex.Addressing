using System;
using System.Collections.Generic;
using System.Linq;
using ISOCodex.Addressing.Profiles;

namespace ISOCodex.Addressing.Brazil
{
    internal static class BrazilAdministrativeAreaData
    {
        private static readonly AdministrativeArea[] Areas =
        {
            Area("AC", "Acre"),
            Area("AL", "Alagoas"),
            Area("AP", "Amapá"),
            Area("AM", "Amazonas"),
            Area("BA", "Bahia"),
            Area("CE", "Ceará"),
            Area("DF", "Distrito Federal"),
            Area("ES", "Espírito Santo"),
            Area("GO", "Goiás"),
            Area("MA", "Maranhão"),
            Area("MT", "Mato Grosso"),
            Area("MS", "Mato Grosso do Sul"),
            Area("MG", "Minas Gerais"),
            Area("PA", "Pará"),
            Area("PB", "Paraíba"),
            Area("PR", "Paraná"),
            Area("PE", "Pernambuco"),
            Area("PI", "Piauí"),
            Area("RJ", "Rio de Janeiro"),
            Area("RN", "Rio Grande do Norte"),
            Area("RS", "Rio Grande do Sul"),
            Area("RO", "Rondônia"),
            Area("RR", "Roraima"),
            Area("SC", "Santa Catarina"),
            Area("SP", "São Paulo"),
            Area("SE", "Sergipe"),
            Area("TO", "Tocantins")
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

        private static AdministrativeArea Area(string code, string name)
        {
            return new AdministrativeArea(code, name);
        }

        private sealed class AdministrativeArea
        {
            public AdministrativeArea(string code, string name)
            {
                Code = code;
                Name = name;
                AcceptedValues = new[] { code, name };
            }

            public string Code { get; }

            public string Name { get; }

            public IReadOnlyList<string> AcceptedValues { get; }
        }
    }
}
