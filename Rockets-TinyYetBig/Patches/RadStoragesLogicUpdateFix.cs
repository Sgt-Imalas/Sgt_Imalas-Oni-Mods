using ClipperLib;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    public class RadStoragesLogicUpdateFix
    {
        [HarmonyPatch(typeof(ReorderableBuilding), "ApplyAnimOffset")]
        public static class ForceUpdateLogicOnReorder
        {
            public static void Postfix(ReorderableBuilding __instance)
            {
                RocketModuleCluster component = __instance.GetComponent<RocketModuleCluster>();
                if (component != null)
                {
                    var Modules = component.CraftInterface.ClusterModules;
                    foreach (var Module in Modules)
                    {
                        if (Module.Get().TryGetComponent<HighEnergyParticleStorage>(out var storage))
                        {
                            //MethodInfo methodInfo = typeof(HighEnergyParticleStorage).GetMethod("UpdateLogicPorts", BindingFlags.NonPublic | BindingFlags.Instance);
                            //methodInfo.Invoke(storage, null);

                            Traverse.Create(storage).Method("UpdateLogicPorts").GetValue();
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(CraftModuleInterface), "DoLand")]
        public static class ForceUpdateLogicOnLand
        {
            public static void Postfix(CraftModuleInterface __instance)
            {
                foreach (var Module in __instance.ClusterModules)
                {
                    if (Module.Get().TryGetComponent<HighEnergyParticleStorage>(out var storage))
                    {
                        //MethodInfo methodInfo = typeof(HighEnergyParticleStorage).GetMethod("UpdateLogicPorts", BindingFlags.NonPublic | BindingFlags.Instance);
                        //methodInfo.Invoke(storage, null);

                        Traverse.Create(storage).Method("UpdateLogicPorts").GetValue();
                    }
                }
            }
        }
    }
}
