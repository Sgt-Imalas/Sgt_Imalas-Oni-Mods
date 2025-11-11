using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UtilLibs.BuildingPortUtils.SharedConduitUtils;

namespace UtilLibs.BuildingPortUtils
{
	[SkipSaveFileSerialization]
	public class PortConduitConsumer : KMonoBehaviour
	{
		protected Guid hasPipeGuid;
		protected Guid pipeBlockedGuid;
		public enum WrongElementResult
		{
			Destroy,
			Dump,
			Store
		}

		[SerializeField]
		public bool showEmptyPipeNotification = false;

		[SerializeField]
		public CellOffset conduitOffset;

		[SerializeField]
		public CellOffset conduitOffsetFlipped;

		[SerializeField]
		public ConduitType conduitType;

		[SerializeField]
		public bool ignoreMinMassCheck;

		[SerializeField]
		public Tag capacityTag = GameTags.Any;

		[SerializeField]
		public float capacityKG = float.PositiveInfinity;

		[SerializeField]
		public bool forceAlwaysSatisfied = false;

		[SerializeField]
		public bool SkipSetOperational = false;

		[SerializeField]
		public bool alwaysConsume = false;

		[SerializeField]
		public bool keepZeroMassObject = true;

		[NonSerialized]
		public bool isConsuming = true;

		private FlowUtilityNetwork.NetworkItem networkItem;

		[MyCmpReq]
		readonly private Operational operational;

		[MyCmpReq]
		readonly private Building building;

		[MyCmpGet]
		public Storage storage;

		[MyCmpReq]
		private KSelectable selectable;

		private int utilityCell = -1;

		public float consumptionRate = float.PositiveInfinity;

		Operational.Flag inputConduitFlag;

		private HandleVector<int>.Handle partitionerEntry;

		private bool satisfied;

		public ConduitConsumer.WrongElementResult wrongElementResult;

		public SimHashes lastConsumedElement = SimHashes.Vacuum;

		public void AssignPort(PortDisplayInput port)
		{
			this.conduitType = port.type;
			this.conduitOffset = port.offset;
			this.conduitOffsetFlipped = port.offsetFlipped;
		}

		bool IsConnected_Cache;
		public bool IsConnected
		{
			get
			{
				GameObject gameObject = Grid.Objects[this.utilityCell, GetConduitLayer(conduitType)];
				return gameObject != null && gameObject.TryGetComponent<BuildingComplete>(out _);
			}
		}

		public bool CanConsume
		{
			get
			{
				return IsConnected_Cache && MassAvailable > 0;
			}
		}

		public ConduitType TypeOfConduit
		{
			get
			{
				return this.conduitType;
			}
		}

		public bool IsAlmostEmpty
		{
			get
			{
				return !this.ignoreMinMassCheck && this.MassAvailable < this.ConsumptionRate * 30f;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !this.ignoreMinMassCheck && (this.MassAvailable == 0f || this.MassAvailable < this.ConsumptionRate);
			}
		}

		public float ConsumptionRate
		{
			get
			{
				return this.consumptionRate;
			}
		}

		public bool IsSatisfied
		{
			get
			{
				return this.satisfied || !this.isConsuming;
			}
			set
			{
				this.satisfied = (value || this.forceAlwaysSatisfied);
			}
		}

		public float MassAvailable
		{
			get
			{
				int inputCell = this.GetInputCell();
				IConduitFlow iconduitManager = GetConduitFlow(conduitType);
				if (iconduitManager is ConduitFlow flow)
					return flow.GetContents(inputCell).mass;
				if (iconduitManager is SolidConduitFlow solidConduitFlow)
				{
					var content = solidConduitFlow.GetPickupable(solidConduitFlow.GetContents(inputCell).pickupableHandle);
					if (content != null)
						return content.TotalAmount;
				}
				return 0;
			}
		}

		public void SetConduitData(ConduitType type)
		{
			this.conduitType = type;
		}
		
		private int GetInputCell()
		{
			var building = base.GetComponent<Building>();
			return building.GetCellWithOffset(building.Orientation == Orientation.Neutral ? this.conduitOffset : this.conduitOffsetFlipped);
		}


		public override void OnSpawn()
		{
			base.OnSpawn();
			this.utilityCell = this.GetInputCell();
			inputConduitFlag = new Operational.Flag($"input_conduit_connected_{utilityCell}_{conduitType}", Operational.Flag.Type.Functional);

			this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Sink, this.utilityCell, base.gameObject);
			GetConduitMng(conduitType).AddToNetworks(this.utilityCell, this.networkItem, true);

			ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[GetConduitLayer(conduitType)];
			this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
			GetConduitFlow(conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
			this.OnConduitConnectionChanged(null);

			UpdateNotifications();
		}

		public override void OnCleanUp()
		{
			GetConduitMng(conduitType).RemoveFromNetworks(this.utilityCell, this.networkItem, true);

			GetConduitFlow(conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
			base.OnCleanUp();
		}

		private void OnConduitConnectionChanged(object _)
		{
			IsConnected_Cache = this.IsConnected;
			base.Trigger((int)GameHashes.ConduitConnectionChanged, BoxedBools.Box(IsConnected_Cache));
			UpdateNotifications();
		}
		public virtual void UpdateNotifications()
		{
			if (!SkipSetOperational)
			{
				if (IsConnected_Cache != wasConnected)
				{
					wasConnected = IsConnected_Cache;
					StatusItem status_item = ConduitDisplayPortPatching.GetInputStatusItem(conduitType);
					this.hasPipeGuid = this.selectable.ToggleStatusItem(status_item, this.hasPipeGuid, !wasConnected, new Tuple<ConduitType, Tag>(this.conduitType, this.capacityTag));
					this.operational.SetFlag(inputConduitFlag, wasConnected);
				}
			}
			if (showEmptyPipeNotification)
			{
				bool connectedAndSatisfied = IsConnected_Cache && IsSatisfied;
				if (wasSatisfied != connectedAndSatisfied)
				{
					wasSatisfied = connectedAndSatisfied;
					pipeBlockedGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.ConduitBlockedMultiples, this.pipeBlockedGuid, !wasSatisfied);
				}
			}

		}
		bool wasSatisfied = true;
		bool wasConnected = true;

		private void ConduitUpdate(float dt)
		{
			if (isConsuming)
				this.Consume(dt, GetConduitFlow(conduitType));
			UpdateNotifications();
		}
		private void Consume(float dt, IConduitFlow iConMng)
		{
			IsSatisfied = false;
			if (this.building.Def.CanMove)
			{
				this.utilityCell = this.GetInputCell();
			}
			if (this.IsConnected_Cache)
			{
				if (iConMng is ConduitFlow conduit_mgr)
				{
					ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(this.utilityCell);
					if (contents.mass > 0f)
					{
						IsSatisfied = true;
						if (this.alwaysConsume || this.operational.IsOperational)
						{
							float num = (!(this.capacityTag != GameTags.Any)) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityTag);
							float remainingStorageCapacity = Mathf.Min(this.storage.RemainingCapacity(), this.capacityKG - num);
							float scaledConsumptionRate = this.ConsumptionRate * dt;
							scaledConsumptionRate = Mathf.Min(scaledConsumptionRate, remainingStorageCapacity);
							float removedConduitMass = 0f;
							if (scaledConsumptionRate > 0f)
							{
								removedConduitMass = conduit_mgr.RemoveElement(this.utilityCell, scaledConsumptionRate).mass;
							}
							Element element = ElementLoader.FindElementByHash(contents.element);

							bool flag = element.HasTag(this.capacityTag);
							if (removedConduitMass > 0f && this.capacityTag != GameTags.Any && !flag)
							{
								this.IsSatisfied = true;
								base.BoxingTrigger(-794517298, new BuildingHP.DamageSourceInfo
								{
									damage = 1,
									source = global::STRINGS.BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
									popString = global::STRINGS.UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
								});
							}
							if (flag || this.wrongElementResult == ConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || this.capacityTag == GameTags.Any)
							{
								if (removedConduitMass > 0f)
								{
									if (element.id != this.lastConsumedElement)
									{
										DiscoveredResources.Instance.Discover(element.tag, element.materialCategory);
										lastConsumedElement = element.id;
									}

									int disease_count = (int)((float)contents.diseaseCount * (removedConduitMass / contents.mass));
									Element element2 = ElementLoader.FindElementByHash(contents.element);
									ConduitType conduitType = this.conduitType;

									if (conduitType == ConduitType.Gas)
									{
										if (element2.IsGas)
										{
											this.storage.AddGasChunk(contents.element, removedConduitMass, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
										}
										else
										{
											global::Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id.ToString());
										}
									}
									else if (conduitType == ConduitType.Liquid)
									{
										if (element2.IsLiquid)
										{
											this.storage.AddLiquid(contents.element, removedConduitMass, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
										}
										else
										{
											global::Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id.ToString());
										}
									}
								}
								else if (removedConduitMass > 0f && this.wrongElementResult == ConduitConsumer.WrongElementResult.Dump)
								{
									int disease_count2 = (int)((float)contents.diseaseCount * (removedConduitMass / contents.mass));
									SimMessages.AddRemoveSubstance(utilityCell, contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, removedConduitMass, contents.temperature, contents.diseaseIdx, disease_count2, true, -1);
								}
							}
						}
					}
				}
				else if (iConMng is SolidConduitFlow conduitFlow)
				{
					SolidConduitFlow.ConduitContents solidcontents = conduitFlow.GetContents(this.utilityCell);
					IsSatisfied = false;
					if (solidcontents.pickupableHandle.IsValid() && (this.alwaysConsume || this.operational.IsOperational))
					{
						float massInStorage = capacityTag != GameTags.Any ? this.storage.GetMassAvailable(this.capacityTag) : this.storage.MassStored();
						float StorageCapacity = Mathf.Min(this.storage.capacityKg, this.capacityKG);
						float remainingStorage = Mathf.Max(0.0f, StorageCapacity - massInStorage);
						IsSatisfied = true;
						if (remainingStorage > 0)
						{
							Pickupable pickupable1 = conduitFlow.GetPickupable(solidcontents.pickupableHandle);
							if (pickupable1.PrimaryElement.Mass <= remainingStorage || pickupable1.PrimaryElement.Mass > StorageCapacity)
							{
								Pickupable pickupable2 = conduitFlow.RemovePickupable(this.utilityCell);
								if (pickupable2 != null)
								{
									this.storage.Store(pickupable2.gameObject, true);
								}
							}
						}
					}
				}
			}
		}
	}
}
