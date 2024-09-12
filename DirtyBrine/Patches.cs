using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace DirtyBrine
{
	internal class Patches
	{
		/// <summary>
		/// only code needed 
		/// </summary>
		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
			{
				var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();

				var brine = list.Find(e => e.elementID == SimHashes.Brine);

				var pWater = list.Find(e => e.elementID == SimHashes.DirtyWater);
				var saltWater = list.Find(e => e.elementID == SimHashes.SaltWater);

				brine.colour = Color.Lerp(saltWater.colour, pWater.colour, 0.5f);
				brine.uiColour = Color.Lerp(saltWater.uiColour, pWater.uiColour, 0.5f);
			}
		}
	}
}
