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
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.OnAsteroidClicked))]
        public static class OnAsteroidClickedHandler
        {
            public static void Postfix(ColonyDestinationAsteroidBeltData cluster)
            {
                CGSMClusterManager.LoadCustomCluster = false;
                CGSMClusterManager.CreateCustomClusterFrom(cluster.beltPath);
            }
        }

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

        [HarmonyPatch(typeof(WorldGen))]
        [HarmonyPatch(nameof(WorldGen.ReportWorldGenError))]
        public static class betterError
        {
            public static void Prefix(Exception e, ref string errorMessage)
            {
                errorMessage = e.Message;
            }
        }

        [HarmonyPatch(typeof(SettingsCache))]
        [HarmonyPatch(nameof(SettingsCache.GetRandomTraits))]
        public static class OverrideWorldTraits
        {
            /// <summary>
            /// Inserting Custom Traits
            /// </summary>
            public static bool Prefix(int seed, ProcGen.World world,ref List<string> __result)
            {

                if (CGSMClusterManager.LoadCustomCluster&& CGSMClusterManager.CustomCluster !=null)
                {
                    var traitIDs = CGSMClusterManager.CustomCluster.GiveWorldTraits(world);
                    List<WorldTrait> list = new List<WorldTrait>(SettingsCache.worldTraits.Values);

                    __result = new List<string>();
                    foreach(var trait in traitIDs)
                    {
                        //WorldTrait gatheredTrait = SettingsCache.GetCachedWorldTrait(trait, true);
                        __result.Add(trait);
                    }
                    //__result.Add(SettingsCache.worldTraits.Values.First().filePath);
                   // __result.Add(SettingsCache.worldTraits.Values.Last().filePath);
//
                    //SgtLogger.l("Should have overridden Traits for " + SettingsCache.worldTraits.Values.First().filePath);
                    //SgtLogger.l("Should have overridden Traits for " + SettingsCache.worldTraits.Values.Last().filePath);

                    return false;
                }
                return true;
            }
        }



        [HarmonyPatch(typeof(Cluster))]
        [HarmonyPatch(typeof(Cluster), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(string), typeof(int), typeof(List<string>), typeof(bool), typeof(bool) })]
        public static class ApplyCustomGen
        {
            public static void Prefix(ref string name)
            {
                //CustomLayout
                if (CGSMClusterManager.LoadCustomCluster)
                {
                    if (CGSMClusterManager.CustomCluster == null)
                    {
                        CGSMClusterManager.AddCustomCluster();
                    }
                    name = CGSMClusterManager.CustomClusterID;
                }
            }

            /// Start Comment
            /// 
            /// 
            ///worldGen.SetWorldSize(worldsize.x, worldsize.y);
            ///

            //private static readonly MethodInfo TargetMethod = AccessTools.Method(
            //        typeof(WorldGen)
            //       ,nameof(WorldGen.SetWorldSize)
            //   );
            //public static void CheckForTag(WorldGen placement ,int x, int y, WorldPlacement placement1)
            //{
            //    //Debug.LogWarning(CGSMClusterManager.CustomCluster);
            //    //if(CGSMClusterManager.CustomCluster!=null 
            //    //    && CGSMClusterManager.CustomCluster.HasStarmapItem(placement1.world, out var planet)
            //    //    && planet.placement != null
            //    //    && planet.placement.width >0 
            //    //    && planet.placement.height > 0)
            //    //{
            //    //    placement.SetWorldSize(planet.placement.width,planet.placement.height);
            //    //    SgtLogger.l("Replaced WorldSize: " + planet.placement.width +", " + planet.placement.height);
            //    //}
            //    //else
            //    //{
            //    //placement.SetWorldSize(200, 200);
            //    //}
            //    placement.SetWorldSize(x, y);
            //    //SgtLogger.l(x + ", " + y + "<- by file : by config->" + placement1.width + ", " + placement1.height, "WorldGen");
            //}

            //private static readonly MethodInfo FixForWorldGen = AccessTools.Method(
            //   typeof(ApplyCustomGen),
            //   nameof(CheckForTag)

            //);

            //private static readonly MethodInfo GetWorld = AccessTools.Method(
            //        typeof(ProcGen.WorldPlacement),
            //        "get_world");

            //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            //{
            //    var code = instructions.ToList();

            //    var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == TargetMethod);

            //    var deserializerSearchIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == GetWorld);
            //    var WorldPlacementIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, deserializerSearchIndex);

            //    if (insertionIndex != -1 && WorldPlacementIndex != -1)
            //    {
            //        //int primaryElementIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndex);

            //        code[insertionIndex]= new CodeInstruction(OpCodes.Call, FixForWorldGen);
            //        code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, WorldPlacementIndex));
            //    }
            //    // Debug.Log("DEBUGMETHOD: " + new CodeInstruction(OpCodes.Call, PacketSizeHelper));
            //    TranspilerHelper.PrintInstructions(code);
            //    return code;
            //}

            ///EndComment
        }
    }
}
