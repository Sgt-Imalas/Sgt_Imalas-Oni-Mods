using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace ConveyorTiles
{
    internal class STRINGS
    {
        public class MODCONFIG
        {
            public class CONVEYORSPEEDMULTIPLIER
            {
                public static LocString NAME = "Speed Multiplier";
                public static LocString TOOLTIP = "Speed at which the conveyor tile moves items. Value defines default belt speed";
            }
            public class CONVEYORUSESPOWER
            {
                public static LocString NAME = "Tile Power Consumption";
                public static LocString TOOLTIP = "Set the amount of power each conveyor tile consumes. Value defines default belt speed";
            }
            public class IMMUNEDUPES
            {
                public static LocString NAME = "Immune Dupes";
                public static LocString TOOLTIP = "Duplicants are not affected by the conveyor.\nMight reduce fun.";
            }
            public class IMMUNECRITTERS
            {
                public static LocString NAME = "Immune Critters";
                public static LocString TOOLTIP = "Critters are not affected by the conveyor.\nMight reduce fun.";
            }
            public class NOLOGICPORT
            {
                public static LocString NAME = "No Logic Ports";
                public static LocString TOOLTIP = "Removes the logic input of conveyor tiles";
            }
            public class COLOREDGEAR
            {
                public static LocString NAME = "Speed defines gear colour";
                public static LocString TOOLTIP = "Tint the gear of the conveyor tile based on its speed, color is based on Factorio belt coloring.";
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
        public class BUILDING
        {
            public class STATUSITEMS
            {
                public class CT_CONVEYORTILE_POWERCONSUMPTION
                {
                    public static LocString NAME = (LocString)"{CONFIG}: {WATTS}";
                    public static LocString TOOLTIP = (LocString)("This conveyor tile is currently in the {CONFIG} configuration, consuming {WATTS} while active.");

                    public static LocString SPEEDBASE = "Normal Speed";
                    public static LocString SPEEDFAST = "Fast Speed";
                    public static LocString SPEEDEXPRESS = "Express Speed";
                    public static LocString SPEEDRAPIT = "Rapit Speed";
                }
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


                    public static LocString LOGIC_PORT_INPUT = "Toggle the tile state and the direction";
                    public static LocString LOGIC_PORT_INPUT_ACTIVE_RIBBON = FormatAsAutomationState("Bit 1", UI.AutomationState.Active) + ": Enable building" +
                        "\n" + FormatAsAutomationState("Bit 2", AutomationState.Active) + ": change the conveyor tile to move items from left to right"
                        +"\n" + FormatAsAutomationState("Bit 3 and 4", AutomationState.Active) + ": increase the speed and power consumption of the conveyor tile."
                        ;

                    public static LocString LOGIC_PORT_INPUT_INACTIVE_RIBBON = FormatAsAutomationState("Bit 1", UI.AutomationState.Standby) + ": Disable building" +
                        "\n" + FormatAsAutomationState("Bit 2", AutomationState.Standby) + ": change the conveyor tile to move items from right to left"
                        +"\n" + FormatAsAutomationState("Bit 3 and 4", AutomationState.Standby) + ": decrease the speed and power consumption of the conveyor tile";
                    public class CHECKBOX_LOGICDIRECTION
                    {
                        public static LocString HEADER = "Conveyor Direction";
                        public static LocString LABEL = "Logic controls conveyor direction";
                        public static LocString TOOLTIP = "When enabled, the second logic input bit will control the direction of the conveyor.\nCheck the bit description in the logic overlay for that.";
                    }

                }
            }
        }
    }
}
