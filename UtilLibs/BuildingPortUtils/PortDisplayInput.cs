using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
	public class PortDisplayInput : DisplayConduitPortInfo
	{
		public PortDisplayInput(ConduitType type, CellOffset offset, CellOffset? offsetFlipped = null, Color? color = null) : base(type, offset, offsetFlipped, true, color) { }
	}
}
