
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
using static ConveyorTiles.ModAssets;

namespace ConveyorTiles
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Shipping, ConveyorTileConfig.ID);
            }
        }

        /// <summary>
        /// dont connect tiles that are opposite direction
        /// </summary>
        [HarmonyPatch(typeof(AnimTileable))]
        [HarmonyPatch(nameof(AnimTileable.HasTileableNeighbour))]
        public static class AnimTileable_HasTileableNeighbour_Patch
        {

            public static void Postfix(AnimTileable __instance, int neighbour_cell, ref bool __result)
            {
                var tile = __instance.GetComponent<ConveyorTileSM>();
                if (tile == null) 
                    return;

                GameObject gameObject = Grid.Objects[neighbour_cell, (int)__instance.objectLayer];
                if (gameObject != null)               
                {
                    var neighborTile = gameObject.GetComponent<ConveyorTileSM>();

                    if(neighborTile !=null)
                        __result = ConveyorTileSM.HasTileableNeighbor(tile,neighborTile);
                }
            }
        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.SolidTransport, ConveyorTileConfig.ID);
            }
        }

        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
        /// <summary>
        /// register custom status items
        /// </summary>
        [HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
        public static class Database_BuildingStatusItems_CreateStatusItems_Patch
        {
            public static void Postfix()
            {
                ConveyorTileSM.RegisterStatusItems();
            }
        }
    }
}
