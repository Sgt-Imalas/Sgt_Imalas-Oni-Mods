using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Imalas_TwitchChaosEvents.STRINGS.ELEMENTS;
using static STRINGS.UI ;

namespace Imalas_TwitchChaosEvents
{
    public class STRINGS
    {
        public class ITEMS
        {
            public class FOOD
            {
                public class ICT_TACO
                {
                    public static LocString NAME = FormatAsLink("Taco", nameof(ICT_TACO));
                    public static LocString DESC = "A staple meal that provides vital nutrients and energy to those who consume it.";
                }
            }

        }
        public class COMETS
        {
            public class ITC_TACOCOMET
            {
                public static LocString NAME = FormatAsLink("Flying Taco", nameof(ITC_TACOCOMET));
                public static LocString DESC = "A flying taco, it looks delicious!";
            }
                
        }

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
            public class ITCE_CREEPYLIQUID
            {
                public static LocString NAME = FormatAsLink("Creeper", nameof(ITCE_CREEPYLIQUID));
                public static LocString DESC = "The Creeper.";
            }
            public class ITCE_CREEPYLIQUIDGAS
            {
                public static LocString NAME = FormatAsLink("Creeper Tendril", nameof(ITCE_CREEPYLIQUIDGAS));
                public static LocString DESC = "the ever extending tendril of creeper";
            }
            
        }

        public class CHAOSEVENTS
        {
            public class INVERSEELEMENT
            {
                public static LocString NAME = "Doolf";
                public static LocString TOAST = "Doolf!";
                public static LocString TOASTTEXT = "sᴉɥʇ ɥʇᴉʍ ɟɟo sᴉ ƃuᴉɥʇǝɯoS";
            }
            public class BUZZSAW
            {
                public static LocString NAME = "Buzzsaw";
                public static LocString TOAST = "Buzzsaw";
                public static LocString TOASTTEXT = "With recommendations of the white palace";
            }
            public class CREEPERRAIN
            {
                public static LocString NAME = "Creeper Rain";
                public static LocString TOAST = "Creeper Rain";
                public static LocString TOASTTEXT = "The Creeper is about to arrive on {0}";
            }
            public class TACORAIN
            {
                public static LocString NAME = "Taco Rain";
                public static LocString TOAST = "Taco Rain!";
                public static LocString TOASTTEXT = "It's raining Tacos!";
                public static LocString NEWRECIPE = "\n\nThere is also a new recipe in the Gas Range.";
            }
        }
    }
}
