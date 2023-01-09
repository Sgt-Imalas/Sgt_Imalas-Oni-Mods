//// MysteryEgg
//using KSerialization;
//using System.Collections.Generic;
//using UnityEngine;

//[SerializationConfig(MemberSerialization.OptIn)]
//public class MysteryEgg : StateMachineComponent<MysteryEgg.StatesInstance>, ISaveLoadable
//{
//	public class StatesInstance : GameStateMachine<States, StatesInstance, MysteryEgg>.GameInstance
//	{
//		public StatesInstance(MysteryEgg smi)
//			: base(smi)
//		{
//		}
//	}

//	public class States : GameStateMachine<States, StatesInstance, MysteryEgg>
//	{
//		public class GroundedState : State
//		{
//			public State idle;
//		}

//		public class IncubatingState : State
//		{
//			public State idle;

//			public State idle_alt;

//			public State wiggle_small;

//			public State wiggle_large;

//			public State wiggle_hatch;
//		}

//		public GroundedState grounded;

//		public IncubatingState incubating;

//		public State hatch_pst;

//		public State dead;

//		public State fall;

//		public override void InitializeStates(out BaseState default_state)
//		{
//			default_state = grounded.idle;
//			base.serializable = true;
//			grounded.EventTransition(GameHashes.TooHotFatal, dead, (StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f).EventTransition(GameHashes.TooColdFatal, dead, (StatesInstance smi) => smi.master.alive && smi.timeinstate > 0f).EventTransition(GameHashes.OnStore, incubating.idle, (StatesInstance smi) => smi.master.transform.parent.GetComponent<EggIncubator>() != null)
//				.Update(delegate(StatesInstance smi)
//				{
//					int num2 = Grid.PosToCell(smi.transform.position + Vector3.down);
//					if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
//					{
//						smi.GoTo(fall);
//					}
//				});
//			grounded.idle.PlayAnim("idle").Enter(delegate(StatesInstance smi)
//			{
//				int num = Grid.PosToCell(smi.transform.position + Vector3.down);
//				if (Grid.IsValidCell(num) && !Grid.Solid[num])
//				{
//					smi.GoTo(fall);
//				}
//			});
//			incubating.EventTransition(GameHashes.OnStorageChange, grounded.idle, (StatesInstance smi) => smi.master.transform.parent == null || smi.master.transform.parent.GetComponent<EggIncubator>() == null);
//			incubating.idle_alt.Enter(delegate(StatesInstance smi)
//			{
//				smi.GoTo(grounded.idle);
//			});
//			incubating.wiggle_small.PlayAnim("wiggle_small").OnAnimQueueComplete(grounded.idle);
//			incubating.wiggle_large.PlayAnim("wiggle_large").OnAnimQueueComplete(grounded.idle);
//			incubating.wiggle_hatch.PlayAnim("hatch").OnAnimQueueComplete(hatch_pst);
//			incubating.idle.PlayAnim("idle").Enter(delegate(StatesInstance smi)
//			{
//				smi.master.incubator = smi.master.transform.parent.GetComponent<EggIncubator>();
//				if (smi.master.maturity > 1200f)
//				{
//					smi.ScheduleGoTo(3f, incubating.wiggle_hatch);
//				}
//				else if (smi.master.maturity > 1020f)
//				{
//					smi.ScheduleGoTo(3f, incubating.wiggle_large);
//				}
//				else if (smi.master.maturity > 150f)
//				{
//					smi.ScheduleGoTo(3f, incubating.wiggle_small);
//				}
//				else
//				{
//					smi.ScheduleGoTo(3f, incubating.idle_alt);
//				}
//			}).Update(delegate(StatesInstance smi)
//			{
//				if (smi.master.incubator != null && smi.master.incubator.operational.IsOperational)
//				{
//					smi.master.maturity += smi.deltatime * smi.master.matureRate;
//				}
//			});
//			hatch_pst.PlayAnim("hatch_pst").Enter(delegate(StatesInstance smi)
//			{
//				smi.master.HatchCreature();
//				smi.Schedule(2f, delegate
//				{
//					Util.KDestroyGameObject(smi.gameObject);
//				});
//			});
//			dead.PlayAnim("dead").Enter(delegate(StatesInstance smi)
//			{
//				smi.Schedule(5f, delegate
//				{
//					Util.KDestroyGameObject(smi.gameObject);
//				});
//			});
//			fall.ToggleGravity(grounded.idle).PlayAnim("idle", KAnim.PlayMode.Loop);
//		}
//	}

//	[MyCmpAdd]
//	private KBatchedAnimController anim;

//	[MyCmpAdd]
//	private CircleCollider2D mCollider;

//	public bool alive = true;

//	private float maturity;

//	private float matureRate = 1f;

//	private EggIncubator incubator;

//	[Serialize]
//	private bool initialized;

//	private Dictionary<string, int> HatchPossibilities = new Dictionary<string, int>();

//	protected override void OnSpawn()
//	{
//		HatchPossibilities.Add("Hatch", 2);
//		HatchPossibilities.Add("Glom", 1);
//		HatchPossibilities.Add("Puft", 4);
//		base.OnSpawn();
//		GetComponent<KPrefabID>().AddTag(GameTags.Egg);
//		if (!initialized)
//		{
//			HandleVector<System.Action>.Handle handle = Game.Instance.callbackManager.Add(delegate
//			{
//				base.smi.StartSM();
//				base.smi.master.initialized = true;
//			});
//			SimMessages.ReplaceElement(Grid.PosToCell(base.gameObject), SimHashes.Dirt, CellEventLogger.Instance.ObjectSetSimOnSpawn, Random.Range(1000f, 3000f), -1f, handle.index);
//			handle.index = -1;
//		}
//		else
//		{
//			base.smi.StartSM();
//		}
//	}

//	private void HatchCreature()
//	{
//		int num = 0;
//		int num2 = 0;
//		foreach (KeyValuePair<string, int> hatchPossibility in HatchPossibilities)
//		{
//			num += hatchPossibility.Value;
//			num2++;
//		}
//		float num3 = Random.Range(0, num);
//		string name = string.Empty;
//		float num4 = 0f;
//		foreach (KeyValuePair<string, int> hatchPossibility2 in HatchPossibilities)
//		{
//			if (num4 + (float)hatchPossibility2.Value >= num3)
//			{
//				name = hatchPossibility2.Key;
//				break;
//			}
//			num4 += (float)hatchPossibility2.Value;
//		}
//		int rootCell = Grid.PosToCell(transform.position);
//		GameObject gameObject = Scenario.SpawnPrefab(rootCell, 0, 1, name);
//		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, gameObject.GetProperName(), gameObject.transform);
//		gameObject.SetActive(value: true);
//		EggIncubator component = transform.parent.GetComponent<EggIncubator>();
//		if ((bool)component)
//		{
//			component.RemoveHatchedEgg(base.gameObject);
//		}
//	}
//}