using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static UL_UniversalLyzer.ModAssets;

namespace UL_UniversalLyzer
{
    internal class Patches
    {

        private static ConduitPortInfo O2Port = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 0));
        private static ConduitPortInfo H2Port = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 0));
        private static ConduitPortInfo ClPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 1));


        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch(nameof(ElectrolyzerConfig.CreateBuildingDef))]
        public static class ModifyBuildingDef
        {
            public static void Postfix(BuildingDef __result)
            {
                __result.EnergyConsumptionWhenActive = Config.Instance.consumption_water;
                __result.ExhaustKilowattsWhenActive = ((float)Config.Instance.consumption_water) / 480f;
                __result.SelfHeatKilowattsWhenActive = ((float)Config.Instance.consumption_water) / 120f;

            }
        }

        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch(nameof(ElectrolyzerConfig.ConfigureBuildingTemplate))]
        public static class AddAdditionalLyzerRecipes
        {

            public static void Postfix(GameObject go)
            {
                bool storeOutput = Config.Instance.IsPiped;
                go.TryGetComponent<Electrolyzer>(out var oldLyzer);
                CellOffset emissionOffset = oldLyzer.emissionOffset;
                CellOffset secondaryEmissionOffset = new CellOffset(1, 1);
                if (go.TryGetComponent<ConduitConsumer>(out var consumer))
                {
                    consumer.capacityTag = GameTags.AnyWater;
                }
                ElementConverter pWaterConverter = go.AddComponent<ElementConverter>();
                pWaterConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(SimHashes.DirtyWater.CreateTag(), 1f)
                };
                pWaterConverter.outputElements =
                    Config.Instance.SolidDebris 
                    ? new ElementConverter.OutputElement[]
                    {
                        new ElementConverter.OutputElement(0.886f, SimHashes.ContaminatedOxygen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.111f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.003f, SimHashes.ToxicSand, 343.15f, useEntityTemperature: false, storeOutput: false, 1, 0)
                    }
                    : new ElementConverter.OutputElement[]
                    {
                        new ElementConverter.OutputElement(0.888f, SimHashes.ContaminatedOxygen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.112f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y)
                    };

                ElementConverter saltWaterConverter = go.AddComponent<ElementConverter>();
                saltWaterConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(SimHashes.SaltWater.CreateTag(), 1f)
                };
                saltWaterConverter.outputElements =
                    Config.Instance.SolidDebris
                    ? 
                    new ElementConverter.OutputElement[]
                    {
                        new ElementConverter.OutputElement(0.826f, SimHashes.Oxygen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.104f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.047f, SimHashes.ChlorineGas, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, secondaryEmissionOffset.x, secondaryEmissionOffset.y),
                        new ElementConverter.OutputElement(0.023f, SimHashes.Salt, 343.15f, useEntityTemperature: false, storeOutput: false, 1, 0)
                    } 
                    :
                    new ElementConverter.OutputElement[]
                    {
                        new ElementConverter.OutputElement(0.826f, SimHashes.Oxygen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.104f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.070f, SimHashes.ChlorineGas, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, secondaryEmissionOffset.x, secondaryEmissionOffset.y)
                    };

                ElementConverter brineWaterConverter = go.AddComponent<ElementConverter>();
                brineWaterConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(SimHashes.Brine.CreateTag(), 1f)
                };
                brineWaterConverter.outputElements =
                    Config.Instance.SolidDebris
                    ? new ElementConverter.OutputElement[]
                    {
                        new ElementConverter.OutputElement(0.622f, SimHashes.Oxygen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.78f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.200f, SimHashes.ChlorineGas, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, secondaryEmissionOffset.x, secondaryEmissionOffset.y),
                        new ElementConverter.OutputElement(0.100f, SimHashes.Salt, 343.15f, useEntityTemperature: false, storeOutput: false, 1, 0 )
                    } 
                    : new ElementConverter.OutputElement[]
                    {
                        new ElementConverter.OutputElement(0.622f, SimHashes.Oxygen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.78f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, emissionOffset.x, emissionOffset.y),
                        new ElementConverter.OutputElement(0.300f, SimHashes.ChlorineGas, 343.15f, useEntityTemperature: false, storeOutput: storeOutput, secondaryEmissionOffset.x, secondaryEmissionOffset.y)
                    };

                UnityEngine.Object.Destroy(oldLyzer);
                go.AddComponent<MultiConverterElectrolyzer>();


                if (storeOutput)
                    AttachPorts(go);

            }
        }
        private static void AttachPorts(GameObject go)
        {
            go.AddComponent<ConduitSecondaryOutput>().portInfo = O2Port;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = H2Port;
            go.AddComponent<ConduitSecondaryOutput>().portInfo = ClPort;
        }
        public static void AddPipes(GameObject go)
        {
            if(!Config.Instance.IsPiped) return;

            //var ox = go.AddComponent<ConduitElementOutput>();
            //ox.portInfo = O2Port;
            //ox.elementFilter = new SimHashes[] { SimHashes.Oxygen, SimHashes.ContaminatedOxygen };
            //ox.alwaysDispense = true;

            //var h2 = go.AddComponent<ConduitElementOutput>();
            //h2.portInfo = H2Port;
            //h2.elementFilter = new SimHashes[] { SimHashes.Hydrogen };
            //h2.alwaysDispense = true;

            //var Cl = go.AddComponent<ConduitElementOutput>();
            //Cl.portInfo = ClPort;
            //Cl.elementFilter = new SimHashes[] { SimHashes.ChlorineGas };
            //Cl.alwaysDispense = true;

        }
        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch(nameof(ElectrolyzerConfig.DoPostConfigureComplete))]
        public static class PostConfig
        {

            public static void Postfix(GameObject go)
            {
                if (!Config.Instance.IsPiped)
                    return;
                
                AttachPorts(go);
                AddPipes(go);
            }
        }

        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
    }
}
