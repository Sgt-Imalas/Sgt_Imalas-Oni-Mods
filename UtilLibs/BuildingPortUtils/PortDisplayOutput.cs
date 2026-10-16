using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
	public class PortDisplayOutput : DisplayConduitPortInfo
	{
		public PortDisplayOutput(ConduitType type, CellOffset offset, CellOffset? offsetFlipped = null, Color? color = null) : base(type, offset, offsetFlipped, false, color) { }
	}
}
