using static STRINGS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BawoonFwiend
{
    internal class STRINGS
    {
        public class MISC
        {
            public class TAGS
            {
                public static LocString BALLOONGAS = "Balloon Gas";
            }

        }
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class BF_BALLOONSTATION
                {
                    public static LocString NAME = (LocString)FormatAsLink("Balloon Dispenser", nameof(BF_BALLOONSTATION));
                    public static LocString DESC = (LocString)"You get a balloon,\n you get a balloon,\neverybody gets a balloon!";
                    public static LocString EFFECT = (LocString)("This building gives out balloons to duplicants during downtime.\n\nConsumes 5kg of either "+ FormatAsLink("Hydrogen", "HYDROGEN")+" or "+ FormatAsLink("Helium", "HELIUM")+" for each balloon created.");
                }
            }
        }
        public class UI
        {
            public class UISIDESCREENS
            {
                public class BF_BALLOONSTAND
                {
                    public static LocString TITLE = (LocString)"Balloon Skins";
                    public static LocString TOGGLEALLON = (LocString)"Activate All";
                    public static LocString TOGGLEALLOFF = (LocString)"Deactivate All";
                }
            }
        }
    }
}
