using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// Random Weather Event
	/// </summary>
	internal class WeatherForecastEvent : ITwitchEventBase
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
			//WeatherEvents from Twitchery by Aki:
			"SolarStormMedium",
			"SolarStormSmall",
			"SandStormMedium",
			"SandStormHigh",
			"SandStormDeadly",
			"BlizzardMedium",
			"BlizzardDeadly",
			"HellFire",
			"HarvestMoon",
			"FrogRain"
		};

		public Action<object> EventAction => (obj) =>
		{
			WeightedList<EventInfo> weatherEvents = new ();
			foreach (var e in WeatherEvents)
			{
				if (EventRegistration.TryGetEvent(e, out var eventInfo))
				{
					bool eventAllowed = eventInfo.CheckCondition(null);
					int weight = (int)EventWeight.WEIGHT_RARE;
					var group = eventInfo.Group;
					if(group != null)
					{
						SgtLogger.l(eventInfo.FriendlyName + " group found");
						var weights = group.GetWeights();
						if (weights != null)
						{
							SgtLogger.l("weights gotten");
							var key = weights.Keys.FirstOrDefault(e => e.Id == eventInfo.Id);
							if (key != null)
								weights.TryGetValue(key, out weight);
						} 
					}

					if (eventAllowed)
					{
						weatherEvents.Add(eventInfo, weight);
					}
					SgtLogger.l("potential weather event: " + eventInfo.FriendlyName + ", can it execute: " + eventAllowed+", event weight: "+weight);
				}
			}
			if (!weatherEvents.Any())
			{
				SgtLogger.error("No available weather events found, aborting");
				return;
			}

			EventInfo EventToTrigger = weatherEvents.Next();
			SgtLogger.l("found weather event: " + EventToTrigger.FriendlyName);

			ToastManager.InstantiateToast(STRINGS.CHAOSEVENTS.WEATHERFORECAST.TOAST, string.Format(STRINGS.CHAOSEVENTS.WEATHERFORECAST.TOASTTEXT, EventToTrigger.FriendlyName));
			GameScheduler.Instance.Schedule("start weather", 20f, (_) => EventToTrigger.Trigger(null));
		};

		public Func<object, bool> Condition => (s) =>
		{
			List<EventInfo> weatherEvents = new List<EventInfo>();
			foreach (var e in WeatherEvents)
			{
				if (EventRegistration.TryGetEvent(e, out var eventInfo) && eventInfo.CheckCondition(null))
				{
					weatherEvents.Add(eventInfo);
				}
			}
			return weatherEvents.Count > 0;
		};

		public Danger EventDanger => Danger.Medium;
	}
}
