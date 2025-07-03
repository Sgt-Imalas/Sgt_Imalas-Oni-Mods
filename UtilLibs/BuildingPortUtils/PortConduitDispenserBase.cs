using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
	// Don't add this directly as components to GameObjects
	// Use a subclass to inherit this and doesn't add anything
	// This is to avoid crashes on loading savegames due to class name clashes
	[SerializationConfig(MemberSerialization.OptIn)]
	public abstract class PortConduitDispenserBase : KMonoBehaviour, ISaveLoadable
	{
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
		public bool invertElementFilter;

		[SerializeField]
		public bool alwaysDispense;

		[SerializeField]
		public bool SkipSetOperational = false;

		private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

		private FlowUtilityNetwork.NetworkItem networkItem;

		[MyCmpReq]
		readonly private Operational operational;

		[MyCmpReq]
		public Storage storage;

		private HandleVector<int>.Handle partitionerEntry;

		private int utilityCell = -1;

		private int elementOutputOffset;

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

		public ConduitFlow.ConduitContents ConduitContents
		{
			get
			{
				return this.GetConduitManager().GetContents(this.utilityCell);
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
				GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
				return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
			}
		}

		public void SetConduitData(ConduitType type)
		{
			this.conduitType = type;
		}

		public ConduitFlow GetConduitManager()
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

		private void OnConduitConnectionChanged(object data)
		{
			base.Trigger(-2094018600, this.IsConnected);
		}

		public virtual CellOffset GetUtilityCellOffset()
		{
			return new CellOffset(0, 1);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();

			var building = base.GetComponent<Building>();
			this.utilityCell = building.GetCellWithOffset(building.Orientation == Orientation.Neutral ? this.conduitOffset : this.conduitOffsetFlipped);
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			this.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitType, Endpoint.Source, this.utilityCell, base.gameObject);
			networkManager.AddToNetworks(this.utilityCell, this.networkItem, true);

			ScenePartitionerLayer layer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
			this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, layer, new Action<object>(this.OnConduitConnectionChanged));
			this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.LastPostUpdate);
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

		protected virtual void ConduitUpdate(float dt)
		{
			if (!SkipSetOperational)
			{
				this.operational.SetFlag(PortConduitDispenserBase.outputConduitFlag, this.IsConnected);
			}
			if (this.operational.IsOperational || this.alwaysDispense)
			{
				PrimaryElement primaryElement = this.FindSuitableElement();
				if (primaryElement != null)
				{
					primaryElement.KeepZeroMassObject = true;
					ConduitFlow conduitManager = this.GetConduitManager();
					float num = conduitManager.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
					if (num > 0f)
					{
						float num2 = num / primaryElement.Mass;
						int num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
						primaryElement.ModifyDiseaseCount(-num3, "CustomConduitDispenser.ConduitUpdate");
						primaryElement.Mass -= num;
						base.Trigger(-1697596308, primaryElement.gameObject);
					}
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
					continue;

				if (primaryElement.Mass <= 0f || primaryElement.Element == null)
					continue;

				if (((this.conduitType == ConduitType.Liquid) ? primaryElement.Element.IsLiquid : primaryElement.Element.IsGas) && ElementAllowedByFilter(primaryElement))
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

			if ((elementFilter == null || this.elementFilter.Length == 0) && (tagFilter == null || tagFilter.Length == 0))
			{
				return true;
			}

			if (tagFilter != null)
			{
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
			return simhashAllowed != invertElementFilter;
		}
	}
}
