using HarmonyLib;
using LogicSatellites.Behaviours;
using LogicSatellites.Buildings;
using LogicSatellites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static ComplexRecipe;

namespace LogicSatellites
{
    class Patches
    {
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                //add buildings to technology tree
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Computers.SensitiveMicroimaging, SatelliteCarrierModuleConfig.ID); 
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                RocketryUtils.AddRocketModuleToBuildList(SatelliteCarrierModuleConfig.ID,"", RocketryUtils.RocketCategory.deployables);
            }
        }

        [HarmonyPatch(typeof(LogicBroadcastReceiver))]
        [HarmonyPatch(nameof(LogicBroadcastReceiver.CheckRange))]
        public static class BroadcastRecieverRangePatch
        {
            public static bool Prefix(LogicBroadcastReceiver __instance, ref bool __result, GameObject broadcaster, GameObject receiver)
            {
                AxialI a, b;
                a = broadcaster.GetMyWorldLocation();
                b = receiver.GetMyWorldLocation();
                bool returnValue = AxialUtil.GetDistance(a, b) <= LogicBroadcaster.RANGE;
                if (returnValue)
                {
                    __result = true;
                    return false;
                }
                __result = ModAssets.FindConnectionViaAdjacencyMatrix(a, b);
                return false;
            }
        }



        //[HarmonyPatch(typeof(ModuleFlightUtilitySideScreen), "SetTarget")]
        //[HarmonyPatch(nameof(ModuleFlightUtilitySideScreen.SetTarget))]
        //public static class ModuleFlightUtilitySideScreen_Gibinfo
        //{
        //    public static void Postfix(ModuleFlightUtilitySideScreen __instance)
        //    {
        //        Debug.Log("FLIGHTSCREEN MONO");
        //        UIUtils.ListAllChildren(__instance.transform);
        //    }
        //}

        [HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
        public static class SatellitePartsPatch
        {
            public static void Postfix()
            {
                AddSatellitePartsRecipe();
            }
            private static void AddSatellitePartsRecipe()
            {
                RecipeElement[] input = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.Glass.CreateTag(), 12f),
                    new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 3f),
                    new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 15f)
                };

                ComplexRecipe.RecipeElement[] output = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SatelliteComponentConfig.ID, 1f)
                };

                string product = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

                SatelliteComponentConfig.recipe = new ComplexRecipe(product, input, output)
                {
                    time = 45,
                    description = "Satellite parts, the bread and butter of satellite construction",
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag>()
                    {
                        CraftingTableConfig.ID
                    },
                };

            }
        }

        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_SatelliteCarrier
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                UIUtils.AddClonedSideScreen<SatelliteCarrierModuleSideScreen>("SatelliteCarrierModuleSideScreen", "ModuleFlightUtilitySideScreen", typeof(ModuleFlightUtilitySideScreen));
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
    }
}
