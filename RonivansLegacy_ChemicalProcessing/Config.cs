using Newtonsoft.Json;
using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
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
		public static bool SubModEnabled(SourceModInfo mod )=> ModBuildingEnabled([mod]);
		public static bool ModBuildingEnabled(IEnumerable<SourceModInfo> buildingMods)
		{
			Debug.Assert(buildingMods != null && buildingMods.Any(), "ModBuildingEnabled called with null or empty buildingMods");
			foreach (var sourceMod in buildingMods)
			{
				switch (sourceMod)
				{
					case SourceModInfo.ChemicalProcessing_IO:
						if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
							return true;
						break;
					case SourceModInfo.ChemicalProcessing_BioChemistry:
						if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
							return true;
						break;
					case SourceModInfo.MineralProcessing_Metallurgy:
						if (Config.Instance.MineralProcessing_Metallurgy_Enabled)
							return true;
						break;
					case SourceModInfo.MineralProcessing_Mining:
						if (Config.Instance.MineralProcessing_Mining_Enabled)
							return true;
						break;
					case SourceModInfo.NuclearProcessing:
						if (Config.Instance.NuclearProcessing_Enabled)
							return true;
						break;
					case SourceModInfo.DupesMachinery:
						if (Config.Instance.DupesMachinery_Enabled)
							return true;
						break;
					case SourceModInfo.DupesEngineering:
						if (Config.Instance.DupesEngineering_Enabled)
							return true;
						break;
					case SourceModInfo.CustomReservoirs:
						if (Config.Instance.CustomReservoirs_Enabled)
							return true;
						break;
					case SourceModInfo.DupesLogistics:
						if (Config.Instance.DupesLogistics_Enabled)
							return true;
						break;
					case SourceModInfo.HighPressureApplications:
						if (Config.Instance.HighPressureApplications_Enabled)
							return true;
						break;
					case SourceModInfo.DupesRefrigeration:
						if (Config.Instance.DupesRefrigeration_Enabled)
							return true;
						break;
					case SourceModInfo.CustomGenerators:
						if (Config.Instance.CustomGenerators_Enabled)
							return true;
						break;
				}
			}
			return false;
		}


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null,null);


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP_ELEMENTS", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO")]
		[JsonProperty]
		public bool ChemicalProcessing_IndustrialOverhaul_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_IO => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.ChemicalProcessing_IO);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_BIOCHEMISTRY")]
		[JsonProperty]
		public bool ChemicalProcessing_BioChemistry_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_BIOCHEMISTRY")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Biochem => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.ChemicalProcessing_BioChemistry);


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.MINERALPROCESSING_METALLURGY")]
		[JsonProperty]
		public bool MineralProcessing_Metallurgy_Enabled { get; set; } = true;
		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.MINERALPROCESSING_METALLURGY")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Metallurgy => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.MineralProcessing_Metallurgy);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.MINERALPROCESSING_MINING")]
		[JsonProperty]
		public bool MineralProcessing_Mining_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.MINERALPROCESSING_MINING")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Mining => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.MineralProcessing_Mining);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.NUCLEARPROCESSING")]
		[JsonProperty]
		[RequireDLC(DlcManager.EXPANSION1_ID)] //hide this option in base game
		public bool NuclearProcessing_Enabled { get; set; } = true; 

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.NUCLEARPROCESSING")]
		[JsonIgnore]
		[RequireDLC(DlcManager.EXPANSION1_ID)]
		public System.Action<object> Button_OpenBuildingConfigEditor_Nuclear => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.NuclearProcessing);


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESMACHINERY")]
		[JsonProperty]
		public bool DupesMachinery_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESMACHINERY")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Machinery => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.DupesMachinery);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESENGINEERING")]
		[JsonProperty]
		public bool DupesEngineering_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESENGINEERING")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Engineering => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.DupesEngineering);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.CUSTOMRESERVOIRS")]
		public bool CustomReservoirs_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.CUSTOMRES_MOVETOCONDUITCATEGORY.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.CUSTOMRES_MOVETOCONDUITCATEGORY.TOOLTIP", "STRINGS.AIO_MODSOURCE.CUSTOMRESERVOIRS")]
		public bool ReservoirsInConduitCategory { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.CUSTOMRESERVOIRS")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Reservoirs => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.CustomReservoirs);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESLOGISTICS")]
		public bool DupesLogistics_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESLOGISTICS")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Logistics => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.DupesLogistics);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		public bool HighPressureApplications_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_HPA => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.HighPressureApplications);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_ENABLE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_ENABLE.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		public bool HPA_Rails_Enabled { get; set; } = true;
		[JsonIgnore]
		public bool HPA_Rails_Mod_Enabled => HPA_Rails_Enabled && HighPressureApplications_Enabled;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_INSULATION_ENABLE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_INSULATION_ENABLE.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		public bool HPA_Rails_Insulation_Enabled  { get; set; } = true;
		
		[JsonIgnore]
		public bool HPA_Rails_Insulation_Mod_Enabled => HPA_Rails_Mod_Enabled && HPA_Rails_Insulation_Enabled;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESREFRIGERATION")]
		public bool DupesRefrigeration_Enabled { get; set; } = true;
		
		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESREFRIGERATION")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Refrigeration => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.DupesRefrigeration);

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.ENABLEMOD.TOOLTIP", "STRINGS.AIO_MODSOURCE.CUSTOMGENERATORS")]
		public bool CustomGenerators_Enabled { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.NAME_ALT", "STRINGS.RONIVAN_AIO_MODCONFIG.BUILDINGEDITOR.TOOLTIP", "STRINGS.AIO_MODSOURCE.CUSTOMGENERATORS")]
		[JsonIgnore]
		public System.Action<object> Button_OpenBuildingConfigEditor_Generators => (_) => BuildingEditor_MainScreen.ShowBuildingEditor(null, SourceModInfo.CustomGenerators);


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO")]
		public bool ModGeysersGeneric { get; set; } = true;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.MODELEMENTSWORLDGEN.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.MODELEMENTSWORLDGEN.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO")]
		public bool WorldgenElementInjection { get; set; } = true;


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_GAS_CAPACITY.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_GAS_CAPACITY.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		[Limit(1, 25)]
		public float HPA_Capacity_Gas_Multiplier { get; set; } = 10;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_LIQUID_CAPACITY.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_LIQUID_CAPACITY.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		[Limit(1, 25)]
		public float HPA_Capacity_Liquid_Multiplier { get; set; } = 4;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_GAS_PUMPCOST.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_GAS_PUMPCOST.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		[Limit(10, 1000)]
		public int HPA_Pump_Base_Mult_Gas { get; set; } = 240;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_LIQUID_PUMPCOST.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_LIQUID_PUMPCOST.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		[Limit(10, 1000)]
		public int HPA_Pump_Base_Mult_Liquid { get; set; } = 240;


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.LOGISTIC_RAIL_CAPACITY.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.LOGISTIC_RAIL_CAPACITY.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESLOGISTICS")]
		[Limit(0.1f, 1f)]
		public float Logistic_Rail_Capacity_Multiplier { get; set; } = 0.5f;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_CAPACITY.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_CAPACITY.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		[Limit(1, 25)]
		public float HPA_Capacity_Solid_Multiplier { get; set; } = 10;

		///moved to building editor
		//[Option("STRINGS.RONIVAN_AIO_MODCONFIG.LOGISTIC_SWEEPER_RANGE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.LOGISTIC_SWEEPER_RANGE.TOOLTIP", "STRINGS.AIO_MODSOURCE.DUPESLOGISTICS")]
		//[Limit(2, 12)]
		//public int Logistic_Arm_Range { get; set; } = 4; //vanilla arm range, only capcaity is nerfed by default

		///moved to building editor
		//[Option("STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_ARMRANGE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.HP_SOLID_CAPACITY.TOOLTIP", "STRINGS.AIO_MODSOURCE.HIGHPRESSUREAPPLICATIONS")]
		//[Limit(6, 24)]
		//public int HPA_Arm_Range { get; set; } = 12;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.BIOCHEM_ANAEROBICDIGESTERBUFF.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.BIOCHEM_ANAEROBICDIGESTERBUFF.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_BIOCHEMISTRY")]
		[Limit(1, 200)]
		public int Biochem_AnaerobicDigesterBuff { get; set; } = 10;

		///[Option("STRINGS.RONIVAN_AIO_MODCONFIG.OLDREFINERIES.NAME", "STRINGS.BIOCHEM_OILPRESSREBALANCE.OLDREFINERIES.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO")]
		///public bool IO_OldRefineries { get; set; } = false;


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.CHEMPROC_REFINERYFUDGE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.CHEMPROC_REFINERYFUDGE.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO", Format = "P0")]
		[Limit(0.5f, 1f)]
		public float ChemProc_RefineryFudge { get; set; } = 0.6f;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.CHEMPROC_REFINERYFUDGEADV.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.CHEMPROC_REFINERYFUDGEADV.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO",Format ="P0")]
		[Limit(0.5f, 1f)]
		public float ChemProc_AdvRefineryFudge { get; set; } = 0.9f;

		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.CHEMPROC_ARCFUDGE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.CHEMPROC_ARCFUDGE.TOOLTIP", "STRINGS.AIO_MODSOURCE.CHEMICALPROCESSING_IO", Format = "P0")]
		[Limit(0.5f, 1f)]
		public float ChemProc_ArcFudge { get; set; } = 0.9f;
	}
}
