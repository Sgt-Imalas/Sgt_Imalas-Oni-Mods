using FMOD;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HPA_ConduitRequirement : KMonoBehaviour, ISim200ms
	{
		private static readonly Operational.Flag highPressureInputConnected = new Operational.Flag("highPressureInputConnected", Operational.Flag.Type.Requirement);
		private static readonly Operational.Flag highPressureOutputConnected = new Operational.Flag("highPressureOutputConnected", Operational.Flag.Type.Requirement);


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

		public bool RequiresHighPressureInput = false, RequiresHighPressureOutput = false;

		private int consumerCell = -1, dispenserCell = -1;
		private ConduitType inputType = ConduitType.None, outputType = ConduitType.None;
		private bool previouslyConnectedHPInput = true, previouslyConnectedHPOutput = true;

		public override void OnSpawn()
		{
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

		bool HasPressureConduitAt(int cell, ConduitType type)
		{
			bool hasHPA = HighPressureConduit.HasHighPressureConduitAt(cell, type);
			return hasHPA;
		}
		
		public void CheckRequirements()
		{
			if (!RequiresHighPressureInput && !RequiresHighPressureOutput)
				return;

			if (RequiresHighPressureInput && consumerCell > 0)
			{
				bool hasHighPressureInput = !consumer.enabled || consumer.IsConnected && HasPressureConduitAt(consumerCell, inputType);

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
						case ConduitType.Solid:
							status_item = StatusItemsDatabase.HPA_NeedSolidIn;
							break;
					}
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureInput);
					this.operational.SetFlag(highPressureInputConnected, hasHighPressureInput);
				}
			}
			if (RequiresHighPressureOutput && dispenserCell > 0)
			{
				bool hasHighPressureOutput = !dispenser.enabled || dispenser.IsConnected && HasPressureConduitAt(dispenserCell, outputType);

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
						case ConduitType.Solid:
							status_item = StatusItemsDatabase.HPA_NeedSolidOut;
							break;
					}
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureOutput);
					this.operational.SetFlag(highPressureOutputConnected, hasHighPressureOutput);
				}
			}
		}
	}
}
