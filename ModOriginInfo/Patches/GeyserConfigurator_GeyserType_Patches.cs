using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using static GeyserConfigurator;

namespace ModOriginInfo.Patches
{
	internal class GeyserConfigurator_GeyserType_Patches
	{
		[HarmonyPatch]
		public static class ComplexRecipe_Constructors_Patch
		{
			[HarmonyTargetMethods]
			public static List<MethodBase> TargetMethods()
			{
				var  list = new List<MethodBase>();
				foreach(var con in typeof(GeyserType).GetConstructors(AccessTools.all))
					list.Add(con);
				return list;
			}

			[HarmonyPostfix]
			public static void Postfix(GeyserType __instance)
			{
				var assembly = Assembly.GetCallingAssembly();
				ModAssets.RegisterGeyser(assembly, __instance);
			}
		}
	}
}
