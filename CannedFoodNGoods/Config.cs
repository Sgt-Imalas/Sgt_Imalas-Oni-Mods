using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace CannedFoods
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("Canned Food")]
	public class Config : SingletonOptions<Config>
	{
		[Option("Can material", "Select the material the cans are made of.\nThis affects the recipes and the drops")]
		[JsonProperty]
		public MaterialUsed UsesAluminumForCans { get; set; }
		public Config()
		{
			UsesAluminumForCans = MaterialUsed.Copper;
		}
		public enum MaterialUsed
		{
			Steel,
			Aluminum,
			Copper
		}
		public SimHashes GetCanElement()
		{
			switch (UsesAluminumForCans)
			{
				case MaterialUsed.Steel:
					return SimHashes.Steel;
				case MaterialUsed.Aluminum:
					return SimHashes.Aluminum;
				case MaterialUsed.Copper:
				default:
					return SimHashes.Copper;
			}
		}

	}
}
