using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralVaccilatorExpanded
{
    internal class STRINGS
    {
        public class DUPLICANTS
        {
            public class STATUSITEMS
            {
                public class NVE_THOUGHTFULLCHATTER
                {
                    public static LocString NAME = (LocString)"Thoughtfull Chatter";
                    public static LocString TOOLTIP = (LocString)"This Duplicant had a very informative conversation.";
                }
            }
            public class TRAITS
            {
                public class NVE_SUPERBRAINS
                {
                    public static LocString NAME = (LocString)"Big Brain";
                    public static LocString DESC = (LocString)"This Duplicant has absorbed a second brain!";
                }
                public class NVE_EXPERTMECHANIC
                {
                    public static LocString NAME = (LocString)"Expert Mechanic";
                    public static LocString DESC = (LocString)"Nuts? Bolts? This Duplicant has them all";
                }
                public class NVE_SHARINGGENIUS
                {
                    public static LocString NAME = (LocString)"Charismatic Genius";
                    public static LocString DESC = (LocString)"Who knew a conversation could be that informative?\n\nLeaves other dupes a bit smarter after a conversation.";
                }
            }
        }
    }
}
