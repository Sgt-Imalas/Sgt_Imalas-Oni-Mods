using Database;
using HarmonyLib;
using JetBrains.Annotations;
using Klei.AI;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UtilLibs;
using static CustomGameSettingsModifier.ModAssets;
using static STRINGS.UI.FRONTEND;

namespace CustomGameSettingsModifier
{
    internal class Patches
    {
        [HarmonyPatch(typeof(CustomGameSettings))]
        [HarmonyPatch(nameof(CustomGameSettings.OnDeserialized))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Postfix(CustomGameSettings __instance)
            {
                if (!__instance.CurrentQualityLevelsBySetting.ContainsKey(CustomGameSettingConfigs.MeteorShowers.id))
                {
                    __instance.CurrentQualityLevelsBySetting.Add(CustomGameSettingConfigs.MeteorShowers.id, "Default");
                }
            }
        }

        [HarmonyPatch(typeof(PauseScreen))]
        [HarmonyPatch(nameof(PauseScreen.OnKeyDown))]
        public static class CatchGoingBack
        {
            public static bool Prefix(KButtonEvent e)
            {
                if ( CustomSettingsController.Instance != null && CustomSettingsController.Instance.CurrentlyActive)
                    return false;
                return true;
            }
        }

        private static readonly KButtonMenu.ButtonInfo TwitchButtonInfo = new KButtonMenu.ButtonInfo((string)STRINGS.UI.CUSTOMGAMESETTINGSCHANGER.BUTTONTEXT, Action.NumActions, new UnityAction(OnCustomMenuButtonPressed));
        private static void OnCustomMenuButtonPressed()
        {
            PauseScreen.Instance.RefreshButtons(); 
            CustomSettingsController.ShowWindow();
            GameScheduler.Instance.ScheduleNextFrame("OpenCustomSettings", (System.Action<object>)(_ =>
            {
                PauseScreen.Instance.RefreshButtons();
            }));
        }

        [HarmonyPatch(typeof(PauseScreen), "OnPrefabInit")]
        private static class PauseScreen_OnPrefabInit_Patch
        {
            [UsedImplicitly]
            private static void Postfix(ref IList<KButtonMenu.ButtonInfo> ___buttons)
            {
                List<KButtonMenu.ButtonInfo> list = ___buttons.ToList<KButtonMenu.ButtonInfo>();
                TwitchButtonInfo.isEnabled = true;
                list.Insert(5, TwitchButtonInfo);
                ___buttons = (IList<KButtonMenu.ButtonInfo>)list;
            }
        }

        [HarmonyPatch(typeof(KButtonMenu), "RefreshButtons")]
        private static class PauseScreen_RefreshButtons_Patch
        {
            [UsedImplicitly]
            private static void Postfix(KButtonMenu __instance)
            {
                //if (!(__instance is PauseScreen) || !((UnityEngine.Object)PauseMenuPatches.TwitchButtonInfo.uibutton != (UnityEngine.Object)null) || !((UnityEngine.Object)PauseMenuPatches.twitchButtonStyle == (UnityEngine.Object)null) && !((UnityEngine.Object)PauseMenuPatches.TwitchButtonInfo.uibutton.bgImage.colorStyleSetting == (UnityEngine.Object)null) && !((UnityEngine.Object)PauseMenuPatches.TwitchButtonInfo.uibutton.bgImage.colorStyleSetting != (UnityEngine.Object)PauseMenuPatches.twitchButtonStyle))
                //    return;
                //PauseMenuPatches.twitchButtonStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
                //PauseMenuPatches.twitchButtonStyle.disabledColor = PauseMenuPatches.DisabledColor;
                //PauseMenuPatches.twitchButtonStyle.inactiveColor = PauseMenuPatches.InactiveTwitchColor;
                //PauseMenuPatches.twitchButtonStyle.hoverColor = PauseMenuPatches.HoverTwitchColor;
                //PauseMenuPatches.twitchButtonStyle.activeColor = PauseMenuPatches.PressedTwitchColor;
                //PauseMenuPatches.TwitchButtonInfo.uibutton.bgImage.colorStyleSetting = PauseMenuPatches.twitchButtonStyle;
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

