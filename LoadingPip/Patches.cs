using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static LoadingPip.ModAssets;

namespace LoadingPip
{
	internal class Patches
	{

		[HarmonyPatch(typeof(LoadingOverlay), nameof(LoadingOverlay.Load))]
		public class Overlay_Icon_Replace
		{
			//replace loading dupe face with pip
			public static void Postfix()
			{
				var instance = LoadingOverlay.instance;
				var image = instance.transform.Find("Image").GetComponent<Image>();
				var pipSprite = Def.GetUISprite(Assets.GetPrefab(SquirrelConfig.ID));
				image.preserveAspect = true;
				image.sprite = pipSprite.first;

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
	}
}
