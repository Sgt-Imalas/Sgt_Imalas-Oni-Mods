using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SafeCellQuery_Patches
	{

        [HarmonyPatch(typeof(SafeCellQuery), nameof(SafeCellQuery.GetFlags))]
        public class SafeCellQuery_GetFlags_Patch
        {
            public static void Postfix(SafeCellQuery __instance, int cell, SafeCellQuery.SafeFlags __result)
            {
                //if its an acid, consider it irradiated; this makes dupes avoid acid areas
                if (Grid.Element[cell].HasTag(ModAssets.Tags.AIO_Acid))
					__result = __result & ~SafeCellQuery.SafeFlags.IsNotRadiated;
			}
        }
	}
}
