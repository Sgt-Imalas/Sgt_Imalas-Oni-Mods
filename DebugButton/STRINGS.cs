using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugButton
{
    internal class STRINGS
    {
        public class UI
        {
            public class TOOLS
            {
                public class DEBUG_INSTABUILD_TOGGLE
                {
                    public static LocString NAME = "INSTA BUILD";
                    public static LocString TOOLTIP_LOCKED = "Debug mode is not enabled!";
                    public static LocString TOOLTIP_TOGGLE = "Toggle <b>"+ global::STRINGS.INPUT_BINDINGS.DEBUG.DEBUGINSTANTBUILDMODE+"</b>\n{Hotkey}";
                }
            }
        }
    }
}
