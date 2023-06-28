using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Util_TwitchIntegrationLib.EventRegistration;

namespace Util_TwitchIntegrationLib
{
    public interface ITwitchEventBase
    {
        string ID { get; }
        string EventGroupID { get; }
        string EventName { get; }
        string EventDescription { get; }
        EventWeight EventWeight { get; }
        Action<object> EventAction { get; }
        Func<object, bool> Condition { get; }
        Danger EventDanger { get; }
    }
}
