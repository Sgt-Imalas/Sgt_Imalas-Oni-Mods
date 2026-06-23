using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Patches
{
	internal class ElementLoader_Patches
	{
		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.FinaliseElementsTable))]
		public class ElementLoader_FinaliseElementsTable_Patch
		{
			public static void Postfix()
			{
				ElementAdjustments.DoModifications();
			}
		}
	}
}
