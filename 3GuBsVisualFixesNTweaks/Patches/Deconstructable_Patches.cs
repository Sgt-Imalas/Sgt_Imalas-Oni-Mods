using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class Deconstructable_Patches
	{

		[HarmonyPatch(typeof(Deconstructable), nameof(Deconstructable.OnPrefabInit))]
		public static class SwitchToolType
		{
			[HarmonyPrepare] public static bool Prepare() => Config.Instance.MoreToolFX;
			public static void Postfix(Deconstructable __instance)
			{
				__instance.multitoolContext = ModAssets.DeconstructToolContext;
				//__instance.multitoolHitEffectTag = DeconstructImpactEffect.ID; //broken; is offset on y axis and has no sound
			}
		}
	}
}
