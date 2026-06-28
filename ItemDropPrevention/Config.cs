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

		[Option("STRINGS.UI.CODEX.CATEGORYNAMES.ELEMENTSGAS", "STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS_TOOLTIP", "STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS")]
		[JsonProperty]
		public bool SweepDroppedItems_Gas { get; set; } = true;
		[Option("STRINGS.UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID", "STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS_TOOLTIP", "STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS")]
		[JsonProperty]
		public bool SweepDroppedItems_Liquid { get; set; } = true;
		[Option("STRINGS.UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID", "STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS_TOOLTIP", "STRINGS.IDP_MOD_CONFIG.SWEEP_DROPPED_ITEMS")]
		[JsonProperty]
		public bool SweepDroppedItems_Solid { get; set; } = true;

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
