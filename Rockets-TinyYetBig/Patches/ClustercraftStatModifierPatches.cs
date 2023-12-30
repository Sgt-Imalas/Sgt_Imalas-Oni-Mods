using HarmonyLib;
using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.Docking.DockingSpacecraftHandler;

namespace Rockets_TinyYetBig.Patches
{
    public class ClustercraftStatModifierPatches
    {
        public static class Rocket_Speed_Docking_Patch
        {
            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.EnginePower))]
            [HarmonyPatch(MethodType.Getter)]
            public static class EnginePower_Patch
            {
                public static void Postfix(Clustercraft __instance, ref float __result)
                {


                    if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
                    {
                        foreach (var docked in manager.GetConnectedRockets())
                        {
                            if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
                            {
                                var engine = craft.ModuleInterface.GetEngine();

                                if (engine != null && engine.TryGetComponent<RocketModuleCluster>(out var engineModule) && Mathf.RoundToInt(craft.ModuleInterface.Range) > 0)
                                {
                                    __result += engineModule.performanceStats.EnginePower;
                                }
                            }
                        }
                    }
                    //SgtLogger.l("EnginePower " + __result);
                }
            }

            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.TotalBurden))]
            [HarmonyPatch(MethodType.Getter)]
            public static class TotalBurden_Patch
            {
                public static void Postfix(Clustercraft __instance, ref float __result)
                {
                    if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
                    {
                        foreach (var docked in manager.GetConnectedRockets())
                        {
                            if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
                            {
                                foreach (var module in craft.ModuleInterface.ClusterModules)
                                {
                                    __result += module.Get().performanceStats.Burden;
                                }
                            }
                        }
                    }
                    //SgtLogger.l("TotalBurden " + __result);
                }
            }
            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.OnSpawn))]
            public static class Pull_Empty_Rockets
            {
                public static void Postfix(Clustercraft __instance)
                {
                    __instance.m_clusterTraveler.onTravelCB += () =>
                    {
                        if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
                        {
                            foreach (var docked in manager.GetCurrentDocks())
                            {
                                var handler = docked.spacecraftHandler;
                                if (handler!=null
                                && (Mathf.RoundToInt(handler.clustercraft.ModuleInterface.Range) == 0 || handler.GetCraftType != DockableType.Rocket) // pull other rockets if they are empty or if __instance is not a rocket (space station or derelict - maybe flying derelicts later?)
                                && handler.clustercraft.Location != __instance.Location)
                                {
                                    if ((ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(__instance.Location, EntityLayer.Asteroid) == null))
                                    {
                                        SgtLogger.l("Pulled stranded rocket " + handler.clustercraft.Name + " to new tile with " + __instance.Name);
                                        handler.clustercraft.Location = __instance.Location;
                                    }
                                    else
                                    {
                                        SgtLogger.l("Disconnected " + handler.clustercraft.Name + " as stranded in orbit");
                                        handler.clustercraft.m_clusterTraveler.m_destinationSelector.SetDestination(handler.clustercraft.Location);
                                        //craft.m_clusterTraveler.m_destinationSelector.SetDestination(__instance.Location);
                                    }
                                }
                            }
                        }
                    };
                }
            }


            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.CheckDesinationInRange))]
            public static class CMI_Range_Patch
            {
                public static void Postfix(Clustercraft __instance, ref bool __result)
                {
                    if (__result == true)
                        return;

                    if (__instance.m_clusterTraveler.CurrentPath == null)
                    {
                        return;
                    }

                    __result = Mathf.RoundToInt(__instance.Speed * __instance.m_clusterTraveler.TravelETA()) <= Mathf.RoundToInt(__instance.ModuleInterface.Range);

                    //if (__instance.TryGetComponent<DockingManager>(out var manager))
                    //{
                    //    SgtLogger.l("speed: " + __instance.Speed + ", eta" + __instance.m_clusterTraveler.TravelETA() + ", range" + __instance.ModuleInterface.Range);
                    //}
                }
            }



            [HarmonyPatch(typeof(Clustercraft))]
            [HarmonyPatch(nameof(Clustercraft.Speed))]
            [HarmonyPatch(MethodType.Getter)]
            public static class Speed_Patch
            {
                public static bool Prefix(Clustercraft __instance, ref float __result)
                {
                    if (__instance.TryGetComponent<DockingSpacecraftHandler>(out var manager))
                    {
                        float unmodifiedSpeed = __instance.EnginePower / __instance.TotalBurden;
                        float totalAutoPilotMultiplier = __instance.AutoPilotMultiplier;
                        float totalPilotSkillMultiplier = __instance.PilotSkillMultiplier;
                        float totalControlStationBuffTimeRemaining = __instance.controlStationBuffTimeRemaining;
                        //float numberOfPilots = 1;


                        if (manager.GetConnectedRockets().Count == 0)
                            return true;


                        foreach (var docked in manager.GetConnectedRockets())
                        {
                            if (ClusterManager.Instance.GetWorld(docked).TryGetComponent<Clustercraft>(out var craft))
                            {
                                totalAutoPilotMultiplier = totalAutoPilotMultiplier < craft.AutoPilotMultiplier ? craft.AutoPilotMultiplier : totalAutoPilotMultiplier;
                                totalPilotSkillMultiplier = totalPilotSkillMultiplier < craft.PilotSkillMultiplier ? craft.PilotSkillMultiplier : totalPilotSkillMultiplier;
                                totalControlStationBuffTimeRemaining = totalControlStationBuffTimeRemaining < craft.controlStationBuffTimeRemaining ? craft.controlStationBuffTimeRemaining : totalControlStationBuffTimeRemaining;
                                //++numberOfPilots;
                            }
                        }
                        //totalAutoPilotMultiplier /= numberOfPilots;
                        //totalPilotSkillMultiplier /= numberOfPilots;
                        //totalControlStationBuffTimeRemaining /= numberOfPilots;


                        float finalValue = unmodifiedSpeed * totalAutoPilotMultiplier * totalPilotSkillMultiplier;
                        if (totalControlStationBuffTimeRemaining > 0f)
                        {
                            finalValue += unmodifiedSpeed * 0.2f;
                        }
                        __result = finalValue;

                        return false;
                    }
                    return true;
                }
            }
        }

    }
}
