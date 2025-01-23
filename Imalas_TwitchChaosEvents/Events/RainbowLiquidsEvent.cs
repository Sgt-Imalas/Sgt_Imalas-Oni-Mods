using ONITwitchLib;
using System;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// all liquids turn rainbow
	/// </summary>
	internal class RainbowLiquidsEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_RainbowLiquidsEvent";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.NAME;

		public string EventDescription => STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.TOASTTEXT;

		public EventWeight EventWeight => EventWeight.WEIGHT_FREQUENT;

		public Action<object> EventAction => (object data) =>
		{
			ModAssets.RainbowLiquids = true;
			ToastManager.InstantiateToast(
				STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.TOAST,
				 STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.TOASTTEXT
				 );



			GameScheduler.Instance.Schedule("rainbow liquid disable", 600f, _ =>
			{
				ModAssets.RainbowLiquids = false;
			});
		};

		public Func<object, bool> Condition =>
			(data) => true;

		public Danger EventDanger => Danger.None;

	}
}
