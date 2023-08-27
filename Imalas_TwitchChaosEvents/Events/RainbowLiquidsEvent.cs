using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util_TwitchIntegrationLib;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Events
{
    internal class RainbowLiquidsEvent : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_RainbowLiquidsEvent";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.NAME;

        public string EventDescription => STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.TOASTTEXT;

        public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;

        public Action<object> EventAction => (object data) =>
        {
            ModAssets.RainbowLiquids = true;
            ToastManager.InstantiateToast(
                STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.TOAST,
                 STRINGS.CHAOSEVENTS.RAINBOWLIQUIDS.TOASTTEXT
                 );

           

            GameScheduler.Instance.Schedule("rainbow liquid disable", 600f, _ =>
            {
                ModAssets.RainbowLiquids = false;
            });
        };

        public Func<object, bool> Condition =>
            (data) => true;

        public Danger EventDanger => Danger.None;

    }
}
