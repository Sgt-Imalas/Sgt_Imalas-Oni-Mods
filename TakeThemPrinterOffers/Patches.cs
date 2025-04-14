using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace TakeThemPrinterOffers
{
	class Patches
	{

		/// <summary>
		/// These patches have to run manually or they break translations on certain screens
		/// </summary>
		[HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
		public static class OnASsetPrefabPatch
		{
			public static void Postfix()
			{
				ImmigrantScreen_Manual_Patch.AssetOnPrefabInitPostfix(Mod.HarmonyInstance);
			}
		}
		public class ImmigrantScreen_Manual_Patch
		{
			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_OnProceed = AccessTools.Method("ImmigrantScreen, Assembly-CSharp:OnProceed");
				var m_Initialize = AccessTools.Method("ImmigrantScreen, Assembly-CSharp:Initialize");

				var m_Prefix = AccessTools.Method(typeof(ImmigrantScreen_Manual_Patch), "OnProceed_Prefix");
				var m_Postfix = AccessTools.Method(typeof(ImmigrantScreen_Manual_Patch), "Init_Postfix");

				harmony.Patch(m_OnProceed, new HarmonyMethod(m_Prefix, Priority.Low));
				harmony.Patch(m_Initialize, postfix: new HarmonyMethod(m_Postfix, Priority.Low));
			}

			//[HarmonyPriority(Priority.Low)]
			public static void OnProceed_Prefix(ImmigrantScreen __instance)
			{
				if (__instance.telepad != null && __instance.selectedDeliverables.Count() > 1)
				{
					for (int i = 1; i < __instance.selectedDeliverables.Count(); i++) //game handles item at index 0
					{
						__instance.telepad.OnAcceptDelivery(__instance.selectedDeliverables[i]);
					}
					__instance.selectedDeliverables =[__instance.selectedDeliverables[0]];	
				}
			}
			public static void Init_Postfix(ImmigrantScreen __instance)
			{
				if (__instance.telepad != null)
				{
					__instance.selectableCount = 3;
				}
			}
		}
	}
}
