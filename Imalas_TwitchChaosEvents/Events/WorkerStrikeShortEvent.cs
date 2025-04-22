using ONITwitchLib;
using System;
using UnityEngine;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// all dupes go on strike - no more working
	/// </summary>
	internal class WorkerStrikeShortEvent : ITwitchEventBase
	{

		public string ID => "ChaosTwitch_WorkerStrikeEvent_Short";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.WORKERSTRIKE.NAME;

		public string EventDescription => STRINGS.CHAOSEVENTS.WORKERSTRIKE.TOASTTEXT;

		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;
		public Action<object> EventAction => (_) =>
		{
			WorkerStrikeBase.StartStrike(120);
		};

		public Func<object, bool> Condition => (data) => Config.Instance.SkipMinCycle || (GameClock.Instance.GetCycle() > 50);

		public Danger EventDanger => Danger.High;
	}
}
