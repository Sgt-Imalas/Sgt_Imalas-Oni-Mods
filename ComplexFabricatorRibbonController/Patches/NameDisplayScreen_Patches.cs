//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UtilLibs;

///Experiment for adding numbers to progress bars

//namespace ComplexFabricatorRibbonController.Patches
//{
//    class NameDisplayScreen_Patches
//    {
//        static GameObject TextPrefab;
//        [HarmonyPatch(typeof(NameDisplayScreen), nameof(NameDisplayScreen.OnSpawn))]
//        public class NameDisplayScreen_OnSpawn_Patch
//        {
//            public static void Postfix(NameDisplayScreen __instance)
//            {
//				TextPrefab = Util.KInstantiateUI(__instance.nameAndBarsPrefab);
//                UIUtils.FindAndDestroy(TextPrefab.transform, "Bars");

//				SgtLogger.l("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
//                UIUtils.ListAllChildren(__instance.nameAndBarsPrefab.transform);
//                UIUtils.ListAllChildrenWithComponents(__instance.nameAndBarsPrefab.transform);

//			}
//        }

//        [HarmonyPatch(typeof(ProgressBar), nameof(ProgressBar.OnSpawn))]
//        public class ProgressBar_OnSpawn_Patch
//        {
//            public static void Postfix(ProgressBar __instance)
//			{
//                if (__instance is HealthBar)
//                    return;

//				SgtLogger.l("BBBBBBBBBBBBBBBBB");
//				UIUtils.ListAllChildren(__instance.transform);
//				UIUtils.ListAllChildrenWithComponents(__instance.transform);

//                var progText = Util.KInstantiateUI(TextPrefab, __instance.gameObject, true);
//                var locText = progText.transform.Find("Name/Label").GetComponent<LocText>();
//                locText.SetText("26 s");
//                SgtLogger.l("SCALE: " + progText.transform.localScale.ToString());
//                SgtLogger.l("FONT:"+locText.fontSize.ToString());
//				progText.transform.localScale = Vector3.one*3.5f;
//                var rect = progText.rectTransform();
//                rect.anchorMin = new Vector2(0.5f, 0.5f);
//                rect.anchorMax = new Vector2(0.5f, 0.5f);
//                rect.pivot = new Vector2(0, 0f);

//               // rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 50, 150);
//			}
//		}
//		[HarmonyPatch(typeof(HealthBar), nameof(HealthBar.OnSpawn))]
//		public class HealthBar_OnSpawn_Patch
//		{
//			public static void Postfix(ProgressBar __instance)
//			{
//				SgtLogger.l("CCCCCCCCCCCCCC");
//				UIUtils.ListAllChildren(__instance.transform);
//				UIUtils.ListAllChildrenWithComponents(__instance.transform);

//				var progText = Util.KInstantiateUI(TextPrefab, __instance.gameObject, true);
//				var locText = progText.transform.Find("Name/Label").GetComponent<LocText>();
//				locText.SetText("26 s");
//				SgtLogger.l("SCALE: " + progText.transform.localScale.ToString());
//				SgtLogger.l("FONT:" + locText.fontSize.ToString());
//				progText.transform.localScale = Vector3.one * 3.5f;
//				var rect = progText.rectTransform();
//				rect.anchorMin = new Vector2(0.5f, 0.5f);
//				rect.anchorMax = new Vector2(0.5f, 0.5f);
//				rect.pivot = new Vector2(0, 0f);

//				// rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 50, 150);
//			}
//		}
//	}
//}
