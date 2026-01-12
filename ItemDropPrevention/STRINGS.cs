using STRINGS;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.UI;

namespace ItemDropPrevention
{
	public class STRINGS
	{
		public class IDP_MOD_CONFIG
		{

			public static LocString PAUSE_MENU_OPEN_CONFIG_BUTTONTEXT = "Adjust GripNowIncluded Settings";

			public static LocString WRANGLE_DROPPED_CRITTERS = "Schedule wrangle task for dropped critters.";
			public static LocString WRANGLE_DROPPED_CRITTERS_TOOLTIP = "Automatically issue a new wrangle task for a critter if it was dropped before reaching its destination";

			public static LocString SWEEP_DROPPED_ITEMS = "Schedule sweep task for dropped items.";
			public static LocString SWEEP_DROPPED_ITEMS_TOOLTIP = "Automatically issue a sweep task for items that were dropped before reaching their destinations";
		}
	}
}
