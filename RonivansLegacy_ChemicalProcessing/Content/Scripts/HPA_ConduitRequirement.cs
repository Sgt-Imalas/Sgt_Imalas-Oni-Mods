using FMOD;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AccessControlSideScreen.MinionIdentitySort;
using static RoomTracker;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HPA_SolidFilterRequirement : HPA_SolidConduitRequirement
	{
		public HPA_SolidFilterRequirement()
		{
			RequiresHighPressureInput = true;
			RequiresHighPressureOutput = true;
		}

		[MyCmpReq]
		public Building building;
		[MyCmpReq]
		public ElementFilter filter;
		protected override void CacheConduitCells()
		{
			base.CacheConduitCells();
			if (filter != null)
			{
				inputType = building.Def.InputConduitType;
				outputType = building.Def.OutputConduitType;

				dispenserCell = building.GetUtilityOutputCell();
				consumerCell = building.GetUtilityInputCell();
			}
		}
		protected override bool DispenserDisabled()
		{
			return !filter.enabled;
		}
		override protected bool ConsumerDisabled()
		{
			return !filter.enabled;
		}
	}

	public class HPA_SolidConduitRequirement : HPA_ConduitRequirement
	{
		[MyCmpGet]
		public SolidConduitConsumer solidconsumer;
		[MyCmpGet]
		public SolidConduitDispenser soliddispenser;
		[SerializeField]
		public bool IsLogisticRail = false;
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
			base.CacheConduitCells();
		}
		protected override bool ConsumerDisabled()
		{
			return !(solidconsumer?.enabled ?? false);
		}
		protected override bool DispenserDisabled()
		{
			return !(soliddispenser?.enabled ?? false);
		}
		protected override StatusItem GetConduitStatusItem(ConduitType type, bool isInput)
		{
			if (isInput)
			{
				if (IsLogisticRail)
					return StatusItemsDatabase.LOGISTIC_NeedSolidIn;
				else
					return StatusItemsDatabase.HPA_NeedSolidIn;
			}
			else
			{
				if (IsLogisticRail)
					return StatusItemsDatabase.LOGISTIC_NeedSolidOut;
				else
					return StatusItemsDatabase.HPA_NeedSolidOut;
			}
		}
		protected override bool ConduitConditionSatisfied(bool isInput)
		{
			if (IsLogisticRail)
			{
				if (isInput)
					return ConsumerConnected() && LogisticConduit.HasLogisticConduitAt(consumerCell);
				else
					return ConsumerConnected() && LogisticConduit.HasLogisticConduitAt(dispenserCell);
			}
			return base.ConduitConditionSatisfied(isInput);
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
			bool hasHPA = HighPressureConduitRegistration.HasHighPressureConduitAt(cell, type);
			return hasHPA;
		}
		protected virtual bool ConsumerConnected() => RequireOutputs.IsConnected(consumerCell, inputType);

		protected virtual bool ConsumerDisabled()
		{
			return !(consumer?.enabled ?? false);
		}
		protected virtual bool DispenserConnected() => RequireOutputs.IsConnected(dispenserCell, outputType);

		protected virtual bool DispenserDisabled()
		{
			return !(dispenser?.enabled ?? false);
		}
		protected virtual StatusItem GetConduitStatusItem(ConduitType type, bool isInput)
		{
			switch (type)
			{
				case ConduitType.Gas:
					return isInput ? StatusItemsDatabase.HPA_NeedGasIn : StatusItemsDatabase.HPA_NeedGasOut;
				case ConduitType.Liquid:
					return isInput ? StatusItemsDatabase.HPA_NeedLiquidIn : StatusItemsDatabase.HPA_NeedLiquidOut;
				case ConduitType.Solid:
					return isInput ? StatusItemsDatabase.HPA_NeedSolidIn : StatusItemsDatabase.HPA_NeedSolidOut;
				default:
					return null;
			}
		}

		protected virtual bool ConduitConditionSatisfied(bool isInput)
		{
			if (isInput)
				return ConsumerDisabled() || ConsumerConnected() && HasPressureConduitAt(consumerCell, inputType);
			else
				return DispenserDisabled() || DispenserConnected() && HasPressureConduitAt(dispenserCell, outputType);
		}

		public void CheckRequirements(bool force = false)
		{
			if (!RequiresHighPressureInput && !RequiresHighPressureOutput)
				return;

			if (RequiresHighPressureInput && consumerCell > 0)
			{
				bool hasHighPressureInput = ConduitConditionSatisfied(true);

				if (previouslyConnectedHPInput != hasHighPressureInput || force)
				{
					previouslyConnectedHPInput = hasHighPressureInput;
					StatusItem status_item = GetConduitStatusItem(inputType, true);
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureInput);
					this.operational.SetFlag(highPressureInputConnected, hasHighPressureInput);
				}
			}
			if (RequiresHighPressureOutput && dispenserCell > 0)
			{
				bool hasHighPressureOutput = ConduitConditionSatisfied(false);
				if (previouslyConnectedHPOutput != hasHighPressureOutput || force)
				{
					previouslyConnectedHPOutput = hasHighPressureOutput;
					StatusItem status_item = GetConduitStatusItem(inputType, false);
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureOutput);
					this.operational.SetFlag(highPressureOutputConnected, hasHighPressureOutput);
				}
			}
		}
	}
}
