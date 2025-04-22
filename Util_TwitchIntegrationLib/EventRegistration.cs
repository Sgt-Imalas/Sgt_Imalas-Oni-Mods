using HarmonyLib;
using ONITwitchLib;
using ONITwitchLib.Core;
using ONITwitchLib.Logger;
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
			if (!TwitchModInfo.TwitchIsPresent)
			{
				Debug.LogWarning(nameSpace + ": Twitch not enabled!");
				return;
			}
			Debug.Log("[" + nameSpace.Split('.').First() + "]: Registering Twitch Events!");
			RegisterAllEventsInNamespace(nameSpace);
		}

		public static bool TryGetEvent(string eventId, out ONITwitchLib.EventInfo eventInfo)
		{
			eventInfo = null;
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
				Debug.Log("count: "+events.Count);
				var eventmanager = EventManager.Instance;
					
				foreach (var nameSpacedEventId in events.Keys)
				{
					var eventIdWithNameSpace = nameSpacedEventId.ToString();
					var result = regex.Match(eventIdWithNameSpace);


					if (!result.Success ||result.Groups.Count<3)
					{
						Debug.LogWarning("Failed to match regex for " + eventIdWithNameSpace);
						continue;
					}

					var eventNameSpace = result.Groups[1].Value;
					var eventId = result.Groups[2].Value;
					var twitchEvent = eventmanager.GetEventByID(eventNameSpace,eventId);

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

		static void RegisterAllEventsInNamespace(string nameSpace)
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
					RegisterEvent(Instance);
					Debug.Log("[" + nameSpace.Split('.').First() + "]: Registered event: " + Instance.EventName);
				}
			}
			Debug.Log(nameSpace.Split('.').First() + ": Added " + events.Count + " Twitch Events");
		}

		public static void RegisterEvent(ITwitchEventBase twitchEvent)
		{
			if ((int)twitchEvent.EventWeight == (int)EventWeight.WEIGHT_NEVER)
				return;

			TwitchDeckManager deckInst = TwitchDeckManager.Instance;

			ONITwitchLib.EventInfo _event;
			ONITwitchLib.EventGroup _eventGroup;
			if (twitchEvent.EventGroupID == null || twitchEvent.EventGroupID.Length == 0 || twitchEvent.EventGroupID == string.Empty)
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
	}
}
