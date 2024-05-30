
using BlueprintsV2;
using BlueprintsV2.BlueprintsV2;
using BlueprintsV2.BlueprintsV2.ModAPI;
using BlueprintsV2.BlueprintsV2.Visualizers;
using HarmonyLib;
using ModFramework;
using PeterHan.PLib.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Blueprints
{
    static class BlueprintsStrings
    {
        // Action strings
        public static string ACTION_CREATE_KEY = "Blueprints.create.opentool";
        public static LocString ACTION_CREATE_TITLE = "Create Blueprint";

        public static string ACTION_USE_KEY = "Blueprints.use.opentool";
        public static LocString ACTION_USE_TITLE = "Use Blueprint";

        public static string ACTION_CREATEFOLDER_KEY = "Blueprints.use.assignfolder";
        public static LocString ACTION_CREATEFOLDER_TITLE = "Create Folder";

        public static string ACTION_RENAME_KEY = "Blueprints.use.rename";
        public static LocString ACTION_RENAME_TITLE = "Rename Blueprint";

        public static string ACTION_CYCLEFOLDERS_NEXT_KEY = "Blueprints.use.cyclefolders.next";
        public static LocString ACTION_CYCLEFOLDERS_NEXT_TITLE = "Next Folder";

        public static string ACTION_CYCLEFOLDERS_PREV_KEY = "Blueprints.use.cyclefolders.previous";
        public static LocString ACTION_CYCLEFOLDERS_PREV_TITLE = "Previous Folder";

        public static string ACTION_CYCLEBLUEPRINTS_NEXT_KEY = "Blueprints.use.cycleblueprints.next";
        public static LocString ACTION_CYCLEBLUEPRINTS_NEXT_TITLE = "Next Blueprint";

        public static string ACTION_CYCLEBLUEPRINTS_PREV_KEY = "Blueprints.use.cycleblueprints.previous";
        public static LocString ACTION_CYCLEBLUEPRINTS_PREV_TITLE = "Previous Blueprint";

        public static string ACTION_SNAPSHOT_KEY = "Blueprints.snapshot.opentool";
        public static LocString ACTION_SNAPSHOT_TITLE = "Take Snapshot";

        public static string ACTION_DELETE_KEY = "Blueprints.multi.delete";
        public static LocString ACTION_DELETE_TITLE = "Delete Blueprint/Snapshot";


        // Tool strings
        public static LocString STRING_BLUEPRINTS_CREATE_NAME = "New Blueprint";
        public static LocString STRING_BLUEPRINTS_CREATE_TOOLTIP = "Create blueprint {0}";
        public static LocString STRING_BLUEPRINTS_CREATE_EMPTY = "Blueprint would have been empty!";
        public static LocString STRING_BLUEPRINTS_CREATE_CREATED = "Created blueprint!";
        public static LocString STRING_BLUEPRINTS_CREATE_CANCELLED = "Cancelled blueprint!";
        public static LocString STRING_BLUEPRINTS_CREATE_TOOLTIP_TITLE = "CREATE BLUEPRINT TOOL";
        public static LocString STRING_BLUEPRINTS_CREATE_ACTION_DRAG = "DRAG";
        public static LocString STRING_BLUEPRINTS_CREATE_ACTION_BACK = "BACK";

        public static LocString STRING_BLUEPRINTS_USE_NAME = "Use Blueprint";
        public static LocString STRING_BLUEPRINTS_USE_TOOLTIP = "Use blueprint {0}";
        public static LocString STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS = "Loaded {0} blueprints! ({1} total)";
        public static LocString STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_ADDITIONAL = "additional";
        public static LocString STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_FEWER = "fewer";
        public static LocString STRING_BLUEPRINTS_USE_TOOLTIP_TITLE = "USE BLUEPRINT TOOL";
        public static LocString STRING_BLUEPRINTS_USE_ACTION_CLICK = "CLICK";
        public static LocString STRING_BLUEPRINTS_USE_ACTION_BACK = "BACK";
        public static LocString STRING_BLUEPRINTS_USE_CYCLEFOLDERS = "Use {0} and {1} to cycle folders.";
        public static LocString STRING_BLUEPRINTS_USE_CYCLEBLUEPRINTS = "Use {0} and {1} to cycle blueprints.";
        public static LocString STRING_BLUEPRINTS_USE_FOLDERBLUEPRINT = "Press {0} to assign folder.";
        public static LocString STRING_BLUEPRINTS_USE_FOLDERBLUEPRINT_NA = "Same folder provided - no change made.";
        public static LocString STRING_BLUEPRINTS_USE_MOVEDBLUEPRINT = "Moved \"{0}\" to \"{1}\"";
        public static LocString STRING_BLUEPRINTS_USE_NAMEBLUEPRINT = "Press {0} to rename blueprint.";
        public static LocString STRING_BLUEPRINTS_USE_DELETEBLUEPRINT = "Press {0} to delete blueprint.";
        public static LocString STRING_BLUEPRINTS_USE_ERRORMESSAGE = "This blueprint contained {0} misconfigured or missing prefabs which have been omitted!";
        public static LocString STRING_BLUEPRINTS_USE_SELECTEDBLUEPRINT = "Selected \"{0}\" ({1}/{2}) from \"{3}\" ({4}/{5})";
        public static LocString STRING_BLUEPRINTS_USE_FOLDEREMPTY = "Selected folder \"{0}\" is empty!";
        public static LocString STRING_BLUEPRINTS_USE_NOBLUEPRINTS = "No blueprints loaded!";

        public static LocString STRING_BLUEPRINTS_SNAPSHOT_NAME = "Take Snapshot";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP = "Take snapshot {0} \n\nCreate a blueprint and quickly place it elsewhere while not cluttering your blueprint collection! \nSnapshots do not persist between games or worlds.";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_EMPTY = "Snapshot would have been empty!";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_TAKEN = "Snapshot taken!";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP_TITLE = "SNAPSHOT TOOL";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_ACTION_CLICK = "CLICK";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_ACTION_DRAG = "DRAG";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_ACTION_BACK = "BACK";
        public static LocString STRING_BLUEPRINTS_SNAPSHOT_NEWSNAPSHOT = "Press {0} to take new snapshot.";

        public static LocString STRING_BLUEPRINTS_NAMEBLUEPRINT_TITLE = "NAME BLUEPRINT";
        public static LocString STRING_BLUEPRINTS_FOLDERBLUEPRINT_TITLE = "ASSIGN FOLDER";

        public static LocString STRING_BLUEPRINTS_MULTIFILTER_GASTILES = "Gas Tiles";
        public static LocString STRING_BLUEPRINTS_MULTIFILTER_ALL = "All";
        public static LocString STRING_BLUEPRINTS_MULTIFILTER_NONE = "None";
    }

    public static class BlueprintsAssets
    {
        public static Config Options { get; set; } = new();

        public static Sprite BLUEPRINTS_CREATE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_CREATE_VISUALIZER_SPRITE;

        public static Sprite BLUEPRINTS_USE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_USE_VISUALIZER_SPRITE;

        public static Sprite BLUEPRINTS_SNAPSHOT_ICON_SPRITE;
        public static Sprite BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE;

        public static Color BLUEPRINTS_COLOR_VALIDPLACEMENT = Color.white;
        public static Color BLUEPRINTS_COLOR_INVALIDPLACEMENT = Color.red;
        public static Color BLUEPRINTS_COLOR_NOTECH = new Color32(30, 144, 255, 255);
        public static Color BLUEPRINTS_COLOR_BLUEPRINT_DRAG = new Color32(0, 119, 145, 255);

        public static HashSet<char> BLUEPRINTS_FILE_DISALLOWEDCHARACTERS;
        public static HashSet<char> BLUEPRINTS_PATH_DISALLOWEDCHARACTERS;

        public static HashSet<string> BLUEPRINTS_AUTOFILE_IGNORE = new();
        public static FileSystemWatcher BLUEPRINTS_AUTOFILE_WATCHER;

        static BlueprintsAssets()
        {
            BLUEPRINTS_FILE_DISALLOWEDCHARACTERS = new HashSet<char>();
            BLUEPRINTS_FILE_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidFileNameChars());

            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS = new HashSet<char>();
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidFileNameChars());
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidPathChars());

            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove('/');
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove('\\');
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove(Path.DirectorySeparatorChar);
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove(Path.AltDirectorySeparatorChar);

        }
        public static void AddSpriteToCollection(Sprite sprite)
        {
            if (Assets.Sprites.ContainsKey(sprite.name))
                Assets.Sprites.Remove(sprite.name);
            Assets.Sprites.Add(sprite.name, sprite);
        }
    }


    public struct CellColorPayload
    {
        public Color Color { get; private set; }
        public ObjectLayer TileLayer { get; private set; }
        public ObjectLayer ReplacementLayer { get; private set; }

        public CellColorPayload(Color color, ObjectLayer tileLayer, ObjectLayer replacementLayer)
        {
            Color = color;
            TileLayer = tileLayer;
            ReplacementLayer = replacementLayer;
        }
    }
}