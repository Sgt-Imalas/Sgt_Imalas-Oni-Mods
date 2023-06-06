using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DupePrioPresetManager
{
    internal class STRINGS
    {
        /// <summary>
        /// Integration help for "Engineer Build Priority" since that mod doesnt properly register its strings
        /// </summary>
        public class DUPLICANTS
        {
            public class CHOREGROUPS
            {
                public class ENGBUILD
                {
                    public static LocString NAME = (LocString)"Engineer Building";
                    public static LocString DESC = (LocString)"Construct buildings requiring engineering.";
                }
            }
        }
        public static LocString UNNAMEDPRESET = "Unnamed Preset";
        public class UI
        {
            public class SCHEDULECLONER
            {
                public class TITLE
                {
                    public static LocString TITLETEXT = "Schedule Offsets";
                }
                public class CONTENT
                {
                    public class CLONEOFFSET
                    {
                        public class OFFSET
                        {
                            public class INPUT
                            {
                                public class TEXTAREA
                                {
                                    public static LocString PLACEHOLDER = "";
                                }
                            }
                        }
                        public static LocString LABEL = "Schedule Offset:";
                    }
                    public class CLONEBUTTON
                    {
                        public static LocString LABEL = "Create Clone from Offset";
                    }
                    public class CANCEL
                    {
                        public static LocString LABEL = "Cancel";
                    }
                    public class APPLYBUTTON
                    {
                        public static LocString LABEL = "Apply Offset";
                    }
                }
            }
            public class PRESETWINDOWDUPEPRIOS
            {
                public class DELETEWINDOW
                {
                    public static LocString TITLE = "Delete {0}";
                    public static LocString DESC = "You are about to delete the stat preset \"{0}\".\nDo you want to continue?";
                    public static LocString YES = "Confirm Deletion";
                    public static LocString CANCEL = "Cancel";

                }
                public static LocString OPENSHIFTCLONE = "Shift/Clone Schedule";

                public static LocString OPENPRESETWINDOW = "Open Preset Window";
                public static LocString TITLE = "Priority Presets";
                public static LocString TITLECONSUMABLES = "Consumable Presets";
                public static LocString TITLESCHEDULES = "Schedule Presets";
                public class SCHEDULESTRINGS
                {
                    public static LocString TITLE = "Delete {0}";
                    public static LocString DESC = "You are about to delete the stat preset \"{0}\".\nDo you want to continue?";
                    public static LocString YES = "Confirm Deletion";

                    public static LocString DEFAULTYES = "Enabled as default.";
                    public static LocString DEFAULTNO = "Not enabled as default.";

                    public static LocString MARKEDASDEFAULT = "Marked as default?";
                    public static LocString MARKEDASDEFAULTTOOLTIP = "Schedules marked as default get generated when the respective button on the schedule screen is clicked.";
                    public static LocString GENERATEALL = "Generate all default schedules";
                    public static LocString GENERATEALLTOOLTIP = "Generate all schedules from presets that are marked as default.\nSchedules that already exist by name wont be generated.";


                    public static LocString GENERATEALLCONFIRM = "Generate {0} new schedules based on your configured default-presets?";
                    public static LocString OMITNUMBER = "\n({0} of your configured default-presets already exist by name and will thus be omitted)";
                    public static LocString ALLGENERATED = "All schedules marked as default-presets already exist by name";
                }

                public class HORIZONTALLAYOUT
                {
                    public class OBJECTLIST
                    {
                        public class SCROLLAREA
                        {
                            public class CONTENT
                            {
                                public class NOPRESETSAVAILABLE
                                {
                                    public static LocString LABEL = "No presets available.";
                                }
                                public class PRESETENTRYPREFAB
                                {
                                    public class ADDTHISTRAITBUTTON
                                    {
                                        public static LocString TEXT = "Load Preset";
                                    }

                                    public static LocString RENAMEPRESETTOOLTIP = "Rename Preset";
                                    public static LocString DELETEPRESETTOOLTIP = "Delete Preset";

                                }
                            }
                        }

                        internal class SEARCHBAR
                        {
                            public static LocString CLEARTOOLTIP = "Clear search bar";
                            public static LocString OPENFOLDERTOOLTIP = "Open the folder where the presets are stored.";
                            internal class INPUT
                            {
                                public class TEXTAREA
                                {
                                    public static LocString PLACEHOLDER = "Enter text to filter presets...";
                                    public static LocString TEXT = "";
                                }
                            }
                        }
                    }
                    public class ITEMINFO
                    {
                        public class BUTTONS
                        {
                            public class CLOSEBUTTON
                            {
                                public static LocString TEXT = "Return";
                                public static LocString TOOLTIP = "Close this preset window.";
                            }
                            public class GENERATEFROMCURRENT
                            {
                                public static LocString TEXT = "Generate Preset";
                                public static LocString TOOLTIP = "Generate and store a new preset from the currenly displayed priorities.\nOnly has an effect if the preset doesn't exist yet";
                            }
                            public class APPLYPRESETBUTTON
                            {
                                public static LocString TEXT = "Apply Preset";
                                public static LocString TOOLTIP = "Apply the currenly displayed priorities to the Duplicant this window was opened from.";
                            }
                        }
                    }
                }
            }
        }
    }
}
