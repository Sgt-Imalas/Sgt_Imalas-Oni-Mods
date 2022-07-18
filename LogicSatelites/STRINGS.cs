using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatelites
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class LS_SATELLITECARRIERMODULE
                {
                    public static LocString NAME = "Satellite Carrier Module";
                    public static LocString DESC = "Stores and delivers Satellites to orbit.";
                    public static LocString EFFECT = "The Satellite Carrier Module is used to deploy and retrieve satellites on the star map";
                }
            }
        }
        public class ITEMS
        {
            public class SATELLITE
            {
                public static LocString TITLE = "Logic Satellite";
                public static LocString DESC = "Deploy this satellite on the star map to create a logic relay";
            }
            public class LS_SATELLITEGRID
            {
                public static LocString TITLE = "Satellite";
                public static LocString DESC = "This Satellite monitors the space around its location and amplifies interplanetary logic signals."+
                    string.Format("\n\n\nThis Satellite will gradually reveal the space around it up to a radius of {0}.\n\nIt will also amplify a logic signal by redirecting it up to {1} Tiles further", Config.Instance.SatelliteScannerRange, Config.Instance.SatelliteLogicRange);
            }
        }
        public class UI
        {
            public class UISIDESCREENS
            {
                public class SATELLITECARRIER_SIDESCREEN
                {
                    public static LocString TITLE = "Satellite Carrier";
                }
            }
        }
    }
}
