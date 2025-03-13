using CannedFoods.Foods;
using STRINGS;

namespace CannedFoods
{
	public class STRINGS
	{
		public class ITEMS
		{
			public class INDUSTRIAL_PRODUCTS
			{
				public class CF_CANSCRAP
				{
					public static LocString NAME = "Can Scraps";
					public static LocString DESC = "You don't want to know where this can has been.\n\nDecreases Decor\n\nCan be recycled at the Rock Crusher or the Refinery.";
				}
				public class CF_EMPTYCAN
				{
					public static LocString NAME = "Empty Can";
					public static LocString DESC = "A can without any food in it. It helps creating preserved food";
				}
			}

			public class FOOD
			{
				public class CF_CANNEDBBQ
				{
					public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned Barbecue", CannedBBQConfig.ID);
					public static LocString DESC = global::STRINGS.UI.FormatAsLink(global::STRINGS.ITEMS.FOOD.COOKEDMEAT.NAME, CookedMeatConfig.ID) + " preserved for the ages.";
				}
				public class CF_CANNEDTUNA
				{
					public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned Tuna", CannedTunaConfig.ID);
					public static LocString DESC = global::STRINGS.UI.FormatAsLink(global::STRINGS.ITEMS.FOOD.COOKEDFISH.NAME, CookedFishConfig.ID) + " preserved for the ages.\nIncreases radiation resistance on consumption.";
				}
				public class CF_CANNEDBREAD
				{
					public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned Bread", CannedBreadConfig.ID);
					public static LocString DESC = global::STRINGS.UI.FormatAsLink(global::STRINGS.ITEMS.FOOD.SPICEBREAD.NAME, SpiceBreadConfig.ID) + " preserved for the ages.";
				}
				//public class CF_CANNEDMILK
				//{
				//	public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned condensed milk", CannedMilkConfig.ID);
				//	public static LocString DESC = "Condensed " + UI.PRE_KEYWORD + "Brackene" + UI.PST_KEYWORD + " sweetened with " + UI.PRE_KEYWORD + "Nectar" + UI.PST_KEYWORD;
				//}
				//public class CF_CANNEDBEANS
				//{
				//	public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned Beans", CannedBeansConfig.ID);
				//	public static LocString DESC = global::STRINGS.UI.FormatAsLink(global::STRINGS.ITEMS.FOOD.DEEPFRIEDNOSH.NAME, DeepFriedNoshConfig.ID) + " preserved for the ages.";
				//}
			}
		}
	}
}
