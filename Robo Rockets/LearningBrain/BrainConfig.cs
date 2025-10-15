using System.Collections.Generic;
using UnityEngine;
using static ComplexRecipe;

namespace RoboRockets.LearningBrain
{
	class BrainConfig : IEntityConfig,IHasDlcRestrictions
	{
		public const string ID = "RR_BrainFlyer";
		public static ComplexRecipe recipe;
		public static RecipeElement[] ProductionCosts = new RecipeElement[]
		{
			new RecipeElement(SimHashes.Glass.CreateTag(),200f),
			new RecipeElement(SimHashes.Steel.CreateTag(),100f),
			new RecipeElement(SimHashes.Ethanol.CreateTag(), 360f),
			new RecipeElement(GeneShufflerRechargeConfig.tag, 1f)
		};

		public GameObject CreatePrefab()
		{
			GameObject prefab = EntityTemplates.CreateLooseEntity(
				id: ID,
				name: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.NAME,
				desc: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.DESC,
				mass: 1f,
				unitMass: true,
				anim: Assets.GetAnim("brain_item_kanim"),
				initialAnim: "object",
				sceneLayer: Grid.SceneLayer.Front,
				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				width: 1.06f,
				height: 1.06f,
				isPickupable: true,
				sortOrder: 0,
				element: SimHashes.Creature,
				additionalTags: new List<Tag>()
				{
					GameTags.IndustrialProduct,
					ModAssets.Tags.SpaceBrain,
					GameTags.PedestalDisplayable
				});

			prefab.AddOrGet<UserNameable>();
			prefab.AddComponent<FlyingBrain>();
			prefab.AddOrGet<DemolishableDroppable>();
			prefab.AddOrGet<OccupyArea>().SetCellOffsets(EntityTemplates.GenerateOffsets(0, 0));
			prefab.AddOrGet<CharacterOverlay>().shouldShowName = true;

			return prefab;
		}

		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];
		public string[] GetDlcIds() => null;

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{

		}

		public string[] GetForbiddenDlcIds() => null;
	}
}
