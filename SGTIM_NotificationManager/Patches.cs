using HarmonyLib;
using UnityEngine;
using UtilLibs;

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



		[HarmonyPatch(typeof(AlertStateManager))]
		[HarmonyPatch(nameof(AlertStateManager.InitializeStates))]
		public static class StartStopSoundRemoval
		{
			public static void Postfix(AlertStateManager __instance)
			{
				if (Config.Instance.MUTE_RED_ALERT)
				{
					__instance.on.red.enterActions.RemoveAll(action => action.name == "SoundsOnRedAlert");
					__instance.on.red.exitActions.RemoveAll(action => action.name == "SoundsOffRedAlert");
				}
				if (Config.Instance.MUTE_YELLOW_ALERT)
				{
					__instance.on.yellow.enterActions.RemoveAll(action => action.name == "SoundsOnYellowAlert");
					__instance.on.yellow.exitActions.RemoveAll(action => action.name == "SoundsOffRedAlert");
				}
			}
		}
		[HarmonyPatch(typeof(Vignette))]
		[HarmonyPatch(nameof(Vignette.Refresh))]
		public static class StressLoopDisableSound
		{
			public static void Postfix(Vignette __instance)
			{
				if (Config.Instance.MUTE_RED_ALERT && __instance.showingRedAlert)
				{
					__instance.looping_sounds.StopSound(GlobalAssets.GetSound("RedAlert_LP"));
				}
				if (Config.Instance.MUTE_YELLOW_ALERT && __instance.showingYellowAlert)
				{
					__instance.looping_sounds.StopSound(GlobalAssets.GetSound("YellowAlert_LP"));
				}
			}
		}


		[HarmonyPatch(typeof(CalorieMonitor))]
		[HarmonyPatch(nameof(CalorieMonitor.InitializeStates))]
		public static class StarvationMonitorStates
		{
			public static void Postfix(CalorieMonitor __instance)
			{
				StatusItem status_item = Db.Get().DuplicantStatusItems.Starving;
				StatusItem hungry_statusItem = Db.Get().DuplicantStatusItems.Hungry;
				__instance.hungry.starving.enterActions.RemoveAll(action => action.name == "AddStatusItem(" + status_item.Id + ")");
				__instance.hungry.starving.exitActions.RemoveAll(action => action.name == "RemoveStatusItem(" + status_item.Id + ")");

				__instance.hungry.starving.Update((CalorieMonitor.Instance smi, float dt) =>
				{
					float percentage = ((float)Config.Instance.STARVATION_THRESHOLD * 1000) / smi.calories.GetMax();
					bool BeyondNotificationThreshold = smi.GetCalories0to1() < percentage;

					if (smi.master.gameObject.TryGetComponent<KSelectable>(out var selectable))
					{
						if (BeyondNotificationThreshold)
						{
							selectable.AddStatusItem(status_item);
							selectable.RemoveStatusItem(hungry_statusItem);
						}
						else
						{
							selectable.RemoveStatusItem(status_item);
							selectable.AddStatusItem(hungry_statusItem);
						}
					}
				}
				).Exit((CalorieMonitor.Instance smi) =>
				{
					if (smi.master.gameObject.TryGetComponent<KSelectable>(out var selectable))
					{
						selectable.RemoveStatusItem(status_item);
						selectable.RemoveStatusItem(hungry_statusItem);
					}
				}
				)
				;
			}
		}

		[HarmonyPatch(typeof(SuffocationMonitor))]
		[HarmonyPatch(nameof(SuffocationMonitor.InitializeStates))]
		public static class SuffocationNotification
		{
			public static void Postfix(SuffocationMonitor __instance)
			{
				__instance.noOxygen.suffocating.enterActions.Clear();
				//__instance.nooxygen.suffocating.exitActions.Clear();
				__instance.noOxygen.suffocating.Update((SuffocationMonitor.Instance smi, float dt) =>
				{
					float breathValuePercentage = ((float)Config.Instance.SUFFOCATION_THRESHOLD) * 100f / 110f;
					bool BeyondNotificationThreshold = (double)smi.breath.value <= breathValuePercentage;

					if (smi.master.gameObject.TryGetComponent<KSelectable>(out var selectable))
					{
						if (BeyondNotificationThreshold)
						{
							selectable.SetStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating);
						}
						else
						{
							selectable.SetStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath);
						}
					}
				}
				);
			}
		}

        [HarmonyPatch(typeof(BionicSuffocationMonitor))]
        [HarmonyPatch(nameof(BionicSuffocationMonitor.InitializeStates))]
        public static class BionicSuffocationNotification
        {
            public static void Postfix(BionicSuffocationMonitor __instance)
            {
                __instance.noOxygen.suffocating.enterActions.Clear();
                //__instance.nooxygen.suffocating.exitActions.Clear();
                __instance.noOxygen.suffocating.Update((BionicSuffocationMonitor.Instance smi, float dt) =>
                {
                    float breathValuePercentage = ((float)Config.Instance.SUFFOCATION_THRESHOLD) * 100f / 110f;
                    bool BeyondNotificationThreshold = (double)smi.breath.value <= breathValuePercentage;

                    if (smi.master.gameObject.TryGetComponent<KSelectable>(out var selectable))
                    {
                        if (BeyondNotificationThreshold)
                        {
                            selectable.SetStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating);
                        }
                        else
                        {
                            selectable.SetStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath);
                        }
                    }
                }
                );
            }
        }
		        

        [HarmonyPatch(typeof(NotificationScreen), nameof(NotificationScreen.PlayDingSound))]
		public static class MuteDingSound
		{
			public static void Prefix(Notification notification)
			{
				if (notification == null)
					return;

				//SgtLogger.l(notification.NotifierName + ", " + notification.titleText);
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
				else if(notification.titleText == global::STRINGS.MISC.NOTIFICATIONS.FOODROT.NAME)
				{
					skipAudio = Config.Instance.MUTE_FOODROT_SOUND;
				}
				else if (notification.titleText == global::STRINGS.MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.NAME)
				{
					skipAudio = Config.Instance.MUTE_UNREFRIGERATEDFOOD_SOUND;
				}

				notification.playSound = !skipAudio;
			}
		}
		[HarmonyPatch(typeof(NotificationScreen.Entry))]
		[HarmonyPatch(nameof(NotificationScreen.Entry.Add))]
		public static class PauseAndZoom
		{
			static float lastTrigger = -1;
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
				else if (notification.titleText == global::STRINGS.MISC.NOTIFICATIONS.FOODROT.NAME)
				{
					pause = Config.Instance.PAUSE_ON_FOODROT;
					moveCam = Config.Instance.PAN_TO_FOODROT;
				}
				else if (notification.titleText == global::STRINGS.MISC.NOTIFICATIONS.UNREFRIGERATEDFOOD.NAME)
				{
					pause = Config.Instance.PAUSE_ON_UNREFRIGERATEDFOOD;
					moveCam = Config.Instance.PAN_TO_UNREFRIGERATEDFOOD;
				}

				if (GameClock.Instance != null)
				{
					var currentTime = GameClock.Instance.GetTimeInCycles();
					if (Mathf.Approximately(lastTrigger, currentTime))
					{
						return;
					}
					else
					{
						lastTrigger = currentTime;
					}
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
