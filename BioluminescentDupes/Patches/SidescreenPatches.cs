using BioluminescentDupes.Content.Scripts;
using BioluminescentDupes.Content.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace BioluminescentDupes.Patches
{
	internal class SidescreenPatches
	{       /// <summary>
			/// Grab ColorPicker gameobject from Pixelpack sidescreen
			/// </summary>
		[HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.OnPrefabInit))]
		public static class CustomSideScreenPatch_OnPrefabInit
		{
			public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
			{
				var pixelPackScreenRef = ___sideScreens.Find(screen => screen.screenPrefab.GetType() == typeof(PixelPackSideScreen));
				if (pixelPackScreenRef != null)
				{
					var pixelPackScreen = pixelPackScreenRef.screenPrefab as PixelPackSideScreen;
					Bioluminescence_Sidescreen.colorPickerContainerPrefab = pixelPackScreen.colorSwatchContainer;
					Bioluminescence_Sidescreen.colorPickerSwatchEntryPrefab = pixelPackScreen.swatchEntry;
					Bioluminescence_Sidescreen.SwatchColors = new(pixelPackScreen.colorSwatch);
				}
				else
					SgtLogger.error("Pixelpack sidescreen not found, mod cannot function!");

			}
		}
		/// <summary>
		/// Show Color picker on colorable buldings
		/// </summary>
		[HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.Refresh))]
		public static class CustomSideScreenPatch_Refresh
		{
			public static void Postfix(DetailsScreen __instance)
			{

				if (__instance.sideScreen != null
					&& __instance.sideScreen.gameObject != null)
				{
					if (__instance.target.TryGetComponent<BioluminescenceColorSelectable>(out var colorable) && colorable.HasLightTrait())
						Bioluminescence_Sidescreen.Target = colorable;
					else
						Bioluminescence_Sidescreen.Target = null;

					Bioluminescence_Sidescreen.RefreshUIState(__instance.sideScreen.transform);
				}
			}
		}

		/// <summary>
		/// Delete Color picker screen
		/// </summary>
		[HarmonyPatch(typeof(Game), nameof(Game.OnDestroy))]
		public static class GameOnDestroy
		{
			public static void Postfix() => Bioluminescence_Sidescreen.Destroy();
		}
	}
}
