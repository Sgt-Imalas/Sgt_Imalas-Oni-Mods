using CannedFoods.Foods;

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
                    public static LocString DESC = global::STRINGS.UI.FormatAsLink(global::STRINGS.ITEMS.FOOD.COOKEDFISH.NAME, CookedFishConfig.ID)+" preserved for the ages.\nIncreases radiation resistance on consumption.";
                }
            }
        }
    }
}
