using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;


namespace RotatableRadboltStorage
{
    internal class Patches
    {

        /// <summary>
        /// add destination selection sidescreen to rad battery
        /// </summary>
        [HarmonyPatch(typeof(HighEnergyParticleDirectionSideScreen))]
        [HarmonyPatch(nameof(HighEnergyParticleDirectionSideScreen.IsValidForTarget))]
        public static class AddTargetValidityForRadBattery
        {
            public static bool Prefix(GameObject target, ref bool __result)
            {
                if (target.TryGetComponent<BatteryDirectionAddon>(out var targetComponent))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HEPBattery))]
        [HarmonyPatch(nameof(HEPBattery.Fire))]
        public static class TargetCellForRadBattery
        {
            public static bool Prefix(HEPBattery.Instance smi)
            {
                var DirectionAddon = smi.GetComponent<BatteryDirectionAddon>();
                if (DirectionAddon != null)
                {
                    int particleOutputCell = DirectionAddon.GetCircularHEPOutputCell();
                    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"HighEnergyParticle"), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
                    gameObject.SetActive(true);
                    if (!(gameObject != null))
                        return false;
                    HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
                    component.payload = smi.particleStorage.ConsumeAndGet(smi.particleThreshold);
                    component.SetDirection(DirectionAddon.Direction);

                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HEPBatteryConfig))]
        [HarmonyPatch(nameof(HEPBatteryConfig.CreateBuildingDef))]
        public static class AdjustNormalHEPBattery
        {
            public static void Postfix(ref BuildingDef __result)
            {
                __result
                    .HighEnergyParticleOutputOffset = new CellOffset(0, 1);
                __result.AnimFiles = new KAnimFile[1]
                {
                    Assets.GetAnim((HashedString) "radbolt_battery_rotatable_kanim")
                };

            }
        }
        [HarmonyPatch(typeof(HEPBatteryConfig))]
        [HarmonyPatch(nameof(HEPBatteryConfig.ConfigureBuildingTemplate))]
        public static class AdjustNormalHEPBatteryTwo
        {
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<BatteryDirectionAddon>();
                //go.AddOrGet<HEPStorageThreshold>();
            }
        }

        [HarmonyPatch(typeof(HighEnergyParticleStorage), "UpdateLogicPorts")]        
        public static class ReplaceNormalPortLogic
        {
            public static bool Prefix(HighEnergyParticleStorage __instance)
            {
                if (__instance.TryGetComponent<HEPStorageThreshold>(out var t))
                {
                    return true;
                   // return false;
                }
                return true;
            }
        }
    }
}
