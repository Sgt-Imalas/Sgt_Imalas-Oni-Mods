using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using System;
using UnityEngine;
using Util_TwitchIntegrationLib;
using Util_TwitchIntegrationLib.Scripts;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// Creeper liquid rain
	/// </summary>
	internal class CreeperRainEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_CreeperRain";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.CREEPERRAIN.NAME;

		public string EventDescription => STRINGS.CHAOSEVENTS.CREEPERRAIN.TOASTTEXT;

		//public EventWeight EventWeight => EventWeight.WEIGHT_VERY_RARE;
		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;

		public Action<object> EventAction => (object data) =>
		{
			var go = new GameObject("creeperSpawner");

			var rain = go.AddComponent<LiquidRainSpawner>();

			rain.totalAmountRangeKg = (1000, 10000);
			rain.durationInSeconds = 40;
			rain.dropletMassKg = 0.05f;
			rain.elementId = ModElements.Creeper;
			rain.spawnRadius = 15;

			SpeedControlScreen.Instance.SetSpeed(0);
			go.SetActive(true);

			GameScheduler.Instance.Schedule("creeper rain", 10f, _ =>
			{
				int cell = ONITwitchLib.Utils.PosUtil.ClampedMouseCell();

				if (SpaceCheeseChecker.HasThereBeenAttemptedSpaceCheese(cell, out var NEWcell, out var dupe, 5, 8, false, 4))
				{
					rain.StartRaining(NEWcell);
					ToastManager.InstantiateToast(
						STRINGS.CHAOSEVENTS.CREEPERRAIN.TOAST,
						string.Format(STRINGS.CHAOSEVENTS.CREEPERRAIN.TOASTTEXT2, ClusterManager.Instance.activeWorld.GetProperName()));

					GameScheduler.Instance.Schedule("creeper rain cheese protection toast", 5f, _ =>
					{
						ToastManager.InstantiateToastWithPosTarget(
						STRINGS.CHAOSEVENTS.SPACECHEESEDETECTED.TOAST,
						string.Format(STRINGS.CHAOSEVENTS.SPACECHEESEDETECTED.TOASTTEXT, EventName, dupe), Grid.CellToPos(NEWcell));
					});
				}
				else
				{
					rain.StartRaining();
					ToastManager.InstantiateToast(
						STRINGS.CHAOSEVENTS.CREEPERRAIN.TOAST,
						string.Format(STRINGS.CHAOSEVENTS.CREEPERRAIN.TOASTTEXT2, ClusterManager.Instance.activeWorld.GetProperName()));
				}

				//AudioUtil.PlaySound(ModAssets.Sounds.SPLAT, ModAssets.GetSFXVolume() * 0.15f); // its loud
			});

			SoundUtils.PlaySound(ModAssets.SOUNDS.EVILSOUND, SoundUtils.GetSFXVolume() * 1.0f, true);
			ToastManager.InstantiateToast(
				STRINGS.CHAOSEVENTS.CREEPERRAIN.TOAST,
				string.Format(STRINGS.CHAOSEVENTS.CREEPERRAIN.TOASTTEXT, ClusterManager.Instance.activeWorld.GetProperName()));
		};

		public Func<object, bool> Condition =>
			(data) =>
			{
				return GameClock.Instance.GetCycle() > 150;
			};

		public Danger EventDanger => Danger.Deadly;

	}
}
