using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OniRetroEdition
{
	[Serializable]
	[RestartRequired]
	//[ConfigFile(SharedConfigLocation: true)]
	public class Config : SingletonOptions<Config>, IOptions
	{
		public enum EarlierVersion
		{
			Alpha,
			[Option("Thermal Update")]
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

		[Option("Old Lights", "old lighting color")]
		[JsonProperty]
		public bool oldLights { get; set; } = true;


		[Option("DarkenTints1", "")]
		[JsonProperty]
		public UnityEngine.Color DarkenTints1 { get; set; } = new UnityEngine.Color(0.846f, 0.846f, 0.846f, 1.000f);
		[Option("DarkenTints2", "")]
		public UnityEngine.Color DarkenTints2 { get; set; } = UnityEngine.Color.white;
		[Option("DarkenTints3", "")]
		public UnityEngine.Color DarkenTints3 { get; set; } = new UnityEngine.Color(1, 0.809f, 0.809f, 1.000f);
		[Option("CharacterLit", "")]
		public UnityEngine.Color CharacterLit { get; set; } = new UnityEngine.Color(1.000f, 0.98f, 0.816f, 1.000f);
		[Option("CharacterUnLit", "")]
		public UnityEngine.Color CharacterUnLit { get; set; } = new UnityEngine.Color(0.651f, 0.647f, 0.757f, 1.000f);
		[Option("Global_LightColor", "")]
		public UnityEngine.Color32 GlobalLightColor { get; set; } = Color.white;

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
