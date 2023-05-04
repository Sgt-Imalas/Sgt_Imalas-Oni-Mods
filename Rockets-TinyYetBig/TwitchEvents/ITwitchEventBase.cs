using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.TwitchEvents
{
    public interface ITwitchEventBase
    {
         string ID { get; }
         string EventName { get; }
         string EventDescription { get; }
         EventWeight EventWeight { get; }
         Action<object> EventAction { get; }
         Func<object, bool> Condition { get; }
         Danger EventDanger { get; }
    }
    public enum EventWeight
    {
        WEIGHT_NEVER = 0,
        WEIGHT_ALMOST_NEVER = 1,
        WEIGHT_VERY_RARE = 4,
        WEIGHT_RARE = 8,
        WEIGHT_NORMAL = 16,
        WEIGHT_COMMON = 40
    }
}
