using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BathTub
{
	[SerializationConfig(MemberSerialization.OptIn)]
	internal class BathTub : StateMachineComponent<BathTub.StatesInstance>, IGameObjectEffectDescriptor
	{
		//public string specificEffect;
		//public string trackingEffect;
		public int basePriority;
		public CellOffset[] choreOffsets = new CellOffset[]
		{
			new CellOffset(-1, 0),
			new CellOffset(1, 0),
			new CellOffset(0, 0),
		};
		private BathTubWorkable[] workables;
		private Chore[] chores;
		public HashSet<int> occupants = new HashSet<int>();
		public float BathTubCapacity = 100f;
		public float BathTubWaterThreshold = 60f;


		[MyCmpGet]
		public Storage waterStorage;
		private MeterController waterMeter;
		private MeterController tempMeter;

		public float PercentFull => 100f * this.waterStorage.GetMassAvailable(SimHashes.Water) / this.BathTubCapacity;
		static StatusItem BathtubFilling;

		public static void RegisterStatusItems()
		{
			BathtubFilling = new StatusItem(
					  "BT_BATHTUBFILLING",
					  "BUILDING",
					  "",
					  StatusItem.IconType.Info,
					  NotificationType.Neutral,
					  false,
					  OverlayModes.None.ID,
					  false
					  );

			BathtubFilling.resolveStringCallback = delegate (string str, object data)
			{
				BathTub hotTub = (BathTub)data;
				str = str.Replace("{fullness}", GameUtil.GetFormattedPercent(hotTub.PercentFull));
				return str;
			};
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule), null, null);
			this.workables = new BathTubWorkable[this.choreOffsets.Length];
			this.chores = new Chore[this.choreOffsets.Length];
			for (int index = 0; index < this.workables.Length; ++index)
			{
				GameObject locator = ChoreHelpers.CreateLocator("BathTubWorkable", Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this), this.choreOffsets[index]), Grid.SceneLayer.Move));
				KSelectable kselectable = locator.AddOrGet<KSelectable>();
				kselectable.SetName(this.GetProperName());
				kselectable.IsSelectable = false;
				BathTubWorkable BathTubWorkable = locator.AddOrGet<BathTubWorkable>();
				int player_index = index;
				BathTubWorkable.OnWorkableEventCB = BathTubWorkable.OnWorkableEventCB + ((workable, ev) => this.OnWorkableEvent(player_index, ev));
				this.workables[index] = BathTubWorkable;
				this.workables[index].bathTub = this;
			}
			this.waterMeter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_water_target", "meter_water", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
			{
				"meter_water_target"
			});

			this.tempMeter = new MeterController(this.GetComponent<KBatchedAnimController>(), "meter_temperature_target", "meter_temp", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
			{
				"meter_temperature_target"
			});
			this.smi.StartSM();
			Subscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
		}

		public void OnStorageChanged(object _) => waterMeter.SetPositionPercent(Mathf.Clamp(waterStorage.GetMassAvailable(SimHashes.Water) / BathTubCapacity, 0.0f, 1f));

		public override void OnCleanUp()
		{
			this.UpdateChores(false);
			for (int index = 0; index < this.workables.Length; ++index)
			{
				if ((bool)this.workables[index])
				{
					Util.KDestroyGameObject(this.workables[index]);
					this.workables[index] = null;
				}
			}
			base.OnCleanUp();
		}

		private Chore CreateChore(int i)
		{
			Workable workable = this.workables[i];
			Workable target = workable;
			Action<Chore> on_end = new Action<Chore>(this.OnSocialChoreEnd);
			ScheduleBlockType schedule_block = Db.Get().ScheduleBlockTypes.Hygiene;
			WorkChore<BathTubWorkable> chore = new WorkChore<BathTubWorkable>(Db.Get().ChoreTypes.Shower, target, on_end: on_end, allow_in_red_alert: false, schedule_block: schedule_block, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
			chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
			return chore;
		}

		private void OnSocialChoreEnd(Chore chore)
		{
			if (!this.gameObject.HasTag(GameTags.Operational))
				return;
			this.UpdateChores();
		}

		public void UpdateChores(bool update = true)
		{
			for (int i = 0; i < this.choreOffsets.Length; ++i)
			{
				Chore chore = this.chores[i];
				if (update)
				{
					if (chore == null || chore.isComplete)
						this.chores[i] = this.CreateChore(i);
				}
				else if (chore != null)
				{
					chore.Cancel("locator invalidated");
					this.chores[i] = null;
				}
			}
		}

		public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
		{
			if (ev == Workable.WorkableEvent.WorkStarted)
				this.occupants.Add(player);
			else
				this.occupants.Remove(player);
			this.smi.sm.userCount.Set(this.occupants.Count, this.smi);
		}

		List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(
		  GameObject go)
		{
			List<Descriptor> descs = new List<Descriptor>();
			Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Water);
			descs.Add(new Descriptor(global::STRINGS.BUILDINGS.PREFABS.HOTTUB.WATER_REQUIREMENT.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.BathTubCapacity)), global::STRINGS.BUILDINGS.PREFABS.HOTTUB.WATER_REQUIREMENT_TOOLTIP.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.BathTubCapacity)), Descriptor.DescriptorType.Requirement));

			//descs.Add(new Descriptor((string)global::STRINGS.UI.BUILDINGEFFECTS.RECREATION, (string)global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION));
			//Effect.AddModifierDescriptions(this.gameObject, descs, this.specificEffect, true);
			return descs;
		}

		public class States : GameStateMachine<States, StatesInstance, BathTub>
		{
			public IntParameter userCount;
			public State unoperational;
			public OffStates off;
			public ReadyStates ready;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = ready;
				this.unoperational
					.TagTransition(GameTags.Operational, off)
					.PlayAnim("off");
				this.off
					.TagTransition(GameTags.Operational, this.unoperational, true)
					.DefaultState(off.filling);
				this.off.filling.DefaultState(this.off.filling.normal)
					.Transition(ready, smi => (double)smi.master.waterStorage.GetMassAvailable(SimHashes.Water) >= smi.master.BathTubWaterThreshold)
					.PlayAnim("off")
					.Enter(smi => smi.GetComponent<ConduitConsumer>().SetOnState(true))
					.Enter(smi => smi.GetComponent<ConduitDispenser>().SetOnState(true))
					.Exit(smi => smi.GetComponent<ConduitConsumer>().SetOnState(true))
					.ToggleMainStatusItem(BathtubFilling, (Func<StatesInstance, object>)(smi => smi.master))
					;
				this.ready
					.DefaultState(this.ready.idle)
					.Enter("CreateChore", smi => smi.master.UpdateChores())
					.Exit("CancelChore", smi => smi.master.UpdateChores(false))
					.TagTransition(GameTags.Operational, this.unoperational, true)
					.Transition(off.filling, smi => smi.master.waterStorage.GetMassAvailable(SimHashes.Water) < smi.master.BathTubWaterThreshold)
					.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal);
				this.ready.idle.PlayAnim("on")
					.ParamTransition(userCount, this.ready.on.pre, (smi, p) => p > 0);
				this.ready.on
					.Enter(smi => smi.SetActive(true))
					.Exit(smi => smi.SetActive(false));
				this.ready.on.pre
					.PlayAnim("working_pre")
					.OnAnimQueueComplete(this.ready.on.relaxing);
				this.ready.on.relaxing
					.PlayAnim("working_loop", KAnim.PlayMode.Loop)
					.ParamTransition(userCount, this.ready.on.post, (smi, p) => p == 0)
					.ParamTransition(userCount, this.ready.on.relaxing_together, (smi, p) => p > 1);
				this.ready.on.relaxing_together
					.PlayAnim("working_loop", KAnim.PlayMode.Loop)
					.ParamTransition(userCount, this.ready.on.post, (smi, p) => p == 0).
					ParamTransition(userCount, this.ready.on.relaxing, (smi, p) => p == 1);
				this.ready.on.post.PlayAnim("working_pst")
					.OnAnimQueueComplete(this.ready.idle);
			}

			private string GetRelaxingAnim(StatesInstance smi)
			{
				bool flag1 = smi.master.occupants.Contains(0);
				bool flag2 = smi.master.occupants.Contains(1);
				if (flag1 && !flag2)
					return "working_loop_one_p";
				return flag2 && !flag1 ? "working_loop_two_p" : "working_loop_coop_p";
			}

			public class OffStates :
			  State
			{
				public State draining;
				public FillingStates filling;
			}

			public class OnStates :
			  State
			{
				public State pre;
				public State relaxing;
				public State relaxing_together;
				public State post;
			}

			public class ReadyStates :
			  State
			{
				public State idle;
				public OnStates on;
			}

			public class FillingStates :
			  State
			{
				public State normal;
			}
		}

		public class StatesInstance :
		  GameStateMachine<States, StatesInstance, BathTub, object>.GameInstance
		{
			private Operational operational;

			public StatesInstance(BathTub smi)
			  : base(smi)
			{
				this.operational = this.master.GetComponent<Operational>();
			}

			public void SetActive(bool active) => this.operational.SetActive(this.operational.IsOperational & active);


		}
	}

}
