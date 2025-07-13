using RonivansLegacy_ChemicalProcessing.Content.ModDb;
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
		protected override void CheckRequirements()
		{
			//skip on high pressure conduits
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
		private bool previouslyConnectedHPInput = true, previouslyConnectedHPOutput = true;

		public override void OnSpawn()
		{
			if (!ConduitsByLayer.ContainsKey((int)buildingComplete.Def.ObjectLayer))
				ConduitsByLayer.Add((int)buildingComplete.Def.ObjectLayer, new());


			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Min()] = this;
			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Max()] = this;

			AllConduits.Add(this);
			CacheConduitCells();
			base.OnSpawn();
			CheckRequirements();
		}

		void CacheConduitCells()
		{
			if (RequiresHighPressureInput && consumer != null)
			{
				consumerCell = consumer.GetInputCell(consumer.conduitType);
				inputType = consumer.conduitType;
			}
			if (RequiresHighPressureOutput && dispenser != null)
			{
				dispenserCell = dispenser.GetOutputCell(dispenser.conduitType);
				outputType = dispenser.conduitType;
			}
		}
		public void Sim200ms(float dt) => CheckRequirements();

		protected virtual void CheckRequirements()
		{
			if (!RequiresHighPressureInput && !RequiresHighPressureOutput)
				return;

			if (RequiresHighPressureInput && consumerCell > 0)
			{
				bool hasHighPressureInput = !consumer.enabled || consumer.IsConnected && HasHighPressureConduitAt(consumerCell, inputType);

				if (previouslyConnectedHPInput != hasHighPressureInput)
				{
					previouslyConnectedHPInput = hasHighPressureInput;
					StatusItem status_item = null;
					switch (inputType)
					{
						case ConduitType.Gas:
							status_item = StatusItemsDatabase.HPA_NeedGasIn;
							break;
						case ConduitType.Liquid:
							status_item = StatusItemsDatabase.HPA_NeedLiquidIn;
							break;
					}
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureInput);
					this.operational.SetFlag(highPressureInputConnected, hasHighPressureInput);
				}
			}
			if (RequiresHighPressureOutput && dispenserCell > 0)
			{
				bool hasHighPressureOutput = !dispenser.enabled || dispenser.IsConnected && HasHighPressureConduitAt(dispenserCell, outputType);

				if (previouslyConnectedHPOutput != hasHighPressureOutput)
				{
					previouslyConnectedHPOutput = hasHighPressureOutput;
					StatusItem status_item = null;
					switch (outputType)
					{
						case ConduitType.Gas:
							status_item = StatusItemsDatabase.HPA_NeedGasOut;
							break;
						case ConduitType.Liquid:
							status_item = StatusItemsDatabase.HPA_NeedLiquidOut;
							break;
					}
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureOutput);
					this.operational.SetFlag(highPressureOutputConnected, hasHighPressureOutput);
				}
			}
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
