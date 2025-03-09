using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Cheese.CheeseRats
{
	internal class GroneHogConfig : IEntityConfig
	{

		public const string Id = "Gronehog";
		public const string BaseTraitId = "GronehogBaseTrait";
		public const string Name = "Cheese Rat";
		public const string PluralName = "Cheese Rats";

		public const float Hitpoints = 25f;
		public const float Lifespan = 50f;
		public const float FertilityCycles = 30f;
		public const float IncubationCycles = 10f;

		public static int PenSizePerCreature = TUNING.CREATURES.SPACE_REQUIREMENTS.TIER3;
		public const float CaloriesPerCycle = 120000.0f;
		public const float StarveCycles = 5.0f;
		public const float StomachSize = CaloriesPerCycle * StarveCycles;

		public const float KgEatenPerCycle = 140.0f;
		public const float MinPoopSizeInKg = 25.0f;
		public static float CaloriesPerKg = GroneHogTuning.STANDARD_CALORIES_PER_CYCLE / KgEatenPerCycle;
		public static float ProducedConversionRate = TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_1;
		public const int EggSortOrder = 700;
		public string[] GetDlcIds() => null;

		public static GameObject CreateGroneHog(string id, string name, string desc, string anim_file, bool is_baby)
		{
			GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseGroneHogConfig.BaseGroneHog(id, name, desc, anim_file, BaseTraitId, is_baby, null), GroneHogTuning.PEN_SIZE_PER_CREATURE, true);

			Trait trait = Db.Get().CreateTrait(BaseTraitId, name, name, null, false, null, true, true);
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, GroneHogTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float)(-GroneHogTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name, false, false, true));
			trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, Hitpoints, name, false, false, true));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, Lifespan, name, false, false, true));

			List<Diet.Info> diet_infos = BaseGroneHogConfig.BasicCheeseDiet(
				SimHashes.MilkIce.CreateTag(),
				CaloriesPerKg,
				ProducedConversionRate, null, 0.0f);
			BaseGroneHogConfig.SetupDiet(wildCreature, diet_infos, CaloriesPerKg, MinPoopSizeInKg);

			return wildCreature;
		}
		public GameObject CreatePrefab()
		{
			return CreateGroneHog(Id, "Cheese Rat", "Cheesy", "gronehog_kanim", false);
			//GameObject rollerSnake = CreateGroneHog(Id, "Cheese Rat", "Cheesy", "rollersnake_kanim", false);
			//return EntityTemplates.ExtendEntityToFertileCreature(rollerSnake, EggId, EggName, Description, "rollersnakeegg_kanim", RollerSnakeTuning.EGG_MASS, BabyRollerSnakeConfig.Id, FertilityCycles, IncubationCycles, RollerSnakeTuning.EGG_CHANCES_BASE, EggSortOrder, true, false, true, 1f);
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
