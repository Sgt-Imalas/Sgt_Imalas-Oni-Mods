using LogicSatellites.Buildings;
using LogicSatellites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites
{
    public class STRINGS
    {

       
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class LS_SATELLITECARRIERMODULE
                {
                    public static LocString NAME = "Satellite Carrier Module";
                    public static LocString DESC = "Stores and delivers Satellites to orbit.";
                    public static LocString EFFECT = "The Satellite Carrier Module is used to deploy and retrieve satellites on the star map\n\nA satellite can be constructed from satellite parts, made at the "+ global::STRINGS.UI.FormatAsLink(global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.NAME, CraftingTableConfig.ID);
                }
            }
        }
        public class ITEMS
        {
            public class SATELLITE
            {
                //public static LocString TITLE = "Deployable ";
                public static LocString TITLE = "Satellite";
                public static LocString DESC = "Deploy this satellite on the star map to create a logic relay "; 
                
                public class SATELLITETYPES
                {
                    public static LocString EXPLORATION = "Exploration ";
                    public static LocString EXPLORATIONDESC = "and slowly reveal the surrounding hexes";
                    public static LocString SOLAR = "Solar Lens ";
                    public static LocString SOLARDESC = "and concentrate light towards the planet below.";
                    //public static LocString DYSONSPHERE = "Solar Collector Satellite";
                    //public static LocString DYSONSPHEREDESC = "Solar Collector Satellite";
                }
            }
            public class LS_CLUSTERSATELLITEPART
            {

                public static LocString TITLE = "Satellite Parts";
                public static LocString DESC = "These parts are used to build a satellite in the " + global::STRINGS.UI.FormatAsLink(STRINGS.BUILDINGS.PREFABS.LS_SATELLITECARRIERMODULE.NAME, SatelliteCarrierModuleConfig.ID)+".";
            }
            public class LS_SATELLITEGRID
            {
                public static LocString TITLE = "Satellite";
                public static LocString DESC = "This Satellite monitors the space around its location and amplifies interplanetary logic signals."+
                    string.Format("\n\n\nThis Satellite will gradually reveal the space around it up to a radius of {0}.\n\nIt will also amplify a logic signal by redirecting it up to {1} Tiles further", Config.Instance.SatelliteScannerRange, Config.Instance.SatelliteLogicRange);
            }
        }
        public class UI
        {
            public class UISIDESCREENS
            {
                public class SATELLITECARRIER_SIDESCREEN
                {
                    public static LocString TITLE = "Satellite Carrier";

                    public static LocString TITLELABEL_HASSAT_TRUE = "{0} stored.";
                    public static LocString TITLELABEL_HASSAT_FALSE = "No Satellite in storage";
                    public static LocString BUTTONLABEL_HASSAT_TRUE = "Deploy Satellite";
                    public static LocString BUTTONLABEL_HASSAT_FALSE = "Retrieve Satellite";
                    public static LocString BUTTONTOOLTIP_DEPLOY = "Deploys a satellite at the current space hex.";
                    public static LocString BUTTONTOOLTIP_RETRIEVE = "Retrieves a satellite from the current space hex";
                    public static LocString BUTTONTOOLTIP_CHANGEMODE = "Change the operation the module should perform";
                }
            }
        }
    }
}
