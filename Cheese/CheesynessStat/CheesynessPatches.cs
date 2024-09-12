using HarmonyLib;
using Klei.AI;
using UnityEngine;

namespace Cheese.CheesynessStat
{
	internal class CheesynessPatches
	{
		public static Amount Cheesyness;


		[HarmonyPatch(typeof(Database.Amounts), nameof(Database.Amounts.Load))]
		public class Database_Amounts_TargetMethod_Patch
		{
			public static void Postfix(Database.Amounts __instance)
			{
				Cheesyness = __instance.CreateAmount(nameof(Cheesyness), 0f, 100f, true, Units.Flat, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_battery");
				Cheesyness.SetDisplayer(new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));
				__instance.Add(Cheesyness);
			}
		}
		[HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.AddMinionAmounts))]
		public class MinionConfig_AddMinionAmounts
		{
			public static void Postfix(Modifiers modifiers)
			{
				modifiers.initialAmounts.Add(Cheesyness.Id);
			}
		}
		[HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.OnPrefabInit))]
		public class MinionConfig_OnPrefabInit
		{
			public static void Postfix(GameObject go)
			{
				AmountInstance cheesynessInstance = Cheesyness.Lookup(go);
				cheesynessInstance.value = 0;
			}
		}
		[HarmonyPatch(typeof(MinionVitalsPanel), nameof(MinionVitalsPanel.Init))]
		public class MinionVitalsPanel_Init
		{
			public static void Postfix(MinionVitalsPanel __instance)
			{
				AccessTools.Method(typeof(MinionVitalsPanel), "AddAmountLine").Invoke(__instance, new object[] { Cheesyness, null });
				//__instance.AddAmountLine(Cheesyness);
			}
		}
	}
}
