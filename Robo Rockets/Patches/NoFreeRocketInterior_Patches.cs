using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;

namespace RoboRockets.Patches
{
	internal class NoFreeRocketInterior_Patches
	{

		[HarmonyPatch(typeof(NoFreeRocketInterior), nameof(NoFreeRocketInterior.EvaluateCondition))]
		public class NoFreeRocketInterior_EvaluateCondition_Patch
		{
			public static void Postfix(NoFreeRocketInterior __instance, ref bool __result)
			{
				if (__result) 
					return;

				int worldCount = 0;
				foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
				{
					if (worldContainer.IsModuleInterior && !ModAssets.ForbiddenInteriorIDs.ContainsKey(worldContainer.id))
					{
						worldCount++;
					}
				}
				__result = worldCount < ClusterManager.MAX_ROCKET_INTERIOR_COUNT;
			}
		}
	}
}
