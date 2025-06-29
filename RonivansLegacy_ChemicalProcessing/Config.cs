using Newtonsoft.Json;
using PeterHan.PLib.Options;
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

		[Option("Chemical Processing - Industrial Overhaul: Enabled")]
		[JsonProperty]
		public bool ChemicalProcessing_IndustrialOverhaul_Enabled { get; set; } = true;
		[Option("Chemical Processing - BioChemistry: Enabled")]
		[JsonProperty]
		public bool ChemicalProcessing_BioChemistry_Enabled { get; set; } = true;
		[Option("Mineral Processing - Mining: Enabled")]
		[JsonProperty]
		public bool MineralProcessing_Mining_Enabled { get; set; } = true;

	}
}
