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
		public bool AutomatedClusterPresets { get; set; } = true;

		[Option("STRINGS.MODCONFIG.CHALLENGEASTEROIDS.NAME", "STRINGS.MODCONFIG.CHALLENGEASTEROIDS.TOOLTIP")]
		[JsonProperty]
		public bool IncludeChallengeStarts { get; set; } = false;

		public Config()
		{
		}
	}
}
