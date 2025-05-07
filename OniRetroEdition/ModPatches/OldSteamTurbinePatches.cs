using HarmonyLib;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
	internal class OldSteamTurbinePatches
	{
		[HarmonyPatch(typeof(SteamTurbineConfig), nameof(SteamTurbineConfig.DoPostConfigureComplete))]
		public class SteamTurbineConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				Tinkerable.MakePowerTinkerable(go);
			}
		}

		[HarmonyPatch(typeof(Turbine), nameof(Turbine.ResolveStrings))]
		public static class TurbineStringFix
		{
			public static void Postfix(ref string __result, object data)
			{
				Turbine turbine = (Turbine)data;
				__result = __result.Replace("{Src_Element}", ElementLoader.FindElementByHash(turbine.srcElem).name);
				__result = __result.Replace("{Active_Temperature}", GameUtil.GetFormattedTemperature(turbine.minActiveTemperature));
			}
		}
		[HarmonyPatch(typeof(Turbine), nameof(Turbine.InitializeStatusItems))]
		public static class TurbineStringFix2
		{
			public static void Postfix()
			{
				SgtLogger.l("patching turbine status");
				Turbine.insufficientMassStatusItem.resolveTooltipCallback = delegate (string str, object data)
				{
					Turbine turbine = (Turbine)data;
					str = str.Replace("{Min_Mass}", GameUtil.GetFormattedMass(turbine.requiredMassFlowDifferential));
					str = str.Replace("{Src_Element}", ElementLoader.FindElementByHash(turbine.srcElem).name);
					return str;
				};
				Turbine.insufficientMassStatusItem.resolveStringCallback = delegate (string str, object data)
				{
					Turbine turbine = (Turbine)data;
					str = str.Replace("{Min_Mass}", GameUtil.GetFormattedMass(turbine.requiredMassFlowDifferential));
					str = str.Replace("{Src_Element}", ElementLoader.FindElementByHash(turbine.srcElem).name);
					return str;
				};
			}
		}
	}
}
