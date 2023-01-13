using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightBridge
{
    internal class STRINGS
    {


        public class BUILDINGS
        {
            public class PREFABS
            {

                public class LB_LIGHTBRIDGEEMITTER
                {

                    public static LocString NAME = (LocString)"Light Bridge Emitter";
                    public static LocString DESC = (LocString)"These bridges are made from light. If you rubbed your cheek on one, it would be like standing outside with the sun shining on your face. It would also set your hair on fire, so don't actually do it.";
                    public static LocString EFFECT = (LocString)"Emitts a semi-permeable light bridge of configurable length.\n\nLonger bridges require higher amounts of power";
                }
            }
        }
        public class UI
        {
            public class UISIDESCREENS
            {
                public class LIGHTBRIDGESIDESCREEN
                {
                    public static LocString TITLE = "Light Bridge";
                    public static LocString TILEUNITS = " Tiles";
                    public static LocString DESC = "Set length of light bridge.\nLonger light bridges cost more power to maintain.";

                }
            }
        }
    }
}
