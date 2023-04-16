using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes
{
    public class STRINGS
    {
        public class UI
        {
            public class PRESETWINDOW
            {
                public class DELETEWINDOW
                {
                    public static LocString TITLE = "Delete {0}";
                    public static LocString DESC = "You are about to delete the dupe preset \"{0}\".\nDo you want to continue?";
                    public static LocString YES = "Confirm Deletion";
                    public static LocString CANCEL = "Cancel";

                }


                public static LocString TITLE = "Presets";
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
                                }
                            }
                        }

                        internal class SEARCHBAR
                        {
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
                            }
                            public class GENERATEFROMCURRENT
                            {
                                public static LocString TEXT = "Generate Preset";
                            }
                            public class APPLYPRESETBUTTON
                            {
                                public static LocString TEXT = "Apply Preset";
                            }
                        }
                    }
                }
            }
            public class MODDEDIMMIGRANTSCREEN
            {
                public static LocString SELECTYOURLONECREWMAN = "CHOOSE YOUR LONE DUPLICANT TO BEGIN";
                public static LocString SELECTYOURCREW = "CHOOSE YOUR DUPLICANTS TO BEGIN";
            } 
            public class DUPESETTINGSSCREEN
            {
                public static LocString APTITUDEENTRY = "{0}, ({1} +{2})";
                public static LocString TRAIT = "{0}";
                public static LocString JOYREACTION = "Overjoyed: {0}";
                public static LocString STRESSREACTION = "Stress Reaction: {0}";

            }
            public class BUTTONS
            {
                public static LocString CYCLENEXT = "Cycle Next";
            }
        }
    }
}
