using STRINGS;

namespace UtilityGlass
{
	internal class STRINGS
	{
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class UG_EXTERIORGLASSWALL
				{
					public static LocString NAME = UI.FormatAsLink("Window Wall", nameof(UG_EXTERIORGLASSWALL));
					public static LocString DESC = "Drywall can be used in conjunction with tiles to build airtight rooms on the surface.";
					public static LocString EFFECT = ("Prevents " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " and " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " loss in space.\n\nBuilds an insulating backwall behind buildings.");
				}
				public class UG_REINFORCEDGLASS
				{
					public static LocString NAME = global::STRINGS.UI.FormatAsLink("Reinforced Window Tile", nameof(UG_REINFORCEDGLASS));
					public static LocString DESC = "\"Transparent Aluminum\"";
					public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nAllows " + UI.FormatAsLink("Light", "LIGHT") + " and " + UI.FormatAsLink("Decor", "DECOR") + " to pass through.\n\nCan withstand extreme pressures and impacts.";
				}
			}
		}
	}
}
