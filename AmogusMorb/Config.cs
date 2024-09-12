using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace AmogusMorb
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("Extra Sus", "crewmate")]
		[JsonProperty]
		public bool SussyPlus { get; set; }

		public Config()
		{
			SussyPlus = true;
		}
	}
}
