using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace ClusterTraitGenerationManager
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.MODCONFIG.MANUALCLUSTERPRESETS.NAME", "STRINGS.MODCONFIG.MANUALCLUSTERPRESETS.TOOLTIP")]
		[JsonProperty]
		public bool AutomatedClusterPresets { get; set; }

		public Config()
		{
			AutomatedClusterPresets = true;
		}
	}
}
