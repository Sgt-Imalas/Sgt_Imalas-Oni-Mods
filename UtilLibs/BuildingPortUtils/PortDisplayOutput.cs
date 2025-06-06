using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{
	public class PortDisplayOutput : DisplayConduitPortInfo
	{
		public PortDisplayOutput(ConduitType type, CellOffset offset, CellOffset? offsetFlipped = null, Color? color = null) : base(type, offset, offsetFlipped, false, color) { }
	}
}
