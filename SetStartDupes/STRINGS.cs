﻿using static SetStartDupes.STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.ITEMINFO.BUTTONS;

namespace SetStartDupes
{
	public class STRINGS
	{
		public static LocString BIONIC_STRESS_HARDWIRED = "Bionic duplicants have a hardwired stress reaction since they are mostly incompatible with normal stress reactions";

		public static LocString UNNAMEDPRESET = "(Unnamed Preset)";
		public static LocString MISSINGTRAIT = "Missing Trait!";
		public static LocString MISSINGTRAITDESC = "This Trait could not be found: {0}";
		public static LocString MISSINGSKILLGROUP = "Missing Attribute!";
		public static LocString MISSINGSKILLGROUPDESC = "This Attribute could not be found: {0}";
		public static LocString EXCLUSIVITY_RULE_CONFLICTING = "Mutual exclusivity detected:\nWith vanilla generation rules,\nthis would not be added, as it is mutually exclusive with:";

		public class UI
		{
			public class STARTAGAIN
			{
				public static LocString SIDESCREEN_TEXT = "Reselect start duplicants";
				public static LocString SIDESCREEN_TOOLTIP = "Restart Light";
			}

			public class CAREPACKAGEEDITOR
			{
				public static LocString TITLE = "Care Package Editor";
				public class RESETALLPACKAGES
				{
					public static LocString TITLE = "Reset Care Packages";
					public static LocString TEXT = "Are you sure you want to reset all modifications you have done to the care package selection?";
				}
				public class CREATECAREPACKAGEPOPUP
				{
					public static LocString TITLE = "Care Package Creation";
					public static LocString SUCCESS = "Successfully created a new care package with {0}";
					public static LocString INVALIDID = "Could not create new care package!\n\nThere is no item with the Id or name \"{0}\"";
				}

				public static LocString UNKNOWN_ITEM_TOOLTIP = "The item of this care package could not be found in the current game version.";
				public class UNLOCKCONDITIONTOOLTIPS
				{
					public static LocString ALWAYSAVAILABLE = "This care package is always unlocked";
					public static LocString START = "This care package unlocks after:";
					public static LocString DISCOVERY = "{0} has been discovered";
					public static LocString CYCLETHRESHOLD = "Cycle {0} has been reached";
					public static LocString AND = "AND";
					public static LocString OR = "OR";
				}
				public class HORIZONTALLAYOUT
				{
					public class OBJECTLIST
					{
						public class SHOWVANILLA
						{
							public static LocString LABEL = "Show Vanilla Care Packages:";
							public static LocString TOOLTIP = "Also show vanilla care packages in the list.\nNote that these cannot be modified.";
						}
						public class SCROLLAREA
						{
							public class CONTENT
							{
								public class NONEAVAILABLE
								{
									public static LocString LABEL = "No custom care packages yet";
								}
								public class PRESETENTRYPREFAB
								{
									public static LocString DELETE_CARE_PACKAGE_TOOLTIP = "Delete Care Package";
									public static LocString TOGGLE_VANILLA_CAREPACKAGE = "Toggle this vanilla care package from the care package list.\nDisabled care packages won't show up in the selection list and cannot be rolled.";
								}
							}
						}

						public class SEARCHBAR
						{
							public static LocString CLEARTOOLTIP = "Clear search bar";
							public class INPUT
							{
								public class TEXTAREA
								{
									public static LocString PLACEHOLDER = "Filter care packages...";
									public static LocString TEXT = "";
								}
							}
						}
						public class CAREPACKAGEITEMID
						{
							public class TEXTAREA
							{
								public static LocString PLACEHOLDER = "insert new care package item Id or Name (experimental)...";
								public static LocString TEXT = "";
							}
						}

						public class ADDCAREPACKAGEBTN
						{
							public static LocString TEXT = "Add new custom care package";
							public static LocString TOOLTIP = "Add a new custom care package.\nEnter the id of the item you want to add in the text field above.";
						}
					}
					public class ITEMINFO
					{
						public class SCROLLAREA
						{
							public class CONTENT
							{
								public class AMOUNTINPUT
								{
									public static LocString LABEL = "Amount:";
								}
								public class REQUIREDDLCS
								{
									public static LocString LABEL = "Required Dlcs:";
								}
								public static LocString UNLOCKCONDITIONSLABEL = "Conditions required to unlock:";
								public class UNLOCKATCYCLE
								{
									public static LocString LABEL = "Unlocked at cycle:";
								}
								public class ITEMDISCOVERED
								{
									public static LocString LABEL = "Item Discovered";
								}
							}
						}
						
					}
					
				}
				public class BUTTONS
				{
					public class CLOSEBUTTON
					{
						public static LocString TEXT = "Return";
						public static LocString TOOLTIP = "Close window";
					}
					public class RESETBUTTON
					{
						public static LocString TEXT = "Reset All Changes";
						public static LocString TOOLTIP = "Reset all changes you have made to the care package selection";
					}
				}
			}

			public class DUPEEDITING
			{
				public class CONFIRMATIONDIALOG
				{
					public static LocString TITLE = "Unsaved Changes";
					public static LocString TEXT = "Warning, there are pending changes that have not been applied yet.\nChoose an action on how to proceed:";
					public static LocString APPLYCHANGES = "Apply changes";
					public static LocString DISCARDCHANGES = "Discard changes";
					public static LocString CANCEL = "Cancel";
				}
				public class CATEGORIES
				{
					public class HEADER
					{
						public static LocString LABEL = "Duplicants";
					}
				}
				public class DETAILS
				{
					public class HEADER
					{
						public static LocString LABEL = "Duplicants";
						public static LocString LABEL_FILLED = "Selected Duplicant: {0}";
						public class BUTTONS
						{
							public class ATTRIBUTEBUTTON
							{
								public static LocString TEXT = "Attributes";
								public static LocString TOOLTIP = "";
							}
							public class APPEARANCEBUTTON
							{
								public static LocString TEXT = "Appearance";
								public static LocString TOOLTIP = "";
							}
							public class HEALTHBUTTON
							{
								public static LocString TEXT = "Health";
								public static LocString TOOLTIP = "";
							}
							public class SKILLSBUTTON
							{
								public static LocString TEXT = "Skills";
								public static LocString TOOLTIP = "";
							}
							public class EFFECTSBUTTON
							{
								public static LocString TEXT = "Effects";
								public static LocString TOOLTIP = "";
							}
						}
					}

					public class CONTENT
					{
						public class SCROLLRECTCONTAINER
						{
							public class NEWBUTTONPREFAB
							{
								public static LocString LABEL = "Add new effect";
								public static LocString TOOLTIP = "Choose another effect to add to the duplicant";
							}
							public class SKILLS
							{
								public static LocString EXPERIENCE = "Experience:";
								public static LocString SKILL = "Skill";
								public static LocString MASTERY = "Mastered";
							}
							public class EFFECTS
							{
								public static LocString EFFECT = "Effect";
								public static LocString TIMEREMAINING = "Remaining time in seconds | Remove Effect";
								public static LocString DYNAMIC_EFFECT_TOOLTIP = "This effect has been created dynamically by the game without registering as a resource.\nThus you won't be able to readd it from the list if you decide to remove it.";
							}
							public class TRAITINTERESTCONTAINER
							{
								public class DESCRIPTOR
								{
									public static LocString TRAITLABEL = "Traits:";
									public static LocString INTERESTLABEL = "Interests:";
								}
								public class CONTENT
								{
									public class GRP2
									{
										public class ADDTRAITBUTTON
										{
											public static LocString TEXT = "Add new trait";
											public static LocString TOOLTIP = "Choose another trait to add to the duplicant";
										}
										public class ADDINTERESTBUTTON
										{
											public static LocString TEXT = "Add new interest";
											public static LocString TOOLTIP = "Choose another interest to add to the duplicant";
										}
									}
								}
							}
						}
					}
					public class FOOTER
					{
						public class BUTTONS
						{
							public class EXITBUTTON
							{
								public static LocString TEXT = "Exit";
								public static LocString TOOLTIP = "";
							}
							public class RESETBUTTON
							{
								public static LocString TEXT = "Reset Changes";
								public static LocString TOOLTIP = "";
							}
							public class SAVECHANGESBUTTON
							{
								public static LocString TEXT = "Save Changes";
								public static LocString TOOLTIP = "";
							}
							public class CLEANSLATEBUTTON
							{
								public static LocString TEXT = "Clear Everything";
								public static LocString TOOLTIP = "Sets all the attributes to 0 and clears all skills, interests, effects and traits.\nIntended for sandbox testing to provide a \"clean slate\"";
							}
						}
					}
				}
			}
			public class DSS_OPTIONS
			{
				public class CATEGORIES
				{
					public static LocString A_GAMESTART = "Options for Game Start";
					public static LocString B_PRINTINGPOD = "Options for Printing Pod";
					public static LocString C_EXTRAS = "Options for Traits and Interests";
					public static LocString D_SKINSETTINGS = "Skin Options";
					public static LocString E_UTIL = "Extra Settings";
				}

				public class DUPLICANTSTARTAMOUNT
				{
					public static LocString NAME = "Number of Starting Duplicants";
					public static LocString TOOLTIP = "Choose the amount of duplicants you want to start with.";
				}

				public class STARTUPRESOURCES
				{
					public static LocString NAME = "Extra Starting Resources";
					public static LocString TOOLTIP = "Add some extra startup resources for your additional duplicants.\nOnly goes in effect with more than 3 dupes.\nOnly accounts for extra dupes above 3.";
				}

				public class SUPPORTEDDAYS
				{
					public static LocString NAME = "Supported Days";
					public static LocString TOOLTIP = "Amount of days the extra starting resources should last.\nNo effect if \"Extra Starting Resources\" is disabled.";
				}

				public class MODIFYDURINGGAME_MINIONS
				{
					public static LocString NAME = "Modification of printing pod Duplicants";
					public static LocString TOOLTIP = "Enable this option to add the modify button to printing pod dupes\nWhen disabled, the feature only appears on the starter dupe selection.\nThis also enables the use of presets.";
				}
				public class MODIFYDURINGGAME_CAREPACKAGES
				{
					public static LocString NAME = "Care Package selection menu";
					public static LocString TOOLTIP = "Enable this option allows replacing printing pod care packages from a list of all at that time unlocked care packages.\nEnable Sandbox or Debug to ignore the unlock condition and make ALL packages available to select, regardless of unlock status.";
				}
				public class REROLLDURINGGAME_MINIONS
				{
					public static LocString NAME = "Reroll printing pod duplicants";
					public static LocString TOOLTIP = "Enable this option to add the reroll button to printing pod Duplicants.";
				}
				public class REROLLDURINGGAME_CAREPACKAGES
				{
					public static LocString NAME = "Reroll Care Packages";
					public static LocString TOOLTIP = "Enable this option to add the reroll button to printing pod care packages.";
				}

				public class PRINTINGPODRECHARGETIME
				{
					public static LocString NAME = "Printing pod cooldown time";
					public static LocString TOOLTIP = "Time it takes for the printing pod to recharge for the next print in cycles.\nDefault is 3 cycles.";
				}
				public class PRINTINGPODRECHARGETIMEFIRST
				{
					public static LocString NAME = "First print time";
					public static LocString TOOLTIP = "Time after the printing pod offers its first print, in cycles.\nDefault is 2.5 cycles.";
				}
				public class CAREPACKAGEMULTIPLIER
				{
					public static LocString NAME = "Care Package Multiplier";
					public static LocString TOOLTIP = "Global multiplier for the amount of item amount inside of care packages.\nDefault is 1.";
				}

				public class PAUSEONREADYTOPRING
				{
					public static LocString NAME = "Pause on \"ready to print\"";
					public static LocString TOOLTIP = "Pause the game when the printing pod has recharged for a new print";
				}
				public class MYSTERIOUSMINIONMODE
				{
					public static LocString NAME = "Mysterious Minion Mode";
					public static LocString TOOLTIP = "All Interests and Traits are hidden until printed";
				}
				public class NEWGAMEPLUS
				{
					public static LocString NAME = "NewGame+";
					public static LocString TOOLTIP = "Start your game with dupes you sent through the tear in previous playthroughs.";
				}
				public class FORCEPRINTINGMODEL
				{
					public static LocString NAME = "Print only specific duplicant models";
					public static LocString TOOLTIP = "Force the printing pod to only produce duplicants of the selected model";
				}
				public class CAREPACKAGESONLY
				{
					public static LocString NAME = "Print Care Packages only";
					public static LocString TOOLTIP = "When enabled, the printing pod will only give care packages when more than the the configured number of duplicants are alive.\nOverrides the care package configuration of a save game (no care package difficulty setting is ignored).";
				}
				public class MORECAREPACKAGES
				{
					public static LocString NAME = "Enable Care Package Editor";
					public static LocString TOOLTIP = "Additional types of care packages become available to print.\nThese can be viewed and modified in the care package editor.\nBy default it contains a small selection of items that can potentially become unavailable.\nAlso allows disabling of vanilla care packages.";
				}
				public class CAREPACKAGEEDITOR
				{
					public static LocString NAME = "Open Care Package Editor";
					public static LocString TOOLTIP = "View and modify the different additional care packages or create new ones.";
				}
				public class SORTEDCAREPACKAGES
				{
					public static LocString NAME = "Sorted Care Package Lists";
					public static LocString TOOLTIP = "Care packages listings are sorted alphabetically.\nAffects care package editor and printing pod selection\nCare packages in the editor can be sorted manually when this option is turned off.";
				}
				public class SORTEDPRINTINGPOD
				{
					public static LocString NAME = "Sorted Printing Pod";
					public static LocString TOOLTIP = "Printing pod entries are sorted by their type, dupes come first, then care packages.";
				}
				public class OVERRIDEPRINTERDUPECOUNT
				{
					public static LocString NAME = "Override printing pod dupe count";
					public static LocString TOOLTIP = "Override the number of duplicants offered in the printing pod.\nA value of 0 or lower disables this feature.";
				}
				public class OVERRIDEPRINTERCAREPACKAGECOUNT
				{
					public static LocString NAME = "Override printing pod care package count";
					public static LocString TOOLTIP = "Override the number of care packages offered in the printing pod.\nA value of 0 or lower disables this feature.\nDisabling care packages in the difficulty settings disables this feature.\nNo effect if care packages only mode is active.";
				}

				public class CAREPACKAGESONLYDUPECAP
				{
					public static LocString NAME = "Duplicant Threshold for care packages only";
					public static LocString TOOLTIP = "Only has an effect if \"" + CAREPACKAGESONLY.NAME + "\" is enabled.\nWhile above or at this number of duplicants, the printer will only give care packages.";
				}

				public class CAREPACKAGESONLYPACKAGECAP
				{
					public static LocString NAME = "Number of care packages for care packages only";
					public static LocString TOOLTIP = "Only has an effect if \"" + CAREPACKAGESONLY.NAME + "\" is enabled.\nSet the number of care packages that generate when the cap is in effect.";
				}
				public class LIVEDUPESTATCHANGE
				{
					public static LocString NAME = "Enable Duplicity-style Duplicant Editor";
					public static LocString TOOLTIP = "Edit your active duplicants in a new editor similar to the Duplicity-savegame editor";
				}
				public class LIVEDUPESKINCHANGE
				{
					public static LocString NAME = "Skins for active Duplicants";
					public static LocString TOOLTIP = "Change the Skin of already existing duplicants";
				}
				public class SKINSDOREACTS
				{
					public static LocString NAME = "Skins include reactions";
					public static LocString TOOLTIP = "When applying a skin, the inherited overjoyed response and stress reaction get applied in addition to the skin.";
				}
				public class ADDANDREMOVE
				{
					public static LocString NAME = "Add/Remove Interests and Traits";
					public static LocString TOOLTIP = "Allows to add new or remove existing Traits and Interests on Duplicants.\nCan be overpowered if used excessively.";
				}
				public class DIRECTATTRIBUTEEDITING
				{
					public static LocString NAME = "Direct Attribute Editing";
					public static LocString TOOLTIP = "Allows to directly edit the starting attributes of duplicants.\nCan be overpowered.";
				}
				public class ADDVACCILATORTRAITS
				{
					public static LocString NAME = "Allow adding Vaccilator Traits";
					public static LocString TOOLTIP = "Allows adding Neural-Vaccilator-Traits with the \"Add Trait\" Feature.";
				}
				public class INTERESTPOINTSBALANCING
				{
					public static LocString NAME = "Interest Point Balancing";
					public static LocString TOOLTIP = "Use the vanilla interest point bonus for active interests determined by active traits.\nDeactivate to override this point limit.";
				}
				public class NORMALTRAITSONBIONICS
				{
					public static LocString NAME = "Allow normal traits on Bionic Duplicants";
					public static LocString TOOLTIP = "When active, allows adding regular traits to bionic duplicants.";
				}
				public class NOJOYREACTION
				{
					public static LocString NAME = "Disable Overjoyed Responses";
					public static LocString TOOLTIP = "Deactivates Overjoyed Responses on newly generated duplicants";
				}
				public class NOSTRESSREACTION
				{
					public static LocString NAME = "Disable Stress Reactions";
					public static LocString TOOLTIP = "Deactivates Stress Reactions on newly generated duplicants";
				}
				public class REROLLCRYOPODANDJORGE
				{
					public static LocString NAME = "Hermit/Cryopod Modification";
					public static LocString TOOLTIP = "Allows modifying of the Hermit and Cryopod Duplicants.\n\"Ancient Knowledge\"-Trait no longer takes the slot of a positive trait during generation of these duplicants.";
				}
				public class HERMITSKIN
				{
					public static LocString NAME = "Hermit Skin";
					public static LocString TOOLTIP = "Adds the Hermit to the pool of selectable skins\nonce the story trait has been completed atleast once on any savegame.";
				}

			}
			public class DUPESKILLSPOPUP
			{
				public class TOREPLACE
				{
					public static LocString LABEL = "To replace:";
				}
				public class SCROLLAREA
				{
					public class CONTENT
					{
						public class NOSKILLSAVAILABLE
						{
							public static LocString LABEL = "No items available.";
						}
						public class PRESETENTRYPREFAB
						{
							public class SWITCHIN
							{
								public static LocString TEXT = "Select this";
							}

						}
					}
				}

				public class SEARCHBAR
				{
					internal class INPUT
					{
						public class TEXTAREA
						{
							public static LocString PLACEHOLDER = "Enter text to filter items...";
							public static LocString TEXT = "";
						}
					}
				}
			}
			public class PRESETWINDOW
			{
				public class DELETEWINDOW
				{
					public static LocString TITLE = "Delete {0}";
					public static LocString DESC = "You are about to delete the stat preset \"{0}\".\nDo you want to continue?";
					public static LocString YES = "Confirm Deletion";
					public static LocString CANCEL = "Cancel";

				}

				public static LocString TITLE = "Stat Presets";
				public static LocString TITLECREW = "Crew Presets";
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

									public static LocString INVALIDMODELTOOLTIP = "Preset not compatible with current duplicant model!";
									public static LocString RENAMEPRESETTOOLTIP = "Rename Preset";
									public static LocString DELETEPRESETTOOLTIP = "Delete Preset";
									public static LocString IMPORTEDPRESET = "This crew preset was imported from your old dgsm presets, it cannot be modified directly.";

								}
							}
						}

						internal class SEARCHBAR
						{
							public static LocString CLEARTOOLTIP = "Clear search bar";
							public static LocString OPENFOLDERTOOLTIP = "Open the folder where the presets are stored.";
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
								public static LocString TOOLTIP = "Close preset window";
							}
							public class GENERATEFROMCURRENT
							{
								public static LocString TEXT = "Generate Preset";
								public static LocString TOOLTIP = "Generate and store a new preset from the currenly displayed values.\nOnly has an effect if the preset doesn't exist yet";
							}
							public class APPLYPRESETBUTTON
							{
								public static LocString TEXT = "Apply Preset";
								public static LocString TOOLTIP = "Apply the currenly displayed stats to the Duplicant this window was opened from.";
								public static LocString TOOLTIPCREW = "Load the Crew Preset thats currently displayed in the preview";
							}
						}
						public class CHECKBOXES
						{
							public class REACTIONOVERRIDE
							{
								public static LocString LABEL = "Preset overrides Reactions";
								public static LocString TOOLTIP = "If enabled, applying presets to duplicants also overrides their inherited stress reaction and overjoyed response.";
							}
							public class NAMEOVERRIDE
							{
								public static LocString LABEL = "Preset overrides Name";
								public static LocString TOOLTIP = "If enabled, applying presets to duplicants also sets their name to that of the preset";
							}
						}
					}
				}
			}
			public class MODDEDIMMIGRANTSCREEN
			{
				public static LocString SELECTYOURLONECREWMAN = "CHOOSE YOUR LONE DUPLICANT TO BEGIN";
				public static LocString SELECTYOURCREW = "CHOOSE YOUR DUPLICANTS TO BEGIN";
				public static LocString ADDDUPE = "ADD SLOT";
				public static LocString ADDDUPETOOLTIP = "Add an additional duplicant slot, increasing your total starting duplicant count by 1.";
				public static LocString REMOVEDUPE = "REMOVE SLOT";
				public static LocString REMOVEDUPETOOLTIP = "Remove this duplicant slot, reducing your total starting duplicant count by 1.";
				public static LocString GUARANTEETRAIT = "Guarantee this trait to be rolled on a reroll.\nWorks in combination with the interest selector.";
				public static LocString ROLLWITHTRAIT_LABEL = "REROLL WITH TRAIT";
				public static LocString LOCKPERSONALITY_TOOLTIP = "Lock the current personality for rolls.\nWhile locked, all duplicants rolled will automatically have the locked personality.";
				public static LocString PRINTINGPOD_SELECT = "The current planet the active printing pod is on.\nClick to change the active printing pod";
			}
			public class DUPESETTINGSSCREEN
			{
				public static LocString APTITUDEENTRY = "{0} ({1} +{2})";
				public static LocString APTITUDEENTRY2 = "{0}\n({1} +{2})";
				public static LocString TRAIT = "{0}";
				public static LocString TRAITBONUSPOINTS = "Interest Bonus:";

				public static LocString TRAITBONUSPOOL = "Interest points to spend:";
				public static LocString CONFIGBALANCINGDISABLED = "ModConfig - Balancing Disabled";
				public static LocString OTHERMODORIGINNAME = "Mysterious Origin (other mods)";
				public static LocString TRAITBONUSPOOLTOOLTIP = "This Duplicant has {0} total interest bonus points\nThese come from the following trait boni:";

				public static LocString TRAITBALANCEHEADER = "Current Trait Balance: {0}";
				public static LocString BALANCE_BALANCED = "<b>balanced</b>";
				public static LocString BALANCE_STRONGER = "<b>stronger</b>";
				public static LocString BALANCE_WEAKER = "<b>weaker</b>";

				public static LocString BALANCE_TOOLTIP = "The current trait composition is {0}.\nThe trait balance value is {1}, for vanilla balancing it's usually between {2} and {3}\nBalance Composition:";
				public static LocString BALANCE_TOOLTIP_SIMPLE = "The current trait balance value is {0}.\nValues above 0 indicate a trait balance favouring positive traits (overall stronger duplicant),\nvalues below 0 indicate a trait balance favouring negative traits (overall weaker duplicant).\nThis is purely informational and does not limit your ability to edit traits\n\nBalance Composition:";

				public static LocString TRAIT_RARITYLEVEL = "Trait Rarity: {0}";

				public static LocString TRAIT_RARITY_LEGENDARY = "<b>Legendary</b>";
				public static LocString TRAIT_RARITY_EPIC = "<b>Epic</b>";
				public static LocString TRAIT_RARITY_RARE = "<b>Rare</b>";
				public static LocString TRAIT_RARITY_UNCOMMON = "<b>Uncommon</b>";
				public static LocString TRAIT_RARITY_COMMON = "<b>Common</b>";
			}
			public class BUTTONS
			{
				public static LocString REROLLCAREPACKAGE = "Reroll for a different Care Package";
				public static LocString CYCLENEXT = "Cycle to next";
				public static LocString CYCLEPREV = "Cycle to previous";
				public static LocString REMOVEFROMSTATS = "Remove";
				public static LocString ADDTOSTATS = "Add new";
				public static LocString MODIFYBUTTONTOOLTIP = "Modify Duplicant Stats";
				public static LocString MODIFYBUTTONTOOLTIP2 = "Apply Stat Changes";
				public static LocString PRESETWINDOWBUTTONTOOLTIP = "Open Duplicant Preset Window\nTo create a preset from this duplicants stats,\nclick the \"" + GENERATEFROMCURRENT.TEXT + "\" button in the preset window.";
				public static LocString DUPESKINBUTTONTOOLTIP = "Select Duplicant Personality (Skin)";
				public static LocString DUPELICITYEDITINGBUTTONTOOLTIP = "Edit Duplicants";

				public static LocString APPLYSKIN = "Apply this Personality";

			}
		}
	}
}
