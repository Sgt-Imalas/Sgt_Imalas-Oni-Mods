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
                    public static LocString DESC = "When showers aren't enough!";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.SHOWER.EFFECT;
                }
            }
        }
    }
}
