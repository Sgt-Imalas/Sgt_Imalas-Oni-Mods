using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace UL_UniversalLyzer
{
    public class ModAssets
    {

        public static Dictionary<SimHashes, ElectrolyzerConfiguration> ElectrolyzerConfigurations = new Dictionary<SimHashes, ElectrolyzerConfiguration>();

        public static CellOffset defaultLyzerOffset = new CellOffset(0, 1);
        public static CellOffset debrisOffset = new CellOffset(1, 0);
        public static CellOffset tertiaryGasOffset = new CellOffset(1, 1);

        public static bool IsPipedAndPipedBuildingsActive => Patches.NightLib_PortDisplayOutput_Type != null && Patches.NightLib_PortDisplayController_Type != null && Patches.NightLib_PipedDispenser_Type != null && Patches.NightLib_PipedOptionalExhaust_Type != null && Config.Instance.IsPiped;

        public static void InitializeOrUpdateLyzerPowerCosts()
        {

            AddLyzerConfiguration(SimHashes.Water, new ElementConverter.ConsumedElement[1]
            {
                    new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 1f)
            },
            new ElementConverter.OutputElement[]
            {
                new ElementConverter.OutputElement(0.888f, SimHashes.Oxygen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_water), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                new ElementConverter.OutputElement(0.112f, SimHashes.Hydrogen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_water), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive,defaultLyzerOffset.x, defaultLyzerOffset.y),
            },
            Config.Instance.consumption_water,
            Config.Instance.PressureThresholdMass_water
            );

            AddLyzerConfiguration(SimHashes.DirtyWater, new ElementConverter.ConsumedElement[1]
            {
                    new ElementConverter.ConsumedElement(SimHashes.DirtyWater.CreateTag(), 1f)
            },

                Config.Instance.SolidDebris
                ? new ElementConverter.OutputElement[]
                {
                        new ElementConverter.OutputElement(0.886f, SimHashes.ContaminatedOxygen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_pollutedwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.111f, SimHashes.Hydrogen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_pollutedwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.003f, SimHashes.ToxicSand, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_pollutedwater), useEntityTemperature: false, storeOutput: false, debrisOffset.x, debrisOffset.y)
                }
                : new ElementConverter.OutputElement[]
                {
                        new ElementConverter.OutputElement(0.888f, SimHashes.ContaminatedOxygen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_pollutedwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.112f, SimHashes.Hydrogen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_pollutedwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y)
                }
            ,
            Config.Instance.consumption_pollutedwater,
            Config.Instance.PressureThresholdMass_pollutedwater
            );

            AddLyzerConfiguration(SimHashes.SaltWater, new ElementConverter.ConsumedElement[1]
            {
                    new ElementConverter.ConsumedElement(SimHashes.SaltWater.CreateTag(), 1f)
            },

               Config.Instance.SolidDebris
                ?
                new ElementConverter.OutputElement[]
                {
                        new ElementConverter.OutputElement(0.826f, SimHashes.Oxygen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_saltwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.104f, SimHashes.Hydrogen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_saltwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.047f, SimHashes.ChlorineGas, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_saltwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, tertiaryGasOffset.x, tertiaryGasOffset.y),
                        new ElementConverter.OutputElement(0.023f, SimHashes.Salt, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_saltwater), useEntityTemperature: false, storeOutput: false, debrisOffset.x, debrisOffset.y)
                }
                :
                new ElementConverter.OutputElement[]
                {
                        new ElementConverter.OutputElement(0.826f, SimHashes.Oxygen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_saltwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.104f, SimHashes.Hydrogen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_saltwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.070f, SimHashes.ChlorineGas, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_saltwater), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, tertiaryGasOffset.x, tertiaryGasOffset.y)
                }
            ,
            Config.Instance.consumption_saltwater,
            Config.Instance.PressureThresholdMass_saltwater
            );

            AddLyzerConfiguration(SimHashes.Brine, new ElementConverter.ConsumedElement[1]
            {
                    new ElementConverter.ConsumedElement(SimHashes.Brine.CreateTag(), 1f)
            },

              Config.Instance.SolidDebris
                ? new ElementConverter.OutputElement[]
                {
                        new ElementConverter.OutputElement(0.622f, SimHashes.Oxygen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_brine), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.078f, SimHashes.Hydrogen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_brine), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.200f, SimHashes.ChlorineGas, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_brine), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, tertiaryGasOffset.x, tertiaryGasOffset.y),
                        new ElementConverter.OutputElement(0.100f, SimHashes.Salt, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_brine), useEntityTemperature: false, storeOutput: false, debrisOffset.x, debrisOffset.y)
                }
                : new ElementConverter.OutputElement[]
                {
                        new ElementConverter.OutputElement(0.622f, SimHashes.Oxygen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_brine), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.78f, SimHashes.Hydrogen, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_brine), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, defaultLyzerOffset.x, defaultLyzerOffset.y),
                        new ElementConverter.OutputElement(0.300f, SimHashes.ChlorineGas, UtilMethods.GetKelvinFromC(Config.Instance.minOutputTemp_brine), useEntityTemperature: false, storeOutput: IsPipedAndPipedBuildingsActive, tertiaryGasOffset.x, tertiaryGasOffset.y)
                }
            ,
            Config.Instance.consumption_brine,
            Config.Instance.PressureThresholdMass_brine
            );
        }

        public static float GetWattageForElement(SimHashes element)
        {
            if (ElectrolyzerConfigurations.ContainsKey(element))
            {
                return ElectrolyzerConfigurations[element].PowerConsumption;
            }
            return ElectrolyzerConfigurations[SimHashes.Water].PowerConsumption;

        }
        public static void AddLyzerConfiguration(SimHashes element, ElectrolyzerConfiguration config)
        {
            ElectrolyzerConfigurations[element] = config;
        }

        public static void AddLyzerConfiguration(SimHashes element, ElementConverter.ConsumedElement[] InputElements, ElementConverter.OutputElement[] outputElements, float power, float threshold)
        {
            ElectrolyzerConfigurations[element] = new ElectrolyzerConfiguration(InputElements, outputElements, power, threshold);
        }


        public class ElectrolyzerConfiguration
        {
            public List<ElementConverter.ConsumedElement> InputElements;
            public List<ElementConverter.OutputElement> OutputElements;

            public float PowerConsumption;
            public float OverpressurisationThreshold;
            public ElectrolyzerConfiguration(ElementConverter.ConsumedElement[] input, ElementConverter.OutputElement[] output, float power, float overpressurisationThreshold)
            {
                InputElements = input.ToList();
                OutputElements = output.ToList();
                PowerConsumption = power;
                OverpressurisationThreshold = overpressurisationThreshold;
            }

        }
    }
}
