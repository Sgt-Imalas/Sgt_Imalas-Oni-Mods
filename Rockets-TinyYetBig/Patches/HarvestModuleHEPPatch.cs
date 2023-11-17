using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Buildings.Nosecones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig//.Patches
{
    class HarvestModuleHEPPatch
    {

        /// <summary>
        /// Adds Laser Nosecone to harvestCheck
        /// </summary>
        [HarmonyPatch(typeof(RocketClusterDestinationSelector), "CanRocketHarvest")]
        public static class AddLaserNosecone_Patch
        {
            public static void Postfix(RocketClusterDestinationSelector __instance, ref bool __result)
            {
                if (!__result)
                {
                    List<NoseConeHEPHarvest.StatesInstance> resourceHarvestModules = GetAllLaserNoseconeHarvestModules(__instance.GetComponent<Clustercraft>());
                    if (resourceHarvestModules.Count > 0)
                    {
                        foreach (var statesInstance in resourceHarvestModules)
                        {
                            
                            if (statesInstance.CheckIfCanHarvest())
                            {
                                __result = true;
                            }
                        }
                    }
                }
            }
            public static List<NoseConeHEPHarvest.StatesInstance> GetAllLaserNoseconeHarvestModules(Clustercraft craft)
            {
                List<NoseConeHEPHarvest.StatesInstance> laserNosecones = new List<NoseConeHEPHarvest.StatesInstance>();
                foreach (Ref<RocketModuleCluster> clusterModule in craft.ModuleInterface.ClusterModules)
                {
                    NoseConeHEPHarvest.StatesInstance smi = clusterModule.Get().GetSMI<NoseConeHEPHarvest.StatesInstance>();
                    if (smi != null)
                        laserNosecones.Add(smi);
                }
                return laserNosecones;
            }
        }




        /// <summary>
        /// Sub all hep storage change handlers on start mining
        /// </summary>
        [HarmonyPatch(typeof(RocketClusterDestinationSelector), "WaitForPOIHarvest")]
        public static class AddSubToParticleStorage
        {
            public static void Postfix(RocketClusterDestinationSelector __instance)
            {
                foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)__instance.GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
                {
                    if ((bool)(UnityEngine.Object)clusterModule.Get().GetComponent<HighEnergyParticleStorage>())
                    {
                        __instance.Subscribe(clusterModule.Get().gameObject, (int)GameHashes.OnParticleStorageChanged, OnHEPHandler(__instance));
                    }
                }
            }
        }

        /// <summary>
        /// Unsub all hep storage change handlers on returntrip
        /// </summary>
        [HarmonyPatch(typeof(RocketClusterDestinationSelector), "OnStorageChange")]
        public static class RemoveSubFromParticleStorage
        {
            public static void Postfix(RocketClusterDestinationSelector __instance)
            {
                var CanHarvestMethod = typeof(RocketClusterDestinationSelector).GetMethod("CanRocketHarvest", BindingFlags.NonPublic | BindingFlags.Instance);
                var canIHarvest = (bool)CanHarvestMethod.Invoke(__instance, null);
                if (canIHarvest)
                    return;

                foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>)__instance.GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
                {


                    if ((bool)(UnityEngine.Object)clusterModule.Get().GetComponent<HighEnergyParticleStorage>())
                    {
                        //SgtLogger.debuglog("HEP FOUND; UNSUBSCRIBING");
                        __instance.Unsubscribe(clusterModule.Get().gameObject, (int)GameHashes.OnParticleStorageChanged, OnHEPHandler(__instance) );
                    }
                }
            }
        }

        public static Action<object> OnHEPHandler(RocketClusterDestinationSelector inst)
        {
            var component = inst.GetComponent<Clustercraft>();
            var UpdateMethod = typeof(RocketClusterDestinationSelector).GetMethod("OnStorageChange", BindingFlags.NonPublic | BindingFlags.Instance);
            var loopmaster = component.GetComponent<RocketClusterDestinationSelector>();
            var action = (Action<object>)UpdateMethod.CreateDelegate(typeof(Action<object>), loopmaster);
            return action;
        }

    }
}
