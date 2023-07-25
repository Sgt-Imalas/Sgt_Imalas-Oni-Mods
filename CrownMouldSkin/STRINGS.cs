using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDINGS.PREFABS;

namespace CrownMouldSkin
{
    internal class STRINGS
    {
        public class UI
        {
            public class KLEI_INVENTORY_SCREEN
            {
                public class SUBCATEGORIES
                {
                    public static LocString BUILDING_CORNER_MOULDING = "Mouldings";
                }

            }
        }
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class CORNERMOULDING
                {
                    public class FACADES
                    {
                        public class DEFAULT_CORNERMOULDING
                        {
                            public static LocString NAME = global::STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.NAME;
                            public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.CORNERMOULDING.DESC;
                        }
                        public class CORNER_TILE_B
                        {
                            public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Bulky Crescent Corner Moulding", nameof(CORNERMOULDING));
                            public static LocString DESC = (LocString)"A new look for the Corner Moulding";
                        }
                        public class CORNER_TILE_C
                        {
                            public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Sprouting Blossom Corner Moulding", nameof(CORNERMOULDING));
                            public static LocString DESC = (LocString)"A new look for the Corner Moulding";
                        }
                        public class CORNER_TILE_D
                        {
                            public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Hand Fan Corner Moulding", nameof(CORNERMOULDING));
                            public static LocString DESC = (LocString)"A new look for the Corner Moulding";
                        }
                        public class CORNER_TILE_E
                        {
                            public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Sleek Angled Corner Moulding", nameof(CORNERMOULDING));
                            public static LocString DESC = (LocString)"A new look for the Corner Moulding";
                        }
                        public class CORNER_TILE_F
                        {
                            public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Greek Ornament Corner Moulding", nameof(CORNERMOULDING));
                            public static LocString DESC = (LocString)"A new look for the Corner Moulding";
                        }
                        public class CORNER_TILE_G
                        {
                            public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Elegant Arc Corner Moulding", nameof(CORNERMOULDING));
                            public static LocString DESC = (LocString)"A new look for the Corner Moulding";
                        }
                    }
                }

            }
        }
    }
}
