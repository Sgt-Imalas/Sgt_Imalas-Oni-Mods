using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;

namespace Rockets_TinyYetBig
{
    public class ModAssets
    {
        public static string DeepSpaceScienceID = "rtb_deepspace";
        public class Techs
        {
            public static string FuelLoaderTechID = "RTB_FuelLoadersTech";
            public static Tech FuelLoaderTech;
            public static string DockingTechID = "RTB_DockingTech";
            public static Tech DockingTech;
            public static string LargerRocketLivingSpaceTechID = "RTB_LargerRocketLivingSpaceTech";
            public static Tech LargerRocketLivingSpaceTech;
            public static string SpaceStationTechID = "RTB_SpaceStationTech";
            public static Tech SpaceStationTech;
            public static string SpaceStationTechMediumID = "RTB_MediumSpaceStationTech";
            public static Tech SpaceStationTechMedium;
            public static string SpaceStationTechLargeID = "RTB_LargeSpaceStationTech";
            public static Tech SpaceStationTechLarge;
            public static string HugeCargoBayTechID = "RTB_HugeCargoBayTech";
            public static Tech HugeCargoBayTech;
        }
        public class Tags
        {
            public static Tag IsSpaceStation = TagManager.Create("RTB_isSpaceStationInteriorWorld");
            public static Tag SpaceStationOnlyInteriorBuilding = TagManager.Create("RTB_SpaceStationInteriorOnly");
            public static Tag RocketInteriorOnlyBuilding = TagManager.Create("RTB_RocketInteriorOnly");
            public static Tag RocketPlatformTag = TagManager.Create("RTB_RocketPlatformTag");
            public static Tag RadiationShielding = TagManager.Create("RadiationShieldingMaterial");
            public static Tag NeutroniumAlloy = TagManager.Create("RTB_NeutroniumAlloyMaterial");
        }

        public enum SpaceStationType
        {
            small = 0,
            medium = 1,
            large = 2,
            jumpBeacon = 3,
            jumpGate = 4,

        }


        public static Dictionary<int, SpaceStationWithStats> SpaceStationTypes = new Dictionary<int, SpaceStationWithStats>()
        {
            {
                (int)SpaceStationType.small,

                new SpaceStationWithStats(
                "RTB_SpaceStationSmall",
                "Small Space Station",
                "a tiny space station",
                new Vector2I (30,30),
                new Dictionary<string,float> { [SimHashes.Steel.CreateTag().ToString()]= 500f },
                "space_station_small_kanim",
                20f,//150f,
                Techs.SpaceStationTechID
                )
            },
            {
                (int)SpaceStationType.medium,

                new SpaceStationWithStats(
                    "RTB_SpaceStationMedium",
                    "Medium Space Station",
                    "a medium sized space station",
                    new Vector2I (45,45),
                    new Dictionary<string,float> { [SimHashes.Steel.CreateTag().ToString()]= 750f,
                                                   [SimHashes.Niobium.CreateTag().ToString()]= 500f },
                    "space_station_medium_kanim",
                    20f,//300f
                    Techs.SpaceStationTechMediumID
                )
            },
            {
                (int)SpaceStationType.large,

            new SpaceStationWithStats(
                "RTB_SpaceStationLarge",
                "Large Space Station",
                "a large space station",
                new Vector2I (60,60),
                new Dictionary<string,float> { [SimHashes.Steel.CreateTag().ToString()]= 1000f,
                                               [SimHashes.TempConductorSolid.CreateTag().ToString()]= 500f,
                                               [SimHashes.Isoresin.CreateTag().ToString()]= 300f ,
                                               [SimHashes.Graphite.CreateTag().ToString()]= 200f },
                "space_station_large_kanim",
                20f,//600f
                Techs.SpaceStationTechLargeID
                )
            }
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
            public static StatusItem RTB_RocketBatteryStatus;
            public static StatusItem RTB_AlwaysActiveOn;
            public static StatusItem RTB_AlwaysActiveOff;
            public static StatusItem RTB_CritterModuleContent;

            public static StatusItem RTB_SpaceStationConstruction_Status;

            public static StatusItem RTB_SpaceStation_DeploymentState;
            public static StatusItem RTB_SpaceStation_FreshlyDeployed;
            public static StatusItem RTB_SpaceStation_OrbitHealth;



            public static void Register()
            {
                RTB_RocketBatteryStatus = new StatusItem(
                      "RTB_ROCKETBATTERYSTATUS",
                      "BUILDING",
                      string.Empty,
                      StatusItem.IconType.Info,
                      NotificationType.Neutral,
                      false,
                      OverlayModes.Power.ID
                      );

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

                RTB_CritterModuleContent = new StatusItem(
                     "RTB_CRITTERMODULECONTENT",
                     "BUILDING",
                     string.Empty,
                     StatusItem.IconType.Info,
                     NotificationType.Neutral,
                false,
                     OverlayModes.None.ID);

                RTB_SpaceStationConstruction_Status = new StatusItem(
                     "RTB_STATIONCONSTRUCTORSTATUS",
                     "BUILDING",
                     string.Empty,
                     StatusItem.IconType.Info,
                     NotificationType.Neutral,
                false,
                     OverlayModes.None.ID);

                RTB_SpaceStationConstruction_Status.resolveStringCallback = ((str, data) =>
                {
                    var StationConstructior = (SpaceStationBuilder)data;
                    float remainingTime = StationConstructior.RemainingTime();
                    if (remainingTime > 0)
                    {
                        str = str.Replace("{TOOLTIP}", RTB_STATIONCONSTRUCTORSTATUS.TIMEREMAINING);
                        str = str.Replace("{TIME}", GameUtil.GetFormattedTime(remainingTime));
                    }
                    else
                    {
                        str = str.Replace("{TOOLTIP}", RTB_STATIONCONSTRUCTORSTATUS.NONEQUEUED);
                    }

                    if (StationConstructior.Constructing())
                    {
                        str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.CONSTRUCTING);
                        str = str.Replace("{TIME}", GameUtil.GetFormattedTime(remainingTime));

                    }
                    else if (StationConstructior.Demolishing())
                    {
                        str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.DECONSTRUCTING);
                        str = str.Replace("{TIME}", GameUtil.GetFormattedTime(remainingTime));
                    }
                    else
                    {
                        str = str.Replace("{STATUS}", RTB_STATIONCONSTRUCTORSTATUS.IDLE);
                    }
                    return str;
                });


                RTB_CritterModuleContent.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    var CritterStorage = (CritterStasisChamberModule)data;
                    //var stats = generator.GetConsumptionStatusStats();
                    //str = str.Replace("{GeneratorType}", generator.GetProperName());

                    string newValue1 = Util.FormatWholeNumber(CritterStorage.CurrentCapacity);
                    string newValue2 = Util.FormatWholeNumber(CritterStorage.CurrentMaxCapacity);
                    string CritterData = CritterStorage.GetStatusItem();

                    str = str.Replace("{0}", newValue1);
                    str = str.Replace("{1}", newValue2);
                    str = str.Replace("{CritterContentStatus}", CritterData);
                    return str;
                });

                RTB_RocketBatteryStatus.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    var stats = (Tuple<float, float>)data;
                    //var stats = generator.GetConsumptionStatusStats();
                    //str = str.Replace("{GeneratorType}", generator.GetProperName());
                    str = str.Replace("{CurrentCharge}", GameUtil.GetFormattedJoules(stats.first));
                    str = str.Replace("{MaxCharge}", GameUtil.GetFormattedJoules(stats.second));
                    return str;
                });

                RTB_ModuleGeneratorFuelStatus.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                {
                    var stats = (Tuple<float, float>)data;
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

                SgtLogger.debuglog("[Rocketry Expanden] Status items initialized");

            }
        }

        public struct SpaceStationWithStats
        {
            public string ID;
            public string Name;
            public string Description;
            public Vector2I InteriorSize;
            public Dictionary<string, float> materials;
            public float constructionTime;
            public float demolishingTime;
            public string Kanim;
            public string requiredTechID;
            public bool HasInterior;
            public SpaceStationWithStats(string _id, string _name, string _description, Vector2I _size, Dictionary<string,float> _mats, string _kanim, float _constructionTime, string _reqTech = "", bool _hasInterior = true)
            {
                ID = _id;
                Name = _name;
                Description = _description;
                InteriorSize = _size;
                materials = _mats;
                Kanim = _kanim;
                requiredTechID = _reqTech == ""? Techs.SpaceStationTechID:_reqTech;
                constructionTime = _constructionTime;
                demolishingTime = _constructionTime / 4;
                HasInterior = _hasInterior;
            }

        }
    }
}
