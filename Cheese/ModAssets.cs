using Cheese.Entities;
using Cheese.Foods;
using Cheese.Traits;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Cheese
{
	internal class ModAssets
	{
		public static void LoadAll()
		{
			SOUNDS.LoadAll();
		}
		public static void ClearCheeseTables() => CheeseTableTargets.Clear();

		public static HashSet<CheeseTable> CheeseTableTargets = new HashSet<CheeseTable>();

		public static Color CheeseColor = UIUtils.rgb(240, 180, 0);

		public class Foods
		{
			public static ComplexRecipe CheeseRecipe;

			public static Dictionary<Tag, Tag> CheeseConversions = new Dictionary<Tag, Tag>()
			{
				{
					BurgerConfig.ID, CheeseBurgerConfig.ID
				}
			}
			;


			public const float CHEESE_KCAL_PER_KG = 20f; //200KCal cheese per day per cow, cows make 50kg milk per day and the chosen conversion ratio is 5->1 
			public const float CHEESEBURGER_KCAL_PER_KG = 6400f;
			public const float CHEESESANDWICH_KCAL_PER_KG = 2800f;
			public const float GRILLEDCHEESE_KCAL_PER_KG = 4800f;

			public static EdiblesManager.FoodInfo CheeseEdible = new(
				ModElements.ModElementRegistration.Cheese.ToString(),
				CHEESE_KCAL_PER_KG * 1000f,
				2,
				255.15f,
				277.15f,
				4800f,
				false,null,null);

			public static EdiblesManager.FoodInfo CheeseBurger =
				new EdiblesManager.FoodInfo(CheeseBurgerConfig.ID,
					CHEESEBURGER_KCAL_PER_KG * 1000f,
					6,
					255.15f,
					277.15f, 2800f,
					can_rot: true, null, null)
				.AddEffects(new List<string> { "GoodEats" })
				.AddEffects(new List<string> { "SeafoodRadiationResistance" }, [DlcManager.EXPANSION1_ID]);

			public static EdiblesManager.FoodInfo CheeseSandwich =
				new EdiblesManager.FoodInfo(CheeseSandwichConfig.ID,
					CHEESESANDWICH_KCAL_PER_KG * 1000f,
					3,
					255.15f,
					277.15f, 2800f,
					can_rot: true, null, null);
			public static EdiblesManager.FoodInfo GrilledCheese =
			   new EdiblesManager.FoodInfo(GrilledCheeseConfig.ID,
				   GRILLEDCHEESE_KCAL_PER_KG * 1000f,
				   5,
				   255.15f,
				   277.15f, 2800f,
				   can_rot: true, null, null);

		}
		public static class SOUNDS
		{
			public const string
				CHEESE = "CHEESE_JAMESMAY"
				;
			public static void LoadAll()
			{
				SoundUtils.LoadSound(SOUNDS.CHEESE, "JamesCheese.wav");
			}
		}


		public class Tags
		{
			/// <summary>
			/// Add to brackene products for the bractose intolerance trait
			/// </summary>
			public static Tag BrackeneProduct = TagManager.Create("CheeseMod_BrackeneProduct");
			public static Tag CheeseMaterial = TagManager.Create("CheeseMod_CheeseMaterial");


		}
	}
}
