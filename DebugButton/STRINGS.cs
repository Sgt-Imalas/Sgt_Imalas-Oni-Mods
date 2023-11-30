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
                public static LocString TOOLTIP_DEBUG_LOCKED = "Debug mode is not enabled!";
                public class DEBUG_INSTABUILD_TOGGLE
                {
                    public static LocString NAME = "INSTA BUILD";
                    public static LocString TOOLTIP_TOGGLE = "Toggle <b>"+ global::STRINGS.INPUT_BINDINGS.DEBUG.DEBUGINSTANTBUILDMODE+"</b>\n{Hotkey}";
                }
                public class DEBUG_SUPERSPEED_TOGGLE
                {
                    public static LocString NAME = "SUPERSPEED";
                    public static LocString TOOLTIP_TOGGLE = "Toggle <b>" + global::STRINGS.INPUT_BINDINGS.DEBUG.DEBUGSUPERSPEED + "</b>\n{Hotkey}";
                }
                public class DEBUG_TOGGLE
                {
                    public static LocString NAME = "DEBUG MODE";
                    public static LocString TOOLTIP_TOGGLE = "Toggle <b>Debug mode</b>";
                }
            }
        }
    }
}
