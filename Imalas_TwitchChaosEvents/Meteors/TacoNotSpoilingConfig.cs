using System.Collections.Generic;
using UnityEngine;
using static EdiblesManager;

namespace Imalas_TwitchChaosEvents.Meteors
{
	internal class TacoNotSpoilingConfig : IEntityConfig
	{
		public static string ID = "ICT_Taco_NonSpoiling";

		public GameObject CreatePrefab()
		{
			GameObject prefab = EntityTemplates.CreateLooseEntity(
				id: ID,
				name: STRINGS.ITEMS.FOOD.ICT_TACO_NONSPOILING.NAME,
				desc: STRINGS.ITEMS.FOOD.ICT_TACO_NONSPOILING.DESC,
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


			FoodInfo foodInfo = new FoodInfo(
				id: ID,
				dlcId: DlcManager.VANILLA_ID,
				caloriesPerUnit: 5800000f,
				quality: 6,
				preserveTemperatue: 255.15f,
				rotTemperature: 277.15f,
				spoilTime: 4800f,
				can_rot: false);
			foodInfo.AddEffects(new List<string>() { "GoodEats" }, DlcManager.AVAILABLE_ALL_VERSIONS);
			return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
		}

		public string[] GetDlcIds()
		{
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}

