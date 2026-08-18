using BionicBoostersPlus.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static STRINGS.LORE.BUILDINGS;

namespace BionicBoostersPlus.Patches
{
	internal class BionicBatteryMonitor_Patches
	{
		[HarmonyPatch(typeof(BionicBatteryMonitor.Instance), nameof(BionicBatteryMonitor.Instance.AddOrUpdateModifier))]
		public class BionicBatteryMonitor_Instance_AddOrUpdateModifier_Patch
		{
			public static void Postfix(BionicBatteryMonitor.Instance __instance) => UpdateEfficiencyMultiplier(__instance);
		}
		[HarmonyPatch(typeof(BionicBatteryMonitor.Instance), nameof(BionicBatteryMonitor.Instance.RemoveModifier))]
		public class BionicBatteryMonitor_Instance_RemoveModifier_Patch
		{
			public static void Postfix(BionicBatteryMonitor.Instance __instance) => UpdateEfficiencyMultiplier(__instance);
		}
		[HarmonyPatch(typeof(BionicBatteryMonitor.Instance), nameof(BionicBatteryMonitor.Instance.ApplyDifficultyModifiers))]
		public class BionicBatteryMonitor_Instance_ApplyDifficultyModifiers_Patch
		{
			public static void Postfix(BionicBatteryMonitor.Instance __instance) => UpdateEfficiencyMultiplier(__instance);
		}

		const string Circuits_WattageReduction = "BB_Circuits_WattageReduction";
		public static void UpdateEfficiencyMultiplier(BionicBatteryMonitor.Instance smi)
		{
			if (!smi.master.gameObject.TryGetComponent<MinionResume>(out var resume))
				return;

			var modifierList = smi.Modifiers;
			bool hasEfficiencyPerk = resume.HasPerk(BB_SkillPerks.BB_Circuits_WattageReduction);

			for (int i = modifierList.Count - 1; i >= 0; --i)
			{
				var modifier = modifierList[i];
				if (modifier.id == Circuits_WattageReduction)
				{
					modifierList.RemoveAt(i);
					break;
				}
			}
			float currentWattage = smi.Wattage;

			float reduction = (float)Math.Round(-currentWattage * BB_SkillPerks.CIRCUITS_WATTAGEREDUCTIONPERCENTAGE, 2);
			if (reduction > 0)
				reduction = 0;

			modifierList.Add(new BionicBatteryMonitor.WattageModifier(Circuits_WattageReduction, string.Format(BB_SkillPerks.WATTAGE_UI_TOOLTIP, reduction), reduction, reduction));
		}
	}
}
