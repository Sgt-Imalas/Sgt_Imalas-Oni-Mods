using AquaticMinnowMinion.Content.ModDb;
using AquaticMinnowMinion.Content.Scripts;
using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UnityEngine;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;
using static AquaticMinnowMinion.STRINGS;

namespace AquaticMinnowMinion.Content.Defs
{
	internal class MinionAquaticConfig : IEntityConfig, IHasDlcRestrictions
	{
		public string[] GetForbiddenDlcIds() => null;
		public string[] GetRequiredDlcIds() => [DlcManager.DLC5_ID];

		public static Tag MODEL = Tags.AquaticMinion;
		public static string NAME = DUPLICANTS.MODEL.AQUATIC.NAME;
		public static string ID = MODEL.ToString();
		public Func<RationalAi.Instance, StateMachine.Instance>[] RATIONAL_AI_STATE_MACHINES = BaseMinionConfig.BaseRationalAiStateMachines().Append<Func<RationalAi.Instance, StateMachine.Instance>>(new Func<RationalAi.Instance, StateMachine.Instance>[]
		{
			(smi => new BreathMonitor.Instance(smi.master)),
			(smi => new SteppedInMonitor.Instance(smi.master)),
			(smi => new Dreamer.Instance(smi.master)),
			(smi => new StaminaMonitor.Instance(smi.master)),
			(smi => new RationMonitor.Instance(smi.master)),
			(smi => new CalorieMonitor.Instance(smi.master)),
			(smi => new BladderMonitor.Instance(smi.master)),
			(smi => new HygieneMonitor.Instance(smi.master)),
			(smi => new TiredMonitor.Instance(smi.master)),
			(smi => new GillIrritationMonitor.Instance(smi.master, new GillIrritationMonitor.Def())),
			(smi => new WaterBreathingEfficiencyMonitor.Instance(smi.master)),
			(smi => new DryGillsMonitor.Instance(smi.master)),
			(smi => new GillsMoistureMonitor.Instance(smi.master)),
		});
		public static string[] GetAttributes()
		{
			return BaseMinionConfig.BaseMinionAttributes().Append<string>(new string[2]
			{
				Db.Get().Attributes.FoodExpectation.Id,
				Db.Get().Attributes.ToiletEfficiency.Id
			});
		}

		public static string[] GetAmounts()
		{
			return BaseMinionConfig.BaseMinionAmounts().Append<string>(new string[3]
			{
				Db.Get().Amounts.Bladder.Id,
				Db.Get().Amounts.Stamina.Id,
				Db.Get().Amounts.Calories.Id
			}).Append<string>(Aq_Amounts.GetAmountIDs());
		}
		public static AttributeModifier[] GetTraits()
		{
			var baseStats = DUPLICANTSTATS.GetStatsFor(MODEL).BaseStats;
			return BaseMinionConfig.BaseMinionTraits(MODEL).Append<AttributeModifier>(new AttributeModifier[]
			{
				new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, baseStats.FOOD_QUALITY_EXPECTATION, NAME),
				new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, baseStats.MAX_CALORIES, NAME),
				new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, baseStats.CALORIES_BURNED_PER_SECOND, NAME),
				new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, baseStats.STAMINA_USED_PER_SECOND, NAME),
				new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, baseStats.BLADDER_INCREASE_PER_SECOND, NAME),
				new AttributeModifier(Db.Get().Attributes.ToiletEfficiency.Id, baseStats.TOILET_EFFICIENCY, NAME),
				new AttributeModifier(Db.Get().Attributes.ThermalConductivityBarrier.Id, 0.008f, NAME)
			}).Append(Aq_Amounts.GetBaseModifiers());
		}


		public GameObject CreatePrefab()
		{
			GameObject go = BaseMinionConfig.BaseMinion(MODEL, GetAttributes(), GetAmounts(), GetTraits());
			go.AddOrGet<CodexEntryRedirector>().CodexID = "DUPLICANTS";
			return go;
		}


		public void OnPrefabInit(GameObject go)
		{
			BaseMinionConfig.BasePrefabInit(go, MODEL);
			DUPLICANTSTATS statsFor = DUPLICANTSTATS.GetStatsFor(MODEL);
			Db.Get().Amounts.Bladder.Lookup(go).value = UnityEngine.Random.Range(0.0f, 10f);
			AmountInstance amountInstance1 = Db.Get().Amounts.Calories.Lookup(go);
			amountInstance1.value = ((statsFor.BaseStats.HUNGRY_THRESHOLD + statsFor.BaseStats.SATISFIED_THRESHOLD) * 0.5f) * amountInstance1.GetMax();
			AmountInstance amountInstance2 = Db.Get().Amounts.Stamina.Lookup(go);
			amountInstance2.value = amountInstance2.GetMax();
			Aq_Amounts.InitializeDefaultValues(go);
		}

		public void OnSpawn(GameObject go)
		{
			Sensors component = go.GetComponent<Sensors>();
			component.Add((Sensor)new ToiletSensor(component));
			BaseMinionConfig.BaseOnSpawn(go, MODEL, this.RATIONAL_AI_STATE_MACHINES);

			var safeCellSensor = component.GetSensor<SafeCellSensor>();
			safeCellSensor.AddIgnoredFlagsSet(ID, SafeCellQuery.SafeFlags.IsNotLiquidOnMyFace | SafeCellQuery.SafeFlags.IsNotLiquid | SafeCellQuery.SafeFlags.IsNotSwimming);

			go.GetComponent<OxygenBreather>().AddGasProvider((OxygenBreather.IGasProvider)new GasOrWaterBreatherFromWorldProvider());

			if(go.TryGetComponent<MinionResume>(out var resume))
			{
				var db = Db.Get().SkillPerks;
				SkillPerk[] swimPerks = [
					db.CanSwim,
					db.ImprovedLiquidTemperatureTolerance,
					db.ReduceSaltWaterSwimmingEyeIrritation,
					db.IncreaseSwimmerStaminaInLiquid,
					db.IncreaseSwimmerAthleticsInLiquid,
					];
				//SgtLogger.l("Aquatic effect addition");
				resume.ApplyAdditionalSkillPerks(swimPerks);
			}
			if(go.TryGetComponent<Effects>(out var effects))
			{
				///Immunities to liquid induced hazards
				effects.AddImmunity(Db.Get().effects.Get("SoakingWet"), NAME);
				effects.AddImmunity(Db.Get().effects.Get("WetFeet"), NAME);
				effects.AddImmunity(Db.Get().effects.Get("RecentlySlippedTracker"), NAME);
			}

			go.Trigger((int)GameHashes.MinionSpawned, (object)go);
			///ensure that the eye irritation immunity goes into effect!
			go.Trigger((int)GameHashes.AssignedRoleChanged);
		}
	}
}
