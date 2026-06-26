using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace BubbleChest
{
	internal class STRINGS
	{
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class BC_BUBBLECHEST
				{
					public static LocString NAME = FormatAsLink("Bubbling Chest", nameof(BC_BUBBLECHEST));
					public static LocString DESC = "Look at all of them bubbles!";
					public static LocString EFFECT = "Spawns a decorative bubble column when provided with a gas input";

					public static LocString SLIDER_TITLE = "Bubble Rate:";
					public static LocString SLIDER_LABEL = "bubble rate per second in g";
				}
			}
		}
		public class CREATURES
		{
			public class MODIFIERS
			{
				public class BC_INTERACTEDWITHBUBBLECHEST
				{
					public static LocString NAME = "Bubbles!";
					public static LocString TOOLTIP = "This critter recently enjoyed playing with a bubble column";
				}
			}
		}
	}
}
