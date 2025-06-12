using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Nosecones;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.ELEMENTS;
using static STRINGS.UI;

namespace Rockets_TinyYetBig
{
	public class STRINGS
	{
		public class ITEMS
		{
			public class INDUSTRIAL_PRODUCTS
			{
				public class RTB_EMPTYDATACARD
				{
					public static LocString NAME = FormatAsLink("Empty Data Card", "RTB_EMPTYDATACARD");
					public static LocString NAME_PLURAL = FormatAsLink("Empty Data Cards", "RTB_EMPTYDATACARD");
					public static LocString DESC = "Empty data card that is required to capture " + RTB_DEEPSPACEINSIGHT.NAME + " at the Deep Space Analyzer";
					public static LocString RECIPE_DESC = "Empty Data Cards, ready to store insights about the depths of space";
				}
				public class RTB_DEEPSPACEINSIGHT
				{
					public static LocString NAME = FormatAsLink("Deep Space Insight", "RTB_DEEPSPACEINSIGHT");
					public static LocString NAME_PLURAL = FormatAsLink("Deep Space Insights", "RTB_DEEPSPACEINSIGHT");
					public static LocString DESC = "Deep space research data that can be processed into " + DEEPSPACERESEARCH.NAME + " points.";
					public static LocString RECIPE_DESC = NAME_PLURAL+ " generated from analyzing the depths of space with a Deep Space Analyzer";
				}
			}			
		}

		public class ROOMS
		{
			public class TYPES
			{
				public class RTB_SPACESTATIONRESEARCHROOM
				{
					public static LocString NAME = "Orbital Research Lab";
					public static LocString EFFECT = "- Efficiency bonus";
					public static LocString TOOLTIP = "Science buildings built in an Orbital Research Lab function more efficiently\n\nAn Orbital Research Lab enables Deep Space Analyzer use";
					public class ROOMCONSTRAINT
					{
						public static LocString NAME = "Four " + FormatAsLink("science buildings", "REQUIREMENTCLASSSCIENCEBUILDING");

						public static LocString DESCRIPTION = "Requires four or more science buildings";

						public static LocString CONFLICT_DESCRIPTION = NAME;
					}
				}
			}
		}
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
				public static LocString RTB_DOCKINGTUBEATTACHMENTSLOT = "Spacefarer Docking Tube Attachment Port";
				public static LocString RTB_RADIATIONSHIELDINGROCKETCONSTRUCTIONMATERIAL = "Radiation Shielding";

				public static LocString RTB_NEUTRONIUMALLOYMATERIAL = "Neutronium Alloy";
				public static LocString RTB_NEUTRONIUMALLOYMATERIAL_DESC = "Neutronium Alloy is an insanely durable " + FormatAsLink("Solid Material", "ELEMENTS_SOLID") + " used in the construction of large space structures.";

				public static LocString RTB_ROCKETFUELMATERIAL = "Rocket Fuel";
				public static LocString RTB_OXIDIZERCORROSIVEREQUIREMENT = "Corrosive Liquid Oxidizer";
				public static LocString RTB_OXIDIZERLOXTANK = "Supercooled Liquid Oxidizer";
				public static LocString RTB_OXIDIZEREFFICIENCY_1 = "Oxidizer Efficiency 1";
				public static LocString RTB_OXIDIZEREFFICIENCY_2 = "Oxidizer Efficiency 2";
				public static LocString RTB_OXIDIZEREFFICIENCY_3 = "Oxidizer Efficiency 3";
				public static LocString RTB_OXIDIZEREFFICIENCY_4 = "Oxidizer Efficiency 4";
				public static LocString RTB_OXIDIZEREFFICIENCY_5 = "Oxidizer Efficiency 5";
				public static LocString RTB_OXIDIZEREFFICIENCY_6 = "Oxidizer Efficiency 6";

				public static LocString KATAIRITE = global::STRINGS.ELEMENTS.KATAIRITE.NAME;
			}

		}

		public class ELEMENTS
		{
			public class SPACESTATIONFORCEFIELD
			{
				public static LocString NAME = FormatAsLink("Station Force Field", nameof(SPACESTATIONFORCEFIELD));
				public static LocString DESC = "A force field, protecting the station against micro meteors.";
			}
			public class UNOBTANIUMALLOY
			{
				public static LocString NAME = FormatAsLink("Neutronium Alloy", nameof(UNOBTANIUMALLOY));
				public static LocString DESC = "An insanely durable and heat resistant alloy.\nRequired in the construction of large space structures.\nVery sparkly";
				public static LocString RECIPE_DESCRIPTION = "Neutronium Alloy is a " + FormatAsLink("Solid Material", "ELEMENTS_SOLID") + " used in the construction of large space structures.";
			}
			public class UNOBTANIUMDUST
			{
				public static LocString NAME = FormatAsLink("Neutronium Dust", nameof(UNOBTANIUMDUST));
				public static LocString DESC = "Harvested from artifact research, this dust might have some useful properties.\nCan be forged into " +
					FormatAsLink("Neutronium Alloy", nameof(UNOBTANIUMALLOY)) + " at the " + FormatAsLink("Molecular Forge", nameof(SUPERMATERIALREFINERY));
			}
		}
		public class DEEPSPACERESEARCH
		{
			public static LocString NAME = FormatAsLink("Deep Space Research",nameof(DEEPSPACERESEARCH));
			public static LocString UNLOCKNAME = (PRE_KEYWORD + NAME + PST_KEYWORD + " Capability");
			public static LocString UNLOCKDESC = ("Allows " + PRE_KEYWORD + NAME + PST_KEYWORD + " points to be accumulated, unlocking higher technology tiers.\nCan be accumulated before research completion via artifact analysis.");
			public static LocString DESC = FormatAsLink("Deep Space Research", nameof(DEEPSPACERESEARCH)) + " is conducted by analyzing the deeper meanings behind mysterious artefacts found in the vastness of deep space and by conducting deep space analysis research in the low artifical gravity of a space station.";
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
			public class CARGOBAYSTORAGE
			{
				public static LocString TITLE = "<b>Module Storage:</b>";

				public static LocString FUELGAS = "\n• {0} of gas fuel storage";
				public static LocString FUELLIQUID = "\n• {0} of liquid fuel storage";
				public static LocString FUELSOLID = "\n• {0} of solid fuel storage";
				public static LocString FUELELEMENT = "\n• {0} of {1} fuel storage";

				public static LocString OXIDIZERGAS = "\n• {0} of gas oxidizer storage";
				public static LocString OXIDIZERLIQUID = "\n• {0} of liquid oxidizer storage";
				public static LocString OXIDIZERSOLID = "\n• {0} of solid oxidizer storage";
				public static LocString OXIDIZERELEMENT = "\n• {0} of {1} oxidizer storage";
				public static LocString OXIDIZERTAG = "\n• {0} of {1} storage";


				public static LocString GASDCARGO = "\n• {0} of gas storage";
				public static LocString LIQUIDCARGO = "\n• {0} of liquid storage";
				public static LocString SOLIDCARGO = "\n• {0} of solid storage";
				public static LocString CRITTERCARGO = "\n• {0} Critters storage";
				public static LocString CRITTERCARGOSINGLE = "\n• 1 Critter storage";
				public static LocString RADBOLTS = "\n• {0} radbolt storage";
				public static LocString ARTIFACT = "\n• 1 artifact storage";
				public static LocString POWER = "\n• {0} power storage";
			}
			public class MODULEPERKS
			{
				public static LocString TITLE = "<b>Module Boni:</b>";
				public static LocString POWER_PRODUCTION = "\n• {0} power production";

			}
		}


		public class BUILDINGS
		{
			public class RTB_CARGOBAY_LOGICPORTS
			{
				public class EMPTY
				{
					public static LocString LOGIC_PORT = (LocString)"Empty/Not Empty";
					public static LocString LOGIC_PORT_ACTIVE = (LocString)("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when completely empty");
					public static LocString LOGIC_PORT_INACTIVE = (LocString)("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));
				}
				public class FULL
				{
					public static LocString LOGIC_PORT = (LocString)"Full/Not Full";
					public static LocString LOGIC_PORT_ACTIVE = (LocString)("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when completely full");
					public static LocString LOGIC_PORT_INACTIVE = (LocString)("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));
				}
			}
			public class PREFABS
			{
				public static LocString GENERATORLIMIT = "\n\n If there is atleast one battery connected, the generator will stop producing if the battery is above 95% charge.";
				public class RTB_LANDERROCKETPLATFORM
				{
					public static LocString NAME = FormatAsLink("Deployable Rocket Platform", nameof(RTB_LANDERROCKETPLATFORM));
					public static LocString DESC = "Perfect for a first landing";
				}
				public class RTB_SOLARPANELMODULE_WIDE
				{
					public static LocString NAME = FormatAsLink("Wide " + SOLARPANELMODULE.NAME, nameof(RTB_SOLARPANELMODULE_WIDE));
					public static LocString DESC = SOLARPANELMODULE.DESC + "\nThat small module looked so off on wide rockets!";
					public static LocString EFFECT = SOLARPANELMODULE.EFFECT;
				}


				public class RTB_POICAPACITYSENSOR
				{
					public static LocString NAME = FormatAsLink("Starmap POI Capacity Sensor", nameof(RTB_POICAPACITYSENSOR));

					public static LocString DESC = "How much stuff is out there?";
					public static LocString EFFECT = ("After pointing the building at a space poi, the building will output logic signals based on the poi artifact existing and the mass remaining in the poi.");
					public static LocString LOGIC_PORT_CAPACITY = "POI mass above set threshold.";
					public static LocString LOGIC_PORT_CAPACITY_ACTIVE = ("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when the targeted POI has more than the configured mass remaining");
					public static LocString LOGIC_PORT_CAPACITY_INACTIVE = ("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));

					public static LocString SIDESCREENTITLE = "Remaining Mass";
					public static LocString REMAININGMASS = "Remaining Mass in POI";
					public static LocString REMAININGMASS_TOOLTIP_ABOVE = ("Will send a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " if the " + PRE_KEYWORD + "remaining Mass in the POI" + PST_KEYWORD + " is above <b>{0}</b>");
					public static LocString REMAININGMASS_TOOLTIP_BELOW = ("Will send a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " if the " + PRE_KEYWORD + "remaining Mass in the POI" + PST_KEYWORD + " is below <b>{0}</b>");


					public static LocString LOGIC_PORT_ARTIFACT = "POI has Artifact";
					public static LocString LOGIC_PORT_ARTIFACT_ACTIVE = ("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when the targeted POI has an artifact");
					public static LocString LOGIC_PORT_ARTIFACT_INACTIVE = ("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));


				}

				public class RTB_FRIDGEMODULEACCESSHATCH
				{
					public static LocString NAME = FormatAsLink("Freezer Access Hatch", nameof(RTB_FRIDGEMODULEACCESSHATCH));
					public static LocString DESC = "Food spoilage can be slowed by ambient conditions as well as by refrigerators.";
					public static LocString EFFECT = ("Has to be attached to the rocket wall.\n\nStores a small amount of " + FormatAsLink("Food", "FOOD") + " at an ideal " + FormatAsLink("Temperature", "HEAT") + " to prevent spoilage.\n\nWill pull food from a connected " + RTB_FRIDGECARGOBAY.NAME + " when low on food.");
					public static LocString LOGIC_PORT = "Full/Not Full";
					public static LocString LOGIC_PORT_ACTIVE = ("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when full");
					public static LocString LOGIC_PORT_INACTIVE = ("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));

					public static LocString LOGIC_PORT_PULL = "Pull food from Freezer Module";
					public static LocString LOGIC_PORT_ACTIVE_PULL = (FormatAsAutomationState("Green Signal", AutomationState.Active) + ": Building will pull more food from the Freezer Module when below 1kg of food.");
					public static LocString LOGIC_PORT_INACTIVE_PULL = (FormatAsAutomationState("Red Signal", AutomationState.Standby) + ": Building wont pull any more food even when below 1kg.");
				}
				public class RTB_DRILLCONEDIAMONDSTORAGE
				{
					public static LocString NAME = FormatAsLink("Drillcone Service Module", nameof(RTB_DRILLCONEDIAMONDSTORAGE));
					public static LocString DESC = "Bringing home those minerals - for Rock and Stone!";
					public static LocString EFFECT = ("Acts as a support module for a normal Drillcone.\n\nProvides additional 1500kg of diamond capacity for the drillcone.\n\nGives a 20% mining speed boost to the drillcone.\n\nCan be toggled between manual loading and automated loading via cargo loader.");
				}

				public class RTB_AIMODULEDOCKINGPORT
				{
					public static LocString NAME = FormatAsLink("Docking Module", nameof(RTB_AIMODULEDOCKINGPORT));
					public static LocString DESC = "Connecting with another (rocket) has never been easier.";
					public static LocString EFFECT = ("Enables docking with other rockets and space stations\n\nBoth docking participants require a docking component to dock.\n\nDuplicants cannot use this docking connection.\n\nAdd it to an AI controlled rocket to allow it to dock.\n\nMultiple Modules allow multiple docking connections");
				}
				
				public class RTB_SPACESTATIONDOCKINGDOOR
				{
					public static LocString NAME = FormatAsLink("[WIP] Orbital Docking Port", nameof(RTB_SPACESTATIONDOCKINGDOOR));
					public static LocString DESC = "Connecting with another (rocket) has never been easier.";
					public static LocString EFFECT = ("Enables docking with rockets to dock\n\nAllows loading and unloading docked rockets\n\nRockets require a docking component to dock.\n\nAssigning a duplicant forces it to use the docking bridge.");
				}
				public class RTB_SPACESTATIONDOCKINGDOOR_INDESTRUCTIBLE
				{
					public static LocString NAME = FormatAsLink("[WIP] Core Orbital Docking Port", nameof(RTB_SPACESTATIONDOCKINGDOOR_INDESTRUCTIBLE));
					public static LocString DESC = "Connecting with another (rocket) has never been easier.";
					public static LocString EFFECT = ("Enables docking with rockets to dock\n\nAllows loading and unloading docked rockets\n\nRockets require a docking component to dock.\n\nAssigning a duplicant forces it to use the docking bridge.");
				}
				public class RTB_DOCKINGTUBEDOOR
				{
					public static LocString NAME = FormatAsLink("Docking Bridge", nameof(RTB_DOCKINGTUBEDOOR));
					public static LocString DESC = "Connecting with another (rocket) has never been easier.";
					public static LocString EFFECT = ("Enables docking with other rockets and space stations\n\nBoth docking participants require a docking component to dock.\n\nAssigning a duplicant forces it to use the docking bridge.");
				}
				public class RTB_NATGASENGINECLUSTER
				{
					public static LocString NAME = FormatAsLink("Natural Gas Engine", nameof(RTB_NATGASENGINECLUSTER));
					public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";
					public static LocString EFFECT = ("Burns " + FormatAsLink("Natural Gas", "METHANE") + " to propel rockets for mid-range space exploration.\n\nEngine must be built via " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME);
				}
				public class RTB_SPACESTATIONMODULEBUILDER
				{
					public static LocString NAME = FormatAsLink("[WIP] Orbital Construction Module", nameof(RTB_SPACESTATIONMODULEBUILDER));
					public static LocString DESC = "TODO";
					public static LocString EFFECT = "Allows construction in deep space.";
				}
				public class RTB_IONENGINECLUSTER
				{
					public static LocString NAME = FormatAsLink("Ion Engine", nameof(RTB_IONENGINECLUSTER));
					public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";
					public static LocString EFFECT = ("Uses large amounts of " + FormatAsLink("Power", "POWER") + " to ionize " + FormatAsLink("Water", "WATER") + ", ejecting the ionized particles outwards to propel rockets for long-range space exploration.\n\nPower is drawn from battery modules.\n\nEngine must be built via " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME);
				}
				public class RTB_SMOLBATTERYMODULE
				{
					public static LocString NAME = FormatAsLink("Small Battery Module", nameof(RTB_SMOLBATTERYMODULE));
					public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.BATTERYMODULE.DESC;
					public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.BATTERYMODULE.EFFECT;
				}
				public class RTB_FRIDGECARGOBAY
				{
					public static LocString NAME = FormatAsLink("Freezer Module", nameof(RTB_FRIDGECARGOBAY));
					public static LocString DESC = "Space food for days";
					public static LocString EFFECT = "Keeps food preserved, prevent spoilage.\nCan only be filled with a cargo loader.\nContents can be accessed during the flight via wall loader";
				}
				public class RTB_VERTICALADAPTERBASE
				{
					public static LocString NAME = FormatAsLink("Vertical Rocket Port Adapter", nameof(RTB_VERTICALADAPTERBASE));
					public static LocString DESC = "Ascending to new levels";
					public static LocString EFFECT = ("A Crosspiece rocket port adapter for vertical expansion.\n\nLinks up with other vertical adapter pieces above and below it.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
				}
				public class RTB_VERTICALADAPTERPIECE
				{
					public static LocString NAME = FormatAsLink("Vertical Rocket Port Piece", nameof(RTB_VERTICALADAPTERPIECE));
					public static LocString DESC = "Gantry Not Included";
					public static LocString EFFECT = ("A rocket port adapter piece for vertical expansion.\n\nLinks up with other vertical adapter pieces above and below it.");
				}
				public class RTB_REINFORCEDLADDER
				{					
					public static LocString NAME = FormatAsLink("Reinforced Ladder", nameof(RTB_REINFORCEDLADDER));
					public static LocString DESC = "Sturdy and quick to climb!";
					public static LocString EFFECT = "Increases duplicant climbing speed. Immune to meteors";
				}
				public class RTB_DEEPSPACERESEARCHTELESCOPE
				{
					public static LocString NAME = FormatAsLink("Deep Space Analysis Station", nameof(RTB_DEEPSPACERESEARCHTELESCOPE));
					public static LocString DESC = "Deep space analysis stations scan and collect data on deep space anomalies";
					public static LocString EFFECT = ("Allows " + DEEPSPACERESEARCH.NAME + " to be accumulated.\n\nRequires a " + RTB_DEEPSPACEINSIGHT.NAME + " to function.");
				}
				public class RTB_DEEPSPACERESEARCHCENTER
				{
					public static LocString NAME = FormatAsLink("Quantum Computer", nameof(RTB_DEEPSPACERESEARCHCENTER));
					public static LocString DESC = "Quantum computers unlock new breakthroughs from the analysis of deep space anomalies.\n\nRequires active cooling to function optimally";
					public static LocString EFFECT = ("Conducts " + DEEPSPACERESEARCH.NAME + " to unlock new technologies.\n\nConsumes " + RTB_DEEPSPACEINSIGHT.NAME_PLURAL + ".\n\nAssigned Duplicants must possess the " + global::STRINGS.DUPLICANTS.ROLES.SPACE_RESEARCHER.NAME + " skill.");
				}
				public class RTB_WALLCONNECTIONADAPTER
				{
					public static LocString NAME = FormatAsLink("Insulated Rocket Port Wall Adapter", nameof(RTB_WALLCONNECTIONADAPTER));
					public static LocString DESC = "Insulated for convenience.";
					public static LocString EFFECT = ("An insulated wall adapter to seal off rocket start areas.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
				}
				public class RTB_WALLCONNECTIONADAPTERBUNKER
				{
					public static LocString NAME = FormatAsLink("Bunkered Rocket Port Wall Adapter", nameof(RTB_WALLCONNECTIONADAPTERBUNKER));
					public static LocString DESC = "Bunkered down for convenience.";
					public static LocString EFFECT = ("A bunker wall adapter to seal off rocket start areas.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
				}
				public class RTB_LADDERCONNECTIONADAPTER
				{
					public static LocString NAME = FormatAsLink("Rocket Port Ladder Adapter", nameof(RTB_WALLCONNECTIONADAPTER));
					public static LocString DESC = "Connecting rocket platforms, now with verticality";
					public static LocString EFFECT = ("Connects adjacent rocket platforms while doubling as a ladder.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
				}
				public class RTB_HEPFUELLOADER
				{
					public static LocString NAME = FormatAsLink("Radbolt Loader", nameof(RTB_HEPFUELLOADER));
					public static LocString DESC = "\"Shoving everything in there\" - now with higly energized particles.";
					public static LocString EFFECT = ("Fills all sorts of Radbolt Storages.\nAllows fueling a Radbolt Engine and the Laser Drillcone\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
				}
				public class RTB_UNIVERSALFUELLOADER
				{
					public static LocString NAME = FormatAsLink("Rocket Fuel Loader", nameof(RTB_UNIVERSALFUELLOADER));
					public static LocString DESC = "Fueling Rockets has never been easier!";
					public static LocString EFFECT = ("Refuels connected rockets with the appropriate fuel.\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);

					public static LocString LOGIC_PORT_ROCKETLOADER = "Currently Loading/Unloading";
					public static LocString LOGIC_PORT_ROCKETLOADER_ACTIVE = ("Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when building is currently loading or unloading a rocket");
					public static LocString LOGIC_PORT_ROCKETLOADER_INACTIVE = ("Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby));

				}
				public class RTB_UNIVERSALOXIDIZERLOADER
				{
					public static LocString NAME = FormatAsLink("Rocket Oxidizer Loader", nameof(RTB_UNIVERSALOXIDIZERLOADER));
					public static LocString DESC = "Fueling Rockets has never been easier!";
					public static LocString EFFECT = ("Refuels connected rockets with the appropriate oxidizer\n\nAutomatically links when built to the side of a " + global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + " or another " + global::STRINGS.BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME);
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
						"\n" + FormatAsAutomationState("Bit 2", AutomationState.Active) + " allows to override the \"Fueled\" Warning under Cargo Manifest that otherwise prevent automated launches. This allows rockets to fly one-way-trips to another launchpad" +
						"\n" + FormatAsAutomationState("Bit 3", AutomationState.Active) + " allows to override all Cargo Warnings (that aren't Fuel) under Cargo Manifest that otherwise prevent automated launches." +
						"\n" + FormatAsAutomationState("Bit 4", AutomationState.Active) + " makes the Overrides of Bit 2 and 3 affect the logic output of the launch pad. ";

					public static LocString LOGIC_PORT_LAUNCH_INACTIVE_RIBBON = FormatAsAutomationState("Red Signal on the first Bit", AutomationState.Standby) + ": Cancel launch";
				}
				public class RTB_RTGGENERATORMODULE
				{
					public static LocString NAME = FormatAsLink("Radioisotope Thermoelectric Generator", nameof(RTB_RTGGENERATORMODULE));
					public static LocString DESC = "Through exploitation of the natural decay of enriched Uranium, this elegantly simple power generator can provide consistent, stable power for hundreds of cycles.";
					public static LocString EFFECT = (string.Format("After adding {0} kg of enriched Uranium, this module will constantly produce {1} W of energy until all of the uranium is depleted.\n\nIt will take {2} Cycles for the uranium to decay.", RTGModuleConfig.UraniumCapacity, RTGModuleConfig.energyProduction, Config.Instance.IsotopeDecayTime));
				}
				public class RTB_STEAMGENERATORMODULE
				{
					public static LocString NAME = FormatAsLink("Steam Generator Module", nameof(SteamGeneratorModuleConfig));
					public static LocString DESC = "Useful for converting hot steam into usable power.";
					public static LocString EFFECT = "Draws in " + FormatAsLink("Steam", "STEAM") + " from gas storage modules and uses it to generate electrical " + FormatAsLink("Power", "POWER") + ".\n\n If there are liquid storage modules with appropriate filters set, outputs hot " + FormatAsLink("Water", "WATER") + " to them." + GENERATORLIMIT;
				}
				public class RTB_GENERATORCOALMODULE
				{
					public static LocString NAME = FormatAsLink("Coal Generator Module", nameof(CoalGeneratorModuleConfig));
					public static LocString DESC = ("Converts " + FormatAsLink("Coal", "CARBON") + " into electrical " + FormatAsLink("Power", "POWER") + ".");
					public static LocString EFFECT = "Burning coal produces more energy than manual power, who could have thought this also works in space." + GENERATORLIMIT;
					public static LocString SIDESCREEN_TOOLTIP = "Duplicants will be requested to deliver " + PRE_KEYWORD + "{0}" + PST_KEYWORD + " when the amount stored falls below <b>{1}</b>";
				}


				public class RTB_HABITATMODULESTARGAZER
				{
					public static LocString NAME = FormatAsLink("Stargazer Nosecone", nameof(HabitatModuleStargazerConfig));
					public static LocString DESC = "The stars have never felt this close before like in this Command Module.";
					public static LocString EFFECT = ("Closes during starts and landings to protect the glass\n\n" +
														"Functions as a Command Module and a Nosecone.\n\n" +
														"One Command Module may be installed per rocket.\n\n" +
													"Must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME +
											". \n\nMust be built at the top of a rocket.\n\nGreat for looking at the stars or a nice sunbathing during the flight.");
				}
				public class RTB_CRITTERCONTAINMENTMODULE
				{
					public static LocString NAME = FormatAsLink("[DEPRECATED] Critter Containment Module", nameof(CritterContainmentModuleConfigOLD));
					public static LocString EFFECT = "This module allows the safe transport of critters to their new home. ";
					public static LocString DESC = "These critters will go where no critter has gone before.";
				}
				public class RTB_CRITTERSTASISMODULE
				{
					public static LocString NAME = FormatAsLink("Critter Stasis Module", nameof(CritterStasisModuleConfig));
					public static LocString EFFECT = "This module allows the safe transport of critters to their new home.\n\nStored Critters wont age.";
					public static LocString DESC = "These critters will go where no critter has gone before.";
				}

				public class RTB_CARGOBAYCLUSTERLARGE
				{
					public static LocString NAME = FormatAsLink("Colossal Cargo Bay", nameof(RTB_CARGOBAYCLUSTERLARGE));
					public static LocString DESC = "Holds even more than a large cargo bay!";
					public static LocString EFFECT = ("Allows Duplicants to store most of the " + FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " found during space missions.\n\nStored resources become available to the colony upon the rocket's return. \n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
				}

				public class RTB_LIQUIDCARGOBAYCLUSTERLARGE
				{
					public static LocString NAME = FormatAsLink("Colossal Liquid Cargo Tank", nameof(RTB_LIQUIDCARGOBAYCLUSTERLARGE));
					public static LocString DESC = "Holds even more than a large cargo tank!";
					public static LocString EFFECT = ("Allows Duplicants to store most of the " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.\n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
				}
				public class RTB_GASCARGOBAYCLUSTERLARGE
				{
					public static LocString NAME = FormatAsLink("Colossal Gas Cargo Canister", nameof(RTB_GASCARGOBAYCLUSTERLARGE));
					public static LocString DESC = "Holds even more than a large gas cargo canister!";
					public static LocString EFFECT = ("Allows Duplicants to store most of the " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.\n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".");
				}
				public class RYB_NOSECONEHEPHARVEST
				{
					public static LocString NAME = FormatAsLink("Laser Drillcone", nameof(NoseConeHEPHarvestConfig));
					public static LocString DESC = "Harvests resources from the universe with the power of radbolts and lasers";
					public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEHARVEST.EFFECT;
				}
				public class RTB_CO2FUELTANK
				{
					public static LocString NAME = FormatAsLink("Carbon Dioxide Fuel Tank", nameof(RTB_CO2FUELTANK));
					public static LocString DESC = "Storing additional fuel increases the distance a rocket can travel before returning.";
					public static LocString EFFECT = ("Stores pressurized " + FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " for " + FormatAsLink("Carbon Dioxide Engines", CO2EngineConfig.ID));
				}

				public class RTB_LIQUIDCHLORINEOXIDIZERTANK
				{
					public static LocString NAME = FormatAsLink("Liquid Chlorine Oxidizer Tank", nameof(RTB_LIQUIDCHLORINEOXIDIZERTANK));
					public static LocString DESC = "Liquid chlorine improves the thrust-to-mass ratio of rocket fuels.";
					public static LocString EFFECT = ("Stores " + FormatAsLink("Liquid Chlorine", nameof(CHLORINE)) + " for burning rocket fuels. \n\nMust be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ".\n\nThe oxidizer efficiency of liquid chlorine sits between those of oxylite and liquid oxygen (3).");
				}
				public class RTB_LIQUIDFUELTANKCLUSTERSMALL
				{
					public static LocString NAME = FormatAsLink("Small Liquid Fuel Tank", nameof(RTB_LIQUIDFUELTANKCLUSTERSMALL));
					public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANKCLUSTER.DESC;
					public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.LIQUIDFUELTANKCLUSTER.EFFECT;
				}

				public class RTB_NOSECONESOLAR
				{
					public static LocString NAME = FormatAsLink("Solar Nosecone", nameof(NoseConeSolarConfig));
					public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.NOSECONEBASIC.DESC;
					public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEBASIC.EFFECT + "\n\n" +
						"Converts " + FormatAsLink("Sunlight", "LIGHT") + " into electrical " + FormatAsLink("Power", "POWER") + " for use on rockets.\n\nMust be exposed to space.";
				}


				public class RTB_HEPBATTERYMODULE
				{
					public static LocString NAME = FormatAsLink("Radbolt Chamber Module", nameof(HEPBatteryModuleConfig));
					public static LocString DESC = "Particles packed up and ready to visit the stars.";
					public static LocString EFFECT = ("Stores Radbolts in a high-energy state, ready for transport.\n\n" +
						"Requires a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " to release radbolts from storage when the Radbolt threshold is reached.\n\n" +
						"Radbolts in storage won't decay as long as the modules solar panels can function.\n\n" +
						"Automatically refills a " + RYB_NOSECONEHEPHARVEST.NAME);
				}

				public class RTB_HABITATMODULEPLATEDLARGE
				{
					public static LocString NAME = FormatAsLink("Plated Spacefarer Nosecone", nameof(HabitatModuleSmallExpandedConfig));
					public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.DESC;
					public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.EFFECT + "\n\nInterior is fully shielded from radiation.";
				}

				public class RTB_HABITATMODULESMALLEXPANDED
				{
					public static LocString NAME = FormatAsLink("Extended Solo Spacefarer Nosecone", nameof(HabitatModuleSmallExpandedConfig));
					public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.DESC;
					public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.HABITATMODULESMALL.EFFECT;
				}
				public class RTB_HABITATMODULEMEDIUMEXPANDED
				{
					public static LocString NAME = FormatAsLink("Extended Spacefarer Module", nameof(HabitatModuleMediumExpandedConfig));
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
					public static LocString NAME = "Power Generation: {ActiveWattage}/{MaxWattage}";
					public static LocString TOOLTIP = ("Module generator will generate " + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + " once traveling through space and fueled\n\nRight now, it's not doing much of anything");
				}

				public class RTB_MODULEGENERATORPOWERED
				{
					public static LocString NAME = "Power Generation: {ActiveWattage}/{MaxWattage}";
					public static LocString TOOLTIP = ("Module generator is producing" + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + "\n\nWhile traveling through space, it will continue generating power as long as are enough resources left");
				}
				public class RTB_MODULEGENERATORALWAYSACTIVEPOWERED
				{
					public static LocString NAME = "Power Generation: {ActiveWattage}/{MaxWattage}";
					public static LocString TOOLTIP = ("Module generator is producing" + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + "\n\nIt will continue generating power as long as are enough resources left");
				}
				public class RTB_MODULEGENERATORALWAYSACTIVENOTPOWERED
				{
					public static LocString NAME = "Power Generation: {ActiveWattage}/{MaxWattage}";
					public static LocString TOOLTIP = ("Module generator will generate " + FormatAsPositiveRate("{MaxWattage}") + " of " + PRE_KEYWORD + "Power" + PST_KEYWORD + " once fueled\n\nRight now, it's not doing much of anything");
				}
				public class RTB_MODULEGENERATORFUELSTATUS
				{
					public static LocString NAME = "Generator Fuel Capacity: {CurrentFuelStorage}/{MaxFuelStorage}";
					public static LocString TOOLTIP = ("This {GeneratorType} has {CurrentFuelStorage} out of {MaxFuelStorage} available.");
				}
				public class RTB_ROCKETBATTERYSTATUS
				{
					public static LocString NAME = "Battery Module Charge: {CurrentCharge}/{MaxCharge}";
					public static LocString TOOLTIP = ("This Rocket has {CurrentCharge}/{MaxCharge} stored in battery modules.");
				}
				public class RTB_ROCKETGENERATORLANDEDACTIVE
				{
					public static LocString NAME = "Active while landed";
					public static LocString TOOLTIP = ("This generator will run even while landed");
				}
				public class RTB_FOODSTORAGESTATUS
				{
					public static LocString NAME = "Total Food: {REMAININGMASS}KCal";
					public static LocString TOOLTIP = ("The connected Fridge Modules still contain {REMAININGMASS} KCal of Food.{FOODLIST}");
					public static LocString FOODINFO = "\n • {0}: {1}KCal";
				}
				public class RTB_CRITTERMODULECONTENT
				{
					public static LocString NAME = "Critter Count: {0}/{1}";
					public static LocString TOOLTIP = ("{CritterContentStatus}");
					public static LocString NOCRITTERS = "No Critters stored.";
					public static LocString HASCRITTERS = "Module currently holds these Critters:";
					public static LocString CRITTERINFO = " • {CRITTERNAME}, {AGE} Cycles old";
					public static LocString CRITTERINFOAGELESS = " • {CRITTERNAME}";
					public static LocString DROPITBUTTON = "Drop all Critters";
					public static LocString DROPITBUTTONTOOLTIP = "Drop it like its hot";
					public static LocString UNITS = " Critters";
				}
				public class RTB_MININGINFORMATIONBOONS
				{
					public static LocString NAME = "Mining Boost: {RATEPERCENTAGE}";
					public static LocString TOOLTIP = "{TOOLTIP}";
					public static LocString TOOLTIPINFO = ("This mining operation is boosted by {RATEPERCENTAGE},\ncurrently mining {YIELDMASS} and consuming {DRILLMATERIALMASS} {DRILLMATERIAL} per second.\nBoosting Factors:\n");

					public static LocString PILOTSKILL = " • Pilot skills: {BOOSTPERCENTAGE}% (affected by piloting and digging skill)";
					public static LocString SUPPORTMODULE = " • Drillcone Maintainance Modules: {BOOSTPERCENTAGE}% (x{COUNT} Modules)";
					public static LocString SUPPORTMODULESINGULAR = " • Drillcone Maintainance Module: {BOOSTPERCENTAGE}% (x1 Module)";

				}

				public class RTB_STATIONCONSTRUCTORSTATUS
				{
					public static LocString NAME = "Module Status: {STATUS}";
					public static LocString IDLE = ("Nominal");
					public static LocString NONEQUEUED = ("No active process");
					public static LocString TIMEREMAINING = ("Time until current operation is completed: {TIME}");
					public static LocString TOOLTIP = ("{TOOLTIP}");
					public static LocString CONSTRUCTING = ("Constructing: {TIME} left");
					public static LocString DECONSTRUCTING = ("Demolishing: {TIME} left");

				}
				public class RTB_DOCKEDSTATUS
				{
					public static LocString NAME = "Docked to {SINGLEDOCKED}.";
					public static LocString TOOLTIP = ("Currently docked to: {DOCKINGLIST}");
					public static LocString DOCKEDINFO = "\n • {0}";
					public static LocString MULTIPLES = "multiple crafts";
				}
			}
		}

		public class UI
		{
			public static LocString PRODUCTINFO_SPACE_STATION_INTERIOR = "Space station interior only";
			public static LocString PRODUCTINFO_SPACE_STATION_NOT_INTERIOR = "Cannot build inside space station";
			public class RTB_RESEACH_UNLOCK
			{
				public static LocString TEXT = "Eureka! We've decrypted the abandoned station computer's salvageable data.\n\nWe now have the ability to construct habitat stations in the vast emptyness of space!\n\nNew Research has become available.";
				
			}

			public static LocString TOOLTIP_ADDON_RTB = UIUtils.ColorText(UIUtils.EmboldenText("Rocketry Expanded"), "a0a0a0") + " Content"; //light grey
			public class KLEI_INVENTORY_SCREEN
			{
				public class SUBCATEGORIES
				{
					public static LocString RTB_MODULE_SKINS = "Rocket Modules";
				}
			}
			public static class DRILLCONE_MODEHANDLER_SIDESCREEN
			{
				public static LocString LABEL = "Load via Cargo Loader";
				public static LocString TOOLTIP = "toggle between automatic and manual loading";
			}
			public static class CLUSTERLOCATIONSENSORADDON
			{
				public static LocString TITLE = "Extra green signal at";
			}
			public class SPACEASSEMBLEMENU_SIDESCREEN
			{
				public static class TITLE
				{
					public static LocString TITLETEXT = "Space Construction";
				}

				public static class PROJECTHEADER
				{
					public static class ROW1
					{
						public static LocString TITLETEXT = "No space construction project here.";
						public static class CREATENEW
						{

							public static LocString LABEL = "Create new construction project";
							public static LocString LABELCANCEL = "Cancel current construction project";
						}
					}
					public static class COSTCONTAINER
					{
						public static LocString TITLETEXT = "Total Progress:";
						public static LocString PARTCOUNT = "{0}/{1}";
					}
				}
				public static LocString TIMEREMAINING = "{0} seconds remaining";
				public static LocString STARTCONSTRUCTION = "Start Construction";
				public static LocString CANCELCONSTRUCTION = "Cancel Construction";
				public static LocString STARTDECONSTRUCTION = "Start Deconstruction";
				public static LocString CANCELDECONSTRUCTION = "Cancel Deconstruction";
			}
			public class CONSTRUCTIONSELECTOR_SECONDARYSIDESCREEN
			{
				public static class TITLE
				{
					public static LocString TITLETEXT = "Available Construction Projects";
				}

				public static class PROJECTSCONTAINER
				{
					public static class SCROLLRECTCONTAINER
					{
						public static class PARTCONTAINERPREFAB
						{
							public static class ROW1
							{
								public static class CONSTRUCTBTN
								{
									public static LocString LABEL = "Start Project";
								}
							}
						}
					}
				}

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

				public static LocString DUPESASSIGNEDTO = "Duplicants assigned to \"{0}\"";
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
				public static LocString TITLE = "Generator State";
				public static LocString BUTTONTEXT = "Active while landed";
				public static LocString TOOLTIP = "Toggle the Generator producing power while landed.";
			}
			public class NEWBUILDCATEGORIES
			{
				public static class ROCKETFUELING
				{
					public static LocString NAME = "Rocket Fueling";
					public static LocString BUILDMENUTITLE = "Rocket Fueling";
					public static LocString TOOLTIP = "";
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
					public static LocString NAME = "Battery Module Charge: {0}/{1}";
				}
				public class ROCKETGENERATORSTATS
				{
					public static LocString NAME = "Power Generation: {0}/{1}";
					public static LocString TOOLTIP = "\n({0}/{1} fuel remaining)";
					public static LocString TOOLTIP2 = "\n(Check cargo bays for fuel type: {0})";
				}
			}
			public class UISIDESCREENS
			{
				public class SPACESTATIONSIDESCREEN
				{
					public static LocString VIEW_WORLD_TOOLTIP = "View Space Station Interior";
					public static LocString TITLE = "Space Station";

					public static LocString VIEW_WORLD_DESC = "Oversee Station Interior";
				}
				public class SPACECONSTRUCTIONSITE
				{
					public static LocString TITLE = "Space Construction Site";
					public static LocString DESC = "This construction site will produce a {0} once all these parts are assembled:";

				}
			}
			public class FLUSHURANIUM
			{
				public static LocString BUTTON = "Flush Generator Fuel";
				public static LocString BUTTONINFO = "Empties the generators storage to allow a refill.";
			}
			public class CONSTRAINTS
			{
				public class ONE_MODULE_PER_ROCKET
				{

					public static LocString COMPLETE = "";
					public static LocString FAILED = "    • There already is a module of this type on this rocket";
				}
			}
			public class COLLAPSIBLEWORLDSELECTOR
			{
				public static LocString DERELICS = "Derelics";
				public static LocString ROCKETS = "Rockets in space";
			}
		}


		public class OPTIONS_ROCKETRYEXPANDED
		{
			public static LocString TOGGLESINGLE = "Toggle to enable/disable this module in the building menu";
			public static LocString TOGGLEMULTI = "Toggle to enable/disable these modules in the building menu";

			public class CATEGORIES
			{
				public static LocString A_ROCKETRYPLUS = "Rocketry Vanilla+";
				public static LocString B_MININGSHIPPING = "Mining & Shipping";
				public static LocString C_FUELLOGISTICS = "Fuel & Logistics";
				public static LocString D_POWERUTILITY = "Power & Utility";
				public static LocString E_SPACEEXPANSION = "Space Expansion";
				public static LocString F_EASTEREGGS = "Easter Eggs";
			}
			public class SPICEEYES
			{
				public static LocString TITLE = "Dune Spice";
				public static LocString TOOLTIP = "When consuming Rocketeer Spice, Dupes will gain the spice eyes from Dune.";
			}
			public class NEUTRONIUMMATERIAL
			{
				public static LocString TITLE = "Neutronium Alloy";
				public static LocString TOOLTIP = "Gather Neutronium Dust by analyzing artifacts and refine it into Neutronium Alloy.\nNeutronium Alloys are required in the construction of large space structures";
			}
			public class ROCKETDOCKING
			{
				public static LocString TITLE = "Enable Rocket Docking";
				public static LocString TOOLTIP = "Dock rockets in space to transfer dupes and contents of the interiors";
			}
			public class SPACESTATIONSANDTECH
			{
				public static LocString TITLE = "[ALPHA] Space Stations & Deep Space Science";
				public static LocString TOOLTIP = "Unlocks space stations. This feature is work in progress, unfinished and might contain bugs.\nRequires Neutronium Alloy, Compressed Interiors, advanced world selector to activate";
			}
			public class ISOTOPEDECAYTIME
			{
				public static LocString TITLE = "Radioisotope Decay time";
				public static LocString TOOLTIP = "Time in cycles for all the enriched uranium in the RTG to decay into depleted uranium. RTG needs a refill if all enriched uranium has decayed.";
			}
			public class ENABLESMOLBATTERY
			{
				public static LocString TITLE = "Enable Small Battery Module";
			}
			public class ENABLEGENERATORS
			{
				public static LocString TITLE = "Enable Generator Modules";
			}
			public class ENABLESOLARNOSECONE
			{
				public static LocString TITLE = "Enable Solar Nosecone";
			}
			public class ENABLEBUNKERPLATFORM
			{
				public static LocString TITLE = "Enable Fortified & Advanced Rocket Platform";
			}
			public class ENABLEWALLADAPTER
			{
				public static LocString TITLE = "Enable Rocket Port Adapters";
			}
			public class ENABLEROCKETLOADERLOGICOUTPUTS
			{
				public static LocString TITLE = "Loader Logic Output";
				public static LocString TOOLTIP = "Add a logic output to rocket loader buildings that outputs whether or not the loader is currently active.";
			}
			public class ENABLEFUELLOADERS
			{
				public static LocString TITLE = "Enable Fuel Loaders";
			}
			public class VERTICALPORT_LADDER
			{
				public static LocString TITLE = "Vertical Port Adapter Ladder";
				public static LocString TOOLTIP = "The vertical port adapter allows duplicants to climb it";
			}
			public class ENABLEEARLYGAMEFUELTANKS
			{
				public static LocString TITLE = "Enable early game Fuel Tanks";
			}
			public class ENABLEELECTRICENGINE
			{
				public static LocString TITLE = "Enable Ion Engine";
			}
			public class NATGASENGINERANGE
			{
				public static LocString TITLE = "Natural Gas Engine Range";
				public static LocString TOOLTIP = "Set the max range of a natural gas engine.";
			}
			public class ENABLENATGASENGINE
			{
				public static LocString TITLE = "Enable Natural Gas Engine";
			}
			public class ENABLEBOOSTERS
			{
				public static LocString TITLE = "Enable Booster Modules";
			}
			public class ETHANOLENGINES
			{
				public static LocString TITLE = "Burn Ethanol as fuel";
				public static LocString TOOLTIP = "Allows Petroleum Engines to also burn Ethanol as fuel.";
			}
			public class BUFFLARGEOXIDIZER
			{
				public static LocString TITLE = "Buff Large Oxidizer Module";
				public static LocString TOOLTIP = "Buff storage capacity of the large Oxidizer Module from 900kg to 1350kg.";
			}
			public class CARGOBAYUNITS
			{
				public static LocString SMALLCARGOBAYUNITS = "Small Cargo Bay Units";
				public static LocString MEDIUMCARGOBAYUNITS = "Large Cargo Bay Units";
				public static LocString COLLOSSALCARGOBAYUNITS = "Collossal Cargo Bay Units";

				public static LocString GASCARGOBAYKGPERUNIT = "Gas Cargobay KG/Unit";
				public static LocString LIQUIDCARGOBAYKGPERUNIT = "Liquid Cargobay KG/Unit";
				public static LocString SOLIDCARGOBAYKGPERUNIT = "Solid Cargobay KG/Unit";

				public static LocString TOOLTIP = "Amount of Cargo Units in this Cargo Bay Size";
				public static LocString UNITDESCRIPTION = "Only has an effect if the option \"Rebalanced Cargo Bays\" is enabled.\n" +
					"Cargo bay capacity is calculated by \"Unit-KG * Cargobay-Unit-Count\"\nUnit-Count per size:\n- Small: 9 Units\n- Large: 28 Units\n- Colossal: 64 Units.\n\nAdjust the Unit-KG for this cargo bay type below.";
			}
			public class REBALANCEDCARGOCAPACITY
			{
				public static LocString TITLE = "Rebalanced Cargobay Capacity";
				public static LocString TOOLTIP = "Cargo Bays have increased and rebalanced Cargo Capacity";
			}
			public class CRITTERSTORAGECAPACITY
			{
				public static LocString TITLE = "Critter Containment Module Capacity";
				public static LocString TOOLTIP = "Amount of critters the module can hold at once";
			}
			public class CARGOBAY_LOGICPORTS
			{
				public static LocString TITLE = "Cargobay logic ports";
				public static LocString TOOLTIP = "Cargobays get two logic output ports, one for \"is full\" and one for \"is empty\"";
			}
			public class ENABLECRITTERSTORAGE
			{
				public static LocString TITLE = "Enable Critter Containment Module";
			}
			public class ENABLEPOISENSOR
			{
				public static LocString TITLE = "Enable POI Capacity Sensor";
			}
			public class RADBOLTSTORAGECAPACITY
			{
				public static LocString TITLE = "Radbolt Storage Module Capacity";
				public static LocString TOOLTIP = "Set the total radbolt capacity of the Radbolt Chamber Module.";
			}
			public class ENABLERADBOLTSTORAGE
			{
				public static LocString TITLE = "Enable Radbolt Storage Module";
			}
			public class INSULATEDCARGOBAYS
			{
				public static LocString TITLE = "Insulated Cargo Bays";
				public static LocString TOOLTIP = "Contents of Cargo Bay modules are insulated from their surroundings";
			}
			public class ENABLELARGECARGOBAYS
			{
				public static LocString TITLE = "Enable Large Cargo Modules";
			}
			public class ENABLEFRIDGE
			{
				public static LocString TITLE = "Enable Fridge Module";
			}
			public class INFINITEPOI
			{
				public static LocString TITLE = "Infinite POI Mining Capacity";
				public static LocString TOOLTIP = "Capacity of mineable POIs becomes infinite. Does not affect artifacts.";
			}
			public class PILOTSKILLAFFECTSDRILLSPEED
			{
				public static LocString TITLE = "Pilot Skill affects Mining Speed";
				public static LocString TOOLTIP = "The pilots' piloting and digging skills affect the drilling speed of the drillcone.\nApplies to all drillcone types";
			}
			public class REFILLDRILLSUPPORT
			{
				public static LocString TITLE = "Refill Service Module via Conveyor";
				public static LocString TOOLTIP = "Refill the service module with diamonds during the flight via interior conveyor wall port.";
			}
			public class DRILLCONESUPPORTDIAMONDMASS
			{
				public static LocString TITLE = "Service Module Diamond Capacity";
				public static LocString TOOLTIP = "Determines the diamond capacity of the drillcone service module in KG";
			}
			public class DRILLCONESUPPORTBOOST
			{
				public static LocString TITLE = "Service Module Drilling Speed Buff";
				public static LocString TOOLTIP = "Determines the drilling speed boost the drillcone service module provides to diamond fueled drillcones in percent";
			}
			public class ENABLEDRILLSUPPORT
			{
				public static LocString TITLE = "Enable Drillcone Service Module";
			}
			public class LASERDRILLCONECAPACITY
			{
				public static LocString TITLE = "Laser Drillcone Capacity";
				public static LocString TOOLTIP = "The total amount of radbolts the Laser Drillcone can hold\nThe Laser Drillcone mines 4kg of material per radbolt";
			}
			public class LASERDRILLCONESPEED
			{
				public static LocString TITLE = "Laser Drillcone Speed";
				public static LocString TOOLTIP = "Mining speed in Kg/s for the Laser Drillcone. (For reference; the vanilla drillcone mines at 7.5kg/s).";
			}
			public class ENABLELASERDRILL
			{
				public static LocString TITLE = "Enable Laser Drillcone";
			}
			public class SLIMLARGEENGINES
			{
				public static LocString TITLE = "Slim Rockets";
				public static LocString TOOLTIP = "Rocket Modules that are wider than 5 tiles (Steam, Hydrogen, Petrol Engine) are reduced to 5 width.";
			}
			public class HABITATINTERIORPORTIMPROVEMENTS
			{
				public static LocString TITLE = "Habitat Interior Port Improvements";
				public static LocString TOOLTIP = "Rocket Connectors count as Rocket Wall for buildings that can only be attached to it.\nRocket Ports block the same amount of radiation as rocket wall";
			}
			public class HABITATINTERIORRADIATION
			{
				public static LocString TITLE = "Spacefarer Habitat Radiation";
				public static LocString TOOLTIP = "Rocket interior radiation is defined by the radiation outside when landed.";
			}
			public class HABITATPOWERPLUG
			{
				public static LocString TITLE = "Spacefarer Power Connector";
				public static LocString TOOLTIP = "Add a power connector to the habitat modules.";
			}
			public class SCANNERMODULESCANSPEED
			{
				public static LocString TITLE = "Cartographic Module Scan Speed";
				public static LocString TOOLTIP = "Time it takes for the module to reveal one hex in cycles.";
			}
			public class SCANNERMODULERANGERADIUS
			{
				public static LocString TITLE = "Cartographic Module Scan Range";
				public static LocString TOOLTIP = "Cartographic Modules will scan hexes in this radius.";
			}
			public class ENABLEEXTENDEDHABS
			{
				public static LocString TITLE = "Enable extra Spacefarer Modules";
			}
			public class COMPRESSINTERIORS
			{
				public static LocString TITLE = "Compress Interiors & Remove Rocket Limit";
				public static LocString TOOLTIP = "Removes the rocket limit of 16 and trims excessive space outside of the rocket walls in spacefarer interiors\nRequired for space stations.";
			}
			public class ENABLEADVWORLDSELECTOR
			{
				public static LocString TITLE = "Improved World Selector";
				public static LocString TOOLTIP = "Makes world selector planets collapsible and improves internal performance.\nRequired for space stations.";
			}
		}
		public class MODIFIEDVANILLASTRINGS
		{
			public static LocString KEROSENEENGINECLUSTERSMALL_EFFECT = ("Burns either " + FormatAsLink("Petroleum", "PETROLEUM") + " or " + FormatAsLink("Ethanol", "ETHANOL") + " to propel rockets for mid-range space exploration.\n\nSmall Petroleum Engines possess the same speed as a " + FormatAsLink("Petroleum Engines", "KEROSENEENGINE") + " but have smaller height restrictions.\n\nEngine must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
			public static LocString KEROSENEENGINECLUSTER_EFFECT = ("Burns either " + FormatAsLink("Petroleum", "PETROLEUM") + " or " + FormatAsLink("Ethanol", "ETHANOL") + " to propel rockets for mid-range space exploration.\n\nPetroleum Engines have generous height restrictions, ideal for hauling many modules.\n\nEngine must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + ". \n\nOnce the engine has been built, more rocket modules can be added.");
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
