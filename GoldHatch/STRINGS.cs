using STRINGS;

namespace GoldHatch
{
	internal class STRINGS
	{
		public class CREATURES
		{
			public class SPECIES
			{
				public class HATCH
				{
					public class VARIANT_GOLD
					{
						public static LocString NAME = UI.FormatAsLink("Gold Hatch", "HATCHGOLD");
						public static LocString DESC = ("Gold Hatches excrete solid " + UI.FormatAsLink("Gold", "GOLD") + " as waste and enjoy burrowing into the ground.");
						public static LocString EGG_NAME = UI.FormatAsLink("Gold Hatchling Egg", "HATCHGOLD");

						public class BABY
						{
							public static LocString NAME = UI.FormatAsLink("Gold Hatchling", "HATCHGOLD");
							public static LocString DESC = ("A doofy little Gold Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Gold Hatch", "HATCHGOLD") + ", which loves nibbling on various metals.");
						}
					}
				}
			}
		}
	}
}
