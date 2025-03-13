//using System.Collections.Generic;
//using UnityEngine;
//using static EdiblesManager;

//namespace CannedFoods.Foods
//{
//	internal class CannedMilkConfig : IEntityConfig
//	{
//		public const string ID = "CF_CannedMilk";
//		public static ComplexRecipe recipe;

//		public GameObject CreatePrefab()
//		{
//			GameObject prefab = EntityTemplates.CreateLooseEntity(
//				id: ID,
//				name: STRINGS.ITEMS.FOOD.CF_CANNEDMILK.NAME,
//				desc: STRINGS.ITEMS.FOOD.CF_CANNEDMILK.DESC,
//				mass: 1f,
//				unitMass: false,
//				anim: Assets.GetAnim("canned_milk_kanim"),
//				initialAnim: "object",
//				sceneLayer: Grid.SceneLayer.Front,
//				collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
//				width: 0.5f,
//				height: 0.7f,
//				isPickupable: true,
//				sortOrder: 0,
//				element: SimHashes.Creature,
//				additionalTags: new List<Tag>
//				{
//					ModAssets.Tags.DropCanOnEat
//				});


//			FoodInfo foodInfo = new FoodInfo(
//				id: ID,
//				caloriesPerUnit: TUNING.FOOD.FOOD_TYPES.SPICEBREAD.CaloriesPerUnit / 2f,
//				quality: TUNING.FOOD.FOOD_TYPES.SPICEBREAD.Quality,
//				preserveTemperatue: TUNING.FOOD.DEFAULT_PRESERVE_TEMPERATURE,
//				rotTemperature: TUNING.FOOD.DEFAULT_ROT_TEMPERATURE,
//				spoilTime: TUNING.FOOD.SPOIL_TIME.VERYSLOW,
//				can_rot: false, null,null);

//			return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
//		}

//		public string[] GetDlcIds() => null;

//		public void OnPrefabInit(GameObject inst)
//		{
//			//inst.AddOrGet<CanRecycler>();
//		}

//		public void OnSpawn(GameObject inst)
//		{
//		}
//	}
//}
