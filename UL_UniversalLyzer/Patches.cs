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
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch(nameof(ElectrolyzerConfig.ConfigureBuildingTemplate))]
        public static class AddAdditionalLyzerRecipes
        {

            public static void Postfix(GameObject go)
            {
                if(go.TryGetComponent<Electrolyzer>(out var oldLyzer))
                {
                    UnityEngine.Object.Destroy(oldLyzer);
                }
                CellOffset emissionOffset = new CellOffset(0, 1);
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
                pWaterConverter.outputElements = new ElementConverter.OutputElement[]
                {
                    new ElementConverter.OutputElement(0.886f, SimHashes.ContaminatedOxygen, 343.15f, useEntityTemperature: false, storeOutput: false, emissionOffset.x, emissionOffset.y),
                    new ElementConverter.OutputElement(0.111f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: false, emissionOffset.x, emissionOffset.y),
                    new ElementConverter.OutputElement(0.003f, SimHashes.ToxicSand, 343.15f, useEntityTemperature: false, storeOutput: false, 0, 0)
                };

                ElementConverter saltWaterConverter = go.AddComponent<ElementConverter>();
                saltWaterConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(SimHashes.SaltWater.CreateTag(), 1f)
                };
                saltWaterConverter.outputElements = new ElementConverter.OutputElement[]
                {
                    new ElementConverter.OutputElement(0.826f, SimHashes.Oxygen, 343.15f, useEntityTemperature: false, storeOutput: false, emissionOffset.x, emissionOffset.y),
                    new ElementConverter.OutputElement(0.104f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: false, emissionOffset.x, emissionOffset.y),
                    new ElementConverter.OutputElement(0.047f, SimHashes.Chlorine, 343.15f, useEntityTemperature: false, storeOutput: false, secondaryEmissionOffset.x, secondaryEmissionOffset.y),
                    new ElementConverter.OutputElement(0.023f, SimHashes.Salt, 343.15f, useEntityTemperature: false, storeOutput: false, 0, 0)
                };

                ElementConverter brineWaterConverter = go.AddComponent<ElementConverter>();
                brineWaterConverter.consumedElements = new ElementConverter.ConsumedElement[1]
                {
                    new ElementConverter.ConsumedElement(SimHashes.Brine.CreateTag(), 1f)
                };
                brineWaterConverter.outputElements = new ElementConverter.OutputElement[]
                {
                    new ElementConverter.OutputElement(0.622f, SimHashes.Oxygen, 343.15f, useEntityTemperature: false, storeOutput: false, emissionOffset.x, emissionOffset.y),
                    new ElementConverter.OutputElement(0.78f, SimHashes.Hydrogen, 343.15f, useEntityTemperature: false, storeOutput: false, emissionOffset.x, emissionOffset.y),
                    new ElementConverter.OutputElement(0.200f, SimHashes.Chlorine, 343.15f, useEntityTemperature: false, storeOutput: false, secondaryEmissionOffset.x, secondaryEmissionOffset.y),
                    new ElementConverter.OutputElement(0.100f, SimHashes.Salt, 343.15f, useEntityTemperature: false, storeOutput: false, 0, 0 )
                };
                go.AddComponent<MultiConverterElectrolyzer>();
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
