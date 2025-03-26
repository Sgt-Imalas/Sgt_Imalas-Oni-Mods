using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class BatteryCharger_Patches
	{
		[HarmonyPatch(typeof(ElectrobankDischarger.States), nameof(ElectrobankDischarger.States.InitializeStates))]
		public class ElectrobankDischarger_States_InitializeStates_Patch
		{
			public static void Postfix(ElectrobankDischarger.States __instance)
			{
				__instance.inoperational.enterActions.Clear();
				__instance.inoperational
					.PlayAnim("on")
					.Enter((smi) => smi.master.UpdateMeter())
					.EnterTransition(__instance.discharging_pst, (smi) => smi.master.storage.items.Count == 0);
				__instance.discharging_pst.OnAnimQueueComplete(__instance.noBattery);
				__instance.discharging_pst.Exit(smi => smi.master.UpdateSymbolSwap());
			}
		}

		/// <summary>
		/// Prevent the anim override clearing when the battery gets ejected to keep it in the pst anim
		/// </summary>
		[HarmonyPatch(typeof(ElectrobankDischarger), nameof(ElectrobankDischarger.UpdateSymbolSwap))]
		public class ElectrobankDischarger_UpdateSymbolSwap_Patch
		{
			public static bool Prefix(ElectrobankDischarger __instance)
			{
				return __instance.storage.items.Count > 0;
			}
		}

		[HarmonyPatch(typeof(ElectrobankCharger), nameof(ElectrobankCharger.InitializeStates))]
		public class ElectrobankCharger_InitializeStates_Patch
		{
			public static void Postfix(ElectrobankCharger __instance)
			{
				__instance.inoperational.enterActions.Clear();
				__instance.inoperational
					.PlayAnim("working_pst")
					.QueueAnim("on");
			}
		}

		[HarmonyPatch(typeof(ElectrobankCharger.Instance), nameof(ElectrobankCharger.Instance.UpdateMeter))]
		public class ElectrobankCharger_Instance_UpdateMeter_Patch
		{
			public static void Postfix(ElectrobankCharger.Instance __instance)
			{
				UpdateSymbolSwap(__instance);
			}
			public static void UpdateSymbolSwap(ElectrobankCharger.Instance smi)
			{
				KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
				SymbolOverrideController component2 = component.GetComponent<SymbolOverrideController>();
				//component.SetSymbolVisiblity("electrobank_l", is_visible: false);
				if (smi.Storage.items.Any())
				{
					KAnim.Build.Symbol source_symbol = smi.Storage.items[0].GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.symbols[0];
					component2.AddSymbolOverride("electrobank_l", source_symbol);
				}
				else
				{
					component2.RemoveSymbolOverride("electrobank_l");
				}
			}
		}

		/// <summary>
		/// Register SOC
		/// </summary>
		[HarmonyPatch(typeof(ElectrobankChargerConfig), nameof(ElectrobankChargerConfig.DoPostConfigureComplete))]
		public class ElectrobankChargerConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				SymbolOverrideControllerUtil.AddToPrefab(go);
			}
		}
	}
}
