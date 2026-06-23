using AquaticMinnowMinion.Content.Defs;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;
using static TUNING.DUPLICANTSTATS;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Amounts
	{
		public static Amount Aquatic_GillMoisture;

		internal static void RegisterAmounts(Database.Amounts instance)
		{
			SgtLogger.l("Registering AquaticAmounts...");

			Aquatic_GillMoisture = instance.CreateAmount(nameof(Aquatic_GillMoisture), 0f, AQ_TUNING.GILL_MOISTURE.MAX, false, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_wet");
			Aquatic_GillMoisture.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));

			instance.Add(Aquatic_GillMoisture);
		}
		public static List<Amount> GetAmounts()
		{
			return
			[
				Aquatic_GillMoisture,
			];
		}
		public static List<string> GetAmountIDs()
		{
			return
			[
				nameof(Aquatic_GillMoisture),
			];
		}
		public static void InitializeDefaultValues(GameObject go)
		{
			AmountInstance moisture = Aquatic_GillMoisture.Lookup(go);
			moisture.value = 100;
		}

		internal static List<AttributeModifier> GetBaseModifiers()
		{

			return [
				new AttributeModifier(Aquatic_GillMoisture.deltaAttribute.Id, AQ_TUNING.GILL_MOISTURE.MOISTURE_BASE_DELTA, MinionAquaticConfig.NAME),
			];
		}
	}
}
