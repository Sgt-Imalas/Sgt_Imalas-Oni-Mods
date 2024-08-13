using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        public static LocString NAME = (LocString)UI.FormatAsLink("Gold Hatch", "HATCHGOLD");
                        public static LocString DESC = (LocString)("Gold Hatches excrete solid " + UI.FormatAsLink("Gold", "GOLD") + " as waste and enjoy burrowing into the ground.");
                        public static LocString EGG_NAME = (LocString)UI.FormatAsLink("Gold Hatchling Egg", "HATCHGOLD");

                        public class BABY
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Gold Hatchling", "HATCHGOLD");
                            public static LocString DESC = (LocString)("A doofy little Gold Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Gold Hatch", "HATCHGOLD") + ", which loves nibbling on various metals.");
                        }
                    }
                }
            }
        }
    }
}
