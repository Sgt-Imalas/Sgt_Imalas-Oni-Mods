using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class CodexConversionPanel_Patches
	{

		[HarmonyPatch(typeof(CodexConversionPanel), nameof(CodexConversionPanel.ConfigureConversion))]
		public class CodexConversionPanel_ConfigureConversion_Patch
		{
			public static void Postfix(CodexConversionPanel __instance)
			{
				// has -10 spacing which causes overlaps in certain recipes, e.g. sour water stripper
				__instance.ingredientsContainer.GetComponent<HorizontalLayoutGroup>()?.spacing = 0;
			}
		}
	}
}
