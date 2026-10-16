using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class ProgressBarsConfig_Patches
	{

		[HarmonyPatch(typeof(Lighting), nameof(Lighting.Start))]
		public class Lighting_Start_Patch
		{
			public static void Postfix(Lighting __instance)
			{
				SgtLogger.l ("Lighting.Start Postfix");
				SgtLogger.l("Lit: " + ((Color)__instance.Settings.characterLighting.litColour).ToString());
				SgtLogger.l("UnLit: " + ((Color)__instance.Settings.characterLighting.unlitColour).ToString());
				SgtLogger.l("number of darkentints: "+__instance.Settings.DarkenTints.Length);
				for(int i = 0; i < __instance.Settings.DarkenTints.Length; i++)
				{
					SgtLogger.l("DarkenTint["+i+"]: "+__instance.Settings.DarkenTints[i].ToString());
					__instance.Settings.DarkenTints[i] = UIUtils.Darken(__instance.Settings.DarkenTints[i], 50);
				}

				//__instance.Settings.characterLighting.unlitColour.b = 166;
				//__instance.Settings.characterLighting.unlitColour = UIUtils.Darken(__instance.Settings.characterLighting.unlitColour, 50);
			}
		}


        [HarmonyPatch(typeof(ProgressBarsConfig), nameof(ProgressBarsConfig.Initialize))]
        public class ProgressBarsConfig_Initialize_Patch
        {
            public static void Postfix(ProgressBarsConfig __instance)
            {
                SgtLogger.l("PROGBAR: ");
				UIUtils.ListAllChildrenPath(__instance.progressBarPrefab.transform);
				UIUtils.ListAllChildrenWithComponents(__instance.progressBarPrefab.transform);
                SgtLogger.l("HELFBAR: ");
				UIUtils.ListAllChildrenPath(__instance.healthBarPrefab.transform);
				UIUtils.ListAllChildrenWithComponents(__instance.healthBarPrefab.transform);
				var image = __instance.progressBarPrefab.transform.Find("RawImage").GetComponent<RawImage>();
				if(image == null)
				{
					SgtLogger.l("PROGBAR: RawImage not found!");
					return;
				}
				var sprite = Assets.GetSprite("progressBar_test2");
				if(sprite == null)
				{
					SgtLogger.l("PROGBAR: Sprite not found!");
					return;
				}
				image.texture = sprite.texture;

			}
        }
	}
}
