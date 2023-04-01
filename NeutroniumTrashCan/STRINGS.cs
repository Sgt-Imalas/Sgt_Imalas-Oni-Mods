using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeutroniumTrashCan
{
    internal class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class NTC_NEUTRONIUMTRASHCAN
                {
                        public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Neutronium Trash Can", nameof(NTC_NEUTRONIUMTRASHCAN));
                        public static LocString DESC = "Its bigger on the inside!";
                        public static LocString EFFECT = "Eats and deletes Neutronium - for those cleanly shaved walls.";
                    
                }
            }
        }
    }
}
