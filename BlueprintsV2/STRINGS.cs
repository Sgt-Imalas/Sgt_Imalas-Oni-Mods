using BlueprintsV2.BlueprintData;
using PeterHan.PLib.Options;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2
{
	public class STRINGS
	{
		public class BLUEPRINTS_PLANNED_ELEMENT_PLACER
		{
			public static LocString ELEMENT_INFO_NAME_FILLABLE = "Planned {0} ({1}, {2})";
			public static LocString ELEMENT_INFO_DESC_FILLABLE = "There should be {1} of {0} here, at a temperature of {2}.";

			public static LocString INFO_TITLE = "Planned Element Settings:";

			public class CANCELINDICATOR
			{
				public static LocString NAME = "Cancel Planned Element Info";
				public static LocString TOOLTIP = "Cancel and remove this planned element info";
			}
			public class SANDBOX_SPAWN
			{
				public static LocString NAME = "Spawn Element (Sandbox)";
				public static LocString TOOLTIP = "Spawn the configured element and remove this element info.";
			}
			public class TEMPERATURECONFIG
			{
				public static LocString TITLE = "Element Target Temperature:";
				public static LocString TOOLTIP = "Planned Temperature: {0}";
			}
			public class MASSCONFIG
			{
				public static LocString TITLE = "Element Target Mass:";
				public static LocString TOOLTIP = "Planned Mass: {0}";
			}
		}
		public class BLUEPRINTS_CONFIG
		{
			public class DEFAULTMENUSELECTION
			{
				public static LocString TITLE = "Default Menu Selections";
				public static LocString TOOLTIP = "The default selections made when an advanced filter menu is opened.";
				public static LocString DEFAULTMENUSELECTION_ALL = "All Defaults";
				public static LocString DEFAULTMENUSELECTION_NONE = "None";
			}
			public static LocString EXTRA_DEFAULTS_CATEGORY = "Extra Filter Selections";
			public class REQUIRECONSTRUCTABLE_MATERIAL
			{
				public static LocString TITLE = "Require blueprint building materials";
				public static LocString TOOLTIP = "If enabled, buildings in blueprints are only placed down if their materials are available.";
			}
			public class REQUIRECONSTRUCTABLE_TECH
			{
				public static LocString TITLE = "Require blueprint building tech";
				public static LocString TOOLTIP = "If enabled, buildings in blueprints are only placed down if their required tech has been researched.";
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
			public class PRECONFIGURE_UNDERCONSTRUCTION
			{
				public static LocString TITLE = "Preconfigure building settings";
				public static LocString TOOLTIP = "Use blueprint data transfer to preconfigure this building";
			}
			public class COLOR_LEGEND
			{
				public static LocString BLUEPRINTS_COLOR_VALIDPLACEMENT = "Valid Building Placement";
				public static LocString BLUEPRINTS_COLOR_INVALIDPLACEMENT = "Invalid Building Placement";
				public static LocString BLUEPRINTS_COLOR_NOTECH = "Building not researched";
				public static LocString BLUEPRINTS_COLOR_NOMATERIALS = "Missing Construction Materials";
				public static LocString BLUEPRINTS_COLOR_NOTALLOWEDINWORLD = "Not allowed in current asteroid";
				public static LocString BLUEPRINTS_COLOR_CAN_APPLY_SETTINGS = "Can apply stored blueprint settings";
			}
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

				public class BASE64_EXPORTED
				{
					public static LocString TITLE = "Copied to Clipboard!";
					public static LocString TEXT = "The blueprint has been added to your clipboard.";
				}
				public class BASE64_IMPORT_FAIL
				{
					public static LocString TITLE = "Import Failure!";
					public static LocString TEXT = "Unable to import a blueprint from the current clipboard contents.";
				}
				public class BASE64_IMPORT_SUCCESS
				{
					public static LocString TITLE = "Import Successful!";
					public static LocString TEXT = "The blueprint {0} has been imported successfully.";
				}
			}
			public static LocString BLUEPRINTS_ROOTFOLDER = "Main Folder";

			public class USEBLUEPRINTSTATECONTAINER
			{
				public static LocString ROTATION_BLOCKED = "Cannot rotate the current blueprint.\nThis is prevented by the contained building: {0}";
				public static LocString FLIP_BLOCKED = "Cannot flip the current blueprint.\nThis is prevented by the contained building: {0}";

				public class TITLE
				{
					public static LocString TITLETEXT = "Current Blueprint State";
				}
				public class INFOITEMSCONTAINER
				{
					public class MATERIALREPLACEMENT
					{
						public static LocString LABEL = "Material Overrides in Snapshots:";
						public static LocString TOOLTIP = "Enable material overrides in snapshots.";
					}
					public class MATERIALOVERRIDES
					{
						public class BUTTON
						{
							public static LocString LABEL = "Change Material Overrides";
						}
					}
					public class FOLDERINFO
					{
						public static LocString LABEL = "Folder: {0}, Position: {1}/{2}";
						public static LocString LABEL_SNAPSHOT = "Current Snapshot: {0}/{1}";
					}
					public class APPLYSTOREDSETTINGS
					{
						public static LocString LABEL = "Apply stored building settings on place:";
					}
					public class FORCEREBUILD
					{
						public static LocString LABEL = "Rebuild existing with mismatched material:";
					}
					public class ROTATEACTIONS
					{
						public class ROTATEL
						{
							public static LocString LABEL = "Rotate Left";
						}
						public class ROTATER
						{
							public static LocString LABEL = "Rotate Right";
						}
					}					
					public class FLIPACTIONS
					{
						public class FLIPH
						{
							public static LocString LABEL = "Flip Horizontal";
						}
						public class FLIPV
						{
							public static LocString LABEL = "Flip Vertical";
						}
					}
				}

			}
			public class BLUEPRINTSELECTOR
			{
				public class FILEHIERARCHY
				{
					public class IMPORTBUTTON
					{
						public static LocString TEXT = "Import new blueprint from clipboard";
						public static LocString TOOLTIP = "Import a bluprint string you have stored in your clipboard";
					}
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
								public static LocString TOOLTIP_RETAKE = "Re-Take the blueprint, replacing its content with a new selection of buildings.";
								public static LocString TOOLTIP_EXPORT = "Export the blueprint as a shareable string to your clipboard.";
								public static LocString TOOLTIP_INFO = "Show more detailed info about the blueprint or add some notes to it";
							}
						}
					}
				}
				public class BLUEPRINTINFO
				{
					public class DESCRIPTION
					{
						public static LocString LABEL = "Description:";
						public class INPUT
						{
							public class TEXTAREA
							{
								public static LocString PLACEHOLDER = "Add description or notes to blueprint";
								public static LocString TEXT = "​";
							}
						}
					}
					public class BUTTONS
					{
						public class RESETBUTTON
						{
							public static LocString TEXT = "Reset Changes";
						}
						public class APPLYBUTTON
						{
							public static LocString TEXT = "Save Changes";
						}
					}
					public class STATS
					{
						public static LocString LABEL = "Parameters:";
						public class DIMENSION
						{
							public class DESCRIPTOR
							{
								public static LocString LABEL = "Dimensions:";
							}
						}
						public class BUILDINGCOUNT
						{
							public class DESCRIPTOR
							{
								public static LocString LABEL = "Number of Buildings:";
							}
						}
						public class DIGCOUNT
						{
							public class DESCRIPTOR
							{
								public static LocString LABEL = "Number of non-solid tiles:";
							}
						}
						public class LIQUIDCOUNT
						{
							public class DESCRIPTOR
							{
								public static LocString LABEL = "Number of planned element infos:";
							}
						}
					}
				}
				public class BUILDINGLIST
				{
					public class HEADER
					{
						public static LocString LABEL = "Building List:";
					}
					public class SEARCHBAR
					{
						public class INPUT
						{
							public class TEXTAREA
							{
								public static LocString PLACEHOLDER = "Enter text to filter buildings...";
							}
						}
					}
					public class SCROLLAREA
					{
						public class CONTENT
						{
							public class NOBUILDINGSINBLUEPRINT
							{
								public static LocString LABEL = "Blueprint does not contain any buildings";
							}
							public class BUILDINGENTRYPREFAB
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "BuildingName";
								}
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
					public class SEARCHBAR
					{
						public static LocString CLEARTOOLTIP = "Clear search bar";
						public class INPUT
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
				public static LocString SNAPSHOT_REUSE_TITLE = "Use last Snapshot";
				public static LocString SELECT_DIFFERENT_TITLE = "Select different Blueprint";
				public static LocString CHANGE_ANCHOR_TITLE = "Change Blueprint Anchor";
				public static LocString TOGGLE_FORCE = "Toggle force rebuild";
				public static LocString TOGGLETOOLTIPS = "Toggle keybind tooltips";


				public static LocString SELECT_NEXT = "Next Blueprint";
				public static LocString SELECT_PREV = "Previous Blueprint";
				public static LocString ROTATE_BLUEPRINT = "Rotate Blueprint";
				public static LocString ROTATE_INV_BLUEPRINT = "Reverse Rotate Blueprint";
				public static LocString FLIP_BLUEPRINT_H = "Flip Blueprint Horizontal";
				public static LocString FLIP_BLUEPRINT_V = "Flip Blueprint Vertical";
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
					public static LocString TOOLTIP_TITLE_RETAKE = "RE-TAKE BLUEPRINT TOOL";
					public static LocString RETAKING_TOOLTIP = "Replacing contents of blueprint: {0}";
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
					public static LocString SELECTEDBLUEPRINT = "Selected blueprint: {0} ({1}/{2} in {3})";

					public static LocString FORCEREBUILD = "Replace buildings with mismatched material:\n{0} (hold {1})";
					public static LocString REBUILD_ACTIVE = "Active";
					public static LocString REBUILD_INACTIVE = "Inactive";

					public static LocString SELECTPREV = "Previous blueprint: {0}";
					public static LocString SELECTNEXT = "Next blueprint: {0}";

					public static LocString SETTINGS_APPLIED = "Settings applied!";

					public static LocString ROTATE = "Rotate blueprint: {0}/{1}";
					public static LocString FLIP = "Flip ({0}:{1}/{2}:{3})";
					public static LocString ORIENTATION_H = "Horizontal";
					public static LocString ORIENTATION_V = "Vertical";


					public static LocString TOGGLE_SHOW_HOTKEYS = "Toggle hotkey infos: {0}";

				}

				public class SNAPSHOT_TOOL
				{
					public static LocString NAME = "Take Snapshot";
					public static LocString TOOLTIP = "Take snapshot {0} or reuse last snapshot {1} \n\nCreate a blueprint and quickly place it elsewhere\nwhile not cluttering your blueprint collection!\nSnapshots do not persist between games or worlds.";
					public static LocString EMPTY = "Snapshot would have been empty!";
					public static LocString TAKEN = "Snapshot taken!";
					public static LocString TOOLTIP_TITLE = "SNAPSHOT TOOL";
					public static LocString NEWSNAPSHOT = "Press {0} to take new snapshot.";
					public static LocString REUSELASTSNAPSHOT = "Press {0} to reuse your last taken snapshot.";

					public static LocString SELECTPREV_SNAPSHOT = "Previous snapshot: {0}";
					public static LocString SELECTNEXT_SNAPSHOT = "Next snapshot: {0}";
				}
				public class FILTERLAYERS
				{
					public static LocString ALL = "All";
					public static LocString NONE = "None";
					public static LocString AUTOSYNC = "Auto. Sync to Overlays";
					public static LocString AUTOSYNC_TOOLTIP = "Automatically synchronize the filters with the active overlay";
					public class BLUEPRINTV2_PRESERVEAIRTILES
					{
						public static LocString NAME = "Dig in empty Spaces";
						public static LocString TOOLTIP = "Store Dig commands for tiles that are not solid, but otherwise empty of buildings";
					}
					public class BLUEPRINTV2_STORE_GASSES
					{
						public static LocString NAME = "Store Gas Tile Infos";
						public static LocString TOOLTIP = "Store information about gases in the blueprint area";
					}
					public class BLUEPRINTV2_STORE_LIQUIDS
					{
						public static LocString NAME = "Liquid Tile Infos";
						public static LocString TOOLTIP = "Store information about liquids in the blueprint area";
					}
					public class BLUEPRINTV2_STORE_SOLIDS
					{
						public static LocString NAME = "Solid Tile Infos";
						public static LocString TOOLTIP = "Store information about solid tiles in the blueprint area";
					}
					public class BLUEPRINTV2_PLANNINGTOOL_SHAPES
					{
						public static LocString NAME = "PlanningTool Shapes";
						public static LocString TOOLTIP = "Store PlanningTool-Mod Shapes found in the blueprint area\nOnly works when the PlanningTool mod is enabled.";
					}
					public class BPV2_ELEMENTPLANINFO_FILTER
					{
						public static LocString NAME = "Planned Element Infos";
						public static LocString TOOLTIP = "Planned element tile infos only";
					}
				}
			}
		}
	}
}
