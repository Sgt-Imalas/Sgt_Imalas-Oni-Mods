using ElementUtilNamespace;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static AttackProperties;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Enum_Patches
    {
		/// <summary>
		/// Required for Simhashes conversion to string to include the modded elements
		/// </summary>
		// Credit: Heinermann (Blood mod)
		public static class EnumPatch
		{

			public static void ManualTestPatch(Harmony harmony)
			{
				foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()
				.Where(a => a.GetName().Name == "0Harmony"))
				{
					Debug.Log($"Harmony loaded: {asm.FullName} @ {asm.Location}");
				}

				Debug.Log("Attempting to patch Enum.ToString method...");

				var original = AccessTools.Method(typeof(Enum), "InternalFormat");
				var prefix = new HarmonyMethod(typeof(SimHashes_ToString_Patch), nameof(SimHashes_ToString_Patch.Prefix));
				harmony.Patch(original, prefix: prefix);

			}

			//[HarmonyPatch(typeof(Enum), "InternalFormat")]
			//[HarmonyPatch]
			public class SimHashes_ToString_Patch
			{
				//[HarmonyTargetMethod]
				//public static MethodBase Get_Enum_ToString()
				//{
				//	var targetType = typeof(System.Enum);
				//	var method = AccessTools.Method(targetType, nameof(Enum.ToString), new Type[] { });
				//	if(method != null)
				//	{
				//		return method;
				//	}
				//	method = AccessTools.Method(targetType, nameof(Enum.ToString));
				//	if (method != null)
				//	{
				//		return method;
				//	}
				//	SgtLogger.error("Failed to find Enum.ToString method?");
				//	return null;
				//}

				//[HarmonyPrefix]
				public static bool Prefix(object eT, object value, ref string __result) => SgtElementUtil.SimHashInternalFormat_EnumPatch(eT, value, ref __result);
			}

			[HarmonyPatch(typeof(Enum), nameof(Enum.Parse), [typeof(Type), typeof(string), typeof(bool)])]
			private class SimHashes_Parse_Patch
			{
				private static bool Prefix(Type enumType, string value, ref object __result) => SgtElementUtil.SimhashParse_EnumPatch(enumType, value, ref __result);
			}
		}
	}
}
