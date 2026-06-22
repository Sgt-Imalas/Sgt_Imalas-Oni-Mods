using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class BaseMinion_Patches
	{
		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.BaseMinion))]
		public static class AddDeconstructGunOverrides
		{
			[HarmonyPrepare] public static bool Prepare() => Config.Instance.MoreToolFX;
			public static void Postfix(GameObject __result)
			{
				var snapon = __result.GetComponent<SnapOn>();
				if (snapon != null)
				{
					snapon.snapPoints.Add(new SnapOn.SnapPoint()
					{
						pointName = "dig",
						automatic = false,
						context = (HashedString)ModAssets.DeconstructToolContext,
						buildFile = Assets.GetAnim((HashedString)"constructor_gun_kanim"),
						overrideSymbol = (HashedString)"snapTo_rgtHand"
					});
				}
			}
		}


		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.SetupLaserEffects))]
		public static class RegisterDeconstructLaser
		{
			[HarmonyPrepare] public static bool Prepare() => Config.Instance.MoreToolFX;
			public static void Postfix(GameObject prefab)
			{
				var laserEffects = prefab.transform.Find("LaserEffect").gameObject;
				var kbatchedAnimEventToggler = laserEffects.GetComponent<KBatchedAnimEventToggler>();
				var kbac = prefab.GetComponent<KBatchedAnimController>();

				SoundUtils.CopySoundsToAnim("deconstruct_fx_kanim", "construct_beam_kanim");
				SoundUtils.CopySoundsToAnim("deconstruct_impact_kanim", "sparks_radial_build_kanim");
				//xxSoundUtils.DumpAllGetSounds();
				InjectionMethods.AddLaserEffect("DeconstructEffect", (HashedString)ModAssets.DeconstructToolContext, kbatchedAnimEventToggler, kbac, "deconstruct_fx_kanim", "beam");
			}
		}
	}
}
