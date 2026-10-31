using System;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.CargoBays
{
	internal class CargoBayStatusMonitor : KMonoBehaviour
	{
		public static readonly HashedString FULL_PORT_ID = new HashedString("CargoBayStatusMonitor.FULL_PORT");
		public static readonly HashedString EMPTY_PORT_ID = new HashedString("CargoBayStatusMonitor.EMPTY_PORT");

		[MyCmpGet]
		public CargoBayCluster cargoBay;
		[MyCmpGet]
		private LogicPorts ports;
		[MyCmpGet]
		public RocketModuleCluster rocketModule;

		[MyCmpGet]
		private Operational operational;
		private static readonly Action<object, object> UpdateLogicCircuitCBDelegate = (ctx, data) => (ctx as CargoBayStatusMonitor).UpdateLogicAndActiveState();
		//private readonly Action<object> UpdateLogicCircuitCBDelegate = (_) => (ctx as CargoBayStatusMonitor).UpdateLogicAndActiveState();
		//private static readonly EventSystem.IntraObjectHandler<CargoBayStatusMonitor> UpdateLogicCircuitOnMoveCBDelegate = new(((component, data) => component.UpdatePortsOnMoveDelayedCB(data)));

		private void UpdateLogicCircuitCB(object data) => this.UpdateLogicAndActiveState();
		private void UpdateLogicAndActiveState()
		{
			if (rocketModule.CraftInterface.m_clustercraft.Status != Clustercraft.CraftStatus.Grounded)
				return;

			if (cargoBay?.storage == null)
			{
				SgtLogger.warning("cargo bay is null");
				return;
			}
			float massStored = cargoBay.AmountStored;
			float remainingCapacity = cargoBay.RemainingCapacity;
			//SgtLogger.l("RemainingCapacity: " + remainingCapacity + ", massStored: " + massStored);
			ports.SendSignal(EMPTY_PORT_ID, massStored <= 0 || Mathf.Approximately(massStored, 0) ? 1 : 0);
			ports.SendSignal(FULL_PORT_ID, remainingCapacity <= 0 ||Mathf.Approximately(remainingCapacity, 0) ? 1 : 0);
		}
		private void UpdatePortsOnMoveCB(object data) => this.UpdateLogicAndActiveState();

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Sub(GameHashes.OnStorageChange, UpdateLogicCircuitCBDelegate);
			this.Sub(GameHashes.OperationalChanged, UpdateLogicCircuitCBDelegate);
			this.Sub(GameHashes.StorageCapacityChanged, UpdateLogicCircuitCBDelegate);
			this.Sub(GameHashes.ReorderableBuildingChanged, UpdateLogicCircuitCBDelegate);
			this.Sub(GameHashes.ClustercraftStateChanged, UpdateLogicCircuitCBDelegate);
			UpdateLogicAndActiveState();

		}
		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();
		}
	}
}
