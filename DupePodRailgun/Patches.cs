using DupePodRailgun.Buildings;
using HarmonyLib;
using UtilLibs;

namespace DupePodRailgun
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
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Shipping, PodRailgunBaseConfig.ID);
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Shipping, PodRailgunRailPieceConfig.ID);
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
