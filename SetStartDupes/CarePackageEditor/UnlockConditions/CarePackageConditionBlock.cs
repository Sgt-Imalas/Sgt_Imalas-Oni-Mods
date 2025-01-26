using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.CarePackageEditor.UnlockConditions
{
	public class CarePackageConditionBlock
	{
		public bool LockedByCycle = false;
		public bool LockedByDiscovery = false;
		public string DiscoveryId = null;
		public int CycleThreshold = 0;

		public void ToggleCycleLock()
		{
			LockedByCycle = !LockedByCycle;
		}
		public void ToggleDiscoveryLock()
		{
			LockedByDiscovery = !LockedByDiscovery;
		}
		public void UpdateDiscoveryId(string id)
		{
			DiscoveryId = id;
		}
		public void UpdateCycleThreshold(int threshold)
		{
			CycleThreshold = threshold;
		}
	}
}
