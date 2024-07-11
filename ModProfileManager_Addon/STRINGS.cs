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
            public static LocString MISSING = "Missing";
            public static LocString LOCAL_MISSING_TOOLTIP = "This local mod is currently not installed";
            public static LocString STEAM_MISSING_TOOLTIP = "This steam mod is currently not installed.\nClick here to subscribe to it.";
            public static LocString PRESET_APPLIED_TITLE = "MOD PRESET APPLIED";
            public static LocString PLIB_CONFIG_FOUND = "Modified Mod Options detected:";
            public class PRESETOVERVIEW
            {
                public class TOPBAR
                {
                    public static LocString LABEL = "Mod Preset Manager";
                }
                public class PRESETHIERARCHYENTRY
                {
                    public static LocString TOOLTIP_SELECT = "Select Preset";
                    public static LocString TOOLTIP_CLONE = "This Preset was imported from another mod and cannot be modified.";
                    public static LocString TOOLTIP_DELETE = "Delete Preset";
                    public static LocString TOOLTIP_RENAME = "Rename Preset";
                    public static LocString TOOLTIP_EXPORT = "Export Preset String.";
                }
                public class EXPORT_POPUP
                {
                    public static LocString TITLE = "Exporting Preset";
                    public static LocString TEXT = "The export string has been copied to the clipboard.\nYou can now share it with others";
                }
                public class IMPORT_POPUP
                {
                    public static LocString TITLE = "Importing Preset";
                    public static LocString FILLER = "Paste Preset string here to import";
                    public static LocString TITLE_ERROR = "Error while importing Preset!";
                    public static LocString EMPTY = "The imported preset had no entries.";
                    public static LocString DUPLICATE = "A preset with the name \"{0}\" already exists!\nChoose how to proceed:";
                    public static LocString DUPLICATE_REPLACE = "Replace existing";
                    public static LocString DUPLICATE_KEEP = "Keep both (rename import to {0}_1)";
                    public static LocString ERROR = "There was an error while importing the preset.";
                    public static LocString SUCCESS = "Success!\nThe Preset \"{0}\" got successfully imported";
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
                    public class IMPORTBUTTON
                    {
                        public static LocString TOOLTIP = "Import Preset string";
                    }
                    public class FOLDERBUTTON
                    {
                        public static LocString TOOLTIP = "Open Preset folder";
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
