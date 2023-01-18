using ONITwitchLib;
using Rockets_TinyYetBig.SpaceStations;
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
        public override string EventDescription => "{0} has recieved a boost!";
        public override EventWeight EventProbability => EventWeight.WEIGHT_NORMAL;
        public override Func<object, bool> Condition 
            =>
                data =>
                {
                    if(SpaceStationManager.GetRockets(out var rockets))
                    {
                        foreach (Clustercraft craft in rockets)
                            if (craft.Status == Clustercraft.CraftStatus.InFlight && craft.controlStationBuffTimeRemaining<=0)
                                return true;
                    }

                    return false;
                };
        public override Action<object> EventAction => (
            data =>
            {
                SpaceStationManager.GetRockets(out var rockets);
                rockets.ShuffleList();
                foreach (Clustercraft craft in rockets)
                    if (craft.Status == Clustercraft.CraftStatus.InFlight && craft.controlStationBuffTimeRemaining <= 0)
                    {
                        craft.controlStationBuffTimeRemaining = (float)new System.Random().Next(600,1200);

                        ToastManager.InstantiateToast(EventName, string.Format(EventDescription,craft.Name));
                        break;
                    }
            });
    }
}
