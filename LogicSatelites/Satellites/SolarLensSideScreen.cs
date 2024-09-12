using UnityEngine;

namespace LogicSatellites.Satellites
{
	class SolarLensSideScreen : SideScreenContent
	{
		public override bool IsValidForTarget(GameObject target)
		{
			return target.TryGetComponent<SolarLens>(out var lens);
		}
	}
}
