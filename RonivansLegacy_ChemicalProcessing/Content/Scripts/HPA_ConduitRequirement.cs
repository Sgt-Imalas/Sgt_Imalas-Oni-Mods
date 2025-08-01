using FMOD;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AccessControlSideScreen.MinionIdentitySort;
using static AmbienceManager;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDING.STATUSITEMS;
using static RoomTracker;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HPA_SolidBridgeRequirement : HPA_SolidConduitRequirement
	{
		public HPA_SolidBridgeRequirement()
		{
			RequiresHighPressureInput = true;
			RequiresHighPressureOutput = true;
		}

		[MyCmpReq]
		public Building building;
		[MyCmpReq]
		public SolidConduitBridge bridge;
		protected override void CacheConduitCells()
		{
			base.CacheConduitCells();
			if (bridge != null)
			{
				inputType = building.Def.InputConduitType;
				outputType = building.Def.OutputConduitType;

				dispenserCell = building.GetUtilityOutputCell();
				consumerCell = building.GetUtilityInputCell();
			}
		}
		protected override bool DispenserDisabled()
		{
			return !bridge.enabled;
		}
		override protected bool ConsumerDisabled()
		{
			return !bridge.enabled;
		}
	}
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
		[MyCmpGet]
		public ConfigurableSolidConduitDispenser varsoliddispenser;
		[SerializeField]
		public bool IsLogisticRail = false;
		protected override void CacheConduitCells()
		{
			if(varsoliddispenser != null)
			{
				soliddispenser = varsoliddispenser;
				outputType = ConduitType.Solid;
			}

			if (RequiresHighPressureInput && solidconsumer != null)
			{
				consumerCell = solidconsumer.GetInputCell();
				inputType = ConduitType.Solid;
			}
			if (RequiresHighPressureOutput && soliddispenser != null)
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
					return DispenserConnected() && LogisticConduit.HasLogisticConduitAt(dispenserCell);
			}
			return base.ConduitConditionSatisfied(isInput);
		}
	}

	public class HPA_ConduitRequirement : KMonoBehaviour
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

		[SerializeField]
		public bool ProhibitHighPressure = false;

		private HandleVector<int>.Handle partitionerEntryInput;
		private HandleVector<int>.Handle partitionerEntryOutput;


		public int consumerCell = -1, dispenserCell = -1;
		public ConduitType inputType = ConduitType.None, outputType = ConduitType.None;
		public bool previouslyConnectedHPInput = true, previouslyConnectedHPOutput = true;

		public override void OnSpawn()
		{
			base.OnSpawn();
			CacheConduitCells();
			SetupPartitioners();
			UpdateRequirements(true);
		}
		public override void OnCleanUp()
		{
			ClearPartitioners();
			base.OnCleanUp();
		}

		void SetupPartitioners()
		{
			if (consumerCell > 0)
				this.partitionerEntryInput = GameScenePartitioner.Instance.Add("HPA_ConduitRequirement_Input", gameObject, consumerCell, GetConduitLayer(inputType), (data => StartCoroutine(ScheduledRequirementUpdate())));
			if (dispenserCell > 0)
				this.partitionerEntryOutput = GameScenePartitioner.Instance.Add("HPA_ConduitRequirement_Output", gameObject, dispenserCell, GetConduitLayer(outputType), (data => StartCoroutine(ScheduledRequirementUpdate())));
		}
		void ClearPartitioners()
		{
			if(this.partitionerEntryInput.IsValid() && GameScenePartitioner.Instance != null)
			{
				GameScenePartitioner.Instance.Free(ref this.partitionerEntryInput);
				this.partitionerEntryInput = HandleVector<int>.InvalidHandle;
			}
			if (this.partitionerEntryOutput.IsValid() && GameScenePartitioner.Instance != null)
			{
				GameScenePartitioner.Instance.Free(ref this.partitionerEntryOutput);
				this.partitionerEntryOutput = HandleVector<int>.InvalidHandle;
			}
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
					return ProhibitHighPressure ? StatusItemsDatabase.HPA_ProhibitGas : isInput ? StatusItemsDatabase.HPA_NeedGasIn : StatusItemsDatabase.HPA_NeedGasOut;
				case ConduitType.Liquid:
					return ProhibitHighPressure ? StatusItemsDatabase.HPA_ProhibitLiquid : isInput ? StatusItemsDatabase.HPA_NeedLiquidIn : StatusItemsDatabase.HPA_NeedLiquidOut;
				case ConduitType.Solid:
					return ProhibitHighPressure ? StatusItemsDatabase.HPA_ProhibitSolid : isInput ? StatusItemsDatabase.HPA_NeedSolidIn : StatusItemsDatabase.HPA_NeedSolidOut;
				default:
					return null;
			}
		}
		protected ScenePartitionerLayer GetConduitLayer(ConduitType type)
		{
			return type switch
			{
				ConduitType.Gas => GameScenePartitioner.Instance.gasConduitsLayer,
				ConduitType.Liquid => GameScenePartitioner.Instance.liquidChangedLayer,
				ConduitType.Solid => GameScenePartitioner.Instance.solidConduitsLayer,
				_ => throw new NotSupportedException($"Unsupported conduit type: {type}"),
			};
		}

		protected virtual bool ConduitConditionSatisfied(bool isInput)
		{
			if (isInput)
				return ConsumerDisabled() || ConsumerConnected() && HasPressureConduitAt(consumerCell, inputType);
			else
				return DispenserDisabled() || DispenserConnected() && HasPressureConduitAt(dispenserCell, outputType);
		}

		/// <summary>
		/// needs to happen a frame delayed, otherwise the conduit isnt registered as high pressure yet
		/// </summary>
		/// <returns></returns>
		IEnumerator ScheduledRequirementUpdate()
		{
			yield return new WaitForSeconds(0.1f);
			UpdateRequirements();
		}

		public void UpdateRequirements(bool force = false)
		{
			if (!RequiresHighPressureInput && !RequiresHighPressureOutput)
				return;

			if (RequiresHighPressureInput && consumerCell > 0)
			{
				bool hasHighPressureInput = ConduitConditionSatisfied(true);
				if(ProhibitHighPressure)
					hasHighPressureInput = !hasHighPressureInput;
				if (previouslyConnectedHPInput != hasHighPressureInput || force)
				{
					previouslyConnectedHPInput = hasHighPressureInput;
					StatusItem status_item = GetConduitStatusItem(inputType, true);
					this.selectable.ToggleStatusItem(status_item, !hasHighPressureInput);
					this.operational.SetFlag(highPressureInputConnected, hasHighPressureInput);
				}
			}
			if (RequiresHighPressureOutput && dispenserCell > 0)
			{
				bool hasHighPressureOutput = ConduitConditionSatisfied(false);

				if (ProhibitHighPressure)
					hasHighPressureOutput = !hasHighPressureOutput;

				if (previouslyConnectedHPOutput != hasHighPressureOutput || force)
				{
					previouslyConnectedHPOutput = hasHighPressureOutput;
					StatusItem status_item = GetConduitStatusItem(outputType, false);
					this.selectable.ToggleStatusItem(status_item, !hasHighPressureOutput);
					this.operational.SetFlag(highPressureOutputConnected, hasHighPressureOutput);
				}
			}
		}
	}
}
