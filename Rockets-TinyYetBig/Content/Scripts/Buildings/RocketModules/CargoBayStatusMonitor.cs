using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.ROCKETBUILDMENUCATEGORIES;

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
		private static readonly EventSystem.IntraObjectHandler<CargoBayStatusMonitor> UpdateLogicCircuitCBDelegate = new(((component, data) => component.UpdateLogicCircuitCB(data)));
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

		List<int> handlers = [];
		public override void OnSpawn()
		{
			base.OnSpawn();
			handlers.Add(Subscribe((int)GameHashes.OnStorageChange, UpdateLogicCircuitCBDelegate));
			handlers.Add(Subscribe((int)GameHashes.OperationalChanged, UpdateLogicCircuitCBDelegate));
			handlers.Add(Subscribe((int)GameHashes.StorageCapacityChanged, UpdateLogicCircuitCBDelegate));
			handlers.Add(Subscribe((int)GameHashes.ReorderableBuildingChanged, UpdateLogicCircuitCBDelegate));
			handlers.Add(Subscribe((int)GameHashes.ClustercraftStateChanged, UpdateLogicCircuitCBDelegate));
			UpdateLogicAndActiveState();

		}
		public override void OnCleanUp()
		{
			foreach(var ha in handlers)
				Unsubscribe(ha);
			base.OnCleanUp();
		}
	}
}
