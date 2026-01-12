using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemDropPrevention
{
	[Serializable]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>, IOptions
	{

		[Option("STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS", "STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS_TOOLTIP")]
		[JsonProperty]
		public bool SweepDroppedItems { get; set; } = true;

		[Option("STRINGS.IDP_MOD_CONFIG.WRANGLE_DROPPED_CRITTERS", "STRINGS.IDP_MOD_CONFIG.WRANGLE_DROPPED_CRITTERS_TOOLTIP")]
		[JsonProperty]
		public bool WrangleDroppedCritters { get; set; } = true;


		public IEnumerable<IOptionsEntry> CreateOptions()
		{
			return [];
		}

		public void OnOptionsChanged()
		{
			Instance = this;
		}
	}
}
