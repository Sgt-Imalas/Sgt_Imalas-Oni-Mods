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
        public class UI
        {

            public class DSS_OPTIONS
            {
                public class CATEGORIES
                {
                    public static LocString A_GAMESTART = "Options for Game Start";
                    public static LocString B_PRINTINGPOD = "Options for Printing Pod";
                    public static LocString C_EXTRAS = "Options for Extra Behaviours";
                }

                public class DUPLICANTSTARTAMOUNT
                {
                    public static LocString NAME = "Amount of Starting Duplicants";
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
                    public static LocString TOOLTIP = "Enable this option to add the reroll button to printing pod Duplicants and care packages.";
                }

                public class PRINTINGPODRECHARGETIME
                {
                    public static LocString NAME = "Printing pod cooldown time";
                    public static LocString TOOLTIP = "Time it takes for the printing pod to recharge for the next print in cycles.\nDefault is 3 cycles.";
                }

                public class PAUSEONREADYTOPRING
                {
                    public static LocString NAME = "Pause on \"ready to print\"";
                    public static LocString TOOLTIP = "Pause the game when the printing pod has recharged for a new print";
                }

                public class CAREPACKAGESONLY
                {
                    public static LocString NAME = "Print Care Packages only";
                    public static LocString TOOLTIP = "When enabled, the printing pod will only give care packages when more than the the configured number of duplicants are alive.\nOverrides the care package configuration of a save game.";
                }

                public class CAREPACKAGESONLYDUPECAP
                {
                    public static LocString NAME = "Duplicant Threshold for care packages only";
                    public static LocString TOOLTIP = "Only has an effect if \"" + CAREPACKAGESONLY.NAME + "\" is enabled.\nWhile above this number of duplicants, the printer will only give care packages.";
                }

                public class CAREPACKAGESONLYPACKAGECAP
                {
                    public static LocString NAME = "Number of care packages for care packages only";
                    public static LocString TOOLTIP = "Only has an effect if \"" + CAREPACKAGESONLY.NAME + "\" is enabled.\nSet the number of care packages that generate when the cap is in effect";
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
                public class BALANCEADDANDREMOVE
                {
                    public static LocString NAME = "Basic balancing for Add/Remove";
                    public static LocString TOOLTIP = "Applies some basic balancing to the adding/removal of traits and interests.\nthis limits the interest count to 3 and adds a \"negative trait\" - cost to positive traits\nOption is recommended to avoid overpowered duplicants.";
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
                                public static LocString TOOLTIP = "Generate and store a new preset from the currenly displayed stats.\nOnly has an effect if the preset doesn't exist yet";
                            }
                            public class APPLYPRESETBUTTON
                            {
                                public static LocString TEXT = "Apply Preset";
                                public static LocString TOOLTIP = "Apply the currenly displayed stats to the Duplicant this window was opened from.";
                            }
                        }
                    }
                }
            }
            public class MODDEDIMMIGRANTSCREEN
            {
                public static LocString SELECTYOURLONECREWMAN = "CHOOSE YOUR LONE DUPLICANT TO BEGIN";
                public static LocString SELECTYOURCREW = "CHOOSE YOUR DUPLICANTS TO BEGIN";
            }
            public class DUPESETTINGSSCREEN
            {
                public static LocString APTITUDEENTRY = "{0}, ({1} +{2})";
                public static LocString TRAIT = "{0}";

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

                public static LocString APPLYSKIN = "Apply this Personality";

            }
        }
    }
}
