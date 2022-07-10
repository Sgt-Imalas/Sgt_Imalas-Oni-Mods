using HarmonyLib;
using LogicSatelites.Behaviours;
using LogicSatelites.Buildings;
using LogicSatelites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ComplexRecipe;

namespace LogicSatelites
{
    class Patches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                RocketryUtils.AddRocketModuleToBuildList(SateliteCarrierModuleConfig.ID);
            }
        }


        [HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
        public static class SatelitePartsPatch
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
        public static class CusomSideScreenPatch_SatelliteCarrier
        {
            public static void Postfix()
            {
                UIUtils.AddClonedSideScreen<SatelliteCarrierModuleSideScreen>("SatelliteCarrierModuleSideScreen", "HabitatModuleSideScreen", typeof(HabitatModuleSideScreen));
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
