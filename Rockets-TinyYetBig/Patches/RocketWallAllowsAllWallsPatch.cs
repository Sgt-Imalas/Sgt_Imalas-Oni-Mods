using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
    internal class RocketWallAllowsAllWallsPatch
    {
        [HarmonyPatch(typeof(RocketInteriorLiquidInputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(GameTags.RocketEnvelopeTile);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorLiquidOutputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall2
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(GameTags.RocketEnvelopeTile);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorGasInputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall3
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(GameTags.RocketEnvelopeTile);
            }
        }
        [HarmonyPatch(typeof(RocketInteriorGasOutputPortConfig), "DoPostConfigureComplete")]
        public static class AddRocketWallTagToTilesThatShouldBeWall4
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(GameTags.RocketEnvelopeTile);
            }
        }
    }
}
