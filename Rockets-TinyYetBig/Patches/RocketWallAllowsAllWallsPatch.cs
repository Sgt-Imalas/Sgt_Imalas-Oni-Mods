using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [HarmonyPatch(typeof(RocketInteriorLiquidInputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    component.AddTag(GameTags.RocketEnvelopeTile);
                    component.AddTag(GameTags.CorrosionProof);
                }
            }
        }
        [HarmonyPatch(typeof(RocketInteriorLiquidOutputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall2
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    component.AddTag(GameTags.RocketEnvelopeTile);
                    component.AddTag(GameTags.CorrosionProof);
                }
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
                    InvertedDictionary[__instance] = __instance.def.invertElementFilter;
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
        [HarmonyPatch(typeof(RocketConduitSender.States), nameof(RocketConduitSender.States.InitializeStates))]
        public static class FixLiquidDeletionOnPorts
        {
            public static void Postfix(RocketConduitSender.States __instance)
            {

                __instance.on.working.ground.Enter(_ => SgtLogger.l("entered on.working.ground"));
                __instance.on.working.notOnGround.Enter(_ => SgtLogger.l("entered on.working.ground"));
                __instance.on.working.Enter(_ => SgtLogger.l("entered on.working"));
                __instance.on.waiting.Enter(_ => SgtLogger.l("entered on.working"));
                __instance.on.Enter(_ => SgtLogger.l("entered on"));


                //.Update("fixing_OnGround", (smi, _) =>
                //{
                //    if (smi.gameObject != null)
                //    {
                //        var Dropper = smi.gameObject.GetSMI<AutoStorageDropper.Instance>();
                //        if (Dropper != null)
                //        {
                //            SgtLogger.l("entered working, disabling elementDropper");
                //            Dropper.SetInvertElementFilter(false);
                //            GameScheduler.Instance.ScheduleNextFrame("delayedDeactivation", (_) => Dropper.SetInvertElementFilter(false));
                //        }
                //        else
                //        {
                //            //SgtLogger.warning("dropper was null on enter");
                //        }
                //    }
                //    else
                //    {
                //        SgtLogger.warning("go was null on enter");
                //    }
                //})
                //;
            }
        }



        [HarmonyPatch(typeof(RocketInteriorGasInputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall3
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    component.AddTag(GameTags.RocketEnvelopeTile);
                    component.AddTag(GameTags.CorrosionProof);
                }
            }
        }
        [HarmonyPatch(typeof(RocketInteriorGasOutputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall4
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                if (Config.Instance.HabitatInteriorPortImprovements)
                {
                    component.AddTag(GameTags.RocketEnvelopeTile);
                    component.AddTag(GameTags.CorrosionProof);
                }
            }
        }
    }
}
