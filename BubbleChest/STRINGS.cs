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
				}
			}
		}
	}
}
