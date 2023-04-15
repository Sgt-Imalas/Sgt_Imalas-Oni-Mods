using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace Rockets_TinyYetBig.SpaceStations
{
    public class AllowBuildingsInsideSpaceStationsPatches
    {
        [HarmonyPatch(typeof(BaseModularLaunchpadPortConfig), "ConfigureBuildingTemplate")]
        public static class AllowPortLoadersInSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
            }
        }

        [HarmonyPatch(typeof(ExobaseHeadquartersConfig), "ConfigureBuildingTemplate")]
        public static class AllowSmallPrintingPodInSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
            }
        }


        [HarmonyPatch(typeof(ClusterUtil), "ActiveWorldHasPrinter")]
        public static class CLusterUtil_AllowTelepads
        {
            public static bool Prefix(ref bool __result)
            {
                __result = SpaceStationManager.ActiveWorldIsRocketInterior() || Components.Telepads.GetWorldItems(ClusterManager.Instance.activeWorldId).Count > 0;
                return false;
            }
        }
    }
}
