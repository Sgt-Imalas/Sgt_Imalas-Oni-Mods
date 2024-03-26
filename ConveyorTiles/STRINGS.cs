using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConveyorTiles
{
    internal class STRINGS
    {
        public class MODCONFIG
        {
            public class CONVEYORSPEEDMULTIPLIER
            {
                public static LocString NAME = "Speed Multiplier";
                public static LocString TOOLTIP = "Speed at which the conveyor tile moves items";
            }
            public class CONVEYORUSESPOWER
            {
                public static LocString NAME = "Tile Power Consumption";
                public static LocString TOOLTIP = "Set the amount of power each conveyor tile consumes.";
            }
            public class IMMUNEDUPES
            {
                public static LocString NAME = "Immune Dupes";
                public static LocString TOOLTIP = "Duplicants are not affected by the conveyor.\nMight reduce fun.";
            }
            public class NOLOGICPORT
            {
                public static LocString NAME = "No Logic Ports";
                public static LocString TOOLTIP = "Removes the logic input of conveyor tiles";
            }
        }
        public class MODBUTTONS
        {
            public class FLIPBUTTON
            {
                public static LocString LABEL = "Flip direction";
                public static LocString TOOLTIP = "Flip the direction of the conveyor tile";
            }
        }



        public class BUILDINGS
        {
            public class PREFABS
            {
                public class CT_CONVEYORTILE
                {
                    public static LocString NAME = UI.FormatAsLink("Conveyor Tile", nameof(CT_CONVEYORTILE));
                    public static LocString EFFECT = "Moves items, critters and duplicants when powered and active.\nGasses and liquids can pass through this tile.";
                    public static LocString DESC = "Wheeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee!";

                }
            }
        }
    }
}
