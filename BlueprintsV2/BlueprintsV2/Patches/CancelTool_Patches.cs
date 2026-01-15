using BlueprintsV2.BlueprintsV2.BlueprintData.LiquidInfo;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.Patches
{
	internal class CancelTool_Patches
	{


		[HarmonyPatch(typeof(CancelTool), nameof(CancelTool.GetDefaultFilters))]
		public class CancelTool_GetDefaultFilters_Patch
		{
			public static void Postfix(CancelTool __instance, Dictionary<string, ToolParameterMenu.ToggleState> filters)
			{
				filters.Add(ElementPlanInfo.FILTERLAYER, ToolParameterMenu.ToggleState.Off);
			}
		}

		//[HarmonyPatch(typeof(CancelTool), nameof(CancelTool.OnPrefabInit))]
		//public class CancelTool_OnPrefabInit_Patch
		//{
		//	public static void Postfix(CancelTool __instance)
		//	{
		//		var pos = __instance.transform.position;
		//		pos.y += 40f;
		//		__instance.transform.SetPosition(pos);
		//	}
		//}

		[HarmonyPatch(typeof(FilteredDragTool), nameof(FilteredDragTool.GetFilterLayerFromGameObject))]
		public class FilteredDragTool_GetFilterLayerFromGameObject_Patch
		{
			public static void Postfix(FilteredDragTool __instance, GameObject input, ref string __result)
			{
				if (input.TryGetComponent<ElementPlanInfo>(out _))
				{
					__result = ElementPlanInfo.FILTERLAYER;
				}
			}
		}
	}
}
