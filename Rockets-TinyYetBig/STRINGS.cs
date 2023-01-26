using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.Buildings.Nosecones;
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
        public class DEEPSPACERESEARCH
        {
            public static LocString NAME = "Deep Space Research";
            public static LocString UNLOCKNAME = (LocString)(UI.PRE_KEYWORD + NAME + UI.PST_KEYWORD + " Capability");
            public static LocString UNLOCKDESC = (LocString)("Allows " + UI.PRE_KEYWORD + NAME + UI.PST_KEYWORD + " points to be accumulated, unlocking higher technology tiers.\nCan be accumulated before research completion via artifact analysis.");
            public static LocString DESC = UI.FormatAsLink("Deep Space Research", nameof(DEEPSPACERESEARCH))+ " is conducted by analyzing the deeper meanings behind mysterious artefacts found in the vastness of deep space and by conducting various experiments in the low artifical gravity of a space station.";
            public static LocString RECIPEDESC = "Unlocks new breakthroughs in space construction";

        }
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

                public class RTB_DRILLCONEDIAMONDSTORAGE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Drillcone Service Module", nameof(RTB_DRILLCONEDIAMONDSTORAGE));
                    public static LocString DESC = (LocString)"Bringing home those minerals - for Rock and Stone!";
                    public static LocString EFFECT = (LocString)("Acts as a support module for a normal Drillcone.\n\nProvides additional 1500kg of diamond capacity for the drillcone.\n\nGives a 20% mining speed boost to the drillcone.\n\nCan be toggled between manual loading and automated loading via cargo loader.");
                }

                public class RTB_DOCKINGTUBEDOOR
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Docking Bridge", nameof(RTB_DOCKINGTUBEDOOR));
                    public static LocString DESC = (LocString)"Connecting with another (rocket) has never been easier.";
                    public static LocString EFFECT = (LocString)("Enables docking with other rockets and space stations\n\nBoth docking participants require a docking component to dock.\n\nAssigning a duplicant forces it to use the docking bridge.");
                }

                public class RTB_NATGASENGINECLUSTER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Natural Gas Engine", nameof(RTB_NATGASENGINECLUSTER));
                    public static LocString DESC = (LocString)"Rockets can be used to send Duplicants into space and retrieve rare resources.";
                    public static LocString EFFECT = (LocString)("Burns " + UI.FormatAsLink("Natural Gas", "METHANE") + " to propel rockets for mid-range space exploration.\n\nEngine must be built via " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
                }

                public class RTB_WALLCONNECTIONADAPTER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Insulated Rocket Port Wall Adapter", nameof(RTB_WALLCONNECTIONADAPTER));
                    public static LocString DESC = (LocString)"Insulated for convenience.\nRockets must be landed to load or unload resources.";
                    public static LocString EFFECT = (LocString)("An insulated wall adapter to seal off rocket start areas.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_LADDERCONNECTIONADAPTER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Rocket Port Ladder Adapter", nameof(RTB_WALLCONNECTIONADAPTER));
                    public static LocString DESC = (LocString)"Connecting rocket platforms, now with verticality";
                    public static LocString EFFECT = (LocString)("Connects adjacent rocket platforms while doubling as a ladder.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_HEPFUELLOADER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Radbolt Loader", nameof(RTB_HEPFUELLOADER));
                    public static LocString DESC = (LocString)"\"Shoving everything in there\" - now with higly energized particles.";
                    public static LocString EFFECT = (LocString)("Fills all sorts of Radbolt Storages.\nAllows fueling a Radbolt Engine and the Laser Drillcone\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_UNIVERSALFUELLOADER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Rocket Fuel Loader", nameof(RTB_UNIVERSALFUELLOADER));
                    public static LocString DESC = (LocString)"Fueling Rockets has never been easier!";
                    public static LocString EFFECT = (LocString)("Refuels connected rockets with the appropriate fuel.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_UNIVERSALOXIDIZERLOADER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Rocket Oxidizer Loader", nameof(RTB_UNIVERSALOXIDIZERLOADER));
                    public static LocString DESC = (LocString)"Fueling Rockets has never been easier!";
                    public static LocString EFFECT = (LocString)("Refuels connected rockets with the appropriate oxidizer\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }

                public class RTB_BUNKERLAUNCHPAD
                {
                    public static LocString NAME = UI.FormatAsLink("Fortified Rocket Platform", nameof(RTB_BUNKERLAUNCHPAD));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.DESC + "\n\nFortified to withstand comets.";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.EFFECT + "\n\nBlocks comets and is immune to comet damage.";
                }
                public class RTB_ADVANCEDLAUNCHPAD
                {
                    public static LocString NAME = UI.FormatAsLink("Advanced Rocket Platform", nameof(RTB_BUNKERLAUNCHPAD));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.EFFECT + "\n\nComes with shifted logic ports and an extra ribbon output that outputs the sub states of the " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY;
                    public static LocString LOGIC_PORT_CATEGORY_READY_ACTIVE = "Each bit sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when its respective check list category is fulfilled.\n" +
                        "\n Bit 1 tracks the category \"Flight Route\"" +
                        "\n Bit 2 tracks the category \"Rocket Construction\"" +
                        "\n Bit 3 tracks the category \"Cargo Manifest\"" +
                        "\n Bit 4 tracks the category \"Crew Manifest\"\n";

                    public static LocString LOGIC_PORT_CATEGORY_READY_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby)+" to the respective Bit.";
                }
                public class RTB_RTGGENERATORMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Radioisotope Thermoelectric Generator", nameof(RTB_RTGGENERATORMODULE));
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

                public class RTB_CARGOBAYCLUSTERLARGE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Colossal Cargo Bay", nameof(RTB_CARGOBAYCLUSTERLARGE));
                    public static LocString DESC = (LocString)"Holds even more than a large cargo bay!";
                    public static LocString EFFECT = (LocString)("Allows Duplicants to store most of the " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " found during space missions.\n\nStored resources become available to the colony upon the rocket's return. \n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
                }

                public class RTB_LIQUIDCARGOBAYCLUSTERLARGE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Colossal Liquid Cargo Tank", nameof(RTB_LIQUIDCARGOBAYCLUSTERLARGE));
                    public static LocString DESC = (LocString)"Holds even more than a large cargo tank!";
                    public static LocString EFFECT = (LocString)("Allows Duplicants to store most of the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.\n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
                }
                public class RTB_GASCARGOBAYCLUSTERLARGE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Colossal Gas Cargo Canister", nameof(RTB_GASCARGOBAYCLUSTERLARGE));
                    public static LocString DESC = (LocString)"Holds even more than a large gas cargo canister!";
                    public static LocString EFFECT = (LocString)("Allows Duplicants to store most of the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.\n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
                }
                public class RYB_NOSECONEHEPHARVEST
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Laser Drillcone", nameof(NoseConeHEPHarvestConfig));
                    public static LocString DESC = (LocString)"Harvests resources from the universe with the power of radbolts and lasers";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEHARVEST.EFFECT;
                }
                public class RTB_CO2FUELTANK
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Carbon Dioxide Fuel Tank", nameof(RTB_CO2FUELTANK));
                    public static LocString DESC = (LocString)"Storing additional fuel increases the distance a rocket can travel before returning.";
                    public static LocString EFFECT = ("Stores pressurized " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " for " + UI.FormatAsLink("Carbon Dioxide Engines", CO2EngineConfig.ID));
                }
                public class RTB_LIQUIDFUELTANKCLUSTERSMALL
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Small Liquid Fuel Tank", nameof(RTB_LIQUIDFUELTANKCLUSTERSMALL));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANKCLUSTER.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANKCLUSTER.EFFECT;
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
                public class RTB_MODULEGENERATORFUELSTATUS
                {
                    public static LocString NAME = (LocString)"Generator Fuel Capacity: {CurrentFuelStorage}/{MaxFuelStorage}";
                    public static LocString TOOLTIP = (LocString)("This {GeneratorType} has {CurrentFuelStorage} out of {MaxFuelStorage} available.");
                }
                public class RTB_ROCKETBATTERYSTATUS
                {
                    public static LocString NAME = (LocString)"Battery Module Charge: {CurrentCharge}/{MaxCharge}";
                    public static LocString TOOLTIP = (LocString)("This Rocket has {CurrentCharge}/{MaxCharge} stored in battery modules.");
                }
                public class RTB_CRITTERMODULECONTENT
                {
                    public static LocString NAME = (LocString)"Critter Count: {CurrentCritterCount}/{MaxCritterCount}";
                    public static LocString TOOLTIP = (LocString)("{CritterContentStatus}");
                    public static LocString NOCRITTERS = "No Critters stored.";
                    public static LocString HASCRITTERS = "Module currently holds these Critters:";
                    public static LocString CRITTERINFO = " • {CRITTERNAME}, {AGE} Cycles old";
                }
                public class RTB_STATIONCONSTRUCTORSTATUS
                {
                    public static LocString NAME = (LocString)"Module Status: {STATUS}";
                    public static LocString IDLE = (LocString)("Nominal");
                    public static LocString NONEQUEUED = (LocString)("No active process");
                    public static LocString TIMEREMAINING = (LocString)("Time until current operation is completed: {TIME}");
                    public static LocString TOOLTIP = (LocString)("{TOOLTIP}");
                    public static LocString CONSTRUCTING = (LocString)("Constructing: {TIME} left");
                    public static LocString DECONSTRUCTING = (LocString)("Demolishing: {TIME} left");

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
                    public static LocString CONSTRUCTTOOLTIP = (LocString)"Space Station Construction";
                    public static LocString CANCELCONSTRUCTION = (LocString)"Cancel Construction";
                    public static LocString STARTCONSTRUCTION = (LocString)"Start Station Construction";

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
        public class RESEARCH
        {
            public class TECHS
            {
                public class RTB_FUELLOADERSTECH
                {
                    public static LocString NAME = UI.FormatAsLink("Fuel Loaders", nameof(RTB_FUELLOADERSTECH));
                    public static LocString DESC = "Automatically refuel your rockets with these Loaders.\nCan be placed inside a space station.";

                }
                public class RTB_DOCKINGTECH
                {
                    public static LocString NAME = UI.FormatAsLink("Celestial Connection", nameof(RTB_DOCKINGTECH));
                    public static LocString DESC = "Dock with other spacecrafts";

                }
                public class RTB_LARGERROCKETLIVINGSPACETECH
                {
                    public static LocString NAME = UI.FormatAsLink("Luxurious Liv'in Space", nameof(RTB_LARGERROCKETLIVINGSPACETECH));
                    public static LocString DESC = "All the living space a dupe could ask for, now in your rocket.";

                }
                public class RTB_SPACESTATIONTECH
                {
                    public static LocString NAME = UI.FormatAsLink("Deep Space Exploration", nameof(RTB_SPACESTATIONTECH));
                    public static LocString DESC = "Mysterious Artifacts have shown new perspectives on living in the vast emptyness";
                }
                public class RTB_MEDIUMSPACESTATIONTECH
                {
                    public static LocString NAME = UI.FormatAsLink("Deep Space Colonization", nameof(RTB_MEDIUMSPACESTATIONTECH));
                    public static LocString DESC = "Extending the perspective";
                }
                public class RTB_LARGESPACESTATIONTECH
                {
                    public static LocString NAME = UI.FormatAsLink("Deep Space Expansion", nameof(RTB_LARGESPACESTATIONTECH));
                    public static LocString DESC = "Conquering the depths of space";
                }
                public class RTB_HUGECARGOBAYTECH
                {
                    public static LocString NAME = UI.FormatAsLink("Thinking larger", nameof(RTB_LARGESPACESTATIONTECH));
                    public static LocString DESC = "Lets bring home ALL those minerals.";
                }
            }
        }
    }
}
