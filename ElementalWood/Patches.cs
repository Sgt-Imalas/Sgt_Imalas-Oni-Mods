using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ElementLoader;

namespace ElementalWood
{
	internal class Patches
	{
		/// <summary>
		/// change wood transition target to coal
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.CollectElementsFromYAML))]
		public static class Patch_ElementLoader_CollectElementsFromYAML
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix(List<ElementEntry> __result)
			{
				var wood = __result.FirstOrDefault(ele => ele.elementId == nameof(SimHashes.WoodLog));
				if (wood != null)
				{
					wood.highTempTransitionTarget = nameof(SimHashes.Carbon);
					wood.highTemp = UtilMethods.GetKelvinFromC(200);
					SgtLogger.l("Changed wood transition target to "+wood.highTempTransitionTarget);
				}
			}
		}
	}
}
