using ONITwitchLib;
using ONITwitchLib.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Util_TwitchIntegrationLib
{
    public static class EventRegistration
    {
        public static void InitializeTwitchEventsInNameSpace(string nameSpace)
        {
            if (!TwitchModInfo.TwitchIsPresent)
            {
                Debug.LogWarning(nameSpace + ": Twitch not enabled!");
                return;
            }

            Debug.Log("["+nameSpace.Split('.').First() + "]: Registering Twitch Events!");
            RegisterAllEventsInNamespace(nameSpace);
        }

        static void RegisterAllEventsInNamespace(string nameSpace)
        {

            var asm = Assembly.GetExecutingAssembly();

            var events = asm.GetTypes().Where(p =>
                 p.Namespace != null &&
                 p.Namespace.Contains (nameSpace)&&
                 p.GetInterfaces().Contains(typeof(ITwitchEventBase))
            ).ToList();

            foreach (Type eventType in events)
            {
                ITwitchEventBase Instance = (ITwitchEventBase)Activator.CreateInstance(eventType);
                if(Instance != default)
                    RegisterEvent(Instance);
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
            if(twitchEvent.EventGroupID == null || twitchEvent.EventGroupID.Length==0 || twitchEvent.EventGroupID == string.Empty)
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
            deckInst.AddGroup(_eventGroup);
        }
    }
}
