using UnityEngine;

namespace Imalas_TwitchChaosEvents.Meteors
{
	internal class TacoDehydratedConfig : IEntityConfig
	{
		public static string ID = "ICT_TacoDehydrated";
		public static ComplexRecipe recipe;
		public const float MASS = 1f;
		public const string ANIM_FILE = "dehydrated_food_berry_pie_kanim";
		public const string INITIAL_ANIM = "idle";

		public string[] GetDlcIds() => null;

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}

		public GameObject CreatePrefab()
		{
			KAnimFile anim = Assets.GetAnim((HashedString)"dehydrated_food_berry_pie_kanim");
			GameObject looseEntity = EntityTemplates.CreateLooseEntity(TacoDehydratedConfig.ID,
				(string)STRINGS.ITEMS.FOOD.ICT_TACO.NAME_DEHYDRATED,
				(string)STRINGS.ITEMS.FOOD.ICT_TACO.DESC_DEHYDRATED, 1f, true, anim, "idle",
				Grid.SceneLayer.BuildingFront, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.7f, true, element: SimHashes.Polypropylene);
			EntityTemplates.ExtendEntityToDehydratedFoodPackage(looseEntity, TacoConfig.foodInfo);
			return looseEntity;
		}
	}
}
