using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radiator_Mod
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class RADIATORBASE
                {
                    public static LocString NAME = (LocString)"Space Radiator";
                    public static LocString DESC = (LocString)"Radiates off heat energy into space as infrared radiation.";
                    public static LocString EFFECT = (LocString)"Exchanges heat with the liquid pumped through it.\n\nIf in space, radiates off infrared energy based off the Stefan–Boltzmann law.";
                }
                public class RADIATORUPDATED
                {
                    public static LocString NAME = (LocString)"Space Radiator";
                    public static LocString DESC = (LocString)"Radiates off heat energy into space as infrared radiation.";
                    public static LocString EFFECT = (LocString)"Exchanges heat with the liquid pumped through it.\n\nIf in space, radiates off infrared energy based off the Stefan–Boltzmann law.";
                }
                public class RM_RADIATORROCKETWALLBUILDABLE
                {
                    public static LocString NAME = (LocString)"Space Radiator (Rocket)";
                    public static LocString DESC = (LocString)"Radiates off heat energy into space as infrared radiation.";
                    public static LocString EFFECT = (LocString)"Exchanges heat with the liquid pumped through it.\n\nIf in space, radiates off infrared energy based off the Stefan–Boltzmann law.\n\nHas to be attached to the rocket interior wall.";
                }
            }
        }

        /// <summary>
        /// StatusItems
        /// </summary>
        public class BUILDING
        {
            public class STATUSITEMS
            {
                public class RM_INSPACERADIATING
                {
                    public static LocString NAME = (LocString)"Radiating {0}";
                    public static LocString TOOLTIP = (LocString)("This radiator is currently radiating heat at {0}.");
                }
                public class RM_NOTINSPACE
                {
                    public static LocString NAME = (LocString)"Not in space";
                    public static LocString TOOLTIP = (LocString)("This radiators panels are not fully exposed to space, thus it won't radiate any heat into space.");
                }
                public class RM_BUNKERDOWN
                {
                    public static LocString NAME = (LocString)"Bunkered down";
                    public static LocString TOOLTIP = (LocString)("This radiator is currently protected from meteor impacts.");
                }
            }
        }
    }
}
