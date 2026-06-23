using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;
using static STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.UI.SANDBOXTOOLS.SETTINGS;

namespace AquaticMinnowMinion.Patches
{
	internal class OxygenBreather_Patches
	{

		[HarmonyPatch(typeof(OxygenBreather), nameof(OxygenBreather.BreathableGasConsumed), [typeof(SimHashes), typeof(float), typeof(float), typeof(byte), typeof(int)])]
		public class OxygenBreather_BreathableGasConsumed_Patch
		{
			static Dictionary<SimHashes, bool> IsItchyLiquid = [];
			public static void Postfix(OxygenBreather __instance, SimHashes elementConsumed, float massConsumed)
			{
				if (!IsItchyLiquid.TryGetValue(elementConsumed, out var itchy))
				{
					var element = ElementLoader.FindElementByHash(elementConsumed);
					itchy = IsItchyLiquid[elementConsumed] = element.HasTag(ModTags.PollutedLiquid);
				}
				if (itchy && !__instance.prefabID.HasTag(GameTags.Dead) && __instance.O2Accumulator != HandleVector<int>.Handle.InvalidHandle)
				{
					__instance.BoxingTrigger((int)GameHashes.PoorAirQuality, massConsumed);
					__instance.BoxingTrigger((int)ModAssets.PoorBreathableLiquidQuality, massConsumed);
				}
			}
		}


		[HarmonyPatch(typeof(OxygenBreather), nameof(OxygenBreather.OnSpawn))]
		public class OxygenBreather_OnSpawn_Patch
		{
			public static void Postfix(OxygenBreather __instance)
			{
				if (__instance.gameObject.PrefabID() != ModTags.AquaticMinion)
					return;
				__instance.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, true);
				__instance.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, true);
				//remove co2 too for ordering
				__instance.o2StatusItem = __instance.selectable.AddStatusItem(Aq_StatusItems.BreathingInAquatic, __instance);
				__instance.cO2StatusItem = __instance.selectable.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, __instance);
			}
		}
	}
}
