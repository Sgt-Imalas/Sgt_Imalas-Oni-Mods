using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class VerticalModuleTiler_Patches
	{
		[HarmonyPatch(typeof(VerticalModuleTiler), nameof(VerticalModuleTiler.HasWideNeighbor))]
		public class VerticalModuleTiler_HasWideNeighbor_Patch
		{
			public static void Postfix(VerticalModuleTiler __instance, int neighbour_cell, ref bool __result)
			{
				if (__instance == null || __instance.gameObject == null)
					return;

				GameObject otherModule = Grid.Objects[neighbour_cell, (int)__instance.objectLayer];


				if (otherModule == null || !otherModule.TryGetComponent<BuildingFacade>(out var facade) || facade.IsOriginal)
					return;

				if (SkinDatabase.SkinOverridesWidth(facade.CurrentFacade, out bool isWideOverride))
					__result = isWideOverride;
			}
		}
	}
}
