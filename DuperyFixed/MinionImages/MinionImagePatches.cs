using HarmonyLib;
using UnityEngine;

namespace DuperyFixed.MinionImages
{
	internal class MinionImagePatches
	{
		[HarmonyPatch(typeof(Personality))]
		[HarmonyPatch(nameof(Personality.GetMiniIcon))]
		public class ReplaceMissingDreamIcons
		{
			[HarmonyPriority(Priority.HigherThanNormal)]
			public static void Postfix(Personality __instance, ref Sprite __result)
			{
				if (__result == null)
				{
					__result = ModAssets.GetDynamicDreamImage(__instance);
				}
			}
		}
	}
}
