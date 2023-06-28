using ClipperLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ConversationManager;

namespace NeuralVaccilatorExpanded
{
    internal class NVE_SharingGenius : KMonoBehaviour
    {
        [MyCmpGet]
        private MinionIdentity identity;
        [MyCmpGet]
        private Effects ownEffects;

        [MyCmpGet]
        private AttributeConverters attributes;



        public static float duration = 600;
        public static string effectID = "NVE_ThoughtfullChatter";

        public static Effect ChatEffect = new Effect(
                     effectID,
                     STRINGS.DUPLICANTS.STATUSITEMS.NVE_THOUGHTFULLCHATTER.NAME,
                     STRINGS.DUPLICANTS.STATUSITEMS.NVE_THOUGHTFULLCHATTER.TOOLTIP,
                     duration,
                     true,
                     true,
                     false)
        {
            SelfModifiers = new List<AttributeModifier>()
            {
                new AttributeModifier(Db.Get().Attributes.Learning.Id, 4)
            }
        };

        public override void OnPrefabInit()
        {
            this.GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse);
            this.Subscribe((int)GameHashes.StartedTalking, new System.Action<object>(this.OnStartedTalking));
            //this.Subscribe((int)GameHashes.StoppedTalking, new System.Action<object>(this.StoppedTalking));
        }


        public void StoppedTalking(object data)
        {
            if (data is StartedTalkingEvent talkingEvent
                //&& UnityEngine.Random.Range(0, 50) <= 1
                && talkingEvent.talker != identity)
            {
                if (talkingEvent.talker.TryGetComponent<Effects>(out var _targetEffects))
                {
                    if (!_targetEffects.HasEffect(effectID)
                        && TalkingPoints.ContainsKey(talkingEvent.talker)
                        && TalkingPoints[talkingEvent.talker] <= 0)
                    {
                        float scieneModifier = attributes.GetConverter(Db.Get().AttributeConverters.ResearchSpeed.Id).Evaluate();
                        ChatEffect.duration = 300f + 600f * scieneModifier;
                        ChatEffect.SelfModifiers = new List<AttributeModifier>()
                        {
                            new AttributeModifier(Db.Get().Attributes.Learning.Id, Mathf.RoundToInt(4f*(1+scieneModifier)))
                        };
                        _targetEffects.Add(ChatEffect, true);

                        SgtLogger.l("added info effect,science mod: " + scieneModifier + " duration: " + ChatEffect.duration + ", strenght: " + Mathf.RoundToInt(4f * (1 + scieneModifier)));
                    }
                }
            }

            // TalkingPoints.Clear();
        }

        public void OnStartedTalking(object data)
        {

            if (data is StartedTalkingEvent talkingEvent
                //&& UnityEngine.Random.Range(0, 50) <= 1
                && talkingEvent.talker != identity)
            {
                if (talkingEvent.talker.TryGetComponent<Effects>(out var _targetEffects))
                {
                    if (!_targetEffects.HasEffect(effectID))
                    {
                        float scieneModifier = attributes.GetConverter(Db.Get().AttributeConverters.ResearchSpeed.Id).Evaluate();
                        ChatEffect.duration = 300f + 600f * scieneModifier;
                        ChatEffect.SelfModifiers = new List<AttributeModifier>()
                        {
                            new AttributeModifier(Db.Get().Attributes.Learning.Id, Mathf.RoundToInt(4f*(1+scieneModifier)))
                        };
                        _targetEffects.Add(ChatEffect, true);

                        SgtLogger.l("added info effect,science mod: " + scieneModifier + " duration: " + ChatEffect.duration + ", strenght: " + Mathf.RoundToInt(4f * (1 + scieneModifier)));
                    }
                }
            }
        }
    }
}
