using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using UnityEngine;

namespace ForceFieldWallTile
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option("STRINGS.FFT_MODCONFIG.FFT_WATTAGE.NAME", "STRINGS.FFT_MODCONFIG.FFT_WATTAGE.TOOLTIP")]
		[JsonProperty]
		[Limit(5f, 100f)]
		public float NormalWattage { get; set; } = 50;

		[Option("STRINGS.FFT_MODCONFIG.FFT_WATTAGE_STEADY.NAME", "STRINGS.FFT_MODCONFIG.FFT_WATTAGE_STEADY.TOOLTIP")]
		[JsonProperty]
		[Limit(0.01f, 1f)]
		public float SteadyWattagePercentage { get; set; } = 0.2f;

		[Option("STRINGS.FFT_MODCONFIG.FFT_DUPEEFFECT.NAME", "STRINGS.FFT_MODCONFIG.FFT_DUPEEFFECT.TOOLTIP")]
		[JsonProperty]
		public bool SlowEffect { get; set; } = true;

		[Option("STRINGS.FFT_MODCONFIG.FFT_PRESSUREDAMAGE.NAME", "STRINGS.FFT_MODCONFIG.FFT_PRESSUREDAMAGE.TOOLTIP")]
		[JsonProperty]
		public bool PressureDamage { get; set; } = true;

		[Option("STRINGS.FFT_MODCONFIG.FFT_METEORYIELD.NAME", "STRINGS.FFT_MODCONFIG.FFT_METEORYIELD.TOOLTIP", Format = "0\\%")]
		[JsonProperty]
		[Limit(1f, 100f)]
		public float MeteorMassPercentage { get; set; } = 50f;

		[Option("STRINGS.FFT_MODCONFIG.FFT_OVERLOADCOOLDOWN.NAME", "STRINGS.FFT_MODCONFIG.FFT_OVERLOADCOOLDOWN.TOOLTIP", Format = "0 s")]
		[JsonProperty]
		[Limit(1, 20)]
		public int OverloadCooldown { get; set; } = 8;

		public float SteadyWattage() => NormalWattage * Mathf.Clamp01(SteadyWattagePercentage);
	}
}
