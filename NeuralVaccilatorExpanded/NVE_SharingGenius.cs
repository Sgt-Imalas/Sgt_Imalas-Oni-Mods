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
        [MyCmpGet]
        private Effects ownEffects;

        Dictionary<MinionIdentity,int> TalkingPoints = new Dictionary<MinionIdentity,int>();


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
            this.Subscribe((int)GameHashes.StoppedTalking, new System.Action<object>(this.StoppedTalking));
        }


        public void StoppedTalking(object data)
        {
            TalkingPoints.Clear();
        }

        public void OnStartedTalking(object data)
        {
            Debug.Log(data);
            Debug.Log(data.GetType());
            if (data is MinionIdentity other
                //&& UnityEngine.Random.Range(0, 50) <= 1
                && other != identity)
            {
                if (other.TryGetComponent<Effects>(out var _targetEffects))
                {
                    if (!_targetEffects.HasEffect(effectID))
                    {
                        SgtLogger.l("Triggered thoughtful conversation");

                        ChatEffect.duration = 
                        _targetEffects.Add(ChatEffect, true);                        
                    }
                }
            }
        }
    }
}
