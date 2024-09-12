using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace MeteorDrops
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("Meteor Drops")]
	public class Config : SingletonOptions<Config>
	{
		[Option("Mass Percentage", "Percentage of its mass a meteor drops when blasted.")]
		[JsonProperty]
		[Limit(0, 200)]
		public int MassPercentage { get; set; }
		public Config()
		{
			MassPercentage = 50;
		}
	}
}
