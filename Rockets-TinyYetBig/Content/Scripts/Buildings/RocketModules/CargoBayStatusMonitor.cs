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
		private Operational operational;
		private static readonly EventSystem.IntraObjectHandler<CargoBayStatusMonitor> UpdateLogicCircuitCBDelegate = new(((component, data) => component.UpdateLogicCircuitCB(data)));
		//private static readonly EventSystem.IntraObjectHandler<CargoBayStatusMonitor> UpdateLogicCircuitOnMoveCBDelegate = new(((component, data) => component.UpdatePortsOnMoveDelayedCB(data)));

		private void UpdateLogicCircuitCB(object data) => this.UpdateLogicAndActiveState();
		private void UpdateLogicAndActiveState()
		{
			if (cargoBay?.storage == null)
			{
				SgtLogger.warning("cargo bay is null");
				return;
			}
			float massStored = cargoBay.AmountStored;
			float remainingCapacity = cargoBay.RemainingCapacity;
			//SgtLogger.l("RemainingCapacity: " + remainingCapacity + ", massStored: " + massStored);
			ports.SendSignal(EMPTY_PORT_ID, massStored < 0 || Mathf.Approximately(massStored, 0) ? 1 : 0);
			ports.SendSignal(FULL_PORT_ID, Mathf.Approximately(remainingCapacity, 0) ? 1 : 0);
		}
		private void UpdatePortsOnMoveCB(object data) => this.UpdateLogicAndActiveState();
		//public void UpdateLogicAndActiveStateDelayed()
		//{
		//	SgtLogger.l("OnPortMove");
		//	//ports.OnMove();
		//	UpdateLogicAndActiveState();
		//	StartCoroutine(DelayedUpdate());
		//}
		//private void UpdatePortsOnMoveDelayedCB(object data) => this.UpdateLogicAndActiveStateDelayed();
		//IEnumerator DelayedUpdate()
		//{
		//	yield return null;
		//	yield return null;
		//	//ports.OnMove();
		//	SgtLogger.l("OnPortMoveDelayed");
		//	ports.OnMove();
		//	UpdateLogicAndActiveState();
		//}

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.OnStorageChange, UpdateLogicCircuitCBDelegate);
			Subscribe((int)GameHashes.OperationalChanged, UpdateLogicCircuitCBDelegate);
			Subscribe((int)GameHashes.StorageCapacityChanged, UpdateLogicCircuitCBDelegate);
			Subscribe((int)GameHashes.ReorderableBuildingChanged, UpdateLogicCircuitCBDelegate);
			UpdateLogicAndActiveState();

		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.OnStorageChange, UpdateLogicCircuitCBDelegate);
			Unsubscribe((int)GameHashes.OperationalChanged, UpdateLogicCircuitCBDelegate);
			Unsubscribe((int)GameHashes.StorageCapacityChanged, UpdateLogicCircuitCBDelegate);
			Unsubscribe((int)GameHashes.ReorderableBuildingChanged, UpdateLogicCircuitCBDelegate);

			base.OnCleanUp();
		}
	}
}
