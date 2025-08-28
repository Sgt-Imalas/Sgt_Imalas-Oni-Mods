using HarmonyLib;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Disease_Patches
	{
		[HarmonyPatch]
		public static class Diseases_PopulateElemGrowthInfo_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(Disease __instance)
			{
				foreach(var element in ElementLoader.elements)
				{
					if (element.HasTag("Antiseptic"))
					{
						SgtLogger.l(__instance.Name + " dies on " + element.name);
						__instance.AddGrowthRule(GermUtils.DieInElement(element.id));
					}
				}
			}

			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(Disease.PopulateElemGrowthInfo);
				yield return typeof(ZombieSpores).GetMethod(name,AccessTools.all);
				yield return typeof(SlimeGerms).GetMethod(name, AccessTools.all);
				yield return typeof(FoodGerms).GetMethod(name, AccessTools.all);
			}
		}
	}
}
