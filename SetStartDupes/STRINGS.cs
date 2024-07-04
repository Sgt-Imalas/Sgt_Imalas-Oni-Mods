using Newtonsoft.Json;
using PeterHan.PLib.Options;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.ITEMINFO.BUTTONS;

namespace SetStartDupes
{
    public class STRINGS
    {
        public static LocString UNNAMEDPRESET = "(Unnamed Preset)";
        public static LocString MISSINGTRAIT = "Missing Trait!";
        public static LocString MISSINGTRAITDESC = "This Trait could not be found: {0}";
        public static LocString MISSINGSKILLGROUP = "Missing Attribute!";
        public static LocString MISSINGSKILLGROUPDESC = "This Attribute could not be found: {0}";
        public class UI
        {

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

                public class MODIFYDURINGGAME
                {
                    public static LocString NAME = "Modification of printing pod Duplicants";
                    public static LocString TOOLTIP = "Enable this option to add the modify button to printing pod dupes\nWhen disabled, the feature only appears on the starter dupe selection.\nThis also enables the use of presets.";
                }
                public class REROLLDURINGGAME
                {
                    public static LocString NAME = "Reroll printing pod";
                    public static LocString TOOLTIP = "Enable this option to add the reroll button to printing pod Duplicants and care packages.\nCare Packages allow selecting from all currently available package configurations.";
                }
                public class PRESETSOVERRIDEREACTIONS
                {
                    public static LocString NAME = "Presets override Reactions";
                    public static LocString TOOLTIP = "If enabled, applying presets to duplicants also overrides their inherited stress reaction and overjoyed response.";
                }
                public class PRESETSOVERRIDENAME
                {
                    public static LocString NAME = "Presets override Name";
                    public static LocString TOOLTIP = "If enabled, applying presets to duplicants also sets their name to that of the preset";
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

                public class CAREPACKAGESONLY
                {
                    public static LocString NAME = "Print Care Packages only";
                    public static LocString TOOLTIP = "When enabled, the printing pod will only give care packages when more than the the configured number of duplicants are alive.\nOverrides the care package configuration of a save game (no care package difficulty setting is ignored).";
                }
                public class MORECAREPACKAGES
                {
                    public static LocString NAME = "Additional Care Package Types";
                    public static LocString TOOLTIP = "Adds a number of items to the list of care packages that would otherwise not be obtainable.";
                }

                public class CAREPACKAGESONLYDUPECAP
                {
                    public static LocString NAME = "Duplicant Threshold for care packages only";
                    public static LocString TOOLTIP = "Only has an effect if \"" + CAREPACKAGESONLY.NAME + "\" is enabled.\nWhile above or at this number of duplicants, the printer will only give care packages.";
                }

                public class CAREPACKAGESONLYPACKAGECAP
                {
                    public static LocString NAME = "Number of care packages for care packages only";
                    public static LocString TOOLTIP = "Only has an effect if \"" + CAREPACKAGESONLY.NAME + "\" is enabled.\nSet the number of care packages that generate when the cap is in effect";
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

                                    public static LocString RENAMEPRESETTOOLTIP = "Rename Preset";
                                    public static LocString DELETEPRESETTOOLTIP = "Delete Preset";

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
                                public static LocString TOOLTIP = "close preset window";
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
