using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace PoisNotIncluded
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.UI.SANDBOXTOOLS.SETTINGS.SPAWN_STORY_TRAIT.NAME", "STRINGS.UI.SANDBOXTOOLS.SETTINGS.SPAWN_STORY_TRAIT.TOOLTIP")]
		[JsonProperty]
		public bool ConstructableStoryTraits { get; set; } = true;
	}
}
