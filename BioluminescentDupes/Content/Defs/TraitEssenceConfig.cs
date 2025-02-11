using BioluminescentDupes.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static KAnim;
using static ResearchTypes;
using static STRINGS.DUPLICANTS.ATTRIBUTES;

namespace BioluminescentDupes.Content.Defs
{
	internal class TraitEssenceConfig : IEntityConfig
	{
		public static string ID = "BD_BioluminEssence";
		public GameObject CreatePrefab()
		{
			var prefab = EntityTemplates.CreateLooseEntity(
				ID,
				STRINGS.ITEMS.PILLS.BD_TRAITESSENCE.NAME,
				STRINGS.ITEMS.PILLS.BD_TRAITESSENCE.DESC,
				1f,
				true,
				Assets.GetAnim("pill_1_kanim"),
				"object",
				Grid.SceneLayer.BuildingBack,
				EntityTemplates.CollisionShape.RECTANGLE,
				0.8f,
				0.4f,
				true,
				additionalTags: new List<Tag>
				{
					GameTags.MiscPickupable,
					GameTags.PedestalDisplayable,
					GameTags.NotRoomAssignable
				});

			prefab.AddComponent<TraitEssence>();

			var light = prefab.AddOrGet<Light2D>();	
			light.Color = Color.green;
			light.Lux = 75;
			light.Range = 2;

			ComplexRecipe.RecipeElement[] inputs = [new ComplexRecipe.RecipeElement((Tag) GeneShufflerRechargeConfig.ID, 1f), new ComplexRecipe.RecipeElement((Tag)LightBugConfig.EGG_ID, 1f)];
			ComplexRecipe.RecipeElement[] outputs = [new ComplexRecipe.RecipeElement(ID.ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)];

			BasicBoosterConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(SupermaterialRefineryConfig.ID, inputs, outputs), inputs, outputs)
			{
				time = 50f,
				description = (string)STRINGS.ITEMS.PILLS.BD_TRAITESSENCE.RECIPEDESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag>() { (Tag)SupermaterialRefineryConfig.ID },
				sortOrder = 1
			};

			return prefab;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
