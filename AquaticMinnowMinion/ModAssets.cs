using AquaticMinnowMinion.Content.ModDb;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace AquaticMinnowMinion
{
	internal class ModAssets
	{
		public static void TriggerDrinkMoisturization(GameObject minion)
		{
			if (minion == null || !minion.HasTag(ModAssets.Tags.AquaticMinion))
				return;

			if (minion.TryGetComponent<Effects>(out var effects))
				effects.Add(Aq_Effects.RefreshingDrink, true);
		}

		internal static void StartMoisturizing(WorkerBase worker)
		{
			if (worker != null)
				worker.Trigger(AqHashes.StartedMoisturizingTask);
		}

		public static class Tags
		{
			public static Tag AquaticMinion = TagManager.Create("AquaticMinion", STRINGS.DUPLICANTS.MODEL.AQUATIC.NAME);
			public static Tag BreathableWater = TagManager.Create("AM_OxygenizedLiquid", STRINGS.MISC.TAGS.AM_OXYGENIZEDLIQUID);
			public static Tag PollutedLiquid = TagManager.Create("AM_PollutedLiquid", STRINGS.MISC.TAGS.AM_POLLUTEDLIQUID);
		}
		public static class AqHashes
		{
			public static UtilLibs.ModHashes
			  PoorBreathableLiquidQuality = new("Aq_" + nameof(PoorBreathableLiquidQuality))
			, StartedBreathingLiquid = new("Aq_" + nameof(StartedBreathingLiquid))
			, StoppedBreathingLiquid = new("Aq_" + nameof(StoppedBreathingLiquid))
			, StartedMoisturizingTask = new("Aq_" + nameof(StartedMoisturizingTask))
			;
		}
	}
}
