using ONITwitchLib;
using System;
using System.Collections.Generic;
using Util_TwitchIntegrationLib;

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
			"MoltenSlugMeteors"
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
			EventInfo EventToTrigger = weatherEvents.GetRandom();


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
