using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static AmbienceManager;
using static Operational;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class MultiTagConduitFilter : KMonoBehaviour, ISecondaryOutput
	{
		[MyCmpReq] TreeFilterable treeFilterable;
		[MyCmpReq] Operational operational;
		[MyCmpReq] Building building;
		[MyCmpReq] KSelectable selectable;
		[MyCmpGet] ConduitConsumer conduitConsumer;
		[SerializeField] public ConduitPortInfo FilteredOutputPort = new(ConduitType.Solid, new CellOffset(0, 0));
		[SerializeField] public float ThroughputMax = Config.Instance.HPA_Capacity_Solid;
		int inputCell, outputCell, filteredOutputCell;

		FlowUtilityNetwork.NetworkItem filteredOutput;

		private HandleVector<int>.Handle partitionerEntry;
		private Guid needsConduitStatusItemGuid;
		private Guid conduitBlockedStatusItemGuid;


		public CellOffset GetSecondaryConduitOffset(ConduitType type) => FilteredOutputPort.offset;
		public bool HasSecondaryConduitType(ConduitType type) => FilteredOutputPort.conduitType == type;


		IConduitFlow GetFlowManager()
		{
			return FilteredOutputPort.conduitType switch
			{
				ConduitType.Solid => Game.Instance.solidConduitFlow,
				ConduitType.Liquid => Game.Instance.liquidConduitFlow,
				ConduitType.Gas => Game.Instance.gasConduitFlow,
				_ => throw new NotSupportedException($"Unsupported conduit type: {FilteredOutputPort.conduitType}"),
			};
		}
		IUtilityNetworkMgr GetNetworkMgr()
		{
			return FilteredOutputPort.conduitType switch
			{
				ConduitType.Solid => Game.Instance.solidConduitSystem,
				ConduitType.Liquid => Game.Instance.liquidConduitSystem,
				ConduitType.Gas => Game.Instance.gasConduitSystem,
				_ => throw new NotSupportedException($"Unsupported conduit type: {FilteredOutputPort.conduitType}"),
			};
		}
		ScenePartitionerLayer GetConduitPartitionerLayer()
		{
			return FilteredOutputPort.conduitType switch
			{
				ConduitType.Solid => GameScenePartitioner.Instance.solidConduitsLayer,
				ConduitType.Liquid => GameScenePartitioner.Instance.liquidChangedLayer,
				ConduitType.Gas => GameScenePartitioner.Instance.gasConduitsLayer,
				_ => throw new NotSupportedException($"Unsupported conduit type: {FilteredOutputPort.conduitType}"),
			};
		}


		public bool IsNonSolidConduit => FilteredOutputPort.conduitType == ConduitType.Liquid || FilteredOutputPort.conduitType == ConduitType.Gas;

		public override void OnSpawn()
		{
			base.OnSpawn();
			inputCell = building.GetUtilityInputCell();
			outputCell = building.GetUtilityOutputCell();
			filteredOutputCell = Grid.OffsetCell(building.NaturalBuildingCell(), FilteredOutputPort.offset);

			filteredOutput = new FlowUtilityNetwork.NetworkItem(FilteredOutputPort.conduitType, Endpoint.Source, filteredOutputCell, this.gameObject);
			GetNetworkMgr().AddToNetworks(filteredOutputCell, filteredOutput, true);
			GetFlowManager().AddConduitUpdater(OnConduitTick);

			if (conduitConsumer != null && IsNonSolidConduit)
				conduitConsumer.isConsuming = false;

			partitionerEntry = GameScenePartitioner.Instance.Add("MultiElementFilterConduitExists", (object)this.gameObject, this.filteredOutputCell, GetConduitPartitionerLayer(), (_ => this.UpdateConduitExistsStatus()));

			UpdateConduitBlockedStatus();
			UpdateConduitExistsStatus();
		}
		public override void OnCleanUp()
		{
			GetFlowManager().RemoveConduitUpdater(OnConduitTick);
			GetNetworkMgr().RemoveFromNetworks(filteredOutputCell, filteredOutput, true);
			if (partitionerEntry.IsValid() && GameScenePartitioner.Instance != null)
				GameScenePartitioner.Instance.Free(ref partitionerEntry);
			base.OnCleanUp();
		}

		void OnConduitTick(float dt)
		{
			bool IsActive = false;
			UpdateConduitBlockedStatus();
			if (operational.IsOperational)
			{
				IConduitFlow IflowManager = GetFlowManager();
				var acceptedTags = treeFilterable.AcceptedTags;
				if (IflowManager is ConduitFlow flowManager)
				{
					var inputContents = flowManager.GetContents(inputCell);
					int outputCell = acceptedTags.Contains(inputContents.element.CreateTag()) ? filteredOutputCell : this.outputCell;
					ConduitFlow.ConduitContents contentsAtOutputCell = flowManager.GetContents(outputCell);
					if (inputContents.mass > 0 && contentsAtOutputCell.mass <= 0)
					{
						IsActive = true;
						float delta = flowManager.AddElement(outputCell, inputContents.element, inputContents.mass, inputContents.temperature, inputContents.diseaseIdx, inputContents.diseaseCount);
						if (delta > 0)
						{
							flowManager.RemoveElement(inputCell, delta);
						}
					}
				}
				else if (IflowManager is SolidConduitFlow solidFlowManager)
				{
					var inputContents = solidFlowManager.GetContents(inputCell);
					Pickupable pickupableInput = solidFlowManager.GetPickupable(inputContents.pickupableHandle);
					if(pickupableInput != null)
					{
						int outputCell = pickupableInput.HasAnyTags(acceptedTags.ToArray()) ? filteredOutputCell : this.outputCell;
						SolidConduitFlow.ConduitContents contentsAtOutputCell = solidFlowManager.GetContents(outputCell);
						Pickupable pickupableAtOutputCell = solidFlowManager.GetPickupable(contentsAtOutputCell.pickupableHandle);

						if (pickupableInput.primaryElement.Mass > 0 && (pickupableAtOutputCell == null || pickupableAtOutputCell.primaryElement?.Mass <= 0))
						{
							IsActive = true;
							var transferItem = solidFlowManager.RemovePickupable(inputCell);
							if (transferItem != null)
							{
								solidFlowManager.AddPickupable(inputCell, transferItem);
							}
						}
					}
					else
						solidFlowManager.RemovePickupable(inputCell);

				}
			}
			operational.SetActive(IsActive);
		}
		void UpdateConduitExistsStatus()
		{
			bool conduitIsConnected = RequireOutputs.IsConnected(this.filteredOutputCell, this.FilteredOutputPort.conduitType);
			StatusItem status_item;
			switch (FilteredOutputPort.conduitType)
			{
				case ConduitType.Gas:
					status_item = Db.Get().BuildingStatusItems.NeedGasOut;
					break;
				case ConduitType.Liquid:
					status_item = Db.Get().BuildingStatusItems.NeedLiquidOut;
					break;
				case ConduitType.Solid:
					status_item = Db.Get().BuildingStatusItems.NeedSolidOut;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			bool conduitWasConnected = this.needsConduitStatusItemGuid != Guid.Empty;
			if (conduitIsConnected != conduitWasConnected)
				return;
			this.needsConduitStatusItemGuid = this.selectable.ToggleStatusItem(status_item, this.needsConduitStatusItemGuid, !conduitIsConnected);
		}
		void UpdateConduitBlockedStatus()
		{
			bool conduitEmpty = GetFlowManager().IsConduitEmpty(filteredOutputCell);
			StatusItem blockedMultiples = Db.Get().BuildingStatusItems.ConduitBlockedMultiples;
			bool wasConduitEmpty = this.conduitBlockedStatusItemGuid != Guid.Empty;
			if (conduitEmpty != wasConduitEmpty)
				return;
			this.conduitBlockedStatusItemGuid = this.selectable.ToggleStatusItem(blockedMultiples, this.conduitBlockedStatusItemGuid, !conduitEmpty);
		}
	}
}
