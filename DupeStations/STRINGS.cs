using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace DupeStations
{
    internal class STRINGS
    {
        public class MISC
        {
            public class TAGS
            {
                public static LocString DS_PAJAMASTAG = global::STRINGS.EQUIPMENT.PREFABS.SLEEPCLINICPAJAMAS.NAME;
            }

        }
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class DS_PAJAMASDISPENSER
                {
                    public static LocString NAME = FormatAsLink("Pajamas Checkpoint", nameof(DS_PAJAMASDISPENSER));
                    public static LocString DESC = "Time to have some beautiful dreams!";
                    public static LocString EFFECT = "Duplicants walking past it equip pajamas if walking to the right or unequip it if walkin to the left.\nDuplicants cannot already wear any other clothing.\n\nCan be flipped to inverse these directions";
                }
            }
        }
    }
}
