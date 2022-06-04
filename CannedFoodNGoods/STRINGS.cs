using CannedFoods.Foods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned Tuna", CannedBBQConfig.ID);
                    public static LocString DESC = "Barbecue preserved for the ages.";
                }
                public class CF_CANNEDFISH
                {
                    public static LocString NAME = global::STRINGS.UI.FormatAsLink("Canned Tuna", cannedTunaConfig.ID);
                    public static LocString DESC = "Pacu fry preserved for the ages.\nReduces rads on consumption";
                }
            }
        }
    }
}
