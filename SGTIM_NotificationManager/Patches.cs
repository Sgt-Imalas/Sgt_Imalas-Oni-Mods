using Database;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Core;
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

        //[HarmonyPatch(typeof(StressMonitor.Instance))]
        //[HarmonyPatch(nameof(StressMonitor.Instance.IsStressed))]
        //public static class IsStressed
        //{
        //    public static bool Prefix(StressMonitor.Instance __instance, ref bool __result)
        //    {
        //        __result = __instance.GetCurrentState().GetType() == typeof(ExtendedStressMonitor.Stressed);
        //        return false;
        //    }
        //}


        //[HarmonyPatch(typeof(StressMonitor))]
        //[HarmonyPatch(nameof(StressMonitor.InitializeStates))]
        //public static class ExtendedStressMonitor
        //{
        //    public class Stressed : GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State
        //    {
        //        public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State tier1 = new GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State();
        //        public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State tier2 = new GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State();
        //        public GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State tier3 = new GameStateMachine<StressMonitor, StressMonitor.Instance, IStateMachineTarget, object>.State();
        //    }
        //    public static void Postfix(StressMonitor __instance)
        //    {
        //        //for(int i = __instance.states.Count- 1; i >= 0;i--)
        //        //{
        //        //    var state = __instance.states[i];
        //        //    if (state.name.Contains("stressed"))
        //        //    {
        //        //        __instance.states.RemoveAt(i);
        //        //    }
        //        //}
        //        Stressed stressed2 = new Stressed();


        //        stressed2
        //            .ToggleStatusItem(Db.Get().DuplicantStatusItems.Stressed)
        //            .Transition(__instance.satisfied, (smi => (double)smi.stress.value < 60.0))
        //            .ToggleReactable((smi => smi.CreateConcernReactable())).TriggerOnEnter(GameHashes.Stressed);

        //        stressed2.tier1
        //            //.Transition(__instance.satisfied, (smi => (double)smi.stress.value < 60.0))
        //            .Transition(stressed2.tier2, (smi => (double)smi.stress.value >= 90.0));
        //        stressed2.tier2
        //            .Transition(stressed2.tier1, (smi => (double)smi.stress.value < 90.0))
        //            .Transition(stressed2.tier3, (smi => smi.HasHadEnough()));
        //        stressed2.tier3
        //                .TriggerOnEnter(GameHashes.StressedHadEnough)
        //                .Transition(stressed2.tier2, (smi => !smi.HasHadEnough()));

        //        __instance.satisfied.transitions.Clear();
        //        __instance.satisfied.Transition(stressed2.tier1, (smi => (double)smi.stress.value >= 60.0));

        //        __instance.states.Add(stressed2);
        //        __instance.stressed.GoTo(stressed2);
        //    }
        //}



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
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_NAME)
                {
                    skipAudio = Config.Instance.MUTE_RADIATIONVOMITING_SOUND;

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
                else if (notification.titleText == global::STRINGS.DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_NAME)
                {
                    pause = Config.Instance.PAUSE_ON_RADIATIONVOMITING;
                    moveCam = Config.Instance.PAN_TO_RADIATIONVOMITING;
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
