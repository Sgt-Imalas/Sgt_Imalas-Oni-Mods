//// GreedyGreen
//using System.Collections.Generic;
//using UnityEngine;

//public class GreedyGreen : StateMachineComponent<GreedyGreen.StatesInstance>
//{
//	public class StatesInstance : GameStateMachine<States, StatesInstance, GreedyGreen>.GameInstance
//	{
//		public StatesInstance(GreedyGreen smi)
//			: base(smi)
//		{
//		}
//	}

//	public class States : GameStateMachine<States, StatesInstance, GreedyGreen>
//	{
//		public class HarvestableState : State
//		{
//			public State idle;

//			public State grow;

//			public State harvest;

//			public State death;
//		}

//		public HarvestableState harvestable;

//		public override void InitializeStates(out BaseState default_state)
//		{
//			default_state = harvestable;
//			harvestable.EventTransition(GameHashes.Harvest, harvestable.harvest);
//			harvestable.Enter(delegate(StatesInstance smi)
//			{
//				int num = Grid.PosToCell(smi.master);
//				if (smi.master.PlantableCell(num))
//				{
//					int rootCell = num;
//					smi.master.rootCell = rootCell;
//					smi.master.partitionerEntry = GameScenePartitioner.Instance.Add("GreedyGreens.Harvestable", smi.gameObject, num, GameScenePartitioner.Instance.solidChangedMask.mask, smi.master.OnDugOut);
//				}
//				else if (Grid.Solid[Grid.CellBelow(num)])
//				{
//					smi.master.rootCell = Grid.CellBelow(num);
//					smi.transform.SetPosition(smi.transform.position + Vector3.down);
//					smi.master.partitionerEntry = GameScenePartitioner.Instance.Add("GreedyGreens.Harvestable", smi.gameObject, Grid.PosToCell(smi.gameObject), GameScenePartitioner.Instance.solidChangedMask.mask, smi.master.OnDugOut);
//				}
//				else
//				{
//					Util.KDestroyGameObject(smi.master.gameObject);
//				}
//				smi.master.SetCollider();
//				smi.GoTo(harvestable.idle);
//			});
//			harvestable.idle.Enter(delegate(StatesInstance smi)
//			{
//				smi.master.MarkTiles();
//				smi.master.SetCollider();
//				smi.master.consumer.sampleCellOffset = smi.master.GrowDirection * smi.master.growthState.Maturity;
//				smi.master.consumer.enabled = true;
//				smi.master.growthState.GrowingEnabled = true;
//			}).EventTransition(GameHashes.GrowthStateMature, harvestable.grow);
//			harvestable.harvest.Enter(delegate(StatesInstance smi)
//			{
//				smi.master.Emit();
//				smi.master.harvestable.ForceCancelHarvest();
//				smi.GoTo(harvestable.death);
//			});
//			harvestable.grow.Enter(delegate(StatesInstance smi)
//			{
//				smi.master.Spread(Grid.PosToCell(smi.transform.position));
//				smi.master.Mature();
//				smi.ScheduleGoTo(1f, harvestable.idle);
//			});
//			harvestable.death.Enter(delegate(StatesInstance smi)
//			{
//				smi.master.growthState.GrowingEnabled = false;
//				smi.master.plantRenderer.SetDead(is_dead: true);
//				smi.Schedule(2f, delegate
//				{
//					smi.master.CleanUp();
//				});
//			});
//		}
//	}

//	//[MyCmpAdd]
//	//private Plant plantRenderer;

//	[MyCmpAdd]
//	private Harvestable harvestable;

//	[MyCmpAdd]
//	private ElementConsumer consumer;

//	[MyCmpAdd]
//	private ElementEmitter emitter;

//	//[MyCmpAdd]
//	//private GrowthState growthState;

//	//private GameScenePartitionerEntry partitionerEntry;

//	private int[] markedTiles;

//	public int rootCell;

//	private Vector3 GrowDirection = Vector3.up;

//	private List<int> OccupiedCells = new List<int>();

//	protected override void OnPrefabInit()
//	{
//		base.OnPrefabInit();
//		for (int i = 0; i < 7; i++)
//		{
//			if (Random.Range(0, 100) > 50)
//			{
//				growthState.ForceMaturity(growthState.Maturity + 1);
//				Mature();
//			}
//		}
//	}

//	protected override void OnSpawn()
//	{
//		base.OnSpawn();
//		base.smi.StartSM();
//	}

//	private void OnDugOut(object param)
//	{
//		if (!Grid.Solid[Grid.PosToCell(transform.position)])
//		{
//			Emit();
//			base.smi.GoTo(base.smi.sm.harvestable.death);
//		}
//	}

//	private void Emit()
//	{
//		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, "Stored CO2 Released", base.gameObject.transform);
//		float mass = Mathf.Max(1f, base.smi.master.consumer.consumedMass);
//		base.smi.master.consumer.consumedMass = 0f;
//		emitter.ForceEmit(mass, (byte)255,0);
//	}

//	private void Mature()
//	{
//		if (!CellIsClear(nextGrowCell()))
//		{
//			growthState.Regress(1);
//		}
//		plantRenderer.SetStage(growthState.Maturity);
//		SetMaturityDisplayed();
//	}

//	private int TopOfVineCell()
//	{
//		Vector3 pos = transform.position + growthState.Maturity * GrowDirection;
//		return Grid.PosToCell(pos);
//	}

//	private int nextGrowCell()
//	{
//		Vector3 pos = transform.position + (growthState.Maturity + 1) * GrowDirection;
//		return Grid.PosToCell(pos);
//	}

//	private void EnableGrow()
//	{
//		growthState.GrowingEnabled = true;
//	}

//	private void DisableGrow()
//	{
//		growthState.GrowingEnabled = false;
//	}

//	private void CleanUp()
//	{
//		Debug.Log("Clean up");
//		if (partitionerEntry != null)
//		{
//			partitionerEntry.Release();
//		}
//		growthState.Regress(growthState.Maturity);
//		Util.KDestroyGameObject(base.gameObject);
//	}

//	private void SetMaturityDisplayed()
//	{
//		plantRenderer.SetStage(growthState.Maturity);
//		consumer.sampleCellOffset = Vector3.up * growthState.Maturity;
//		MarkTiles();
//		SetCollider();
//	}

//	private void MarkTiles()
//	{
//		for (int i = 0; i < OccupiedCells.Count; i++)
//		{
//			if (Grid.Objects[OccupiedCells[i], 22] == base.gameObject)
//			{
//				Grid.Objects[OccupiedCells[i], 22] = null;
//			}
//			if (Grid.Objects[OccupiedCells[i], 3] == base.gameObject)
//			{
//				Grid.Objects[OccupiedCells[i], 3] = null;
//			}
//		}
//		OccupiedCells.Clear();
//		OccupiedCells.Add(Grid.PosToCell(base.gameObject.transform.position));
//		Grid.Objects[Grid.PosToCell(base.gameObject.transform.position), 22] = base.gameObject;
//		Grid.Objects[Grid.PosToCell(base.gameObject.transform.position), 3] = base.gameObject;
//		for (int j = 1; j < growthState.Maturity; j++)
//		{
//			OccupiedCells.Add(Grid.PosToCell(base.gameObject.transform.position + GrowDirection * j));
//			Grid.Objects[Grid.PosToCell(base.gameObject.transform.position + GrowDirection * j), 22] = base.gameObject;
//			Grid.Objects[Grid.PosToCell(base.gameObject.transform.position + GrowDirection * j), 3] = base.gameObject;
//		}
//	}

//	private void SetCollider()
//	{
//		BoxCollider2D component = GetComponent<BoxCollider2D>();
//		if (growthState.Maturity == 0)
//		{
//			component.size = Vector2.one;
//			component.offset = Vector2.zero;
//		}
//		else
//		{
//			component.size = new Vector2(1f, growthState.Maturity + 1);
//			component.offset = new Vector2(0f, Mathf.Clamp((growthState.Maturity + 1) / 2, 1, growthState.Maturity));
//		}
//	}

//	private void Spread(int startCell)
//	{
//		int cell = Grid.CellLeft(startCell);
//		int cell2 = Grid.CellRight(startCell);
//		int cell3 = Grid.PosToCell(Grid.CellToPos(cell2, 0f, 0f - GrowDirection.y, 0f));
//		int cell4 = Grid.PosToCell(Grid.CellToPos(cell, 0f, 0f - GrowDirection.y, 0f));
//		int cell5 = Grid.PosToCell(Grid.CellToPos(cell2, 0f, GrowDirection.y, 0f));
//		int cell6 = Grid.PosToCell(Grid.CellToPos(cell, 0f, GrowDirection.y, 0f));
//		if (PlantableCell(cell))
//		{
//			GameObject gameObject = Scenario.SpawnPrefab(cell, 0, 0, "GreedyGreen");
//			gameObject.SetActive(value: true);
//		}
//		else if (PlantableCell(cell4))
//		{
//			GameObject gameObject2 = Scenario.SpawnPrefab(cell, 0, (int)(0f - GrowDirection.y), "GreedyGreen");
//			gameObject2.SetActive(value: true);
//		}
//		else if (PlantableCell(cell6))
//		{
//			GameObject gameObject3 = Scenario.SpawnPrefab(cell, 0, (int)GrowDirection.y, "GreedyGreen");
//			gameObject3.SetActive(value: true);
//		}
//		if (PlantableCell(cell2))
//		{
//			GameObject gameObject4 = Scenario.SpawnPrefab(cell2, 0, 0, "GreedyGreen");
//			gameObject4.SetActive(value: true);
//		}
//		else if (PlantableCell(cell3))
//		{
//			GameObject gameObject5 = Scenario.SpawnPrefab(cell2, 0, (int)(0f - GrowDirection.y), "GreedyGreen");
//			gameObject5.SetActive(value: true);
//		}
//		else if (PlantableCell(cell5))
//		{
//			GameObject gameObject6 = Scenario.SpawnPrefab(cell2, 0, (int)GrowDirection.y, "GreedyGreen");
//			gameObject6.SetActive(value: true);
//		}
//	}

//	private bool PlantableCell(int cell)
//	{
//		if (Grid.Solid[cell] && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Objects[cell, 22] == null && Grid.Objects[cell, 1] == null && Grid.Objects[cell, 3] == null)
//		{
//			return true;
//		}
//		return false;
//	}

//	private bool CellIsClear(int cell)
//	{
//		if (!Grid.Solid[cell] && Grid.Objects[cell, 22] == null && Grid.Objects[cell, 1] == null && Grid.Objects[cell, 3] == null)
//		{
//			return true;
//		}
//		return false;
//	}
//}