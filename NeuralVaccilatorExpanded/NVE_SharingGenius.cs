using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static ConversationManager;

namespace NeuralVaccilatorExpanded
{
    internal class NVE_SharingGenius : KMonoBehaviour
    {
        [MyCmpGet]
        private MinionIdentity identity;

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
            this.identity = this.GetComponent<MinionIdentity>(); 
            
        }

        public void OnStartedTalking(object data)
        {
            Debug.Log(data);
            if (data is StartedTalkingEvent talkingEvent
                && UnityEngine.Random.Range(0, 50) <= 1
                && talkingEvent.talker != identity)
            {
                if (talkingEvent.talker.TryGetComponent<Effects>(out var _targetEffects))
                {
                    if (!_targetEffects.HasEffect(effectID))
                    {
                        SgtLogger.l("Triggered thoughtful conversation");
                        _targetEffects.Add(ChatEffect, true);
                    }
                }
            }
        }
    }
}
