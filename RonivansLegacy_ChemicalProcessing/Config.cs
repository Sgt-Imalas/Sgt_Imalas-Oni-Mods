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
		[Option("Dupes Machinery: Enabled")]
		[JsonProperty]
		public bool DupesMachinery_Enabled { get; set; } = true;


		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.GEYSERS.TOOLTIP", "STRINGS.RONIVANL_AIO_MODCONFIG.A_CATEGORY_GENERIC")]
		public bool ModGeysersGeneric { get; set; } = true;
		[Option("STRINGS.RONIVAN_AIO_MODCONFIG.RONIVANDUPE.NAME", "STRINGS.RONIVAN_AIO_MODCONFIG.RONIVANDUPE.TOOLTIP", "STRINGS.RONIVANL_AIO_MODCONFIG.A_CATEGORY_GENERIC")]
		public bool RonivanDuplicant { get; set; } = true;

	}
}
