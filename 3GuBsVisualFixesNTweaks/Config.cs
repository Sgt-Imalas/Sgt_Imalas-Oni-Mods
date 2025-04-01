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

		[Option("STRINGS.VFNT_MODCONFIG.ROCKETPLATFORM_FRONT.NAME", "STRINGS.VFNT_MODCONFIG.ROCKETPLATFORM_FRONT.TOOLTIP")]
		[JsonProperty]
		public bool RocketPlatformRenderChange { get; set; }

		public Config()
		{
			RocketPlatformRenderChange = true;
		}
	}
}
