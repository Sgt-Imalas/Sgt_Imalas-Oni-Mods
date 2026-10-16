using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace AquaticMinnowMinion
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option(title: "STRINGS.AQ_CONFIG.AQUATIC_CREW.NAME", tooltip: "STRINGS.AQ_CONFIG.AQUATIC_CREW.TOOLTIP")]
		[JsonProperty]
		public bool AquaticCrew { get; set; } = false;
	}
}
