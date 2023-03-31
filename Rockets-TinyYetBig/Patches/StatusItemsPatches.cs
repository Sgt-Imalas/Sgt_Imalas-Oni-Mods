using Database;
using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using YamlDotNet.Core.Tokens;
using static Rockets_TinyYetBig.STRINGS;
using static Rockets_TinyYetBig.STRINGS.UI_MOD.CLUSTERMAPROCKETSIDESCREEN;

namespace Rockets_TinyYetBig.Patches
{
    class StatusItemsPatches
    {
        class StatusItemsRegisterPatch
        {
            [HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
            public static class Database_BuildingStatusItems_CreateStatusItems_Patch
            {
                public static void Postfix()
                {
                    ModAssets.StatusItems.Register();
                }
            }
        }



        [HarmonyPatch(typeof(Clustercraft))]
        [HarmonyPatch(nameof(Clustercraft.UpdateStatusItem))]
        public static class GeneratorModuleStatusItems
        {
            static Dictionary<KSelectable, Guid> batteryStatusItemGuids = new Dictionary<KSelectable, Guid>();
            static Dictionary<KSelectable, Guid> generatorStatusItemGuids = new Dictionary<KSelectable, Guid>();
            public static void Postfix(Clustercraft __instance)
            {
                KSelectable selectable = __instance.GetComponent<KSelectable>();

                //selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_RocketBatteryStatus);
                //selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);
                //selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_SpaceStationConstruction_Status);

                //Tuple<float, float> data = new Tuple<float, float>(0, 0);
                //Tuple<float, float> dataBattery = new Tuple<float, float>(0, 0);
                SpaceStationBuilder constructionModule = null;

                foreach (var module in __instance.ModuleInterface.ClusterModules)
                {
                    //if (moduleGet.gameObject.TryGetComponent<RTB_ModuleGenerator>(out var generator))
                    //{
                    //    var genStats = generator.GetConsumptionStatusStats();
                    //    data.first += genStats.first;
                    //    data.second += genStats.second;
                    //    //generator.FuelStatusHandle =
                    //}
                    //if (moduleGet.gameObject.TryGetComponent<ModuleBattery>(out var battery))
                    //{
                    //    dataBattery.first += battery.JoulesAvailable;
                    //    dataBattery.second += battery.capacity;
                    //    //generator.FuelStatusHandle =
                    //}
                    if (module.Get().gameObject.TryGetComponent<SpaceStationBuilder>(out var builder))
                    {
                        constructionModule = builder;
                    }
                }

                //if (data.first > 0 || data.second > 0)
                //    selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus, (object)data);
                //else
                //    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_ModuleGeneratorFuelStatus);


                //if (dataBattery.first > 0 || dataBattery.second > 0)
                //    selectable.SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, ModAssets.StatusItems.RTB_RocketBatteryStatus, (object)dataBattery);
                //else
                //    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_RocketBatteryStatus);

                if (constructionModule != null)
                {
                    selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, ModAssets.StatusItems.RTB_SpaceStationConstruction_Status, (object)constructionModule);
                }
                else
                    selectable.RemoveStatusItem(ModAssets.StatusItems.RTB_SpaceStationConstruction_Status);

            }

        }


        [HarmonyPatch(typeof(RocketSimpleInfoPanel))]
        [HarmonyPatch(nameof(RocketSimpleInfoPanel.Refresh))]
        public static class NoStatusItemsForSpaceStation2
        {
            static Clustercraft clustercraftPREVIOUS = null;
            static GameObject TargetPREVIOUS = null;
            const int perSecond = 50;
            static int counter = 0;
            static RocketEngineCluster targetEngine = null;
            static float FuelPerHexEngine = 0;
            static float FuelPerHexEnginePREVIOUS = -1f;
            static string FuelPerHexEngineSTRING = string.Empty;

            static float FuelRemaining = 0;
            static float FuelRemainingPREVIOUS = -1f;
            static string FuelRemainingSTRING = string.Empty;

            static Tag FuelTag = null;
            static bool RequiresOxidizer = false;

            static float OxidizerRemaining = 0;
            static float OxidizerRemainingPREVIOUS = -1f;
            static string OxidizerRemainingSTRING = string.Empty;

            static string RangeRemainingToolTipSTRING = string.Empty;
            static string RangeRemainingTextSTRING = string.Empty;


            static float RocketBurden = 0f;
            static float RocketBurdenPREVIOUS = -1f;
            static float RocketEnginePower = 0f;
            static float RocketEnginePowerPREVIOUS = -1f;

            static float Speed = 0f;
            static float SpeedPREVIOUS = -1f;

            static string SpeedToolTipSTRING = string.Empty;
            static string SpeedTextSTRING = string.Empty;

            static int RocketHeight = 0;
            static int RocketHeightPREVIOUS = -1;
            static int RocketWidth = 0;
            static int RocketWidthPREVIOUS = -1;

            static string RocketDimensionsSTRING = string.Empty;
            static string RocketDimensionsTooltipSTRING = string.Empty;


            static float PowerGeneration = 0f;
            static float PowerGenerationPREVIOUS = -1f;
            static float PowerGenerationMax = 0f;
            static float PowerGenerationMaxPREVIOUS = -1f;
            static float PowerStorage = 0f;
            static float PowerStoragePREVIOUS = -1f;
            static float PowerStorageMax = 0f;
            static float PowerStorageMaxPREVIOUS = -1f;

            static string PowerGenerationSTRING = string.Empty;
            static string PowerGenerationTOOLTIP = string.Empty;
            static string PowerStorageSTRING = string.Empty;
            static string PowerStorageTOOLTIP = string.Empty;

            static string ModuleOrderSTRING = string.Empty;
            static string ModuleOrderSTRINGPREVIOUS = string.Empty;
            static string ModuleOrderTOOLTIP = string.Empty;
            static string ModuleOrderTOOLTIPPREVIOUS = string.Empty;
            static int moduleCounter = 0;

            public struct CargoBayAndFriends
            {
                public CargoBayCluster CargoBay;
                public CritterStasisChamberModule CritterModule;
                public RadiationBatteryOutputHandler HepBatteryModule;
                public CargoBayAndFriends(ref CargoBayCluster cluster)
                {
                    CargoBay = cluster; CritterModule = null; HepBatteryModule = null;
                }
                public CargoBayAndFriends(ref CritterStasisChamberModule cluster)
                {
                    CargoBay = null; CritterModule = cluster; HepBatteryModule = null;
                }
                public CargoBayAndFriends(ref RadiationBatteryOutputHandler cluster)
                {
                    CargoBay = null; CritterModule = null; HepBatteryModule = cluster;
                }
            }

            /// <summary>
            /// Replaces StatusiteminfoPanel with a much more efficient version
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="rocketStatusContainer"></param>
            /// <param name="selectedTarget"></param>
            /// <returns></returns>
            public static bool Prefix(RocketSimpleInfoPanel __instance, CollapsibleDetailContentPanel rocketStatusContainer, GameObject selectedTarget)
            {
                if(TargetPREVIOUS == null || selectedTarget == null)
                {
                    TargetPREVIOUS = selectedTarget;
                    return false;
                }
                if (TargetPREVIOUS == selectedTarget && counter > 0)
                {
                    counter--;
                    return false;
                }
                counter += perSecond;
                if (TargetPREVIOUS != selectedTarget)
                {
                    foreach (KeyValuePair<string, GameObject> cargoBayLabel in __instance.cargoBayLabels)
                    {
                        //  UnityEngine.Object.Destroy(cargoBayLabel.Value);
                        cargoBayLabel.Value.SetActive(false);
                    }
                    __instance.cargoBayLabels.Clear();

                    foreach (KeyValuePair<string, GameObject> artifactLabel in __instance.artifactModuleLabels)
                    {
                        //UnityEngine.Object.Destroy(artifactLabel.Value);
                        artifactLabel.Value.SetActive(false);
                    }
                    __instance.artifactModuleLabels.Clear();
                    TargetPREVIOUS = selectedTarget;
                }

                if (selectedTarget.TryGetComponent<SpaceStation>(out var testS))
                {
                    rocketStatusContainer.gameObject.SetActive(false);
                    rocketStatusContainer.Commit();
                    return false;
                }

                bool FuelSorted = false;

                RocketBurden = 0;
                RocketEnginePower = 0;
                RocketHeight = 0;
                RocketWidth = 0;
                Speed = 0;
                FuelPerHexEngine = float.PositiveInfinity;
                OxidizerRemaining = 0;
                FuelRemaining = 0;
                FuelTag = null;
                RequiresOxidizer = false;
                targetEngine = null;
                ListPool<CargoBayAndFriends, SimpleInfoScreen>.PooledList CargoBays = ListPool<CargoBayAndFriends, SimpleInfoScreen>.Allocate();
                ListPool<ArtifactModule, SimpleInfoScreen>.PooledList ArtifactModules = ListPool<ArtifactModule, SimpleInfoScreen>.Allocate();
                PowerGeneration = 0;
                PowerGenerationMax = 0;
                PowerStorage = 0;
                PowerStorageMax = 0;
                PowerStorageTOOLTIP = string.Empty;
                PowerGenerationTOOLTIP = string.Empty;
                ModuleOrderTOOLTIP = string.Empty;
                moduleCounter = 0;

                //SgtLogger.debuglog(System.DateTime.Now);
                CraftModuleInterface craftModuleInterface = null;
                if (selectedTarget.TryGetComponent(out Clustercraft clusterCraft))
                {
                    craftModuleInterface = clusterCraft.ModuleInterface;
                }

                if (selectedTarget.TryGetComponent<RocketModuleCluster>(out var rocketModuleCluster))
                {
                    craftModuleInterface = rocketModuleCluster.CraftInterface;
                    if (clusterCraft == null)
                    {
                        craftModuleInterface.TryGetComponent(out clusterCraft);
                    }
                }


                if (clusterCraft == null)
                {
                    rocketStatusContainer.gameObject.SetActive(false);
                    rocketStatusContainer.Commit();
                    return false;
                }
                else
                {
                    rocketStatusContainer.gameObject.SetActive(true);
                }

                if (craftModuleInterface != null)
                {
                    int NumberOfModules = craftModuleInterface.clusterModules.Count;
                    foreach (var module in craftModuleInterface.clusterModules)
                    {
                        var moduleGet = module.Get();
                        if (moduleGet.TryGetComponent<RocketEngineCluster>(out var engine))
                        {
                            targetEngine = engine;
                            FuelPerHexEngine = moduleGet.performanceStats.fuelKilogramPerDistance * 600f;
                            FuelTag = targetEngine.fuelTag;
                            RequiresOxidizer = targetEngine.requireOxidizer;
                            RocketEnginePower = moduleGet.performanceStats.EnginePower;

                            if (moduleGet.TryGetComponent<HEPFuelTank>(out var fueltank))
                            {
                                FuelSorted = true;
                                FuelRemaining = Mathf.CeilToInt(fueltank.hepStorage.Particles);
                            }
                        }
                        RocketBurden += moduleGet.performanceStats.Burden;
                        if (moduleGet.TryGetComponent<Building>(out var building))
                        {
                            RocketHeight += building.Def.HeightInCells;
                            RocketWidth = building.Def.WidthInCells > RocketWidth ? building.Def.WidthInCells : RocketWidth;

                            ModuleOrderTOOLTIP = string.Concat((NumberOfModules - moduleCounter).ToString(), ") ", building.GetProperName(), ": ", building.Def.WidthInCells, "x", building.Def.HeightInCells, "\n", ModuleOrderTOOLTIP);

                            ++moduleCounter;
                        }
                        if (moduleGet.TryGetComponent<CargoBayCluster>(out var cargoBay))
                        {
                            CargoBays.Add(new CargoBayAndFriends(ref cargoBay));
                        }
                        else if (moduleGet.TryGetComponent<CritterStasisChamberModule>(out var stasisChamberModule))
                        {
                            CargoBays.Add(new CargoBayAndFriends(ref stasisChamberModule));
                        }
                        else if (moduleGet.TryGetComponent<RadiationBatteryOutputHandler>(out var hepChamberModule))
                        {
                            CargoBays.Add(new CargoBayAndFriends(ref hepChamberModule));
                        }
                        if (moduleGet.TryGetComponent<ArtifactModule>(out var artifactModule))
                        {
                            ArtifactModules.Add(artifactModule);
                        }
                        if (moduleGet.TryGetComponent<ModuleBattery>(out var batteryModule))
                        {
                            PowerStorageMax += batteryModule.capacity;
                            PowerStorage += batteryModule.joulesAvailable;
                            PowerStorageTOOLTIP = string.Concat("• ", batteryModule.GetProperName(), ": ", GameUtil.GetFormattedJoules(batteryModule.joulesAvailable), "/", GameUtil.GetFormattedJoules(batteryModule.capacity), "\n", PowerStorageTOOLTIP);
                        }
                        if (moduleGet.TryGetComponent<ModuleGenerator>(out var generatorModule))
                        {
                            PowerGeneration += generatorModule.IsProducingPower() ? generatorModule.WattageRating : 0;
                            PowerGenerationMax += generatorModule.WattageRating;
                            PowerGenerationTOOLTIP = string.Concat("• ", generatorModule.GetProperName(), ": ", GameUtil.GetFormattedWattage(generatorModule.IsProducingPower() ? generatorModule.WattageRating : 0), "/", GameUtil.GetFormattedWattage(generatorModule.WattageRating), "\n", PowerGenerationTOOLTIP);
                        }
                        else if (moduleGet.TryGetComponent<ModuleSolarPanel>(out var solarPanel))
                        {
                            PowerGeneration += solarPanel.IsProducingPower() ? solarPanel.CurrentWattage : 0;
                            PowerGenerationMax += solarPanel.WattageRating;
                            PowerGenerationTOOLTIP = string.Concat("• ", solarPanel.GetProperName(), ": ", GameUtil.GetFormattedWattage(solarPanel.IsProducingPower() ? solarPanel.CurrentWattage : 0), "/", GameUtil.GetFormattedWattage(solarPanel.WattageRating), "\n", PowerGenerationTOOLTIP);
                        }
                        else if (moduleGet.TryGetComponent<ModuleSolarPanelAdjustable>(out var solarPanela))
                        {
                            PowerGeneration += solarPanela.IsProducingPower() ? solarPanela.CurrentWattage : 0;
                            PowerGenerationMax += solarPanela.WattageRating;
                            PowerGenerationTOOLTIP = string.Concat("• ", solarPanela.GetProperName(), ": ", GameUtil.GetFormattedWattage(solarPanela.IsProducingPower() ? solarPanela.CurrentWattage : 0), "/", GameUtil.GetFormattedWattage(solarPanela.WattageRating), "\n", PowerGenerationTOOLTIP);
                        }
                        else if (moduleGet.TryGetComponent<RTB_ModuleGenerator>(out var generatorModule2))
                        {
                            var ConsumptionStats = generatorModule2.GetConsumptionStatusStats();
                            PowerGeneration += generatorModule2.IsProducingPower() ? generatorModule2.WattageRating : 0;
                            PowerGenerationMax += generatorModule2.WattageRating;
                            PowerGenerationTOOLTIP = string.Concat("• ", generatorModule2.GetProperName(), ": ", GameUtil.GetFormattedWattage(generatorModule2.IsProducingPower() ? generatorModule2.WattageRating : 0), "/", GameUtil.GetFormattedWattage(generatorModule2.WattageRating),
                               ConsumptionStats.second >= 0 ? string.Format(ROCKETGENERATORSTATS.TOOLTIP, GameUtil.GetFormattedMass(ConsumptionStats.first), GameUtil.GetFormattedMass(ConsumptionStats.second)) :
                               string.Format(ROCKETGENERATORSTATS.TOOLTIP2, ElementLoader.GetElement(generatorModule2.consumptionElement).name)
                               , "\n", PowerGenerationTOOLTIP);
                        }
                    }
                    ModuleOrderTOOLTIP = UI_MOD.CLUSTERMAPROCKETSIDESCREEN.ROCKETDIMENSIONS.MODULEORDER + ModuleOrderTOOLTIP;

                    ///get fuel if not radbolt engine
                    if (!FuelSorted)
                    {
                        foreach (var module in craftModuleInterface.clusterModules)
                        {
                            var moduleGet = module.Get();
                            //SgtLogger.debuglog(moduleGet.GetProperName());
                            if (moduleGet.TryGetComponent<IFuelTank>(out var fueltank))
                            {
                                FuelRemaining += fueltank.Storage.GetAmountAvailable(FuelTag);
                            }
                            if (RequiresOxidizer && moduleGet.TryGetComponent<OxidizerTank>(out var oxTanktank))
                            {
                                OxidizerRemaining += oxTanktank.TotalOxidizerPower;
                            }
                        }

                        if (RequiresOxidizer)
                            OxidizerRemaining = Mathf.CeilToInt(OxidizerRemaining);
                        else
                            OxidizerRemaining = 0;
                    }

                    ///Range based on engine
                    if (targetEngine != null)
                    {
                        if ((!Mathf.Approximately(FuelRemaining, FuelRemainingPREVIOUS)) || (RequiresOxidizer ? (!Mathf.Approximately(OxidizerRemaining, OxidizerRemainingPREVIOUS)) : false) || (!Mathf.Approximately(FuelPerHexEngine, FuelPerHexEnginePREVIOUS)))
                        {

                            if (FuelTag == GameTags.HighEnergyParticle)
                            {
                                FuelPerHexEngineSTRING = GameUtil.GetFormattedHighEnergyParticles(FuelPerHexEngine);
                                FuelRemainingSTRING = GameUtil.GetFormattedHighEnergyParticles(FuelRemaining);
                            }
                            else
                            {

                                FuelPerHexEngineSTRING = GameUtil.GetFormattedMass(FuelPerHexEngine );
                                FuelRemainingSTRING = GameUtil.GetFormattedMass(FuelRemaining);
                            }

                            RangeRemainingToolTipSTRING = string.Concat(global::STRINGS.UI.CLUSTERMAP.ROCKETS.RANGE.TOOLTIP,
                            "\n    • ", string.Format(global::STRINGS.UI.CLUSTERMAP.ROCKETS.FUEL_PER_HEX.NAME, FuelPerHexEngineSTRING),
                            "\n    • ", global::STRINGS.UI.CLUSTERMAP.ROCKETS.FUEL_REMAINING.NAME, FuelRemainingSTRING,
                            RequiresOxidizer ? "\n    • " : string.Empty,
                            RequiresOxidizer ? (string)global::STRINGS.UI.CLUSTERMAP.ROCKETS.OXIDIZER_REMAINING.NAME : string.Empty,
                            RequiresOxidizer ? GameUtil.GetFormattedMass(OxidizerRemaining) : string.Empty);

                            float RangeRemaining = targetEngine != null ? (RequiresOxidizer ? Mathf.Min(FuelRemaining, OxidizerRemaining) : FuelRemaining) / FuelPerHexEngine : 0;

                            RangeRemainingTextSTRING = string.Concat(global::STRINGS.UI.CLUSTERMAP.ROCKETS.RANGE.NAME, GameUtil.GetFormattedRocketRange(RangeRemaining, GameUtil.TimeSlice.None));

                            FuelRemainingPREVIOUS = FuelRemaining;
                            OxidizerRemainingPREVIOUS = OxidizerRemaining;
                            FuelPerHexEnginePREVIOUS = FuelPerHexEngine;
                        }
                        rocketStatusContainer.SetLabel("RangeRemaining", RangeRemainingTextSTRING, RangeRemainingToolTipSTRING);
                    }
                    else
                    {
                        rocketStatusContainer.SetLabel("RangeRemaining", string.Empty, string.Empty);
                    }

                    ///Rocket burden
                    if ((!Mathf.Approximately(RocketBurden, RocketBurdenPREVIOUS)) || (!Mathf.Approximately(RocketEnginePower, RocketEnginePowerPREVIOUS)))
                    {
                        Speed = RocketEnginePower / RocketBurden * clusterCraft.AutoPilotMultiplier * clusterCraft.PilotSkillMultiplier;
                        if (clusterCraft.controlStationBuffTimeRemaining > 0)
                        {
                            Speed += (RocketEnginePower / RocketBurden) * 0.2f;
                        }
                        SpeedToolTipSTRING = string.Concat(global::STRINGS.UI.CLUSTERMAP.ROCKETS.SPEED.TOOLTIP,
                            "\n    • ", global::STRINGS.UI.CLUSTERMAP.ROCKETS.POWER_TOTAL.NAME,
                            RocketEnginePower.ToString(),
                            "\n    • ", global::STRINGS.UI.CLUSTERMAP.ROCKETS.BURDEN_TOTAL.NAME,
                            RocketBurden.ToString());
                        SpeedTextSTRING = string.Concat(global::STRINGS.UI.CLUSTERMAP.ROCKETS.SPEED.NAME, GameUtil.GetFormattedRocketRange(Speed, GameUtil.TimeSlice.PerCycle));

                        RocketBurdenPREVIOUS = RocketBurden;
                        RocketEnginePowerPREVIOUS = RocketEnginePower;
                        SpeedPREVIOUS = Speed;
                    }
                    rocketStatusContainer.SetLabel("Speed", SpeedTextSTRING, SpeedToolTipSTRING);

                    ///Rocket dimensions, defined by engine
                    if (targetEngine != null)
                    {
                        //SgtLogger.debuglog("dims " + RocketDimensionsSTRING);
                        if (RocketHeight != RocketHeightPREVIOUS || RocketWidth != RocketWidthPREVIOUS)
                        {
                            RocketDimensionsTooltipSTRING = string.Format(UI_MOD.CLUSTERMAPROCKETSIDESCREEN.ROCKETDIMENSIONS.TOOLTIP, targetEngine.GetProperName(), RocketHeight.ToString(), RocketWidth.ToString());
                            RocketDimensionsSTRING = string.Format(UI_MOD.CLUSTERMAPROCKETSIDESCREEN.ROCKETDIMENSIONS.NAME, RocketHeight.ToString(), targetEngine.maxHeight.ToString(), RocketWidth.ToString());

                            RocketWidthPREVIOUS = RocketWidth;
                            RocketHeightPREVIOUS = RocketHeight;
                        }
                        rocketStatusContainer.SetLabel("MaxHeight", RocketDimensionsSTRING, RocketDimensionsTooltipSTRING);
                    }
                    else
                    {
                        rocketStatusContainer.SetLabel("MaxHeight", string.Empty, string.Empty);
                    }

                    ///ModuleOrder
                    if (ModuleOrderTOOLTIP != ModuleOrderTOOLTIPPREVIOUS)
                    {
                        ModuleOrderSTRING = string.Concat(UI_MOD.CLUSTERMAPROCKETSIDESCREEN.ROCKETDIMENSIONS.MODULECOUNT, moduleCounter.ToString());
                        ModuleOrderTOOLTIPPREVIOUS = ModuleOrderTOOLTIP;
                    }
                    rocketStatusContainer.SetLabel("ModuleOrder", ModuleOrderSTRING, ModuleOrderTOOLTIP);

                    ///PowerGeneration
                    if (PowerGenerationMax > 0f)
                    {
                        if (!Mathf.Approximately(PowerGeneration, PowerGenerationPREVIOUS))
                        {
                            PowerGenerationSTRING = string.Format(UI_MOD.CLUSTERMAPROCKETSIDESCREEN.ROCKETGENERATORSTATS.NAME, GameUtil.GetFormattedWattage(PowerGeneration), GameUtil.GetFormattedWattage(PowerGenerationMax));
                            PowerGenerationPREVIOUS = PowerGeneration;
                            PowerGenerationMaxPREVIOUS = PowerGenerationMax;
                        }
                        rocketStatusContainer.SetLabel("PowerGeneration", PowerGenerationSTRING, PowerGenerationTOOLTIP);
                    }
                    else
                    {
                       rocketStatusContainer.SetLabel("PowerGeneration", string.Empty, string.Empty);
                    }
                    ///PowerStorage

                    if (PowerStorageMax > 0f)
                    {
                        if (!Mathf.Approximately(PowerStorage, PowerStoragePREVIOUS))
                        {

                            PowerStorageSTRING = string.Format(UI_MOD.CLUSTERMAPROCKETSIDESCREEN.ROCKETBATTERYSTATUS.NAME, GameUtil.GetFormattedJoules(PowerStorage), GameUtil.GetFormattedJoules(PowerStorageMax));
                            PowerStoragePREVIOUS = PowerStorage;
                            PowerStorageMaxPREVIOUS = PowerStorageMax;
                        }
                        rocketStatusContainer.SetLabel("PowerStorage", PowerStorageSTRING, PowerStorageTOOLTIP);
                    }
                    else
                    {
                        rocketStatusContainer.SetLabel("PowerStorage", string.Empty, string.Empty);
                    }

                    rocketStatusContainer.SetLabel("RocketSpacer2", string.Empty, string.Empty);

                    if (rocketModuleCluster != null)
                    {
                        rocketStatusContainer.SetLabel("zModuleStats", string.Concat(global::STRINGS.UI.CLUSTERMAP.ROCKETS.MODULE_STATS.NAME, selectedTarget.GetProperName()), global::STRINGS.UI.CLUSTERMAP.ROCKETS.MODULE_STATS.TOOLTIP);
                        float burden = rocketModuleCluster.performanceStats.Burden;
                        float enginePower = rocketModuleCluster.performanceStats.EnginePower;
                        if (burden != 0f)
                        {
                            rocketStatusContainer.SetLabel("LocalBurden", string.Concat(Constants.TABBULLETSTRING, global::STRINGS.UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.NAME, burden.ToString()), string.Format(global::STRINGS.UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.TOOLTIP, burden));
                        }

                        if (enginePower != 0f)
                        {
                            rocketStatusContainer.SetLabel("LocalPower", string.Concat(Constants.TABBULLETSTRING, global::STRINGS.UI.CLUSTERMAP.ROCKETS.POWER_MODULE.NAME, enginePower.ToString()), string.Format(global::STRINGS.UI.CLUSTERMAP.ROCKETS.POWER_MODULE.TOOLTIP, enginePower));
                        }

                    }

                    ///Cargos
                    if (clusterCraft != null)
                    {
                        ///Artifact modules

                        foreach (KeyValuePair<string, GameObject> artifactModuleLabel in __instance.artifactModuleLabels)
                        {
                            //UnityEngine.Object.Destroy(artifactModuleLabel.Value);
                            artifactModuleLabel.Value.SetActive(value: false);
                        }
                        for (int j = 0; j < ArtifactModules.Count; ++j)
                        {
                            var aModule = ArtifactModules[j];
                            GameObject gameObject = __instance.simpleInfoRoot.AddOrGetStorageLabel(__instance.artifactModuleLabels, rocketStatusContainer.gameObject, "artifactModule_" + j);

                            string text2 = string.Empty;
                            text2 = ((!(aModule.Occupant != null)) ? $"{aModule.GetProperName()}: {(global::STRINGS.UI.CLUSTERMAP.ROCKETS.ARTIFACT_MODULE.EMPTY)}" : (aModule.GetProperName() + ": " + aModule.Occupant.GetProperName()));

                            Transform transform = gameObject.transform.Find("removeAttributeButton");
                            if (transform != null)
                            {
                                KButton kButton = transform.FindComponent<KButton>();
                                kButton.enabled = false;
                                kButton.gameObject.SetActive(value: false);
                                kButton.ClearOnClick();
                            }

                            gameObject.GetComponentInChildren<LocText>().text = text2;
                            gameObject.SetActive(value: true);
                        }

                        ///Cargo Bays

                        ///Resetting Container 
                        foreach (KeyValuePair<string, GameObject> cargoBayLabel in __instance.cargoBayLabels)
                        {
                            //UnityEngine.Object.Destroy(cargoBayLabel.Value);
                            cargoBayLabel.Value.SetActive(value: false);
                        }

                        for (int j = 0; j < CargoBays.Count; ++j)
                        {
                            CargoBayAndFriends currentModule = CargoBays[j];
                            if (currentModule.CargoBay != null)
                            {
                                var currentCargoBay = currentModule.CargoBay;
                                ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.PooledList pooledList = ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.Allocate();

                                GameObject RowEntry = __instance.simpleInfoRoot.AddOrGetStorageLabel(__instance.cargoBayLabels, rocketStatusContainer.gameObject, "cargoBay_" + j);

                                Transform transform = RowEntry.transform.Find("removeAttributeButton");
                                if (transform != null)
                                {
                                    KButton kButton = transform.FindComponent<KButton>();
                                    kButton.enabled = false;
                                    kButton.gameObject.SetActive(value: false);
                                    kButton.ClearOnClick();
                                }

                                string CargobayText = $"{currentCargoBay.storage.GetComponent<KPrefabID>().GetProperName()}: {GameUtil.GetFormattedMass(currentCargoBay.storage.MassStored())}/{GameUtil.GetFormattedMass(currentCargoBay.storage.capacityKg)}";

                                foreach (GameObject item2 in currentCargoBay.storage.GetItems())
                                {
                                    item2.TryGetComponent<KPrefabID>(out KPrefabID component2);
                                    item2.TryGetComponent<PrimaryElement>(out PrimaryElement component3);
                                    string a = $"{component2.GetProperName()} : {GameUtil.GetFormattedMass(component3.Mass)}";
                                    pooledList.Add(new Tuple<string, TextStyleSetting>(a, PluginAssets.Instance.defaultTextStyleSetting));
                                }
                                string CarboBayTooltip = string.Empty;
                                for (int i = 0; i < pooledList.Count; i++)
                                {
                                    CarboBayTooltip += pooledList[i].first;
                                    if (i != pooledList.Count - 1)
                                    {
                                        CarboBayTooltip += "\n";
                                    }
                                }
                                RowEntry.GetComponentInChildren<LocText>().text = CargobayText;
                                RowEntry.GetComponentInChildren<ToolTip>().SetSimpleTooltip(CarboBayTooltip);
                                pooledList.Recycle();
                            }
                            else if (currentModule.CritterModule != null)
                            {
                                var critterHolder = currentModule.CritterModule;
                                GameObject RowEntry = __instance.simpleInfoRoot.AddOrGetStorageLabel(__instance.cargoBayLabels, rocketStatusContainer.gameObject, "cargoBay_" + j);
                                string CargobayText = $"{critterHolder.GetComponent<KPrefabID>().GetProperName()}: {Util.FormatWholeNumber(critterHolder.CurrentCapacity)}/{Util.FormatWholeNumber(Config.Instance.CritterStorageCapacity)}";
                                string ToolTip = critterHolder.GetStatusItem();

                                Transform transform = RowEntry.transform.Find("removeAttributeButton");
                                if (transform != null)
                                {
                                    KButton kButton = transform.FindComponent<KButton>();
                                    kButton.enabled = false;
                                    kButton.gameObject.SetActive(value: false);
                                    kButton.ClearOnClick();
                                }

                                RowEntry.GetComponentInChildren<LocText>().text = CargobayText;
                                if(ToolTip!=string.Empty)
                                    RowEntry.GetComponentInChildren<ToolTip>().SetSimpleTooltip(ToolTip);
                            }
                            else if (currentModule.HepBatteryModule != null)
                            {
                                var HepBattery = currentModule.HepBatteryModule;
                                GameObject RowEntry = __instance.simpleInfoRoot.AddOrGetStorageLabel(__instance.cargoBayLabels, rocketStatusContainer.gameObject, "cargoBay_" + j);
                                string CargobayText = $"{HepBattery.GetComponent<KPrefabID>().GetProperName()}: {Util.FormatWholeNumber(HepBattery.hepStorage.Particles)}/{Util.FormatWholeNumber(HepBattery.hepStorage.capacity)}";
                                string ToolTip = string.Empty;

                                Transform transform = RowEntry.transform.Find("removeAttributeButton");
                                if (transform != null)
                                {
                                    KButton kButton = transform.FindComponent<KButton>();
                                    kButton.enabled = false;
                                    kButton.gameObject.SetActive(value: false);
                                    kButton.ClearOnClick();
                                }

                                RowEntry.GetComponentInChildren<LocText>().text = CargobayText;
                                if (ToolTip != string.Empty)
                                    RowEntry.GetComponentInChildren<ToolTip>().SetSimpleTooltip(ToolTip);
                            }
                        }

                    }

                }
                ArtifactModules.Recycle();
                CargoBays.Recycle();

                rocketStatusContainer.Commit();
                return false;
            }
        }

        class ExtendSolarNotification
        {
            [HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
            public static class SolarNoseconeStatusItems
            {
                public static void Postfix(BuildingStatusItems __instance)
                {
                    __instance.ModuleSolarPanelWattage.resolveStringCallback = (Func<string, object, string>)((str, data) =>
                    {
                        if (data is ModuleSolarPanel)
                        {
                            ModuleSolarPanel moduleSolarPanel = (ModuleSolarPanel)data;
                            str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(moduleSolarPanel.CurrentWattage));
                        }
                        else if (data is ModuleSolarPanelAdjustable)
                        {
                            ModuleSolarPanelAdjustable moduleSolarPanel = (ModuleSolarPanelAdjustable)data;
                            str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(moduleSolarPanel.CurrentWattage));
                        }

                        return str;
                    });
                }

            }

        }

    }
}
