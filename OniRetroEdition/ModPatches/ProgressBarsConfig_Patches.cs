using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
	internal class ProgressBarsConfig_Patches
	{



		[HarmonyPatch(typeof(ProgressBarsConfig), nameof(ProgressBarsConfig.Initialize))]
		public class ProgressBarsConfig_Initialize_Patch
		{
			static bool Initialized = false;

			static void MakeProgressBarRetro(GameObject prefab)
			{
				var rawImage = prefab.transform.Find("RawImage").GetComponent<RawImage>();
				var img = prefab.transform.Find("Bar").GetComponent<Image>();
				img.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 330);

				if (rawImage == null)
				{
					SgtLogger.l("PROGBAR: RawImage not found!");
					return;
				}
				var newTexture = Assets.GetSprite("retro_progressbar").texture;
				RectTransform rectTransform = rawImage.rectTransform;

				float currentHeight = rectTransform.rect.height;
				float aspectRatio = (float)newTexture.width / newTexture.height;
				float sizeIncrease = 1;


				rawImage.texture = newTexture;

				rectTransform.SetSizeWithCurrentAnchors(
					RectTransform.Axis.Horizontal,
					currentHeight * aspectRatio * sizeIncrease
				);
				rectTransform.SetSizeWithCurrentAnchors(
					RectTransform.Axis.Vertical,
					currentHeight * sizeIncrease
				);

				var offset = new Vector2(25f, 0f);

				rectTransform.anchoredPosition += offset;
				img.rectTransform().anchoredPosition += new Vector2(75f, 0);


				var fg = Util.KInstantiateUI<RawImage>(rawImage.gameObject);
				fg.transform.SetParent(rawImage.transform.parent, false);
				fg.gameObject.SetActive(true);
				fg.texture = Assets.GetSprite("retro_progressbar_fg").texture;
				fg.rectTransform().anchoredPosition = rectTransform.anchoredPosition;
			}

			public static void Postfix(ProgressBarsConfig __instance)
			{
				if (Initialized)
					return;
				Initialized = true;

				MakeProgressBarRetro(__instance.progressBarPrefab);
				MakeProgressBarRetro(__instance.progressBarUIPrefab);
				MakeProgressBarRetro(__instance.healthBarPrefab);
			}
		}
	}
}
