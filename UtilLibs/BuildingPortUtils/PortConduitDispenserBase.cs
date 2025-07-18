using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Operational;
using static STRINGS.BUILDINGS.PREFABS.CONDUIT;

namespace UtilLibs.BuildingPortUtils
{
	// Don't add this directly as components to GameObjects
	// Use a subclass to inherit this and doesn't add anything
	// This is to avoid crashes on loading savegames due to class name clashes
	[SerializationConfig(MemberSerialization.OptIn)]
	public abstract class PortConduitDispenserBase : KMonoBehaviour, ISaveLoadable
	{
		/// <summary>
		/// DO NOT USE FOR SOLIDS.
		/// </summary>


		protected Guid hasPipeGuid;
		protected Guid pipeEmptyGuid;

		[SerializeField]
		public CellOffset conduitOffset;

		[SerializeField]
		public CellOffset conduitOffsetFlipped;

		[SerializeField]
		public ConduitType conduitType;

		[SerializeField]
		public SimHashes[] elementFilter = null;

		[SerializeField]
		public Tag[] tagFilter = null;

		[SerializeField]
		public bool invertElementFilter = false;

		[SerializeField]
		public bool alwaysDispense;

		[SerializeField]
		public bool showFullPipeNotification = true;

		[SerializeField]
		public bool SkipSetOperational = false;

		private Operational.Flag outputConduitFlag;

		private FlowUtilityNetwork.NetworkItem networkItem;

		[MyCmpReq]
		readonly private Operational operational;

		[MyCmpReq]
		[SerializeField]
		public Storage storage;

		private HandleVector<int>.Handle partitionerEntry;

		private int utilityCell = -1;

		private int elementOutputOffset;

		[MyCmpReq]
		private KSelectable selectable;

		public void AssignPort(PortDisplayOutput port)
		{
			this.conduitType = port.type;
			this.conduitOffset = port.offset;
			this.conduitOffsetFlipped = port.offsetFlipped;
		}

		public ConduitType TypeOfConduit
		{
			get
			{
				return this.conduitType;
			}
		}

		public int UtilityCell
		{
			get
			{
				return this.utilityCell;
			}
		}

		public bool IsConnected
		{
			get
			{
				GameObject gameObject = Grid.Objects[this.utilityCell, GetConduitLayer()];
				return gameObject != null && gameObject.TryGetComponent<BuildingComplete>(out _);
			}
		}

		public void SetConduitData(ConduitType type)
		{
			this.conduitType = type;
		}

		public int GetConduitLayer()
		{
			switch (this.conduitType)
			{
				case ConduitType.Gas:
					return (int)ObjectLayer.GasConduit;
				case ConduitType.Liquid:
					return (int)ObjectLayer.LiquidConduit;
					//case ConduitType.Solid:
					//	return (int)ObjectLayer.SolidConduit;
			}
			return -1;
		}


		public ConduitFlow GetConduitManager()
		{
			switch (this.conduitType)
			{
				case ConduitType.Gas:
					return Game.Instance.gasConduitFlow;
				case ConduitType.Liquid:
					return Game.Instance.liquidConduitFlow;
					//case ConduitType.Solid:
					//	return Game.Instance.solidConduitFlow;
			}
			return null;
		}

		private void OnConduitConnectionChanged(object data)
		{
			base.Trigger(-2094018600, this.IsConnected);
			UpdateNotifications(false);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();

			var building = base.GetComponent<Building>();
			this.utilityCell = building.GetCellWithOffset(building.Orientation == Orientation.Neutral ? this.conduitOffset : this.conduitOffsetFlipped);
			outputConduitFlag = new Operational.Flag($"output_conduit_{utilityCell}_{conduitType}", Operational.Flag.Type.Functional);

			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Source, this.utilityCell, base.gameObject);
			networkManager.AddToNetworks(this.utilityCell, this.networkItem, true);

			ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
			this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
			this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.LastPostUpdate);
			this.OnConduitConnectionChanged(null);
			UpdateNotifications(false);
		}

		public override void OnCleanUp()
		{
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			networkManager.RemoveFromNetworks(this.utilityCell, this.networkItem, true);

			this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
			base.OnCleanUp();
		}

		protected virtual void ConduitUpdate(float dt)
		{
			bool outputFull = false;
			if (this.operational.IsOperational || this.alwaysDispense)
			{
				PrimaryElement primaryElement = this.FindSuitableElement();
				if (primaryElement != null)
				{
					var conduitFlow = GetConduitManager();
					//if (iFlow is ConduitFlow conduitFlow)
					//{
					primaryElement.KeepZeroMassObject = true;
					float massAddedToConduit = conduitFlow.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
					if (massAddedToConduit > 0f)
					{
						float percentageMassAdded = massAddedToConduit / primaryElement.Mass;
						int diseaseCountTransferred = (int)(percentageMassAdded * primaryElement.DiseaseCount);
						primaryElement.ModifyDiseaseCount(-diseaseCountTransferred, "CustomConduitDispenser.ConduitUpdate");
						primaryElement.Mass -= massAddedToConduit;
						base.Trigger((int)GameHashes.OnStorageChange, primaryElement.gameObject);
					}
					else
						outputFull = true;
					//}
					//else if(iFlow is SolidConduitFlow solidConduitFlow && solidConduitFlow.IsConduitEmpty(utilityCell))
					//{

					//}
				}
			}

			UpdateNotifications(outputFull);
		}
		bool wasFull = false;
		bool wasConnected = true;
		public virtual void UpdateNotifications(bool isFull)
		{
			if (!SkipSetOperational)
			{
				if (wasConnected != IsConnected)
				{
					wasConnected = IsConnected;
					this.operational.SetFlag(outputConduitFlag, wasConnected);

					StatusItem status_item = null;
					switch (this.conduitType)
					{
						case ConduitType.Gas:
							status_item = Db.Get().BuildingStatusItems.NeedGasOut;
							break;
						case ConduitType.Liquid:
							status_item = Db.Get().BuildingStatusItems.NeedLiquidOut;
							break;
							//case ConduitType.Solid:
							//	status_item = Db.Get().BuildingStatusItems.NeedSolidOut;
							//	break;
					}
					if (status_item != null)
						this.hasPipeGuid = this.selectable.ToggleStatusItem(status_item, this.hasPipeGuid, !this.wasConnected, this);
				}
			}
			if (showFullPipeNotification)
			{
				if (wasFull != isFull)
				{
					wasFull = isFull;
					pipeEmptyGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.ConduitBlocked, this.pipeEmptyGuid, isFull);
				}
			}
		}

		protected virtual PrimaryElement FindSuitableElement()
		{
			List<GameObject> items = this.storage.items;
			int count = items.Count;
			for (int i = 0; i < count; i++)
			{
				int index = (i + this.elementOutputOffset) % count;
				var item = items[index];
				if (item == null || !item.TryGetComponent<PrimaryElement>(out var primaryElement))
				{
					//SgtLogger.l("item " + index + " was null");
					continue;
				}

				if (primaryElement.Mass <= 0f || primaryElement.Element == null)
					continue;

				bool correctElementType = false;
				if (conduitType == ConduitType.Liquid)
					correctElementType = primaryElement.Element.IsLiquid;
				else if (conduitType == ConduitType.Gas)
					correctElementType = primaryElement.Element.IsGas;
				//SgtLogger.l(conduitType + " was correct for element " + primaryElement.GetProperName() + "= " + correctElementType);

				if (correctElementType && ElementAllowedByFilter(primaryElement))
				{
					this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
					return primaryElement;
				}
			}
			return null;
		}

		private bool ElementAllowedByFilter(PrimaryElement primaryelement)
		{
			var element = primaryelement.ElementID;

			//SgtLogger.l(gameObject.GetProperName()+ " - checking element: " + primaryelement.GetProperName() + ", " + primaryelement.ElementID);
			//SgtLogger.l("Filters are: ");
			//if (elementFilter != null)
			//	foreach (var filter in elementFilter)
			//		SgtLogger.l(filter.ToString());

			if ((elementFilter == null || !elementFilter.Any()) && (tagFilter == null || !tagFilter.Any()))
			{
				//SgtLogger.l("filters null");
				return true;
			}

			if (tagFilter != null && tagFilter.Any())
			{
				//SgtLogger.l("Tagfilter not null");
				return primaryelement.HasAnyTags(tagFilter) != invertElementFilter;
			}

			bool simhashAllowed = false;
			for (int num = 0; num != this.elementFilter.Length; num++)
			{
				if (this.elementFilter[num] == element)
				{
					simhashAllowed = true;
				}
			}
			//SgtLogger.l("Simhash is allowed: " + simhashAllowed + " + filter is inverted? " + invertElementFilter + ", result: " + (simhashAllowed != invertElementFilter).ToString());
			return simhashAllowed != invertElementFilter;
		}
	}
}
