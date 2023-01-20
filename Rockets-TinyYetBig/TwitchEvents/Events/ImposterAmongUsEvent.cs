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
    internal class ImposterAmongUsEven : ITwitchEventBase
    {
        public string ID => "RTB_TwitchEvent_AmongUsImposter";
        public string EventName => "Imposter Among Us";

        public Danger EventDanger => Danger.Small;
        public string EventDescription => "{0} is sus";
        public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;
        public Func<object, bool> Condition =>
                (data) =>
                {

                    if (GameClock.Instance.GetCycle() < 100 || Components.MinionIdentities.Count < 5 && UtilLibs.ModListUtils.ModIsActive("Amorbus"))
                        return false;
                    return true;
                };
        public Action<object> EventAction => 
            (data) =>
            {
                SpaceStationManager.GetRockets(out var rockets);
                rockets.ShuffleList();
                string susName = Components.MinionIdentities.Items.GetRandom().GetProperName();
                var dupeCoords = Components.MinionIdentities.Items.GetRandom().transform.position;


                foreach (Clustercraft craft in Components.MinionIdentities)
                    if (craft.Status == Clustercraft.CraftStatus.InFlight)
                    {
                        GameObject pet = GameUtil.KInstantiate(Assets.GetPrefab(GlomConfig.ID), Components.Telepads[0].gameObject.transform.position, Grid.SceneLayer.Creatures);
                        pet.SetActive(true);
                        ToastManager.InstantiateToast(EventName, string.Format(EventDescription,craft.Name));
                        break;
                    }
            };
    }
}
