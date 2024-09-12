using Klei.AI;
using System.Collections.Generic;
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


		public override void OnPrefabInit()
		{
			this.GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse);
			this.Subscribe((int)GameHashes.StartedTalking, new System.Action<object>(this.OnStartedTalking));
			//this.Subscribe((int)GameHashes.StoppedTalking, new System.Action<object>(this.StoppedTalking));
		}


		public void StoppedTalking(object data)
		{
		}

		public void OnStartedTalking(object data)
		{

			if (data is StartedTalkingEvent talkingEvent && talkingEvent.talker != identity.gameObject)
			{
				if (UnityEngine.Random.Range(0, 20) <= 1)
				{
					if (talkingEvent.talker.TryGetComponent<Effects>(out var _targetEffects))
					{
						int increase = 0;
						float scieneModifier = attributes.GetConverter(Db.Get().AttributeConverters.ResearchSpeed.Id).Evaluate();

						if (scieneModifier < 1)
							scieneModifier = 1;
						scieneModifier += 1;

						increase = (int)((scieneModifier / 3f) + 2);
						var effectDuration = 400 + 150f * scieneModifier;

						var ChatEffect = new Effect(
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

						ChatEffect.duration = effectDuration;
						ChatEffect.SelfModifiers = new List<AttributeModifier>()
						{
							new AttributeModifier(Db.Get().Attributes.Learning.Id, increase)
						};

						if (!_targetEffects.HasEffect(effectID))
						{
							_targetEffects.Add(ChatEffect, true);
							SgtLogger.l("added infoConversation effect, science mod: " + scieneModifier + " duration: " + ChatEffect.duration + ", strenght: " + increase);
						}
						else
						{
							EffectInstance activeEffect = _targetEffects.Get(effectID);

							if (activeEffect.timeRemaining < effectDuration)
							{
								_targetEffects.Remove(effectID);
								_targetEffects.Add(ChatEffect, true);
								SgtLogger.l("refreshed infoConversation effect, science mod: " + scieneModifier + " duration: " + ChatEffect.duration + ", strenght: " + increase);
							}
						}
					}
				}
				else
					SgtLogger.l("interesting conversation");
			}
		}
	}
}
