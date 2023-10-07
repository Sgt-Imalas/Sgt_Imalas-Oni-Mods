using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    /// <summary>
    /// Make Rocket interior wall ports part of the rocket wall to allow conduit loaders/unloaders to connect to them
    /// </summary>
    public class RocketWallAllowsAllWallsPatch
    {

        [HarmonyPatch]
        public static class AddRocketWallTagToTilesThatShouldBeWall
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    component.AddTag(GameTags.RocketEnvelopeTile);
                    component.AddTag(GameTags.CorrosionProof);
                }
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
                yield return typeof(RocketInteriorGasInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorGasOutputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidOutputPortConfig).GetMethod(name);
            }
        }



        /// <summary>
        /// Fixes bugged rad absorpion of habitat ports
        /// </summary>
        [HarmonyPatch]
        public static class MakeHabitatPortsFoundationTiles
        {
            [HarmonyPostfix]
            static void Postfix(ref BuildingDef __result)
            {
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    BuildingTemplates.CreateFoundationTileDef(__result);
                }
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.CreateBuildingDef);
                yield return typeof(RocketInteriorGasInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorGasOutputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidInputPortConfig).GetMethod(name);
                yield return typeof(RocketInteriorLiquidOutputPortConfig).GetMethod(name);
            }
        }


        public class AutoDropperRocketFix
        {
            public static Dictionary<AutoStorageDropper.Instance, bool> InvertedDictionary = new Dictionary<AutoStorageDropper.Instance, bool>();

            [HarmonyPatch(typeof(AutoStorageDropper.Instance), MethodType.Constructor)]
            [HarmonyPatch(new Type[] { typeof(IStateMachineTarget), typeof(AutoStorageDropper.Def) })]
            public class RegisterDropper
            {
                public static void Postfix(AutoStorageDropper.Instance __instance)
                {
                    //always start off (otherwise it will drop a single blob)
                    if (__instance.master.gameObject.TryGetComponent<RocketConduitSender>(out _))
                    {
                        //SgtLogger.l("blocked dispenser from deleting liquid");
                        InvertedDictionary[__instance] = false;// __instance.def.invertElementFilter;

                    }
                    else
                    {
                        //SgtLogger.l("toilet found doing nothin");
                        InvertedDictionary[__instance] =  __instance.def.invertElementFilter;
                    }


                    //SgtLogger.l("Instance: " + __instance.master.ToString() + ": " + __instance.def.invertElementFilter);
                }
            }
            [HarmonyPatch(typeof(AutoStorageDropper.Instance), nameof(AutoStorageDropper.Instance.SetInvertElementFilter))]
            public class ApplyInvert
            {
                public static void Postfix(AutoStorageDropper.Instance __instance, bool value)
                {
                    InvertedDictionary[__instance] = value;
                }
            }


            [HarmonyPatch(typeof(AutoStorageDropper.Instance), nameof(AutoStorageDropper.Instance.AllowedToDrop))]
            public static class FixAllowedToDrop
            {
                public static bool Prefix(AutoStorageDropper.Instance __instance, SimHashes element, ref bool __result)
                {
                    if(__instance.def.elementFilter == null || __instance.def.elementFilter.Length == 0)
                    {
                        __result = true;
                        return false;
                    }

                    if (__instance.def.elementFilter.Contains(element))
                    {
                        ///When element is in filter,return true if filters are not inverted, false if filters are inverted
                        __result = !InvertedDictionary[__instance];
                    }
                    else
                    {
                        __result = InvertedDictionary[__instance];
                    }
                    return false;
                }
            }
        }

    }
}
