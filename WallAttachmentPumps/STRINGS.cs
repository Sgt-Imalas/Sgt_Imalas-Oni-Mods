using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace WallAttachmentPumps
{
	internal class STRINGS
	{
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class WAP_LIQUIDPUMP
				{
					public static LocString NAME = FormatAsLink("Wall Attachment Liquid Pump", nameof(WAP_LIQUIDPUMP));
					public static LocString DESC = "Piping a pump's output to a building's intake will send liquid to that building.";
					public static LocString EFFECT = $"Draws in {UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID")} and runs it through {UI.FormatAsLink("Pipes", "LIQUIDPIPING")}.\n\nMust be attached to a floor, wall or ceiling in order to draw in liquid from the other side.";
				}
				public class WAP_GASPUMP
				{
					public static LocString NAME = FormatAsLink("Wall Attachment Gas Pump", nameof(WAP_GASPUMP));
					public static LocString DESC = "Piping a pump's output to a building's intake will send gas to that building.";
					public static LocString EFFECT = $"Draws in {UI.FormatAsLink("Gas", "ELEMENTS_GAS")} and runs it through {UI.FormatAsLink("Pipes", "GASPIPING")}.\n\nMust be attached to a floor, wall or ceiling in order to draw in gas from the other side.";
				}
			}
		}
	}
}
