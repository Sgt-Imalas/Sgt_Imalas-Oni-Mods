using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace UL_UniversalLyzer
{
    [Serializable]
    [RestartRequired]
    [ModInfo("Universal Electrolyzer")]
    public class Config : SingletonOptions<Config>
    {
        [Option("Piped Output Compatibility - Use piped outputs", "Does nothing on its own. When the mod \"Piped Output\" is installed,\nthe Electrolyzer will ignore that mod unless this option here is checked.\nIf checked and piped output is enabled, the electrolyzer will gain two additional piped outputs,\nchlorine and polluted oxygen", "(1) General Settings") ]
        [JsonProperty]
        public bool IsPiped { get; set; }

        [Option("Solid Debris", "Using polluted water, salt water or brine will cause a tiny amount of debris to drop. Deactivate to have that mass added to the gasses instead.", "(1) General Settings")]
        [JsonProperty]
        public bool SolidDebris { get; set; }

        [Option("Liquid Conductivity affects power consumption", "When active, the power consumption of the electrolyzer will be affected by the conductivity of the liquid.\nHigh conductivity liquids (salt water, brine), will make it consume less power,\npolluted water will slightly increase the power consumption.", "(1) General Settings")]
        [JsonProperty]
        public bool LiquidConductivity { get; set; }

        [Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Water.\nWhen \"Liquid Conductivity\" is disabled, this option defines the general electrolyzer power consumption.", "(2) Default / Water Settings")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_water { get; set; }
        [Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(2) Default / Water Settings")]
        [JsonProperty]
        [Limit(10, 160)]
        public int minOutputTemp_water { get; set; }

        [Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Polluted Water.\nOnly has an effect when \"Liquid Conductivity\" is enabled.", "(3) Polluted Water")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_pollutedwater { get; set; }

        [Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(3) Polluted Water")]
        [JsonProperty]
        [Limit(10, 160)]
        public int minOutputTemp_pollutedwater { get; set; }

        [Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Salt Water.\nOnly has an effect when \"Liquid Conductivity\" is enabled.", "(4) Salt Water")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_saltwater { get; set; }

        [Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(4) Salt Water")]
        [JsonProperty]
        [Limit(10, 160)]
        public int minOutputTemp_saltwater { get; set; }


        [Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Brine.\nOnly has an effect when \"Liquid Conductivity\" is enabled.", "(5) Brine")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_brine { get; set; }

        [Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(5) Brine")]
        [JsonProperty]
        [Limit(10, 160)]
        public int minOutputTemp_brine { get; set; }

        public Config()
        {
            IsPiped = false;
            SolidDebris = true;
            LiquidConductivity = true;

            consumption_water = 120;
            consumption_pollutedwater = 130;
            consumption_saltwater = 80;
            consumption_brine = 50;

            minOutputTemp_brine = 70; 
            minOutputTemp_saltwater = 70;
            minOutputTemp_pollutedwater = 70;
            minOutputTemp_water = 70;
        }
    }
}
