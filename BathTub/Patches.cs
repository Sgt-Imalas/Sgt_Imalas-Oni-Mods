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
using static BathTub.ModAssets;

namespace BathTub
{
    internal class Patches
    {
        /// <summary>
        /// register custom status items
        /// </summary>
        [HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
        public static class Database_BuildingStatusItems_CreateStatusItems_Patch
        {
            public static void Postfix()
            {
                BathTub.RegisterStatusItems();
            }
        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Liquids.Sanitation, BathTubConfig.ID);

                //SgtLogger.l("grabbing sparkle streaker fx for foam:");
                //ModAssets.FoamFX = Util.KInstantiateUI(EffectPrefabs.Instance.SparkleStreakFX);
                //ModAssets.InitFoam();

                //UIUtils.ListAllChildrenWithComponents(EffectPrefabs.Instance.SparkleStreakFX.transform);
            }
        }
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Plumbing, BathTubConfig.ID, ShowerConfig.ID);
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
    }
}
