using Database;
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
        
        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            public static void Prefix(Assets __instance)
            {
                InjectionMethods.AddSpriteToAssets(__instance, "LS_Exploration_Sat");
                InjectionMethods.AddSpriteToAssets(__instance, "LS_Solar_Sat");
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
                //DestroySatellitePartsRecipe();
            }

            private static void DestroySatellitePartsRecipe()
            {
                RecipeElement[] input = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SatelliteComponentConfig.ID, 1f)
                };

                ComplexRecipe.RecipeElement[] output = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.Glass.CreateTag(), 12f),
                    new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 3f),
                    new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 15f)
                };

                string product = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

                SatelliteComponentConfig.recipe = new ComplexRecipe(product, input, output)
                {
                    time = 1,
                    description = "No longer in use, get your ressources back.",
                    nameDisplay = RecipeNameDisplay.Ingredient,
                    fabricators = new List<Tag>()
                    {
                        CraftingTableConfig.ID
                    },
                };

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
