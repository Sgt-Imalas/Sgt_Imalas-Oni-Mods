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
			PickRandomWeatherEvent();
		};

		class WeatherEventTrigger
		{
			public WeatherEventTrigger(string id, string friendly, System.Action<object> trigger)
			{
				Id = id;
				FriendlyName = friendly;
				TriggerEventAction = trigger;
			}
			public string Id;
			public string FriendlyName;
			public System.Action<object> TriggerEventAction;
		}

		void PickRandomWeatherEvent()
		{
			WeightedList<WeatherEventTrigger> weatherEvents = new();
			foreach (var e in WeatherEvents)
			{
				if (ConditionHelper.AsquaredTwitchIntegration_Active() && EventRegistration.TryGetEvent_Asquared(e, out var eventInfo))
				{
					bool eventAllowed = eventInfo.CheckCondition(null);
					int weight = (int)EventWeight.WEIGHT_RARE;
					var group = eventInfo.Group;
					if (group != null)
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
						weatherEvents.Add(new WeatherEventTrigger(eventInfo.Id, eventInfo.FriendlyName, (data) => eventInfo.Trigger(data)), weight);
					}
					SgtLogger.l("potential weather event: " + eventInfo.FriendlyName + ", can it execute: " + eventAllowed + ", event weight: " + weight);
				}
				if (ConditionHelper.TwitchColony_Active() && EventRegistration.TryGetEvent_TwitchColony(e, out var eventInfo2))
				{
					SgtLogger.l("potential weather event: " + eventInfo2.DisplayName + " (" + eventInfo2.Id + ")" + ", event weight: " + eventInfo2.Weight + ", eligible: " + eventInfo2.Eligible);
					if (eventInfo2.Eligible)
					{
						weatherEvents.Add(new(eventInfo2.Id, eventInfo2.DisplayName, (data) => TwitchColony.Api.TwitchColonyApi.TriggerEvent(eventInfo2.Id)), eventInfo2.Weight);
					}
				}
			}
			if (!weatherEvents.Any())
			{
				SgtLogger.error("No available weather events found, aborting");
				return;
			}

			var EventToTrigger = weatherEvents.Next();
			SgtLogger.l("found weather event: " + EventToTrigger.FriendlyName+ " ("+EventToTrigger.Id+")");
			ToastHelper.InstantiateToast(STRINGS.CHAOSEVENTS.WEATHERFORECAST.TOAST, string.Format(STRINGS.CHAOSEVENTS.WEATHERFORECAST.TOASTTEXT, EventToTrigger.FriendlyName));
			GameScheduler.Instance.Schedule("start weather", 20f, EventToTrigger.TriggerEventAction);
		}



		public Func<object, bool> Condition => (s) =>
		{
			return ConditionHelper.FindAnyEligibleEvent(WeatherEvents);
		};

		public Danger EventDanger => Danger.None;
	}
}
