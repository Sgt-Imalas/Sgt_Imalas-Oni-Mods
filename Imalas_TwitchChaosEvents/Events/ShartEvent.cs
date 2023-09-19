using Database;
using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
    internal class ShartEvent : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_Shart";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.SHART.NAME;

        public string EventDescription => STRINGS.CHAOSEVENTS.SHART.TOASTTEXT;

        public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;

        public Action<object> EventAction => (obj) =>
        {
            foreach (var minion in Components.LiveMinionIdentities.Items)
            {
                DoShart(minion.gameObject, 25f);
            }

            DoCringeEffect();

            ToastManager.InstantiateToast(STRINGS.CHAOSEVENTS.SHART.TOAST, STRINGS.CHAOSEVENTS.SHART.TOASTTEXT);
        };


        private static readonly AccessTools.FieldRef<HashedString[]> WorkAnimsGetter =
            AccessTools.StaticFieldRefAccess<HashedString[]>(AccessTools.Field(typeof(Flatulence), "WorkLoopAnims"));

        // most of this logic copied from Asquared `Fart`-Event
        private static void DoShart(GameObject dupe, float shartMass)
        {
            var dupePos = dupe.transform.position;
            var temperature = Db.Get().Amounts.Temperature.Lookup(dupe).value;
            var diseaseIDX = Db.Get().Diseases.GetIndex("FoodPoisoning");
            var diseaseCount = 200000;

            var equippable = dupe.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
            if (equippable!=null)
            {
                equippable.GetComponent<Storage>()
                    .AddLiquid(ModElements.LiquidPoop.SimHash, shartMass, temperature, diseaseIDX, diseaseCount, false);
            }
            else
            {
                SimMessages.AddRemoveSubstance(
                    Grid.PosToCell(dupePos),
                    ModElements.LiquidPoop.SimHash,
                    CellEventLogger.Instance.ElementConsumerSimUpdate,
                    shartMass,
                    temperature,
                    diseaseIDX,
                    diseaseCount
                );
                var effect = FXHelpers.CreateEffect(
                    "odor_fx_kanim",
                    dupePos,
                    dupe.transform,
                    true
                );
                effect.Play(WorkAnimsGetter());
                effect.destroyOnAnimComplete = true;
            }

            var objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(dupe);
            var audioPos = dupePos with { z = 0.0f };
            var volume = 3f;
            if (objectIsSelectedAndVisible)
            {
                audioPos = SoundEvent.AudioHighlightListenerPosition(audioPos);
                volume = SoundEvent.GetVolume(true);
            }

            KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence"), audioPos, volume);
        }


        private static void DoCringeEffect()
        {
            foreach (var minionIdentity in Components.LiveMinionIdentities.Items)
            {
                minionIdentity.Trigger(
                    (int)GameHashes.Cringe,
                    Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String
                );
                minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
            }
        }
        public Func<object, bool> Condition => (data) =>
        {
           return Components.LiveMinionIdentities.Count > 0;
        };

        public Danger EventDanger => Danger.Medium;
    }
}
