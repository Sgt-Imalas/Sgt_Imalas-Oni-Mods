using Newtonsoft.Json;
using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP")]
		[JsonIgnore]
		public System.Action<object> Button_OpenCarepackageEditor => BuildingEditor_MainScreen.ShowBuildingEditor;

		[Option("Chemical Processing - Industrial Overhaul: Enabled")]
		[JsonProperty]
		public bool ChemicalProcessing_IndustrialOverhaul_Enabled { get; set; } = true;
		[Option("Chemical Processing - BioChemistry: Enabled")]
		[JsonProperty]
		public bool ChemicalProcessing_BioChemistry_Enabled { get; set; } = true;
		[Option("Mineral Processing - Metallurgy: Enabled")]
		[JsonProperty]
		public bool MineralProcessing_Metallurgy_Enabled { get; set; } = true;
		[Option("Mineral Processing - Mining: Enabled")]
		[JsonProperty]
		public bool MineralProcessing_Mining_Enabled { get; set; } = true;
		[Option("Nuclear Processing: Enabled")]
		[JsonProperty]
		public bool NuclearProcessing_Enabled { get; set; } = true;

		[Option("Dupes Machinery: Enabled")]
		[JsonProperty]
		public bool DupesMachinery_Enabled { get; set; } = true;
		[Option("Dupes Engineering: Enabled")]
		[JsonProperty]
		public bool DupesEngineering_Enabled { get; set; } = true;

		[Option("Custom Reservoirs: Enabled")]
		public bool CustomReservoirs { get; set; } = true;
		[Option("Dupes Logistics: Enabled")]
		public bool DupesLogistics { get; set; } = true;

		[Option("High Pressure Applications: Enabled")]
		public bool HighPressureApplications { get; set; } = true;		


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.TOOLTIP")]
		public bool ModGeysersGeneric { get; set; } = true;


		[Option("High Pressure Gas Capacity")]
		[Limit(2, 20)]
		public int HPA_Capacity_Gas { get; set; } = 10;
		[Option("High Pressure Liquid Capacity")]
		[Limit(11, 200)]
		public int HPA_Capacity_Liquid { get; set; } = 40;

		[Option("High Pressure Pump base Wattage", "the base wattage value is multiplied with the respective pipe capacity. For the liquid pump, it is additionally divided by 10")]
		[Limit(10, 1000)]
		public int HPA_Pump_Base_Mult { get; set; } = 240;



		//[Option("STRINGS.RONIVAN_AIO_MODCONFIG.RONIVANDUPE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.RONIVANDUPE.TOOLTIP", "STRINGS.RONIVANL_AIO_MODCONFIG.A_CATEGORY_GENERIC")]
		//public bool RonivanDuplicant { get; set; } = true;

	}
}
