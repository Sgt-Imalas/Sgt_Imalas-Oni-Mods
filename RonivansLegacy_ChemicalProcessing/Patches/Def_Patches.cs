using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class Def_Patches
	{

		[HarmonyPatch(typeof(Def), nameof(Def.GetUISprite), [typeof(object), typeof(string), typeof(bool)])]
		public class Def_GetUISprite_Patch
		{
			public static void Postfix(object item, ref Tuple<Sprite, Color> __result)
			{
				ModTagUiSprites.SetMissingTagSprites(item, ref __result);
			}
		}
	}
}
