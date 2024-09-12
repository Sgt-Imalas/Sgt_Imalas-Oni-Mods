using STRINGS;

namespace BathTub
{
	internal class STRINGS
	{
		public class MOODLAMPSKINS
		{
			public static LocString RUBBERDUCKIE = "Rubber Duckie";
		}
		public class ITEMS
		{
			public class INDUSTRIAL_PRODUCTS
			{
				public class BT_RUBBERDUCKIE
				{
					public static LocString NAME = "Rubber Duckie";
					public static LocString DESC = "All you need for a refreshing bath.\nFloats on liquids.\n\nArthur likes to know its exact function.\n\nHelps you think.";


					public static LocString TOOLTIP = "Toggle random quack sounds. On by default";
					public static LocString BUTTON_ON = "Shut! (disables random quacks)";
					public static LocString BUTTON_OFF = "Quack! (enables random quacks)";
				}
			}
		}
		public class BUILDING
		{
			public class STATUSITEMS
			{
				public class BT_BATHTUBFILLING
				{
					public static LocString NAME = (LocString)"Filling Up: ({fullness})";
					public static LocString TOOLTIP = (LocString)("This bathtub is currently filling with " + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD + "\n\nIt will be available to use when the " + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD + " level reaches <b>100%</b>");
				}
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class SGTIMALAS_BATHTUB
				{
					public static LocString NAME = "Bathtub";
					public static LocString DESC = "When showers aren't enough!";
					public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.SHOWER.EFFECT;
					public class FACADES
					{
						public class HANDY_RETRO_TUB
						{
							public static LocString NAME = "Handy Retro Tub";
							public static LocString DESC = "No, there are no duplicants buried beneath that hold it...";
						}
					}
				}
			}
		}
	}
}
