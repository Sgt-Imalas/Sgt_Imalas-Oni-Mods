using ONITwitchLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
using Rockets_TinyYetBig.TwitchEvents.TwitchEventAddons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.TwitchEvents.Events
{
    internal class ImposterAmongUsVentEvent : ITwitchEventBase
    {
        public string ID => "RTB_TwitchEvent_AmongUsVent";
        public string EventName => "Vent Hopper";

        public Danger EventDanger => Danger.Small;
        public string EventDescription => "Something crawled out of the vent!";
        public EventWeight EventWeight => (EventWeight)33;
        public Func<object, bool> Condition =>
                (data) =>
                {

                    if (GameClock.Instance.GetCycle() < 25 || Components.MinionIdentities.Count < 5 || VentComponents.Vents.Count<1 ||UtilLibs.ModListUtils.ModIsActive("Amorbus"))
                        return false;
                    return true;
                };
        public Action<object> EventAction => 
            (data) =>
            {
                if (VentComponents.Vents.Count == 0)
                {
                    Debug.LogWarning("No Vents found");
                    return;

                }    
                var dupeCoords = VentComponents.Vents.Items
                    .Where(vent => !vent.IsNullOrDestroyed())
                    .Where(vent => Grid.Element[vent.NaturalBuildingCell()].id == SimHashes.Oxygen)
                    .ToList();


                Vector3 targetCoords;
                if (dupeCoords.Count == 0)
                    targetCoords = Grid.CellToPos(VentComponents.Vents.Items.GetRandom().NaturalBuildingCell());
                else
                    targetCoords = Grid.CellToPos(dupeCoords.GetRandom().NaturalBuildingCell());
                

                GameObject pet = GameUtil.KInstantiate(Assets.GetPrefab(ImposterConfig.ID), targetCoords, Grid.SceneLayer.Creatures);
                pet.SetActive(true);
                ToastManager.InstantiateToastWithPosTarget(EventName, EventDescription, targetCoords);
            };
    }
}
