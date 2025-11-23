using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
		[JsonProperty]
		[Option("STRINGS.DECORPACKB_CONFIG.FUNCTIONALFOSSILS.NAME", "STRINGS.DECORPACKB_CONFIG.FUNCTIONALFOSSILS.TOOLTIP")]
		public bool FunctionalFossils { get; set; } = true;

		[JsonProperty]
		[Option("STRINGS.DECORPACKB_CONFIG.OILLANTERN_FLICKERS.NAME", "STRINGS.DECORPACKB_CONFIG.OILLANTERN_FLICKERS.TOOLTIP")]
		public bool OilLantern_Flickers { get; set; } = true;

		[JsonProperty]
		[Option("STRINGS.DECORPACKB_CONFIG.POTCAPACITY.NAME", "STRINGS.DECORPACKB_CONFIG.POTCAPACITY.TOOLTIP")]
		[Limit(100, 100000)]
		public float PotCapacity { get; set; } = 5000;
	}
}
