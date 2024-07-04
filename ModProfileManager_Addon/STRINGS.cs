using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModProfileManager_Addon
{
    internal class STRINGS
    {
        public class UI
        {
            public static LocString LOCAL_MOD = "Local";
            public static LocString PRESET_APPLIED_TITLE = "MODS PRESET APPLIED";
            public static LocString PLIB_CONFIG_FOUND = "Modified Mod Config detected:";
            public class PRESETOVERVIEW
            {
                public class TOPBAR
                {
                    public static LocString LABEL = "Mod Preset Manager";
                }
                public class PRESETHIERARCHYENTRY
                {
                    public static LocString TOOLTIP_EDIT = "Edit Preset";
                    public static LocString TOOLTIP_DELETE = "Delete Preset";
                    public static LocString TOOLTIP_RENAME = "Rename Preset";
                }
                public class DELETE_POPUP
                {
                    public static LocString TITLE = "Deleting Preset";
                    public static LocString TEXT = "Confirm to delete Preset \"{0}\"?";
                }
                public class RENAME_POPUP
                {
                    public static LocString TITLE = "Renaming Preset";
                }
                public class CREATE_POPUP
                {
                    public static LocString TITLE = "Create New Preset";
                }
                public class FILEHIERARCHY
                {
                    public class SAVEBUTTON
                    {
                        public static LocString TEXT = "Create New Preset";
                    }
                    public class SEARCHBAR
                    {
                        public static LocString CLEARTOOLTIP = "Clear search bar";
                        public static LocString OPENFOLDERTOOLTIP = "Open the folder where the Presets are stored.";
                        public class INPUT
                        {
                            public class TEXTAREA
                            {
                                public static LocString PLACEHOLDER = "Enter text to filter Presets...";
                                public static LocString TEXT = "";
                            }
                        }
                    }
                }
                public class MODENTRYVIEW
                {
                    public class APPLYPRESET
                    {
                        public static LocString TEXT = "Apply Preset";
                    }
                    internal class SEARCHBAR
                    {
                        public static LocString CLEARTOOLTIP = "Clear search bar";
                        internal class INPUT
                        {
                            public class TEXTAREA
                            {
                                public static LocString PLACEHOLDER = "Enter text to filter mods...";
                                public static LocString TEXT = "";
                            }
                        }
                    }
                }
            }
        }
    }
}
