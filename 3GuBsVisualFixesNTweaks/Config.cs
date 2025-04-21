using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
		public enum TileBitChange
		{
			[Option("STRINGS.VFNT_MODCONFIG.HIDDENTILEBITS.TILEBITCHANGE.NONE")]
			None,
			[Option("STRINGS.VFNT_MODCONFIG.HIDDENTILEBITS.TILEBITCHANGE.BITSONLY")]
			BitsOnly,
			[Option("STRINGS.VFNT_MODCONFIG.HIDDENTILEBITS.TILEBITCHANGE.BITSANDTOPS")]
			BitsAndTops,
			[Option("STRINGS.VFNT_MODCONFIG.HIDDENTILEBITS.TILEBITCHANGE.EVERYTHING")]
			Everything
		}

		[Option("STRINGS.VFNT_MODCONFIG.ROCKETPLATFORM_FRONT.NAME", "STRINGS.VFNT_MODCONFIG.ROCKETPLATFORM_FRONT.TOOLTIP")]
		[JsonProperty]
		public bool RocketPlatformRenderChange { get; set; } = true;

		[Option("STRINGS.VFNT_MODCONFIG.HIDDENTILEBITS.NAME", "STRINGS.VFNT_MODCONFIG.HIDDENTILEBITS.TOOLTIP")]
		[JsonProperty]
		public TileBitChange HiddenTileBits { get; set; } = TileBitChange.None;
	}
}
