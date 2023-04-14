using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DistributionPlatform;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.MISC.NOTIFICATIONS;
using static STRINGS.UI.TOOLS;

namespace CustomGameSettingsModifier
{
    internal class STRINGS
    {
        public class UI
        {
            public class CUSTOMGAMESETTINGSCHANGER
            {
                public static LocString BUTTONTEXT = "Change Savegame Settings";
                public class TITLE
                {
                    public static LocString TITLETEXT = "Savegame Settings";
                }
                public class CLOSE
                {
                    public static LocString TEXT = "Close";
                }
            }
        }
    }
}
