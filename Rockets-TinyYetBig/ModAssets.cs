using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig
{
    public class ModAssets
    {
        public class Techs
        {
            public static string FuelLoaderTech = "RTB_FuelLoadersTech";
            public static string DockingTech = "RTB_DockingTech";
            public static string SpaceStationTech = "RTB_SpaceStationTech";
        }
        public class Tags
        {
            public static Tag IsSpaceStation = TagManager.Create("RTB_isSpaceStationInteriorWorld");
            public static Tag SpaceStationOnlyInteriorBuilding = TagManager.Create("RTB_SpaceStationInteriorOnly");
            public static Tag RocketInteriorOnlyBuilding = TagManager.Create("RTB_RocketInteriorOnly");
            public static Tag RocketPlatformTag = TagManager.Create("RTB_RocketPlatformTag");
        }

        public static List<SpaceStationWithStats> SpaceStationTypes = new List<SpaceStationWithStats>()
        {
            new SpaceStationWithStats(
                "RTB_SpaceStationSmall",
                "Small Space Station",
                "a tiny space station",
                new Vector2I (30,30),
                new string[] {"RefinedMetal" },
                new float[] { 300f }),

            new SpaceStationWithStats(
                "RTB_SpaceStationMedium",
                "Medium Space Station",
                "a medium sized space station",
                new Vector2I (45,45),
                new string[] {"RefinedMetal" },
                new float[] { 300f }),

            new SpaceStationWithStats(
                "RTB_SpaceStationLarge",
                "Large Space Station",
                "a large space station",
                new Vector2I (60,60),
                new string[] {"RefinedMetal" },
                new float[] { 300f })

    };



        public static Components.Cmps<DockingManager> Dockables = new Components.Cmps<DockingManager>();
        
        public static Dictionary<Tuple<BuildingDef, int>, GameObject> CategorizedButtons = new Dictionary<Tuple<BuildingDef, int>, GameObject>();

        public static readonly CellOffset PLUG_OFFSET_SMALL = new CellOffset(-1, 0);
        public static readonly CellOffset PLUG_OFFSET_MEDIUM = new CellOffset(-2, 0);

        public static int InnerLimit = 0;
        public static int Rings = 0;
        public class StatusItems
        {
            public static StatusItem RTB_ModuleGeneratorNotPowered;
            public static StatusItem RTB_ModuleGeneratorPowered;
            public static StatusItem RTB_ModuleGeneratorFuelStatus;
            public static StatusItem RTB_AlwaysActiveOn;
            public static StatusItem RTB_AlwaysActiveOff;

            public static void Register()
            {
                RTB_ModuleGeneratorNotPowered = new StatusItem(
                      "RTB_MODULEGENERATORNOTPOWERED",
                      "BUILDING",
                      string.Empty,
                      StatusItem.IconType.Info,
                      NotificationType.Neutral,
                      false,
                      OverlayModes.Power.ID
                      );
                RTB_ModuleGeneratorPowered = new StatusItem(
                   "RTB_MODULEGENERATORPOWERED",
                   "BUILDING",
                   string.Empty,
                   StatusItem.IconType.Info,
                   NotificationType.Neutral,
                   false,
                   OverlayModes.Power.ID);
                RTB_AlwaysActiveOn = new StatusItem(
                    "RTB_MODULEGENERATORALWAYSACTIVEPOWERED",
                    "BUILDING",
                    string.Empty,
                    StatusItem.IconType.Info,
                    NotificationType.Neutral,
                    false,
                    OverlayModes.Power.ID); 
                RTB_AlwaysActiveOff = new StatusItem(
                     "RTB_MODULEGENERATORALWAYSACTIVENOTPOWERED",
                     "BUILDING",
                     string.Empty,
                     StatusItem.IconType.Info,
                     NotificationType.Neutral,
                     false,
                     OverlayModes.Power.ID);
                RTB_ModuleGeneratorFuelStatus = new StatusItem(
                     "RTB_MODULEGENERATORFUELSTATUS",
                     "BUILDING",
                     string.Empty,
                     StatusItem.IconType.Info,
                     NotificationType.Neutral,
                     false,
                     OverlayModes.Power.ID);


                RTB_ModuleGeneratorFuelStatus.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    var stats = (Tuple<float,float>)data;
                    //var stats = generator.GetConsumptionStatusStats();
                    //str = str.Replace("{GeneratorType}", generator.GetProperName());
                    str = str.Replace("{CurrentFuelStorage}", GameUtil.GetFormattedMass(stats.first));
                    str = str.Replace("{MaxFuelStorage}", GameUtil.GetFormattedMass(stats.second));
                    return str;
                });

                RTB_ModuleGeneratorNotPowered.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    Generator generator = (RTB_ModuleGenerator)data;
                    str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(0.0f));
                    str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
                    return str;
                });
                RTB_ModuleGeneratorPowered.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    Generator generator = (RTB_ModuleGenerator)data;
                    str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
                    str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
                    return str;
                });
                RTB_AlwaysActiveOff.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    Generator generator = (RTB_ModuleGenerator)data;
                    str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(0.0f));
                    str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
                    return str;
                });
                RTB_AlwaysActiveOn.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    Generator generator = (RTB_ModuleGenerator)data; 
                    str = str.Replace("{ActiveWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
                    str = str.Replace("{MaxWattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
                    return str;
                });

                Debug.Log("Status items initialized");

            }
        }

        public struct SpaceStationWithStats
        {
            public string ID;
            public string Name; 
            public string Description;
            public Vector2I InteriorSize;
            public string[] materials;
            public float[] materialAmounts;
            public SpaceStationWithStats(string _id, string _name, string _description, Vector2I _size, string[] _mats, float[] _matCosts)
            {
                ID = _id;
                Name = _name;
                Description = _description;
                InteriorSize = _size;
                materials = _mats;
                materialAmounts = _matCosts;
            }
            
        }
    }
}
