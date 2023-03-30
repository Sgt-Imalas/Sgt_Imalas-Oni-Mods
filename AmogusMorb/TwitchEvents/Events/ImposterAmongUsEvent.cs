using ONITwitchLib;
using AmogusMorb.TwitchEvents.TwitchEventAddons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmogusMorb.TwitchEvents.Events
{
    internal class ImposterAmongUsEven : ITwitchEventBase
    {
        public string ID => "RTB_TwitchEvent_AmongUsImposter";
        public string EventName => "Imposter Among Us";

        public Danger EventDanger => Danger.Small;
        public string EventDescription => "Emergency Meeting!\nI saw someone vent";
        public EventWeight EventWeight => (EventWeight)33;
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
                var susName = Components.MinionIdentities.Items.GetRandom();
                var dupeCoords = Components.MinionIdentities.Items.GetRandom().transform.position;

                GameObject pet = GameUtil.KInstantiate(Assets.GetPrefab(ImposterConfig.ID), dupeCoords, Grid.SceneLayer.Creatures);
                pet.SetActive(true);
                ToastManager.InstantiateToastWithPosTarget(EventName, string.Format(EventDescription, susName.GetProperName()), dupeCoords);
            };
    }
}
