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
        [Option("Piped", "Electrolyzer has piped outputs")]
        [JsonProperty]
        public bool IsPiped { get; set; }

        [Option("Solid Debris", "Using polluted water, salt water or brine will cause a tiny amount of debris to drop")]
        [JsonProperty]
        public bool SolidDebris { get; set; }

        [Option("Liquid Conductivity affects power consumption", "When active, the power consumption of the electrolyzer will be affected by the conductivity of the liquid.\nHigh conductivity liquids (salt water, brine), will make it consume less power, polluted water will slightly increase the power consumption.")]
        [JsonProperty]
        public bool LiquidConductivity { get; set; }

        [Option("Power Consumption: Default/Water", "Power Consumption of the electrolyzer when it uses Water.\nWhen \"Liquid Conductivity\" is disabled, this option defines the general electrolyzer power consumption.")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_water { get; set; }

        [Option("Power Consumption: Polluted Water", "Power Consumption of the electrolyzer when it uses Polluted Water.\nOnly has an effect when \"Liquid Conductivity\" is enabled.")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_pollutedwater { get; set; }

        [Option("Power Consumption: Salt Water", "Power Consumption of the electrolyzer when it uses Salt Water.\nOnly has an effect when \"Liquid Conductivity\" is enabled.")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_saltwater { get; set; }


        [Option("Power Consumption: Brine", "Power Consumption of the electrolyzer when it uses Brine.\nOnly has an effect when \"Liquid Conductivity\" is enabled.")]
        [JsonProperty]
        [Limit(10, 300)]
        public int consumption_brine { get; set; }

        public Config()
        {
            IsPiped = false;
            SolidDebris = true;
            LiquidConductivity = true;

            consumption_water = 120;
            consumption_pollutedwater = 130;
            consumption_saltwater = 60;
            consumption_brine = 30;
        }


        
    }
}
