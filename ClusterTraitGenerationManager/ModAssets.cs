using HarmonyLib;
using Klei.AI;
using MonoMod.Utils;
using ProcGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.STRINGS.WORLD_TRAITS;
using static STRINGS.CREATURES.STATS;

namespace ClusterTraitGenerationManager
{
    internal class ModAssets
    {
        Transform Screen;
        public static GameObject CustomPlanetSideScreen;
        public static GameObject TraitPopup;
        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("clustergenerationsettingsmanager_menuassets", platformSpecific: true);
            var Assets = bundle.LoadAsset<GameObject>("Assets/CGMExport.prefab");

            //UIUtils.ListAllChildren(Assets.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(Assets);

            CustomPlanetSideScreen = Assets.transform.Find("IndividualSettings").gameObject;
            TraitPopup = Assets.transform.Find("TraitPopup").gameObject;
            //var TMPConverter = new TMPConverter();
            //TMPConverter.ReplaceAllText(wallSidescreenPrefab);
            //TMPConverter.ReplaceAllText(settingsDialogPrefab);
            RandomizedTraitsTrait = new WorldTrait();
            RandomizedTraitsTrait.name = "STRINGS.WORLD_TRAITS.CGM_RANDOMTRAIT.NAME";
            RandomizedTraitsTrait.description = "STRINGS.WORLD_TRAITS.CGM_RANDOMTRAIT.DESCRIPTION";
            RandomizedTraitsTrait.colorHex = "FFFFFF";
            RandomizedTraitsTrait.filePath = "traits/CGMRandomTraits";
        }
        public static readonly string CustomTraitID = "traits/CGMRandomTraits";
        public static WorldTrait RandomizedTraitsTrait;

        public static Dictionary<ProcGen.World, List<string>> ChangedMeteorSeasons = new Dictionary<ProcGen.World, List<string>>();

        public static Dictionary<string, WorldTrait> AllTraitsWithRandomDict
        {
            get
            {
                Dictionary<string, WorldTrait> traits = new Dictionary<string, WorldTrait>();
                traits.Add(ModAssets.CustomTraitID, ModAssets.RandomizedTraitsTrait);
                traits.AddRange(SettingsCache.worldTraits);
                return traits;
            }
        }

        public static List<KeyValuePair<string, WorldTrait>> AllTraitsWithRandom
        {
            get
            {
                List<KeyValuePair<string, WorldTrait>> traits = new List<KeyValuePair<string, WorldTrait>>();
                traits.Add(new KeyValuePair<string, WorldTrait>(ModAssets.CustomTraitID, ModAssets.RandomizedTraitsTrait));
                traits.AddRange(SettingsCache.worldTraits.ToList());
                return traits;
            }
        }
        public static List<WorldTrait> AllTraitsWithRandomValuesOnly
        {
            get
            {
                List<WorldTrait> traits = new List<WorldTrait>();
                foreach(var trait in AllTraitsWithRandom)
                {
                    traits.Add(trait.Value);
                }
                return traits;
            }
        }
    }
}
