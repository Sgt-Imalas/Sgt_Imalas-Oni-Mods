using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UtilLibs;
using static UL_UniversalLyzer.ModAssets;

namespace UL_UniversalLyzer
{
    internal class Patches
    {
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
                newLyzer.simRenderLoadBalance = oldLyzer.simRenderLoadBalance;

                UnityEngine.Object.Destroy(oldLyzer);
            }
        }

        static System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public
                                            | System.Reflection.BindingFlags.NonPublic
                                            | System.Reflection.BindingFlags.Static
                                            | System.Reflection.BindingFlags.Instance;


        public static Type NightLib_PortDisplayOutput_Type = Type.GetType("NightLib.PortDisplayOutput, PipedOutput", false, false);
        public static Type NightLib_PortDisplayController_Type = Type.GetType("NightLib.PortDisplayController, PipedOutput", false, false);
        public static Type NightLib_PipedDispenser_Type = Type.GetType("Nightinggale.PipedOutput.PipedDispenser, PipedOutput", false, false);
        public static Type NightLib_PipedOptionalExhaust_Type = Type.GetType("Nightinggale.PipedOutput.PipedOptionalExhaust, PipedOutput", false, false);

        public static void AddPipes(GameObject go)
        {
            NightLib_PortDisplayOutput_Type = Type.GetType("NightLib.PortDisplayOutput, PipedOutput", false, false);
            NightLib_PortDisplayController_Type = Type.GetType("NightLib.PortDisplayController, PipedOutput", false, false);
            NightLib_PipedDispenser_Type = Type.GetType("Nightinggale.PipedOutput.PipedDispenser, PipedOutput", false, false);
            NightLib_PipedOptionalExhaust_Type = Type.GetType("Nightinggale.PipedOutput.PipedOptionalExhaust, PipedOutput", false, false);

        SgtLogger.Assert(nameof(NightLib_PortDisplayOutput_Type), NightLib_PortDisplayOutput_Type);
            SgtLogger.Assert(nameof(NightLib_PortDisplayController_Type), NightLib_PortDisplayController_Type);
            SgtLogger.Assert(nameof(NightLib_PipedDispenser_Type), NightLib_PipedDispenser_Type);
            SgtLogger.Assert(nameof(NightLib_PipedOptionalExhaust_Type), NightLib_PipedOptionalExhaust_Type);

            //SgtLogger.l(1 + " " + TryFindType("PortDisplayOutput"));
            //SgtLogger.l(2 + " " + TryFindType("PortDisplayController"));
            //SgtLogger.l(3 + " " + TryFindType("PipedDispenser"));
            //SgtLogger.l(4 + " " + TryFindType("PipedOptionalExhaust"));




            if (NightLib_PortDisplayOutput_Type == null || NightLib_PortDisplayController_Type == null || NightLib_PipedDispenser_Type == null || NightLib_PipedOptionalExhaust_Type == null )
            {
                SgtLogger.warning("Failed to initialize Piped Output Class types, Piped Output wont be active");
                return;
            }

            SgtLogger.l("Successfully initialized Piped Output types");
            InitializeOrUpdateLyzerPowerCosts();

            var ConstructorMethod_PortDisplayOutput = NightLib_PortDisplayOutput_Type.GetConstructor(flags, null, new System.Type[]
            {
                typeof (ConduitType),
                typeof (CellOffset),
                typeof (CellOffset?),
                typeof (Color?)
            }, null);

            if (ConstructorMethod_PortDisplayOutput == null)
            {
                SgtLogger.logwarning(nameof(ConstructorMethod_PortDisplayOutput) + "Not Found!");
                return;
            }
            

            var controller = go.GetComponent(NightLib_PortDisplayController_Type);
            if (controller == null)
            {
                controller = go.AddComponent(NightLib_PortDisplayController_Type);
                SgtLogger.Assert("PortDisplayController", controller);

                Traverse.Create(controller).Method("Init", new object[] { go }).GetValue();
            }
            

            ///Chlorine
            var chlorineColour = ElementLoader.GetElement(SimHashes.ChlorineGas.CreateTag()).substance.conduitColour;
            chlorineColour.a = 255;

            var PortDisplayOutput_Instance_Chlorine = ConstructorMethod_PortDisplayOutput.Invoke(new object[] { ConduitType.Gas, new CellOffset(0, 0), null, ((Color?)chlorineColour )});


            SgtLogger.Assert("PortDisplayOutput_Instance_Chlorine", PortDisplayOutput_Instance_Chlorine);

            Traverse.Create(controller).Method("AssignPort", new object[] { go, PortDisplayOutput_Instance_Chlorine }).GetValue();
            

            var PipedDispenser_Cl = go.AddComponent(NightLib_PipedDispenser_Type);

            Traverse.Create(PipedDispenser_Cl).Field("elementFilter").SetValue(new SimHashes[] { SimHashes.ChlorineGas });
            Traverse.Create(PipedDispenser_Cl).Method("AssignPort", PortDisplayOutput_Instance_Chlorine).GetValue();
            Traverse.Create(PipedDispenser_Cl).Field("alwaysDispense").SetValue(true );
            Traverse.Create(PipedDispenser_Cl).Field("SkipSetOperational").SetValue(true);


            var PipedOptionalExhaust_Cl = go.AddComponent(NightLib_PipedOptionalExhaust_Type);
            Traverse.Create(PipedOptionalExhaust_Cl).Field("dispenser").SetValue(PipedDispenser_Cl);
            Traverse.Create(PipedOptionalExhaust_Cl).Field("elementHash").SetValue(SimHashes.ChlorineGas);
            Traverse.Create(PipedOptionalExhaust_Cl).Field("elementTag").SetValue(SimHashes.ChlorineGas.CreateTag() );
            Traverse.Create(PipedOptionalExhaust_Cl).Field("capacity").SetValue(5f);

            

            ///pOx

            var poxColour = ElementLoader.GetElement(SimHashes.ContaminatedOxygen.CreateTag()).substance.conduitColour;
            poxColour.a = 255;

            var PortDisplayOutput_Instance_pOx = ConstructorMethod_PortDisplayOutput.Invoke(new object[] { ConduitType.Gas, new CellOffset(1, 0), null, ((Color?)poxColour) });
            
            Traverse.Create(controller).Method("AssignPort",new object[] { go, PortDisplayOutput_Instance_pOx }).GetValue();
            

            var PipedDispenser_pox = go.AddComponent(NightLib_PipedDispenser_Type);
            Traverse.Create(PipedDispenser_pox).Field("elementFilter").SetValue(new SimHashes[] { SimHashes.ContaminatedOxygen });
            Traverse.Create(PipedDispenser_pox).Method("AssignPort", PortDisplayOutput_Instance_pOx).GetValue();
            Traverse.Create(PipedDispenser_pox).Field("alwaysDispense").SetValue(true);
            Traverse.Create(PipedDispenser_pox).Field("SkipSetOperational").SetValue(true);
            

            var PipedOptionalExhaust_POX = go.AddComponent(NightLib_PipedOptionalExhaust_Type);
            Traverse.Create(PipedOptionalExhaust_POX).Field("dispenser").SetValue(PipedDispenser_pox);
            Traverse.Create(PipedOptionalExhaust_POX).Field("elementHash").SetValue(SimHashes.ContaminatedOxygen);
            Traverse.Create(PipedOptionalExhaust_POX).Field("elementTag").SetValue(SimHashes.ContaminatedOxygen.CreateTag());
            Traverse.Create(PipedOptionalExhaust_POX).Field("capacity").SetValue(5f);

            



        }
        [HarmonyPatch(typeof(ElectrolyzerConfig))]
        [HarmonyPatch(nameof(ElectrolyzerConfig.DoPostConfigureComplete))]
        public static class PostConfig
        {
            [HarmonyPriority(Priority.LowerThanNormal)]
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
