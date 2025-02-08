using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.CREATURES.SPECIES;

namespace LoadingPip
{
	[Serializable]
	[ConfigFile(SharedConfigLocation: true)]
	internal class Config : SingletonOptions<Config>, PeterHan.PLib.Options.IOptions
	{

		[Option("STRINGS.MODOPTIONS.LOADINGICON.NAME", "STRINGS.MODOPTIONS.LOADINGICON.TOOLTIP")]
		[JsonProperty]
		public string LoadingIdPrefabId { get; set; } = "Squirrel";
		[Option("STRINGS.MODOPTIONS.LOADINGICON.NAME", "STRINGS.MODOPTIONS.LOADINGICON.TOOLTIP")]
		[JsonProperty]
		public bool RandomizedCritters { get; set; } = false;

		[Option("STRINGS.MODOPTIONS.PRIMALASPID.NAME", "STRINGS.MODOPTIONS.PRIMALASPID.TOOLTIP")]
		[JsonIgnore]
		public System.Action<object> BUTTON_PRIMALASPID => (_) =>
		{
			RandomizedCritters = false;
			LoadingIdPrefabId = "Primal Aspid";
			POptions.WriteSettings(this);
		};
		public IEnumerable<IOptionsEntry> CreateOptions()
		{
			return new List<IOptionsEntry>();
		}

		public static Dictionary<string, string> SpecialIcons = new()
		{
			{"Primal Aspid","aspid"},
		};			

		public Tuple<Sprite,Color> GetTargetIcon()
		{
			if (SpecialIcons.TryGetValue(LoadingIdPrefabId, out var specialIcon) && Assets.GetSprite(specialIcon))
			{
				SgtLogger.l("getting special icon: " + LoadingIdPrefabId);
				return new(Assets.GetSprite(specialIcon), Color.white);
			}	

			var prefab = Assets.GetPrefab(LoadingIdPrefabId);

			if (RandomizedCritters)
			{
				List<GameObject> critters = new(Assets.GetPrefabsWithTag(GameTags.CreatureBrain));				
                if (critters.Any())
				{
					SgtLogger.l("randomized critters active, picking a random critter entity");
					prefab = critters.GetRandom();
                }
            }

			if (prefab != null && Def.GetUISprite(prefab) != null) //is a valid prefab
			{
				SgtLogger.l("fetching image from: "+ prefab.GetProperName());
				return Def.GetUISprite(prefab);
			}

			if (Assets.GetSprite(LoadingIdPrefabId) != null)
			{
				SgtLogger.l("fetching sprite from id: " + LoadingIdPrefabId);
				return new(Assets.GetSprite(LoadingIdPrefabId), Color.white);
			}

			if (NameIdHelper.TryGetIdFromName(LoadingIdPrefabId, out var id)) //user entered name instead of id
			{
				prefab = Assets.GetPrefab(id);
				if (prefab != null && Def.GetUISprite(prefab) != null) //is a valid prefab
				{
					SgtLogger.l("fetching image from name: "+LoadingIdPrefabId+" -> " + id);
					return Def.GetUISprite(prefab);
				}
			}

			//id was invalid, reverting to pip as a fallback
			SgtLogger.l("fallback to pip");			
			return Def.GetUISprite(Assets.GetPrefab("Squirrel"));
		}

		public void OnOptionsChanged()
		{
			Config.Instance = this; //instance doesnt get updated when a config entry changes, update this manually
		}
	}
}
