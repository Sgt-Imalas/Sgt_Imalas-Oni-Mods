using HarmonyLib;
using RoboRockets.LearningBrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;

namespace RoboRockets.Patches
{
    internal class NameFixPatch
    {
        [HarmonyPatch(typeof(DetailsScreen), "TrySetRocketTitle")]
        [HarmonyPatch(new Type[] { typeof(ClustercraftExteriorDoor) })]
        public static class Patch_CraftingTableConfig_ConfigureRecipes
        {
            public static bool Prefix(ClustercraftExteriorDoor clusterCraftDoor, DetailsScreen __instance, EditableTitleBar ___TabTitle, int ___setRocketTitleHandle)
            {
                Debug.Log(__instance.target.GetProperName() + ", target, world id: " +clusterCraftDoor.GetMyWorldId());

                if (clusterCraftDoor.HasTargetWorld())
                {
                    Debug.Log("target has World");
                    WorldContainer targetWorld = clusterCraftDoor.GetTargetWorld();
                    if ((UnityEngine.Object)targetWorld != (UnityEngine.Object)null)
                    {
                        ___TabTitle.SetTitle(targetWorld.GetComponent<ClusterGridEntity>().Name);
                        ___TabTitle.SetUserEditable(true);
                    }
                    ___TabTitle.SetSubText(__instance.target.GetProperName());
                    ___setRocketTitleHandle = -1;
                }
                else
                {
                    if (___setRocketTitleHandle != -1)
                        return false;
                    ___setRocketTitleHandle = __instance.target.Subscribe(-71801987, (System.Action<object>)(clusterCraftDoor2 =>
                    {
                        __instance.OnRefreshData((object)null);
                        __instance.target.Unsubscribe(___setRocketTitleHandle);
                        ___setRocketTitleHandle = -1;
                    }));
                }
                return false;
            }
        }
    }
}
