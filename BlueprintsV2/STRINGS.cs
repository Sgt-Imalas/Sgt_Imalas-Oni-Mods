using PeterHan.PLib.Options;

namespace BlueprintsV2
{
	public class STRINGS
	{
		public class BLUEPRINTS_CONFIG
		{
			public class DEFAULTMENUSELECTION
			{
				public static LocString TITLE = "Default Menu Selections";
				public static LocString TOOLTIP = "The default selections made when an advanced filter menu is opened.";
				public static LocString DEFAULTMENUSELECTION_ALL = "All";
				public static LocString DEFAULTMENUSELECTION_NONE = "None";
			}
			public class REQUIRECONSTRUCTABLE
			{
				public static LocString TITLE = "Require Constructable";
				public static LocString TOOLTIP = "Whether buildings must be constructable by the player to be used in blueprints.";
			}
			public class FXTIME
			{
				public static LocString TITLE = "FX Time";
				public static LocString TOOLTIP = "How long FX created by Blueprints remain on the screen. Measured in seconds.";
			}
			public class CREATEBLUEPRINTTOOLSYNC
			{
				public static LocString TITLE = "Blueprint Tool Overlay Sync";
				public static LocString TOOLTIP = "Whether the Blueprint Tool syncs with the current overlay. (configurable in game too)";
			}
			public class SNAPSHOTTOOLSYNC
			{
				public static LocString TITLE = "Snapshot Tool Overlay Sync";
				public static LocString TOOLTIP = "Whether the Snapshot Tool syncs with the current overlay. (configurable in game too)";
			}
		}
		public class UI
		{
			public class DIALOGUE
			{
				public class CONFIRMDELETE
				{
					public static LocString TITLE = "Confirm Delete";
					public static LocString TEXT = "Do you really want to delete the blueprint {0}?";
				}
				public static LocString NAMEBLUEPRINT_TITLE = "NAME BLUEPRINT";
				public static LocString FOLDERBLUEPRINT_TITLE = "ASSIGN FOLDER";
				public static LocString MOVETOFOLDER_TITLE = "MOVE TO FOLDER";
				public static LocString RENAMEBLUEPRINT_TITLE = "RENAME BLUEPRINT";
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
								public static LocString TOOLTIP_MOVE = "Move Blueprint to other folder.\nWill create new folder if it does not exist.\nLeaving the new folder name empty will move the blueprint to the main folder.";
								public static LocString TOOLTIP_RENAME = "Rename Blueprint";
							}
						}
					}
				}
				public class MATERIALSWITCH
				{
					public static LocString WARNING = "Some materials in the blueprint require more mass than available on the current asteroid!";
					public static LocString WARNINGSEVERE = "Some materials in the blueprint are not unlocked yet!";
					public class MATERIALSHEADER
					{
						public static LocString LABEL = "Materials in \"{0}\":";
					}
					public class SCROLLAREA
					{
						public class CONTENT
						{
							public class NOELEMENTSINBLUEPRINT
							{
								public static LocString LABEL = "No elements in blueprint";
							}
							public class PRESETENTRYPREFAB
							{
								public static LocString MASSTEXT = "Total Mass:";
								public static LocString LABEL = "{0} ( as {1})";
								public static LocString LOCALLYREPLACE = "Replace Locally: ";
								public static LocString NONE = "No";
							}
						}
					}
					public class PERBUILDINGOVERRIDES
					{
						public static LocString LABEL = "Advanced Material Overrides:";
						public static LocString TOOLTIP = "Enabling this allows selecting replacement materials per building type.";
					}
					public class BUTTONS
					{
						public class CREATEMODIFIED
						{
							public static LocString TOOLTIP = "Create a copy of the current blueprint with all local material replacements applied to it.";
						}
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
						public static LocString LABEL = "Material Type:";
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
								public static LocString LABEL = "No suitable elements found";
							}
							public class ELEMENTSTATE
							{
								public static LocString NOTFOUND = "This material has not been found in the current game!";
								public static LocString NOTENOUGH = "You don't have enough of that material on the current asteroid!\nCurrently available: {0}\nRequired when selecting this: {1}";
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
				public static LocString SNAPSHOT_TITLE = "Take Snapshot";
				public static LocString SELECT_DIFFERENT_TITLE = "Select different Blueprint";
				public static LocString CHANGE_ANCHOR_TITLE = "Change Blueprint Anchor";
				public static LocString TOGGLE_FORCE = "Toggle force rebuild";
				public static LocString ROTATE_BLUEPRINT_TITLE = "Rotate Blueprint";
				public static LocString FLIP_BLUEPRINT_TITLE = "FLIP Blueprint";
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
					public static LocString TOOLTIP_TITLE = "USE BLUEPRINT TOOL";
					public static LocString ACTION_CLICK = "PLACE";
					public static LocString ACTION_BACK = "CANCEL";
					public static LocString ACTION_DRAG = "DRAG";

					public static LocString ACTION_SELECT = "Adjust selected blueprint: {0}";
					public static LocString ACTION_CHANGE_ANCHOR = "Change blueprint anchor point: {0}";

					public static LocString ERRORMESSAGE = "This blueprint contained {0} misconfigured or missing prefabs which have been omitted!";
					public static LocString NOBLUEPRINTS = "No blueprints loaded!";
					public static LocString NONESELECTED = "No blueprint selected!";
					public static LocString SELECTEDBLUEPRINT = "Selected blueprint: {0}";

					public static LocString FORCEREBUILD = "Force rebuild existing buildings with blueprint materials if different:\n{0} (hold {1} to enable)";
					public static LocString REBUILD_ACTIVE = "Active";
					public static LocString REBUILD_INACTIVE = "Inactive";


					public static LocString SETTINGS_APPLIED = "settings applied!";

				}
				public class SNAPSHOT_TOOL
				{
					public static LocString NAME = "Take Snapshot";
					public static LocString TOOLTIP = "Take snapshot {0} \n\nCreate a blueprint and quickly place it elsewhere while not cluttering your blueprint collection! \nSnapshots do not persist between games or worlds.";
					public static LocString EMPTY = "Snapshot would have been empty!";
					public static LocString TAKEN = "Snapshot taken!";
					public static LocString TOOLTIP_TITLE = "SNAPSHOT TOOL";
					public static LocString NEWSNAPSHOT = "Press {0} to take new snapshot.";
				}
				public class FILTERLAYERS
				{
					public static LocString ALL = "All";
					public static LocString NONE = "None";
					public static LocString AUTOSYNC = "Auto. Sync";
					public class BLUEPRINTV2_PRESERVEAIRTILES
					{
						public static LocString NAME = "Air Pockets";
						public static LocString TOOLTIP = "Dig commands on tiles that are not solid";
					}
				}
			}
		}
	}
}
