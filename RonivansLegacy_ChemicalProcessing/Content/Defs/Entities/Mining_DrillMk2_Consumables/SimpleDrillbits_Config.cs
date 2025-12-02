using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ResearchTypes;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;
using static RonivansLegacy_ChemicalProcessing.STRINGS.UI;
using static UtilLibs.GameStrings;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.Mining_DrillMk2_Consumables
{
	internal class SimpleDrillbits_Config : IMultiEntityConfig
	{
		public static readonly string
			ID_BASIC = "AIO_SimpleDrillbit_Basic",
			ID_IRON = "AIO_SimpleDrillbit_Iron",
			ID_HARDENED = "AIO_SimpleDrillbit_Hardened";

		public static readonly string TECHITEM_ID = "AIO_TECHSCREEN_ITEM";
		public static GameObject CreateSimpleDrillbit(string ID, string name, string desc, string anim)
		{
			GameObject go = EntityTemplates.CreateLooseEntity(ID, name, desc, 1f, true, Assets.GetAnim(anim), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.35f, 0.35f, true, 0, SimHashes.Creature, [GameTags.IndustrialProduct, ModAssets.Tags.RandomRecipeIngredient_DestroyOnCancel, ModAssets.Tags.MineralProcessing_Drillbit]);
			go.AddOrGet<EntitySplitter>();
			go.AddOrGet<SimpleMassStatusItem>();
			return go;
		}
		public List<GameObject> CreatePrefabs()
		{
			return
				[
				CreateSimpleDrillbit(ID_BASIC,AIO_SIMPLEDRILLBIT_BASIC.NAME,AIO_SIMPLEDRILLBIT_BASIC.DESC,"drillbits_simple_kanim"),
				CreateSimpleDrillbit(ID_IRON,AIO_SIMPLEDRILLBIT_IRON.NAME,AIO_SIMPLEDRILLBIT_IRON.DESC,"drillbits_basic_kanim"),
				CreateSimpleDrillbit(ID_HARDENED,AIO_SIMPLEDRILLBIT_HARDENED.NAME,AIO_SIMPLEDRILLBIT_HARDENED.DESC,"drillbits_steel_kanim"),
				];
		}
		public static int CreateSimpleDrillRecipes(string ID, bool craftingTableRecipe)
		{
			int pos = 800;
			int mult = craftingTableRecipe ? 1 : 2;
			float drillbitCost = 400 * mult;
			float duration = craftingTableRecipe ? 70 : 50;

			var basic = RecipeBuilder.Create(ID, duration)
				.Input(RefinementRecipeHelper.GetStarterMetals(), drillbitCost)
				.Output(ID_BASIC, 1 * mult, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(craftingTableRecipe ? AIO_SIMPLEDRILLBIT_BASIC.RECIPE_DESC_CRAFTINGTABLE : AIO_SIMPLEDRILLBIT_BASIC.RECIPE_DESC)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.RequiresTech(Technology.SolidMaterial.SolidControl)
				.SortOrder(pos++)
				.Build();

			var iron = RecipeBuilder.Create(ID, duration)
				.Input(SimHashes.Iron, drillbitCost)
				.Output(ID_IRON, 1 * mult, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(craftingTableRecipe ? AIO_SIMPLEDRILLBIT_IRON.RECIPE_DESC_CRAFTINGTABLE : AIO_SIMPLEDRILLBIT_IRON.RECIPE_DESC)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.RequiresTech(Technology.SolidMaterial.SolidControl)
				.SortOrder(pos++)
				.Build();

			//steel drilling
			var steel = RecipeBuilder.Create(ID, duration)
				.Input(RefinementRecipeHelper.GetSteelLikes(), drillbitCost)
				.Output(ID_HARDENED, 1 * mult, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.Description(craftingTableRecipe ? AIO_SIMPLEDRILLBIT_HARDENED.RECIPE_DESC_CRAFTINGTABLE : AIO_SIMPLEDRILLBIT_HARDENED.RECIPE_DESC)
				.NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
				.RequiresTech(Technology.SolidMaterial.SolidControl)
				.SortOrder(pos++)
				.Build();

			return pos;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
