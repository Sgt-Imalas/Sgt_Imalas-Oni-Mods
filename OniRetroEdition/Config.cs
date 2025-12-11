using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace OniRetroEdition
{
	[Serializable]
	[RestartRequired]
	//[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>
	{
		public enum EarlierVersion
		{
			Alpha,
			Beta
		}

		[Option("Iron Ore Tile version", "")]
		[JsonProperty]
		public EarlierVersion IronOreTexture { get; set; } = EarlierVersion.Beta;

		[Option("Connect certain tile tops", "")]
		[JsonProperty]
		public bool TileTopsMerge { get; set; } = true;

		//[Option("mop becomes water succ", "")]
		//[JsonProperty]
		//public bool succmop { get; set; }

		//[Option("manual space can opener", "")]
		//[JsonProperty]
		//public bool manualRailgunPayloadOpener { get; set; }

		[Option("manual slime machine", "")]
		[JsonProperty]
		public bool manualSlimemachine { get; set; } = true;

		[Option("Gas Element Sensor takes power", "")]
		[JsonProperty]
		public bool gassensorpower { get; set; } = true;

		[Option("Gamma Ray Oven uses radbolts", "")]
		[JsonProperty]
		public bool GammaRayOvenRadbolts { get; set; } = true;
		[Option("liquid element sensor power requirement", "")]
		[JsonProperty]
		public bool liquidsensorpower { get; set; } = false;
		[Option("Duplicants rot forever", "when activated, unburied duplicants will rot forever. otherwise they will decompose into bones.")]
		[JsonProperty]
		public bool endlessRotting { get; set; } = false;


		[Option("Old Pipe Icons", "pipe input and output icons are replaced with older versions that change based on the connection state")]
		[JsonProperty]
		public bool oldPipeIcons { get; set; } = true;

		[Option("Old Dupe Outfits", "all outfits are red")]
		[JsonProperty]
		public bool oldDupeSuits { get; set; } = true;
	}
}
