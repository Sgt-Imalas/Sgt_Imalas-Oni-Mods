using HarmonyLib;
using Klei;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace LoadingPip
{
	internal class Patches
	{

		[HarmonyPatch(typeof(LoadingOverlay), nameof(LoadingOverlay.Load))]
		public class Overlay_Icon_Replace
		{
			//replace loading dupe face with custom icon
			public static void Postfix()
			{
				if (Config.IsDefault && ModAssets.PipLicksButt)
				{
					LickPipButt();
				}
				else
					SetIcon();

			}
			static void LickPipButt()
			{
				var instance = LoadingOverlay.instance;
				var image = instance.transform.Find("Image").GetComponent<Image>();
				var rect = image.rectTransform();
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
				rect.position -= new Vector3(0,90);

				var gameObject = image.gameObject;
				//image.SetAlpha(0);

				gameObject.SetActive(false);
				var kbac = gameObject.AddComponent<KBatchedAnimController>();
				var renderer = gameObject.AddComponent<KBatchedAnimCanvasRenderer>();
				kbac.materialType = KAnimBatchGroup.MaterialType.UI;
				kbac.setScaleFromAnim = false;
				kbac.sceneLayer = Grid.SceneLayer.SceneMAX;
				kbac.AnimFiles = [Assets.GetAnim("squirrel_build_kanim"), Assets.GetAnim("plb_pip_licks_butt_kanim")];
				kbac.isMovable = true;
				kbac.defaultAnim = "lick_butt_loop";
				gameObject.SetActive(true);
				if (!kbac.HasAnimation("lick_butt_loop"))
				{
					SgtLogger.error("Squirrel kbac has no lick_butt_loop animation!");
				}
				kbac.Play("lick_butt_loop");
				Global.Instance.StartCoroutine(FakePlaying());
				IEnumerator FakePlaying()
				{
					int maxFrameCount = kbac.GetCurrentAnim().numFrames;
					int frameToPlay = 0;
					while (!gameObject.IsNullOrDestroyed())
					{
						frameToPlay++;
						if (frameToPlay >= maxFrameCount)
						{
							frameToPlay = 0;
						}
						kbac.SetPositionPercent((float)frameToPlay / (float)maxFrameCount);
						yield return new WaitForSecondsRealtime(0.033f);
					}
				}
			}


			static void SetIcon()
			{
				var instance = LoadingOverlay.instance;
				var image = instance.transform.Find("Image").GetComponent<Image>();
				var loadingIcon = Config.Instance.GetTargetIcon();
				image.preserveAspect = true;
				image.sprite = loadingIcon.first;
				image.color = loadingIcon.second;

				var rect = image.rectTransform();
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}

		public static string primal_aspid_sprite_id = "aspid";

		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{
				string dreamIconDicrectory = FileSystem.Normalize(System.IO.Path.Combine(IO_Utils.ModPath, "assets"));
				if (System.IO.Directory.Exists(dreamIconDicrectory))
				{
					foreach (var file in System.IO.Directory.GetFiles(dreamIconDicrectory))
					{
						var fileInfo = new FileInfo(file);
						if (fileInfo.Exists && fileInfo.Extension == ".png" && IO_Utils.NotAMacFile(fileInfo))
						{
							SgtLogger.l("loading custom load screen icon: " + fileInfo.Name);
							var sprite = AssetUtils.AddSpriteToAssets(fileInfo, __instance);
							ModAssets.CustomLoadedIcons.Add(sprite.name);
						}
					}
				}
			}
		}
	}
}
