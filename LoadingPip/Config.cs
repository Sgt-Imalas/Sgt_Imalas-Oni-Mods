using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UtilLibs;
using static STRINGS.CREATURES.SPECIES;

namespace LoadingPip
{
	[Serializable]
	[ConfigFile(SharedConfigLocation: true)]
	internal class Config : SingletonOptions<Config>, PeterHan.PLib.Options.IOptions
	{
		public enum RandomIconOption
		{
			[Option("STRINGS.MODOPTIONS.RANDOMIZEDLOADINGICON.OPTION_NONE")]
			None = 0,
			[Option("STRINGS.MODOPTIONS.RANDOMIZEDLOADINGICON.OPTION_CRITTERS")]
			Critters = 1,
			[Option("STRINGS.MODOPTIONS.RANDOMIZEDLOADINGICON.OPTION_CUSTOM")]
			Custom = 2,
			[Option("STRINGS.MODOPTIONS.RANDOMIZEDLOADINGICON.OPTION_CHAOS")]
			Chaos = 3,
		}


		[Option("STRINGS.MODOPTIONS.LOADINGICON.NAME", "STRINGS.MODOPTIONS.LOADINGICON.TOOLTIP")]
		[JsonProperty]
		public string LoadingIdPrefabId { get; set; } = "Squirrel";

		[Option("STRINGS.MODOPTIONS.OPEN_ICON_FOLDER.NAME", "STRINGS.MODOPTIONS.OPEN_ICON_FOLDER.TOOLTIP")]
		[JsonIgnore]
		public System.Action<object> BUTTON_OPENFOLDER => (_) =>
		{
			App.OpenWebURL("file://" + System.IO.Path.Combine(IO_Utils.ModPath, "assets"));
		};
		[Option("STRINGS.MODOPTIONS.RANDOMIZEDLOADINGICON.NAME", "STRINGS.MODOPTIONS.RANDOMIZEDLOADINGICON.TOOLTIP")]
		[JsonProperty]
		public RandomIconOption RandomizationOption { get; set; } = RandomIconOption.None;

		[Option("STRINGS.MODOPTIONS.PRIMALASPID.NAME", "STRINGS.MODOPTIONS.PRIMALASPID.TOOLTIP")]
		[JsonIgnore]
		public System.Action<object> BUTTON_PRIMALASPID => (_) =>
		{
			this.RandomizationOption = RandomIconOption.None;
			this.LoadingIdPrefabId = "Primal Aspid";
			SgtLogger.l("Releasing the primal aspid >:3");
			POptions.WriteSettings(this);
			Instance = POptions.ReadSettings<Config>();
		};
		public IEnumerable<IOptionsEntry> CreateOptions()
		{
			return new List<IOptionsEntry>();
		}

		public static Dictionary<string, string> SpecialIcons = new()
		{
			{"Primal Aspid","aspid"},
		};

		public Tuple<Sprite, Color> GetTargetIcon()
		{
			string loadingPrefabId = LoadingIdPrefabId;

			GameObject prefab = null;
			if (RandomizationOption != RandomIconOption.None)
			{
				SgtLogger.l("Randomization Option: " + RandomizationOption);
				if (RandomizationOption == RandomIconOption.Critters)
				{

					List<GameObject> critters = new(Assets.GetPrefabsWithTag(GameTags.CreatureBrain));
					if (critters.Any())
					{
						SgtLogger.l("randomized critters active, picking a random critter entity");
						prefab = critters.GetRandom();
					}
				}
				else if (RandomizationOption == RandomIconOption.Custom)
				{
					SgtLogger.l("picking random custom icon from list of " + ModAssets.CustomLoadedIcons.Count);
					loadingPrefabId = ModAssets.CustomLoadedIcons.GetRandom();
				}
				else if (RandomizationOption == RandomIconOption.Chaos)
				{
					SgtLogger.l("picking fully random icon");
					List<string> allItemsList = [.. Assets.PrefabsByTag.Keys.Select(k => k.ToString())];
					allItemsList.AddRange(ModAssets.CustomLoadedIcons);

					loadingPrefabId = allItemsList.GetRandom();
				}
			}
			if (prefab == null)
				prefab = Assets.TryGetPrefab(loadingPrefabId);

			if (SpecialIcons.TryGetValue(loadingPrefabId, out var specialIcon) && Assets.GetSprite(specialIcon))
			{
				SgtLogger.l("getting special icon: " + loadingPrefabId);
				return new(Assets.GetSprite(specialIcon), Color.white);
			}
			if (prefab != null && Def.GetUISprite(prefab) != null) //is a valid prefab
			{
				SgtLogger.l("fetching image from: " + prefab.GetProperName());
				return Def.GetUISprite(prefab);
			}

			if (Assets.GetSprite(loadingPrefabId) != null)
			{
				SgtLogger.l("fetching sprite from id: " + loadingPrefabId);
				return new(Assets.GetSprite(loadingPrefabId), Color.white);
			}

			if (NameIdHelper.TryGetIdFromName(loadingPrefabId, out var id)) //user entered name instead of id
			{
				prefab = Assets.TryGetPrefab(id);
				if (prefab != null && Def.GetUISprite(prefab) != null) //is a valid prefab
				{
					SgtLogger.l("fetching image from name: " + loadingPrefabId + " -> " + id);
					return Def.GetUISprite(prefab);
				}
			}

			//id was invalid, reverting to pip as a fallback
			SgtLogger.l("fallback to pip");
			return Def.GetUISprite(Assets.TryGetPrefab("Squirrel"));
		}

		public void OnOptionsChanged()
		{
			POptions.WriteSettings(this);
			Instance = POptions.ReadSettings<Config>();
		}
	}
}
