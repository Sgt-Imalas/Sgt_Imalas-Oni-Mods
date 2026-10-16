using HarmonyLib;
using ONITwitchLib;
using ONITwitchLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Util_TwitchIntegrationLib
{
	public static class EventRegistration
	{
		public static Dictionary<string, ONITwitchLib.EventInfo> Events = new Dictionary<string, ONITwitchLib.EventInfo>();
		public static Dictionary<string, ONITwitchLib.EventInfo> GlobalCachedEvents = null;
		public static void InitializeTwitchEventsInNameSpace(string nameSpace)
		{
			InitializeTwitchEvents_AsquaredTwitchIntegration(nameSpace);
			InitializeTwitchEvents_TwitchColony(nameSpace);
		}
		private static void InitializeTwitchEvents_AsquaredTwitchIntegration(string nameSpace)
		{
			if (!TwitchModInfo.TwitchIsPresent)
			{
				Debug.LogWarning(nameSpace + ": Twitch Integration by Asquared is not enabled!");
				return;
			}
			Debug.Log("[" + nameSpace.Split('.').First() + "]: Registering Twitch Events!");
			RegisterAllEventsInNamespace_Asquared(nameSpace);
		}
		private static void InitializeTwitchEvents_TwitchColony(string nameSpace)
		{
			if (!TwitchColony.Api.TwitchColonyApi.IsAvailable)
			{
				Debug.LogWarning(nameSpace + ": OniColony Twitch Integration is not enabled!");
				return;
			}
			Debug.Log("[" + nameSpace.Split('.').First() + "]: Registering Twitch Events!");
			RegisterAllEventsInNamespace_TwitchColony(nameSpace);
		}

		public static bool TryGetEvent_Asquared(string eventId, out ONITwitchLib.EventInfo eventInfo)
		{
			eventInfo = null;
			if (!ConditionHelper.AsquaredTwitchIntegration_Active())
				return false;

			if (Events.TryGetValue(eventId, out eventInfo) && eventInfo != null)
			{
				return true;
			}
			CacheGlobalEventList();
			if (GlobalCachedEvents.TryGetValue(eventId, out eventInfo) && eventInfo != null)
			{
				return true;
			}
			return false;
		}
		public static bool TryGetEvent_TwitchColony(string eventId, out TwitchColony.Api.EventDataInfo eventInfo)
		{
			eventInfo = null;
			if (!ConditionHelper.TwitchColony_Active())
				return false;
			return TwitchColony.Api.TwitchColonyApi.TryGetEventData(eventId, out eventInfo);
		}
		static void CacheGlobalEventList()
		{
			if (GlobalCachedEvents != null)
				return;
			GlobalCachedEvents = new();

			var t_EventManager = Type.GetType("ONITwitch.EventLib.EventManager, ONITwitch");

			if (t_EventManager == null)
			{
				Debug.LogWarning("t_EventManager type is null");
				return;
			}

			var f_Instance = AccessTools.Field(t_EventManager, "instance");

			if (f_Instance == null)
			{
				Debug.LogWarning("f_Instance is null");
				return;
			}
			var instance = f_Instance.GetValue(null);
			if (instance == null)
			{
				Debug.LogWarning("EventManager instance is null");
				return;
			}

			var regex = new Regex(@"(.*)\.(.*)");

			try
			{
				var events = Traverse.Create(instance).Field("registeredEvents")?.GetValue<IDictionary>();
				Debug.Log(events);
				Debug.Log("count: " + events.Count);
				var eventmanager = EventManager.Instance;

				foreach (var nameSpacedEventId in events.Keys)
				{
					var eventIdWithNameSpace = nameSpacedEventId.ToString();
					var result = regex.Match(eventIdWithNameSpace);


					if (!result.Success || result.Groups.Count < 3)
					{
						Debug.LogWarning("Failed to match regex for " + eventIdWithNameSpace);
						continue;
					}

					var eventNameSpace = result.Groups[1].Value;
					var eventId = result.Groups[2].Value;
					var twitchEvent = eventmanager.GetEventByID(eventNameSpace, eventId);

					if (Events.ContainsKey(eventId))
					{
						Debug.LogWarning("Event " + eventId + " already exists in local events");
						continue;
					}

					if (twitchEvent != null)
					{
						if (GlobalCachedEvents.ContainsKey(eventId))
						{
							Debug.LogWarning("Event " + eventId + " already exists in global cached events");
							continue;
						}
						Debug.Log("Caching event " + eventId);
						GlobalCachedEvents[eventId] = twitchEvent;
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("Error while caching global events: " + e);
			}
		}
		private static IEnumerable<DictionaryEntry> CastDict(IDictionary dictionary)
		{
			foreach (DictionaryEntry entry in dictionary)
			{
				yield return entry;
			}
		}

		static void RegisterAllEventsInNamespace_Asquared(string nameSpace)
		{

			var asm = Assembly.GetExecutingAssembly();

			var events = asm.GetTypes().Where(p =>
				 p.Namespace != null &&
				 p.Namespace.Contains(nameSpace) &&
				 p.GetInterfaces().Contains(typeof(ITwitchEventBase))
			).ToList();

			foreach (Type eventType in events)
			{
				ITwitchEventBase Instance = (ITwitchEventBase)Activator.CreateInstance(eventType);
				if (Instance != default)
				{
					RegisterEvent_Asquared(Instance);
					Debug.Log("[" + nameSpace.Split('.').First() + "]: Registered event: " + Instance.EventName);
				}
			}
			Debug.Log(nameSpace.Split('.').First() + ": Added " + events.Count + " Twitch Events");
		}

		public static void RegisterEvent_Asquared(ITwitchEventBase twitchEvent)
		{
			if ((int)twitchEvent.EventWeight == (int)EventWeight.WEIGHT_NEVER)
				return;

			TwitchDeckManager deckInst = TwitchDeckManager.Instance;

			ONITwitchLib.EventInfo _event;
			ONITwitchLib.EventGroup _eventGroup;
			if (twitchEvent.EventGroupID.IsNullOrWhiteSpace())
			{
				(_event, _eventGroup) = EventGroup.DefaultSingleEventGroup(twitchEvent.ID, (int)twitchEvent.EventWeight, twitchEvent.EventName);
			}
			else
			{
				_eventGroup = EventGroup.GetOrCreateGroup(twitchEvent.EventGroupID);
				_event = _eventGroup.AddEvent(twitchEvent.ID, (int)twitchEvent.EventWeight, twitchEvent.EventName);
			}

			_event.AddListener(twitchEvent.EventAction);
			_event.AddCondition(twitchEvent.Condition);
			_event.Danger = twitchEvent.EventDanger;

			Events[twitchEvent.ID] = (_event);

			deckInst.AddGroup(_eventGroup);
		}
		static void RegisterAllEventsInNamespace_TwitchColony(string nameSpace)
		{

			var asm = Assembly.GetExecutingAssembly();

			var events = asm.GetTypes().Where(p =>
				 p.Namespace != null &&
				 p.Namespace.Contains(nameSpace) &&
				 p.GetInterfaces().Contains(typeof(ITwitchEventBase))
			).ToList();

			foreach (Type eventType in events)
			{
				ITwitchEventBase Instance = (ITwitchEventBase)Activator.CreateInstance(eventType);
				if (Instance != default)
				{
					RegisterEvent_TwitchColony(Instance, nameSpace);
					Debug.Log("[" + nameSpace.Split('.').First() + "]: Registered event: " + Instance.EventName);
				}
			}
			Debug.Log(nameSpace.Split('.').First() + ": Added " + events.Count + " Twitch Events");
		}


		public static TwitchColony.Api.EventDanger ToColonyDanger(this Danger danger) => danger switch
		{
			Danger.None => TwitchColony.Api.EventDanger.None,
			Danger.Small => TwitchColony.Api.EventDanger.Small,
			Danger.Medium => TwitchColony.Api.EventDanger.Medium,
			Danger.High => TwitchColony.Api.EventDanger.High,
			//extreme+deadly+everything else:
			_ => TwitchColony.Api.EventDanger.Deadly,
		};
		public static TwitchColony.Api.EventWeight ToColonyWeight(this EventWeight weight) => weight switch
		{
			EventWeight.WEIGHT_FREQUENT => TwitchColony.Api.EventWeight.VeryCommon,
			EventWeight.WEIGHT_COMMON => TwitchColony.Api.EventWeight.Common,
			EventWeight.WEIGHT_UNCOMMON => TwitchColony.Api.EventWeight.Uncommon,
			EventWeight.WEIGHT_RARE => TwitchColony.Api.EventWeight.Uncommon,
			EventWeight.WEIGHT_VERY_RARE=> TwitchColony.Api.EventWeight.Rare,
			EventWeight.WEIGHT_ALMOST_NEVER => TwitchColony.Api.EventWeight.Rare,
			//everything else:
			_ => TwitchColony.Api.EventWeight.Never,
		};

		public static void RegisterEvent_TwitchColony(ITwitchEventBase twitchEvent, string namespaceName)
		{
			if ((int)twitchEvent.EventWeight == (int)EventWeight.WEIGHT_NEVER)
				return;

			TwitchColony.Api.TwitchColonyApi.RegisterEvent(
				twitchEvent.ID,
				twitchEvent.EventName,
				twitchEvent.EventAction,
				twitchEvent.EventGroupID,
				twitchEvent.EventWeight.ToColonyWeight(),
				twitchEvent.EventDanger.ToColonyDanger(),
				twitchEvent.Condition,
				namespaceName);
		}
	}
}
