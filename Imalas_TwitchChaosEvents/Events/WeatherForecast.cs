using ONITwitchLib;
using System;
using System.Collections.Generic;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// Random Weather Event
	/// </summary>
	internal class WeatherForecast : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_WeatherForecast";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.WEATHERFORECAST.NAME;

		public EventWeight EventWeight => EventWeight.WEIGHT_FREQUENT;


		public static List<string> WeatherEvents = new List<string>()
		{
			new FogEvent().ID,
			new TacoRainEvent().ID,
			//WeatherEvents from ChaosReigns by StuffyDoll:
			"MagmaRain",
			"NuclearWasteRain",
			"ZoologicalMeteors",
			"WaterBalloonMeteors",
			"MoltenSlugMeteors",
			//Solar storms from Twitchery by Aki:
			"SolarStormMedium",
			"SolarStormSmall",
		};

		public Action<object> EventAction => (obj) =>
		{
			List<EventInfo> weatherEvents = new List<EventInfo>();
			foreach (var e in WeatherEvents)
			{
				if (EventRegistration.TryGetEvent(e, out var eventInfo))
				{
					weatherEvents.Add(eventInfo);
				}
			}
			weatherEvents.Shuffle();
			EventInfo EventToTrigger = null;
			for (int i = 0; i < weatherEvents.Count; i++)
			{
				var weatherEvent = weatherEvents[i];
				if (weatherEvent.CheckCondition(null))
				{
					SgtLogger.l("executable weather event found: " + weatherEvent.FriendlyName);
					EventToTrigger = weatherEvent;
					break;
				}
			}
			if (EventToTrigger == null)
			{
				SgtLogger.l("no executable weather event found, using first xecutable event: " + weatherEvents[0].FriendlyName);
				EventToTrigger = weatherEvents[0];
			}

			ToastManager.InstantiateToast(STRINGS.CHAOSEVENTS.WEATHERFORECAST.TOAST, string.Format(STRINGS.CHAOSEVENTS.WEATHERFORECAST.TOASTTEXT, EventToTrigger.FriendlyName));
			GameScheduler.Instance.Schedule("start weather", 20f, (_) => EventToTrigger.Trigger(null));
		};

		public Func<object, bool> Condition => (s) =>
		{
			List<EventInfo> weatherEvents = new List<EventInfo>();
			foreach (var e in WeatherEvents)
			{
				if (EventRegistration.TryGetEvent(e, out var eventInfo))
				{
					weatherEvents.Add(eventInfo);
				}
			}
			return weatherEvents.Count > 0;
		};

		public Danger EventDanger => Danger.Medium;
	}
}
