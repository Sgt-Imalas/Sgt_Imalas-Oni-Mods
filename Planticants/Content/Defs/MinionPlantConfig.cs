using Klei.AI;
using Planticants.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Planticants.Content.Defs
{
	class MinionPlantConfig : IEntityConfig
	{
		public string[] GetDlcIds() => null;

		public static Tag MODEL = ModTags.PlantMinion;
		public static string ID = ModTags.PlantMinion.ToString();
		public static string NAME = (string)STRINGS.DUPLICANTS.MODEL.PLANT.NAME;
		public Func<RationalAi.Instance, StateMachine.Instance>[] RATIONAL_AI_STATE_MACHINES = [
			(RationalAi.Instance smi) => new RadiationMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new ThoughtGraph.Instance(smi.master),
			(RationalAi.Instance smi) => new StressMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new EmoteMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new SneezeMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new DecorMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new IncapacitationMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new IdleMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new DoctorMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new SicknessMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new GermExposureMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new RoomMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new TemperatureMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new ExternalTemperatureMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new ScaldingMonitor.Instance(smi.master, new ScaldingMonitor.Def
			{
				defaultScaldingTreshold = 345f
			}),
			(RationalAi.Instance smi) => new ColdImmunityMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new HeatImmunityMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new LightMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new RedAlertMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new CringeMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new FallMonitor.Instance(smi.master, shouldPlayEmotes: true, "anim_emotes_default_kanim"),
			(RationalAi.Instance smi) => new WoundMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new SafeCellMonitor.Instance(smi.master, new SafeCellMonitor.Def()),
			(RationalAi.Instance smi) => new SuffocationMonitor.Instance(smi.master, new SuffocationMonitor.Def()),
			(RationalAi.Instance smi) => new MoveToLocationMonitor.Instance(smi.master, new MoveToLocationMonitor.Def()),
			(RationalAi.Instance smi) => new RocketPassengerMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new ReactionMonitor.Instance(smi.master, new ReactionMonitor.Def()),
			(RationalAi.Instance smi) => new SuitWearer.Instance(smi.master),
			(RationalAi.Instance smi) => new TubeTraveller.Instance(smi.master),
			(RationalAi.Instance smi) => new MingleMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new MournMonitor.Instance(smi.master),
			(RationalAi.Instance smi) => new SpeechMonitor.Instance(smi.master, new SpeechMonitor.Def()),
			(RationalAi.Instance smi) => new BlinkMonitor.Instance(smi.master, new BlinkMonitor.Def()),
			(RationalAi.Instance smi) => new ConversationMonitor.Instance(smi.master, new ConversationMonitor.Def()),
			(RationalAi.Instance smi) => new CoughMonitor.Instance(smi.master, new CoughMonitor.Def()),
			(RationalAi.Instance smi) => new GameplayEventMonitor.Instance(smi.master, new GameplayEventMonitor.Def()),
			(RationalAi.Instance smi) => new GasLiquidExposureMonitor.Instance(smi.master, new GasLiquidExposureMonitor.Def()),
			(RationalAi.Instance smi) => new InspirationEffectMonitor.Instance(smi.master, new InspirationEffectMonitor.Def()),
			(RationalAi.Instance smi) => new SlipperyMonitor.Instance(smi.master, new SlipperyMonitor.Def()),
			(RationalAi.Instance smi) => new PressureMonitor.Instance(smi.master, new PressureMonitor.Def()),
			(RationalAi.Instance smi) => new ThreatMonitor.Instance(smi.master, new ThreatMonitor.Def
			{
				fleethresholdState = DUPLICANTSTATS.GetStatsFor(smi.MinionModel).Combat.FLEE_THRESHOLD,
				offsets = BaseMinionConfig.ATTACK_OFFSETS
			}),
			(RationalAi.Instance smi) => new RecreationTimeMonitor.Instance(smi.master, new RecreationTimeMonitor.Def()),
			smi =>  new SteppedInMonitor.Instance(smi.master),

	 //smi =>  new BreathMonitor.Instance(smi.master),
	 //smi =>  new Dreamer.Instance(smi.master),
	 //smi =>  new StaminaMonitor.Instance(smi.master),
	 //smi =>  new RationMonitor.Instance(smi.master),
	// smi =>  new CalorieMonitor.Instance(smi.master),
	 //smi =>  new BladderMonitor.Instance(smi.master),
	// smi =>  new HygieneMonitor.Instance(smi.master),
	 //smi =>  new TiredMonitor.Instance(smi.master)
		];

		public static string[] GetAttributes() => (
		[
			Db.Get().Attributes.MaxUnderwaterTravelCost.Id,
			Db.Get().Attributes.DecorExpectation.Id,
			Db.Get().Attributes.RoomTemperaturePreference.Id,
			Db.Get().Attributes.CarryAmount.Id,
			Db.Get().Attributes.QualityOfLife.Id,
			Db.Get().Attributes.SpaceNavigation.Id,
			Db.Get().Attributes.Sneezyness.Id,
			Db.Get().Attributes.RadiationResistance.Id,
			Db.Get().Attributes.RadiationRecovery.Id,
			Db.Get().Attributes.TransitTubeTravelSpeed.Id,
			Db.Get().Attributes.Luminescence.Id,
			Db.Get().Attributes.ToiletEfficiency.Id
		]);

		public static string[] GetAmounts() => new string[] {
			Db.Get().Amounts.HitPoints.Id,
			Db.Get().Amounts.ImmuneLevel.Id,
			Db.Get().Amounts.Stress.Id,
			Db.Get().Amounts.Toxicity.Id,
			Db.Get().Amounts.Temperature.Id,
			Db.Get().Amounts.Decor.Id,
			Db.Get().Amounts.RadiationBalance.Id,
		}.Append<string>(PlantAmounts.GetAmountIDs());

		public static AttributeModifier[] GetTraits() => BaseMinionConfig.BaseMinionTraits(MODEL).Append(new AttributeModifier[]
		{
			new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, DUPLICANTSTATS.GetStatsFor(MODEL).BaseStats.FOOD_QUALITY_EXPECTATION, NAME),
			new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DUPLICANTSTATS.GetStatsFor(MODEL).BaseStats.MAX_CALORIES, NAME),
			new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, DUPLICANTSTATS.GetStatsFor(MODEL).BaseStats.CALORIES_BURNED_PER_SECOND, NAME),
			new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, DUPLICANTSTATS.GetStatsFor(MODEL).BaseStats.STAMINA_USED_PER_SECOND, NAME),
			new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, DUPLICANTSTATS.GetStatsFor(MODEL).BaseStats.BLADDER_INCREASE_PER_SECOND, NAME),
		});

		public GameObject CreatePrefab()
		{
			var minion = BaseMinionConfig.BaseMinion(MODEL, GetAttributes(), GetAmounts(), GetTraits());
			UnityEngine.Object.Destroy(minion.GetComponent<OxygenBreather>());

			return minion;
		}

		public void OnPrefabInit(GameObject go)
		{
			AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(go);
			amountInstance.value = amountInstance.GetMax();
			AmountInstance amountInstance2 = Db.Get().Amounts.Stress.Lookup(go);
			amountInstance2.value = 3f;
			AmountInstance amountInstance3 = Db.Get().Amounts.Temperature.Lookup(go);
			amountInstance3.value = DUPLICANTSTATS.GetStatsFor(MODEL).Temperature.Internal.IDEAL;
			PlantAmounts.InitializeDefaultValues(go);
		}

		public void OnSpawn(GameObject go)
		{
			Sensors component = go.GetComponent<Sensors>();
			BaseMinionConfig.BaseOnSpawn(go, MODEL, this.RATIONAL_AI_STATE_MACHINES);
			go.Trigger(1589886948, go);
		}
	}
}
