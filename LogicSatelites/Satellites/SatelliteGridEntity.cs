using KSerialization;
using System.Collections.Generic;
using static LogicSatellites.Behaviours.ModAssets;

namespace LogicSatellites.Behaviours
{
	public class SatelliteGridEntity : ClusterGridEntity
	{
		[Serialize]
		public string clusterAnimName;
		[Serialize]
		private string m_name;
		[Serialize]
		public int satelliteType;
		[Serialize]
		public bool ShowOnMap = true;
		// private string clusterAnimSymbolSwapTarget;
		// private string clusterAnimSymbolSwapSymbol;

		public override bool SpaceOutInSameHex() => true;

		public override EntityLayer Layer => EntityLayer.Payload;

		public override string Name => this.m_name;
		public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
		{
			new ClusterGridEntity.AnimConfig()
			{
				animFile = Assets.GetAnim((HashedString) this.clusterAnimName),
				initialAnim = "idle_loop",
                //symbolSwapTarget = this.clusterAnimSymbolSwapTarget,
               // symbolSwapSymbol = this.clusterAnimSymbolSwapSymbol
            }
		};
		public override void OnSpawn()
		{
			base.OnSpawn();
			SetSatelliteName(ModAssets.GetSatelliteNameRandom());
			ModAssets.Satellites.Add(this);
			ModAssets.AdjazenzMatrixHolder.AddItemToGraph(this.Location);
		}
		public void SetSatelliteType(SatType type)
		{
			satelliteType = (int)type;
		}
		public void SetSatelliteName(string newName)
		{
			this.m_name = newName;

			this.name = SatelliteConfigurations[satelliteType].NAME + " " + newName;
			CharacterOverlay component = this.GetComponent<CharacterOverlay>();
			KSelectable selectable = this.GetComponent<KSelectable>();
			if (selectable != null)
			{
				selectable.SetName(this.name);
				selectable.entityName = this.name;
			}
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				NameDisplayScreen.Instance.UpdateName(component.gameObject);
			}

		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		public override void OnCleanUp()
		{
			ModAssets.Satellites.Remove(this);
			ModAssets.AdjazenzMatrixHolder.RemoveItemTFromGraph(this.Location);
			base.OnCleanUp();
		}
		public override bool IsVisible => ShowOnMap;

		public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
		public void Init(AxialI location) => this.Location = location;

	}
}
