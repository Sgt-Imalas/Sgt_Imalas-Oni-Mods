using HarmonyLib;
using MassMoveTo.Tools;
using MassMoveTo.Tools.SweepByType;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MassMoveTo.Patches
{
	internal class PlayerController_Patches
	{

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.OnPrefabInit))]
        public class PlayerController_OnPrefabInit_Patch
        {
			public static void Postfix(PlayerController __instance)
			{
				var interfaceTools = new List<InterfaceTool>(__instance.tools);

				var moveToSelectTool = new GameObject(nameof(FilteredMoveSelectTool), typeof(FilteredMoveSelectTool));
				moveToSelectTool.transform.SetParent(__instance.gameObject.transform);
				moveToSelectTool.gameObject.SetActive(true);
				moveToSelectTool.gameObject.SetActive(false);
				interfaceTools.Add(moveToSelectTool.GetComponent<FilteredMoveSelectTool>());

				var moveToTargetTool = new GameObject(typeof(TargetSelectTool).Name, typeof(TargetSelectTool));
				moveToTargetTool.transform.SetParent(__instance.gameObject.transform);
				moveToTargetTool.gameObject.SetActive(true);
				moveToTargetTool.gameObject.SetActive(false);
				interfaceTools.Add(moveToTargetTool.GetComponent<TargetSelectTool>());
				__instance.tools = interfaceTools.ToArray();
			}
		}
	}
}
