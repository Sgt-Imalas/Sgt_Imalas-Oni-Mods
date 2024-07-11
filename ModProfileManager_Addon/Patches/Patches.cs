using Database;
using HarmonyLib;
using Klei.AI;
using ModProfileManager_Addon.UnityUI;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UnityEngine;
using UtilLibs;
using static ModProfileManager_Addon.ModAssets;

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
                SgtLogger.l(__instance.transform.parent.gameObject.name, "Parent");
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
    }
}
