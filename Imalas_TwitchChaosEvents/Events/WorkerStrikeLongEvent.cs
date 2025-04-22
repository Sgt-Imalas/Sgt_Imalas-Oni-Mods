using ONITwitchLib;
using System;
using UnityEngine;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// all dupes go on strike - no more working
	/// </summary>
	internal class WorkerStrikeLongEvent : ITwitchEventBase
	{

		public string ID => "ChaosTwitch_WorkerStrikeEvent_Long";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.WORKERSTRIKE.NAME;

		public string EventDescription => STRINGS.CHAOSEVENTS.WORKERSTRIKE.TOASTTEXT;

		public EventWeight EventWeight => EventWeight.WEIGHT_VERY_RARE;
		public Action<object> EventAction => (_) =>
		{
			WorkerStrikeBase.StartStrike(600);
		};

		public Func<object, bool> Condition => (data) => Config.Instance.SkipMinCycle || (GameClock.Instance.GetCycle() > 100);

		public Danger EventDanger => Danger.Extreme;
	}
}
