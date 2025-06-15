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

		[Option("ChemicalProcessing Industrial Overhaul Enabled")]
		[JsonProperty]
		public bool ChemicalProcessing_IndustrialOverhaul_Enabled { get; set; } = true;
		[Option("ChemicalProcessing BioChemistry Enabled")]
		[JsonProperty]
		public bool ChemicalProcessing_BioChemistry_Enabled { get; set; } = true;

	}
}
