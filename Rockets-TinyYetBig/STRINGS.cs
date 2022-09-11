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

            public static LocString ENGINES = "Every rocket has to fly somehow.\nOne of these can provide the thrust."+ REQUIRED;
            public static LocString HABITATS = "Strapped to the side a pilot won't survive long.\nBuild them a nice home to live in one of these."+ REQUIRED;
            public static LocString NOSECONES = "When not using a habitat nosecone,\nthe rocket needs one of these to keep it's tip nicely shaped.";
            public static LocString DEPLOYABLES = "Colonizing new worlds needs some perimeter establishment.\nThese modules help with getting the ressources down there.";
            public static LocString FUEL = "A rocket without fuel or oxidizer won't fly far.\nThese modules help you with that.";
            public static LocString POWER = "Without power, the lights inside of the rocket will go out\nThese modules help you store it and some even generate it.";
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
                    public static LocString EFFECT = (string.Format("After adding {0} kg of enriched Uranium, this module will constantly produce {1} W of energy until all of the uranium is depleted", RTGModuleConfig.UraniumCapacity, RTGModuleConfig.energyProduction));
                }

                public class RTB_HABITATMODULESTARGAZER
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Stargazer Nosecone", nameof(HabitatModuleStargazerConfig));
                    public static LocString DESC = "The stars have never felt this close before like in this Command Module.";
                    public static LocString EFFECT = ("Closes during starts and landings to protect the glass\n\n"+
                                                        "Functions as a Command Module and a Nosecone.\n\n" +
                                                        "One Command Module may be installed per rocket.\n\n" +
                                                    "Must be built via " + (string)global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.NAME + 
                                            ". \n\nMust be built at the top of a rocket.");
                }
                public class RTB_CRITTERCONTAINMENTMODULE
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Critter Containment Module", nameof(CritterContainmentModuleConfig));
                    public static LocString EFFECT = "This module allows the safe transport of critters to their new home. ";
                    public static LocString DESC = "These critters will go where no critter has gone before.";
                }
                public class RYB_NOSECONEHEPHARVEST
                {
                    public static LocString NAME = (LocString)UI.FormatAsLink("Laser Drillcone", nameof(NoseConeHEPHarvestConfig));
                    public static LocString DESC = (LocString)"Harvests resources from the universe with the power of radbolts and lasers";
                    public static LocString EFFECT = global::STRINGS.BUILDINGS.PREFABS.NOSECONEHARVEST.EFFECT;
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
            }
        }
    }
}
