using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI ;

namespace Imalas_TwitchChaosEvents
{
    internal class STRINGS
    {
        public class ELEMENTS
        {
            public class ITCE_INVERSE_ICE
            {
                public static LocString NAME = FormatAsLink("Eci", nameof(ITCE_INVERSE_ICE));
                public static LocString DESC = "weird Ice";
            }
            public class ITCE_INVERSE_WATER
            {
                public static LocString NAME = FormatAsLink("Retaw", nameof(ITCE_INVERSE_WATER));
                public static LocString DESC = "weird Water";
            }
            public class ITCE_INVERSE_STEAM
            {
                public static LocString NAME = FormatAsLink("Maets", nameof(ITCE_INVERSE_STEAM));
                public static LocString DESC = "weird Steam";
            }
        }

        public class CHAOSEVENTS
        {
            public class INVERSEELEMENT
            {
                public static LocString NAME = "Inversium";
                public static LocString TOASTTEXT = "ʇuǝɯǝlƎ sᴉɥʇ ɥʇᴉʍ ɟɟo sᴉ ƃuᴉɥʇǝɯoS";
            }
            public class BUZZSAW
            {
                public static LocString NAME = "Buzzsaw";
                public static LocString TOASTTEXT = "With recommendations of the white palace";
            }
        }
    }
}
