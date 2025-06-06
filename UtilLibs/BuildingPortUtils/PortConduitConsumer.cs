using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
	[SkipSaveFileSerialization]
	public class PortConduitConsumer : KMonoBehaviour
	{
		public enum WrongElementResult
		{
			Destroy,
			Dump,
			Store
		}

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
		public bool forceAlwaysSatisfied;

		[SerializeField]
		public bool alwaysConsume;

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

		private int utilityCell = -1;

		public float consumptionRate = float.PositiveInfinity;

		public static readonly Operational.Flag elementRequirementFlag = new Operational.Flag("elementRequired", Operational.Flag.Type.Requirement);

		private HandleVector<int>.Handle partitionerEntry;

		private bool satisfied;

		public ConduitConsumer.WrongElementResult wrongElementResult;

		public void AssignPort(PortDisplayInput port)
		{
			this.conduitType = port.type;
			this.conduitOffset = port.offset;
			this.conduitOffsetFlipped = port.offsetFlipped;
		}

		public bool IsConnected
		{
			get
			{
				GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
				return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
			}
		}

		public bool CanConsume
		{
			get
			{
				bool result = false;
				if (this.IsConnected)
				{
					ConduitFlow conduitManager = this.GetConduitManager();
					result = (conduitManager.GetContents(this.utilityCell).mass > 0f);
				}
				return result;
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
				ConduitFlow conduitManager = this.GetConduitManager();
				return conduitManager.GetContents(inputCell).mass;
			}
		}

		public void SetConduitData(ConduitType type)
		{
			this.conduitType = type;
		}

		private ConduitFlow GetConduitManager()
		{
			ConduitType conduitType = this.conduitType;
			if (conduitType == ConduitType.Gas)
			{
				return Game.Instance.gasConduitFlow;
			}
			if (conduitType != ConduitType.Liquid)
			{
				return null;
			}
			return Game.Instance.liquidConduitFlow;
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

			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Sink, this.utilityCell, base.gameObject);
			networkManager.AddToNetworks(this.utilityCell, this.networkItem, true);

			ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
			this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
			this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
			this.OnConduitConnectionChanged(null);
		}

		public override void OnCleanUp()
		{
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			networkManager.RemoveFromNetworks(this.utilityCell, this.networkItem, true);

			this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
			base.OnCleanUp();
		}

		private void OnConduitConnectionChanged(object data)
		{
			base.Trigger(-2094018600, this.IsConnected);
		}

		private void ConduitUpdate(float dt)
		{
			if (this.isConsuming)
			{
				ConduitFlow conduitManager = this.GetConduitManager();
				this.Consume(dt, conduitManager);
			}
		}

		private void Consume(float dt, ConduitFlow conduit_mgr)
		{
			if (this.building.Def.CanMove)
			{
				this.utilityCell = this.GetInputCell();
			}
			if (this.IsConnected)
			{
				ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(this.utilityCell);
				if (contents.mass > 0f)
				{
					this.IsSatisfied = true;
					if (this.alwaysConsume || this.operational.IsOperational)
					{
						float num = (!(this.capacityTag != GameTags.Any)) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityTag);
						float b = Mathf.Min(this.storage.RemainingCapacity(), this.capacityKG - num);
						float num2 = this.ConsumptionRate * dt;
						num2 = Mathf.Min(num2, b);
						float num3 = 0f;
						if (num2 > 0f)
						{
							num3 = conduit_mgr.RemoveElement(this.utilityCell, num2).mass;
						}
						Element element = ElementLoader.FindElementByHash(contents.element);
						bool flag = element.HasTag(this.capacityTag);
						if (num3 > 0f && this.capacityTag != GameTags.Any && !flag)
						{
							base.Trigger(-794517298, new BuildingHP.DamageSourceInfo
							{
								damage = 1,
								source = global::STRINGS.BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
								popString = global::STRINGS.UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
							});
						}
						if (flag || this.wrongElementResult == ConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || this.capacityTag == GameTags.Any)
						{
							if (num3 > 0f)
							{
								int disease_count = (int)((float)contents.diseaseCount * (num3 / contents.mass));
								Element element2 = ElementLoader.FindElementByHash(contents.element);
								ConduitType conduitType = this.conduitType;
								if (conduitType != ConduitType.Liquid)
								{
									if (conduitType == ConduitType.Gas)
									{
										if (element2.IsGas)
										{
											this.storage.AddGasChunk(contents.element, num3, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
										}
										else
										{
											global::Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id.ToString());
										}
									}
								}
								else if (element2.IsLiquid)
								{
									this.storage.AddLiquid(contents.element, num3, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
								}
								else
								{
									global::Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id.ToString());
								}
							}
						}
						else if (num3 > 0f && this.wrongElementResult == ConduitConsumer.WrongElementResult.Dump)
						{
							int disease_count2 = (int)((float)contents.diseaseCount * (num3 / contents.mass));
							int gameCell = Grid.PosToCell(base.transform.GetPosition());
							SimMessages.AddRemoveSubstance(gameCell, contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num3, contents.temperature, contents.diseaseIdx, disease_count2, true, -1);
						}
					}
				}
				else
				{
					this.IsSatisfied = false;
				}
			}
			else
			{
				this.IsSatisfied = false;
			}
		}
	}
}
