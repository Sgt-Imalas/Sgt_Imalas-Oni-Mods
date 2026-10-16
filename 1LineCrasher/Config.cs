using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace _1LineCrasher
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
		[Option(title: "Cycle Clock Commas", tooltip: "The cycle clock number display is now more accurate.\nUseful for speedruns")]
		[JsonProperty]
		public bool CycleComma  { get; set; } = true;

		[Option(title: "Void Worlds", tooltip: "The asteroids back has been cut open, there is space exposure everywhere!")]
		[JsonProperty]
		public bool VoidWorld { get; set; } = false;

		[Option(title: "Power Consumption Multiplier", tooltip: "All buildings have become more expensive to run")]
		[JsonProperty]
		public float EnergyMultiplier { get; set; } = 1f;
	}
}
