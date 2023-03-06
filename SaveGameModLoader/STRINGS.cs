
namespace SaveGameModLoader
{
    public class STRINGS
    {
        public class UI
        {
            public class FRONTEND
            {
                public class MODSYNCING
                {
                    public static LocString CONTINUEANDSYNC = "SYNC AND RESUME";

                    public static LocString SYNCMODSBUTTONBG = "SYNC MODS";
                    public static LocString SYNCALL = "SYNC ALL AND LOAD";
                    public static LocString MODDIFFS = "MOD DIFFERENCES";
                    public static LocString SYNCSELECTED = "SYNC CURRENT CONFIG AND LOAD";
                    public static LocString MISSINGMOD = "MISSING MODS!";
                    public static LocString ALLSYNCED = "ALL MODS SYNCED, CLOSE";

                    public static LocString EXPORTMODLISTCONFIRMSCREEN = "Export Mod Profile";
                    public static LocString IMPORTMODLISTCONFIRMSCREEN = "Paste collection link to import Mod Profile";

                    public static LocString MISSINGMODSTITLE = "MISSING MODS";
                }
                public class MODLISTVIEW
                {
                    public static LocString MODLISTSBUTTON = "MOD PROFILES";
                    public static LocString MODLISTSBUTTONINFO = "View and manage all your mod profiles";

                    public static LocString MODLISTWINDOWTITLE = "Mod Profiles";

                    public static LocString IMPORTCOLLECTIONLIST = "Import Mod Profile";
                    public static LocString IMPORTCOLLECTIONLISTTOOLTIP = "Create a mod profile by importing a Steam collection";

                    public static LocString EXPORTMODLISTBUTTON = "Export Mod Profile";
                    public static LocString EXPORTMODLISTBUTTONINFO = "Export your current mod config to a file.";
                    public static LocString OPENMODLISTFOLDERBUTTON = "Open Mod Profile Folder";
                    public static LocString OPENMODLISTFOLDERBUTTONINFO = "Open the Mod Profile Folder to view all stored mod profles";

                    public static LocString MODLISTSTANDALONEHEADER = "Created & Imported Mod Profiles";
                    public static LocString MODLISTSAVEGAMEHEADER = "Mod Profiles from Save Games";

                    public static LocString IMPORTEDTITLEANDAUTHOR = "{0}, Collection by {1}";

                    public class SINGLEENTRY
                    {
                        public static LocString ONESTOREDLIST = " stored Profiles";
                        public static LocString MORESTOREDLISTS = " stored Profiles";
                        public static LocString LATESTCOUNT = "Latest Version contains {0} Mods.";
                    }

                    public class POPUP
                    {
                        public static LocString ERRORTITLE = "Could not import Collection";
                        public static LocString WRONGFORMAT = "Not a valid Collection link";
                        public static LocString PARSINGERROR = "Could not determine the workshop ID";
                        public static LocString STEAMINFOERROR = "Error parsing steam data";
                        public static LocString SUCCESSTITLE = "Import successful";
                        public static LocString ADDEDNEW = "Successfully imported Mod Profile:\n{0}";

                    }
                }

                public class SINGLEMODLIST
                {
                    public static LocString TITLE = "Mod Profiles stored in {0}";
                    public static LocString TITLESAVEGAMELIST = "Mod Profiles from SaveGame \"{0}\"";
                    public static LocString MODLISTCOUNT= "Profile contains {0} mods.";
                    public static LocString LOADLIST= "View Profile";
                    public static LocString SYNCLIST = "Apply Profile";
                    public static LocString SUBTOALL = "Subscribe to all missing Mods";
                    public static LocString WORKSHOPFINDTOOLTIP = "Click here to to select workshop action";
                    public static LocString REFRESH = "Refresh View";
                    public static LocString RETURN= "RETURN";
                    public static LocString RETURNTWO= "RETURN TO MODS";
                    public static LocString POPUPSYNCEDTITLE = "Mod List Loaded";
                    public static LocString POPUPSYNCEDTEXT= "The modlist has been applied.";
                    //public static LocString WORKSHOPFINDTOOLTIP= "Click here to subscribe to this mod.";
                    public static LocString NOSTEAMMOD= "Not a Steam Mod!";
                    public static LocString MISSING= "Missing";

                    public static LocString WARNINGMANYMODS = "Large amount of mod differences detected.";
                    public static LocString WARNINGMANYMODSQUESTION = "Large amount of mod differences can lead to long game freezes.\nRestart game with this Modlist applied?\n(This is an alternative mod list deployment mode).";
                    public static LocString USEALTERNATIVEMODE = "Load mod list with a restart\n(Restarts game)";
                    public static LocString USENORMALMETHOD = "Load mod list using normal method\n(could freeze the game)";
                    public class WORKSHOPACTIONS
                    {
                        public static LocString BUTTON = "Workshop Actions";
                        public static LocString TITLE = "Steam Workshop Integration";
                        public static LocString INFO = "This mod is currently missing from the game.\nThe following actions are available:";
                        public static LocString INFOLIST = "This config is currently missing {0} mods from the game.\nThe following actions are available:";

                        public static LocString SUB= "Subscribe to this mod";
                        public static LocString SUBLIST = "Subscribe to all missing mods";
                        public static LocString VISIT = "Visit Workshop";
                    }

                }

                
            }
        }
    }
}
