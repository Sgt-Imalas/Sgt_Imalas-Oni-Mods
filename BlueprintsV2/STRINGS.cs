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
