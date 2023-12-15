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
using YamlDotNet.Core.Tokens;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER.VANILLAPOI_RESOURCES;
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
        public static GameObject SO_StarmapScreen;
        public static string CustomClusterTemplatesPath;
        public static readonly string TemporalTearId = "TemporalTear", TeapotId = "ArtifactSpacePOI_RussellsTeapot";

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



        public class POI_Data
        {
            public string Id;
            public string Name;
            public string Description;
            public Sprite Sprite;
            public Dictionary<SimHashes, float> Mineables;
            public bool HasArtifacts;

            public float RechargeMin, RechargeMax;
            public float CapacityMin,CapacityMax;

        }

        private static void InitPOIs()
        {
            _so_POIs = new Dictionary<string, POI_Data>();
            _nonUniquePOI_Ids = new List<string>();

            foreach (var item in Assets.GetPrefabsWithComponent<HarvestablePOIClusterGridEntity>())
            {
                var data = GetPoiData(item);
                if (data != null && !_so_POIs.ContainsKey(data.Id))
                {
                    _so_POIs.Add(data.Id, data);
                    _nonUniquePOI_Ids.Add(data.Id);
                }
            }
            foreach (var item in Assets.GetPrefabsWithComponent<ArtifactPOIClusterGridEntity>())
            {
                var data = GetPoiData(item);
                if (data != null && !_so_POIs.ContainsKey(data.Id))
                {
                    _so_POIs.Add(data.Id, data);
                    if(data.Id!=TeapotId)
                        _nonUniquePOI_Ids.Add(data.Id);
                }
            }
            foreach (var item in Assets.GetPrefabsWithComponent<TemporalTear>())
            {
                var data = GetPoiData(item);
                if (data != null && !_so_POIs.ContainsKey(data.Id))
                {
                    _so_POIs.Add(data.Id, data);
                }
            }
            var randomPoiData = new POI_Data();
            RandomPOIId = RandomKey + StarmapItemCategory.POI.ToString();
            randomPoiData.Id = RandomPOIId;
            randomPoiData.Name = STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.NAME;
            randomPoiData.Description = STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.DESCRIPTION;
            randomPoiData.Sprite = Assets.GetSprite(SpritePatch.randomPOI);
            randomPoiData.HasArtifacts = true;
            _so_POIs.Add(randomPoiData.Id, randomPoiData);

            _nonUniquePOI_Ids.Sort();
        }

        private static Dictionary<string, POI_Data> _so_POIs;
        private static List<string> _nonUniquePOI_Ids;
        public static List<string> NonUniquePOI_Ids
        {
            get
            {
                if (_nonUniquePOI_Ids == null)
                {
                    InitPOIs();
                }
                return _nonUniquePOI_Ids;
            }
        }
        public static string RandomPOIId;
        public static Dictionary<string, POI_Data> SO_POIs
        {
            get
            {
                if(_so_POIs == null)
                {
                    InitPOIs();
                }
                return _so_POIs;
            }
        }
        static POI_Data GetPoiData(GameObject item)
        {
            if (item.TryGetComponent<ClusterGridEntity>(out var component1))
            {
                POI_Data data = new POI_Data();

                data.Id = component1.PrefabID().ToString();

                var animName = component1.AnimConfigs.First().initialAnim;
                var animFile = component1.AnimConfigs.First().animFile;

                if (data.Id.Contains(HarvestablePOIConfig.CarbonAsteroidField)) ///carbon field fix
                    animName = "carbon_asteroid_field";

                if (animName == "closed_loop")///Temporal tear
                    animName = "ui";

                data.Sprite = Def.GetUISpriteFromMultiObjectAnim(animFile, animName, true);

                if (item.TryGetComponent<HarvestablePOIConfigurator>(out var harvest))
                {
                    var HarvestableConfig = HarvestablePOIConfigurator.FindType(harvest.presetType);
                    data.Mineables = new(HarvestableConfig.harvestableElements);
                    data.CapacityMin = HarvestableConfig.poiCapacityMin;
                    data.CapacityMax = HarvestableConfig.poiCapacityMax;
                    data.RechargeMin = HarvestableConfig.poiRechargeMin;
                    data.RechargeMax = HarvestableConfig.poiRechargeMax;
                }
                if (item.TryGetComponent<ArtifactPOIConfigurator>(out _))
                {
                    data.HasArtifacts = true;
                }

                if (item.TryGetComponent<InfoDescription>(out var descHolder))
                {
                    data.Description = descHolder.description;
                    data.Name = component1.Name;
                }
                if (component1 is ArtifactPOIClusterGridEntity && data.Name == null)
                {
                    string artifact_ID = component1.PrefabID().ToString().Replace("ArtifactSpacePOI_", string.Empty);
                    data.Name = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".NAME"));
                    data.Description = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".DESC"));
                }
                if (component1 is TemporalTear && data.Name == null)
                {
                    data.Name = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.NAME"));
                    data.Description = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.DESCRIPTION"));
                }

                return data;
            }
            return null;
        }



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
                    case StarmapItemCategory.VanillaStarmap:
                        categoryName = STRINGS.UI.CATEGORYENUM.VANILLASTARMAP;
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
                if (itemCategory == StarmapItemCategory.POI)
                    return plural ? STRINGS.UI.STARMAPITEMDESCRIPTOR.POI_GROUP_PLURAL : STRINGS.UI.STARMAPITEMDESCRIPTOR.POI_GROUP;

                if (itemCategory == StarmapItemCategory.VanillaStarmap)
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
            SO_StarmapScreen = bundle.LoadAsset<GameObject>("Assets/UIs/CGM_SOStarmap.prefab");

            //UIUtils.ListAllChildren(Assets.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(Assets);
            TMPConverter.ReplaceAllText(CGM_MainMenu);
            TMPConverter.ReplaceAllText(SO_StarmapScreen);

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
