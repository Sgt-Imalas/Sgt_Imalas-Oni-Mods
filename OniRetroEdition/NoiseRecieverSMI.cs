using OniRetroEdition.ModPatches;

namespace OniRetroEdition
{

	public class NoiseRecieverSMI : GameStateMachine<NoiseRecieverSMI, NoiseRecieverSMI.Instance, IStateMachineTarget, NoiseRecieverSMI.Def>
	{
		private State AllQuiet;
		private State NormalNoise;
		private State LoudNoise;
		private State VeryLoudNoise;
		public FloatParameter timeUntilNextExposureReact;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = NormalNoise;

			AllQuiet
				.Transition(NormalNoise, smi => smi.GetLoudness() >= 20)
				.ToggleEffect("NoisePeaceful");

			NormalNoise
				.Transition(AllQuiet, smi => smi.GetLoudness() < 20)
				.Transition(LoudNoise, smi => smi.GetLoudness() >= 70);
			LoudNoise
				.Transition(NormalNoise, smi => smi.GetLoudness() < 70)
				.Transition(VeryLoudNoise, smi => smi.GetLoudness() >= 120)
				.ToggleEffect("NoiseMinor");
			VeryLoudNoise
				.Transition(LoudNoise, smi => smi.GetLoudness() < 120)
				.ToggleEffect("NoiseMajor");
		}
		public class Def : BaseDef
		{
		}

		public new class Instance : GameInstance
		{
			public Instance(IStateMachineTarget master, Def def) : base(master, def)
			{
			}
			//called every 200ms;
			public float GetLoudness()
			{
				int cell = Grid.PosToCell(this);
				if (AudioEventManager.Get() == null) { return 0; }
				var timeTillNextReact = smi.sm.timeUntilNextExposureReact.Delta(-0.2f, smi);

				var decibils = AudioEventManager.Get().GetDecibelsAtCell(cell);

				if(timeTillNextReact <= 0 && !smi.HasTag(GameTags.InTransitTube) && decibils >= 100)
				{
					//smi.sm.timeUntilNextExposureReact.Set(120f, smi);
					smi.master.gameObject.GetSMI<ReactionMonitor.Instance>()
						.AddSelfEmoteReactable(smi.master.gameObject, (HashedString) "NoiseLevelHighReact", Emotes_Patches.High_Noise_React, true, Db.Get().ChoreTypes.EmoteHighPriority);
				}
				return decibils;

			}
		}
	}
}
