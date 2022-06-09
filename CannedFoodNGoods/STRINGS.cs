using CannedFoods.Foods;

namespace CannedFoods
{
    public class STRINGS
    {
        public class ITEMS
        {
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
