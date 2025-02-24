using Imalas_TwitchChaosEvents.OmegaSawblade;
using ONITwitchLib;
using System;
using UnityEngine;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// Noita Omega Sawblade that either homes for wiggling mouse cursor or dupes
	/// </summary>
	internal class BuzzsawEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_Buzzsaw";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.BUZZSAW.NAME;

		public string EventDescription => STRINGS.CHAOSEVENTS.BUZZSAW.TOASTTEXT;

		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;

		public Action<object> EventAction => (object data) =>
		{
			SpeedControlScreen.Instance.SetSpeed(0);
			//SpeedControlScreen.Instance.Pause();

			SpawnBuzzSaw();
		};

		public Func<object, bool> Condition =>
			(data) =>
			{
				return Config.Instance.SkipMinCycle || GameClock.Instance.GetCycle() > 50;
			};

		public Danger EventDanger => Danger.Extreme;

		public void SpawnBuzzSaw()
		{
			//var spawningPosition = CameraController.Instance.baseCamera.transform.GetPosition(); ///This gives middle of the screeen
			///Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()); this gives mouse pos
			var spawningPosition = Grid.CellToPos(ONITwitchLib.Utils.PosUtil.ClampedMouseCellWithRange(0));
			SgtLogger.l(spawningPosition.ToString(), "POS1");
			spawningPosition.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
			var blade = Util.KInstantiate(Assets.GetPrefab(OmegaSawbladeConfig.ID), spawningPosition, Quaternion.identity);
			blade.SetActive(true);
			SgtLogger.l(spawningPosition.ToString(), "POS2");

			ToastManager.InstantiateToastWithGoTarget(
			STRINGS.CHAOSEVENTS.BUZZSAW.TOAST,
			 STRINGS.CHAOSEVENTS.BUZZSAW.TOASTTEXT, blade
			 );
		}
	}
}
