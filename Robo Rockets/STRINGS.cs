using RoboRockets;
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
        public class ITEMS
        {
            public class INDUSTRIAL_PRODUCTS
            {
                public class RR_BRAINFLYER
                {
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Brain", nameof(RoboRocketConfig));
                    public static LocString DESC = "This brain learned to fly.";
                    public static LocString EFFECT = "Seated in a brain jar, this brain will fly your rockets\n\nFlying a rocket slowly increases the brains piloting skill, resulting in faster rockets.\n\nSince it does not have anything to learn, it may fly even more efficient than a duplicant.";
                }
            }
        }
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

        public class BUILDING
        {
            public class STATUSITEMS
            {
                public class RR_BRAINEXPERIENCE
                {
                    public static LocString NAME = "{BRAINXPSTATE}";
                    public static LocString TOOLTIP = "This Brain is a {BRAINXPSTATE}";
                    public static LocString LVL1 = (LocString) global::STRINGS.UI.FormatAsLink("Novice Pilot", nameof(LVL1));
                    public static LocString LVL2 = (LocString)global::STRINGS.UI.FormatAsLink("Advanced Beginner Pilot",nameof(LVL2));
                    public static LocString LVL3 = (LocString)global::STRINGS.UI.FormatAsLink("Competent Pilot", nameof(LVL3));
                    public static LocString LVL4 = (LocString)global::STRINGS.UI.FormatAsLink("Proficient Pilot", nameof(LVL4));
                    public static LocString LVL5 = (LocString)global::STRINGS.UI.FormatAsLink("Expert Pilot", nameof(LVL5));
                    public static LocString LVL6 = (LocString)global::STRINGS.UI.FormatAsLink("Master Pilot", nameof(LVL6));
                    public static LocString LVL0 = "";
                }
                public class CRY_DUPLICANTCRYODAMAGE
                {
                    public static LocString NAME = "Warning, Duplicant is thawing improperly";
                    public static LocString TOOLTIP = "When this duplicant thaws fully, it won't have a good time.";
                }
                public class CRY_DUPLICANTATTEMPERATURE
                {
                    public static LocString NAME = "Energy Saving Mode";
                    public static LocString TOOLTIP = "Fully cooled down, this buiding has entered energy saving mode.";
                }
            }
        }
    }
}
