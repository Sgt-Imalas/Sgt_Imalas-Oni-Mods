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
using static Rockets_TinyYetBig.STRINGS.BUILDINGS.PREFABS;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.ELEMENTS;
using static STRINGS.MISC.NOTIFICATIONS;
using static STRINGS.UI;

namespace Rockets_TinyYetBig
{
    public class STRINGS
    {
        public class CODEX
        {
            public class STORY_TRAITS
            {
                public class RTB_CRASHEDUFOSTORYTRAIT
                {
                    public static LocString NAME = "Crashed Starship";
                    public static LocString DESCRIPTION = "tba";
                }
            }

        }

        public class MISC
        {
            public class TAGS
            {
                public static LocString RADIATIONSHIELDINGMATERIAL = "Radiation Shielding";
                public static LocString RTB_NEUTRONIUMALLOYMATERIAL = "Neutronium Alloy";
                public static LocString RTB_ROCKETFUELMATERIAL = "Rocket Fuel";
                public static LocString RTB_OXIDIZERCORROSIVEREQUIREMENT = "Corrosive Liquid Oxidizer";
                public static LocString RTB_OXIDIZERLOXTANK = "Liquid Oxidizer";
                public static LocString RTB_OXIDIZEREFFICIENCY_1 = "Oxidizer Efficiency 1";
                public static LocString RTB_OXIDIZEREFFICIENCY_2 = "Oxidizer Efficiency 2";
                public static LocString RTB_OXIDIZEREFFICIENCY_3 = "Oxidizer Efficiency 3";
                public static LocString RTB_OXIDIZEREFFICIENCY_4 = "Oxidizer Efficiency 4";
                public static LocString RTB_OXIDIZEREFFICIENCY_5 = "Oxidizer Efficiency 5";
                public static LocString RTB_OXIDIZEREFFICIENCY_6 = "Oxidizer Efficiency 6";
            }

        }

        public class ELEMENTS
        {
            public class SPACESTATIONFORCEFIELD
            {
                public static LocString NAME = (LocString)FormatAsLink("Station Force Field", nameof(SPACESTATIONFORCEFIELD));
                public static LocString DESC = "A force field, protecting the station against micro meteors.";
            }
            public class UNOBTANIUMALLOY
            {
                public static LocString NAME = (LocString)FormatAsLink("Neutronium Alloy", nameof(UNOBTANIUMALLOY));
                public static LocString DESC = "An insanely durable and heat resistant alloy.\nRequired in the construction of large space structures.\nVery sparkly";
                public static LocString RECIPE_DESCRIPTION = "Neutronium Alloy is a " + FormatAsLink("Solid Material", "ELEMENTS_SOLID") + " used in the construction of large space structures.";
            }
            public class UNOBTANIUMDUST
            {
                public static LocString NAME = (LocString)FormatAsLink("Neutronium Dust", nameof(UNOBTANIUMDUST));
                public static LocString DESC = "Harvested from artifact research, this dust might have some useful properties.\nCan be forged into " +
                    FormatAsLink("Neutronium Alloy", nameof(UNOBTANIUMALLOY)) + " at the " + (LocString)FormatAsLink("Molecular Forge", nameof(SUPERMATERIALREFINERY));
            }
        }
        public class DEEPSPACERESEARCH
        {
            public static LocString NAME = "Deep Space Research";
            public static LocString UNLOCKNAME = (LocString)(PRE_KEYWORD + NAME + PST_KEYWORD + " Capability");
            public static LocString UNLOCKDESC = (LocString)("Allows " + PRE_KEYWORD + NAME + PST_KEYWORD + " points to be accumulated, unlocking higher technology tiers.\nCan be accumulated before research completion via artifact analysis.");
            public static LocString DESC = FormatAsLink("Deep Space Research", nameof(DEEPSPACERESEARCH)) + " is conducted by analyzing the deeper meanings behind mysterious artefacts found in the vastness of deep space and by conducting various experiments in the low artifical gravity of a space station.";
            public static LocString RECIPEDESC = "Unlocks new breakthroughs in space construction";

        }

        public class ROCKETBUILDMENUCATEGORIES
        {
            public static LocString SEARCHBARFILLER = "Search all modules...";
            public class CATEGORYTOOLTIPS
            {
                public static LocString REQUIRED = "\nA Rocket needs atleast one of these!";

                public static LocString ENGINES = "Every rocket has to fly somehow.\nA rocket engine provides the necessary thrust." + REQUIRED;
                public static LocString HABITATS = "Strapped to the side, a pilot wouldn't survive long.\nBuild them a nice home to live in a Spacefarer ." + REQUIRED;
                public static LocString NOSECONES = "When not using a habitat nosecone,\nthe rocket needs one of these\nto keep it's tip nicely shaped.";
                public static LocString DEPLOYABLES = "Colonizing new worlds needs some perimeter establishment.\nThese modules help with deployment.";
                public static LocString FUEL = "A rocket without fuel or oxidizer won't fly far.\nThese modules help you with that.";
                public static LocString CARGO = "All those resources, but where to put them?\nStore them within these modules.";
                public static LocString POWER = "Without power the lights inside of the rocket won't turn on\nThese modules help you store electricity, some even generate it.";
                public static LocString PRODUCTION = "Just bring the production with you!\nThese modules can produce something.";
                public static LocString UTILITY = "These modules add some nice utility functions to your rocket.";
                public static LocString UNCATEGORIZED = "What do these do?\n(not properly categorized, contact the author of that mod)";

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
            public class CATEGORYTITLES
            {
                public static LocString ENGINES = "Rocket Engines";
                public static LocString HABITATS = "Command Modules";
                public static LocString NOSECONES = "Nosecones";
                public static LocString DEPLOYABLES = "Deployables";
                public static LocString FUEL = "Fuel & Oxidizer Tanks";
                public static LocString CARGO = "Cargo & Storage";
                public static LocString POWER = "Power Storage & Production";
                public static LocString PRODUCTION = "Production";
                public static LocString UTILITY = "Utility";
                public static LocString UNCATEGORIZED = "Uncategorized";

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
        }


        public class BUILDINGS
        {
            public class PREFABS
            {
                public static LocString GENERATORLIMIT = "\n\n If there is atleast one battery connected, the generator will stop producing if the battery is above 95% charge.";
                public class RTB_LANDERROCKETPLATFORM
                {
                    public static LocString NAME = (LocString)FormatAsLink("Deployable Rocket Platform", nameof(RTB_LANDERROCKETPLATFORM));
                    public static LocString DESC = (LocString)"Perfect for a first landing";
                }


                public class RTB_POICAPACITYSENSOR
                {
                    public static LocString NAME = (LocString)FormatAsLink("Starmap POI Capacity Sensor", nameof(RTB_POICAPACITYSENSOR));

                    public static LocString DESC = (LocString)"How much stuff is out there?";
                    public static LocString EFFECT = (LocString)("After pointing the building at a space poi, the building will output logic signals based on the poi artifact existing and the mass remaining in the poi.");
                    public static LocString LOGIC_PORT_CAPACITY = (LocString)"POI mass above set threshold.";
                    public static LocString LOGIC_PORT_CAPACITY_ACTIVE = (LocString)("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when the targeted POI has more than the configured mass remaining");
                    public static LocString LOGIC_PORT_CAPACITY_INACTIVE = (LocString)("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));

                    public static LocString SIDESCREENTITLE = (LocString)"Remaining Mass";
                    public static LocString REMAININGMASS = (LocString)"Remaining Mass in POI";
                    public static LocString REMAININGMASS_TOOLTIP_ABOVE = (LocString)("Will send a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " if the " + PRE_KEYWORD + "remaining Mass in the POI" + PST_KEYWORD + " is above <b>{0}</b>");
                    public static LocString REMAININGMASS_TOOLTIP_BELOW = (LocString)("Will send a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " if the " + PRE_KEYWORD + "remaining Mass in the POI" + PST_KEYWORD + " is below <b>{0}</b>");


                    public static LocString LOGIC_PORT_ARTIFACT = (LocString)"POI has Artifact";
                    public static LocString LOGIC_PORT_ARTIFACT_ACTIVE = (LocString)("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when the targeted POI has an artifact");
                    public static LocString LOGIC_PORT_ARTIFACT_INACTIVE = (LocString)("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));


                }

                public class RTB_FRIDGEMODULEACCESSHATCH
                {
                    public static LocString NAME = (LocString)FormatAsLink("Freezer Access Hatch", nameof(RTB_FRIDGEMODULEACCESSHATCH));
                    public static LocString DESC = (LocString)"Food spoilage can be slowed by ambient conditions as well as by refrigerators.";
                    public static LocString EFFECT = (LocString)("Has to be attached to the rocket wall.\n\nStores a small amount of " + FormatAsLink("Food", "FOOD") + " at an ideal " + FormatAsLink("Temperature", "HEAT") + " to prevent spoilage.\n\nWill pull food from a connected " + RTB_FRIDGECARGOBAY.NAME + " when low on food.");
                    public static LocString LOGIC_PORT = (LocString)"Full/Not Full";
                    public static LocString LOGIC_PORT_ACTIVE = (LocString)("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when full");
                    public static LocString LOGIC_PORT_INACTIVE = (LocString)("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));

                    public static LocString LOGIC_PORT_PULL = (LocString)"Pull food from Freezer Module";
                    public static LocString LOGIC_PORT_ACTIVE_PULL = (LocString)(FormatAsAutomationState("Green Signal", AutomationState.Active) + ": Building will pull more food from the Freezer Module when below 1kg of food.");
                    public static LocString LOGIC_PORT_INACTIVE_PULL = (LocString)(FormatAsAutomationState("Red Signal", AutomationState.Standby) + ": Building wont pull any more food even when below 1kg.");
                }
                public class RTB_DRILLCONEDIAMONDSTORAGE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Drillcone Service Module", nameof(RTB_DRILLCONEDIAMONDSTORAGE));
                    public static LocString DESC = (LocString)"Bringing home those minerals - for Rock and Stone!";
                    public static LocString EFFECT = (LocString)("Acts as a support module for a normal Drillcone.\n\nProvides additional 1500kg of diamond capacity for the drillcone.\n\nGives a 20% mining speed boost to the drillcone.\n\nCan be toggled between manual loading and automated loading via cargo loader.");
                }

                public class RTB_AIMODULEDOCKINGPORT
                {
                    public static LocString NAME = (LocString)FormatAsLink("Docking Module", nameof(RTB_AIMODULEDOCKINGPORT));
                    public static LocString DESC = (LocString)"Connecting with another (rocket) has never been easier.";
                    public static LocString EFFECT = (LocString)("Enables docking with other rockets and space stations\n\nBoth docking participants require a docking component to dock.\n\nDuplicants cannot use this docking connection.\n\nAdd it to an AI controlled rocket to allow it to dock.\n\nMultiple Modules allow multiple docking connections");
                }
                public class RTB_DOCKINGTUBEDOOR
                {
                    public static LocString NAME = (LocString)FormatAsLink("Docking Bridge", nameof(RTB_DOCKINGTUBEDOOR));
                    public static LocString DESC = (LocString)"Connecting with another (rocket) has never been easier.";
                    public static LocString EFFECT = (LocString)("Enables docking with other rockets and space stations\n\nBoth docking participants require a docking component to dock.\n\nAssigning a duplicant forces it to use the docking bridge.");
                }

                public class RTB_NATGASENGINECLUSTER
                {
                    public static LocString NAME = (LocString)FormatAsLink("Natural Gas Engine", nameof(RTB_NATGASENGINECLUSTER));
                    public static LocString DESC = (LocString)"Rockets can be used to send Duplicants into space and retrieve rare resources.";
                    public static LocString EFFECT = (LocString)("Burns " + FormatAsLink("Natural Gas", "METHANE") + " to propel rockets for mid-range space exploration.\n\nEngine must be built via " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
                }
                public class RTB_SMOLBATTERYMODULE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Small Battery Module", nameof(RTB_SMOLBATTERYMODULE));
                    public static LocString DESC = (LocString)global::STRINGS.BUILDINGS.PREFABS.BATTERYMODULE.DESC;
                    public static LocString EFFECT = (LocString)global::STRINGS.BUILDINGS.PREFABS.BATTERYMODULE.EFFECT;
                }
                public class RTB_FRIDGECARGOBAY
                {
                    public static LocString NAME = (LocString)FormatAsLink("Freezer Module", nameof(RTB_FRIDGECARGOBAY));
                    public static LocString DESC = (LocString)"Space food for days";
                    public static LocString EFFECT = (LocString)"Keeps food preserved, prevent spoilage.\nCan only be filled with a cargo loader.\nContents can be accessed during the flight via wall loader";
                }
                public class RTB_WALLCONNECTIONADAPTER
                {
                    public static LocString NAME = (LocString)FormatAsLink("Insulated Rocket Port Wall Adapter", nameof(RTB_WALLCONNECTIONADAPTER));
                    public static LocString DESC = (LocString)"Insulated for convenience.\nRockets must be landed to load or unload resources.";
                    public static LocString EFFECT = (LocString)("An insulated wall adapter to seal off rocket start areas.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_LADDERCONNECTIONADAPTER
                {
                    public static LocString NAME = (LocString)FormatAsLink("Rocket Port Ladder Adapter", nameof(RTB_WALLCONNECTIONADAPTER));
                    public static LocString DESC = (LocString)"Connecting rocket platforms, now with verticality";
                    public static LocString EFFECT = (LocString)("Connects adjacent rocket platforms while doubling as a ladder.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_HEPFUELLOADER
                {
                    public static LocString NAME = (LocString)FormatAsLink("Radbolt Loader", nameof(RTB_HEPFUELLOADER));
                    public static LocString DESC = (LocString)"\"Shoving everything in there\" - now with higly energized particles.";
                    public static LocString EFFECT = (LocString)("Fills all sorts of Radbolt Storages.\nAllows fueling a Radbolt Engine and the Laser Drillcone\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_UNIVERSALFUELLOADER
                {
                    public static LocString NAME = (LocString)FormatAsLink("Rocket Fuel Loader", nameof(RTB_UNIVERSALFUELLOADER));
                    public static LocString DESC = (LocString)"Fueling Rockets has never been easier!";
                    public static LocString EFFECT = (LocString)("Refuels connected rockets with the appropriate fuel.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }
                public class RTB_UNIVERSALOXIDIZERLOADER
                {
                    public static LocString NAME = (LocString)FormatAsLink("Rocket Oxidizer Loader", nameof(RTB_UNIVERSALOXIDIZERLOADER));
                    public static LocString DESC = (LocString)"Fueling Rockets has never been easier!";
                    public static LocString EFFECT = (LocString)("Refuels connected rockets with the appropriate oxidizer\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
                }

                public class RTB_BUNKERLAUNCHPAD
                {
                    public static LocString NAME = FormatAsLink("Fortified Rocket Platform", nameof(RTB_BUNKERLAUNCHPAD));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.DESC + "\n\nFortified to withstand comets.";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.EFFECT + "\n\nBlocks comets and is immune to comet damage.";
                }
                public class RTB_ADVANCEDLAUNCHPAD
                {
                    public static LocString NAME = FormatAsLink("Advanced Rocket Platform", nameof(RTB_BUNKERLAUNCHPAD));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.EFFECT + "\n\nComes with shifted logic ports and extra ribbon ports for more control over the rocket.";
                    public static LocString LOGIC_PORT_CATEGORY_READY_ACTIVE = "Each bit sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when its respective check list category is fulfilled.\n" +
                        "\n Bit 1 tracks the category \"Flight Route\"" +
                        "\n Bit 2 tracks the category \"Rocket Construction\"" +
                        "\n Bit 3 tracks the category \"Cargo Manifest\"" +
                        "\n Bit 4 tracks the category \"Crew Manifest\"\n";

                    public static LocString LOGIC_PORT_CATEGORY_READY_INACTIVE = "Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby) + " to the respective Bit.";


                    public static LocString LOGIC_PORT_LAUNCH_ACTIVE_RIBBON = FormatAsAutomationState("Green Signal on the first Bit", AutomationState.Active) + ": Launch rocket" +
                        "\n" + FormatAsAutomationState("Bit 2", AutomationState.Active) + " allows to overrride the \"Fueled\" Warning under Cargo Manifest that otherwise prevent automated launches. This allows rockets to fly one-way-trips to another launchpad" +
                        "\n" + FormatAsAutomationState("Bit 3", AutomationState.Active) + " allows to overrride all Cargo Warnings (that aren't Fuel) under Cargo Manifest that otherwise prevent automated launches." +
                        "\n" + FormatAsAutomationState("Bit 4", AutomationState.Active) + " makes the Overrides of Bit 2 and 3 affect the logic output of the launch pad. ";

                    public static LocString LOGIC_PORT_LAUNCH_INACTIVE_RIBBON = FormatAsAutomationState("Red Signal on the first Bit", AutomationState.Standby) + ": Cancel launch";
                }
                public class RTB_RTGGENERATORMODULE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Radioisotope Thermoelectric Generator", nameof(RTB_RTGGENERATORMODULE));
                    public static LocString DESC = "Through exploitation of the natural decay of enriched Uranium, this elegantly simple power generator can provide consistent, stable power for hundreds of cycles.";
                    public static LocString EFFECT = (string.Format("After adding {0} kg of enriched Uranium, this module will constantly produce {1} W of energy until all of the uranium is depleted.\n\nIt will take {2} Cycles for the uranium to decay.", RTGModuleConfig.UraniumCapacity, RTGModuleConfig.energyProduction, Config.Instance.IsotopeDecayTime));
                }
                public class RTB_STEAMGENERATORMODULE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Steam Generator Module", nameof(SteamGeneratorModuleConfig));
                    public static LocString DESC = "Useful for converting hot steam into usable power.";
                    public static LocString EFFECT = "Draws in " + FormatAsLink("Steam", "STEAM") + " from gas storage modules and uses it to generate electrical " + FormatAsLink("Power", "POWER") + ".\n\n If there are liquid storage modules with appropriate filters set, outputs hot " + FormatAsLink("Water", "WATER") + " to them." + GENERATORLIMIT;
                }
                public class RTB_GENERATORCOALMODULE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Coal Generator Module", nameof(CoalGeneratorModuleConfig));
                    public static LocString DESC = ("Converts " + FormatAsLink("Coal", "CARBON") + " into electrical " + FormatAsLink("Power", "POWER") + ".");
                    public static LocString EFFECT = "Burning coal produces more energy than manual power, who could have thought this also works in space." + GENERATORLIMIT;
                    public static LocString SIDESCREEN_TOOLTIP = "Duplicants will be requested to deliver " + PRE_KEYWORD + "{0}" + PST_KEYWORD + " when the amount stored falls below <b>{1}</b>";
                }


                public class RTB_HABITATMODULESTARGAZER
                {
                    public static LocString NAME = (LocString)FormatAsLink("Stargazer Nosecone", nameof(HabitatModuleStargazerConfig));
                    public static LocString DESC = "The stars have never felt this close before like in this Command Module.";
                    public static LocString EFFECT = ("Closes during starts and landings to protect the glass\n\n" +
                                                        "Functions as a Command Module and a Nosecone.\n\n" +
                                                        "One Command Module may be installed per rocket.\n\n" +
                                                    "Must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME +
                                            ". \n\nMust be built at the top of a rocket.\n\nGreat for looking at the stars or a nice sunbathing during the flight.");
                }
                public class RTB_CRITTERCONTAINMENTMODULE
                {
                    public static LocString NAME = (LocString)FormatAsLink("[DEPRECATED] Critter Containment Module", nameof(CritterContainmentModuleConfigOLD));
                    public static LocString EFFECT = "This module allows the safe transport of critters to their new home. ";
                    public static LocString DESC = "These critters will go where no critter has gone before.";
                }
                public class RTB_CRITTERSTASISMODULE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Critter Stasis Module", nameof(CritterStasisModuleConfig));
                    public static LocString EFFECT = "This module allows the safe transport of critters to their new home.\n\nStored Critters wont age.";
                    public static LocString DESC = "These critters will go where no critter has gone before.";
                }

                public class RTB_CARGOBAYCLUSTERLARGE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Colossal Cargo Bay", nameof(RTB_CARGOBAYCLUSTERLARGE));
                    public static LocString DESC = (LocString)"Holds even more than a large cargo bay!";
                    public static LocString EFFECT = (LocString)("Allows Duplicants to store most of the " + FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " found during space missions.\n\nStored resources become available to the colony upon the rocket's return. \n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
                }

                public class RTB_LIQUIDCARGOBAYCLUSTERLARGE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Colossal Liquid Cargo Tank", nameof(RTB_LIQUIDCARGOBAYCLUSTERLARGE));
                    public static LocString DESC = (LocString)"Holds even more than a large cargo tank!";
                    public static LocString EFFECT = (LocString)("Allows Duplicants to store most of the " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.\n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
                }
                public class RTB_GASCARGOBAYCLUSTERLARGE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Colossal Gas Cargo Canister", nameof(RTB_GASCARGOBAYCLUSTERLARGE));
                    public static LocString DESC = (LocString)"Holds even more than a large gas cargo canister!";
                    public static LocString EFFECT = (LocString)("Allows Duplicants to store most of the " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.\n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
                }
                public class RYB_NOSECONEHEPHARVEST
                {
                    public static LocString NAME = (LocString)FormatAsLink("Laser Drillcone", nameof(NoseConeHEPHarvestConfig));
                    public static LocString DESC = (LocString)"Harvests resources from the universe with the power of radbolts and lasers";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEHARVEST.EFFECT;
                }
                public class RTB_CO2FUELTANK
                {
                    public static LocString NAME = (LocString)FormatAsLink("Carbon Dioxide Fuel Tank", nameof(RTB_CO2FUELTANK));
                    public static LocString DESC = (LocString)"Storing additional fuel increases the distance a rocket can travel before returning.";
                    public static LocString EFFECT = ("Stores pressurized " + FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " for " + FormatAsLink("Carbon Dioxide Engines", CO2EngineConfig.ID));
                }

                public class RTB_LIQUIDCHLORINEOXIDIZERTANK
                {
                    public static LocString NAME = (LocString)FormatAsLink("Liquid Chlorine Oxidizer Tank", nameof(RTB_LIQUIDCHLORINEOXIDIZERTANK));
                    public static LocString DESC = (LocString)"Liquid chlorine improves the thrust-to-mass ratio of rocket fuels.";
                    public static LocString EFFECT = (LocString)("Stores " + FormatAsLink("Liquid Chlorine", nameof(CHLORINE)) + " for burning rocket fuels. \n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".\n\nThe oxidizer efficiency of liquid chlorine sits between those of oxylite and liquid oxygen (3).");
                }
                public class RTB_LIQUIDFUELTANKCLUSTERSMALL
                {
                    public static LocString NAME = (LocString)FormatAsLink("Small Liquid Fuel Tank", nameof(RTB_LIQUIDFUELTANKCLUSTERSMALL));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANKCLUSTER.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANKCLUSTER.EFFECT;
                }

                public class RTB_NOSECONESOLAR
                {
                    public static LocString NAME = (LocString)FormatAsLink("Solar Nosecone", nameof(NoseConeSolarConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.NOSECONEBASIC.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEBASIC.EFFECT + "\n\n" +
                        "Converts " + FormatAsLink("Sunlight", "LIGHT") + " into electrical " + FormatAsLink("Power", "POWER") + " for use on rockets.\n\nMust be exposed to space.";
                }


                public class RTB_HEPBATTERYMODULE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Radbolt Chamber Module", nameof(HEPBatteryModuleConfig));
                    public static LocString DESC = (LocString)"Particles packed up and ready to visit the stars.";
                    public static LocString EFFECT = (LocString)("Stores Radbolts in a high-energy state, ready for transport.\n\n" +
                        "Requires a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " to release radbolts from storage when the Radbolt threshold is reached.\n\n" +
                        "Radbolts in storage won't decay as long as the modules solar panels can function.");
                }

                public class RTB_HABITATMODULEPLATEDLARGE
                {
                    public static LocString NAME = (LocString)FormatAsLink("Plated Spacefarer Nosecone", nameof(HabitatModuleSmallExpandedConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.EFFECT + "\n\nInterior is fully shielded from radiation.";
                }

                public class RTB_HABITATMODULESMALLEXPANDED
                {
                    public static LocString NAME = (LocString)FormatAsLink("Extended Solo Spacefarer Nosecone", nameof(HabitatModuleSmallExpandedConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.EFFECT;
                }
                public class RTB_HABITATMODULEMEDIUMEXPANDED
                {
                    public static LocString NAME = (LocString)FormatAsLink("Extended Spacefarer Module", nameof(HabitatModuleMediumExpandedConfig));
                    public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULEMEDIUM.DESC;
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULEMEDIUM.EFFECT;
                }
                public class RTB_ROCKETPLATFORMTAG
                {
                    public static LocString NAME = "Rocket Platform";
                }

            }
        }

        public class BUILDING
        {
            public class STATUSITEMS
            {
                public class RTB_MODULEGENERATORNOTPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator will generate " + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + " once traveling through space and fueled\n\nRight now, it's not doing much of anything");
                }

                public class RTB_MODULEGENERATORPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator is producing" + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + "\n\nWhile traveling through space, it will continue generating power as long as are enough resources left");
                }
                public class RTB_MODULEGENERATORALWAYSACTIVEPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator is producing" + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + "\n\nIt will continue generating power as long as are enough resources left");
                }
                public class RTB_MODULEGENERATORALWAYSACTIVENOTPOWERED
                {
                    public static LocString NAME = (LocString)"Power Generation: {ActiveWattage}/{MaxWattage}";
                    public static LocString TOOLTIP = (LocString)("Module generator will generate " + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + " once fueled\n\nRight now, it's not doing much of anything");
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
                public class RTB_ROCKETGENERATORLANDEDACTIVE
                {
                    public static LocString NAME = (LocString)"Active while landed";
                    public static LocString TOOLTIP = (LocString)("This generator will run even while landed");
                }
                public class RTB_FOODSTORAGESTATUS
                {
                    public static LocString NAME = (LocString)"Total Food: {REMAININGMASS}KCal";
                    public static LocString TOOLTIP = (LocString)("The connected Fridge Modules still contain {REMAININGMASS} KCal of Food.{FOODLIST}");
                    public static LocString FOODINFO = "\n • {0}: {1}KCal";
                }
                public class RTB_CRITTERMODULECONTENT
                {
                    public static LocString NAME = (LocString)"Critter Count: {0}/{1}";
                    public static LocString TOOLTIP = (LocString)("{CritterContentStatus}");
                    public static LocString NOCRITTERS = "No Critters stored.";
                    public static LocString HASCRITTERS = "Module currently holds these Critters:";
                    public static LocString CRITTERINFO = " • {CRITTERNAME}, {AGE} Cycles old";
                    public static LocString CRITTERINFOAGELESS = " • {CRITTERNAME}";
                    public static LocString DROPITBUTTON = "Drop all Critters";
                    public static LocString DROPITBUTTONTOOLTIP = "Drop it like its hot";
                    public static LocString UNITS = " Critters";
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
                public class RTB_DOCKEDSTATUS
                {
                    public static LocString NAME = (LocString)"Docked to {SINGLEDOCKED}.";
                    public static LocString TOOLTIP = (LocString)("Currently docked to: {DOCKINGLIST}");
                    public static LocString DOCKEDINFO = "\n • {0}";
                    public static LocString MULTIPLES = "multiple crafts";
                }
            }
        }

        public class UI
        {
            public static class CLUSTERLOCATIONSENSORADDON
            {
                public static LocString TITLE = "Extra green signal at";
            }

            public class DOCKINGSCREEN
            {
                public static class TITLE
                {
                    public static LocString TITLETEXT = "Docking Management";
                }

                public static class DOCKINGBRIDGES
                {
                    public static LocString TITLETEXT = "Available Docking Ports: {0} / {1}";
                }

                public static class OWNDUPESCONTAINER
                {
                    public static class SCROLLRECTCONTAINER
                    {
                        public static class ITEMPREFAB
                        {

                            public static LocString DOCKINGTOOLTIP = "Dock to this craft.";
                            public static LocString UNDOCKINGTOOLTIP = "Undock from this craft.";

                            public static class ROW1
                            {
                                public static LocString TITLETEXT = "{SpacecraftName}";
                            }
                            public static class ROW2
                            {
                                public static class TRANSFERBUTTON
                                {
                                    public static LocString LABEL = "Transfer Duplicants between Crafts";
                                }
                                public static class VIEWDOCKEDBUTTON
                                {
                                    public static LocString LABEL = "View docked Interior";
                                    public static LocString TOOLTIP = "View the interior this docking tube is currently connected to.";
                                    public static LocString TOOLTIPNOINTERIOR = "No interior found";

                                }
                            }
                        }
                    }
                }

            }
            public class DOCKINGTRANSFERSCREEN
            {
                public static class TITLE
                {
                    public static LocString TITLETEXT = "Crew Management";
                }

                public static class CONTENTHEADEROWN
                {
                    public static LocString TITLETEXT = "Duplicants assigned to this Spacecraft:";
                }
                public static class CONTENTHEADERDOCKED
                {
                    public static LocString TITLETEXT = "Duplicants assigned the docked Spacecraft:";
                }
                public static class OWNDUPESCONTAINER
                {
                    public static class SCROLLRECTCONTAINER
                    {
                        public static class NODUPESASSIGNED
                        {
                            public static LocString TITLETEXT = "No Duplicants are currently assigned to this spacecraft.";
                        }
                    }
                }
                public static class TARGETDUPESCONTAINER
                {
                    public static class SCROLLRECTCONTAINER
                    {
                        public static class NODUPESASSIGNED
                        {
                            public static LocString TITLETEXT = "No Duplicants are currently assigned to this spacecraft.";
                        }
                    }
                }
                public static LocString ASSIGNTOOTHERBUTTONTEXT = "Assign Duplicant to the other Spacecraft";
                 
            }
            public static class ROCKETGENERATOR
            {
                public static LocString BUTTONTEXT = (LocString)"Toggle On/Off";
                public static LocString TOOLTIP = (LocString)"Toggle the Generator producing power while landed";
            }
            public class NEWBUILDCATEGORIES
            {
                public static class ROCKETFUELING
                {
                    public static LocString NAME = (LocString)"Rocket Fueling";
                    public static LocString BUILDMENUTITLE = (LocString)"Rocket Fueling";
                    public static LocString TOOLTIP = (LocString)"";
                }

            }
        }

        public class UI_MOD
        {
            public class CLUSTERMAPROCKETSIDESCREEN
            {

                public class ROCKETDIMENSIONS
                {

                    public static LocString NAME = "Height: {0}/{1}, max. Width: {2} ";

                    public static LocString NAME_RAW = "Height: ";

                    public static LocString NAME_MAX_SUPPORTED = "Maximum supported rocket height: ";
                    public static LocString MODULECOUNT = "Number of Modules: ";
                    public static LocString MODULEORDER = "Rocket Modules:\n";

                    public static LocString TOOLTIP = "The {0} can support a total rocket height {1}\nThe maximum width of the rocket is {2} tiles.";
                }
                public class ROCKETBATTERYSTATUS
                {
                    public static LocString NAME = (LocString)"Battery Module Charge: {0}/{1}";
                }
                public class ROCKETGENERATORSTATS
                {
                    public static LocString NAME = (LocString)"Power Generation: {0}/{1}";
                    public static LocString TOOLTIP = "\n({0}/{1} fuel remaining)";
                    public static LocString TOOLTIP2 = "\n(Check cargo bays for fuel type: {0})";
                }
            }
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
            public class COLLAPSIBLEWORLDSELECTOR
            {
                public static LocString DERELICS = (LocString)"Derelics";
                public static LocString ROCKETS = (LocString)"Rockets";
            }
        }


        public class OPTIONS_ROCKETRYEXPANDED
        {
            public const string UNITDESCRIPTION = "Only has an effect if the option \"Rebalanced Cargo Bays\" is enabled.\n" +
                "Cargo bay capacity is calculated by \"Unit-KG * Cargobay-Unit-Count\"\nUnit-Count per size:\n- Small: 9 Units\n- Large: 28 Units\n- Colossal: 64 Units.\n\nAdjust the Unit-KG for this cargo bay type below.";

            public const string TOGGLESINGLE = "Toggle to enable/disable this module in the building menu";
            public const string TOGGLEMULTI = "Toggle to enable/disable these modules in the building menu";
        }
        public class MODIFIEDVANILLASTRINGS
        {
            public static LocString KEROSENEENGINECLUSTERSMALL_EFFECT = (LocString)("Burns either " + FormatAsLink("Petroleum", "PETROLEUM") + " or " + FormatAsLink("Ethanol", "ETHANOL") + " to propel rockets for mid-range space exploration.\n\nSmall Petroleum Engines possess the same speed as a " + FormatAsLink("Petroleum Engines", "KEROSENEENGINE") + " but have smaller height restrictions.\n\nEngine must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
            public static LocString KEROSENEENGINECLUSTER_EFFECT = (LocString)("Burns either " + FormatAsLink("Petroleum", "PETROLEUM") + " or " + FormatAsLink("Ethanol", "ETHANOL") + " to propel rockets for mid-range space exploration.\n\nPetroleum Engines have generous height restrictions, ideal for hauling many modules.\n\nEngine must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
        }
        public class RESEARCH
        {
            public class TECHS
            {
                public class RTB_FUELLOADERSTECH
                {
                    public static LocString NAME = FormatAsLink("Fuel Loaders", nameof(RTB_FUELLOADERSTECH));
                    public static LocString DESC = "Automatically refuel your rockets with these Loaders.\nCan be placed inside a space station.";

                }
                public class RTB_DOCKINGTECH
                {
                    public static LocString NAME = FormatAsLink("Celestial Connection", nameof(RTB_DOCKINGTECH));
                    public static LocString DESC = "Dock with other spacecrafts";

                }
                public class RTB_LARGERROCKETLIVINGSPACETECH
                {
                    public static LocString NAME = FormatAsLink("Luxurious Liv'in Space", nameof(RTB_LARGERROCKETLIVINGSPACETECH));
                    public static LocString DESC = "All the living space a dupe could ask for, now in your rocket.";

                }
                public class RTB_SPACESTATIONTECH
                {
                    public static LocString NAME = FormatAsLink("Deep Space Exploration", nameof(RTB_SPACESTATIONTECH));
                    public static LocString DESC = "Mysterious Artifacts have shown new perspectives on living in the vast emptyness";
                }
                public class RTB_MEDIUMSPACESTATIONTECH
                {
                    public static LocString NAME = FormatAsLink("Deep Space Colonization", nameof(RTB_MEDIUMSPACESTATIONTECH));
                    public static LocString DESC = "Extending the perspective";
                }
                public class RTB_LARGESPACESTATIONTECH
                {
                    public static LocString NAME = FormatAsLink("Deep Space Expansion", nameof(RTB_LARGESPACESTATIONTECH));
                    public static LocString DESC = "Conquering the depths of space";
                }
                public class RTB_HUGECARGOBAYTECH
                {
                    public static LocString NAME = FormatAsLink("Thinking larger", nameof(RTB_LARGESPACESTATIONTECH));
                    public static LocString DESC = "Lets bring home ALL those minerals.";
                }
            }
        }
    }
}
