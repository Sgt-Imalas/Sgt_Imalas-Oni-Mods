using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace OniRetroEdition.ModPatches
{
	internal class Lighting_Patches
	{

        [HarmonyPatch(typeof(Lighting), nameof(Lighting.Start))]
        public class Lighting_Awake_Patch
        {
            public static void Prefix(Lighting __instance)
            {

                __instance.Settings.DarkenTints[0] = Config.Instance.DarkenTints1;
                __instance.Settings.DarkenTints[1] = Config.Instance.DarkenTints2;
				__instance.Settings.DarkenTints[2] = Config.Instance.DarkenTints3;

				__instance.Settings.characterLighting.litColour = Config.Instance.CharacterLit;
				__instance.Settings.characterLighting.unlitColour = Config.Instance.CharacterUnLit;

                __instance.Settings.LightColour = Config.Instance.GlobalLightColor;
			}
        }
	}
}
