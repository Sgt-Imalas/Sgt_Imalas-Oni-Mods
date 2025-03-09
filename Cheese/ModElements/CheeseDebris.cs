using UnityEngine;

namespace Cheese.ModElements
{
	internal class CheeseDebris : IOreConfig
	{
		public SimHashes ElementID => ModElementRegistration.Cheese.SimHash;

		public string[] GetDlcIds() => null;

		public static GameObject GetPrefabForRecipe()
		{
			GameObject OreEntity = EntityTemplates.CreateSolidOreEntity(ModElementRegistration.Cheese.SimHash);
			ExtendEntityToFood(OreEntity, ModAssets.Foods.CheeseEdible);
			return OreEntity;
		}

		public GameObject CreatePrefab()
		{
			GameObject OreEntity = EntityTemplates.CreateSolidOreEntity(this.ElementID);
			ExtendEntityToFood(OreEntity, ModAssets.Foods.CheeseEdible);
			return OreEntity;
		}
		public static GameObject ExtendEntityToFood(GameObject template, EdiblesManager.FoodInfo foodInfo)
		{
			if (template.TryGetComponent(out KPrefabID kPrefabID))
			{
				kPrefabID.AddTag(GameTags.Edible);
				template.AddOrGet<Edible>().FoodInfo = foodInfo;

				kPrefabID.instantiateFn += go => go.GetComponent<Edible>().FoodInfo = foodInfo;

				if (template.TryGetComponent(out PrimaryElement primaryElement))
				{
					primaryElement.MassPerUnit = 1f;
					primaryElement.Units = 1000f;
				}
				kPrefabID.AddTag(ModAssets.Tags.BrackeneProduct);
				GameTags.DisplayAsCalories.Add(kPrefabID.PrefabTag);
			}
			return template;
		}
	}
}
