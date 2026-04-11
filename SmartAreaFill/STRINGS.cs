using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAreaFill
{
	internal class STRINGS
	{
		public class FLOODFILLTOOLS_OPTIONS
		{
			public class HOLD_DELAY
			{
				public static LocString NAME = "Flood fill hold delay";
				public static LocString TOOLTIP = "Time delay from holding down leftclick to flood fill starting to propagate, value in seconds.";
			}
			public class SOLID_DOORS
			{
				public static LocString NAME = "Doors count as solids";
				public static LocString TOOLTIP = "Affects tools that propagate in non-solid tiles;\nMakes these tools stop their floodfill at doors, even if the door is not solid (like a pneumatic door).";
			}
		}
	}
}
