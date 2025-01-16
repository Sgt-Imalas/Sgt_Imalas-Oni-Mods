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
			//replace loading dupe face with custom icon
			public static void Postfix()
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

		public static Sprite primal_aspid_sprite;
		public static string primal_aspid_sprite_id = "aspid";


		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{
				primal_aspid_sprite = InjectionMethods.AddSpriteToAssets(__instance, primal_aspid_sprite_id);
			}
		}
	}
}
