using HarmonyLib;
using SmartAreaFill.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAreaFill.Patches
{
	internal class DragTool_Patches
	{

        [HarmonyPatch(typeof(DragTool), nameof(DragTool.OnPrefabInit))]
        public class DragTool_OnPrefabInit_Patch
        {
            public static void Postfix(DragTool __instance)
            {
                __instance.gameObject.AddOrGet<SmartFillToolInjector>();
			}
		}

		[HarmonyPatch(typeof(BuildTool), nameof(DragTool.OnPrefabInit))]
		public class BuildTool_OnPrefabInit_Patch
		{
			public static void Postfix(BuildTool __instance)
			{
				__instance.gameObject.AddOrGet<SmartFillToolInjector>();
			}
		}


		[HarmonyPatch(typeof(DragTool), nameof(DragTool.OnLeftClickDown))]
        public class DragTool_OnLeftClickDown_Patch
        {
            public static void Postfix(DragTool __instance)
            {
                if(__instance.TryGetComponent<SmartFillToolInjector>(out var injector))
                {
                    injector.OnLeftClickDown();
				}
			}
        }

        [HarmonyPatch(typeof(DragTool), nameof(DragTool.OnLeftClickUp))]
        public class DragTool_OnLeftClickUp_Patch
        {
            public static void Prefix(DragTool __instance)
			{
				if (__instance.TryGetComponent<SmartFillToolInjector>(out var injector))
				{
					injector.OnLeftClickUp();
				}
			}
        }

        [HarmonyPatch(typeof(InterfaceTool), nameof(InterfaceTool.DeactivateTool))]
        public class DragTool_DeactivateTool_Patch
        {
            public static void Prefix(InterfaceTool __instance)
            {
                if (__instance.TryGetComponent<SmartFillToolInjector>(out var injector))
                {
                    injector.OnDeactivateTool();
				}
			}
        }

        [HarmonyPatch(typeof(DragTool), nameof(DragTool.CancelDragging))]
        public class DragTool_CancelDragging_Patch
        {
            public static void Prefix(DragTool __instance)
            {
				if (__instance.TryGetComponent<SmartFillToolInjector>(out var injector))
				{
					injector.OnDeactivateTool();
				}
			}
        }

	}
}
