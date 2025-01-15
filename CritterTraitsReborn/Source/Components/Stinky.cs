using UnityEngine;
using UtilLibs;
using static Klei.SimUtil;

namespace CritterTraitsReborn.Components
{
	// NOTE: Copy of the original Stinky, modified
	[SkipSaveFileSerialization]
	public class Stinky : StateMachineComponent<Stinky.StatesInstance>
	{
		const float STINKY_EMIT_INTERVAL_MIN = 10f;
		const float STINKY_EMIT_INTERVAL_MAX = 30f;

		public class StatesInstance : GameStateMachine<States, StatesInstance, Stinky, object>.GameInstance
		{
			public StatesInstance(Stinky master) : base(master) { }
		}

		public class States : GameStateMachine<States, StatesInstance, Stinky>
		{
			public State idle;

			public State emit;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = idle;

				root
				  .Update("StinkyFX", (smi, dt) =>
				  {
					  smi.master.stinkyController?.Play(WorkLoopAnims);
				  }, UpdateRate.SIM_4000ms);

				idle
				  .Enter("ScheduleNextFart", smi =>
				  {
					  smi.ScheduleGoTo(GetNewInterval(), emit);
				  });

				emit
				  .Enter("Fart", smi =>
				  {
					  smi.master.Emit(smi.master.gameObject);
				  }).ScheduleGoTo(3f, idle);
			}

			private float GetNewInterval()
			{
				return Random.Range(STINKY_EMIT_INTERVAL_MIN, STINKY_EMIT_INTERVAL_MAX);
			}
		}

		private KBatchedAnimController stinkyController;

		private static readonly HashedString[] WorkLoopAnims = new HashedString[3] { "working_pre", "working_loop", "working_pst" };

		public override void OnSpawn()
		{
			stinkyController = FXHelpers.CreateEffect("odor_fx_kanim", gameObject.transform.GetPosition(), smi.master.gameObject.transform, true);
			base.smi.StartSM();
		}

		private void Emit(object data)
		{
			GameObject gameObject = this.gameObject;

			global::Components.Cmps<MinionIdentity> liveMinionIdentities = global::Components.LiveMinionIdentities;
			Vector2 a = gameObject.transform.GetPosition();
			for (int i = 0; i < liveMinionIdentities.Count; i++)
			{
				MinionIdentity minionIdentity = liveMinionIdentities[i];

				Vector2 b = minionIdentity.transform.GetPosition();
				float num = Vector2.SqrMagnitude(a - b);
				if (num <= 2.25f)
				{
					minionIdentity.Trigger((int)GameHashes.Cringe, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
					minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
				}
			}

			int gameCell = Grid.PosToCell(gameObject.transform.GetPosition());
			float temp = UtilMethods.GetKelvinFromC(10);
			if (gameObject.TryGetComponent<PrimaryElement>(out var ele))
			{
				temp = ele.Temperature;
			}
			else
				SgtLogger.warning("no primary element found");
			SimMessages.AddRemoveSubstance(gameCell, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.0025f, temp, DiseaseInfo.Invalid.idx, DiseaseInfo.Invalid.count);
			KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence"), base.transform.GetPosition());
		}

		private void OnDeath(object data)
		{
			enabled = false;
		}

		private void OnRevived(object data)
		{
			enabled = true;
		}
	}
}
