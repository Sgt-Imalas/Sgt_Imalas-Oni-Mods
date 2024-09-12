using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cheese.CheeseGerms
{
	internal class GermPatches
	{
		[HarmonyPatch(typeof(Diseases))]
		[HarmonyPatch(MethodType.Constructor)]
		[HarmonyPatch(new Type[] { typeof(ResourceSet), typeof(bool) })]
		public class Diseases_Constructor_Patch
		{
			public static void Prefix()
			{
				Assets.instance.DiseaseVisualization.info.Add(new DiseaseVisualization.Info() { name = CheeseMakingBacteria.ID, overlayColourName = CheeseMakingBacteria.ID });
			}

			public static void Postfix(ref Diseases __instance, bool statsOnly)
			{
				__instance.Add(new CheeseMakingBacteria(statsOnly));
			}
		}
		[HarmonyPatch(typeof(ColorSet))]
		[HarmonyPatch("Init")]
		public static class ColorSet_Init_Patch
		{
			static bool initalized = false;

			public static void Postfix(ColorSet __instance)
			{
				if (initalized)
					return;

				Dictionary<string, Color32> namedLookup = Traverse.Create(__instance).Field("namedLookup").GetValue<Dictionary<string, Color32>>();
				namedLookup.Add(CheeseMakingBacteria.ID, ModAssets.CheeseColor);

				initalized = true;
			}
		}


	}
}
