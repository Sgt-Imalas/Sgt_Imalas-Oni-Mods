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
                    public static LocString DESC = "Barbecue preserved for the ages.";
                }
                public class CF_CANNEDTUNA
                {
                    public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned Tuna", CannedTunaConfig.ID);
                    public static LocString DESC = "Pacu fry preserved for the ages.\nIncreases radiation resistance on consumption.";
                }
            }
        }
    }
}
