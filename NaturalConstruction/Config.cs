using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace NaturalConstruction
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{

		[Option(title: "STRINGS.UI.NC_MODCONFIG.CONSTRUCTIONTIME_MASS_SCALING.TITLE", tooltip: "STRINGS.UI.NC_MODCONFIG.CONSTRUCTIONTIME_MASS_SCALING.TOOLTIP")]
		[JsonProperty]
		public bool ScalingConstructionTime { get; set; } = true;

		[Option(title: "STRINGS.UI.NC_MODCONFIG.DEFAULT_MASS_TILE.TITLE", tooltip: "STRINGS.UI.NC_MODCONFIG.DEFAULT_MASS_TILE.TOOLTIP")]
		[JsonProperty]
		[Limit(1, 2000f)]
		public int DefaultMass_Tile { get; set; } = 100;

		[Option(title: "STRINGS.UI.NC_MODCONFIG.DEFAULT_MASS_BACKWALL.TITLE", tooltip: "STRINGS.UI.NC_MODCONFIG.DEFAULT_MASS_BACKWALL.TOOLTIP")]
		[JsonProperty]
		[Limit(1, 2000f)]
		public int DefaultMass_Backwall { get; set; } = 100;

		[Option(title: "STRINGS.UI.NC_MODCONFIG.CONSTRUCTION_MASS_MULTIPLIER.TITLE", tooltip: "STRINGS.UI.NC_MODCONFIG.CONSTRUCTION_MASS_MULTIPLIER.TOOLTIP")]
		[JsonProperty]
		[Limit(1f, 2f)]
		public float SpawningMassMultiplier { get; set; } = 1f;
	}
}
