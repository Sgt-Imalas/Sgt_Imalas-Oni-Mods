using _3GuBsVisualFixesNTweaks.Defs.Entities.FX;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class Diggable_Patches
	{
		[HarmonyPatch(typeof(Diggable), nameof(Diggable.UpdateColor))]
		public static class UpdateHitEffect
		{
			[HarmonyPrepare]public static bool Prepare() => Config.Instance.MoreToolFX;


			static bool HasOrIsTag(Element e, Tag tag)
			{
				return e.HasTag(tag) || e.GetMaterialCategoryTag() == tag;
			}
			public static void Postfix(Diggable __instance)
			{
				if (__instance.childRenderer == null)
					return;

				Material material = __instance.childRenderer.material;
				int targetCell = Grid.PosToCell(__instance.gameObject);
				Element element = Grid.Element[targetCell];

				if (Diggable.RequiresTool(element) || Diggable.Undiggable(element))
					return;

				if (element.HasTag(GameTags.IceOre))
				{
					__instance.multitoolHitEffectTag = DigIceEffect.ID;
				}
				else if (HasOrIsTag(element, GameTags.Metal) || HasOrIsTag(element, GameTags.RefinedMetal))
				{
					__instance.multitoolHitEffectTag = DigMetalEffect.ID;
				}
				else if (HasOrIsTag(element, GameTags.BuildableRaw))
				{
					__instance.multitoolHitEffectTag = DigRockEffect.ID;
				}
				else if (element.IsUnstable)
				{
					__instance.multitoolHitEffectTag = DigRubbleEffect.ID;
				}
			}
		}
	}
}
