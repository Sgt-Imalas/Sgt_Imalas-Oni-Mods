using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OniRetroEdition
{
    internal class STRINGS
    {
        public class MISC
        {
            public class PLACERS
            {
                public class SLURPPLACER
                {
                    public static LocString NAME = (LocString)"Pump";
                    public static LocString TOOL_NAME = (LocString)"Pumping Tool";
                    public static LocString ACTION_NAME = (LocString)"Pump Liquids";

                    public static LocString SLURPBUTTON = "Command duplicants to pump liquid\nRequires a duplicant with plumbing skill";
                }
            }
        }
        public class ITEMS
        {
            public class RETROONI_BONES
            {
                public static LocString NAME = "Bones";
                public static LocString DESC = "The last remains of a lost friend.";
                public static LocString RECIPEDESC = "Crushing bones into "+global::STRINGS.ELEMENTS.LIME.NAME;
            }
        }
        public class CREATURES
        {
            public class SPECIES
            {
                public static LocString SHOCKWORMSPECIES = "Shock Worm";
            }
            public class FAMILY_PLURAL
            {
                public static LocString SHOCKWORMSPECIES = "Shock Worms";
            }
        }
        public class UI
        {
            public class RETRO_OVERLAY
            {
                public class TOXICITY
                {
                    public static LocString SLIGHTLYTOXIC = "Slightly Toxic";
                    public static LocString VERYYTOXIC = "Very Toxic";
                }
                public class SOUND
                {
                    public static LocString OVERLAYNAME = "NOISE";
                    public static LocString TOOLTIP1 = "Total noise Level: {0} dB";
                    public static LocString TOOLTIP2 = "Noise Sources:";
                }

            }
            public class KLEI_INVENTORY_SCREEN
            {
                public class SUBCATEGORIES
                {
                    public static LocString BUILDING_ONI_RETRO = "ONI Retro Skins";
                }

            }
            public class NEWBUILDCATEGORIES
            {
                public static class WORKSTATIONS
                {
                    public static LocString NAME = (LocString)"Skill Assignment";
                    public static LocString BUILDMENUTITLE = (LocString)"Skill Assignment";
                    public static LocString TOOLTIP = (LocString)"";
                }
            }
            public class TOOLTIPS
            {
                public static LocString MANAGEMENTMENU_REQUIRES_SKILL_STATION_RETRO = (LocString)("Build a Skills Board to unlock this menu\n\nThe " + (string)BUILDINGS.PREFABS.ROLESTATION.NAME + " can be found in the " + global::STRINGS.UI.FormatAsBuildMenuTab("Stations Tab", Action.Plan10) + " of the Build Menu");
            }

        }
    }
}
