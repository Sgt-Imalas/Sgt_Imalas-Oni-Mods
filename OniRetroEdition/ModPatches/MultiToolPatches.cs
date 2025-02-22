using HarmonyLib;
using OniRetroEdition.FX;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
	internal class MultiToolPatches
	{
		public static string DeconstructTool = "deconstruct";

		[HarmonyPatch(typeof(Deconstructable), nameof(Deconstructable.OnPrefabInit))]
		public static class SwitchToolType
		{
			public static void Postfix(Deconstructable __instance)
			{
				__instance.multitoolContext = DeconstructTool;
				//__instance.multitoolHitEffectTag = DeconstructImpactEffect.ID; //broken; is offset on y axis and has no sound
			}
		}

		[HarmonyPatch(typeof(Diggable), nameof(Diggable.UpdateColor))]
		public static class UpdateHitEffect
		{
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

		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.BaseMinion))]
		public static class AddDeconstructGunOverrides
		{
			public static void Postfix(GameObject __result)
			{
				var snapon = __result.GetComponent<SnapOn>();
				if (snapon != null)
				{
					snapon.snapPoints.Add(new SnapOn.SnapPoint()
					{
						pointName = "dig",
						automatic = false,
						context = (HashedString)DeconstructTool,
						buildFile = Assets.GetAnim((HashedString)"constructor_gun_kanim"),
						overrideSymbol = (HashedString)"snapTo_rgtHand"
					});
				}
			}
		}


		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.SetupLaserEffects))]
		public static class RegisterDeconstructLaser
		{
			public static void Postfix(GameObject prefab)
			{
                var laserEffects = prefab.transform.Find("LaserEffect").gameObject;
                var kbatchedAnimEventToggler = laserEffects.GetComponent<KBatchedAnimEventToggler>();
                var kbac = prefab.GetComponent<KBatchedAnimController>();

				SoundUtils.CopySoundsToAnim("deconstruct_fx_kanim", "construct_beam_kanim");
				SoundUtils.CopySoundsToAnim("deconstruct_impact_kanim", "sparks_radial_build_kanim");
				SoundUtils.GetSounds();
				InjectionMethods.AddLaserEffect("DeconstructEffect", (HashedString)DeconstructTool, kbatchedAnimEventToggler, kbac, "deconstruct_fx_kanim", "beam");
			}
		}
	}
}
