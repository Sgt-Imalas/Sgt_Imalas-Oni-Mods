using DupeStations.PajamasLocker;
using HarmonyLib;
using UnityEngine;
using UtilLibs;

namespace DupeStations
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
                GameTags.MaterialBuildingElements.Add(PajamasDispenserConfig.PajamasMaterialTag);
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Stations,PajamasDispenserConfig.ID,SuitMarkerConfig.ID);
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Stations,TubeCrossConfig.ID,SuitMarkerConfig.ID);
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
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
        [HarmonyPatch(typeof(SleepClinicPajamas), nameof(SleepClinicPajamas.DoPostConfigure))]
        public static class SleepClinicPajamas_AddBuildableTag
        {
            public static void Postfix(GameObject go)
            {
                KPrefabID component = go.GetComponent<KPrefabID>();
                component.AddTag(PajamasDispenserConfig.PajamasMaterialTag);
            }
        }
    }
}
