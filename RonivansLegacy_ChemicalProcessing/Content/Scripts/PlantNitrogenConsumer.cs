using Klei.AI;
using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.BUILDING.STATUSITEMS;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class PlantNitrogenConsumer : KMonoBehaviour, ISim200ms
	{
		public static readonly float NitrogenConsumedPerSecond = 0.020f;//20 g/s
		public static readonly float GrowthBoost = 0.20f;//20%
		public static readonly string CodexMapKey = "PlantNitrogenConsumer_CodexId";
		ElementConsumer nitrogenConsumer;
		[Serialize] public bool ConsumptionSatisfied = false;
		[MyCmpGet] Growing growing;
		[MyCmpGet] Effects effects;

		static Effect Nitrogenized = null;

		static void InitEffect()
		{
			if (Nitrogenized != null) return;

			string desc = string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.TOOLTIP, GameUtil.GetFormattedMass(NitrogenConsumedPerSecond, GameUtil.TimeSlice.PerSecond));
			Nitrogenized = new Effect("AIO_Nitrogenized", STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.NAME, desc, 0, true, false, false);
			Nitrogenized.Add(new AttributeModifier(Db.Get().Amounts.Maturity.deltaAttribute.Id, GrowthBoost, STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.NAME, true));
			Nitrogenized.Add(new AttributeModifier(Db.Get().Amounts.Maturity2.deltaAttribute.Id, GrowthBoost, STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.NAME, true));
			Db.Get().effects.Add(Nitrogenized);
		}

		public override void OnPrefabInit()
		{
			InitEffect();

			nitrogenConsumer = gameObject.AddComponent<ElementConsumer>();
			nitrogenConsumer.elementToConsume = ModElements.Nitrogen_Gas;
			nitrogenConsumer.consumptionRate = NitrogenConsumedPerSecond;
			nitrogenConsumer.consumptionRadius = (byte)3;
			nitrogenConsumer.showInStatusPanel = false;
			nitrogenConsumer.sampleCellOffset = new Vector3(0.0f, 0.0f, 0.0f);
			nitrogenConsumer.isRequired = false;
			nitrogenConsumer.storeOnConsume = false;
			nitrogenConsumer.showDescriptor = false;
			nitrogenConsumer.ignoreActiveChanged = false;

			base.OnPrefabInit();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			UpdateEffect();
		}

		public void Sim200ms(float dt)
		{
			nitrogenConsumer.EnableConsumption(growing.smi.IsInsideState(growing.smi.sm.growing));
			bool hasConsumedMass = nitrogenConsumer.consumedMass > 0;
			if (ConsumptionSatisfied != hasConsumedMass)
			{
				ConsumptionSatisfied = hasConsumedMass;
				UpdateEffect();
			}
			nitrogenConsumer.consumedMass = 0;
		}
		void UpdateEffect()
		{
			if (ConsumptionSatisfied)
			{
				effects?.Add(Nitrogenized, false);
			}
			else
			{
				effects?.Remove(Nitrogenized);
			}
		}
	}
}
