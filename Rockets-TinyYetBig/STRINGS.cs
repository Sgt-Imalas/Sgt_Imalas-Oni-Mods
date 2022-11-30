using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.NonRocketBuildings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig
{
    public class STRINGS
    {
        public class CATEGORYTOOLTIPS
        {
            public static LocString REQUIRED = "\nA Rocket needs atleast one of these!";

            public static LocString ENGINES = "Every rocket has to fly somehow.\nOne of these can provide the thrust." + REQUIRED;
            public static LocString HABITATS = "Strapped to the side a pilot won't survive long.\nBuild them a nice home to live in one of these." + REQUIRED;
            public static LocString NOSECONES = "When not using a habitat nosecone,\nthe rocket needs one of these\nto keep it's tip nicely shaped.";
            public static LocString DEPLOYABLES = "Colonizing new worlds needs some perimeter establishment.\nThese modules help with deployment.";
            public static LocString FUEL = "A rocket without fuel or oxidizer won't fly far.\nThese modules help you with that.";
            public static LocString CARGO = "All those resources, but where to put them?\nStore them within these modules.";
            public static LocString POWER = "Without power the lights inside of the rocket won't turn on\nThese modules help you store electricity, some even generate it.";
            public static LocString PRODUCTION = "Just bring the production with you!\nThese modules can produce something.";
            public static LocString UTILITY = "These modules add some nice utility functions to your rocket.";
            public static LocString UNCATEGORIZED = "What do these do?\n(not properly categorized)";

            //engines = 0,
            //habitats = 1,
            //nosecones = 2,
            //deployables = 3,
            //fuel = 4,
            //cargo = 5,
            //power = 6,
            //production = 7,
            //utility = 8,
            //uncategorized = -1
        }

        public class BUILDINGS
        {
            public class PREFABS
            {
                public static LocString GENERATORLIMIT = "\n\n If there is atleast one battery connected, the generator will stop producing if the battery is above 95% charge.";


                public class RTB_WALLCONNECTIONADAPTER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Insulated Rocket Port Wall Adapter", nameof(RTB_WALLCONNECTIONADAPTER));
                    public static LocString DESC = (LocString)"Insulated for convenience.\nRockets must be landed to load or unload resources.";
                    public static LocString EFFECT = (LocString)("An insulated wall adapter to seal of rocket start areas.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }

                public class RTB_BUNKERLAUNCHPAD
                {
                    public static LocString NAME = UI.FormatAsLink("Fortified Rocket Platform", nameof(BunkeredLaunchPadConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.DESC + "\n\nFortified to withstand comets.";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.EFFECT + "\n\nBlocks comets and is immune to comet damage.";
                }
                public class RTB_RTGGENERATORMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Radioisotope Thermoelectric Generator", nameof(RTGModuleConfig));
                    public static LocString DESC = "Through exploitation of the natural decay of enriched Uranium, this elegantly simple power generator can provide consistent, stable power for hundreds of cycles.";
                    public static LocString EFFECT = (string.Format("After adding {0} kg of enriched Uranium, this module will constantly produce {1} W of energy until all of the uranium is depleted.\n\nIt will take {2} Cycles for the uranium to decay.", RTGModuleConfig.UraniumCapacity, RTGModuleConfig.energyProduction, Config.Instance.IsotopeDecayTime));
                }
                public class RTB_STEAMGENERATORMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Steam Generator Module", nameof(SteamGeneratorModuleConfig));
                    public static LocString DESC = "Useful for converting hot steam into usable power.";
                    public static LocString EFFECT = "Draws in " + UI.FormatAsLink("Steam", "STEAM") + " from gas storage modules and uses it to generate electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\n If there are liquid storage modules with appropriate filters set, outputs hot " + UI.FormatAsLink("Water", "WATER") + " to them."+ GENERATORLIMIT;
                }
                public class RTB_GENERATORCOALMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Coal Generator Module", nameof(CoalGeneratorModuleConfig));
                    public static LocString DESC = ("Converts " + UI.FormatAsLink("Coal", "CARBON") + " into electrical " + UI.FormatAsLink("Power", "POWER") +".");
                    public static LocString EFFECT = "Burning coal produces more energy than manual power, who could have thought this also works in space." + GENERATORLIMIT;
                }


                public class RTB_HABITATMODULESTARGAZER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Stargazer Nosecone", nameof(HabitatModuleStargazerConfig));
                    public static LocString DESC = "The stars have never felt this close before like in this Command Module.";
                    public static LocString EFFECT = ("Closes during starts and landings to protect the glass\n\n" +
                                                        "Functions as a Command Module and a Nosecone.\n\n" +
                                                        "One Command Module may be installed per rocket.\n\n" +
                                                    "Must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME +
                                            ". \n\nMust be built at the top of a rocket.\n\nGreat for looking at the stars or a nice sunbathing during the flight.");
                }
                public class RTB_CRITTERCONTAINMENTMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("[DEPRECATED] Critter Containment Module", nameof(CritterContainmentModuleConfigOLD));
                    public static LocString EFFECT = "This module allows the safe transport of critters to their new home. ";
                    public static LocString DESC = "These critters will go where no critter has gone before.";
                }
                public class RTB_CRITTERSTASISMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Critter Stasis Module", nameof(CritterStasisModuleConfig));
                    public static LocString EFFECT = "This module allows the safe transport of critters to their new home.\n\nStored Critters wont age.";
                    public static LocString DESC = "These critters will go where no critter has gone before.";
                }
                public class RYB_NOSECONEHEPHARVEST
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Laser Drillcone", nameof(NoseConeHEPHarvestConfig));
                    public static LocString DESC = (LocString)"Harvests resources from the universe with the power of radbolts and lasers";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEHARVEST.EFFECT;
                }

                public class RTB_NOSECONESOLAR
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Solar Nosecone", nameof(NoseConeSolarConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.NOSECONEBASIC.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEBASIC.EFFECT +"\n\n"+
                        "Converts " + UI.FormatAsLink("Sunlight", "LIGHT") + " into electrical " + UI.FormatAsLink("Power", "POWER") + " for use on rockets.\n\nMust be exposed to space.";
                }


                public class RTB_HEPBATTERYMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Radbolt Chamber Module", nameof(HEPBatteryModuleConfig));
                    public static LocString DESC = (LocString)"Particles packed up and ready to visit the stars.";
                    public static LocString EFFECT = (LocString)("Stores Radbolts in a high-energy state, ready for transport.\n\n" +
                        "Requires a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to release radbolts from storage when the Radbolt threshold is reached.\n\n" +
                        "Radbolts in storage won't decay as long as the modules solar panels can function.");
                }
                

                public class RTB_HABITATMODULESMALLEXPANDED
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Extended Solo Spacefarer Nosecone", nameof(HabitatModuleSmallExpandedConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.EFFECT;
                }
                public class RTB_HABITATMODULEMEDIUMEXPANDED
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Extended Spacefarer Module", nameof(HabitatModuleMediumExpandedConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULEMEDIUM.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULEMEDIUM.EFFECT;
                }
                public class RTB_ROCKETPLATFORMTAG
                {
                    public static LocString NAME = "Rocket Platform";
                }
                
            }
        }

        /// <summary>
        /// StatusItems
        /// </summary>
        public class BUILDING
        {
            public class STATUSITEMS
            {
                public class RTB_MODULEGENERATORNOTPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator will generate " + UI.FormatAsPositiveRate("{MaxWattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " once traveling through space and fueled\n\nRight now, it's not doing much of anything");
                }

                public class RTB_MODULEGENERATORPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator is producing" + UI.FormatAsPositiveRate("{MaxWattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + "\n\nWhile traveling through space, it will continue generating power as long as are enough resources left");
                }
                public class RTB_MODULEGENERATORALWAYSACTIVEPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator is producing" + UI.FormatAsPositiveRate("{MaxWattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + "\n\nIt will continue generating power as long as are enough resources left");
                }
                public class RTB_MODULEGENERATORALWAYSACTIVENOTPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator will generate " + UI.FormatAsPositiveRate("{MaxWattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " once fueled\n\nRight now, it's not doing much of anything");
                }
            }
        }

        public class UI_MOD
        {

            public class UISIDESCREENS
            {
                public class SPACESTATIONSIDESCREEN
                {
                    public static LocString VIEW_WORLD_TOOLTIP = (LocString)"View Space Station Interior";
                    public static LocString TITLE = (LocString)"Space Station";

                    public static LocString VIEW_WORLD_DESC = (LocString)"Oversee Station Interior";
                }
                public class SPACESTATIONBUILDERMODULESIDESCREEN
                {
                    public static LocString TITLE = (LocString)"Space Station Construction";

                }
                public class DOCKINGSIDESCREEN
                {
                    public static LocString TITLE = (LocString)"Docking Management";

                }
            }
            public class DOCKINGUI
            {
                public static LocString BUTTON = (LocString)"View connected Target";
                public static LocString BUTTONINFO = (LocString)"View the interior this docking tube is currently connected to.";
            }
            public class FLUSHURANIUM
            {
                public static LocString BUTTON = (LocString)"Flush Generator Fuel";
                public static LocString BUTTONINFO = (LocString)"Empties the generators storage to allow a refill.";
            }
            public class CONSTRAINTS
            {
                public class ONE_MODULE_PER_ROCKET
                {

                    public static LocString COMPLETE = (LocString)"";
                    public static LocString FAILED = (LocString)"    • There already is a module of this type on this rocket";
                }
            }
        }

        public class OPTIONS
        {
            public const string TOGGLESINGLE = "Toggle to enable/disable this module in the building menu";
            public const string TOGGLEMULTI = "Toggle to enable/disable these modules in the building menu";
        }
        public class MODIFIEDVANILLASTRINGS
        {
            public static LocString KEROSENEENGINECLUSTERSMALL_EFFECT = (LocString)("Burns either " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " or " + UI.FormatAsLink("Ethanol", "ETHANOL") + " to propel rockets for mid-range space exploration.\n\nSmall Petroleum Engines possess the same speed as a " + UI.FormatAsLink("Petroleum Engines", "KEROSENEENGINE") + " but have smaller height restrictions.\n\nEngine must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
            public static LocString KEROSENEENGINECLUSTER_EFFECT = (LocString)("Burns either " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " or " + UI.FormatAsLink("Ethanol", "ETHANOL") + " to propel rockets for mid-range space exploration.\n\nPetroleum Engines have generous height restrictions, ideal for hauling many modules.\n\nEngine must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
        }
    }
}
