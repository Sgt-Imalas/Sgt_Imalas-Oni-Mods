using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using ONITwitchLib.Utils;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ComplexRecipe;

namespace Imalas_TwitchChaosEvents.Elements
{

    public class ELEMENTpatches
    {



        /// <summary>
        /// akis beached 
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public class ElementLoader_Load_Patch
        {
            public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
            {
                // Add my new elements
                var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
                ModElements.RegisterSubstances(list);

                //SgtLogger.l("ElementList length after that method; " + substanceTablesByDlc[DlcManager.VANILLA_ID].GetList().Count);
                //SgtLogger.l("ElementList SO length; " + substanceTablesByDlc[DlcManager.EXPANSION1_ID].GetList().Count);
            }
            public static void Postfix(ElementLoader __instance)
            {
                //SgtLogger.l("ElementList length in postfix; " + ElementLoader.elementTable.Count);
                SgtElementUtil.FixTags();
            }
        }

        // Credit: Heinermann (Blood mod)
        public static class EnumPatch
        {
            [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
            public class SimHashes_ToString_Patch
            {
                public static bool Prefix(ref Enum __instance, ref string __result)
                {
                    if (__instance is SimHashes hashes)
                    {
                        return !SgtElementUtil.SimHashNameLookup.TryGetValue(hashes, out __result);
                    }

                    return true;
                }
            }
        }

        //public static class  DangerousCreeperPatch
        //{
        //    [HarmonyPatch(typeof(SafeCellQuery), "GetFlags")]
        //    public class SimHashes_ToString_Patch
        //    {
        //        public static void Postfix(int cell, ref SafeCellQuery.SafeFlags __result)
        //        {                  
        //            int headCell = Grid.CellAbove(cell);

        //            if (
        //                (Grid.Element[cell].id == ModElements.Creeper.SimHash || Grid.Element[headCell].id == ModElements.Creeper.SimHash))
        //            {
        //                SgtLogger.l(Grid.Element[cell].tag.ToString(),"DANGER");
        //                SgtLogger.l(Grid.CellToPos(cell).ToString(),"DANGER2");

        //                __result  = (SafeCellQuery.SafeFlags)0;
        //            }
        //        }
        //    }
        //}


        [HarmonyPatch(typeof(WaterCoolerChore.States), "Drink")]
        public class WaterCoolerChore_States_Drink_Patch
        {
            public static void Prefix(WaterCoolerChore.States __instance, WaterCoolerChore.StatesInstance smi)
            {
                var storage = __instance.masterTarget.Get<Storage>(smi);

                if (storage.IsEmpty())
                    return;

                Tag tag = storage.items[0].PrefabID();

                if (ModAssets.WaterCoolerDrinks.Beverages.TryGetValue(tag, out var effect))
                    __instance.stateTarget
                        .Get<Worker>(smi)
                        .GetComponent<Effects>()
                        .Add(effect, true);
            }
        }
        [HarmonyPatch(typeof(SupermaterialRefineryConfig))]
        [HarmonyPatch(nameof(SupermaterialRefineryConfig.ConfigureBuildingTemplate))]
        public static class Patch_CraftingTableConfig_ConfigureRecipes
        {
            public static void Postfix()
            {
                AddRetawRecipe();
            }
            private static void AddRetawRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(ModElements.InverseWater.Tag, 1000f),
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Water.CreateTag(), 999f),
                    new RecipeElement(SimHashes.Unobtanium.CreateTag(), 1f)
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
                new ComplexRecipe(recipeID, input, output)
                {
                    time = 30f,
                    description = STRINGS.ELEMENTS.ITCE_INVERSE_WATER.RECIPE_DESCRIPTION,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
                };

            }
            private static void AddRetawReverseRecipe()
            {
                RecipeElement[] input = new RecipeElement[]
                {
                    new RecipeElement(SimHashes.Water.CreateTag(), 499.5f),
                    new RecipeElement(ModElements.InverseWater.Tag, 499.5f),
                };

                RecipeElement[] output = new RecipeElement[]
                {
                    new RecipeElement(ModElements.InverseWater.Tag, 1000f),
                };

                string recipeID = ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, input, output);
                new ComplexRecipe(recipeID, input, output)
                {
                    time = 30f,
                    description = STRINGS.ELEMENTS.ITCE_INVERSE_WATER.RECIPE_DESCRIPTION_CREATE,
                    nameDisplay = RecipeNameDisplay.Result,
                    fabricators = new List<Tag> { SupermaterialRefineryConfig.ID }
                };

            }
        }
    }
}
