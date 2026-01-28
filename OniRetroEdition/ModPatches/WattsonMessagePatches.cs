using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static BestFit;

namespace OniRetroEdition.ModPatches
{
	internal class WattsonMessagePatches
	{
		static GameObject welcomeDupe;
		static RectTransform expandPanel;


		[HarmonyPatch(typeof(WattsonMessage), nameof(WattsonMessage.PauseAndShowMessage))]
		public class WattsonMessage_PauseAndShowMessage_Patch
		{
			public static void Postfix(WattsonMessage __instance)
			{
				__instance.StartCoroutine(ExpandDupe());
			}
		}

		[HarmonyPatch(typeof(WattsonMessage), nameof(WattsonMessage.OnActivate))]
		public class WattsonMessage_OnActivate_Patch
		{
			public static void Postfix(WattsonMessage __instance)
			{
				__instance.button.onClick += delegate
				{
					__instance.StartCoroutine(CollapsePanel());
				};
			}
		}
		static IEnumerator CollapsePanel()
		{
			float height = 300f;
			while (height > 1f)
			{
				height = Mathf.Lerp(height, 0f, Time.unscaledDeltaTime * 15f);
				expandPanel.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, height);
				yield return 0;
			}
			yield return null;
		}
		public static IEnumerator ExpandDupe()
		{
			yield return SequenceUtil.WaitForSecondsRealtime(0.199f);
			float height = 5f;
			welcomeDupe.SetActive(true);
			while (height <= 299f)
			{
				height = Mathf.Lerp(height, 300f, Time.unscaledDeltaTime * 15f);
				expandPanel.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, height);
				yield return 0;
			}
			yield return null;
		}

		[HarmonyPatch(typeof(WattsonMessage), nameof(WattsonMessage.OnPrefabInit))]
		private static class ReaddWattsonDupe
		{
			public static void Postfix(WattsonMessage __instance)
			{
				var bg = __instance.transform.Find("Dialog/BG").gameObject;

				welcomeDupe = Util.KInstantiateUI(bg, bg.transform.parent.parent.gameObject, true);

				welcomeDupe.TryGetComponent<Image>(out var image);
				var rect = welcomeDupe.rectTransform();
				expandPanel = rect;
				rect.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, 350);
				rect.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, 300);
				rect.anchoredPosition = new(-460, 10);

				image.color = Color.white;
				image.sprite = Assets.GetSprite("welcomeDialog_guy");
				welcomeDupe.SetActive(false);
			}
		}
	}
}
