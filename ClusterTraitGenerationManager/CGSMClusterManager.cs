using ClusterTraitGenerationManager.SO_StarmapEditor;
using Database;
using FMOD;
using HarmonyLib;
using Klei.AI;
using Klei.CustomSettings;
using Newtonsoft.Json;
using ProcGen;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UtilLibs;
using YamlDotNet.Samples;
using static ClusterTraitGenerationManager.Patches;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static Klei.ClusterLayoutSave;
using static ProcGen.WorldPlacement;
using static SandboxSettings;
using static STRINGS.NAMEGEN;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS;

namespace ClusterTraitGenerationManager
{
    public class CGSMClusterManager
    {
        public const float MaxAmountPOI = 5f;
        public const float MaxAmountRandomPOI = 32f; //TODO: dynamic scale with map size
        public const float MaxAmountRandomPlanet = 6f;
        public const string RandomKey = "CGM_RANDOM_";
        static Dictionary<string, StarmapItem> PlanetsAndPOIs = null;
        static List<string> RandomOuterPlanets = new List<string>();

        public const int ringMax = 25, ringMin = 6;
        public static int MaxPOICount = 1;
        public static GameObject Screen = null;

        public static ColonyDestinationSelectScreen selectScreen;
        // public static StarmapItem RandomPOIStarmapItem = null;
        public static StarmapItem RandomOuterPlanetsStarmapItem = null;

        public static HashSet<string> BlacklistedTraits = new HashSet<string>();

        public static int MaxClassicOuterPlanets = 3, CurrentClassicOuterPlanets = 0;

        public static bool LoadCustomCluster
        {
            get
            {
                return _loadCustomCluster;
            }
            set
            {
                _loadCustomCluster = value;
                if (value == false)
                {
                    ResetMeteorSeasons();
                }
            }
        }
        private static bool _loadCustomCluster = false;


        public static bool RandomTraitInBlacklist(string traitId) => BlacklistedTraits.Contains(traitId);
        public static bool ToggleRandomTraitBlacklist(string traitID)
        {
            if (BlacklistedTraits.Contains(traitID))
            {
                BlacklistedTraits.Remove(traitID);
                return false;
            }
            else
            {
                BlacklistedTraits.Add(traitID);
                return true;
            }
        }

        public static void ResetMeteorSeasons(ProcGen.World singleItem = null)
        {
            if (singleItem == null)
            {
                foreach (var planet in ModAssets.ChangedMeteorSeasons)
                {
                    planet.Key.seasons = new List<string>(planet.Value);
                }
                ModAssets.ChangedMeteorSeasons.Clear();
            }
            else
            {
                if (singleItem != null && ModAssets.ChangedMeteorSeasons.ContainsKey(singleItem))
                {
                    singleItem.seasons = new List<string>(ModAssets.ChangedMeteorSeasons[singleItem]);
                    ModAssets.ChangedMeteorSeasons.Remove(singleItem);
                }
            }
        }
        public static CGM_MainScreen_UnityScreen CGM_Screen = null;

        public static async void InstantiateClusterSelectionView(ColonyDestinationSelectScreen parent, System.Action onClose = null)
        {
            if (Screen == null)
            {
                ///Change to check for moonlet/classic start
                if (CustomCluster == null)
                {
                    string defaultCluster;
                    if (DlcManager.IsExpansion1Active())
                        defaultCluster = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                    else
                        defaultCluster = "SandstoneDefault";

                    CGSMClusterManager.CreateCustomClusterFrom(defaultCluster);
                }

                Screen = Util.KInstantiateUI(ModAssets.CGM_MainMenu, FrontEndManager.Instance.gameObject, true);
                selectScreen = parent;

                Screen.name = "ClusterSelectionView";
                CGM_Screen = Screen.AddComponent<CGM_MainScreen_UnityScreen>();
            }
            if (CustomCluster == null)
            {
                string defaultCluster;

                if (DlcManager.IsExpansion1Active())
                    defaultCluster = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                else
                    defaultCluster = "SandstoneDefault";

                CGSMClusterManager.CreateCustomClusterFrom(defaultCluster);
            }
            LoadCustomCluster = false;

            Screen.transform.SetAsLastSibling();
            Screen.gameObject.SetActive(true);
            CGM_Screen = Screen.GetComponent<CGM_MainScreen_UnityScreen>();
            CGM_Screen.SelectCategory(StarmapItemCategory.Starter);
            CGM_Screen.RefreshView();
        }

        public static void SetAndStretchToParentSize(RectTransform _mRect, RectTransform _parent)
        {
            _mRect.anchoredPosition = _parent.position;
            _mRect.anchorMin = new Vector2(1, 0);
            _mRect.anchorMax = new Vector2(0, 1);
            _mRect.pivot = new Vector2(0.5f, 0.5f);
            _mRect.sizeDelta = _parent.rect.size;
            _mRect.transform.SetParent(_parent);
        }

        public enum StarmapItemCategory
        {
            Starter = 0,
            Warp = 1,
            Outer = 2,
            POI = 3,

            ///These only exist for the UI handlers
            None = -1,
            StoryTraits = -2,
            GameSettings = -3,
            VanillaStarmap = -4,
            SpacedOutStarmap = -5,
        }


        public class CustomClusterData
        {
            int GetAdjustedOuterExpansion()
            {
                int outerPlanetcount = RandomOuterPlanetsStarmapItem != null ? (int)RandomOuterPlanetsStarmapItem.InstancesToSpawn : 0;
                bool outerplanetSelected = RandomOuterPlanetsStarmapItem != null ? CustomCluster.HasStarmapItem(RandomOuterPlanetsStarmapItem.id, out _) : false;

                int planetDiff = (CustomCluster.OuterPlanets.Count + (outerplanetSelected ? -1 : 0) + outerPlanetcount - CustomCluster.defaultOuterPlanets);


                return planetDiff;
            }

            [JsonIgnore] public bool HasTear => POIs != null && POIs.Any(item => item.Value.placementPOI != null && item.Value.placementPOI.pois != null && item.Value.placementPOI.pois.Contains("TemporalTear"));
            [JsonIgnore] public bool HasTeapot => POIs != null && POIs.Any(item => item.Value.placementPOI != null && item.Value.placementPOI.pois != null && item.Value.placementPOI.pois.Contains("ArtifactSpacePOI_RussellsTeapot"));
            [JsonIgnore] public int AdjustedOuterExpansion => GetAdjustedOuterExpansion();

            public int defaultRings = 12;
            public int defaultOuterPlanets = 6;
            public int Rings { get; private set; }

            [JsonIgnore] private SO_StarmapLayout _so_Starmap;
            [JsonIgnore]
            public SO_StarmapLayout SO_Starmap
            {
                get
                {
                    if (_so_Starmap == null)
                        _so_Starmap = new SO_StarmapLayout(CurrentSeed);
                    return _so_Starmap;
                }
                set
                {
                    _so_Starmap = value;
                }
            }

            public StarmapItem StarterPlanet { get; set; }
            public StarmapItem WarpPlanet { get; set; }
            public Dictionary<string, StarmapItem> OuterPlanets = new Dictionary<string, StarmapItem>();
            public Dictionary<string, StarmapItem> POIs = new Dictionary<string, StarmapItem>();
            public string DLC_Id = DlcManager.GetHighestActiveDlcId();

            public Dictionary<int, List<string>> VanillaStarmapItems = new Dictionary<int, List<string>>();
            public int MaxStarmapDistance;

            public bool HasStarmapItem(string id, out StarmapItem item1)
            {
                if (id == null || id.Length == 0)
                {
                    item1 = null;
                    return false;
                }

                if (StarterPlanet != null && StarterPlanet.id == id)
                {
                    item1 = StarterPlanet;
                    return true;
                }
                else if (WarpPlanet != null && WarpPlanet.id == id)
                {
                    item1 = WarpPlanet;
                    return true;
                }
                else if (OuterPlanets.ContainsKey(id))
                {
                    item1 = OuterPlanets[id];
                    return true;
                }
                else if (POIs.ContainsKey(id))
                {
                    item1 = POIs[id];
                    return true;
                }

                if (PlanetoidDict.TryGetValue(id, out item1))
                {
                    return false;
                }
                return false;
            }

            public List<StarmapItem> GetAllPlanets()
            {
                var list = new List<StarmapItem>();
                if (StarterPlanet != null)
                    list.Add(StarterPlanet);
                if (WarpPlanet != null) list.Add(WarpPlanet);
                list.AddRange(OuterPlanets.Values);
                return list;
            }

            public List<string> GiveWorldTraitsForWorldGen(ProcGen.World world, int seed)
            {

                List<string> list = new List<string>();

                if (HasStarmapItem(world.filePath, out var starmapItem))
                {
                    list = starmapItem.GetWorldTraits();
                }
                else ///randomly selected planet
                {
                    int random = new System.Random(seed).Next(101);
                    int count = 0;
                    if (random >= 85)
                        count = 2;
                    else if (random >= 40)
                        count = 1;
                    List<string> randomSelectedTraits = new List<string>();

                    if (world.disableWorldTraits || world.worldTraitRules == null)
                    {
                        SgtLogger.l("worldtraits disabled, not rolling any random traits", Strings.Get(world.name));
                        return randomSelectedTraits;
                    }

                    return AddRandomTraitsForWorld(randomSelectedTraits, world, count, seed);
                }

                if (list.Any(id => id == ModAssets.CustomTraitID))
                {
                    int random = new System.Random(seed).Next(101);
                    int count = 1;
                    if (random >= 85)
                        count = 3;
                    else if (random >= 50)
                        count = 2;

                    List<string> randomSelectedTraits = new List<string>();


                    if (world.disableWorldTraits || world.worldTraitRules == null)
                        return randomSelectedTraits;

                    return AddRandomTraitsForWorld(randomSelectedTraits, world, count, seed);
                }


                return list;
            }
            public static List<string> AddRandomTraitsForWorld(List<string> existing, ProcGen.World world, int count, int seed)
            {
                SgtLogger.l($"rolling {count} random traits", Strings.Get(world.name));
                for (int i = 0; i < count; ++i)
                {
                    var possibleTraits = StarmapItem.AllowedWorldTraitsFor(existing, world)
                        .Where(item =>
                        item.filePath != ModAssets.CustomTraitID
                        //&& !item.filePath.ToUpperInvariant().Contains("SPACEHOLE")
                        && !RandomTraitInBlacklist(item.filePath));
                    if (possibleTraits.Count() == 0)
                        break;
                    else
                    {
                        possibleTraits = possibleTraits.Shuffle(new System.Random(seed));
                        string randTrait = possibleTraits.First().filePath == ModAssets.CustomTraitID ? possibleTraits.Last().filePath : possibleTraits.First().filePath;

                        if (randTrait != ModAssets.CustomTraitID)
                        {
                            existing.Add(randTrait);
                            SgtLogger.l(seed + " rolled " + randTrait, Strings.Get(world.name));
                        }
                        seed += 1;
                    }
                }
                return existing;
            }

            public void SetRings(int rings, bool defaultRing = false)
            {
                rings = Math.Min(rings, ringMax);
                rings = Math.Max(rings, ringMin);

                Rings = rings;
                if (defaultRing)
                    defaultRings = rings;

                if (StarterPlanet != null && StarterPlanet.placement != null)
                {
                    if (StarterPlanet.placement.allowedRings.max >= rings)
                    {
                        StarterPlanet.SetOuterRing(rings);
                    }
                    if (StarterPlanet.placement.allowedRings.min >= rings)
                    {
                        StarterPlanet.SetInnerRing(rings);
                    }
                }
                if (WarpPlanet != null && WarpPlanet.placement != null)
                {
                    if (WarpPlanet.placement.allowedRings.max >= rings)
                    {
                        WarpPlanet.SetOuterRing(rings);
                    }
                    if (WarpPlanet.placement.allowedRings.min >= rings)
                    {
                        WarpPlanet.SetInnerRing(rings);
                    }
                }

                foreach (var planet in OuterPlanets.Values)
                {
                    if (planet.placement != null)
                    {
                        if (planet.placement.allowedRings.max >= rings)
                        {
                            planet.SetOuterRing(rings);
                        }
                        if (planet.placement.allowedRings.min >= rings)
                        {
                            planet.SetInnerRing(rings);
                        }
                    }
                }
                foreach (var planet in POIs.Values)
                {
                    if (planet.placementPOI != null)
                    {
                        if (planet.placementPOI.allowedRings.max >= rings)
                        {
                            planet.SetOuterRing(rings);
                        }
                        if (planet.placementPOI.allowedRings.min >= rings)
                        {
                            planet.SetInnerRing(rings);
                        }
                    }
                }
                //if (RandomPOIStarmapItem != null)
                MaxPOICount = Math.Max(16, Mathf.RoundToInt((7.385f * ((float)rings)) - 56.615f));
            }

            public bool SomeStarmapitemsMissing(out List<string> missings)
            {
                missings = Db.Get().SpaceDestinationTypes.resources.Select(entry => entry.Id).ToList();

                foreach (var poiList in VanillaStarmapItems.Values)
                {
                    missings.RemoveAll(entry => poiList.Contains(entry));

                }
                return missings.Count > 0;
            }

            public int AddVanillaStarmapDistance()
            {
                VanillaStarmapItems[MaxStarmapDistance].RemoveAll(item => item == "Wormhole");

                VanillaStarmapItems[++MaxStarmapDistance] = new List<string>() { "Wormhole" };
                return MaxStarmapDistance;
            }
            public void RemoveFurthestVanillaStarmapDistance()
            {
                VanillaStarmapItems.Remove(MaxStarmapDistance);
                VanillaStarmapItems[--MaxStarmapDistance].Add("Wormhole");
            }
            public void RemoveVanillaPoi(Tuple<string, int> item) => RemoveVanillaPoi(item.first, item.second);
            public void RemoveVanillaPoi(string id, int range)
            {
                SgtLogger.l("removing " + id + " from " + range);
                VanillaStarmapItems[range].Remove(id);
            }
            public void AddVanillaPoi(string id, int range)
            {
                SgtLogger.l("adding " + id + " to " + range);
                VanillaStarmapItems[range].Add(id);
            }

            public void ResetVanillaStarmap()
            {
                if (DlcManager.IsExpansion1Active())
                    return;
                SgtLogger.l("Resetting vanilla starmap");
                VanillaStarmapItems.Clear();
                GenerateVanillaStarmapDestinations();
            }

            void PopulateVanillaStarmapLocations()
            {
                SpaceDestinationTypes destinationTypes = Db.Get().SpaceDestinationTypes;
                _vanillaSpawns = new List<List<string>>()
                {
                    new List<string>(),
                    new List<string>() { destinationTypes.OilyAsteroid.Id },
                    new List<string>() { destinationTypes.Satellite.Id },
                    new List<string>()
                    {
                        destinationTypes.Satellite.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.CarbonaceousAsteroid.Id,
                        destinationTypes.ForestPlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.CarbonaceousAsteroid.Id,
                        destinationTypes.SaltDwarf.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.CarbonaceousAsteroid.Id,
                        destinationTypes.IcyDwarf.Id,
                        destinationTypes.OrganicDwarf.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.IcyDwarf.Id,
                        destinationTypes.OrganicDwarf.Id,
                        destinationTypes.DustyMoon.Id,
                        destinationTypes.ChlorinePlanet.Id,
                        destinationTypes.RedDwarf.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.DustyMoon.Id,
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.VolcanoPlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.GasGiant.Id,
                        destinationTypes.IceGiant.Id,
                        destinationTypes.RustPlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.GasGiant.Id,
                        destinationTypes.IceGiant.Id,
                        destinationTypes.HydrogenGiant.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.RustPlanet.Id,
                        destinationTypes.VolcanoPlanet.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.MetallicAsteroid.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.ShinyPlanet.Id,
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.RockyAsteroid.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.GoldAsteroid.Id,
                        destinationTypes.OrganicDwarf.Id,
                        destinationTypes.ForestPlanet.Id,
                        destinationTypes.ChlorinePlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.IcyDwarf.Id,
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.DustyMoon.Id,
                        destinationTypes.VolcanoPlanet.Id,
                        destinationTypes.IceGiant.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.ShinyPlanet.Id,
                        destinationTypes.RedDwarf.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.GasGiant.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.HydrogenGiant.Id,
                        destinationTypes.ForestPlanet.Id,
                        destinationTypes.OilyAsteroid.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.GoldAsteroid.Id,
                        destinationTypes.SaltDwarf.Id,
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.VolcanoPlanet.Id
                    }
                };

                _possibleVanillaStarmapLocations = new Dictionary<string, List<int>>();

                for (int distance = 0; distance < _vanillaSpawns.Count; distance++)
                {
                    foreach (var planet in _vanillaSpawns[distance])
                    {
                        if (!_possibleVanillaStarmapLocations.ContainsKey(planet))
                            _possibleVanillaStarmapLocations.Add(planet, new List<int>());
                        _possibleVanillaStarmapLocations[planet].Add(distance);
                    }
                }
            }


            [JsonIgnore]
            List<List<string>> _vanillaSpawns = null;

            [JsonIgnore]
            List<List<string>> VanillaSpawns
            {
                get
                {
                    if (_vanillaSpawns == null)
                        PopulateVanillaStarmapLocations();
                    return _vanillaSpawns;
                }
            }
            [JsonIgnore]
            Dictionary<string, List<int>> PossibleVanillaStarmapLocations
            {
                get
                {
                    if (_possibleVanillaStarmapLocations == null)
                        PopulateVanillaStarmapLocations();

                    return _possibleVanillaStarmapLocations;
                }
            }
            [JsonIgnore]
            Dictionary<string, List<int>> _possibleVanillaStarmapLocations = null;

            public void AddMissingStarmapItems()
            {
                if (SomeStarmapitemsMissing(out var missingIds))
                {
                    string setting = selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
                    int seed = int.Parse(setting);
                    SgtLogger.l(setting, "seed");
                    var random = new System.Random(seed);
                    foreach (string planetId in missingIds)
                    {
                        List<int> possibleLocations = PossibleVanillaStarmapLocations.ContainsKey(planetId)
                            ? PossibleVanillaStarmapLocations[planetId]
                            : MaxStarmapDistance > 5
                                ? Enumerable.Range(4, MaxStarmapDistance - 5).ToList()
                                : new List<int>() { 0 };

                        possibleLocations = possibleLocations.Shuffle(random).ToList();
                        int distance = possibleLocations.First();
                        SgtLogger.l(planetId + ": " + distance, "adding missing," + PossibleVanillaStarmapLocations.ContainsKey(planetId));
                        AddVanillaPoi(planetId, distance);
                    }
                }
            }
            ///<summary>
            /// copied from SpaceCraftManager.GenerateFixedDestinations and SpaceCraftManager.GenerateRandomDestinations.
            /// required since those methods require the savegame seed and arent returning anything
            /// </summary>
            void GenerateVanillaStarmapDestinations()
            {

                string setting = selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
                int seed = int.Parse(setting);
                SpaceDestinationTypes destinationTypes = Db.Get().SpaceDestinationTypes;

                List<Tuple<string, int>> destinationsWithDistance = new List<Tuple<string, int>>();
                ///Fixed Items:
                destinationsWithDistance.Add(new(destinationTypes.CarbonaceousAsteroid.Id, 0));
                destinationsWithDistance.Add(new(destinationTypes.CarbonaceousAsteroid.Id, 0));
                destinationsWithDistance.Add(new(destinationTypes.MetallicAsteroid.Id, 1));
                destinationsWithDistance.Add(new(destinationTypes.RockyAsteroid.Id, 2));
                destinationsWithDistance.Add(new(destinationTypes.IcyDwarf.Id, 3));
                destinationsWithDistance.Add(new(destinationTypes.OrganicDwarf.Id, 4));

                destinationsWithDistance.Add(new(destinationTypes.Earth.Id, 4));
                ///Random Items:
                KRandom krandom = new KRandom(seed);


                List<int> intList = new List<int>();
                int num1 = 3;
                int minValue = 15;
                int maxValue = 25;
                for (int index1 = 0; index1 < VanillaSpawns.Count; ++index1)
                {
                    if (VanillaSpawns[index1].Count != 0)
                    {
                        for (int index2 = 0; index2 < num1; ++index2)
                            intList.Add(index1);
                    }
                }
                int num2 = krandom.Next(minValue, maxValue);
                for (int index3 = 0; index3 < num2; ++index3)
                {
                    int index4 = krandom.Next(0, intList.Count - 1);
                    int num3 = intList[index4];
                    intList.RemoveAt(index4);
                    List<string> stringList = VanillaSpawns[num3];
                    destinationsWithDistance.Add(new(stringList[krandom.Next(0, stringList.Count)], num3));
                }

                destinationsWithDistance.Add(new(destinationTypes.Wormhole.Id, VanillaSpawns.Count));
                MaxStarmapDistance = VanillaSpawns.Count;

                for (int distance = 0; distance <= MaxStarmapDistance; distance++)
                {
                    VanillaStarmapItems[distance] = new List<string>();
                }

                foreach (var entry in destinationsWithDistance)
                {
                    VanillaStarmapItems[entry.second].Add(entry.first);
                }
            }

            public void RemovePoiGroup(string key)
            {
                if (POIs.ContainsKey(key))
                    POIs.Remove(key);
            }

            internal StarmapItem AddNewPoiGroupFromPOI(string startPoiId)
            {
                return AddLegacyPOIGroup(startPoiId, 0, CustomCluster.Rings, 1);
            }

            internal StarmapItem AddPoiGroup(string key, SpaceMapPOIPlacement spaceMapPOIPlacement, float numberToSpawn)
            {
                StarmapItem item = new StarmapItem(key, StarmapItemCategory.POI, null);
                item.MakeItemPOI(spaceMapPOIPlacement);
                item.SetSpawnNumber(numberToSpawn, true);
                POIs[key] = item;
                return item;
            }

            internal StarmapItem AddLegacyPOIGroup(string key, int minRing, int maxRing, float numberToSpawn)
            {
                SpaceMapPOIPlacement placement = new SpaceMapPOIPlacement()
                {
                    allowedRings = new MinMaxI(minRing, maxRing),
                    pois = new List<string> { key },
                    numToSpawn = 1,
                    avoidClumping = false,
                    canSpawnDuplicates = false
                };
                return AddPoiGroup(GetPOIGroupId(placement, true), placement, numberToSpawn);
            }
        }

        public enum WorldSizePresets
        {
            Tiny = 30,
            Smaller = 45,
            Small = 60,
            SlightlySmaller = 80,

            Custom = -1,
            Normal = 100,
            SlightlyLarger = 125,
            Large = 150,
            Huge = 200,
            Massive = 300,
            Enormous = 400
        }
        public enum WorldRatioPresets
        {
            Custom = -1,
            LotWider = 70,
            Wider = 80,
            SlightlyWider = 90,
            Normal = 100,
            SlightlyTaller = 110,
            Taller = 120,
            LotTaller = 130
        }


        public class StarmapItem
        {
            public string id;
            public StarmapItemCategory category;
            public bool DisablesStoryTraits = false;
            [JsonIgnore] public bool originalPOIGroup => POIGroupUID == string.Empty;

            public string POIGroupUID = string.Empty;

            [JsonIgnore] public Sprite planetSprite;

            [JsonIgnore] public ProcGen.World world;
            [JsonIgnore] public Vector2I originalWorldDimensions;

            public WorldPlacement placement;

            public SpaceMapPOIPlacement placementPOI;

            public bool IsPOI => category == StarmapItemCategory.POI;
            public bool IsRandom => id.Contains(RandomKey);

            public int PredefinedPlacementOrder = -1;

            public string DisplayName
            {
                get
                {
                    if (id.Contains(RandomKey))
                    {
                        switch (category)
                        {
                            case StarmapItemCategory.Starter:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_STARTER.NAME;
                            case StarmapItemCategory.Warp:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_WARP.NAME;
                            case StarmapItemCategory.Outer:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_OUTER.NAME;
                            case StarmapItemCategory.POI:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.NAME;
                        }
                    }

                    if (world != null && world.name != null)
                    {
                        if (Strings.TryGet(world.name, out var nameEntry))
                        {
                            var name = nameEntry.ToString();
                            if (id.ToUpperInvariant().Contains("EVERYTHING"))
                                name += "-Everything";

                            return name;
                        }
                    }
                    else if (category == StarmapItemCategory.POI)
                    {
                        return id.Substring(0, 8);
                    }

                    //else if (_poiID != null)
                    //{
                    //    return _poiName;
                    //}
                    return id;
                }
            }
            public string DisplayDescription
            {
                get
                {

                    if (id.Contains(RandomKey))
                    {
                        switch (category)
                        {
                            case StarmapItemCategory.Starter:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_STARTER.DESCRIPTION;
                            case StarmapItemCategory.Warp:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_WARP.DESCRIPTION;
                            case StarmapItemCategory.Outer:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_OUTER.DESCRIPTION;
                            case StarmapItemCategory.POI:
                                return STRINGS.UI.SPACEDESTINATIONS.CGM_RANDOM_POI.DESCRIPTION;
                        }
                    }

                    if (world != null && world.description != null)
                    {
                        if (Strings.TryGet(world.description, out var description))
                            return description.String;
                    }
                    //else if (_poiID != null)
                    //{
                    //    return _poiDesc;
                    //}
                    return id;
                }
            }

            //private float XYratio = -1f;
            public float ApplySizeMultiplierToValue(float inputValue)
            {
                return (CurrentSizeMultiplier * inputValue);
            }

            [JsonIgnore]
            public Vector2I CustomPlanetDimensions
            {
                get
                {
                    if (UsingCustomDimensions)
                    {
                        return new(CustomX, CustomY);
                    }
                    var dim = new Vector2I(0, 0);

                    if (world != null)
                    {
                        float sizePercentage = (float)SizePreset / 100f;
                        float ratioModifier = 0;

                        bool? ChooseHeight = null;

                        int intRatio = (int)RatioPreset;
                        if (intRatio != 100)
                        {
                            if (intRatio < 100)
                            {
                                ratioModifier = 1f - ((float)RatioPreset / 100f);
                                ChooseHeight = false;
                            }
                            else if (intRatio > 100)
                            {
                                ratioModifier = ((float)RatioPreset / 100f) - 1;
                                ChooseHeight = true;
                            }
                        }
                        float sizeIncreaseMultiplier = Mathf.Sqrt(sizePercentage);

                        dim.X = Mathf.RoundToInt(originalWorldDimensions.X * sizeIncreaseMultiplier);
                        dim.Y = Mathf.RoundToInt(originalWorldDimensions.Y * sizeIncreaseMultiplier);

                        if (ChooseHeight == true)
                        {
                            dim.Y = dim.Y + Mathf.RoundToInt(dim.Y * ratioModifier);
                        }
                        if (ChooseHeight == false)
                        {
                            dim.X = dim.X + Mathf.RoundToInt(dim.X * ratioModifier);
                        }
                    }
                    return dim;
                }
            }

            //int CustomWorldSizeX = -1, CustomWorldSizeY = -1;
            WorldSizePresets SizePreset = WorldSizePresets.Normal;
            WorldRatioPresets RatioPreset = WorldRatioPresets.Normal;

            [JsonIgnore] public WorldSizePresets CurrentSizePreset => SizePreset;
            [JsonIgnore] public WorldRatioPresets CurrentRatioPreset => RatioPreset;

            [JsonIgnore] public float CurrentSizeMultiplier => UsingCustomDimensions ? CustomSizeIncrease : (float)SizePreset / 100f;

            public void SetPlanetSizeToPreset(WorldSizePresets preset)
            {
                CustomX = -1;
                CustomY = -1;
                UsingCustomDimensions = false;
                SizePreset = preset;
            }
            public void SetPlanetRatioToPreset(WorldRatioPresets preset)
            {
                CustomX = -1;
                CustomY = -1;
                UsingCustomDimensions = false;
                RatioPreset = preset;
            }
            public int CustomX = -1, CustomY = -1;
            private bool UsingCustomDimensions = false;
            public void ApplyCustomDimension(int value, bool heightTrueWidthFalse)
            {

                var currentDims = CustomPlanetDimensions;
                UsingCustomDimensions = true;
                if (CustomX == -1)
                {
                    CustomX = currentDims.X;
                }
                if (CustomY == -1)
                {
                    CustomY = currentDims.Y;
                }

                if (world != null)
                {
                    if (heightTrueWidthFalse)
                    {
                        var rounded = Mathf.RoundToInt(Mathf.Max(Mathf.Min(value, originalWorldDimensions.Y * 2.6f), originalWorldDimensions.Y * 0.55f));
                        if (rounded != CustomY)
                            CustomY = rounded;
                    }
                    else
                    {
                        var rounded = Mathf.RoundToInt(Mathf.Max(Mathf.Min(value, originalWorldDimensions.X * 2.6f), originalWorldDimensions.X * 0.55f));

                        if (rounded != CustomX)
                            CustomX = rounded;
                    }
                    CustomSizeIncrease = (float)(CustomX * CustomY) / (float)(originalWorldDimensions.X * originalWorldDimensions.Y);
                }
            }
            public float SizeMultiplierX()
            {
                if (UsingCustomDimensions && world != null)
                {
                    return CustomX / originalWorldDimensions.X;
                }
                else
                {
                    float sizePercentage = (float)SizePreset / 100f;
                    return Mathf.Sqrt(sizePercentage);
                }

            }
            public float SizeMultiplierY()
            {
                if (UsingCustomDimensions && world != null)
                {
                    return CustomY / originalWorldDimensions.Y;
                }
                else
                {
                    float sizePercentage = (float)SizePreset / 100f;
                    return Mathf.Sqrt(sizePercentage);
                }
            }


            float CustomSizeIncrease = -1f;

            public float InstancesToSpawn = 1;
            public bool MoreThanOnePossible = false;
            //public float MaxNumberOfInstances = 1;
            [JsonIgnore] public int minRing => placement != null ? placement.allowedRings.min : placementPOI != null ? placementPOI.allowedRings.min : -1;
            [JsonIgnore] public int maxRing => placement != null ? placement.allowedRings.max : placementPOI != null ? placementPOI.allowedRings.max : -1;
            [JsonIgnore] public int buffer => placement != null ? placement.buffer : -1;


            #region SetterMethods

            public void RefresDuplicateState()
            {
                if (placementPOI != null && InstancesToSpawn < placementPOI.pois.Count)
                {
                    placementPOI.canSpawnDuplicates = true;
                }
            }

            public void SetSpawnNumber(float newNumber, bool force = false)
            {
                if (newNumber < 0)
                    newNumber = 0;
                if (this.IsPOI)
                {
                    if (newNumber <= MaxPOICount || force)
                    {
                        InstancesToSpawn = newNumber;
                    }
                    else
                        InstancesToSpawn = MaxPOICount;

                    RefresDuplicateState();
                }
                else
                {
                    if (newNumber <= MaxAmountRandomPlanet || force)
                    {
                        InstancesToSpawn = newNumber;
                    }
                    else
                        InstancesToSpawn = MaxAmountRandomPlanet;
                }
            }
            public void ResetPOI()
            {
                if (this.category == StarmapItemCategory.POI)
                {
                    if (originalMaxPOI != null)
                    {
                        SetOuterRing((int)originalMaxPOI);
                    }
                    if (originalMinPOI != null)
                    {
                        SetInnerRing((int)originalMinPOI);
                    }
                }
            }

            [JsonIgnore] public int? originalMinPOI = null, originalMaxPOI = null;
            public void SetInnerRing(int newRing, bool original = false)
            {
                if (original)
                    originalMinPOI = newRing;

                newRing = Math.Min(newRing, CustomCluster.Rings);
                if (newRing >= 0)
                {
                    if (placement != null)
                    {
                        var rings = placement.allowedRings;
                        rings.min = newRing;
                        if (newRing > rings.max)
                        {
                            rings.max = newRing;
                        }
                        placement.allowedRings = rings;
                    }
                    else if (placementPOI != null)
                    {
                        var rings = placementPOI.allowedRings;
                        rings.min = newRing;
                        if (newRing > rings.max)
                        {
                            rings.max = newRing;
                        }
                        placementPOI.allowedRings = rings;
                    }
                    else
                        SgtLogger.warning(this.id + ": no placement component found!");
                }
            }
            public void SetOuterRing(int newRing, bool original = false)
            {
                newRing = Math.Min(newRing, CustomCluster.Rings);
                if (original)
                    originalMaxPOI = newRing;
                if (newRing >= 0)
                {
                    if (placement != null)
                    {
                        var rings = placement.allowedRings;
                        rings.max = newRing;
                        if (newRing < rings.min)
                        {
                            rings.min = newRing;
                        }
                        placement.allowedRings = rings;
                    }
                    else if (placementPOI != null)
                    {
                        var rings = placementPOI.allowedRings;
                        rings.max = newRing;
                        if (newRing < rings.min)
                        {
                            rings.min = newRing;
                        }
                        placementPOI.allowedRings = rings;
                    }
                    else
                        SgtLogger.warning(this.id + ": no placement component found!");
                }
            }

            public void SetBuffer(int newBuffer)
            {
                newBuffer = Math.Min(newBuffer, CustomCluster.Rings);
                if (newBuffer >= 0)
                {
                    if (placement != null)
                    {
                        //var buffer = placement.buffer;
                        //rings.max = newRing;
                        //if (newRing < rings.min)
                        //{
                        //    rings.min = newRing;
                        //}
                        placement.buffer = newBuffer;
                    }
                    else
                        SgtLogger.warning(this.id + ": no placement component found!");
                }
            }

            #endregion

            public StarmapItem(string id, StarmapItemCategory category, Sprite sprite)
            {
                this.id = id;
                this.category = category;
                this.planetSprite = sprite;
            }
            public StarmapItem MakeItemPlanet(ProcGen.World world)
            {
                this.world = world;
                this.originalWorldDimensions = world.worldsize;
                //this.InitGeyserInfo();

                //XYratio = (float)world.worldsize.X / (float)world.worldsize.Y;
                return this;
            }

            //private void InitGeyserInfo()
            //{
            //    var Templates = this.world.worldTemplateRules;
            //    foreach (var Template in Templates)
            //    {

            //    }
            //}
            //[JsonIgnore]public List<GeyserInfo> GeyserInfos;
            //public class GeyserInfo
            //{
            //    public Sprite PreviewImage;
            //    public string Name, Description;
            //    public int minCount, maxCount;
            //}


            public StarmapItem AddItemWorldPlacement(WorldPlacement placement2, bool morethanone = false)
            {
                //this.MaxNumberOfInstances = morethanone;
                this.MoreThanOnePossible = morethanone;
                this.placement = new WorldPlacement();
                placement.startWorld = placement2.startWorld;
                placement.world = placement2.world;
                placement.y = placement2.y;
                placement.x = placement2.x;
                placement.height = placement2.height;
                placement.width = placement2.width;
                placement.allowedRings = new(placement2.allowedRings.min, placement2.allowedRings.max);
                placement.buffer = placement2.buffer;
                placement.locationType = placement2.locationType;
                return this;
            }
            public StarmapItem MakeItemPOI(SpaceMapPOIPlacement placement2)
            {
                MoreThanOnePossible = true;
                placementPOI = new SpaceMapPOIPlacement();
                placementPOI.pois = new(placement2.pois);
                placementPOI.canSpawnDuplicates = placement2.canSpawnDuplicates;
                placementPOI.avoidClumping = placement2.avoidClumping;
                placementPOI.numToSpawn = placement2.numToSpawn;
                placementPOI.allowedRings = new(placement2.allowedRings.min, placement2.allowedRings.max);
                originalMaxPOI = placement2.allowedRings.max;
                originalMinPOI = placement2.allowedRings.min;
                //MaxNumberOfInstances = placement2.numToSpawn * 5f; 
                InstancesToSpawn = placement2.numToSpawn;
                return this;
            }


            #region PlanetMeteors

            public List<MeteorShowerSeason> CurrentMeteorSeasons
            {
                get
                {
                    var seasons = new List<MeteorShowerSeason>();
                    if (world != null)
                    {
                        var db = Db.Get();
                        foreach (var season in world.seasons)
                        {
                            var seasonData = (db.GameplaySeasons.TryGet(season) as MeteorShowerSeason);
                            if (seasonData != null)
                                seasons.Add(seasonData);
                        }
                    }
                    return seasons;
                }
            }

            public List<MeteorShowerEvent> CurrentMeteorShowerTypes
            {
                get
                {
                    var showers = new List<MeteorShowerEvent>();
                    if (world != null)
                    {
                        var db = Db.Get();
                        foreach (var season in world.seasons)
                        {
                            var seasonData = (db.GameplaySeasons.TryGet(season) as MeteorShowerSeason);
                            if (seasonData != null)
                            {
                                foreach (var shower in seasonData.events)
                                {
                                    var showerEvent = shower as MeteorShowerEvent;
                                    if (!showers.Contains(showerEvent))
                                        showers.Add(showerEvent);

                                }

                            }
                        }
                    }
                    return showers;
                }
            }

            private void BackupOriginalWorldTraits()
            {
                if (world != null)
                {
                    if (!ModAssets.ChangedMeteorSeasons.ContainsKey(world))
                    {
                        ModAssets.ChangedMeteorSeasons[world] = new List<string>(world.seasons);
                    }
                }
            }

            public void AddMeteorSeason(string id)
            {
                BackupOriginalWorldTraits();
                if (world != null)
                {
                    world.seasons.Add(id);
                }
            }

            public void RemoveMeteorSeason(string id)
            {
                BackupOriginalWorldTraits();
                if (world != null)
                {
                    if (world.seasons.Contains(id))
                    {
                        world.seasons.Remove(id);
                    }
                }
            }




            #endregion
            #region PlanetTraits

            private List<string> currentPlanetTraits = new List<string>();
            [JsonIgnore] public List<string> CurrentTraits => currentPlanetTraits;
            public void SetWorldTraits(List<string> NEWs) => currentPlanetTraits = NEWs;


            public static List<WorldTrait> AllowedWorldTraitsFor(List<string> currentTraits, ProcGen.World world)
            {
                List<WorldTrait> AllTraits = ModAssets.AllTraitsWithRandomValuesOnly;

                if (world == null)
                {
                    return new List<WorldTrait>();
                }
                List<string> ExclusiveWithTags
                    = new List<string>();

                if (currentTraits.Count > 0 || (world != null && world.disableWorldTraits))
                {
                    AllTraits.RemoveAll((WorldTrait trait) => trait.filePath == ModAssets.CustomTraitID);
                }


                foreach (var trait in currentTraits)
                {
                    if (SettingsCache.worldTraits.ContainsKey(trait))
                    {
                        ExclusiveWithTags.AddRange(SettingsCache.worldTraits[trait].exclusiveWithTags);
                    }
                    if (trait == ModAssets.CustomTraitID)
                        return new List<WorldTrait>();
                }

                foreach (ProcGen.World.TraitRule rule in world.worldTraitRules)
                {

                    TagSet requiredTags = ((rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null);
                    TagSet forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);

                    AllTraits.RemoveAll((WorldTrait trait) =>
                        (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags))
                        || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags))
                        || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.filePath))
                        || !trait.IsValid(world, logErrors: true));

                }
                AllTraits.RemoveAll((WorldTrait trait) =>
                     !trait.IsValid(world, logErrors: true)
                    || trait.exclusiveWithTags.Any(x => ExclusiveWithTags.Any(y => y == x))
                    || currentTraits.Contains(trait.filePath)
                    || trait.exclusiveWith.Any(x => currentTraits.Any(y => y == x))
                    );


                return AllTraits;

            }
            [JsonIgnore] public List<WorldTrait> AllowedPlanetTraits => AllowedWorldTraitsFor(currentPlanetTraits, world);

            public bool RemoveWorldTrait(WorldTrait trait)
            {
                //SgtLogger.l(trait.filePath, "TryingToRemove");

                string traitID = trait.filePath;
                bool allowed = currentPlanetTraits.Contains(traitID);
                if (allowed)
                    currentPlanetTraits.Remove(traitID);
                return allowed;
            }

            public bool AddWorldTrait(WorldTrait trait)
            {
                string traitID = trait.filePath;
                bool allowed = !currentPlanetTraits.Contains(traitID);
                if (allowed)
                    currentPlanetTraits.Add(traitID);
                return allowed;
            }
            public void ClearWorldTraits()
            {
                currentPlanetTraits.Clear();
            }
            public List<string> GetWorldTraits()
            {
                return currentPlanetTraits;
            }
            #endregion
        }

        public const string CustomClusterIDCoordinate = "CGM";
        public const string CustomClusterID = "expansion1::clusters/CGMCluster";
        public static ClusterLayout GeneratedLayout => GenerateClusterLayoutFromCustomData(false);
        public static CustomClusterData CustomCluster;


        public static void AddCustomClusterAndInitializeClusterGen()
        {
            LoadCustomCluster = true;
            var GeneratedCustomCluster = GenerateClusterLayoutFromCustomData(true);
            SettingsCache.clusterLayouts.clusterCache[CustomClusterID] = GeneratedCustomCluster;

            CustomGameSettings.Instance.SetQualitySetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout, GeneratedCustomCluster.filePath);
            //SgtLogger.l(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout).id.ToString());
            CGSMClusterManager.selectScreen.LaunchClicked();
        }

        //static void ApplySizeMultiplier(WorldPlacement placement, float multiplier)
        //{
        //    float min = placement.allowedRings.min, max = placement.allowedRings.max;
        //    //min*= multiplier;

        //    //if (max < 3)
        //        //max = 3;

        //    max *= multiplier;
        //    max = Math.Min(max, CustomCluster.Rings);

        //    int max2 = Math.Min(placement.allowedRings.max + CustomCluster.AdjustedOuterExpansion, CustomCluster.Rings);
        //    int newMax = Math.Max((int)Math.Round(max), max2);
        //    placement.allowedRings = new MinMaxI((int)min, newMax);

        //    SgtLogger.l("Set inner and outer limits to " + placement.allowedRings.ToString(), placement.world);
        //}

        static void ApplySizeMultiplier(SpaceMapPOIPlacement placement, float multiplier)
        {
            float min = placement.allowedRings.min, max = placement.allowedRings.max;
            //min*= multiplier;x
            max *= multiplier;

            int max2 = placement.allowedRings.max + CustomCluster.AdjustedOuterExpansion;
            int newMax = Math.Max((int)Math.Round(max), max2);
            placement.allowedRings = new MinMaxI((int)min, newMax);
        }

        public static bool PlanetIsClassic(StarmapItem item)
        {
            if (item.IsPOI)
                return false;

            if (item.id.Contains("Vanilla") || item.world != null && item.originalWorldDimensions.x * item.originalWorldDimensions.y > 90000)
                return true;

            return false;
        }
        public static bool PlanetIsMiniBase(StarmapItem item)
        {
            if (item.IsPOI)
                return false;

            string KeyUpper = item.id.ToUpperInvariant();

            return PlanetByIdIsMiniBase(KeyUpper);
        }
        public static bool PlanetByIdIsMiniBase(string KeyUpper)
        {
            KeyUpper = KeyUpper.ToUpperInvariant();
            return (KeyUpper.Contains("BABY") || KeyUpper.Contains("MINIBASE"));
        }

        ///Random Asteroids stay random in preview

        public static ClusterLayout GenerateClusterLayoutFromCustomData(bool log)
        {
            SgtLogger.l("Started generating custom cluster layout");
            var layout = new ClusterLayout();
            CurrentClassicOuterPlanets = 0;


            string setting = selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
            int seed = int.Parse(setting);
            if (log)
                SgtLogger.l(setting, "CurrentSeed");
            if (log)
                SgtLogger.l(MaxClassicOuterPlanets.ToString(), "Max. allowed classic sized");

            //var Reference = SettingsCache.clusterLayouts.GetClusterData(ClusterID);
            //SgtLogger.log(Reference.ToString());
            layout.filePath = CustomClusterID;
            layout.name = "STRINGS.CLUSTER_NAMES.CGM.NAME";
            layout.description = "STRINGS.CLUSTER_NAMES.CGM.DESCRIPTION";
            layout.worldPlacements = new List<WorldPlacement>();
            layout.coordinatePrefix = CustomClusterIDCoordinate;

            if (DlcManager.IsExpansion1Active())
                layout.requiredDlcId = DlcManager.EXPANSION1_ID;
            else
                layout.forbiddenDlcId = DlcManager.EXPANSION1_ID;

            float multiplier = 1f;
            if (CustomCluster.Rings > CustomCluster.defaultRings)
            {
                multiplier = (float)CustomCluster.Rings / (float)CustomCluster.defaultRings;
            }
            if (log)
                SgtLogger.l("Cluster Size: " + (CustomCluster.Rings + 1));
            if (log)
                SgtLogger.l("Placement Multiplier: " + multiplier);

            if (CustomCluster.StarterPlanet != null)
            {

                if (PlanetIsClassic(CustomCluster.StarterPlanet))
                    CurrentClassicOuterPlanets++;

                ///Disabling Story Traits if MiniBase worlds are active
                if (CustomCluster.StarterPlanet.DisablesStoryTraits)
                    layout.disableStoryTraits = true;


                if (CustomCluster.StarterPlanet.id.Contains(RandomKey))
                {
                    var randomItem = GetRandomItemOfType(StarmapItemCategory.Starter, seed);
                    var placement = GivePrefilledItem(randomItem).placement;

                    placement.allowedRings = CustomCluster.StarterPlanet.placement.allowedRings;
                    placement.buffer = CustomCluster.StarterPlanet.placement.buffer;

                    placement.startWorld = true;

                    layout.worldPlacements.Add(placement);
                    if (log)
                        SgtLogger.l(randomItem.id, "Random Start Planet");

                }
                else
                {
                    CustomCluster.StarterPlanet.placement.startWorld = true;
                    layout.worldPlacements.Add(CustomCluster.StarterPlanet.placement);
                    if (log)
                        SgtLogger.l(CustomCluster.StarterPlanet.id, "Start Planet");
                }
                seed++;
            }
            else
                SgtLogger.warning("No start planet selected");


            if (CustomCluster.WarpPlanet != null)
            {
                if (PlanetIsClassic(CustomCluster.WarpPlanet))
                    CurrentClassicOuterPlanets++;
                ///Disabling Story Traits if MiniBase worlds are active
                if (CustomCluster.WarpPlanet.DisablesStoryTraits)
                    layout.disableStoryTraits = true;

                //if (CustomCluster.StarterPlanet.placement.allowedRings.max == 0
                //    && CustomCluster.WarpPlanet.placement.allowedRings.max < CustomCluster.StarterPlanet.placement.buffer)
                //{
                //    var vector = CustomCluster.WarpPlanet.placement.allowedRings;
                //    CustomCluster.WarpPlanet.placement.allowedRings = new MinMaxI(vector.min, CustomCluster.StarterPlanet.placement.buffer + 1);
                //}
                if (CustomCluster.WarpPlanet.id.Contains(RandomKey))
                {
                    var randomItem = GetRandomItemOfType(StarmapItemCategory.Warp, seed);
                    var placement = GivePrefilledItem(randomItem).placement;

                    placement.allowedRings = CustomCluster.WarpPlanet.placement.allowedRings;
                    placement.buffer = CustomCluster.WarpPlanet.placement.buffer;

                    layout.worldPlacements.Add(placement);
                    if (log)
                        SgtLogger.l(randomItem.id, "Random Warp Planet");
                }
                else
                {
                    layout.worldPlacements.Add(CustomCluster.WarpPlanet.placement);
                    if (log)
                        SgtLogger.l(CustomCluster.WarpPlanet.id, "Warp Planet");
                }
                seed++;
            }
            else
            {
                if (DlcManager.IsExpansion1Active())
                {
                    if (log)
                        SgtLogger.log("No warp planet selected");
                    CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.Teleporters, (CustomGameSettingConfigs.Teleporters as ToggleSettingConfig).off_level.id);
                }
            }
            ///STRINGS.NAMEGEN.WORLD.ROOTS

            RandomOuterPlanets.Clear();
            List<StarmapItem> OuterPlanets = CustomCluster.OuterPlanets.Values.ToList();
            if (OuterPlanets.Count > 0)
            {
                if (log)
                    SgtLogger.l(OuterPlanets.Count + " outer planets selected");
                OuterPlanets =
                    OuterPlanets.OrderBy(item => item.placement.allowedRings.max)
                    .ThenBy(item => (item.placement.allowedRings.max - item.placement.allowedRings.min))
                    .ThenBy(item => (item.placement.world))
                    .ToList();

                foreach (var world in OuterPlanets)
                {
                    ///Disabling Story Traits if MiniBase worlds are active
                    if (world.DisablesStoryTraits)
                        layout.disableStoryTraits = true;

                    if (world.id.Contains(RandomKey))
                    {
                        if (log)
                            SgtLogger.l(world.InstancesToSpawn.ToString(), "Random Planets to select");
                        for (int i = 0; i < (int)world.InstancesToSpawn; i++)
                        {
                            var randomItem = GetRandomItemOfType(StarmapItemCategory.Outer, seed);
                            seed++;

                            if (randomItem == null)
                            {

                                if (log)
                                    SgtLogger.l("Failed to get unused", "Random Outer Planet");
                                break;
                            }

                            var placement = GivePrefilledItem(randomItem).placement;
                            placement.allowedRings = world.placement.allowedRings;
                            placement.buffer = world.placement.buffer;

                            //ApplySizeMultiplier(placement, multiplier);
                            layout.worldPlacements.Add(placement);

                            if (log)
                                SgtLogger.l(randomItem.id, "selected random outer Planet");
                        }
                    }
                    else
                    {
                        var placement = world.placement;
                        //ApplySizeMultiplier(placement, multiplier);
                        layout.worldPlacements.Add(placement);
                        if (log)
                            SgtLogger.l(world.id, "Outer Planet");
                        seed++;
                    }
                }
            }

            //layout.worldPlacements = layout.worldPlacements.OrderBy(item => item.allowedRings.max).ThenBy(item => item.allowedRings.min).ToList();
            //layout.startWorldIndex = layout.worldPlacements.FindIndex(placement => placement.startWorld == true);

            if (log)
                SgtLogger.l("Planet Placements done");
            layout.poiPlacements = new List<SpaceMapPOIPlacement>();

            seed = int.Parse(setting) + 100;
            foreach (var poi in CustomCluster.POIs)
            {
                var radomns = poi.Value.placementPOI.pois.Any(i => i.Contains(RandomKey));

                //poi.Value.placementPOI.pois.RemoveAll(i => i.Contains(RandomKey));

                poi.Value.placementPOI.numToSpawn = Mathf.FloorToInt(poi.Value.InstancesToSpawn);
                float percentageAdditional = poi.Value.InstancesToSpawn % 1f;
                if (percentageAdditional > 0)
                {
                    float rolledChance = ((float)new System.Random(seed).Next(1, 101)) / 100f;
                    if (rolledChance < percentageAdditional)
                    {
                        if (log)
                            SgtLogger.l(poi.Value.id + ", succeeded: " + rolledChance * 100f, "POI Chance: " + percentageAdditional.ToString("P"));
                        poi.Value.placementPOI.numToSpawn += 1;
                    }
                    else
                    {
                        if (log)
                            SgtLogger.l(poi.Value.id + ", failed: " + rolledChance * 100f, "POI Chance: " + percentageAdditional.ToString("P"));
                    }
                    seed++;
                }
                if (radomns)
                {
                    //for (int i = 0; i < poi.Value.placementPOI.numToSpawn; i++)
                    //{
                    //    string randomId = GetRandomPOI(seed);
                    //    if (randomId.Length > 0)
                    //        poi.Value.placementPOI.pois.Add(randomId);
                    //    seed++;
                    //}
                    poi.Value.placementPOI.canSpawnDuplicates = true;
                }
                if (log)
                    poi.Value.placementPOI.pois.ForEach(poi => SgtLogger.l(poi, "poi in group"));

                if (log)
                    SgtLogger.l($"\navoidClumping: {poi.Value.placementPOI.avoidClumping},\nallowDuplicates: {poi.Value.placementPOI.canSpawnDuplicates},\nRings: {poi.Value.placementPOI.allowedRings.ToString()}\nNumberToSpawn: {poi.Value.placementPOI.numToSpawn}", "POIGroup " + poi.Key.Substring(0, 8));


                layout.poiPlacements.Add(poi.Value.placementPOI);
            }

            if (log)
                SgtLogger.l("POI Placements done");
            layout.numRings = CustomCluster.Rings + 1;

            if (log)
                SgtLogger.l("Ordering Asteroids");

            foreach (var item in CustomCluster.GetAllPlanets())
            {
                SgtLogger.l(item.PredefinedPlacementOrder.ToString(), item.id);
            }

            if (CustomCluster.GetAllPlanets().All(item => item.PredefinedPlacementOrder != -1))
            {
                if (log)
                    SgtLogger.l("vanilla cluster loadout, using predefined order");
                layout.worldPlacements =
                    layout.worldPlacements.OrderBy(placement =>
                    {
                        CustomCluster.HasStarmapItem(placement.world, out var item);
                        return item.PredefinedPlacementOrder;
                    })
                    .ThenBy(placement => (placement.world)).ToList();
            }
            else
            {
                if (log)
                    SgtLogger.l("not all planets had predefined world order, using fallback method");
                ///Not perfect, hence using the original order if nothing had changed:
                layout.worldPlacements = layout.worldPlacements
                    .OrderBy(placement => placement.allowedRings.max)
                    .ThenBy(placement => (placement.allowedRings.max - placement.allowedRings.min))
                    .ThenBy(placement => (placement.world)).ToList();
            }
            foreach (var world in layout.worldPlacements)
            {
                if (log)
                    SgtLogger.l(world.world + ": " + world.allowedRings.ToString());
            }


            if (CustomCluster.StarterPlanet == null)
                layout.startWorldIndex = 0;
            else
            {
                layout.startWorldIndex = layout.worldPlacements.FindIndex(item => item.startWorld == true);
            }
            if (log)
                SgtLogger.l("StartIndex: " + layout.startWorldIndex);

            SgtLogger.l("Finished generating custom cluster");

            lastWorldGenFailed = false;
            return layout;
        }


        static string LastPresetGenerated = string.Empty;
        public static void ResetToLastPreset()
        {
            if (LastPresetGenerated != string.Empty)
            {
                CreateCustomClusterFrom(LastPresetGenerated);
            }
        }
        public static void ResetPlanetFromPreset(string PlanetID)
        {
            if (LastPresetGenerated != string.Empty)
            {
                CreateCustomClusterFrom(LastPresetGenerated, PlanetID);
            }
        }
        public static int CurrentSeed = -1;


        public static string GetPOIGroupId(SpaceMapPOIPlacement _placement, bool includeTimeForUId = false)
        {
            string data = string.Empty;
            _placement.pois.ForEach(id => data += id);
            data += _placement.numToSpawn;
            data += _placement.avoidClumping;
            data += _placement.canSpawnDuplicates;
            data += _placement.allowedRings.ToString();
            if (includeTimeForUId)
                data += System.DateTime.Now.ToString();
            return GenerateHash(data);

        }
        public static string GenerateHash(string str)
        {
            using (var md5Hasher = MD5.Create())
            {
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
                return BitConverter.ToString(data).Replace("-", "");
            }
        }

        public static List<StarmapItem> GetPOIGroups(ClusterLayout cluster)
        {
            var values = new List<StarmapItem>();

            foreach (SpaceMapPOIPlacement pOIPlacement in cluster.poiPlacements)
            {
                if (pOIPlacement.pois == null)
                    continue;

                string id = GetPOIGroupId(pOIPlacement);
                var item = new StarmapItem(id, StarmapItemCategory.POI, null);
                item.MakeItemPOI(pOIPlacement);
                values.Add(item);
            }
            return values;
        }

        public static void CreateCustomClusterFrom(string clusterID, string singleItemId = "", bool ForceRegen = false)
        {
            if (singleItemId != string.Empty)
            {
                SgtLogger.l("Regenerating stats for " + singleItemId + " in " + clusterID);
            }
            else
                SgtLogger.l("Generating custom cluster from " + clusterID);

            if (lastWorldGenFailed && !ForceRegen)
                return;
            ClusterLayout Reference = SettingsCache.clusterLayouts.GetClusterData(clusterID);

            if (Reference == null || selectScreen == null || selectScreen.newGameSettings == null)
                return;
            string setting = selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);

            if (setting == null || setting.Length == 0)
                return;

            int seed = int.Parse(setting);

            CurrentSeed = seed;
            if (singleItemId == string.Empty)
            {
                SgtLogger.l("Rebuilding Cluster Data");
                CustomCluster = new CustomClusterData();
                CustomCluster.SetRings(Reference.numRings - 1, true);
                ResetStarmap();
            }
            else
            {
                ///when planet not normally in cluster, but selected rn and to reset - reload from data
                if (!Reference.worldPlacements.Any(placement => placement.world == singleItemId))
                {
                    if (PlanetoidDict.TryGetValue(singleItemId, out var FoundPlanet))
                    {
                        FoundPlanet = GivePrefilledItem(FoundPlanet);
                        switch (FoundPlanet.category)
                        {
                            case StarmapItemCategory.Starter:
                                CustomCluster.StarterPlanet = FoundPlanet;
                                break;
                            case StarmapItemCategory.Warp:
                                CustomCluster.WarpPlanet = FoundPlanet;
                                break;
                            case StarmapItemCategory.Outer:
                                CustomCluster.OuterPlanets[FoundPlanet.id] = FoundPlanet;
                                break;
                        }
                        ResetMeteorSeasons(FoundPlanet.world);
                        FoundPlanet.ClearWorldTraits();
                        //SgtLogger.l("Grabbing Traits");
                        //foreach (var planetTrait in SettingsCache.GetRandomTraits(seed, FoundPlanet.world))
                        //{
                        //    WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
                        //    FoundPlanet.AddWorldTrait(cachedWorldTrait);
                        //    SgtLogger.l(planetTrait, FoundPlanet.DisplayName);
                        //}
                        FoundPlanet.SetPlanetSizeToPreset(WorldSizePresets.Normal);
                        FoundPlanet.SetPlanetRatioToPreset(WorldRatioPresets.Normal);
                    }
                }
            }
            for (int i = 0; i < Reference.worldPlacements.Count; i++)
            {
                WorldPlacement planetPlacement = Reference.worldPlacements[i];

                string planetpath = planetPlacement.world;

                if (PlanetoidDict.TryGetValue(planetpath, out StarmapItem FoundPlanet))
                {
                    if (singleItemId != string.Empty && FoundPlanet.id != singleItemId)
                    {
                        continue;
                    }
                    FoundPlanet.PredefinedPlacementOrder = i;

                    FoundPlanet.AddItemWorldPlacement(planetPlacement);
                    switch (FoundPlanet.category)
                    {
                        case StarmapItemCategory.Starter:
                            CustomCluster.StarterPlanet = FoundPlanet;
                            break;
                        case StarmapItemCategory.Warp:
                            CustomCluster.WarpPlanet = FoundPlanet;
                            break;
                        case StarmapItemCategory.Outer:
                            CustomCluster.OuterPlanets[FoundPlanet.id] = FoundPlanet;
                            break;
                    }

                    ResetMeteorSeasons(FoundPlanet.world);
                    FoundPlanet.ClearWorldTraits();
                    //SgtLogger.l("Grabbing Traits");
                    foreach (var planetTrait in SettingsCache.GetRandomTraits(seed + i, FoundPlanet.world))
                    {
                        WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
                        FoundPlanet.AddWorldTrait(cachedWorldTrait);
                        //SgtLogger.l(planetTrait, FoundPlanet.DisplayName);
                    }
                    FoundPlanet.SetPlanetSizeToPreset(WorldSizePresets.Normal);
                    FoundPlanet.SetPlanetRatioToPreset(WorldRatioPresets.Normal);
                }
            }

            if (singleItemId == string.Empty)
            {
                CustomCluster.defaultOuterPlanets = Reference.worldPlacements.Count;
                CustomCluster.POIs.Clear();
            }
            else
            {
                if (CustomCluster.POIs.ContainsKey(singleItemId))
                {
                    CustomCluster.POIs.Remove(singleItemId);
                }
            }

            LastPresetGenerated = clusterID;

            if (Reference.poiPlacements == null)
                return;

            var items = GetPOIGroups(Reference);
            foreach (var item in items)
            {
                if (singleItemId != string.Empty && item.id != singleItemId)
                {
                    continue;
                }
                CustomCluster.POIs[item.id] = item;
            }
            CustomCluster.SO_Starmap = new SO_StarmapLayout(CurrentSeed);

            if (CGM_Screen != null)
            {
                CGM_Screen.PresetApplied = false;
                CGM_Screen.RebuildStarmap(true);
            }
        }

        public static bool RerollVanillaStarmapWithSeedChange = true;
        public static bool RerollTraitsWithSeedChange = true;
        public static void RerollTraits()
        {
            if (CustomCluster == null || (!RerollTraitsWithSeedChange && CGM_Screen.IsCurrentlyActive))
                return;

            int seed = int.Parse(CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id);

            CurrentSeed = seed;

            var planets = CustomCluster.GetAllPlanets();
            for (int i = 0; i < planets.Count; i++)
            {
                var FoundPlanet = planets[i];
                if (FoundPlanet.world == null) continue;

                FoundPlanet.ClearWorldTraits();
                foreach (var planetTrait in SettingsCache.GetRandomTraits(seed + i, FoundPlanet.world))
                {
                    WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(planetTrait, true);
                    FoundPlanet.AddWorldTrait(cachedWorldTrait);
                }
            }
        }

        public static StarmapItem GivePrefilledItem(StarmapItem ToAdd)
        {
            PopulatePredefinedClusterPlacements();

            if (ToAdd.id.Contains(CGSMClusterManager.RandomKey))
                return ToAdd;

            if (ToAdd.id == null)
                return ToAdd;

            if (PredefinedPlacementData.ContainsKey(ToAdd.id))
                ToAdd.AddItemWorldPlacement(PredefinedPlacementData[ToAdd.id]);
            else
            {
                if (!ToAdd.IsPOI)
                {

                    MinMaxI startCoords = new MinMaxI(0, CustomCluster.Rings);
                    if (ToAdd.category == StarmapItemCategory.Starter)
                        startCoords = new MinMaxI(0, 0);
                    else if (ToAdd.category == StarmapItemCategory.Warp)
                        startCoords = new MinMaxI(3, 5);

                    if (ModAssets.Moonlets.Any(moonlet => ToAdd.id.Contains(moonlet)))
                    {
                        if (PredefinedPlacementData.TryGetValue(ToAdd.id.Replace("Start", ""), out var OuterVariant))
                        {
                            startCoords = OuterVariant.allowedRings;
                        }
                        else if (PredefinedPlacementData.TryGetValue(ToAdd.id.Replace("Warp", ""), out var OuterVariant2))
                        {
                            startCoords = OuterVariant2.allowedRings;
                        }
                        else if (PredefinedPlacementData.TryGetValue(ToAdd.id + "Start", out var StartVariant))
                        {
                            startCoords = StartVariant.allowedRings;
                        }
                        else if (PredefinedPlacementData.TryGetValue(ToAdd.id + "Warp", out var WarpVariant))
                        {
                            startCoords = WarpVariant.allowedRings;
                        }
                    }

                    var item = new WorldPlacement();
                    item.world = ToAdd.id;
                    item.allowedRings = startCoords;
                    item.startWorld = ToAdd.category == StarmapItemCategory.Starter;
                    ToAdd.AddItemWorldPlacement(item);
                }
            }

            return ToAdd;
        }

        public static void TogglePlanetoid(StarmapItem ToAdd)
        {
            var item = GivePrefilledItem(ToAdd); ///Prefilled
                                                 ///only one starter at a time
                                                 ///
            CustomCluster.SO_Starmap = null;
            if (item.category == StarmapItemCategory.Starter)
            {
                if (item.Equals(CustomCluster.StarterPlanet))
                    return;
                else
                {
                    CustomCluster.StarterPlanet = item;
                    return;
                }
            }
            ///only one teleport asteroid at a time 
            else if (item.category == StarmapItemCategory.Warp)
            {
                if (item.Equals(CustomCluster.WarpPlanet))
                {
                    CustomCluster.WarpPlanet = null;
                    return;
                }
                else
                {
                    CustomCluster.WarpPlanet = item;
                    return;
                }
            }
            if (ToAdd.category != StarmapItemCategory.POI)
            {
                if (!CustomCluster.OuterPlanets.ContainsKey(item.id))
                    CustomCluster.OuterPlanets[item.id] = (item);
                else
                    CustomCluster.OuterPlanets.Remove(item.id);
            }
            else
            {
                if (!CustomCluster.POIs.ContainsKey(item.id))
                    CustomCluster.POIs[item.id] = (item);
                else
                    CustomCluster.POIs.Remove(item.id);
            }
            return;
        }


        static Dictionary<string, WorldPlacement> PredefinedPlacementData = null;
        public static List<StarmapItem> GetActivePlanetsStarmapitems()
        {
            var planets = new List<StarmapItem>();

            if (CustomCluster.StarterPlanet != null)
                planets.Add(CustomCluster.StarterPlanet);

            if (CustomCluster.WarpPlanet != null)
                planets.Add(CustomCluster.WarpPlanet);

            foreach (var planet in CustomCluster.OuterPlanets)
            {
                planets.Add(planet.Value);
            }
            //foreach (var planet in CustomCluster.POIs)
            //{
            //    planets.Add(planet.Value);
            //}
            return planets;
        }
        public static List<string> GetActivePlanetsCluster()
        {
            var planetPaths = new List<string>();

            if (CustomCluster.StarterPlanet != null)
                planetPaths.Add(CustomCluster.StarterPlanet.id);

            if (CustomCluster.WarpPlanet != null)
                planetPaths.Add(CustomCluster.WarpPlanet.id);

            foreach (var planet in CustomCluster.OuterPlanets)
            {
                planetPaths.Add(planet.Key);
            }
            //foreach (var planet in CustomCluster.POIs)
            //{
            //    planetPaths.Add(planet.Key);
            //}
            return planetPaths;
        }

        public static void PopulatePredefinedClusterPlacements()
        {
            if (PredefinedPlacementData != null) { return; }

            SgtLogger.l("Populating cluster placements");
            PredefinedPlacementData = new Dictionary<string, WorldPlacement>();
            //PredefinedPlacementDataPOI = new Dictionary<string, SpaceMapPOIPlacement>();

            foreach (var ClusterLayout in SettingsCache.clusterLayouts.clusterCache.ToList())
            {

                if (DlcManager.IsExpansion1Active())
                {
                    if (ClusterLayout.Key.Contains("clusters/SandstoneDefault") || ClusterLayout.Value.forbiddenDlcId == DlcManager.EXPANSION1_ID)
                    {
                        continue;
                    }
                }

                if (ClusterLayout.Value.disableStoryTraits)
                {
                    foreach (var planet in ClusterLayout.Value.worldPlacements)
                    {
                        if (PlanetsAndPOIs.ContainsKey(planet.world))
                        {
                            SgtLogger.l(planet.world + " will disable story traits");
                            PlanetsAndPOIs[planet.world].DisablesStoryTraits = true;
                        }
                        else
                            SgtLogger.warning("Tried to add disabling story traits to a planet that does not exist: " + planet.world);
                    }
                }


                foreach (var planetPlacement in ClusterLayout.Value.worldPlacements)
                {
                    PredefinedPlacementData[planetPlacement.world] = planetPlacement;
                }

                if (ClusterLayout.Value.poiPlacements == null)
                    continue;

                //foreach (var poi_tab in ClusterLayout.Value.poiPlacements)
                //{
                //    foreach (var lonePOI in poi_tab.pois)
                //    {
                //        //var animFile = lonePOI.Contains("ArtifactSpacePOI") ? Assets.GetAnim((HashedString)"gravitas_space_poi_kanim") : Assets.GetAnim((HashedString)"harvestable_space_poi_kanim");

                //        KAnimFile animFile = null;
                //        string animName = "ui";
                //        string name = string.Empty;
                //        string desc = string.Empty;
                //        string id = string.Empty;

                //        //SgtLogger.l(lonePOI, "LonePOI");
                //        GameObject gameObject = Util.KInstantiateUI(Assets.GetPrefab((Tag)lonePOI));

                //        if (gameObject.TryGetComponent<ClusterGridEntity>(out var component1))
                //        {
                //            animName = component1.AnimConfigs.First().initialAnim;
                //            animFile = component1.AnimConfigs.First().animFile;
                //            if (gameObject.TryGetComponent<InfoDescription>(out var descHolder))
                //            {
                //                desc = descHolder.description;
                //                name = component1.Name;
                //            }
                //            if (component1 is ArtifactPOIClusterGridEntity)
                //            {
                //                string artifact_ID = component1.PrefabID().ToString().Replace("ArtifactSpacePOI_", string.Empty);
                //                name = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".NAME"));
                //                desc = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".DESC"));
                //            }
                //            if (component1 is TemporalTear)
                //            {
                //                name = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.NAME"));
                //                desc = Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.DESCRIPTION"));
                //            }
                //            id = component1.PrefabID().ToString();
                //        }

                //        //id = id.Replace("ArtifactSpacePOI_", string.Empty);
                //        //id = id.Replace("HarvestableSpacePOI_", string.Empty);

                //        if (lonePOI.Contains(HarvestablePOIConfig.CarbonAsteroidField)) ///carbon field fix
                //            animName = "carbon_asteroid_field";

                //        if (animName == "closed_loop")///Temporal tear
                //            animName = "ui";


                //        float moreThanOne = lonePOI != "ArtifactSpacePOI_RussellsTeapot" && lonePOI != "TemporalTear" ? MaxAmountPOI : 1;

                //        Sprite POIsprite = Def.GetUISpriteFromMultiObjectAnim(animFile, animName, true);

                //        UnityEngine.Object.Destroy(gameObject);

                //        var poi = new StarmapItem(lonePOI, StarmapItemCategory.POI, POIsprite);

                //        SpaceMapPOIPlacement placement = new SpaceMapPOIPlacement();
                //        placement.pois = new List<string> { lonePOI };
                //        placement.numToSpawn = 1;
                //        placement.allowedRings = poi_tab.allowedRings;
                //        placement.avoidClumping = poi_tab.avoidClumping;
                //        poi = poi.MakeItemPOI(id, placement, moreThanOne, name, desc);

                //        if (!PlanetsAndPOIs.ContainsKey(lonePOI))
                //        {
                //            poi.SetInnerRing(placement.allowedRings.min, true);
                //            poi.SetOuterRing(placement.allowedRings.max, true);
                //            PlanetsAndPOIs.Add(lonePOI, poi);
                //        }
                //        else
                //        {
                //            var toEdit = PlanetsAndPOIs[lonePOI];
                //            toEdit.SetInnerRing(Math.Min(toEdit.minRing, placement.allowedRings.min), true);
                //            toEdit.SetOuterRing(Math.Max(toEdit.maxRing, placement.allowedRings.max), true);
                //        }
                //    }

                //    ///Requires different handling
                //    //PredefinedPlacementDataPOI[poi.]
                //    //SgtLogger.l("", "POI:");
                //    //foreach (var poi2 in poi_tab.pois)
                //    //{
                //    //    //SgtLogger.l(poi2, "Poi in list:");
                //    //}
                //    //SgtLogger.l(poi.avoidClumping.ToString(), "avoid clumping");
                //    //SgtLogger.l(poi.canSpawnDuplicates.ToString(), "Allow Duplicates");
                //    //SgtLogger.l(poi.allowedRings.ToString(), "Allowed Rings");
                //    //SgtLogger.l(poi.numToSpawn.ToString(), "Number to spawn");
                //}
            }
        }

        public static string GetRandomPOI(int seed)
        {
            var ItemList = ModAssets.NonUniquePOI_Ids.Shuffle(new System.Random(seed)).ToList();
            return ItemList[0];
        }


        public static StarmapItem GetRandomItemOfType(StarmapItemCategory starmapItemCategory, int seed = -1)
        {
            List<StarmapItem> items = PlanetoidDict.Values.ToList().FindAll(item => item.category == starmapItemCategory);

            SgtLogger.l(seed + "", "Getting Random item, Seed");

            if (seed != -1)
                items = items.Shuffle(new System.Random(seed)).ToList();
            else
                items.Shuffle();

            bool isClassic = false;
            StarmapItem item = null;
            int i;
            for (i = 0; i < items.Count; ++i)
            {
                isClassic = false;
                item = items[i];

                isClassic = PlanetIsClassic(item);

                if (isClassic)
                {
                    if (CurrentClassicOuterPlanets >= MaxClassicOuterPlanets)
                    {
                        continue;
                        item.SetPlanetSizeToPreset(WorldSizePresets.Smaller); ///Reduce size to terrania level size for classic size planet
                        SgtLogger.l("Classic size limit reached, generating " + item.id + " at smaller size");
                    }
                }

                if (!(item.id.Contains("TemporalTear")
                    || item.id == null
                    || item.id == string.Empty
                    || CustomCluster.OuterPlanets.ContainsKey(item.id)
                    || RandomOuterPlanets.Contains(item.id)
                    || item.id.Contains(RandomKey))
                    )
                {
                    break;
                }

                if (i == items.Count - 1 && starmapItemCategory == StarmapItemCategory.Outer)
                {
                    item = null;
                }
            }

            if (item != null && starmapItemCategory == StarmapItemCategory.Outer)
            {
                RandomOuterPlanets.Add(item.id);
                if (isClassic)
                    CurrentClassicOuterPlanets++;
            }


            //while (item.category != starmapItemCategory || item.id.Contains("TemporalTear") || item.id == null || item.id == string.Empty)
            //{
            //    item = PlanetoidDict.Values.GetRandom();
            //}
            return item;
        }



        static Sprite GetRandomSprite(StarmapItemCategory category)
        {
            switch (category)
            {
                case StarmapItemCategory.Starter:
                    return Assets.GetSprite(SpritePatch.randomStarter);
                case StarmapItemCategory.Warp:
                    return Assets.GetSprite(SpritePatch.randomWarp);
                case StarmapItemCategory.Outer:
                    return Assets.GetSprite(SpritePatch.randomOuter);
                case StarmapItemCategory.POI:
                    return Assets.GetSprite(SpritePatch.randomPOI);
                default:
                    return Assets.GetSprite("unknown");
            }
        }

        public static StarmapItemCategory DeterminePlanetType(ProcGen.World world)
        {

            StarmapItemCategory category = StarmapItemCategory.Outer;

            if (world.startingBaseTemplate != null)
            {
                string stripped = world.startingBaseTemplate.Replace("bases/", string.Empty);

                // SgtLogger.l(stripped.ToUpperInvariant(),"KEY");

                if (stripped.ToUpperInvariant().Contains("WARPWORLD")
                    //|| KeyUpper.ToUpperInvariant().Contains("CGSM")&&
                    //stripped.ToUpperInvariant().Contains("WARPBASE")
                    )
                {
                    category = StarmapItemCategory.Warp;
                }
                else if (stripped.ToUpperInvariant().Contains("BASE")
                    || stripped.ToUpperInvariant().Contains("ALLIN1") ///Vortex base name.
                    //||KeyUpper.ToUpperInvariant().Contains("CGSM") && 
                    //stripped.ToUpperInvariant().Contains("BASE")
                    )
                {
                    category = StarmapItemCategory.Starter;
                }
            }
            else
            {
                category = StarmapItemCategory.Outer;
            }

            return category;
        }

        public static List<StarmapItemCategory> AvailableStarmapItemCategories
        {
            get
            {
                var items = new List<StarmapItemCategory>();
                items.Add(StarmapItemCategory.Starter);
                if (DlcManager.IsExpansion1Active())
                {
                    items.Add(StarmapItemCategory.Warp);
                    items.Add(StarmapItemCategory.Outer);
                    //items.Add(StarmapItemCategory.POI);
                }
                return items;
            }
        }

        public static Dictionary<string, StarmapItem> PlanetoidDict
        {
            get
            {
                if (PlanetsAndPOIs == null)
                {
                    PlanetsAndPOIs = new Dictionary<string, StarmapItem>();

                    foreach (StarmapItemCategory category in AvailableStarmapItemCategories)
                    {
                        if (category < 0)
                            continue;


                        var key = RandomKey + category.ToString();
                        var randomItem = new StarmapItem
                            (
                            key,
                            category,
                            GetRandomSprite(category)
                            );

                        randomItem.SetSpawnNumber(1);

                        //if (category == StarmapItemCategory.POI)
                        //{
                        //    var placement = new SpaceMapPOIPlacement();
                        //    placement.allowedRings = new MinMaxI(0, CustomCluster.Rings);
                        //    placement.canSpawnDuplicates = true;
                        //    placement.numToSpawn = 1;
                        //    placement.avoidClumping = false;
                        //    randomItem = randomItem.MakeItemPOI(key, placement, MaxAmountRandomPOI, SPACEDESTINATIONS.CGM_RANDOM_POI.NAME, SPACEDESTINATIONS.CGM_RANDOM_POI.DESCRIPTION);
                        //    PlanetsAndPOIs[key] = randomItem;
                        //    RandomPOIStarmapItem = randomItem;
                        //}
                        //else
                        //{
                        var placement = new WorldPlacement();
                        MinMaxI startCoords = new MinMaxI(0, CustomCluster.Rings);

                        if (category == StarmapItemCategory.Starter)
                            startCoords = new MinMaxI(0, 0);
                        else if (category == StarmapItemCategory.Warp)
                            startCoords = new MinMaxI(3, 5);



                        placement.allowedRings = startCoords;
                        placement.startWorld = category == StarmapItemCategory.Starter;
                        placement.locationType = category == StarmapItemCategory.Starter ? LocationType.Startworld : LocationType.Cluster;

                        randomItem = randomItem.AddItemWorldPlacement(placement, category == StarmapItemCategory.Outer);
                        PlanetsAndPOIs[key] = randomItem;

                        if (category == StarmapItemCategory.Outer)
                            RandomOuterPlanetsStarmapItem = randomItem;
                        //}
                    }

                    foreach (var WorldFromCache in SettingsCache.worlds.worldCache)
                    {
                        StarmapItemCategory category = StarmapItemCategory.Outer;
                        //SgtLogger.l(World.Key + "; " + World.Value.ToString());
                        ProcGen.World world = WorldFromCache.Value;

                        //SgtLogger.l(world.skip.ToString(), "skip?");
                        if ((int)world.skip >= 99
                            //&& !DebugHandler.enabled
                            )
                            continue;

                        ///Hardcoded checks due to others not having the correct folder structure
                        string KeyUpper = WorldFromCache.Key.ToUpperInvariant();
                        bool SkipWorld =
                               KeyUpper.Contains("EMPTERA") && DlcManager.IsExpansion1Active() ? !KeyUpper.Contains("DLC") : KeyUpper.Contains("DLC")
                            || KeyUpper.Contains("ISLANDS") && DlcManager.IsExpansion1Active() ? !KeyUpper.Contains("DLC") : KeyUpper.Contains("DLC")
                            || KeyUpper.Contains("FULERIA") && DlcManager.IsExpansion1Active() ? !KeyUpper.Contains("DLC") : KeyUpper.Contains("DLC")
                            || DlcManager.IsExpansion1Active() && KeyUpper.Contains("WORLDS/SANDSTONEDEFAULT");





                        if (!SkipWorld)
                        {
                            category = DeterminePlanetType(world);

                            Sprite sprite = ColonyDestinationAsteroidBeltData.GetUISprite(WorldFromCache.Value.asteroidIcon);

                            PlanetsAndPOIs[WorldFromCache.Key] = (new StarmapItem
                            (
                            WorldFromCache.Key,
                            category,
                            sprite
                            ).MakeItemPlanet(world));

                            if (PlanetIsMiniBase(PlanetsAndPOIs[WorldFromCache.Key]))
                            {
                                SgtLogger.l(WorldFromCache.Key + " will disable story traits due to Baby size");
                                PlanetsAndPOIs[WorldFromCache.Key].DisablesStoryTraits = true;
                            }
                            SgtLogger.l("isClassic: " + PlanetIsClassic(PlanetsAndPOIs[WorldFromCache.Key]), WorldFromCache.Key);
                        }

                    }

                    PopulatePredefinedClusterPlacements();
                }
                return PlanetsAndPOIs;
            }
        }

        static int AdjustedClusterSize => CustomCluster.defaultRings + Mathf.Max(0, (CustomCluster.AdjustedOuterExpansion / 4));
        public static void InitializeGeneration()
        {

            int randoPlanets = 0;
            if (CustomCluster.HasStarmapItem(RandomKey + StarmapItemCategory.Outer.ToString(), out var item))
            {
                randoPlanets = -1;
                randoPlanets += (int)item.InstancesToSpawn;

            }

            if (CustomCluster.OuterPlanets.Count + randoPlanets > 6 && CustomCluster.Rings < AdjustedClusterSize)
            {
                System.Action AjustSize = () =>
                {
                    int ringAmount = AdjustedClusterSize;
                    SgtLogger.l("Adjusted Ring Amount to " + ringAmount);
                    CustomCluster.SetRings(ringAmount);
                    InitializeGeneration();
                };
                System.Action nothing = () =>
                {
                    //int ringAmount = AdjustedClusterSize;
                    //CustomCluster.SetRings(ringAmount);
                    //InitializeGeneration();
                };
                System.Action aaanyways = () =>
                {
                    AddCustomClusterAndInitializeClusterGen();
                };


                KMod.Manager.Dialog(Global.Instance.globalCanvas,
               GENERATIONWARNING.WINDOWNAME,
               GENERATIONWARNING.DESCRIPTION,
               GENERATIONWARNING.YES,
               AjustSize,
               GENERATIONWARNING.NOMANUAL
               , nothing
               , (DebugHandler.enabled) ? UIUtils.ColorText("[Debug only] Try generating anyway", Color.red) : null
               , (DebugHandler.enabled) ? aaanyways : null
               );
            }
            else
            {
                AddCustomClusterAndInitializeClusterGen();
            }
        }

        public static bool LastGenFailed => lastWorldGenFailed;

        static bool lastWorldGenFailed = false;
        public static void LastWorldGenDidFail(bool fail = true)
        {
            lastWorldGenFailed = fail;
        }

        public static void OpenPresetWindow(System.Action onclose = null)
        {
            UnityPresetScreen.ShowWindow(CustomCluster, onclose);
        }

        internal static void ResetStarmap()
        {
            if (CustomCluster != null)
            {
                if (!DlcManager.IsExpansion1Active())
                    CustomCluster.ResetVanillaStarmap();
            }
        }
    }
}
