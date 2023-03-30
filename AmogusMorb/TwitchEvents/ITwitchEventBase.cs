using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmogusMorb.TwitchEvents
{
    internal interface ITwitchEventBase
    {
         string ID { get; }
         string EventName { get; }
         string EventDescription { get; }
         EventWeight EventWeight { get; }
         Action<object> EventAction { get; }
         Func<object, bool> Condition { get; }
         Danger EventDanger { get; }
    }
    enum EventWeight
    {
        WEIGHT_NEVER = 0,
        WEIGHT_ALMOST_NEVER = 1,
        WEIGHT_RARE = 5,
        WEIGHT_NORMAL = 10,
        WEIGHT_COMMON = 30
    }
}
