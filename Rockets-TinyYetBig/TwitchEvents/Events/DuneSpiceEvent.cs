using Database;
using Klei.AI;
using KSerialization;
using ONITwitchLib;
using Rockets_TinyYetBig.SpaceStations;
using Rockets_TinyYetBig.TwitchEvents.SpaceSpice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static EdiblesManager;
using static STRINGS.CREATURES.SPECIES;

namespace Rockets_TinyYetBig.TwitchEvents.Events
{
    public class DuneSpicetEvent : ITwitchEventBase
    {
        float EventDuration = 600f * 5;
        public string ID => "RTB_TwitchEvent_DuneSpice";
        public string EventName => "The Spice must flow!";

        public Danger EventDanger => Danger.None;
        public string EventDescription => "\"With best regards from the Spacing Guild\"\nAll (rocket) Navigators have recieved an extra portion of Melange.";
        public EventWeight EventWeight => (EventWeight)(40);
        public Func<object, bool> Condition =>
                (data) =>
                {
                    var RocketryPerk = Db.Get().SkillPerks.CanUseRocketControlStation;
                    foreach (MinionIdentity dupe in Components.MinionIdentities)
                    {
                        if(dupe.TryGetComponent<MinionResume>(out var resume)){
                            if(resume.HasPerk(RocketryPerk))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                };
        public Action<object> EventAction => 
            (data) =>
            {
                List<MinionResume> Pilots = new List<MinionResume>();
                var RocketryPerk = Db.Get().SkillPerks.CanUseRocketControlStation;
                foreach (MinionIdentity dupe in Components.MinionIdentities)
                {
                    if (dupe.TryGetComponent<MinionResume>(out var resume))
                    {
                        if (resume.HasPerk(RocketryPerk))
                        {
                            Pilots.Add(resume);
                        }
                    }
                    var DuneSpice = Db.Get().effects.Get("PILOTING_SPICE");
                    DuneSpice.duration = EventDuration;
                    DuneSpice.Name = "Melange Spice";
                    DuneSpice.description= global::STRINGS.ITEMS.SPICES.PILOTING_SPICE.DESC;

                    foreach (var pilot in Pilots)
                    {
                        if(pilot.TryGetComponent<Effects>(out var effect)) 
                        {
                            
                            effect.Add(DuneSpice, true);
                            if (pilot.TryGetComponent<SpiceEyes>(out var spiceEyes))
                                spiceEyes.AddEyeDuration(EventDuration);
                        }
                        SgtLogger.debuglog(pilot.name +" got the spice eyes");
                    }
                    ToastManager.InstantiateToast(EventName, EventDescription);
                }
            };
    }
}
