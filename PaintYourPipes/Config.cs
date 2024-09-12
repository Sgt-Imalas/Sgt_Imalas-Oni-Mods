using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace PaintYourPipes
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.MODCONFIG.OVERLAYONLYNAME", "STRINGS.MODCONFIG.OVERLAYONLYDESC")]
		[JsonProperty]
		public bool OverlayOnly { get; set; }

		public Config()
		{
			OverlayOnly = false;
		}
	}
}
