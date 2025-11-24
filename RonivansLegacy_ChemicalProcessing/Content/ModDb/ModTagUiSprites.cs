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
				else if(t == GameTags.Filter)
				{
					__result.first = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim("filter_media_kanim"));
					__result.second = Color.white;
				}
				else if (t == GameTags.AnyWater)
				{
					__result.first = Assets.GetSprite("AIO_AnyWater");
					__result.second = Color.white;
				}
			}
		}
	}
}
