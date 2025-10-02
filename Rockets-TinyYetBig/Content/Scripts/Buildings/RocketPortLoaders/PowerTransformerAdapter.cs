using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
	internal class PowerTransformerAdapter :
		UtilityNetworkLink,
		IHaveUtilityNetworkMgr,
		ICircuitConnected
	{
		[SerializeField]
		public Wire.WattageRating maxWattageRating;

		public Wire.WattageRating GetMaxWattageRating() => this.maxWattageRating;

		[Serialize]
		[SerializeField]
		LaunchPad TargetLaunchpad = null;

		[MyCmpGet] Building building;
		int powerInputCell;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}
		public override void OnSpawn()
		{
			powerInputCell = building.GetPowerInputCell();
			if (TargetLaunchpad != null && TargetLaunchpad.LandedRocket != null)
			{
				this.VirtualCircuitKey = TargetLaunchpad.LandedRocket.CraftInterface;
			}
			else
			{

			}
			base.OnSpawn();
		}

		public void OnSelectLaunchpad(LaunchPad launchPad)
		{

			if (TargetLaunchpad != null)
			{
				TargetLaunchpad.Unsubscribe((int)GameHashes.RocketLanded, OnRocketLanded);
				TargetLaunchpad.Unsubscribe((int)GameHashes.RocketLaunched, OnRocketLaunched);
				TargetLaunchpad.Unsubscribe((int)GameHashes.RocketModuleChanged, OnRocketModuleChanged);
				if (TargetLaunchpad.HasRocket())
				{
				}
			}
			TargetLaunchpad = launchPad;


			if (TargetLaunchpad.HasRocket())
			{
				Game.Instance.electricalConduitSystem.AddToVirtualNetworks(TargetLaunchpad.LandedRocket.CraftInterface, this, true);
				Game.Instance.electricalConduitSystem.AddSemiVirtualLink(this.powerInputCell, TargetLaunchpad.LandedRocket.CraftInterface);

				TargetLaunchpad.Subscribe((int)GameHashes.RocketLanded, OnRocketLanded);
				TargetLaunchpad.Subscribe((int)GameHashes.RocketLaunched, OnRocketLaunched);
				TargetLaunchpad.Subscribe((int)GameHashes.RocketModuleChanged, OnRocketModuleChanged);
			}
		}
		void OnRocketModuleChanged(object data)
		{
			if (data is RocketModuleCluster rmc)
			{
				ReevaluateConnectionState(rmc.CraftInterface);
			}
		}
		void OnRocketLaunched(object data)
		{
			if (data is RocketModuleCluster rmc)
			{
				if (VirtualCircuitKey == data)
					DisconnectFromPower(rmc.CraftInterface);
			}
		}
		void OnRocketLanded(object data)
		{
			if (data is RocketModuleCluster rmc)
			{
				if (VirtualCircuitKey != data)
					DisconnectFromPower(VirtualCircuitKey);

				ConnectToPower(rmc.CraftInterface);
			}
		}
		void ReevaluateConnectionState(object cmi)
		{
			if (cmi == null)
				return;

			if (VirtualCircuitKey != cmi)
				DisconnectFromPower(cmi);
			VirtualCircuitKey = cmi;


			Game.Instance.electricalConduitSystem.RemoveSemiVirtualLink(powerInputCell, cmi);
			Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(cmi, this, true);
		}
		void ConnectToPower(object cmi)
		{
			if (cmi == null)
				return;

			Game.Instance.electricalConduitSystem.RemoveSemiVirtualLink(powerInputCell, cmi);
			Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(cmi, this, true);
		}
		void DisconnectFromPower(object cmi)
		{
			if (cmi == null)
				return;

			Game.Instance.electricalConduitSystem.RemoveSemiVirtualLink(powerInputCell, cmi);
			Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(cmi, this, true);
		}


		public void SetLinkConnected(bool connect)
		{
			if (connect && this.visualizeOnly)
			{
				this.visualizeOnly = false;
				if (!this.isSpawned)
					return;
				this.Connect();
			}
			else
			{
				if (connect || this.visualizeOnly)
					return;
				if (this.isSpawned)
					this.Disconnect();
				this.visualizeOnly = true;
			}
		}

		public override void OnDisconnect(int cell1, int cell2) => Game.Instance.electricalConduitSystem.RemoveSemiVirtualLink(cell1, this.VirtualCircuitKey);

		public override void OnConnect(int cell1, int cell2) => Game.Instance.electricalConduitSystem.AddSemiVirtualLink(cell1, this.VirtualCircuitKey);

		public IUtilityNetworkMgr GetNetworkManager() => (IUtilityNetworkMgr)Game.Instance.electricalConduitSystem;

		public bool IsVirtual { get; private set; }

		public int PowerCell => this.GetNetworkCell();

		public object VirtualCircuitKey { get; private set; }

		public void AddNetworks(ICollection<UtilityNetwork> networks)
		{
			int networkCell = this.GetNetworkCell();
			UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(networkCell);
			if (networkForCell == null)
				return;
			networks.Add(networkForCell);
		}

		public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
		{
			int networkCell = this.GetNetworkCell();
			UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(networkCell);
			return networks.Contains(networkForCell);
		}
	}
}
