using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvisibleClothings.Patches
{

	/// <summary>
	/// logic mirrored over from Outfit Override mod (https://steamcommunity.com/sharedfiles/filedetails/?id=3674972348) for personal use, overriding ALL outfits
	/// </summary>
	internal class WearableAccessorizer_Patches
	{

		[HarmonyPatch(typeof(WearableAccessorizer), nameof(WearableAccessorizer.ApplyWearable))]
		public class WearableAccessorizer_ApplyWearable_Patch
		{
			public static void Prefix(WearableAccessorizer __instance)
			{
				if (__instance.Wearables.TryGetValue(WearableAccessorizer.WearableType.CustomClothing, out var wearable))
					wearable.buildOverridePriority = 6;
			}
		}

		[HarmonyPatch(typeof(WearableAccessorizer), nameof(WearableAccessorizer.GetHighestAccessory))]
		public class WearableAccessorizer_GetHighestAccessory_Patch
		{
			public static void Postfix(WearableAccessorizer __instance, ref WearableAccessorizer.WearableType __result)
			{
				if (!__instance.Wearables.Any()
					|| !__instance.Wearables.ContainsKey(WearableAccessorizer.WearableType.CustomClothing) 
					|| !__instance.Wearables.ContainsKey(WearableAccessorizer.WearableType.Outfit)
					|| __instance.Wearables.ContainsKey(WearableAccessorizer.WearableType.Suit)
					|| __instance.Wearables.ContainsKey(WearableAccessorizer.WearableType.CustomSuit)
					)
					return;
				__result = WearableAccessorizer.WearableType.CustomClothing;
			}
		}
	}
}
