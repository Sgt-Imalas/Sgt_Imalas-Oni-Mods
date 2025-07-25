using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class MaterialSelector_Patches
	{
		[HarmonyPatch(typeof(MaterialSelector), nameof(MaterialSelector.UpdateHeader))]
		public class MaterialSelector_UpdateHeader_Patch
		{
			public static void Postfix(MaterialSelector __instance)
			{
				LocText headerText = __instance.Headerbar.GetComponentInChildren<LocText>();
				string stripped = global::STRINGS.UI.StripLinkFormatting(headerText.text);
				float count = stripped.Length;
				int linecount = Mathf.CeilToInt(count / 40f);
				int height = linecount * 24;
				__instance.Headerbar.GetComponent<LayoutElement>().minHeight = height;
			}
		}
	}
}
