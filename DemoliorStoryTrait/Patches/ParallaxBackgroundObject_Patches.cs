using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoliorStoryTrait.Patches
{
    class ParallaxBackgroundObject_Patches
    {
		[HarmonyPatch(typeof(ParallaxBackgroundObject), nameof(ParallaxBackgroundObject.Initialize))]
		public class ParallaxBackgroundObject_Initialize_Patch
		{
			public static void Prefix(ParallaxBackgroundObject __instance, ref string texture)
			{
				if (Config.Instance.PipReplaceDemoliorSprite) 
					texture = "ImpactorPip";
			}
		}
	}
}
