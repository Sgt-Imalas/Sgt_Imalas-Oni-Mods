using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	internal class ProgressBarsConfig_Patches
	{



 //       [HarmonyPatch(typeof(ProgressBarsConfig), nameof(ProgressBarsConfig.Initialize))]
 //       public class ProgressBarsConfig_Initialize_Patch
 //       {
 //           public static void Postfix(ProgressBarsConfig __instance)
 //           {
 //               SgtLogger.l("PROGBAR: ");
	//			UIUtils.ListAllChildrenPath(__instance.progressBarPrefab.transform);
	//			UIUtils.ListAllChildrenWithComponents(__instance.progressBarPrefab.transform);
 //               SgtLogger.l("HELFBAR: ");
	//			UIUtils.ListAllChildrenPath(__instance.healthBarPrefab.transform);
	//			UIUtils.ListAllChildrenWithComponents(__instance.healthBarPrefab.transform);
	//			var image = __instance.progressBarPrefab.transform.Find("RawImage").GetComponent<RawImage>();
	//			if(image == null)
	//			{
	//				SgtLogger.l("PROGBAR: RawImage not found!");
	//				return;
	//			}
	//			var sprite = Assets.GetSprite("progressBar_test2");
	//			if(sprite == null)
	//			{
	//				SgtLogger.l("PROGBAR: Sprite not found!");
	//				return;
	//			}
	//			image.texture = sprite.texture;

	//		}
 //       }
	}
}
