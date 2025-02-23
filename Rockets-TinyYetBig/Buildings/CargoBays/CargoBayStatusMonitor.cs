using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.CargoBays
{
	internal class CargoBayStatusMonitor:KMonoBehaviour
	{
		public static readonly HashedString FULL_PORT_ID = new HashedString("CargoBayStatusMonitor.FULL_PORT");
		public static readonly HashedString EMPTY_PORT_ID = new HashedString("CargoBayStatusMonitor.EMPTY_PORT");

		[MyCmpGet]
		public Storage cargoBayStorage;
		[MyCmpGet]
		private LogicPorts ports;

		[MyCmpGet]
		private Operational operational;
		private static readonly EventSystem.IntraObjectHandler<CargoBayStatusMonitor> UpdateLogicCircuitCBDelegate = new (((component, data) => component.UpdateLogicCircuitCB(data)));
		
		private void UpdateLogicCircuitCB(object data) => this.UpdateLogicAndActiveState();
		private void UpdateLogicAndActiveState()
		{
			if(cargoBayStorage == null)
			{
				SgtLogger.warning("cargo bay storage is null");
				return;
			}
			float massStored = cargoBayStorage.MassStored();

			ports.SendSignal(EMPTY_PORT_ID, massStored < 0 || Mathf.Approximately(cargoBayStorage.MassStored(),0) ? 1 : 0);
			ports.SendSignal(FULL_PORT_ID, cargoBayStorage.IsFull() ? 1 : 0);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.Subscribe((int)GameHashes.OnStorageChange, UpdateLogicCircuitCBDelegate);
			this.Subscribe((int)GameHashes.OperationalChanged, UpdateLogicCircuitCBDelegate);
			//this.Subscribe((int)GameHashes.StorageCapacityChanged, UpdateLogicCircuitCBDelegate);
			UpdateLogicAndActiveState();
			
		}
		public override void OnCleanUp()
		{
			this.Unsubscribe((int)GameHashes.OnStorageChange, UpdateLogicCircuitCBDelegate);
			this.Unsubscribe((int)GameHashes.OperationalChanged, UpdateLogicCircuitCBDelegate);
			//this.Unsubscribe((int)GameHashes.StorageCapacityChanged, UpdateLogicCircuitCBDelegate);
			base.OnCleanUp();
		}
	}
}
