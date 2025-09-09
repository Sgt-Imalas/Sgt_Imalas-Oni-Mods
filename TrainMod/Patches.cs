using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMod.Content.Defs.Buildings;
using UnityEngine;
using UtilLibs;
using static TrainMod.ModAssets;

namespace TrainMod
{
    class Patches
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
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RailStationConfig.ID);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RailPieceConfig.ID);
				ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RailCurveConfig.ID);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RailSwitchLeftConfig.ID);
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, RailSwitchRightConfig.ID);
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
                LocalisationUtil.Translate(typeof(STRINGS));
            }
        }
    }
}
