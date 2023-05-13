using HarmonyLib;
using Rockets_TinyYetBig.Buildings.CargoBays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;

namespace Rockets_TinyYetBig.Patches
{
    public class CargoCapacityPatches
    {
        [HarmonyPatch(typeof(CargoBayCluster), nameof(CargoBayCluster.OnSpawn))]
        public static class CargoBayRebalance
        {
            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix(CargoBayCluster __instance)
            {
                float targetCapacity = 0f;

                if (__instance.storageType == CargoBay.CargoType.Entities) return;

                if (__instance.TryGetComponent<KPrefabID>(out var def))
                {

                   // SgtLogger.l(def.PrefabID().ToString());
                    ModAssets.GetCargoBayCapacity(def.PrefabID().ToString(), out targetCapacity);

                    //SgtLogger.l(targetCapacity.ToString(), def.PrefabID().ToString());
                    //SgtLogger.l(__instance.storage.capacityKg.ToString(), def.PrefabID().ToString());

                    if (targetCapacity == 0) return;

                    SgtLogger.l(__instance.storage.capacityKg + " old -> new " + targetCapacity);
                    if (__instance.storage.capacityKg != targetCapacity)
                    {
                        if (__instance.userMaxCapacity == __instance.storage.capacityKg)
                            __instance.userMaxCapacity = targetCapacity;

                        __instance.storage.capacityKg = targetCapacity;
                    }

                    if (!Config.Instance.RebalancedCargoCapacity)
                    {
                        if (__instance.userMaxCapacity > targetCapacity)
                            __instance.userMaxCapacity = targetCapacity;
                    }
                }
            }
        }
    }
}
