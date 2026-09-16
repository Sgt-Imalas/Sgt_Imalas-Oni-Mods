using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace ModOriginInfo.Patches
{
	internal class ComplexRecipe_Patches
	{
		[HarmonyPatch]
		public static class ComplexRecipe_Constructors_Patch
		{
			[HarmonyTargetMethods]
			public static List<MethodBase> TargetMethods()
			{
				var  list = new List<MethodBase>();
				foreach(var con in typeof(ComplexRecipe).GetConstructors())
					list.Add(con);
				return list;
			}

			[HarmonyPostfix]
			public static void Postfix(ComplexRecipe __instance)
			{
				var assembly = Assembly.GetCallingAssembly();
				ModAssets.RegisterRecipe(assembly, __instance);
			}
		}
	}
}
