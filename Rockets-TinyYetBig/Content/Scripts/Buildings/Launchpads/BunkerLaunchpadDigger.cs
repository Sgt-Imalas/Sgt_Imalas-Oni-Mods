using Klei.AI;
using Rockets_TinyYetBig.Content.Defs.Entities;
using System.Collections.Generic;
using UnityEngine;
using static Diggable;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.Launchpads
{

	public class BunkerLaunchpadDigger : StateMachineComponent<BunkerLaunchpadDigger.StatesInstance>
	{
		[MyCmpReq] Operational operational;
		[MyCmpReq] SkipOperationalEnergyConsumer EnergyConsumer;
		[MyCmpReq] LoopingSounds sounds;

		public CellOffset diggableAreaMin = new(-3, 2), diggableAreaMax = new(3, 3);
		GameObject SawbladeVis;
		KBatchedAnimController SawbladeKbac;
		int sawFrameTurnpoint = 28;
		public float EntityDamagePerSecond = 20;
		int width;
		int minCell;

		List<int> DetectionCells;

		public override void OnSpawn()
		{
			//SoundUtils.DumpAllGetSounds();
			InitializeDetectionCells();
			InitializeSawbladeVis();
			smi.StartSM();
		}
		void InitializeDetectionCells()
		{
			DetectionCells = new List<int>();
			int baseCell = this.NaturalBuildingCell();
			width = Mathf.Abs(diggableAreaMax.x - diggableAreaMin.x);
			minCell = Grid.OffsetCell(baseCell, diggableAreaMin);
			for (int x = diggableAreaMin.x; x <= diggableAreaMax.x; x++)
			{
				for (int y = diggableAreaMin.y; y <= diggableAreaMax.y; y++)
				{
					int detectionCell = Grid.OffsetCell(baseCell, new(x, y));
					if (Grid.IsValidCell(detectionCell))
						DetectionCells.Add(detectionCell);
				}
			}
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			if (SawbladeVis != null)
				Destroy(SawbladeVis);
		}
		void InitializeSawbladeVis()
		{
			Vector3 worldPos = transform.position;
			SawbladeVis = GameUtil.KInstantiate(Assets.GetPrefab(BunkerLaunchpadSawbladeConfig.ID), worldPos, Grid.SceneLayer.GasFront);
			SawbladeVis.SetActive(true);
			SawbladeVis.TryGetComponent(out SawbladeKbac);
			SawbladeKbac.onAnimComplete += (_) => smi.Trigger((int)GameHashes.AnimQueueComplete);
			//SawbladeKbac.Play("working_loop", KAnim.PlayMode.Loop);
		}
		private bool CanDigDebris()
		{
			if (!EnergyConsumer.IsPowered)
				return false;

			foreach (var detectionCell in DetectionCells)
			{
				if (Grid.IsSolidCell(detectionCell)
					&& !Grid.Objects[detectionCell, (int)ObjectLayer.FoundationTile]
					&& !Grid.HasDoor[detectionCell]
					&& Grid.Element[detectionCell].hardness < byte.MaxValue)
					return true;
			}

			return false;
		}

		private void PlaySawbladeAnim(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			SawbladeKbac.Play(anim, mode);
		}
		private void QueueSawbladeAnim(string anim, KAnim.PlayMode mode = KAnim.PlayMode.Once)
		{
			SawbladeKbac.Queue(anim, mode);
		}
		private void HandleSounds(bool start = false)
		{
			if (start)
			{
				//sounds.StartSound(GlobalAssets.GetSound("plantmatter_compressor_saw"));
				//sounds.StartSound(GlobalAssets.GetSound("shinebug_wings_lp"));
				sounds.StartSound(GlobalAssets.GetSound("IceCooledFan_fan_LP"));
				//sounds.StartSound(GlobalAssets.GetSound("manualradboltgenerator_geargrindingdn"));
			}
			else
			{
				sounds.StopAllSounds();
			}

		}

		private void SawbladeDig(float dt)
		{
			if (SawbladeKbac.currentAnim != "working_loop")
				return;

			int currentFrame = SawbladeKbac.currentFrame;
			float percentage = currentFrame < sawFrameTurnpoint
				? currentFrame / (float)sawFrameTurnpoint
				: 1f - (currentFrame - (float)sawFrameTurnpoint) / sawFrameTurnpoint;


			int targetCellOffset = Mathf.RoundToInt(percentage * width);
			int targetCell = minCell + targetCellOffset;

			//SgtLogger.l($"Frame: {currentFrame}, percentage: {percentage}, offsetTarget: {targetCellOffset}, targeting: {targetCell}");
			DigForCell(targetCell, dt);
			HandleSounds(true);
		}

		Element targetElement;
		float targetMass;
		float targetHardness;
		float icehardness = -1;
		void TryDigging(int cell, float dtAmount)
		{
			if (!Grid.IsValidCell(cell) || !Grid.IsSolidCell(cell) || Grid.Foundation[cell])
				return;

			targetElement = Grid.Element[cell];
			targetHardness = targetElement.hardness;
			if (targetHardness == byte.MaxValue)
				return;
			targetMass = Grid.Mass[cell];
			if (icehardness == -1)
				icehardness = ElementLoader.FindElementByHash(SimHashes.Ice).hardness;

			float hardnessMultiplier = targetHardness / icehardness;
			float minDigAmount = 4f * (Mathf.Min(targetMass, 400f) / 400f);
			float approxDigTime = minDigAmount + hardnessMultiplier * minDigAmount;

			WorldDamage.Instance.ApplyDamage(cell, dtAmount / approxDigTime, -1, WorldDamage.DamageType.NoBuildingDamage);

		}

		void DigForCell(int cell, float dt)
		{
			List<int> directAdjacentCells =
				[
				Grid.CellLeft(cell),
				Grid.CellRight(cell),
				Grid.CellUpLeft(cell),
				Grid.CellUpRight(cell),
				];

			float digAmount = dt * 2;
			var above = Grid.CellAbove(cell);

			if (Grid.IsValidCell(above))
			{
				TryDigging(above, digAmount);
				DamageEntitiesInCell(above, EntityDamagePerSecond * dt);
			}
			TryDigging(cell, digAmount);
			DamageEntitiesInCell(cell, EntityDamagePerSecond * dt);
			float adjacentDamage = 0.5f * EntityDamagePerSecond * dt;
			foreach (var adj in directAdjacentCells)
			{
				if (!Grid.IsValidCell(adj))
					continue;

				TryDigging(cell, 0.5f * digAmount);
				DamageEntitiesInCell(cell, adjacentDamage);
			}
		}
		void DamageEntitiesInCell(int cell, float amount)
		{
			ListPool<ScenePartitionerEntry, BunkerLaunchpadDigger>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, BunkerLaunchpadDigger>.Allocate();
			var pos = Grid.CellToXY(cell);
			GameScenePartitioner.Instance.VisitEntries(pos.x, pos.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, (obj, smi) => DamageCrittersAndDupes(obj, smi, amount), this);
		}

		private static Util.IterationInstruction DamageCrittersAndDupes(object obj, BunkerLaunchpadDigger smi, float amount)
		{
			if (obj is not Pickupable pickupable || !pickupable.TryGetComponent<Health>(out var health) || health.IsDefeated())
				return Util.IterationInstruction.Continue;
			health.Damage(amount);
			return Util.IterationInstruction.Continue;
		}

		public class StatesInstance : GameStateMachine<States, StatesInstance, BunkerLaunchpadDigger, object>.GameInstance
		{
			public StatesInstance(BunkerLaunchpadDigger master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<States, StatesInstance, BunkerLaunchpadDigger>
		{
			State waitingForDebris;
			DebrisDetected debrisDetected;
			public class DebrisDetected : State
			{
				public State ClearingPre;
				public State ClearingLoop;
				public State ClearingLoop_AnimPost;
				public State ClearingPost;
			}

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = waitingForDebris;
				waitingForDebris
					.UpdateTransition(debrisDetected, (smi, dt) => smi.master.CanDigDebris(), UpdateRate.SIM_1000ms);
				debrisDetected
					.DefaultState(debrisDetected.ClearingPre)
					.Enter(smi => smi.master.PlaySawbladeAnim("working_pre"));
				debrisDetected.ClearingPre
					.OnAnimQueueComplete(debrisDetected.ClearingLoop);
				debrisDetected.ClearingLoop
					.Enter(smi => smi.master.operational.SetActive(true))
					.EventHandler(GameHashes.AnimQueueComplete, smi => smi.master.PlaySawbladeAnim("working_loop"))
					.Update((smi, dt) => smi.master.SawbladeDig(dt), UpdateRate.SIM_33ms)
					.UpdateTransition(debrisDetected.ClearingPost, (smi, dt) => !smi.master.CanDigDebris());
				debrisDetected.ClearingPost
					.Update((smi, dt) => smi.master.SawbladeDig(dt), UpdateRate.SIM_33ms)
					.Enter(smi => smi.master.QueueSawbladeAnim("working_pst"))
					.Exit(smi => smi.master.operational.SetActive(false))
					.Exit(smi => smi.master.HandleSounds(false))
					.OnAnimQueueComplete(waitingForDebris);
			}
		}
	}
}
