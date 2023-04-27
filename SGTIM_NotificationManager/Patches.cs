using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static SGTIM_NotificationManager.ModAssets;

namespace SGTIM_NotificationManager
{
    internal class Patches
    {
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

        /// <summary>
        /// Starving
        /// </summary>
        [HarmonyPatch(typeof(CalorieMonitor.Instance))]
        [HarmonyPatch(nameof(CalorieMonitor.Instance.IsStarving))]
        public static class StarvingNotification
        {
            public static bool Prefix(CalorieMonitor.Instance __instance, ref bool __result)
            {
                float percentage = ((float)Config.Instance.STARVATION_THRESHOLD*1000) / __instance.calories.GetMax();
                __result = __instance.GetCalories0to1() < percentage;
                return false;
            }
        }

        [HarmonyPatch(typeof(SuffocationMonitor.Instance))]
        [HarmonyPatch(nameof(SuffocationMonitor.Instance.IsSuffocating))]
        public static class SuffocationNotification
        {
            public static bool Prefix(SuffocationMonitor.Instance __instance, ref bool __result)
            {
                float breathValuePercentage = ((float)Config.Instance.SUFFOCATION_THRESHOLD) * 100f / 110f;
                if (__instance.breath.deltaAttribute.GetTotalValue() <= 0f)
                {
                    return __instance.breath.value <= breathValuePercentage;
                }
                __result = false;
                return false;
            }
        }
        [HarmonyPatch(typeof(SuitSuffocationMonitor.Instance))]
        [HarmonyPatch(nameof(SuitSuffocationMonitor.Instance.IsSuffocating))]
        public static class SuffocationNotificationSuit
        {
            public static bool Prefix(SuffocationMonitor.Instance __instance, ref bool __result)
            {
                float breathValuePercentage = ((float)Config.Instance.SUFFOCATION_THRESHOLD) * 100f / 110f;
                __result = (double)__instance.breath.value <= breathValuePercentage;
                return false;
            }
        }
        [HarmonyPatch(typeof(NotificationScreen))]
        [HarmonyPatch(nameof(NotificationScreen.PlayDingSound))]
        public static class MuteDingSound
        {
            public static void Prefix(Notification notification)
            {
                if (notification == null)
                    return;

                SgtLogger.l(notification.NotifierName + ", " + notification.titleText);
                bool skipAudio = false;
                if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.STARVING.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_STARVATION_SOUND;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.SUFFOCATING.NOTIFICATION_NAME)
                {

                    skipAudio = Config.Instance.MUTE_SUFFOCATION_SOUND;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.FIGHTING.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_ATTACK_SOUND;

                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.FLEEING.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_FLEE_SOUND;

                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_PEE_SOUND;

                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.STRESSED.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_STRESS_SOUND;

                }
                else if (notification.titleText == global::STRINGS.CREATURES.STATUSITEMS.SCALDING.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_SCALDING_SOUND;

                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.INCAPACITATED.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_INCAPACITATED_SOUND;

                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.ENTOMBEDCHORE.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_ENTOMBED_SOUND;

                }
                else if (notification.titleText == global::STRINGS.CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION)
                {
                    skipAudio = Config.Instance.MUTE_PLANTDEATH_SOUND;

                }

                notification.playSound = !skipAudio;
            }
        }
        [HarmonyPatch(typeof(NotificationScreen.Entry))]
        [HarmonyPatch(nameof(NotificationScreen.Entry.Add))]
        public static class PauseAndZoom
        {
            public static void Postfix(Notification notification)
            {
                if (notification == null)
                    return;
                bool pause = false;
                bool moveCam = false;

                if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.STARVING.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_STARVATION;
                    moveCam = Config.Instance.PAN_TO_STARVATION;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.SUFFOCATING.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_SUFFOCATION;
                    moveCam = Config.Instance.PAN_TO_SUFFOCATION;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.FIGHTING.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_ATTACK;
                    moveCam = Config.Instance.PAN_TO_ATTACK;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.FLEEING.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_FLEE;
                    moveCam = Config.Instance.PAN_TO_FLEE;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_PEE;
                    moveCam = Config.Instance.PAN_TO_PEE;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.STRESSED.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_STRESS;
                    moveCam = Config.Instance.PAN_TO_STRESS;
                }
                else if (notification.titleText == global::STRINGS.CREATURES.STATUSITEMS.SCALDING.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_SCALDING;
                    moveCam = Config.Instance.PAN_TO_SCALDING;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.INCAPACITATED.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_INCAPACITATED;
                    moveCam = Config.Instance.PAN_TO_INCAPACITATED;
                }
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.ENTOMBEDCHORE.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_ENTOMBED;
                    moveCam = Config.Instance.PAN_TO_ENTOMBED;
                }
                else if (notification.titleText == global::STRINGS.CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION)
                {
                    pause = Config.Instance.PAUSE_ON_PLANTDEATH;
                    moveCam = Config.Instance.PAN_TO_PLANTDEATH;
                }

                if (notification.Notifier != null && moveCam)
                {
                    CameraController.Instance.ActiveWorldStarWipe(notification.Notifier.gameObject.GetMyWorldId(), notification.Notifier.transform.GetPosition(), 8f);
                }
                if (pause)
                {
                    SpeedControlScreen.Instance.Pause(false);
                }

            }
        }
    }
}
