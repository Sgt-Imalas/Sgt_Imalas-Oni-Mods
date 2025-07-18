using FMOD;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HPA_SolidConduitRequirement : HPA_ConduitRequirement
	{
		[MyCmpGet]
		public SolidConduitConsumer solidconsumer;
		[MyCmpGet]
		public SolidConduitDispenser soliddispenser;
		protected override void CacheConduitCells()
		{
			if (RequiresHighPressureInput && solidconsumer != null)
			{
				consumerCell = solidconsumer.GetInputCell();
				inputType = ConduitType.Solid;
			}
			if (RequiresHighPressureOutput && dispenser != null)
			{
				dispenserCell = soliddispenser.GetOutputCell();
				outputType = ConduitType.Solid;
			}
		}
		protected override bool ConsumerConnected()
		{
			return solidconsumer.IsConnected;
		}
		protected override bool ConsumerDisabled()
		{
			return !solidconsumer.enabled;
		}
		protected override bool DispenserConnected()
		{
			return soliddispenser.IsConnected;
		}
		protected override bool DispenserDisabled()
		{
			return !soliddispenser.enabled;
		}
	}

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

		[SerializeField]
		public bool RequiresHighPressureInput = false, RequiresHighPressureOutput = false;

		protected int consumerCell = -1, dispenserCell = -1;
		protected ConduitType inputType = ConduitType.None, outputType = ConduitType.None;
		protected bool previouslyConnectedHPInput = true, previouslyConnectedHPOutput = true;

		public override void OnSpawn()
		{
			base.OnSpawn();
			CacheConduitCells();
			CheckRequirements(true);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();	
		}
		protected virtual void CacheConduitCells()
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
		protected virtual bool ConsumerConnected()
		{
			return consumer.IsConnected;
		}
		protected virtual bool ConsumerDisabled()
		{
			return !consumer.enabled;
		}
		protected virtual bool DispenserConnected()
		{
			return dispenser.IsConnected;
		}
		protected virtual bool DispenserDisabled()
		{
			return !dispenser.enabled;
		}

		public void CheckRequirements(bool force = false)
		{
			if (!RequiresHighPressureInput && !RequiresHighPressureOutput)
				return;

			if (RequiresHighPressureInput && consumerCell > 0)
			{
				bool hasHighPressureInput = ConsumerDisabled() || ConsumerConnected() && HasPressureConduitAt(consumerCell, inputType);

				if (previouslyConnectedHPInput != hasHighPressureInput || force)
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
				bool hasHighPressureOutput = DispenserDisabled() || DispenserConnected() && HasPressureConduitAt(dispenserCell, outputType);
				if (previouslyConnectedHPOutput != hasHighPressureOutput || force)
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
