using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace MineralizerReborn
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("Mineralizer")]
	public class Config : SingletonOptions<Config>
	{
		[Option("Watts used", "Amount of power the mineralizer uses.")]
		[JsonProperty]
		[Limit(10, 480)]
		public int WattsUsed { get; set; } = 120;
	}
}
