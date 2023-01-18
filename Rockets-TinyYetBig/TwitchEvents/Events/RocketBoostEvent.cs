using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.TwitchEvents.Events
{
    internal class RocketBoostEvent : TwitchEventBase
    {
        public override string ID => "RTB_TwitchEvent_RocketBoost";
        public override string EventName => "Rocket Boost";

        public override Danger EventDanger => Danger.None;
        public override string EventDescription => "A rocket has recieved a boost!";
        public override EventWeight EventProbability => EventWeight.WEIGHT_NORMAL;
        public override Func<object, bool> Condition 
            =>
                data =>
                {
                    foreach (Clustercraft craft in Components.Clustercrafts)
                        if (craft.Status == Clustercraft.CraftStatus.InFlight)
                            return true;

                    return false;
                };
        public override Action<object> EventAction => (
            data =>
            {
                List<Clustercraft> rockets = Components.Clustercrafts.Items;
                rockets.ShuffleList();
                foreach (Clustercraft craft in rockets)
                    if (craft.Status == Clustercraft.CraftStatus.InFlight)
                    {
                        craft.controlStationBuffTimeRemaining = Math.Max(600f, (float)new System.Random().Next(300,900)) ;
                        break;
                    }
                ToastManager.InstantiateToast(EventName, EventDescription);
            });
    }
}
