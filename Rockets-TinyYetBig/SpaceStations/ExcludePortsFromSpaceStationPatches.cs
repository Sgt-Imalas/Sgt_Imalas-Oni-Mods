using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations
{
    internal class ExcludePortsFromSpaceStationPatches
    {
        [HarmonyPatch(typeof(RocketInteriorGasInputConfig), "ConfigureBuildingTemplate")]
        public static class RmGasInputFromSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorGasOutputConfig), "ConfigureBuildingTemplate")]
        public static class RmGasOutputFromSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorLiquidInputConfig), "ConfigureBuildingTemplate")]
        public static class RmLiquidInputFromSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorLiquidOutputConfig), "ConfigureBuildingTemplate")]
        public static class RmLiquidOutputFromSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorSolidInputConfig), "ConfigureBuildingTemplate")]
        public static class RmLiquidSolidFromSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorSolidOutputConfig), "ConfigureBuildingTemplate")]
        public static class RmSolidOutputFromSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorPowerPlugConfig), "ConfigureBuildingTemplate")]
        public static class RmPowerPlugFromSpaceStation
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(ModAssets.Tags.RocketInteriorOnlyBuilding);
            }
        }
    }
}
