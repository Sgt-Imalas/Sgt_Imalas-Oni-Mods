using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAreaFill
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.FLOODFILLTOOLS_OPTIONS.SOLID_DOORS.NAME", "STRINGS.FLOODFILLTOOLS_OPTIONS.SOLID_DOORS.TOOLTIP")]
		[JsonProperty]
		public bool SolidDoors { get; set; } = true;

		[Option("STRINGS.FLOODFILLTOOLS_OPTIONS.HOLD_DELAY.NAME", "STRINGS.FLOODFILLTOOLS_OPTIONS.HOLD_DELAY.TOOLTIP", Format ="0.0 s")]
		[JsonProperty]
		public float ActivationDelay { get; set; } = 0.5f;
	}
}
