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
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, LightFocussingLensConfig.ID);
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, LightScatteringLensConfig.ID);

                RocketryUtils.AddRocketModuleToBuildList(SatelliteCarrierModuleConfig.ID, new RocketryUtils.RocketCategory[] { RocketryUtils.RocketCategory.deployables, RocketryUtils.RocketCategory.utility },ArtifactCargoBayConfig.ID);
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

                //InjectionMethods.AddBuildingToTechnology(ModAssets.SatelliteConfigurations[0].TechId, ModAssets.SatelliteConfigurations[0].TechItemId);
                //InjectionMethods.AddBuildingToTechnology(ModAssets.SatelliteConfigurations[1].TechId, ModAssets.SatelliteConfigurations[1].TechItemId);


                ModAssets.ExplorationSatellite = InjectionMethods.AddItemToTechnologySprite(ModAssets.SatelliteConfigurations[0].TechItemId, ModAssets.SatelliteConfigurations[0].TechId, (string)ModAssets.SatelliteConfigurations[0].NAME, (string)ModAssets.SatelliteConfigurations[0].DESC, "LS_Exploration_Sat", DlcManager.AVAILABLE_EXPANSION1_ONLY);
                //ModAssets.SolarSatellite = InjectionMethods.AddItemToTechnology(ModAssets.SatelliteConfigurations[1].TechItemId, ModAssets.SatelliteConfigurations[1].TechId, (string)ModAssets.SatelliteConfigurations[1].NAME, (string)ModAssets.SatelliteConfigurations[1].DESC, "LS_Solar_Sat", DlcManager.AVAILABLE_EXPANSION1_ONLY);

            }
        }

    }
}
