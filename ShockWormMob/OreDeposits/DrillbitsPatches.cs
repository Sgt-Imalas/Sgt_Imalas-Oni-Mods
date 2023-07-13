using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe;
using static SandboxToolParameterMenu.SelectorValue;

namespace ShockWormMob.OreDeposits
{
    internal class DrillbitsPatches
    {
        [HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
        public static class AdDrillbitRecipesToCraftingTable
        {
            public static void Postfix()
            {
                AddDrillbitRecipesForEachRefinedMetal();
            }
            private static void AddDrillbitRecipesForEachRefinedMetal()
            {                                                                                                                    //allow both refined metals   &         steel, thermium, niobium
                foreach (Element refinedMetal in ElementLoader.elements.FindAll((Element e) => e.IsSolid && (e.HasTag(GameTags.RefinedMetal))|| e.HasTag(GameTags.Alloy)))
                {

                    RecipeElement[] input = new RecipeElement[1]
                    {
                        new RecipeElement(refinedMetal.tag, 50f, true)
                    };
                    RecipeElement[] output = new RecipeElement[1]
                    {
                        new RecipeElement(TagManager.Create(DrillbitConfig.ID), 5f)
                    };


                    var recipeID = ComplexRecipeManager.MakeRecipeID(MetalRefineryConfig.ID, input, output);

                    ComplexRecipe complexRecipe = new ComplexRecipe(recipeID, input, output)
                    {
                        time = 10f,
                        description = DrillbitConfig.DESC_RECIPE,
                        nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
                        fabricators = new List<Tag>()
                        {
                            TagManager.Create(CraftingTableConfig.ID)
                        }
                    };

                }
            }
        }
    }
    class SandboxToolParameterMenuPatch
    {
        private static readonly HashSet<Tag> tags = new HashSet<Tag>()
        {
			// Geysers
			//"HarvestableDeposit_" + OreDepositsConfig.MiscDepositID,
            "HarvestableDeposit_" + OreDepositsConfig.KatheriumDepositID
          //  ,
           // "HarvestableDeposit_" + OreDepositsConfig.CopperOreDepositID
			/*"GeyserGeneric_" + GeyserConfigs.MURKY_BRINE_GEYSER,
			"GeyserGeneric_" + GeyserConfigs.BISMUTH_VOLCANO,
			"GeyserGeneric_" + GeyserConfigs.CORAL_REEF,*/
		};

        [HarmonyPatch(typeof(SandboxToolParameterMenu), "ConfigureEntitySelector")]
        public static class SandboxToolParameterMenu_ConfigureEntitySelector_Patch
        {
            public static void Postfix(SandboxToolParameterMenu __instance)
            {
                var sprite = Def.GetUISprite(Assets.GetPrefab(CrabConfig.ID));
                var parent = __instance.entitySelector.filters.First(x =>
                    x.Name == global::STRINGS.UI.SANDBOXTOOLS.FILTERS.ENTITIES.SPECIAL);

                var filter = new SearchFilter("Chemical", obj => obj is KPrefabID id && tags.Contains(id.PrefabTag), parent, sprite);

                AddFilters(__instance, filter);
            }


            public static void AddFilters(SandboxToolParameterMenu menu, params SearchFilter[] newFilters)
            {
                var filters = menu.entitySelector.filters;

                if (filters == null)
                {
                    //Log.Warning("Filters are null");
                    return;
                }

                var f = new List<SearchFilter>(filters);
                f.AddRange(newFilters);
                menu.entitySelector.filters = f.ToArray();

                UpdateOptions(menu);
            }

            public static void UpdateOptions(SandboxToolParameterMenu menu)
            {
                var filters = menu.entitySelector.filters;

                if (filters == null)
                {
                    return;
                }

                var options = ListPool<object, SandboxToolParameterMenu>.Allocate();

                foreach (var prefab in Assets.Prefabs)
                {
                    foreach (var filter in filters)
                    {
                        if (filter.condition(prefab))
                        {
                            options.Add(prefab);
                            break;
                        }
                    }
                }

                menu.entitySelector.options = options.ToArray();
                options.Recycle();
            }

            private static SearchFilter FindParent(SandboxToolParameterMenu menu, string parentFilterID)
            {
                return parentFilterID != null ? menu.entitySelector.filters.First(x => x.Name == parentFilterID) : null;
            }
        }
    }
}
