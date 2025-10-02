using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.BuildingDefPatches
{
	internal class OxidizerTankLiquidClusterConfig_Patches
	{
		[HarmonyPatch(typeof(OxidizerTankLiquidClusterConfig), nameof(OxidizerTankLiquidClusterConfig.DoPostConfigureComplete))]
		public static class SwapTagInLiquidOxidizer
		{
			/// <summary>
			/// swap tag of lox module to the mod's cryofuel tag
			/// </summary>
			/// <param name="go"></param>
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Postfix(GameObject go)
			{
				go.GetComponent<ConduitConsumer>().capacityTag = ModAssets.Tags.LOXTankOxidizer;
				go.GetComponent<Storage>().storageFilters = new List<Tag> { ModAssets.Tags.LOXTankOxidizer };
			}
		}
	}
}
