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
		public bool CustomReservoirs_Enabled { get; set; } = true;
		[Option("Dupes Logistics: Enabled")]
		public bool DupesLogistics_Enabled { get; set; } = true;

		[Option("High Pressure Applications: Enabled")]
		public bool HighPressureApplications_Enabled { get; set; } = true;


		[Option("Dupes Refrigeration: Enabled")]
		public bool DupesRefrigeration_Enabled { get; set; } = true;



		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.TOOLTIP")]
		public bool ModGeysersGeneric { get; set; } = true;


		[Option("High Pressure Gas Capacity")]
		[Limit(2, 20)]
		public int HPA_Capacity_Gas { get; set; } = 10;
		[Option("High Pressure Liquid Capacity")]
		[Limit(11, 200)]
		public int HPA_Capacity_Liquid { get; set; } = 40;

		[Option("High Pressure Gas Pump base Wattage", "the base wattage value is multiplied with the respective pipe capacity. For the liquid pump, it is additionally divided by 10")]
		[Limit(10, 1000)]
		public int HPA_Pump_Base_Mult_Gas { get; set; } = 240;

		[Option("High Pressure Liquid Pump base Wattage", "the base wattage value is multiplied with the respective pipe capacity. For the liquid pump, it is additionally divided by 10")]
		[Limit(10, 1000)]
		public int HPA_Pump_Base_Mult_Liquid { get; set; } = 240;


		[Option("Logistic Rail Capacity", "Logistic Rails serve as an early game version to conveyor rails, lacking the mechatronic requirements and unlocking earlier at the cost of lower throughput")]
		[Limit(1, 20)]
		public int Rail_Capacity_Logistic { get; set; } = 10;

		[Option("Heavy Duty Rail Capacity", "Logistic Rails serve as an late game alternative to conveyor rails, having a much higher throughput at the cost of more complex build requirements.")]
		[Limit(20, 400)]
		public int Rail_Capacity_HPA { get; set; } = 200;

		[Option("Logistic Auto-Sweeper Range", "The Logistic Auto-Sweeper serves as an early game version of the autosweeper, trading reduced carrying capacity for a lack of a mechatronics requirement")]
		[Limit(2, 12)]
		public int Logistic_Arm_Range { get; set; } = 4; //vanilla arm range, only capcaity is nerfed by default


		[Option("Heavy Duty Auto-Sweeper Range", "The Heavy Duty Auto-Sweeper serves as a late game version of the autosweeper, having higher range and throughput at the cost of more complex build requirements")]
		[Limit(6, 24)]
		public int HPA_Arm_Range { get; set; } = 10;



		//[Option("STRINGS.RONIVAN_AIO_MODCONFIG.RONIVANDUPE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.RONIVANDUPE.TOOLTIP", "STRINGS.RONIVANL_AIO_MODCONFIG.A_CATEGORY_GENERIC")]
		//public bool RonivanDuplicant { get; set; } = true;

	}
}
