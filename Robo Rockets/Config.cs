using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace RoboRockets
{
	[Serializable]
	[RestartRequired]
	[ModInfo("Automated AI Rockets")]
	public class Config : SingletonOptions<Config>
	{
		[Option("Ai Speed", "Adjust the speed multiplier of AI Controlled Rockets; 0.5 is the vanilla autopilot speed, 1.0 is no penalty, 1.75 is the Equivalent of a dupe with maxed piloting")]
		[JsonProperty]
		[Limit(0.33f, 2f)]
		public float NoBrainRockets { get; set; }
		public Config()
		{
			AiLearnStart = 0.75f;
			NoBrainRockets = 0.8f;
		}


		[Option("Ai Brain Start Speed", "Adjust the speed multiplier AI Brains start with.\nAI Brains will steadily increase their speed")]
		[JsonProperty]
		[Limit(0.33f, 1.0f)]
		public float AiLearnStart { get; set; }

	}
}
