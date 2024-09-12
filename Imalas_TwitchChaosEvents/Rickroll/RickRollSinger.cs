using Klei.AI;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Rickroll
{
	internal class RickRollSinger : GameStateMachine<RickRollSinger, RickRollSinger.Instance>
	{
		private Vector3 offset = new Vector3(0.0f, 0.0f, 0.1f);
		public GameStateMachine<RickRollSinger, RickRollSinger.Instance, IStateMachineTarget, object>.State neutral;
		public RickRollSinger.OverjoyedStates overjoyed;
		public string soundPath = GlobalAssets.GetSound("DupeSinging_NotesFX_LP");

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = neutral;
			this.root.TagTransition(GameTags.Dead, null);
			this.neutral.TagTransition(GameTags.Overjoyed, overjoyed);
			this.overjoyed.DefaultState(this.overjoyed.idle)
				.TagTransition(GameTags.Overjoyed, this.neutral, true).ToggleEffect("IsJoySinger")
				.ToggleLoopingSound(this.soundPath)
				.ToggleAnims("anim_loco_singer_kanim").ToggleAnims("anim_idle_singer_kanim")
				.EventHandler(GameHashes.TagsChanged, (smi, obj) => smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep))).Enter(smi =>
			{
				smi.musicParticleFX = Util.KInstantiate(EffectPrefabs.Instance.HappySingerFX, smi.master.transform.GetPosition() + this.offset);
				smi.musicParticleFX.transform.SetParent(smi.master.transform);
				smi.CreatePasserbyReactable();
				smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep));
			}).Update((smi, dt) =>
			{
				if (smi.GetSpeechMonitor().IsPlayingSpeech() || !SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
					return;
				// smi.GetSpeechMonitor().PlaySpeech(Db.Get().Thoughts.CatchyTune.speechPrefix, Db.Get().Thoughts.CatchyTune.sound);
			}, UpdateRate.SIM_1000ms).Exit(smi =>
			{
				Util.KDestroyGameObject(smi.musicParticleFX);
				smi.ClearPasserbyReactable();
				smi.musicParticleFX.SetActive(false);
			});
		}

		public class OverjoyedStates :
		  GameStateMachine<RickRollSinger, RickRollSinger.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<RickRollSinger, RickRollSinger.Instance, IStateMachineTarget, object>.State idle;
			public GameStateMachine<RickRollSinger, RickRollSinger.Instance, IStateMachineTarget, object>.State moving;
		}

		public new class Instance :
		  GameStateMachine<RickRollSinger, RickRollSinger.Instance, IStateMachineTarget, object>.GameInstance
		{
			private Reactable passerbyReactable;
			public GameObject musicParticleFX;
			public SpeechMonitor.Instance speechMonitor;

			public Instance(IStateMachineTarget master)
			  : base(master)
			{
			}

			public void CreatePasserbyReactable()
			{
				if (this.passerbyReactable != null)
					return;
				EmoteReactable emoteReactable = new EmoteReactable(this.gameObject, (HashedString)"WorkPasserbyAcknowledgement", Db.Get().ChoreTypes.Emote, 5, 5, localCooldown: 600f);
				Emote sing = Db.Get().Emotes.Minion.Sing;
				emoteReactable.SetEmote(sing).SetThought(Db.Get().Thoughts.CatchyTune).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor));
				emoteReactable.RegisterEmoteStepCallbacks((HashedString)"react", new System.Action<GameObject>(this.AddReactionEffect), null);
				this.passerbyReactable = emoteReactable;
			}

			public SpeechMonitor.Instance GetSpeechMonitor()
			{
				if (this.speechMonitor == null)
					this.speechMonitor = this.master.gameObject.GetSMI<SpeechMonitor.Instance>();
				return this.speechMonitor;
			}

			private void AddReactionEffect(GameObject reactor) => reactor.Trigger(-1278274506);

			private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition) => transition.end == NavType.Floor;

			public void ClearPasserbyReactable()
			{
				if (this.passerbyReactable == null)
					return;
				this.passerbyReactable.Cleanup();
				this.passerbyReactable = null;
			}
		}
	}

}
