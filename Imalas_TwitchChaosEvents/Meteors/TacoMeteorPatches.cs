using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Meteors;
using Klei.AI;
using ONITwitchLib;
using System.Collections.Generic;
using TUNING;
using UtilLibs;
using static ComplexRecipe;

namespace Imalas_TwitchChaosEvents
{
	internal class TacoMeteorPatches
	{
		public const string ITC_TacoMeteorsID = "ITC_TacoMeteorShowerEvent";
		public const string ITC_FakeTacoMeteorsID = "ITC_FakeTacoMeteorShowerEvent";
		public static GameplayEvent ITC_TacoMeteors;
		public static GameplayEvent ITC_FakeTacoMeteors;
		public static void Register(GameplayEvents gameplayEvents)
		{
			ITC_TacoMeteors = gameplayEvents.Add(new MeteorShowerEvent(
				ITC_TacoMeteorsID,
				35f,
				0.12f,
				METEORS.BOMBARDMENT_OFF.NONE,
				METEORS.BOMBARDMENT_ON.UNLIMITED,
				null,
				false)
				.AddMeteor(TacoCometConfig.ID, 0.15f));

			ITC_FakeTacoMeteors = gameplayEvents.Add(new MeteorShowerEvent(
				ITC_FakeTacoMeteorsID,
				Config.Instance.FakeTacoEventDuration,
				0.1f,
				METEORS.BOMBARDMENT_OFF.NONE,
				METEORS.BOMBARDMENT_ON.UNLIMITED,
				null,
				false)
				.AddMeteor(GhostlyTacoCometConfig.ID, 0.15f));

		}

		[HarmonyPatch(typeof(ComplexFabricatorSideScreen), nameof(ComplexFabricatorSideScreen.AnyRecipeRequirementsDiscovered))]
		public class ComplexFabricatorSideScreen_AnyRecipeRequirementsDiscovered_Patch
		{
			public static void Postfix(ComplexRecipe recipe, ref bool __result)
			{
				if (ChaosTwitch_SaveGameStorage.Instance == null)
					return;

				if (recipe.id == TacoDehydratedConfig.recipe.id)
					__result = ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe;
			}
		}
		[HarmonyPatch(typeof(PlayerController), "OnKeyDown")]
		public class PlayerController_OnKeyDown_Patch
		{
			public static void Prefix(KButtonEvent e)
			{
				if (ClusterManager.Instance == null)
					return;

				if (e.TryConsume(ModAssets.HotKeys.UnlockTacoRecipe.GetKAction()))
				{
					if (!ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe)
					{
						ChaosTwitch_SaveGameStorage.Instance.hasUnlockedTacoRecipe = true;
						ToastManager.InstantiateToast(STRINGS.HOTKEYACTIONS.UNLOCK_TACO_RECIPE_TITLE, STRINGS.HOTKEYACTIONS.UNLOCK_TACO_RECIPE_BODY);
					}
				}
				else if (e.TryConsume(ModAssets.HotKeys.TriggerTacoRain.GetKAction()))
				{
					TriggerGhostTacoMeteors();
				}
				else if (e.TryConsume(ModAssets.HotKeys.ToggleRainbowLiquid.GetKAction()))
				{
					ModAssets.RainbowLiquids = !ModAssets.RainbowLiquids;
				}
			}
		}

		static void TriggerGhostTacoMeteors()
		{
			int activeWorld = ClusterManager.Instance.activeWorldId;
			if (ClusterManager.Instance.activeWorld.IsModuleInterior)
			{
				activeWorld = 0;
			}
			GameplayEventInstance eventInstance = GameplayEventManager.Instance.StartNewEvent(TacoMeteorPatches.ITC_FakeTacoMeteors, activeWorld);
			if (Config.Instance.FakeTacoEventMusic)
				SoundUtils.PlaySound(ModAssets.SOUNDS.TACORAIN, SoundUtils.GetSFXVolume() * 0.3f, true);
		}



		[HarmonyPatch(typeof(FoodDehydratorConfig), "ConfigureRecipes")]
		public static class FoodDehydrator_ConfigureRecipes
		{
			public static void Postfix()
			{
				AddDehydriedTacoRecipe();
			}
			private static void AddDehydriedTacoRecipe()
			{
				var foodInfo = TacoConfig.foodInfo;
				var material = TacoDehydratedConfig.ID;

				RecipeElement[] input = new RecipeElement[2]
				{
					new RecipeElement(foodInfo, 6000000f / foodInfo.CaloriesPerUnit),
					new RecipeElement(SimHashes.Polypropylene.CreateTag(), 12f)
				};
				RecipeElement[] output = new RecipeElement[2]
				{
					new RecipeElement(material, 6f, ComplexRecipe.RecipeElement.TemperatureOperation.Dehydrated),
					new RecipeElement(SimHashes.Water.CreateTag(), 6f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
				};
				TacoDehydratedConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("FoodDehydrator", (IList<RecipeElement>)input, (IList<RecipeElement>)output), input, output)
				{
					time = 250f,
					nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom,
					customName = string.Format((string)global::STRINGS.BUILDINGS.PREFABS.FOODDEHYDRATOR.RECIPE_NAME, (object)foodInfo.Name),
					description = string.Format((string)global::STRINGS.BUILDINGS.PREFABS.FOODDEHYDRATOR.RESULT_DESCRIPTION, (object)foodInfo.Name),
					fabricators = new List<Tag>()
					{
						TagManager.Create("FoodDehydrator")
					},
					sortOrder = 28
				};

				//TacoDehydratedConfig.recipe.requiredTech = ModAssets.Techs.TacoTechID;
			}
		}


		[HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureRecipes")]
		public static class AddGasRangeRecipes
		{
			public static void Postfix()
			{
				AddTacoRecipe();
			}
			private static void AddTacoRecipe()
			{
				RecipeElement[] input = new RecipeElement[]
				{
					new RecipeElement((Tag) "ColdWheatSeed", 4f),
					new RecipeElement( TableSaltConfig.ID, 0.01f),
					new RecipeElement((Tag) "Lettuce", 1f),
					new RecipeElement((Tag) "CookedMeat", 1f),
					new RecipeElement((Tag) SpiceNutConfig.ID, 1f)
				};

				RecipeElement[] output = new RecipeElement[]
				{
					new RecipeElement(TacoConfig.ID, 1f)
				};

				string recipeID = ComplexRecipeManager.MakeRecipeID(GourmetCookingStationConfig.ID, input, output);

				TacoConfig.recipe = new ComplexRecipe(recipeID, input, output)
				{
					time = FOOD.RECIPES.STANDARD_COOK_TIME,
					description = STRINGS.ITEMS.FOOD.ICT_TACO.DESC,
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag> { GourmetCookingStationConfig.ID },
					sortOrder = 900
				};

				//TacoConfig.recipe.requiredTech = ModAssets.Techs.TacoTechID;
			}
		}
	}
}
