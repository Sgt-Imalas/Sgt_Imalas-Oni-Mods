using HarmonyLib;
using ModProfileManager_Addon.UnityUI;
using PeterHan.PLib.UI;
using UnityEngine;
using UtilLibs;

namespace ModProfileManager_Addon.Patches
{
	internal class Patches
	{
		[HarmonyPatch(typeof(MainMenu))]
		[HarmonyPatch(nameof(MainMenu.MakeButton))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Postfix(MainMenu __instance, MainMenu.ButtonInfo info, KButton __result)
			{
				if (info.text.ToString() == global::STRINGS.UI.FRONTEND.MODS.TITLE.ToString())
				{
					var presetButton = Util.KInstantiateUI<KButton>(__result.gameObject, __result.gameObject);
					presetButton.gameObject.name = "PresetButton";
					var rec = presetButton.rectTransform();
					bool SO = DlcManager.IsExpansion1Active();
					rec.SetInsetAndSizeFromParentEdge(SO ? RectTransform.Edge.Right : RectTransform.Edge.Left, -63, 60);
					rec.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, SO ? 7 : 4, 50);
					UIUtils.TryChangeText(rec, "Text", "Presets");
					presetButton.bgImage.colorStyleSetting = PUITuning.Colors.ButtonPinkStyle;

					presetButton.ClearOnClick();
					presetButton.onClick += () => { ModsPresetScreen.ShowWindow(null); };



					presetButton.gameObject.SetActive(true);
				}
				//SgtLogger.l(__instance.transform.parent.gameObject.name, "Parent");
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
		[HarmonyPatch(typeof(FileNameDialog))]
		[HarmonyPatch(nameof(FileNameDialog.OnActivate))]
		public static class FixCrashOnActivate
		{
			public static bool Prefix(FileNameDialog __instance)
			{
				if (CameraController.Instance == null)
				{
					__instance.OnShow(show: true);
					__instance.inputField.Select();
					__instance.inputField.ActivateInputField();
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(FileNameDialog))]
		[HarmonyPatch(nameof(FileNameDialog.OnDeactivate))]
		public static class FixCrashOnDeactivate
		{
			public static bool Prefix(FileNameDialog __instance)
			{
				if (CameraController.Instance == null)
				{
					__instance.OnShow(show: false);
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(KMod.Mod), nameof(KMod.Mod.Install))]
		public static class ModManager_Install
		{
			public static void Postfix(KMod.Mod __instance)
			{
				ModsPresetScreen.NewModInstalled(__instance);
			}
		}
		[HarmonyPatch(typeof(KMod.Mod), nameof(KMod.Mod.Uninstall))]
		public static class ModManager_Uninstall
		{
			public static void Postfix(KMod.Mod __instance)
			{
				ModsPresetScreen.ModUninstalled(__instance);
			}
		}
	}
}
