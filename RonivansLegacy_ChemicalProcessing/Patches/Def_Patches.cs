using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class Def_Patches
	{

		[HarmonyPatch(typeof(Def), nameof(Def.GetUISprite), [typeof(object), typeof(string), typeof(bool)])]
		public class Def_GetUISprite_Patch
		{
			public static void Postfix(object item, Tuple<Sprite, Color> __result)
			{
				if(__result.first.name == "unknown" && item is Tag t)
				{
					if(t == GameTags.CombustibleGas)
					{
						__result.first = Assets.GetSprite("ui_combustible_gases");
						__result.second = Color.white;
					} 
					else if (t == GameTags.CombustibleLiquid)
					{
						__result.first = Assets.GetSprite("ui_combustible_liquids");
						__result.second = Color.white;
					}
				}
			}
		}
	}
}
