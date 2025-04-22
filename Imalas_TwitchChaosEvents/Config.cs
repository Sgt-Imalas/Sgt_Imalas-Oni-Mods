using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace Imalas_TwitchChaosEvents
{

	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("Chaos Twitch Events")]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.CHAOS_CONFIG.TACORAIN_MUSIC_NAME", "STRINGS.CHAOS_CONFIG.TACORAIN_MUSIC_TOOLTIP")]
		[JsonProperty]
		public bool TacoEventMusic { get; set; } = true;
		[Option("STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_MUSIC_NAME", "STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_MUSIC_TOOLTIP")]
		[JsonProperty]
		public bool FakeTacoEventMusic { get; set; } = true;

		[Option("STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_DURATION_NAME", "STRINGS.CHAOS_CONFIG.FAKE_TACORAIN_DURATION_TOOLTIP")]
		[JsonProperty]
		public int FakeTacoEventDuration { get; set; } = 50;


		[Option("STRINGS.CHAOS_CONFIG.FOG_DURATION_NAME", "STRINGS.CHAOS_CONFIG.FOG_DURATION_TOOLTIP")]
		[JsonProperty]
		public float FogDuration { get; set; } = 1;


		[Option("STRINGS.CHAOS_CONFIG.SKIP_MIN_CYCLE", "STRINGS.CHAOS_CONFIG.SKIP_MIN_CYCLE_TOOLTIP")]
		[JsonProperty]
		public bool SkipMinCycle { get; set; } = true;

		[Option("STRINGS.CHAOS_CONFIG.SHOW_WARNINGS", "STRINGS.CHAOS_CONFIG.SKIP_SHOW_WARNINGS_TOOLTIP")]
		[JsonProperty]
		public bool ShowWarnings { get; set; } = true;


		public Config()
		{
		}

	}
}
