using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathTub
{
    internal class STRINGS
    {
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
                    public static LocString DESC = "Stores and delivers Satellites to orbit.";
                    public static LocString EFFECT = "The Satellite Carrier Module is used to deploy and retrieve satellites on the star map\n\nA satellite can be constructed from satellite parts, made at the " + global::STRINGS.UI.FormatAsLink(global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.NAME, CraftingTableConfig.ID);
                }
            }
        }
    }
}
