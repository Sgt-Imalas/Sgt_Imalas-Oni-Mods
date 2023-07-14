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
        public class UI
        {
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
