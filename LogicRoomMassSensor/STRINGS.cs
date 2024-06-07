using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicRoomMassSensor
{
    internal class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class LRMS_ROOMPRESSURESENSOR
                {
                    public static LocString NAME = UI.FormatAsLink("Room Mass Sensor", "LOGICPRESSURESENSORLIQUID");

                    public static LocString DESC = "how much stuff is in that infinite storage?!";

                    public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when  room mass enters the chosen range.";

                    public static LocString LOGIC_PORT = " Room Mass";

                    public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if room mass is within the selected range";

                    public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);


                    public static LocString SIDESCREEN_TITLE = "Element Mass Filter";
                    public static LocString SIDESCREEN_LABEL = "Tile Element Mass only";
                    public static LocString SIDESCREEN_TOOLTIP = "Enable to only count the mass of the element the sensor sits in";
                }
            }
        }
    }
}
