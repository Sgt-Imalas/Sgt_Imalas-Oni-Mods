using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintsV2
{
    public class STRINGS
    {
        public class UI
        {
            public class DIALOGE
            {
                public class CONFIRMDELETE
                {
                    public static LocString TITLE = "Confirm Delete";
                    public static LocString TEXT = "Do you really want to delete the blueprint {0}?";
                }
            }

            public class BLUEPRINTSELECTOR
            {
                public class FILEHIERARCHY
                {
                    public class SEARCHBAR
                    {
                        public static LocString CLEARTOOLTIP = "Clear search bar";
                        public static LocString OPENFOLDERTOOLTIP = "Open the folder where the Blueprints are stored.";
                        public class INPUT
                        {
                            public class TEXTAREA
                            {
                                public static LocString PLACEHOLDER = "Enter text to filter Blueprints...";
                                public static LocString TEXT = "";
                            }
                        }
                    }
                    public class SCROLLAREA
                    {
                        public class CONTENT
                        {
                            public class FOLDERUP
                            {
                                public static LocString LABEL = "Go to parent folder";
                            }
                            public class NONEAVAILABLE
                            {
                                public static LocString LABEL = "No Blueprints available";
                            }
                            public class BLUEPRINTENTRY
                            {
                                public static LocString TOOLTIP_DELETE = "Delete Blueprint.\nDeleting the last blueprint in a sub folder will also delete the folder.";
                                public static LocString TOOLTIP_MOVE = "Move Blueprint to other folder.\nWill create new folder if it does not exist\nleaving the new folder name empty will move the blueprint to the main folder";
                                public static LocString TOOLTIP_RENAME = "Rename Blueprint";
                            }
                        }
                    }
                }
                public class MATERIALSWITCH
                {
                    public class MATERIALSHEADER
                    {
                        public static LocString LABEL = "Materials in Blueprint:";
                    }
                    public class SCROLLAREA
                    {
                        public class CONTENT
                        {
                            public class NOELEMENTSINBLUEPRINT
                            {
                                public static LocString LABEL = "No elements in blueprint";
                            }
                        }
                    }
                    public class BUTTONS
                    {
                        public class RESETBUTTON
                        {
                            public static LocString TEXT = "Reset Replacements";
                        }
                        public class PLACEBPBTN
                        {
                            public static LocString TEXT = "Place Blueprint";
                        }
                    }

                }
                public class MATERIALREPLACER
                {
                    public class TOREPLACE
                    {
                        public static LocString LABEL = "To replace:";
                    }
                    internal class SEARCHBAR
                    {
                        public static LocString CLEARTOOLTIP = "Clear search bar";
                        internal class INPUT
                        {
                            public class TEXTAREA
                            {
                                public static LocString PLACEHOLDER = "Enter text to filter elements...";
                                public static LocString TEXT = "";
                            }
                        }
                    }
                    public class SCROLLAREA
                    {
                        public class CONTENT
                        {
                            public class NOREPLACEELEMENTS
                            {
                                public static LocString LABEL = "No suitable elements for replacement found";
                            }
                        }
                    }
                }
                public class CLOSEBUTTON
                {
                    public static LocString TEXT = "Close";
                }
            }
            public class ACTIONS
            {
                public static LocString CREATE_TITLE = "Create Blueprint";
                public static LocString USE_TITLE = "Use Blueprint";
                public static LocString CREATEFOLDER_TITLE = "Create Folder";
                public static LocString RENAME_TITLE = "Rename Blueprint";
                public static LocString CYCLEFOLDERS_NEXT_TITLE = "Next Folder";
                public static LocString CYCLEFOLDERS_PREV_TITLE = "Previous Folder";
                public static LocString CYCLEBLUEPRINTS_NEXT_TITLE = "Next Blueprint";
                public static LocString CYCLEBLUEPRINTS_PREV_TITLE = "Previous Blueprint";
                public static LocString SNAPSHOT_TITLE = "Take Snapshot";
                public static LocString DELETE_TITLE = "Delete Blueprint/Snapshot";
            }
            public class TOOLS
            {
                public class CREATE_TOOL
                {
                    public static LocString NAME = "New Blueprint";
                    public static LocString TOOLTIP = "Create blueprint {0}";
                    public static LocString EMPTY = "Blueprint would have been empty!";
                    public static LocString CREATED = "Created blueprint!";
                    public static LocString CANCELLED = "Cancelled blueprint!";
                    public static LocString TOOLTIP_TITLE = "CREATE BLUEPRINT TOOL";
                    public static LocString ACTION_DRAG = "DRAG";
                    public static LocString ACTION_BACK = "BACK";
                }
                public class USE_TOOL
                {
                    public static LocString NAME = "Use Blueprint";
                    public static LocString TOOLTIP = "Use blueprint {0}";
                    public static LocString LOADEDBLUEPRINTS = "Loaded {0} blueprints! ({1} total)";
                    public static LocString LOADEDBLUEPRINTS_ADDITIONAL = "additional";
                    public static LocString LOADEDBLUEPRINTS_FEWER = "fewer";
                    public static LocString TOOLTIP_TITLE = "USE BLUEPRINT TOOL";
                    public static LocString ACTION_CLICK = "CLICK";
                    public static LocString ACTION_BACK = "BACK";
                    public static LocString CYCLEFOLDERS = "Use {0} and {1} to cycle folders.";
                    public static LocString CYCLEBLUEPRINTS = "Use {0} and {1} to cycle blueprints.";
                    public static LocString FOLDERBLUEPRINT = "Press {0} to assign folder.";
                    public static LocString FOLDERBLUEPRINT_NA = "Same folder provided - no change made.";
                    public static LocString MOVEDBLUEPRINT = "Moved \"{0}\" to \"{1}\"";
                    public static LocString NAMEBLUEPRINT = "Press {0} to rename blueprint.";
                    public static LocString DELETEBLUEPRINT = "Press {0} to delete blueprint.";
                    public static LocString ERRORMESSAGE = "This blueprint contained {0} misconfigured or missing prefabs which have been omitted!";
                    public static LocString SELECTEDBLUEPRINT = "Selected \"{0}\" ({1}/{2}) from \"{3}\" ({4}/{5})";
                    public static LocString FOLDEREMPTY = "Selected folder \"{0}\" is empty!";
                    public static LocString NOBLUEPRINTS = "No blueprints loaded!";
                }
                public class SNAPSHOT_TOOL
                {
                    public static LocString NAME = "Take Snapshot";
                    public static LocString TOOLTIP = "Take snapshot {0} \n\nCreate a blueprint and quickly place it elsewhere while not cluttering your blueprint collection! \nSnapshots do not persist between games or worlds.";
                    public static LocString EMPTY = "Snapshot would have been empty!";
                    public static LocString TAKEN = "Snapshot taken!";
                    public static LocString TOOLTIP_TITLE = "SNAPSHOT TOOL";
                    public static LocString ACTION_CLICK = "CLICK";
                    public static LocString ACTION_DRAG = "DRAG";
                    public static LocString ACTION_BACK = "BACK";
                    public static LocString NEWSNAPSHOT = "Press {0} to take new snapshot.";
                }
                public static LocString NAMEBLUEPRINT_TITLE = "NAME BLUEPRINT";
                public static LocString FOLDERBLUEPRINT_TITLE = "ASSIGN FOLDER";
                public class FILTERLAYERS
                {
                    public static LocString GASTILES = "Gas Tiles";
                    public static LocString ALL = "All";
                    public static LocString NONE = "None";
                    public static LocString BLUEPRINTV2_PRESERVEAIRTILES = "Air Pockets";
                }
            }
        }
    }
}
