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
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using static ClusterTraitGenerationManager.STRINGS.WORLD_TRAITS;
using static STRINGS.CREATURES.STATS;

namespace ClusterTraitGenerationManager
{
    internal class ModAssets
    {
        Transform Screen;
        public static GameObject CustomPlanetSideScreen;
        public static GameObject CGM_MainMenu;
        public static GameObject TraitPopup;
        public static GameObject PresetScreen;
        public static GameObject CustomGameSettings;
        public static string CustomClusterTemplatesPath;


        /// <summary>
        /// These have a guaranteed Start,Warp and Outer Version
        /// </summary>
        public static List<string> Moonlets = new List<string>()
            {
                "MiniBadlands",
                "MiniFlipped",
                "MiniForestFrozen",
                "MiniMetallicSwampy",
                "MiniRadioactiveOcean"
            };


        public static Sprite GetTraitSprite(WorldTrait trait)
        {
            Sprite TraitSprite = null;
            if(trait.icon != null && trait.icon.Length > 0)
            {
                TraitSprite = Assets.GetSprite(trait.icon); 
            }
            if(TraitSprite == null)
            {
                string associatedIcon = trait.filePath.Substring(trait.filePath.LastIndexOf("/") + 1);

                TraitSprite = Assets.GetSprite(associatedIcon);
            }
            return TraitSprite;
        }

        public static class Strings
        {
            public static string ApplyCategoryTypeToString(string input, StarmapItem item) => ApplyCategoryTypeToString(input, item.category);
            public static string ApplyCategoryTypeToString(string input, StarmapItemCategory category)
            {
                string returnVal = input.Replace("[STARMAPITEMTYPE]", ItemDescriptor(category, false));

                return returnVal.Replace("[STARMAPITEMTYPEPL]", ItemDescriptor(category, true));
            }

            public static string CategoryEnumToName(StarmapItemCategory starmapItemCategory)
            {
                string categoryName = string.Empty; //CATEGORYENUM
                switch (starmapItemCategory)
                {
                    case StarmapItemCategory.Starter:
                        categoryName = STRINGS.UI.CATEGORYENUM.START;
                        break;
                    case StarmapItemCategory.Warp:
                        categoryName = STRINGS.UI.CATEGORYENUM.WARP;
                        break;
                    case StarmapItemCategory.Outer:
                        categoryName = STRINGS.UI.CATEGORYENUM.OUTER;
                        break;
                    case StarmapItemCategory.POI:
                        categoryName = STRINGS.UI.CATEGORYENUM.POI;
                        break;
                }
                return categoryName;
            }
            public static string ItemDescriptor(StarmapItem item, bool plural = false)
            {
                return ItemDescriptor(item.category, plural);
            }
            public static string ItemDescriptor(StarmapItemCategory itemCategory, bool plural = false)
            {
                if(itemCategory == StarmapItemCategory.StoryTraits)
                {
                    return plural ?  STRINGS.UI.STARMAPITEMDESCRIPTOR.STORYTRAITPLURAL : STRINGS.UI.STARMAPITEMDESCRIPTOR.STORYTRAIT;
                }

                if (itemCategory == StarmapItemCategory.POI || itemCategory == StarmapItemCategory.VanillaStarmap)
                    return plural ? STRINGS.UI.STARMAPITEMDESCRIPTOR.POIPLURAL : STRINGS.UI.STARMAPITEMDESCRIPTOR.POI;
                else
                    return plural ? STRINGS.UI.STARMAPITEMDESCRIPTOR.ASTEROIDPLURAL : STRINGS.UI.STARMAPITEMDESCRIPTOR.ASTEROID;
            }

        }

        public static void LoadAssets()
        {
            var bundle = AssetUtils.LoadAssetBundle("clustergenerationsettingsmanager_menuassets", platformSpecific: true);
            var Assets = bundle.LoadAsset<GameObject>("Assets/UIs/CGMExport_SideMenus.prefab");
            CGM_MainMenu = bundle.LoadAsset<GameObject>("Assets/UIs/CGM_MainScreenExport.prefab");

            //UIUtils.ListAllChildren(Assets.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(Assets);
            TMPConverter.ReplaceAllText(CGM_MainMenu);

            CustomPlanetSideScreen = Assets.transform.Find("IndividualSettings").gameObject;
            TraitPopup = Assets.transform.Find("TraitPopup").gameObject;
            PresetScreen = Assets.transform.Find("PresetWindowCGM").gameObject;
            CustomGameSettings = Assets.transform.Find("GameSettingsChangerCGM").gameObject;
            //var TMPConverter = new TMPConverter();
            //TMPConverter.ReplaceAllText(wallSidescreenPrefab);
            //TMPConverter.ReplaceAllText(settingsDialogPrefab);
            RandomizedTraitsTrait = new WorldTrait();
            RandomizedTraitsTrait.name = "STRINGS.WORLD_TRAITS.CGM_RANDOMTRAIT.NAME";
            RandomizedTraitsTrait.description = "STRINGS.WORLD_TRAITS.CGM_RANDOMTRAIT.DESCRIPTION";
            RandomizedTraitsTrait.colorHex = "FFFFFF";
            RandomizedTraitsTrait.filePath = CustomTraitID;
        }
        public static readonly string CustomTraitID = "traits/CGMRandomTraits";
        public static WorldTrait RandomizedTraitsTrait;

        public static Dictionary<ProcGen.World, List<string>> ChangedMeteorSeasons = new Dictionary<ProcGen.World, List<string>>();

        public static Dictionary<string, WorldTrait> AllTraitsWithRandomDict
        {
            get
            {
                Dictionary<string, WorldTrait> traits = new Dictionary<string, WorldTrait>
                {
                    { ModAssets.CustomTraitID, ModAssets.RandomizedTraitsTrait }
                };
                traits.AddRange(SettingsCache.worldTraits);
                return traits;
            }
        }

        public static List<KeyValuePair<string, WorldTrait>> AllTraitsWithRandom
        {
            get
            {
                List<KeyValuePair<string, WorldTrait>> traits = new List<KeyValuePair<string, WorldTrait>>
                {
                    new KeyValuePair<string, WorldTrait>(ModAssets.CustomTraitID, ModAssets.RandomizedTraitsTrait)
                };
                traits.AddRange(SettingsCache.worldTraits.ToList());
                return traits;
            }
        }
        public static List<WorldTrait> AllTraitsWithRandomValuesOnly
        {
            get
            {
                List<WorldTrait> traits = new List<WorldTrait>();
                foreach (var trait in AllTraitsWithRandom)
                {
                    traits.Add(trait.Value);
                }
                return traits;
            }
        }
    }
}
