using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlowUtilityNetwork;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HighPressureOutput : HighPressureConduitComponent
	{
		public HighPressureOutput()
		{
			RequiresHighPressureInput = false;
			RequiresHighPressureOutput = true;
		}
	}
	public class HighPressureInput : HighPressureConduitComponent
	{
		public HighPressureInput()
		{
			RequiresHighPressureInput = true;
			RequiresHighPressureOutput = false;
		}
	}
	public class HighPressureConduit : HighPressureConduitComponent
	{
		public HighPressureConduit()
		{
			RequiresHighPressureInput = false;
			RequiresHighPressureOutput = false;
		}
	}

	public class HighPressureConduitComponent : KMonoBehaviour, ISim200ms
	{
		public static Dictionary<int, Dictionary<int, HighPressureConduitComponent>> ConduitsByLayer = new()
		{
			{ (int)ObjectLayer.GasConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.GasConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.Building,new Dictionary<int, HighPressureConduitComponent>() },

		};
		public static void FlushDictionary()
		{
			AllConduits.Clear();
			ConduitsByLayer = new()
			{
			{ (int)ObjectLayer.GasConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.GasConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.Building,new Dictionary<int, HighPressureConduitComponent>() },
			};
		}

		public static HashSet<HighPressureConduitComponent> AllConduits = [];

		private static readonly Operational.Flag highPressureInputConnected = new Operational.Flag("highPressureInputConnected", Operational.Flag.Type.Requirement);
		private static readonly Operational.Flag highPressureOutputConnected = new Operational.Flag("highPressureOutputConnected", Operational.Flag.Type.Requirement);

		[MyCmpReq]
		KPrefabID prefab;
		[MyCmpReq]
		KSelectable selectable;
		[MyCmpGet]
		public BuildingComplete buildingComplete;
		[MyCmpGet]
		public Operational operational;
		[MyCmpGet]
		public ConduitConsumer consumer;
		[MyCmpGet]
		public ConduitDispenser dispenser;

		public bool RequiresHighPressureInput = true, RequiresHighPressureOutput = true;
		private int consumerCell = -1, dispenserCell = -1;
		private ConduitType inputType = ConduitType.None, outputType = ConduitType.None;
		private bool previouslyConnectedHPInput = true, previouslyConnectedOutput = true;

		public override void OnSpawn()
		{
			if (!ConduitsByLayer.ContainsKey((int)buildingComplete.Def.ObjectLayer))
				ConduitsByLayer.Add((int)buildingComplete.Def.ObjectLayer, new());


			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Min()] = this;
			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Max()] = this;

			AllConduits.Add(this);
			CacheConduitCells();
			base.OnSpawn();
			CheckRequirements(true);
		}

		void CacheConduitCells()
		{
			if (RequiresHighPressureInput && consumer != null)
			{
				consumerCell = consumer.GetInputCell(consumer.conduitType);
				inputType = consumer.conduitType;
			}
			if(RequiresHighPressureOutput && dispenser != null)
			{
				dispenserCell = dispenser.GetOutputCell(dispenser.conduitType);
				outputType = dispenser.conduitType;
			}
		}

		private void CheckRequirements(bool forceEvent)
		{
			if (!RequiresHighPressureInput && !RequiresHighPressureOutput)
				return;

			if (RequiresHighPressureInput && consumerCell > 0)
			{
				bool noHPInput = !consumer.enabled || !consumer.IsConnected || !HasHighPressureConduitAt(consumerCell, inputType);

				if (previouslyConnectedHPInput != noHPInput)
				{
					previouslyConnectedHPInput = noHPInput;

				}
			}


			if (this.requireConduit)
			{
				bool flag3 = !this.conduitConsumer.enabled || this.conduitConsumer.IsConnected;
				bool flag4 = !this.conduitConsumer.enabled || this.conduitConsumer.IsSatisfied;
				if (this.VisualizeRequirement(RequireInputs.Requirements.ConduitConnected) && this.previouslyConnected != flag3)
				{
					this.previouslyConnected = flag3;
					StatusItem status_item = (StatusItem)null;
					switch (this.conduitConsumer.TypeOfConduit)
					{
						case ConduitType.Gas:
							status_item = Db.Get().BuildingStatusItems.NeedGasIn;
							break;
						case ConduitType.Liquid:
							status_item = Db.Get().BuildingStatusItems.NeedLiquidIn;
							break;
					}
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !flag3, (object)new Tuple<ConduitType, Tag>(this.conduitConsumer.TypeOfConduit, this.conduitConsumer.capacityTag));
					this.operational.SetFlag(RequireInputs.inputConnectedFlag, flag3);
				}
				flag1 &= flag3;
				if (this.VisualizeRequirement(RequireInputs.Requirements.ConduitEmpty) && this.previouslySatisfied != flag4)
				{
					this.previouslySatisfied = flag4;
					StatusItem status_item = (StatusItem)null;
					switch (this.conduitConsumer.TypeOfConduit)
					{
						case ConduitType.Gas:
							status_item = Db.Get().BuildingStatusItems.GasPipeEmpty;
							break;
						case ConduitType.Liquid:
							status_item = Db.Get().BuildingStatusItems.LiquidPipeEmpty;
							break;
					}
					if (this.requireConduitHasMass)
					{
						if (status_item != null)
							this.selectable.ToggleStatusItem(status_item, !flag4, (object)this);
						this.operational.SetFlag(RequireInputs.pipesHaveMass, flag4);
					}
				}
			}
			this.requirementsMet = flag1;
		}
		static bool HasHighPressureConduitAt(int cell, ConduitType type)
		{
			if (type == ConduitType.Gas)
			{
				return ConduitsByLayer[(int)ObjectLayer.GasConduit].ContainsKey(cell);
			}
			else if (type == ConduitType.Liquid)
			{
				return ConduitsByLayer[(int)ObjectLayer.LiquidConduit].ContainsKey(cell);
			}
			return false;
		}
	}
}
