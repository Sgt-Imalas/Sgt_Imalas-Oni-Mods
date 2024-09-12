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
		[Option("Piped Output Compatibility - Use piped outputs", "Does nothing on its own.\nWhen the mod \"Piped Output\" or \"Piped Everything\" is installed,\nthe Electrolyzer will ignore that mod unless this option here is checked.\nIf checked and piped output is enabled, the electrolyzer will gain two additional piped outputs,\nchlorine and polluted oxygen", "(1) General Settings")]
		[JsonProperty]
		public bool IsPiped { get; set; }

		[Option("Solid Debris", "Using polluted water, salt water or brine will cause a tiny amount of debris to drop. Deactivate to have that mass added to the gasses instead.", "(1) General Settings")]
		[JsonProperty]
		public bool SolidDebris { get; set; }


		[Option("Water Type used affects Electrolyzer", "When active, the power consumption, output temperature and overpressurisation threshold are affected by the water type.", "(1) General Settings")]
		[JsonProperty]
		public bool PerLiquidSettings { get; set; }

		[Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Water.\nWhen \"Liquid Conductivity\" is disabled, this option defines the general electrolyzer power consumption.", "(2) Default Settings / Water")]
		[JsonProperty]
		[Limit(10, 300)]
		public int consumption_water { get; set; }
		[Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(2) Default Settings / Water")]
		[JsonProperty]
		[Limit(10, 160)]
		public int minOutputTemp_water { get; set; }
		[Option("Overpressurisation Threshold", "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Water.", "(2) Default Settings / Water")]
		[JsonProperty]
		[Limit(1, 15)]
		public float PressureThresholdMass_water { get; set; }



		[Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Polluted Water.", "(3) Polluted Water")]
		[JsonProperty]
		[Limit(10, 300)]
		public int consumption_pollutedwater { get; set; }

		[Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(3) Polluted Water")]
		[JsonProperty]
		[Limit(10, 160)]
		public int minOutputTemp_pollutedwater { get; set; }
		[Option("Overpressurisation Threshold", "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Polluted Water.", "(3) Polluted Water")]
		[JsonProperty]
		[Limit(1, 15)]
		public float PressureThresholdMass_pollutedwater { get; set; }




		[Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Salt Water.", "(4) Salt Water")]
		[JsonProperty]
		[Limit(10, 300)]
		public int consumption_saltwater { get; set; }

		[Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(4) Salt Water")]
		[JsonProperty]
		[Limit(10, 160)]
		public int minOutputTemp_saltwater { get; set; }

		[Option("Overpressurisation Threshold", "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Salt Water.", "(4) Salt Water")]
		[JsonProperty]
		[Limit(1, 15)]
		public float PressureThresholdMass_saltwater { get; set; }




		[Option("Power Consumption", "Power Consumption of the electrolyzer when it uses Brine.", "(5) Brine")]
		[JsonProperty]
		[Limit(10, 300)]
		public int consumption_brine { get; set; }

		[Option("Minimum Gas Output Temperature", "Minimum Temperature of the Output Gasses in °C ", "(5) Brine")]
		[JsonProperty]
		[Limit(10, 160)]
		public int minOutputTemp_brine { get; set; }
		[Option("Overpressurisation Threshold", "Gas Threshold Mass in KG above which the Electrolyzer overpressurizes when electrolyzing Brine.", "(5) Brine")]
		[JsonProperty]
		[Limit(1, 15)]
		public float PressureThresholdMass_brine { get; set; }

		public Config()
		{
			IsPiped = true;
			SolidDebris = false;
			PerLiquidSettings = true;

			PressureThresholdMass_water = 2.5f;
			PressureThresholdMass_pollutedwater = 1.8f;
			PressureThresholdMass_saltwater = 2.7f;
			PressureThresholdMass_brine = 3.0f;

			consumption_water = 120;
			consumption_pollutedwater = 130;
			consumption_saltwater = 90;
			consumption_brine = 60;

			minOutputTemp_brine = 75;
			minOutputTemp_saltwater = 70;
			minOutputTemp_pollutedwater = 50;
			minOutputTemp_water = 80;
		}
	}
}
