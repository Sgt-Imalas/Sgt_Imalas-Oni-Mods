using Database;
using HarmonyLib;
using Klei.AI;
using KMod;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static ClusterTraitGenerationManager.ModAssets;
using static KAnim;
using ProcGen;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;
using static ClusterTraitGenerationManager.STRINGS.UI;
using STRINGS;
using Klei.CustomSettings;
using static STRINGS.UI.FRONTEND;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using System.Threading;
using static ClusterTraitGenerationManager.STRINGS;

namespace ClusterTraitGenerationManager
{
    internal class Patches
    {

        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        /// <summary>
        /// adds gear button to cluster view
        /// </summary>
        [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.OnSpawn))]
        public static class InsertCustomClusterOption
        {
            public static void Postfix(ColonyDestinationSelectScreen __instance)
            {
                var InsertLocation = __instance.shuffleButton.transform.parent; //__instance.transform.Find("Layout/DestinationInfo/Content/InfoColumn/Horiz/Section - Destination/DestinationDetailsHeader/");
                var copyButton = Util.KInstantiateUI(__instance.shuffleButton.gameObject, InsertLocation.gameObject, true); //UIUtils.GetShellWithoutFunction(InsertLocation, "CoordinateContainer", "cgsm");

                // UIUtils.ListAllChildrenPath(__instance.transform); 

                UIUtils.TryFindComponent<Image>(copyButton.transform, "FG").sprite = Assets.GetSprite("icon_gear");
                UIUtils.TryFindComponent<ToolTip>(copyButton.transform, "").toolTip = STRINGS.UI.CGMBUTTON.DESC;
                UIUtils.TryFindComponent<KButton>(copyButton.transform, "").onClick += () => CGSMClusterManager.InstantiateClusterSelectionView(__instance);
                CGSMClusterManager.selectScreen = __instance;

            }
        }

        /// <summary>
        /// Regenerates Custom cluster with newly created traits on seed shuffle
        /// </summary>
        [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.ShuffleClicked))]
        public static class TraitShuffler
        {
            public static void Postfix(ColonyDestinationSelectScreen __instance)
            {
                CGSMClusterManager.selectScreen = __instance;
                if (__instance.newGameSettings == null)
                    return;

                string clusterPath = __instance.newGameSettings.GetSetting(CustomGameSettingConfigs.ClusterLayout);
                if (clusterPath == null || clusterPath.Count() == 0)
                {
                    clusterPath = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                }
                CGSMClusterManager.LoadCustomCluster = false;
                CGSMClusterManager.CreateCustomClusterFrom(clusterPath);
            }
        }
        [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.CoordinateChanged))]
        public static class SeedInserted
        {
            public static void Postfix(ColonyDestinationSelectScreen __instance)
            {
                CGSMClusterManager.selectScreen = __instance;
                if (__instance.newGameSettings == null)
                    return;

                string clusterPath = __instance.newGameSettings.GetSetting(CustomGameSettingConfigs.ClusterLayout);
                if (clusterPath == null || clusterPath.Count() == 0)
                {
                    clusterPath = DestinationSelectPanel.ChosenClusterCategorySetting == 1 ? "expansion1::clusters/VanillaSandstoneCluster" : "expansion1::clusters/SandstoneStartCluster";
                }
                CGSMClusterManager.LoadCustomCluster = false;
                CGSMClusterManager.CreateCustomClusterFrom(clusterPath);
            }
        }

        /// <summary>
        /// custom meteor example code
        /// </summary>
        //[HarmonyPatch(typeof(Db), "Initialize")]
        //public static class Db_addSeason
        //{
        //    public static void Postfix(Db __instance)
        //    {
        //        __instance.GameplayEvents.Add(
        //            new MeteorShowerSeason(
        //                "AllShowersInOnceID",
        //                GameplaySeason.Type.World,
        //                "EXPANSION1_ID",
        //                20f,
        //                false,
        //                startActive: true,
        //                clusterTravelDuration: 6000f)
        //            .AddEvent(Db.Get().GameplayEvents.MeteorShowerDustEvent)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterCopperShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterGoldShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterIronShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterIceShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterBiologicalShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterBleachStoneShower)
        //            .AddEvent(Db.Get().GameplayEvents.ClusterUraniumShower));
        //        ///obv. not all events
        //    }
        //}


        /// <summary>
        /// Resets Custom cluster with newly generated preset
        /// </summary>
        [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.OnAsteroidClicked))]
        public static class OnAsteroidClickedHandler
        {
            public static void Postfix(ColonyDestinationAsteroidBeltData cluster)
            {
                CGSMClusterManager.LoadCustomCluster = false;
                CGSMClusterManager.CreateCustomClusterFrom(cluster.beltPath);
            }
        }

        /// <summary>
        /// CoreTraitFix_SolarSystemWorlds
        /// </summary>
        [HarmonyPatch(typeof(SettingsCache))]
        [HarmonyPatch(nameof(SettingsCache.LoadWorldTraits))]
        public static class TraitInitPostfix_ExclusionFix
        {
            public static void Postfix()
            {
                string coreKey = string.Empty;
                string holesKey = string.Empty;
                foreach (var trait in SettingsCache.GetCachedWorldTraitNames())
                {
                    if (trait.Contains(SpritePatch.missingMoltenCoreTexture))
                        coreKey = trait;

                    if (trait.Contains(SpritePatch.missingHoleTexture))
                        holesKey = trait;
                }

                var IronCoreTrait = SettingsCache.GetCachedWorldTrait(coreKey, false);
                if (IronCoreTrait != null)
                {
                    IronCoreTrait.colorHex = "B7410E"; /// BA5C3F  or B7410E
                    if (IronCoreTrait.exclusiveWithTags == null)
                        IronCoreTrait.exclusiveWithTags = new List<string>();
                    if (!IronCoreTrait.exclusiveWithTags.Contains("CoreTrait"))
                        IronCoreTrait.exclusiveWithTags.Add("CoreTrait");
                }
                var HolesTrait = SettingsCache.GetCachedWorldTrait(holesKey, false);
                if (HolesTrait != null)
                {
                    ///Light purple
                    HolesTrait.colorHex = "9696e2";
                    ///black
                    //HolesTrait.colorHex = "000000"; 
                }
            }
        }


        /// <summary>
        /// Prevents the normal cluster menu from closing when the custom cluster menu is open
        /// </summary>
        [HarmonyPatch(typeof(NewGameFlowScreen))]
        [HarmonyPatch(nameof(NewGameFlowScreen.OnKeyDown))]
        public static class CatchGoingBack
        {
            public static bool Prefix(KButtonEvent e)
            {
                if (CGSMClusterManager.Screen != null && CGSMClusterManager.Screen.activeSelf)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Allow planets to load into the world cache if 
        /// - they are in a cluster (default condition)
        /// - have "CGM"/"CGSM" in their Name (added)
        /// </summary>
        [HarmonyPatch(typeof(Worlds))]
        [HarmonyPatch(nameof(Worlds.UpdateWorldCache))]
        public static class AllowUnusedWorldTemplatesToLoadIntoCache
        {
            public static bool ContainsOrIsPredefined(ISet<string> referencedWorlds, string toContain)
            {
                return referencedWorlds.Contains(toContain) || toContain.Contains("CGSM") || toContain.Contains("CGM");
            }

            private static readonly MethodInfo AllowTemplates = AccessTools.Method(
               typeof(AllowUnusedWorldTemplatesToLoadIntoCache),
               nameof(ContainsOrIsPredefined)
            );

            private static readonly MethodInfo TargetMethod = AccessTools.Method(
                    typeof(System.Collections.Generic.ICollection<string>),
                    nameof(System.Collections.Generic.ICollection<string>.Contains));

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();

                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == TargetMethod);

                if (insertionIndex != -1)
                {
                    code[insertionIndex] = new CodeInstruction(OpCodes.Call, AllowTemplates);
                }

                return code;
            }
        }

        /// <summary>
        /// Makes error msg display the actual error instead of "couldn't germinate"
        /// </summary>
        [HarmonyPatch(typeof(WorldGen))]
        [HarmonyPatch(nameof(WorldGen.ReportWorldGenError))]
        public static class betterError
        {
            public static void Prefix(Exception e, ref string errorMessage)
            {
                CGSMClusterManager.LastWorldGenFailed();
                if (e.Message.Contains("Could not find a spot in the cluster for"))
                {
                    string planetName = e.Message.Replace("Could not find a spot in the cluster for ", string.Empty).Split()[0];
                    SgtLogger.l(planetName);
                    var planetData = SettingsCache.worlds.GetWorldData(planetName);
                    if (planetData != null)
                    {
                        if (Strings.TryGet(planetData.name, out var name))
                        {
                            SgtLogger.error(name + " could not be placed.");
                            errorMessage = string.Format(STRINGS.ERRORMESSAGES.PLANETPLACEMENTERROR, name);
                            return;
                        }

                    }
                }
                if (e is TemplateSpawningException)
                {
                    errorMessage = e.Message;
                }

            }
        }

        //[HarmonyPatch(typeof(GameUtil))]
        //[HarmonyPatch(nameof(GameUtil.GenerateRandomWorldName))]

        //public static class names
        //{
        //    //global::STRINGS.NAMEGEN.WORLD.ROOTS
        //    public static void Postfix(string[] nameTables, ref string __result)
        //    {
        //        foreach(var nameTable in nameTables )
        //        {
        //           // SgtLogger.l(nameTable, "NAMETABLE");
        //        }
        //           // SgtLogger.l(__result, "NAME CHOSEN");
        //    }
        //}

        /// <summary>
        /// During Cluster generation, load traits from custom cluster instead of randomized
        /// </summary>
        [HarmonyPatch(typeof(SettingsCache))]
        [HarmonyPatch(nameof(SettingsCache.GetRandomTraits))]
        public static class OverrideWorldTraits
        {
            /// <summary>
            /// Inserting Custom Traits
            /// </summary>
            public static bool Prefix(int seed, ProcGen.World world, ref List<string> __result)
            {
                if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
                {
                    var traitIDs = CGSMClusterManager.CustomCluster.GiveWorldTraitsForWorldGen(world);

                    if (traitIDs.Count > 0)
                    {
                        List<WorldTrait> list = new List<WorldTrait>(SettingsCache.worldTraits.Values);

                        __result = new List<string>();
                        foreach (var trait in traitIDs)
                        {
                            //WorldTrait gatheredTrait = SettingsCache.GetCachedWorldTrait(trait, true);
                            __result.Add(trait);
                        }
                        return false;
                    }

                }
                return true;
            }
            public static void Postfix(ProcGen.World world, ref List<string> __result)
            {
                if (__result.Count > 0)
                {
                    var list = new List<string>();
                    int replaceCount = 0;
                    foreach (var trait in __result)
                    {
                        if (!trait.Contains("CGMRandomTraits"))
                        {
                            list.Add(trait);
                        }
                        else
                            ++replaceCount;
                    }
                    if (replaceCount > 0)
                    {
                        __result = CGSMClusterManager.CustomClusterData.AddRandomTraitsForWorld(list, world, replaceCount);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Worlds))]
        [HarmonyPatch(nameof(Worlds.GetWorldData))]
        public static class OverrideWorldSizeOnDataGetting
        {
            static Dictionary<string, Vector2I> OriginalPlanetSizes = new Dictionary<string, Vector2I>();
            /// <summary>
            /// Inserting Custom Traits
            /// </summary>
            public static bool Prefix(Worlds __instance, string name, ref ProcGen.World __result)
            {
                if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
                {
                    if (!name.IsNullOrWhiteSpace() && __instance.worldCache.TryGetValue(name, out var value))
                    {
                        if (CGSMClusterManager.CustomCluster.HasStarmapItem(name, out var item) && value.worldsize != item.CustomPlanetDimensions)
                        {

                            if (!OriginalPlanetSizes.ContainsKey(name))
                            {
                                Vector2I newDimensions = item.CustomPlanetDimensions;
                                SgtLogger.l(value.worldsize.ToString(), "Original World Size");
                                OriginalPlanetSizes[name] = value.worldsize;
                                value.worldsize = newDimensions;
                                SgtLogger.l("Applied custom planet size to " + item.DisplayName + ", new size: " + newDimensions.X + "x" + newDimensions.Y);
                            }
                        }
                        else if (OriginalPlanetSizes.ContainsKey(name))
                        {
                            value.worldsize = OriginalPlanetSizes[name];
                            OriginalPlanetSizes.Remove(name);
                        }
                        //value.worldsize = new((int)(value.worldsize.x*0.8f), (int)(value.worldsize.y*0.8));
                        //SgtLogger.l("Applied custom planet size to " + name);

                        __result = value;
                    }
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(TemplateSpawning))]
        [HarmonyPatch(nameof(TemplateSpawning.SpawnTemplatesFromTemplateRules))]
        public static class AddSomeGeysers
        {
            static Dictionary<string, Dictionary<List<string>, int>> OriginalGeyserAmounts = new Dictionary<string, Dictionary<List<string>, int>>();
            /// <summary>
            /// Inserting Custom Traits
            /// </summary>
            public static void Prefix(WorldGenSettings settings)
            {
                const string geyserKey = "GEYSER";
                if (CGSMClusterManager.LoadCustomCluster && CGSMClusterManager.CustomCluster != null)
                {
                    if (!OriginalGeyserAmounts.ContainsKey(settings.world.filePath))
                        OriginalGeyserAmounts[settings.world.filePath] = new Dictionary<List<string>, int>();

                    if (CGSMClusterManager.CustomCluster.HasStarmapItem(settings.world.filePath, out var item) && item.CurrentSizePreset != WorldSizePresets.Normal)
                    {
                        foreach (var WorldTemplateRule in settings.world.worldTemplateRules)
                        {
                            if (WorldTemplateRule.names.Any(name => name.ToUpperInvariant().Contains(geyserKey)))
                            {
                                if (!OriginalGeyserAmounts[settings.world.filePath].ContainsKey(WorldTemplateRule.names))
                                {
                                    OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names] = WorldTemplateRule.times;
                                }

                                WorldTemplateRule.times = Mathf.RoundToInt(((float)OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names]) * (float)item.CurrentSizePreset / 100f);
                                SgtLogger.l(string.Format("Adjusting geyser roll amount to worldsize for {0}; {1} -> {2}", WorldTemplateRule.names.FirstOrDefault(), OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names], WorldTemplateRule.times), item.id);
                            }
                        }
                    }

                }
                else if (OriginalGeyserAmounts.ContainsKey(settings.world.filePath))
                {
                    foreach (var WorldTemplateRule in settings.world.worldTemplateRules)
                    {
                        if (OriginalGeyserAmounts[settings.world.filePath].ContainsKey(WorldTemplateRule.names))
                            WorldTemplateRule.times = OriginalGeyserAmounts[settings.world.filePath][WorldTemplateRule.names];
                    }
                    //OriginalGeyserAmounts.Remove(settings.world.filePath);
                }
            }
        }

        [HarmonyPatch(typeof(Cluster))]
        [HarmonyPatch(typeof(Cluster), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(string), typeof(int), typeof(List<string>), typeof(bool), typeof(bool) })]
        public static class ApplyCustomGen
        {
            /// <summary>
            /// Setting ClusterID to custom cluster if it should load
            /// 
            /// </summary>
            public static void Prefix(ref string name)
            {
                //CustomLayout
                if (CGSMClusterManager.LoadCustomCluster)
                {
                    if (CGSMClusterManager.CustomCluster == null)
                    {
                        ///Generating custom cluster if null
                        CGSMClusterManager.AddCustomCluster();
                    }
                    name = CGSMClusterManager.CustomClusterID;
                }
            }
        }
    }
}
