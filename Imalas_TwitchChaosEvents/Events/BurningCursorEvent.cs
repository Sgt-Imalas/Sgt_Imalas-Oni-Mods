using Imalas_TwitchChaosEvents.Fire;
using ONITwitchLib;
using System;
using UnityEngine;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// Cursor catches fire, ignites underlying tiles
	/// </summary>
	internal class BurningCursorEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_BurningCursor";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.BURNINGCURSOR.NAME;

		public string EventDescription => STRINGS.CHAOSEVENTS.BURNINGCURSOR.TOASTTEXT;

		public EventWeight EventWeight => EventWeight.WEIGHT_UNCOMMON;

		public Action<object> EventAction => (object data) =>
		{

			ToastManager.InstantiateToast(
			STRINGS.CHAOSEVENTS.BURNINGCURSOR.TOAST,
			 STRINGS.CHAOSEVENTS.BURNINGCURSOR.TOASTTEXT
			 );

			SpawnFire();
		};

		public Func<object, bool> Condition =>
			(data) =>
			{
				return Config.Instance.SkipMinCycle || GameClock.Instance.GetCycle() > 50;
			};

		public Danger EventDanger => Danger.Extreme;

		public void SpawnFire()
		{
			var spawningPosition = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
			spawningPosition.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
			var fire = Util.KInstantiate(ModAssets.FireSpawner, spawningPosition);
			fire.AddComponent<BurningCursorLogic>();
			fire.SetActive(true);

		}
	}
}
