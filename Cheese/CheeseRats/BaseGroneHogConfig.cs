using Cheese.ModElements;
using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Cheese.CheeseRats
{
	internal class BaseGroneHogConfig
	{
		public const string NavGridId = "WalkerNavGrid1x1";
		public const string NavGridBabyId = "WalkerBabyNavGrid";
		public const float Mass = 100.0f;
		public const float MoveSpeed = 2.0f;
		public const string OnDeathDropId = "Meat";
		public const int OnDeathDropCount = 1;

		public const string SpeciesId = "GroneHogSpecies";

		public const float DefaultTemperature = 320f;
		public const float TemperatureLethalLow = 258.15f;
		public const float TemperatureWarningLow = 308.15f;
		public const float TemperatureWarningHigh = 358.15f;
		public const float TemperatureLethalHigh = 448.15f;

		public static GameObject BaseGroneHog(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null)
		{
			GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id, name, desc, Mass, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures,
				width: 1,
				height: 1,
				TUNING.DECOR.BONUS.TIER1, new EffectorValues(), SimHashes.Creature, null, DefaultTemperature);
			string NavGridName = NavGridId;
			if (is_baby)
				NavGridName = NavGridBabyId;
			EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, traitId, NavGridName, NavType.Floor, 32,
				moveSpeed: MoveSpeed,
				onDeathDropID: OnDeathDropId,
				onDeathDropCount: OnDeathDropCount,
				false, false,
				lethalLowTemperature: TemperatureLethalLow,
				warningLowTemperature: TemperatureWarningLow,
				warningHighTemperature: TemperatureWarningHigh,
				lethalHighTemperature: TemperatureLethalHigh);
			if (symbolOverridePrefix != null)
				placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix, null, 0);
			placedEntity.AddOrGet<Trappable>();
			placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
			placedEntity.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Alright;
			placedEntity.AddWeapon(1f, 1f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.Single, 1, 0.0f);
			EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true, false);
			KPrefabID component = placedEntity.GetComponent<KPrefabID>();
			component.AddTag(GameTags.Creatures.Walker, false);
			component.prefabInitFn += (inst => inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost));
			bool condition = !is_baby;
			ChoreTable.Builder chore_table = new ChoreTable.Builder()
				.Add(new DeathStates.Def(), true)
				.Add(new AnimInterruptStates.Def(), true)
				.Add(new GrowUpStates.Def(), true)
				.Add(new TrappedStates.Def(), true)
				.Add(new IncubatingStates.Def(), true)
				.Add(new BaggedStates.Def(), true)
				.Add(new FallStates.Def(), true)
				.Add(new StunnedStates.Def(), true)
				.Add(new DebugGoToStates.Def(), true)
				.Add(new FleeStates.Def(), true)
				.Add(new AttackStates.Def(), condition).PushInterruptGroup()
				.Add(new CreatureSleepStates.Def(), true)
				.Add(new FixedCaptureStates.Def(), true)
				//.Add(new RanchedStates.Def(), true) //missing anim crashes
				//.Add(new LayEggStates.Def(), true)
				.Add(new EatStates.Def(), true)
				.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP), true)
				.Add(new CallAdultStates.Def(), true).PopInterruptGroup()
				.Add(new IdleStates.Def(), true);
			EntityTemplates.AddCreatureBrain(placedEntity, chore_table, SpeciesId, symbolOverridePrefix);
			return placedEntity;
		}
		public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos, float referenceCaloriesPerKg, float minPoopSizeInKg)
		{
			Diet diet = new Diet(diet_infos.ToArray());
			CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
			def.diet = diet;
			def.minConsumedCaloriesBeforePooping = referenceCaloriesPerKg * minPoopSizeInKg;
			prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
			return prefab;
		}
		public static List<Diet.Info> BasicCheeseDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
		{
			return new List<Diet.Info>()
			{
				new Diet.Info(new HashSet<Tag>()
				{
					ModElementRegistration.Cheese.SimHash.CreateTag()
				}, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, false)
			};
		}
	}
}
