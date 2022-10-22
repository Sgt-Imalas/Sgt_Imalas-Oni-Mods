using HarmonyLib;
using LogicSatellites.Behaviours;
using LogicSatellites.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace LogicSatellites
{
    class AddBuildingsPatches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, LightFocussingLensConfig.ID);

                RocketryUtils.AddRocketModuleToBuildList(SatelliteCarrierModuleConfig.ID, RocketryUtils.RocketCategory.deployables);
            }
        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                //add buildings to technology tree
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Computers.SensitiveMicroimaging, SatelliteCarrierModuleConfig.ID);

                InjectionMethods.AddBuildingToTechnology(ModAssets.SatelliteConfigurations[0].TechId, ModAssets.SatelliteConfigurations[0].TechItemId);
                InjectionMethods.AddBuildingToTechnology(ModAssets.SatelliteConfigurations[1].TechId, ModAssets.SatelliteConfigurations[1].TechItemId);


                ModAssets.ExplorationSatellite = __instance.TechItems.AddTechItem(ModAssets.SatelliteConfigurations[0].TechItemId, (string)ModAssets.SatelliteConfigurations[0].NAME, (string)ModAssets.SatelliteConfigurations[0].DESC, GetSpriteFnBuilder("LS_Exploration_Sat"), DlcManager.AVAILABLE_EXPANSION1_ONLY);
                ModAssets.SolarSatellite = __instance.TechItems.AddTechItem(ModAssets.SatelliteConfigurations[1].TechItemId, (string)ModAssets.SatelliteConfigurations[1].NAME, (string)ModAssets.SatelliteConfigurations[1].DESC, GetSpriteFnBuilder("LS_Solar_Sat"), DlcManager.AVAILABLE_EXPANSION1_ONLY);

            }
            private static Func<string, bool, Sprite> GetSpriteFnBuilder(string spriteName) => (Func<string, bool, Sprite>)((anim, centered) => Assets.GetSprite((HashedString)spriteName));
        }

    }
}
