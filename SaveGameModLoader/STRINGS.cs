
namespace SaveGameModLoader
{
    public class STRINGS
    {
        public class MPM_CONFIG
        {
            public class USECUSTOMFOLDERPATH
            {
                public static LocString NAME = "Use Custom Folder";
                public static LocString TOOLTIP = "Use a custom folder path for mod profiles";
            }
            public class FOLDERPATH
            {
                public static LocString NAME = "Custom Mod Profile Folder";
                public static LocString TOOLTIP = "Defines the folder the mod stores and loads the mod profiles";
            }
            public class FILTERBUTTONS
            {
                public static LocString NAME = "Filter Style";
                public static LocString TOOLTIP = "change the mod menu filter style.";
            }
            public class NEVERDISABLE
            {
                public static LocString NAME = "Keep mod enabled on crash";
                public static LocString TOOLTIP = "any crash during load will wrongfully blame this mod and disable it.\nEnable this option to prevent the mod from getting disabled.";
            }
        }
        public class UI
        {
            public class FRONTEND
            {
                public class FILTERSTRINGS
                {
                    public class DROPDOWN
                    {
                        public static LocString SHOW = "Show: {0}";
                        public static LocString ALL = "All";
                        public static LocString NONE = "None";


                        public static LocString STATE_LABEL = "State Filters:"; //active, inactive, incompatible //, pinned
                        public static LocString PLATFORM_LABEL = "Platform Filters:"; //steam, local , dev


                        public static LocString LOCAL = "Local";
                        public static LocString STEAM = "Steam";
                        public static LocString DEV = "Dev";

                        public static LocString INCOMPATIBLE = "Incompatibles";
                        public static LocString ACTIVE = "Active";
                        public static LocString INACTIVE = "Inactive";
                        public static LocString PINNED = "Pinned";
                    }


                    public static LocString HIDE_INCOMPATIBLE = "Hide Incompatibles";
                    public static LocString UNHIDE_INCOMPATIBLE = "Unhide Incompatibles";
                    public static LocString HIDE_INCOMPATIBLE_TOOLTIP = "Hide or unhide mods that are not compatible\nwith the current version of the game (DLC/BaseGame).";

                    public static LocString HIDE_DEV = "Hide Dev\nMods";
                    public static LocString UNHIDE_DEV = "Unhide Dev Mods";
                    public static LocString HIDE_DEV_TOOLTIP = "Hide or unhide dev mods.";

                    public static LocString HIDE_LOCAL = "Hide Local Mods";
                    public static LocString UNHIDE_LOCAL = "Unhide Local Mods";
                    public static LocString HIDE_LOCAL_TOOLTIP = "Hide or unhide local mods.";

                    public static LocString HIDE_PLATFORM = "Hide Steam Mods";
                    public static LocString UNHIDE_PLATFORM = "Unhide Steam Mods";
                    public static LocString HIDE_PLATFORM_TOOLTIP = "Hide or unhide steam mods.";

                    public static LocString HIDE_ACTIVE = "Hide active Mods";
                    public static LocString UNHIDE_ACTIVE = "Unhide active Mods";
                    public static LocString HIDE_ACTIVE_TOOLTIP = "Hide or unhide active mods.";

                    public static LocString HIDE_INACTIVE = "Hide inactive Mods";
                    public static LocString UNHIDE_INACTIVE = "Unhide inactive Mods";
                    public static LocString HIDE_INACTIVE_TOOLTIP = "Hide or unhide inactive mods.";

                    public static LocString HIDE_PINS = "Hide pinned Mods";
                    public static LocString UNHIDE_PINS = "Unhide pinned Mods";
                    public static LocString HIDE_PINS_TOOLTIP = "Hide or unhide pinned mods.";


                    public static LocString ADJUST_TAG_FILTERS = "Tag Filters";
                }
                public class MODTAGS
                {
                    public class TAGEDITWINDOW
                    {
                        public static LocString TITLE_MOD = "EDIT MOD TAGS";
                        public static LocString TITLE_SELECTOR = "TAG FILTERS";
                        public static LocString FILTERTAGS = "Filter Mod Tags";
                        public static LocString ADDNEWTAGLABEL = "Create new Tag";
                        public static LocString ADDNEWTAGBTN = "Add Tag";
                        public static LocString DELETETAG = "Delete";
                        public static LocString CURRENTTAGS = "Current mod tags:";
                        public static LocString NOTAGS = "No mod tags set";
                        public static LocString ADJUST_TAG_FILTERS_TOOLTIP = "Toggle and manage your mod tag filters.\nCurrently enabled Tag Filters:";
                        public static LocString NOFILTERS = "None";
                        public static LocString ALLFILTERS = "All";
                        public static LocString CLEARFILTERS = "Disable all Tag filters";

                    }
                }
                public class MODSYNCING
                {
                    public static LocString CONTINUEANDSYNC = "SYNC AND RESUME";

                    public static LocString SYNCMODSBUTTONBG = "SYNC MODS";
                    public static LocString SYNCALL = "SYNC AND LOAD SAVE";
                    public static LocString MODDIFFS = "MOD DIFFERENCES";
                    public static LocString SYNCSELECTED = "LOAD WITH CURRENT LOADOUT";
                    public static LocString MISSINGMOD = "MISSING MODS!";
                    public static LocString ALLSYNCED = "ALL MODS SYNCED, CLOSE";

                    public static LocString MISSINGMODSTITLE = "MISSING MODS";



                }
                public class MODLISTVIEW
                {
                    public static LocString COPYTOCLIPBOARD_TOOLTIP = "Copies a list of all active mods to the clipboard.\nDouble click to include their workshop links";
                    public static LocString MODLISTSBUTTON = "MOD PROFILES";
                    public static LocString MODLISTSBUTTONINFO = "View and manage all your mod profiles";

                    public static LocString MODLISTWINDOWTITLE = "Mod Profiles";

                    public static LocString IMPORTCOLLECTIONLIST = "Import Mod Profile";
                    public static LocString IMPORTCOLLECTIONLISTTOOLTIP = "Create a mod profile by importing a Steam collection";

                    public static LocString EXPORTMODLISTBUTTON = "Create Mod Profile";
                    public static LocString EXPORTMODLISTBUTTONINFO = "Create a new profile from your current mod config.";
                    public static LocString OPENMODLISTFOLDERBUTTON = "Open Mod Profile Folder";
                    public static LocString OPENMODLISTFOLDERBUTTONINFO = "Open the Mod Profile Folder to view all stored mod profles";

                    public static LocString MODLISTSTANDALONEHEADER = "Created & Imported Mod Profiles";
                    public static LocString MODLISTSAVEGAMEHEADER = "Mod Profiles from Save Games";

                    public static LocString IMPORTEDTITLEANDAUTHOR = "{0}, Collection by {1}";
                    public class SINGLEENTRY
                    {
                        public static LocString ONESTOREDLIST = " stored Profile";
                        public static LocString MORESTOREDLISTS = " stored Profiles";
                        public static LocString LATESTCOUNT = "Latest Version contains {0} Mods.";
                    }

                    public class POPUP
                    {
                        public static LocString VALIDCOLLECTIONHEADER = "Steam Collection found";
                        public static LocString IMPORTYESNO = "Do you want to import the Steam Collection \"{0}\" with {1} mods in it?";

                        public static LocString VALIDCOLLECTIONHEADER_FILE = "File parsing successful";
                        public static LocString IMPORTYESNO_LOCAL = "Do you want to import the the mod profile from \"{0}\" with {1} mods in it?";




                        public static LocString ERRORTITLE = "Could not import Collection";
                        public static LocString WRONGFORMAT = "Not a valid Collection link";
                        public static LocString PARSINGERROR = "Could not determine the workshop ID";
                        public static LocString STEAMINFOERROR = "Error parsing steam data";
                        public static LocString SUCCESSTITLE = "Import successful";
                        public static LocString ADDEDNEW = "Successfully imported Mod Profile:\n{0} with {1} mods";

                        public static LocString EXPORTMODLISTCONFIRMSCREEN = "Create Mod Profile with {0} mods";
                        public static LocString ENTERNAME = "Enter profile name...";
                        public static LocString ENTERCOLLECTIONLINK = "Enter collection link...";

                        public static LocString IMPORTMODLISTCONFIRMSCREEN = "Paste collection link to import Mod Profile";
                        public static LocString EXPORTCONFIRMATION = "New Mod Profile created!";
                        public static LocString EXPORTCONFIRMATIONTOOLTIP = "Mod Profile \"{0}\" with {1} mods has been created.";

                    }
                }

                public class SINGLEMODLIST
                {
                    public static LocString TITLE = "Mod Profiles stored in {0}";
                    public static LocString TITLESAVEGAMELIST = "Mod Profiles from SaveGame \"{0}\"";
                    public static LocString MODLISTCOUNT= "Profile contains {0} mods.";
                    public static LocString LOADLIST= "View Profile";
                    public static LocString SYNCLIST = "Apply Profile";
                    public static LocString SYNCLISTTOOLTIP = "Applies the Profile.\nThis enables all mods listed in the profile\nand disables all mods not listed in the profile.";

                    public static LocString SYNCLISTADDITIVE = "Make all active";
                    public static LocString SYNCLISTADDITIVETOOLTIP = "This enables all mods stored in the profile\nbut doesnt affect already enabled mods.";

                    public static LocString SUBTOALL = "Subscribe to all missing Mods";
                    public static LocString WORKSHOPFINDTOOLTIP = "Click here to to select workshop action";
                    public static LocString REFRESH = "Refresh View";
                    public static LocString RETURN= "RETURN";
                    public static LocString RETURNTWO= "RETURN TO MODS";
                    public static LocString POPUPSYNCEDTITLE = "Mod List Loaded";
                    public static LocString POPUPSYNCEDTEXTENABLEONLY = "All mods in the list have been enabled.";
                    public static LocString POPUPSYNCEDTEXT= "The modlist has been applied.";
                    //public static LocString WORKSHOPFINDTOOLTIP= "Click here to subscribe to this mod.";
                    public static LocString NOSTEAMMOD= "Not a Steam Mod!";
                    public static LocString MISSING= "Missing";

                    public static LocString WARNINGMANYMODS = "Large amount of mod differences detected.";
                    public static LocString WARNINGMANYMODSQUESTION = "Large amount of mod differences can lead to long game freezes during applying.\nRestart game with this Modlist applied?\n(This is an alternative mod list deployment mode).";
                    public static LocString USEALTERNATIVEMODE = "Load mod list with a restart\n(Restarts game, no freezing involved)";
                    public static LocString USENORMALMETHOD = "Load mod list using normal method\n(could freeze the game during applying)";
                    public class WORKSHOPACTIONS
                    {
                        public static LocString BUTTON = "Workshop Actions";
                        public static LocString TITLE = "Steam Workshop Integration";
                        public static LocString INFO = "This mod is currently missing from the game.\nThe following actions are available:";
                        public static LocString INFOLIST = "This profile contains {0} currently missing mods.\nThe following actions are available:";

                        public static LocString SUB= "Subscribe to this mod";
                        public static LocString SUBLIST = "Subscribe to all missing mods";
                        public static LocString VISIT = "Visit Workshop page";
                    }
                }
            }
        }
    }
}
