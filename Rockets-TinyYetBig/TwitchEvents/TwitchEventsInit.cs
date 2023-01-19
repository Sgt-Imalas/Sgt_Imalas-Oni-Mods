using ONITwitchLib.Core;
using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DebugHandler;
using Rockets_TinyYetBig.TwitchEvents.Events;
using System.Reflection;

namespace Rockets_TinyYetBig.TwitchEvents
{
    internal class TwitchEventsInit
    {
        static bool initalized = false;
        static EventManager eventInst;
        static DataManager dataInst;
        static TwitchDeckManager deckInst;

        public const int WEIGHT_NEVER = 0;
        public const int WEIGHT_ALMOST_NEVER = 1;
        public const int WEIGHT_RARE = 3;
        public const int WEIGHT_NORMAL = 10;
        public const int WEIGHT_COMMON = 30;

        static void Init()
        {
            if (initalized)
                return;

            eventInst = EventManager.Instance;
            dataInst = DataManager.Instance;
            deckInst = TwitchDeckManager.Instance;

            initalized = true;
            Debug.Log("Rocketry Expanded: Initalized Twitch integration");
        }
        public static void RegisterAll()
        {
            if (!TwitchModInfo.TwitchIsPresent)
            {
                Debug.LogWarning($"Rocketry Expanded: Twitch not enabled!");
                return;
            }

            Init();
            RegisterAllEvents();
            return;
            RegisterEvent(new RocketBoostEvent());
            RegisterEvent(new RocketBoostEventAll());
        }

        static void RegisterAllEvents()
        {
            var nameSpace = "Rockets_TinyYetBig.TwitchEvents.Events";

            var asm = Assembly.GetExecutingAssembly();
            var events = asm.GetTypes().Where(p =>
                 p.Namespace == nameSpace &&
                 p.IsInstanceOfType(typeof(ITwitchEventBase))
            ).ToList();
        }

        public static void RegisterEvent(ITwitchEventBase twitchEvent)
        {
            if ((int)twitchEvent.EventWeight == WEIGHT_NEVER)
                return;

            var deckInst = TwitchDeckManager.Instance;

            var (_event, _eventGroup) = EventGroup.DefaultSingleEventGroup(twitchEvent.ID, (int)twitchEvent.EventWeight, twitchEvent.EventName);
            _event.AddListener(twitchEvent.EventAction);
            _event.AddCondition(twitchEvent.Condition);
            _event.Danger = twitchEvent.EventDanger;
            deckInst.AddGroup(_eventGroup);
        }


    }
}
