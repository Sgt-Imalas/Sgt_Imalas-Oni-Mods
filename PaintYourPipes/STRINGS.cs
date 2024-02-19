using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintYourPipes
{
    internal class STRINGS
    {
        public class PAINTABLEBUILDING
        {
            public static LocString TITLE = "Tint Colour";
            public static LocString LABEL = "Paint whole network";
            public static LocString TOOLTIP = "Applies the colour of the currently selected item to its whole network.";

            public static LocString TOGGLE_TEXT = "COLOR OVERLAYS";
            public static LocString TOGGLE_TOOLTIP = "Toggle <b>Colors in Overlays</b>\n{Hotkey}";
        }
        public class HOTKEYACTIONS
        {
            public static LocString TOGGLE_OVERLAY_COLOR = "Toggle Color in Overlays";

        }
        public class MODCONFIG
        {
            public static LocString OVERLAYONLYNAME = "Color Overlays Only";
            public static LocString OVERLAYONLYDESC = "Only show colors in overlays";

        }
    }
}
