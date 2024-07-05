using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyRoutine
{
    class STRINGS
    {
        public class UISTRINGS
        {
            public static LocString TIMELEFTTEXT = "Time when the tasks reset every day (in seconds):";
            public class TOGGLECUSTOMTIME
            {
                public static LocString LABEL = "Toggle Custom Time";
                public static LocString TOOLTIP = "By default, the recipes reset at\nthe start of each cycle.\nToggle to adjust the time of that reset.";
            }
            public class TOGGLEQUEUEING
            {
                public static LocString LABEL = "Toggle Recipe Queueing";
                public static LocString TOOLTIP = "When disabled, the recipe count for each recipe will be set to the stored value regardless of remaining recipe count.\nEnabling this option instead adds the amount to the existing count instead.";
            }
            public class REFRESHDAILYTASKS
            {
                public static LocString LABEL = "Update Daily Tasks\n(Hover to see current)";
            }
            public class FORMATTEDCOUNTEXT
            {
                public static LocString LABEL = "{0} items set as daily task.";
                public static LocString TOOLTIP = "{0} items over {1} recipes set as daily tasks";
                public static LocString NONE = "No recipes queued.";
            }
            public class FORMATTEDRECIPETEXT
            {
                public static LocString LABEL = "Daily Routine Recipes:";
                public static LocString NONE = "No recipes queued.";
            }
        }
    }
}
