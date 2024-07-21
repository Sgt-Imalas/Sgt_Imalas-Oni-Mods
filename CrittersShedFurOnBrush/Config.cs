using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrittersShedFurOnBrush
{
    [Serializable]
    [RestartRequired]
    [ModInfo("Critter Shedding")]
    public class Config : SingletonOptions<Config>
    {

        [Option("STRINGS.CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.NAME")]
        [JsonProperty]
        public bool SageHatch { get; set; } = true;

        [Option("STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.NAME")]
        [JsonProperty]
        public bool CuddlePip { get; set; } = true;

        [Option("STRINGS.CREATURES.SPECIES.DRECKO.NAME")]
        [JsonProperty]
        public bool Drecko { get; set; } = true;

        [Option("STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.NAME")]
        [JsonProperty]
        public bool OilFloaterFur { get; set; } = true;

        [Option("STRINGS.CREATURES.SPECIES.STATERPILLAR.NAME")]
        [JsonProperty]
        public bool PlugSlug { get; set; } = true;

        [Option("STRINGS.CREATURES.FAMILY_PLURAL.PUFTSPECIES")]
        [JsonProperty]
        public bool Pufts { get; set; } = true;


        [Option("STRINGS.CREATURES.FAMILY_PLURAL.DEERSPECIES")]
        [JsonProperty]
        public bool Flox { get; set; } = true;

        [Option("STRINGS.CREATURES.FAMILY_PLURAL.BELLYSPECIES")]
        [JsonProperty]
        public bool Bammoth { get; set; } = true;

        public Config()
        {
            SageHatch = true;
            CuddlePip = true;
            Drecko = true;
            OilFloaterFur = true;
            Pufts = true;
            PlugSlug = true;
            Bammoth = true;
            Flox = true;
        }
    }
}
