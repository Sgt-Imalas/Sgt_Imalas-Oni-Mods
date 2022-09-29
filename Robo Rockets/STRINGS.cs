using Robo_Rockets;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboRockets
{
    public class STRINGS
    {
        public class UI
        {
            public class STARMAP
            {
                public class AISTATUS
                {
                    public static LocString NAME = "Ai controlled";
                    public static LocString TOOLTIP = "This Rocket flies on it's own - your duplicants are scared yet impressed!";
                }
            }
        }
        public class BUILDINGS
        {

            public class PREFABS
            {
                public class AIMODULE
                {

                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Control Module [DEPRECATED]", nameof(RoboRocketConfig));
                    public static LocString DESC = "A Module that controls your Rocket without any duplicant input.";
                    public static LocString EFFECT = "Functions as a Command Module.\n\nOne Command Module may be installed per rocket.\n\nWon't allow any duplicants inside"; 
                }
                public class RR_AICONTROLMODULE
                {
                    public static LocString DisplayName = "AI Control Module";
                    public static LocString Description = "A Module that controls your Rocket without any duplicant input.";

                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Control Module", nameof(AIControlModuleConfig));
                    public static LocString DESC = "A Module that controls your Rocket without any duplicant input.";
                    public static LocString EFFECT = "Functions as a Command Module.\n\nOne Command Module may be installed per rocket.\n\nWon't allow any duplicants inside"; 
                }
            }
        }
    }
}
