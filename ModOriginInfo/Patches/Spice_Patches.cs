using Database;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UtilLibs;

namespace ModOriginInfo.Patches
{
	internal class Spice_Patches
	{
		[HarmonyPatch]
		public static class ComplexRecipe_Constructors_Patch
		{
			[HarmonyTargetMethods]
			public static List<MethodBase> TargetMethods()
			{
				var list = new List<MethodBase>();
				foreach (var con in typeof(Spice).GetConstructors(AccessTools.all))
					list.Add(con);
				return list;
			}

			[HarmonyPostfix]
			public static void Postfix(Spice __instance)
			{
				var assembly = Assembly.GetCallingAssembly();
				ModAssets.RegisterSpice(assembly, __instance);
			}
		}

		[HarmonyPatch(typeof(SpiceGrinder.Option), nameof(SpiceGrinder.Option.CreateDescription))]
		public class SpiceGrinder_Option_CreateDescription_Patch
		{
			public static void Postfix(SpiceGrinder.Option __instance)
			{
				string id = __instance.GetID().ToString();
				if (ModAssets.IsModded(id, out _))
				{
					SgtLogger.l(id + " is modded");
					__instance.fullDescription = __instance.spiceDescription + ModAssets.GetModNameIfValid(id, 2) + __instance.ingredientDescriptions;
				}
			}
		}
	}
}
