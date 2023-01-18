using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.TwitchEvents
{
    internal class TwitchEventBase
    {
        TwitchEventBase CreateEvent()
        {
            return null;
        }

        public virtual string ID { get; }
        public virtual string EventName { get; }
        public virtual string EventDescription { get; }
        public virtual EventWeight EventProbability { get; }
        public int GetWeight => (int)EventProbability;
        public virtual Action<object> EventAction { get; }
        public virtual Func<object, bool> Condition { get; }
        public virtual Danger EventDanger { get; }
    }
    enum EventWeight
    {
        WEIGHT_NEVER = 0,
        WEIGHT_ALMOST_NEVER = 1,
        WEIGHT_RARE = 3,
        WEIGHT_NORMAL = 10,
        WEIGHT_COMMON = 30
    }
}
