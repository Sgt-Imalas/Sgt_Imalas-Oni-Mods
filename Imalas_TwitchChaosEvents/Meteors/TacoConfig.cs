using System.Collections.Generic;
using UnityEngine;
using static EdiblesManager;

namespace Imalas_TwitchChaosEvents.Meteors
{
	internal class TacoConfig : IEntityConfig
	{
		public static string ID = "ICT_Taco";
		public static ComplexRecipe recipe;
		public static FoodInfo foodInfo = new FoodInfo(
				id: ID,
				caloriesPerUnit: 5800000f,
				quality: 6,
				preserveTemperatue: 255.15f,
				rotTemperature: 277.15f,
				spoilTime: 4800f,
				can_rot: true,null,null)
		{
			Effects = new List<string>() { "GoodEats" }
		};


		public GameObject CreatePrefab()
		{
			GameObject prefab = EntityTemplates.CreateLooseEntity(
				id: ID,
				name: STRINGS.ITEMS.FOOD.ICT_TACO.NAME,
				desc: STRINGS.ITEMS.FOOD.ICT_TACO.DESC,
				mass: 1f,
				unitMass: false,
				anim: Assets.GetAnim("taco_food_kanim"),
				initialAnim: "object",
				sceneLayer: Grid.SceneLayer.Front,
				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
				width: 1.0f,
				height: 0.6f,
				isPickupable: true,
				sortOrder: 0,
				element: SimHashes.Creature
				);

			return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
		}

		public string[] GetDlcIds() => null;

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}

