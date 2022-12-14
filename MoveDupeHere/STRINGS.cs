using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveDupeHere
{
    internal class STRINGS
    {


        public class BUILDINGS
        {
            public class PREFABS
            {
                public class MDH_GOHERETILE
                {
                    public static LocString NAME = UI.FormatAsLink("Duplicant Caller Tile", nameof(MDH_GOHERETILE));
                    public static LocString DESC = (LocString)"Get ova here!";
                    public static LocString EFFECT = (LocString)"Assign a duplicant to this tile\n\nWhile this tile is powered and recieving a "+ UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active)+", the assigned duplicant will get called to this tile.\n\nSend a "+ UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby)+" to cancel an ongoing move command.";
                }
            }
        }
    }
}
