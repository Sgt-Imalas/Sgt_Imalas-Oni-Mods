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

                if (go.TryGetComponent<ConduitConsumer>(out var consumer))
                {
                    consumer.capacityTag = GameTags.AnyWater;
                }

                var newLyzer = go.AddComponent<MultiConverterElectrolyzer>();

                newLyzer.emissionOffset = oldLyzer.emissionOffset;
                newLyzer.maxMass = oldLyzer.maxMass;
                newLyzer.simRenderLoadBalance = oldLyzer.simRenderLoadBalance;

                UnityEngine.Object.Destroy(oldLyzer);
            }
        }
        static System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public
                                            | System.Reflection.BindingFlags.NonPublic
                                            | System.Reflection.BindingFlags.Static
                                            | System.Reflection.BindingFlags.Instance;


        public static Type NightLib_PortDisplayOutput_Type = Type.GetType("NightLib.PortDisplayOutput, NightLib", false, false);
        public static Type NightLib_PortDisplayController_Type = Type.GetType("NightLib.PortDisplayController, NightLib", false, false);
        public static Type NightLib_PipedDispenser_Type = Type.GetType("Nightinggale.PipedOutput.PipedDispenser, NightLib", false, false);
        public static Type NightLib_PipedOptionalExhaust_Type = Type.GetType("Nightinggale.PipedOutput.PipedOptionalExhaust, NightLib", false, false);
        public static void AddPipes(GameObject go)
        {
            if (NightLib_PortDisplayOutput_Type == null)
            {
                SgtLogger.warning("Piped Output Class not found, Piped Output wont be active");
                return;
            }
            var ConstructorMethod_PortDisplayOutput = NightLib_PortDisplayOutput_Type.GetConstructor(flags, null, new System.Type[]
            {
                typeof (ConduitType),
                typeof (CellOffset),
                typeof (CellOffset?),
                typeof (Color?)
            }, null);

            if (ConstructorMethod_PortDisplayOutput == null)
            {
                SgtLogger.logwarning(nameof(ConstructorMethod_PortDisplayOutput)+"Not Found!");
                return;
            }

            var controller = go.GetComponent(NightLib_PortDisplayController_Type);
            if (controller == null)
            {
                controller = go.AddComponent(NightLib_PortDisplayController_Type);
                Traverse.Create(controller).Method("Init").GetValue(new object[] { go });
            }

            ///Chlorine
            var chlorineColour =  ElementLoader.GetElement(SimHashes.ChlorineGas.CreateTag()).substance.conduitColour;
            chlorineColour.a = 255;

            var PortDisplayOutput_Instance_Chlorine = ConstructorMethod_PortDisplayOutput.Invoke(new object[] { ConduitType.Gas, new CellOffset(0,0),null, chlorineColour });

            Traverse.Create(controller).Method("AssignPort").GetValue(new object[] { go, PortDisplayOutput_Instance_Chlorine });

            var PipedDispenser_Cl = go.AddComponent(NightLib_PipedDispenser_Type);
            Traverse.Create(PipedDispenser_Cl).Field("elementFilter").SetValue(new object[] { new SimHashes[] { SimHashes.ChlorineGas } });
            Traverse.Create(PipedDispenser_Cl).Method("AssignPort").GetValue(new object[] { go, PortDisplayOutput_Instance_Chlorine });
            Traverse.Create(PipedDispenser_Cl).Field("alwaysDispense").SetValue(new object[] { true });
            Traverse.Create(PipedDispenser_Cl).Field("SkipSetOperational").SetValue(new object[] { true });

            var PipedOptionalExhaust_Cl = go.AddComponent(NightLib_PipedOptionalExhaust_Type);
            Traverse.Create(PipedOptionalExhaust_Cl).Field("dispenser").SetValue(new object[] { PipedDispenser_Cl });
            Traverse.Create(PipedOptionalExhaust_Cl).Field("elementHash").SetValue(new object[] { SimHashes.ChlorineGas });
            Traverse.Create(PipedOptionalExhaust_Cl).Field("elementTag").SetValue(new object[] { SimHashes.ChlorineGas.CreateTag() });
            Traverse.Create(PipedOptionalExhaust_Cl).Field("capacity").SetValue(new object[] { 1f });


            ///pOx

            var poxColour = ElementLoader.GetElement(SimHashes.ContaminatedOxygen.CreateTag()).substance.conduitColour;
            poxColour.a = 255;

            var PortDisplayOutput_Instance_pOx = ConstructorMethod_PortDisplayOutput.Invoke(new object[] { ConduitType.Gas, new CellOffset(1, 0), null, poxColour });


            Traverse.Create(controller).Method("AssignPort").GetValue(new object[] {go, PortDisplayOutput_Instance_pOx });

            var PipedDispenser_pox = go.AddComponent(NightLib_PipedDispenser_Type);
            Traverse.Create(PipedDispenser_pox).Field("elementFilter").SetValue(new object[] { new SimHashes[] { SimHashes.ContaminatedOxygen } });
            Traverse.Create(PipedDispenser_pox).Method("AssignPort").GetValue(new object[] { go, PortDisplayOutput_Instance_Chlorine });
            Traverse.Create(PipedDispenser_pox).Field("alwaysDispense").SetValue(new object[] { true });
            Traverse.Create(PipedDispenser_pox).Field("SkipSetOperational").SetValue(new object[] { true });

            var PipedOptionalExhaust_POX = go.AddComponent(NightLib_PipedOptionalExhaust_Type);
            Traverse.Create(PipedOptionalExhaust_POX).Field("dispenser").SetValue(new object[] { PipedDispenser_pox });
            Traverse.Create(PipedOptionalExhaust_POX).Field("elementHash").SetValue(new object[] { SimHashes.ContaminatedOxygen });
            Traverse.Create(PipedOptionalExhaust_POX).Field("elementTag").SetValue(new object[] { SimHashes.ContaminatedOxygen.CreateTag() });
            Traverse.Create(PipedOptionalExhaust_POX).Field("capacity").SetValue(new object[] { 1f });




        }
        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch(nameof(ElectrolyzerConfig.DoPostConfigureComplete))]
        public static class PostConfig
        {

            public static void Postfix(GameObject go)
            {
                if (!Config.Instance.IsPiped)
                    return;

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
