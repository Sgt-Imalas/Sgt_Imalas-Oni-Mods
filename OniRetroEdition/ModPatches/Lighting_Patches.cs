using ElementUtilNamespace;
using HarmonyLib;
using OniRetroEdition.Behaviors;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
	internal class Lighting_Patches
	{

        [HarmonyPatch(typeof(Lighting), nameof(Lighting.Start))]
        public class Lighting_Awake_Patch
        {
            public static void Prefix(Lighting __instance)
            {
                SgtLogger.l("Patching Retro Lighting");
				ModAssets.ApplyLegacyLighting();
				//SgtLogger.l($"DarkenTints1: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.DarkenTints[0]) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.DarkenTints1)}");
				//SgtLogger.l($"DarkenTints2: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.DarkenTints[0]) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.DarkenTints2)}");
				//SgtLogger.l($"DarkenTints3: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.DarkenTints[0]) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.DarkenTints3)}");
				//SgtLogger.l($"CharacterLit: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.characterLighting.litColour) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.CharacterLit)}");
				//SgtLogger.l($"CharacterUnLit: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.characterLighting.unlitColour) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.CharacterUnLit)}");
				//SgtLogger.l($"LightColour: #{ColorUtility.ToHtmlStringRGB(__instance.Settings.LightColour) + " -> #" + ColorUtility.ToHtmlStringRGB(Config.Instance.GlobalLightColor)}");
				//__instance.Settings.DarkenTints[0] = Config.Instance.DarkenTints1;
				//            __instance.Settings.DarkenTints[1] = Config.Instance.DarkenTints2;
				//__instance.Settings.DarkenTints[2] = Config.Instance.DarkenTints3;

				//__instance.Settings.characterLighting.litColour = Config.Instance.CharacterLit;
				//__instance.Settings.characterLighting.unlitColour = Config.Instance.CharacterUnLit;

				//            __instance.Settings.LightColour = Config.Instance.GlobalLightColor;

			}

			[HarmonyPatch(typeof(FloorLampConfig), nameof(FloorLampConfig.DoPostConfigureComplete))]
			public class FloorLampConfig_DoPostConfigureComplete_Patch
			{
				public static void Postfix(GameObject go)
				{
					if(go.TryGetComponent<StateMachineController>(out var smc))
					{
						smc.cmpdef.defs.RemoveAll(def => def is LightController.Def);
					}
					go.AddOrGetDef<RetroOni_LightController.Def>();
				}
			}
        }
	}
}
