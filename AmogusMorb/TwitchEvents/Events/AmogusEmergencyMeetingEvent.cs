using Klei.AI;
using ONITwitchLib;
using AmogusMorb.TwitchEvents.TwitchEventAddons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.Image;
using Util_TwitchIntegrationLib;

namespace AmogusMorb.TwitchEvents.Events
{
    internal class AmogusEmergencyMeetingEvent : ITwitchEventBase
    {
        public string ID => "RTB_TwitchEvent_AmongUsEmergencyMeeting";
        public string EventName => "Emergency Meeting";

        public Danger EventDanger => Danger.Small;
        public string EventDescription => "It looks like there is an Imposter among us";
        public EventWeight EventWeight => (EventWeight)36;
        public Func<object, bool> Condition =>
                (data) =>
                {

                    if (GameClock.Instance.GetCycle() < 50 || Components.MinionIdentities.Count < 5 )
                        return false;
                    return true;
                };
        public Action<object> EventAction => 
            (data) =>
            {

                int worldID = ClusterManager.Instance.activeWorld.IsModuleInterior ? ClusterManager.Instance.WorldContainers[0].id : ClusterManager.Instance.activeWorldId;
                Telepad Printer;
                if (Components.Telepads.GetWorldItems(worldID).Count > 0 )
                    Printer = Components.Telepads.GetWorldItems(worldID).First();
                else 
                    Printer = Components.Telepads.items.data.GetRandom();

                int dupecount = Components.MinionIdentities.Items.Count();
                var printerCoords = Printer.transform.position;

                int max = 2 + dupecount < 20 ? 5 : Mathf.RoundToInt((float)dupecount / 4f);
                for (int i = 0; i< max; i++)
                {
                    GameObject go = i%5 == 0 ?
                    Util.KInstantiate(Assets.GetPrefab(ImposterConfig.ID)): 
                    Util.KInstantiate(Assets.GetPrefab(CrewMateConfig.ID));

                    go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(printerCoords), Grid.SceneLayer.Creatures));
                    go.SetActive(true);

                    //Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-3f, 3f) * 1f, (float)((double)UnityEngine.Random.value * 3.0 + 4.0));
                    //if (GameComps.Fallers.Has((object)go))
                    //    GameComps.Fallers.Remove(go);
                    //GameComps.Fallers.Add(go, initial_velocity);
                }
                ToastManager.InstantiateToastWithPosTarget(EventName, EventDescription, printerCoords);
            };

        public string EventGroupID => null;
    }
}
