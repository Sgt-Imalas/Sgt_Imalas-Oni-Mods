using RoboRockets;
using RoboRockets.LearningBrain;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RoboRockets.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;
using static RoboRockets.STRINGS.MISC.TAGS;
using static STRINGS.BUILDINGS.PREFABS;

namespace RoboRockets
{
    public class STRINGS
    {
        public class MISC
        {
            public class TAGS
            {
                public static LocString RR_SPACEBRAINFLYER = RR_BRAINFLYER.NAME; 
            }
        }
        public class ITEMS
        {
            public class INDUSTRIAL_PRODUCTS
            {
                public class RR_BRAINFLYER
                {
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Brain", nameof(BrainConfig));
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
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Control Module [DEPRECATED]", nameof(AIMODULE));
                    public static LocString DESC = "[Module Deprecated]\nPlease replace with one of the new Modules";
                    public static LocString EFFECT = "[Module Deprecated]\nPlease replace with one of the new Modules";
                }
                public class RR_AINOSECONE
                {
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Nosecone", nameof(RR_AINOSECONE));
                    public static LocString DESC = "The big camera lens helps its AI with navigation.";
                    public static LocString EFFECT = "Will pilot a rocket without any duplicant interaction.\n\nFunctions as a Command Module an a Nosecone.\n\nOne Command Module may be installed per rocket.\n\nMust be built at the top of a Rocket\n\nWon't allow any duplicants inside";
                }
                public class RR_EARLYGAMEAICONTROLMODULE
                {
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Control Module", nameof(RR_EARLYGAMEAICONTROLMODULE));
                    public static LocString DESC = "Piloting a rocket, now without duplicants.";
                    public static LocString EFFECT = "Will pilot a rocket without any duplicant interaction.\n\nFunctions as a Command Module.\n\nOne Command Module may be installed per rocket.\n\nWon't allow any duplicants inside";
                }
                public class RR_AILEARNINGCONTROLMODULE
                {
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Brain Module [OLD]", nameof(RR_AINOSECONE));
                    public static LocString DESC = "Piloting is fun, even for an artificial brain!";
                    public static LocString EFFECT = "Requires a " + MISC.TAGS.RR_SPACEBRAINFLYER + " to function.\nBrains can be made at the " + global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.NAME + "\nA brain will pilot the rocket and slowly gain piloting experience, resulting in faster flight times.\n\nFunctions as a Command Module.\n\nOne Command Module may be installed per rocket.\n\nWon't allow any duplicants inside";
                }
                public class RR_AILEARNINGCONTROLMODULEV2
                {
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Brain Module", nameof(RR_AINOSECONE));
                    public static LocString DESC = "Piloting is fun, even for an artificial brain!";
                    public static LocString EFFECT = "Requires a " + MISC.TAGS.RR_SPACEBRAINFLYER + " to function.\nBrains can be made at the " + global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.NAME + "\nA brain will pilot the rocket and slowly gain piloting experience, resulting in faster flight times.\n\nFunctions as a Command Module.\n\nOne Command Module may be installed per rocket.\n\nWon't allow any duplicants inside";
                }
                public class RR_AICONTROLMODULE
                {
                    //public static LocString DisplayName = "AI Control Module [DEPRECATED]";
                    //public static LocString Description = "A Module that controls your Rocket without any duplicant input.";

                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("AI Control Module [DEPRECATED]", nameof(AIControlModuleConfig));
                    public static LocString DESC = "[Module Deprecated]\nPlease replace with one of the new Modules";
                    public static LocString EFFECT = "[Module Deprecated]\nPlease replace with one of the new Modules";
                }
            }
        }

        public class RESEARCH
        {

            public class TECHS
            {
                
                public class RR_BRAINMODULETECH
                {
                    public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Artificial Learning Capabilities", nameof(RR_BRAINMODULETECH));
                    public static LocString DESC = "Grow a brain and let it reach for the stars";
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
                    public static LocString TOOLTIP = "{BRAINNAME} is a {BRAINXPSTATE} ({BRAINBOOST}% total rocket speed)";
                    public static LocString UNNAMEDBRAIN = "This Brain";
                    public static LocString NOBRAIN = "This AI-Module";
                    public static LocString LVLNONE = (LocString)global::STRINGS.UI.FormatAsLink("Automated Pilot", nameof(LVLNONE));
                    public static LocString LVL1 = (LocString)global::STRINGS.UI.FormatAsLink("Novice Pilot", nameof(LVL1));
                    public static LocString LVL2 = (LocString)global::STRINGS.UI.FormatAsLink("Advanced Beginner Pilot",nameof(LVL2));
                    public static LocString LVL3 = (LocString)global::STRINGS.UI.FormatAsLink("Competent Pilot", nameof(LVL3));
                    public static LocString LVL4 = (LocString)global::STRINGS.UI.FormatAsLink("Proficient Pilot", nameof(LVL4));
                    public static LocString LVL5 = (LocString)global::STRINGS.UI.FormatAsLink("Expert Pilot", nameof(LVL5));
                    public static LocString LVL6 = (LocString)global::STRINGS.UI.FormatAsLink("Master Pilot", nameof(LVL6));
                }
                public class RR_NOBRAIN
                {
                    public static LocString NAME = "No brain installed!";
                    public static LocString TOOLTIP = "This Module is missing a brain!";
                }
            }
        }
    }
}
