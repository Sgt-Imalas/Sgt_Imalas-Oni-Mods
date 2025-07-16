using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class ModTagUiSprites
	{
		internal static void SetMissingTagSprites(object item, ref Tuple<Sprite, Color> __result)
		{
			if (__result.first.name == "unknown" && item is Tag t)
			{
				if (t == GameTags.CombustibleGas)
				{
					__result.first = Assets.GetSprite("ui_combustible_gases");
					__result.second = Color.white;
				}
				else if (t == GameTags.CombustibleLiquid)
				{
					__result.first = Assets.GetSprite("ui_combustible_liquids");
					__result.second = Color.white;
				}
				else if (t == GameTags.CombustibleSolid)
				{
					__result.first = Assets.GetSprite("ui_combustible_solids");
					__result.second = Color.white;
				}
				else if (t == ModAssets.Tags.BioOil_Composition)
					__result = Def.GetUISprite(ElementLoader.FindElementByHash(ModElements.VegetableOil_Liquid));
				else if (t == ModAssets.Tags.Biodiesel_Composition)
					__result = Def.GetUISprite(ElementLoader.FindElementByHash(ModElements.BioDiesel_Liquid));
			}
		}
	}
}
