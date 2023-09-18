using Klei;
using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace OniRetroEdition.BuildingDefModification
{
    internal class BuildingModifications
    {
        public static string BuildingConfigPath;

        public static BuildingModifications Instance;

        public Dictionary<string, BuildingModification> LoadedBuildingOverrides = new Dictionary<string, BuildingModification>();
        public static void LoadBuildingOverrides()
        {

        }
        public static void InitializeFolderPath()
        {

            SgtLogger.debuglog("Initializing file path..");
            BuildingConfigPath = FileSystem.Normalize(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BuildingOverrides.json"));

            if (!new FileInfo(BuildingConfigPath).Exists)
            {
                BuildingModifications tmp = new BuildingModifications();
                tmp.LoadedBuildingOverrides = new Dictionary<string, BuildingModification>()
                {
                    {
                        GasBottlerConfig.ID,
                        new BuildingModification()
                        {
                            HeightOverride = 3,
                            WidthOverride = 2,
                            UtilityInputOffsetOverride = new CellOffset(1, 2)
                        }
                    }
                    ,{
                        BottleEmptierConfig.ID,
                        new BuildingModification()
                        {
                            HeightOverride = 3,
                            WidthOverride = 2,
                        }
                    },{
                        OuthouseConfig.ID,
                        new BuildingModification()
                        {
                            HeightOverride = 2,
                            WidthOverride = 2,
                        }
                    }
                    ,{
                        HydrogenGeneratorConfig.ID,
                        new BuildingModification()
                        {
                            HeightOverride = 2,
                            WidthOverride = 4,
                        }
                    }
                    ,{
                        TravelTubeEntranceConfig.ID,
                        new BuildingModification()
                        {
                            HeightOverride = 2,
                            WidthOverride = 2,
                        }
                    }
                    ,{
                        WaterPurifierConfig.ID,
                        new BuildingModification()
                        {
                            HeightOverride = 2,
                            WidthOverride = 3,
                            PowerInputOffsetOverride = new CellOffset(1, 0),
                            UtilityOutputOffsetOverride = new CellOffset(1, 1),
                            UtilityInputOffsetOverride = new CellOffset(-1, 1),
                        }
                    }
                    ,{
                        MouldingTileConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Base,
                            placedBehindBuildingId = CarpetTileConfig.ID,
                            foundationFloorTile = true
                        }
                    },{
                        CrewCapsuleConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Rocketry,
                            placedBehindBuildingId = ""
                        }
                    },{
                        AtmoicGardenConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Food,
                            placedBehindBuildingId = FarmTileConfig.ID
                        }
                    },{
                        FlyingCreatureBaitConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Food,
                            placedBehindBuildingId = EggCrackerConfig.ID
                        }
                    },{
                        AirborneCreatureLureConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Food,
                            placedBehindBuildingId = EggCrackerConfig.ID
                        }
                    },{
                        FishTrapConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Food,
                            placedBehindBuildingId = EggCrackerConfig.ID
                        }
                    },{
                        CreatureTrapConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Food,
                            placedBehindBuildingId = EggCrackerConfig.ID
                        }
                    },{
                        GenericFabricatorConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Refinement,
                            placedBehindBuildingId = RockCrusherConfig.ID
                        }
                    },{
                        MachineShopConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Stations,
                            placedBehindBuildingId = PowerControlStationConfig.ID
                        }
                    },{
                        AdvancedApothecaryConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Medicine,
                            placedBehindBuildingId = ApothecaryConfig.ID
                        }
                    },{
                        OxygenMaskStationConfig.ID,
                        new BuildingModification()
                        {
                            buildMenuCategory = GameStrings.PlanMenuCategory.Stations,
                            placedBehindBuildingId = OxygenMaskMarkerConfig.ID,
                            placeBefore = true

                        }
                    },{
                        PressureSwitchGasConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        GasConduitOverflowConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        GasConduitPreferentialFlowConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        LiquidCooledFanConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        PressureSwitchLiquidConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        LiquidConduitOverflowConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        LiquidConduitPreferentialFlowConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        AstronautTrainingCenterConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        SteamTurbineConfig.ID,
                        new BuildingModification()
                        {
                        }
                    },{
                        TemperatureControlledSwitchConfig.ID,
                        new BuildingModification()
                        {
                        }
                    }
                };
                IO_Util.WriteToFile<BuildingModifications>(tmp, BuildingConfigPath);
            }

            try
            {
                IO_Util.ReadFromFile<BuildingModifications>(BuildingConfigPath, out Instance);
            }
            catch (Exception e)
            {
                SgtLogger.error("Could not create folder, Exception:\n" + e);
            }
            SgtLogger.log("Folders succesfully initialized");
        }
    }
    internal class BuildingModification
    {
        public int? WidthOverride, HeightOverride;
        public bool? foundationFloorTile;
        public string animOverride;
        public string buildMenuCategory, placedBehindBuildingId;

        public CellOffset? UtilityInputOffsetOverride, UtilityOutputOffsetOverride, PowerInputOffsetOverride;
        public bool? placeBefore;

        public bool? requiresMinionWorker;
        public string workableAnimOverride;


    }
}
