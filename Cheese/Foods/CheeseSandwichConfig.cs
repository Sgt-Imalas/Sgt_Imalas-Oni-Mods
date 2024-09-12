using UnityEngine;
using static Cheese.STRINGS;

namespace Cheese.Foods
{
	internal class CheeseSandwichConfig : IEntityConfig
	{
		public const string ID = "CheeseSandwich";
		public static ComplexRecipe recipe;

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(ID, ITEMS.FOOD.CHEESESANDWICH.NAME, ITEMS.FOOD.CHEESESANDWICH.DESC, 1f, false, Assets.GetAnim((HashedString)"cheese_sandwich_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true), ModAssets.Foods.CheeseSandwich);

		public void OnPrefabInit(GameObject inst)
		{
			if (inst.TryGetComponent<KPrefabID>(out var id))
			{
				id.AddTag(ModAssets.Tags.BrackeneProduct);
			}
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
