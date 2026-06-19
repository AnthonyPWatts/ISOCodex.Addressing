using System;
using System.Collections.Generic;
using System.Linq;
using ISOCodex.Addressing.Profiles;

namespace ISOCodex.Addressing.Italy
{
    internal static class ItalyProvinceData
    {
        private static readonly Province[] Provinces =
        {
            CreateProvince("AG", "Agrigento"), CreateProvince("AL", "Alessandria"), CreateProvince("AN", "Ancona"),
            CreateProvince("AO", "Aosta"), CreateProvince("AR", "Arezzo"), CreateProvince("AP", "Ascoli Piceno"),
            CreateProvince("AT", "Asti"), CreateProvince("AV", "Avellino"), CreateProvince("BA", "Bari"),
            CreateProvince("BT", "Barletta-Andria-Trani"), CreateProvince("BL", "Belluno"),
            CreateProvince("BN", "Benevento"), CreateProvince("BG", "Bergamo"), CreateProvince("BI", "Biella"),
            CreateProvince("BO", "Bologna"), CreateProvince("BZ", "Bolzano"), CreateProvince("BS", "Brescia"),
            CreateProvince("BR", "Brindisi"), CreateProvince("CA", "Cagliari"), CreateProvince("CL", "Caltanissetta"),
            CreateProvince("CB", "Campobasso"), CreateProvince("CE", "Caserta"), CreateProvince("CT", "Catania"),
            CreateProvince("CZ", "Catanzaro"), CreateProvince("CH", "Chieti"), CreateProvince("CO", "Como"),
            CreateProvince("CS", "Cosenza"), CreateProvince("CR", "Cremona"), CreateProvince("KR", "Crotone"),
            CreateProvince("CN", "Cuneo"), CreateProvince("EN", "Enna"), CreateProvince("FM", "Fermo"),
            CreateProvince("FE", "Ferrara"), CreateProvince("FI", "Firenze"), CreateProvince("FG", "Foggia"),
            CreateProvince("FC", "Forlì-Cesena"), CreateProvince("FR", "Frosinone"), CreateProvince("GE", "Genova"),
            CreateProvince("GO", "Gorizia"), CreateProvince("GR", "Grosseto"), CreateProvince("IM", "Imperia"),
            CreateProvince("IS", "Isernia"), CreateProvince("AQ", "L'Aquila"), CreateProvince("SP", "La Spezia"),
            CreateProvince("LT", "Latina"), CreateProvince("LE", "Lecce"), CreateProvince("LC", "Lecco"),
            CreateProvince("LI", "Livorno"), CreateProvince("LO", "Lodi"), CreateProvince("LU", "Lucca"),
            CreateProvince("MC", "Macerata"), CreateProvince("MN", "Mantova"), CreateProvince("MS", "Massa-Carrara"),
            CreateProvince("MT", "Matera"), CreateProvince("ME", "Messina"), CreateProvince("MI", "Milano"),
            CreateProvince("MO", "Modena"), CreateProvince("MB", "Monza e Brianza"), CreateProvince("NA", "Napoli"),
            CreateProvince("NO", "Novara"), CreateProvince("NU", "Nuoro"), CreateProvince("OR", "Oristano"),
            CreateProvince("PD", "Padova"), CreateProvince("PA", "Palermo"), CreateProvince("PR", "Parma"),
            CreateProvince("PV", "Pavia"), CreateProvince("PG", "Perugia"), CreateProvince("PU", "Pesaro e Urbino"),
            CreateProvince("PE", "Pescara"), CreateProvince("PC", "Piacenza"), CreateProvince("PI", "Pisa"),
            CreateProvince("PT", "Pistoia"), CreateProvince("PN", "Pordenone"), CreateProvince("PZ", "Potenza"),
            CreateProvince("PO", "Prato"), CreateProvince("RG", "Ragusa"), CreateProvince("RA", "Ravenna"),
            CreateProvince("RC", "Reggio Calabria"), CreateProvince("RE", "Reggio Emilia"), CreateProvince("RI", "Rieti"),
            CreateProvince("RN", "Rimini"), CreateProvince("RM", "Roma"), CreateProvince("RO", "Rovigo"),
            CreateProvince("SA", "Salerno"), CreateProvince("SS", "Sassari"), CreateProvince("SV", "Savona"),
            CreateProvince("SI", "Siena"), CreateProvince("SR", "Siracusa"), CreateProvince("SO", "Sondrio"),
            CreateProvince("SU", "Sud Sardegna"), CreateProvince("TA", "Taranto"), CreateProvince("TE", "Teramo"),
            CreateProvince("TR", "Terni"), CreateProvince("TO", "Torino"), CreateProvince("TP", "Trapani"),
            CreateProvince("TN", "Trento"), CreateProvince("TV", "Treviso"), CreateProvince("TS", "Trieste"),
            CreateProvince("UD", "Udine"), CreateProvince("VA", "Varese"), CreateProvince("VE", "Venezia"),
            CreateProvince("VB", "Verbano-Cusio-Ossola"), CreateProvince("VC", "Vercelli"),
            CreateProvince("VR", "Verona"), CreateProvince("VV", "Vibo Valentia"), CreateProvince("VI", "Vicenza"),
            CreateProvince("VT", "Viterbo")
        };

        private static readonly Dictionary<string, Province> ProvincesByValue =
            Provinces
                .SelectMany(
                    province => province.AcceptedValues.Select(
                        value => new KeyValuePair<string, Province>(value, province)))
                .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);

        public static AddressFieldOption[] Options =>
            Provinces
                .Select(province => new AddressFieldOption(province.Code, province.Name))
                .ToArray();

        public static bool IsValid(string value)
        {
            return ProvincesByValue.ContainsKey(value.Trim());
        }

        private static Province CreateProvince(string code, string name)
        {
            return new Province(code, name);
        }

        private sealed class Province
        {
            public Province(string code, string name)
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
