using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using UtilLibs;

namespace GoodByeFrostByte
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("Goodbye Frostbyte")]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLEHEATDEBUFF.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLEHEATDEBUFF.TOOLTIP")]
		[JsonProperty]
		public bool DisableDupeHeatDebuff { get; set; } = true;

		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLECOLDDEBUFF.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLECOLDDEBUFF.TOOLTIP")]
		[JsonProperty]
		public bool DisableDupeColdDebuff { get; set; } = true;
		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLECOLDDAMAGE.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLECOLDDAMAGE.TOOLTIP")]
		[JsonProperty]
		public bool DisableDupeColdDamage { get; set; } = false;

		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLECOLDSLEEP.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.DISABLECOLDSLEEP.TOOLTIP")]
		[JsonProperty]
		public bool DisableDupeColdSleep { get; set; } = false;

		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.FROSTBITETHRESHOLD.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.FROSTBITETHRESHOLD.TOOLTIP")]
		[JsonProperty]
		public float FrostBiteThreshold { get; set; } = UtilMethods.GetCFromKelvin(183f); //from ScaldingMonitor.Def

		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.LOGARITHMICSPEEDDEBUFF.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.LOGARITHMICSPEEDDEBUFF.TOOLTIP")]
		[JsonProperty]
		public bool LogarithmicSpeedDebuff { get; set; } = false;

		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.OLDCRITTERTEMPDETECTION.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.OLDCRITTERTEMPDETECTION.TOOLTIP")]
		[JsonProperty]
		public bool OldCritterTemperatureDetection { get; set; } = true;

		[Option("STRINGS.MODCONFIG_GOODBYEFROSTBITE.OLDCRITTERTEMPHAPPINESS.NAME", "STRINGS.MODCONFIG_GOODBYEFROSTBITE.OLDCRITTERTEMPHAPPINESS.TOOLTIP")]
		[JsonProperty]
		public bool OldCritterTemperatureHappyness { get; set; } = true;


	}
}
