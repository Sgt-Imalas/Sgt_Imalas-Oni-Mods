using HarmonyLib;

namespace Rockets_TinyYetBig.Patches
{
	internal class CritterTemperatureMonitor_Patches
	{

        [HarmonyPatch(typeof(CritterTemperatureMonitor.Instance), nameof(CritterTemperatureMonitor.Instance.IsEntirelyInVaccum))]
        public class CritterTemperatureMonitor_Instance_IsEntirelyInVaccum_Patch
		{
			/// <summary>
			/// If the critter is in a cargo bay, it should always fall back to its internal temperature
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="__result"></param>
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix(CritterTemperatureMonitor.Instance __instance, ref bool __result)
			{
				if (__instance.gameObject.HasTag(GameTags.Creatures.TrappedInCargoBay))
					__result = true;
			}
		}

		[HarmonyPatch(typeof(CritterTemperatureMonitor.Instance), nameof(CritterTemperatureMonitor.Instance.GetTemperatureExternal))]
		public static class CritterTemperatureMonitor_GetTemperatureExternal_Patch
		{
			public static bool Prefix(CritterTemperatureMonitor.Instance __instance, ref float __result)
			{
				if (!__instance.gameObject.HasTag(GameTags.Creatures.TrappedInCargoBay))
					return true;

				__result = __instance.GetTemperatureInternal();
				return false;
			}
		}
	}
}
