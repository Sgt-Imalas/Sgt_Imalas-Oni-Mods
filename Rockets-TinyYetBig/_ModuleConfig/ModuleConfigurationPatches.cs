using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.Patches.AnimationFixes;

namespace Rockets_TinyYetBig._ModuleConfig
{
    internal class ModuleConfigurationPatches
    {
        //[HarmonyPatch(typeof(BuildingTemplates), nameof(BuildingTemplates.CreateRocketBuildingDef))]
        public static class ReadAndModifyBuildingDefs
        {
            public static void Postfix(BuildingDef __result)
            {
                ///Read Settings
                SgtLogger.l($"Reading Settings for buildingdef: {__result.PrefabID}, width: {__result.WidthInCells}, height: {__result.HeightInCells}");
                ModuleConfigManager.Instance.PrepareBuildingDefForRegistration(__result);
            }

            //[HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                List<Type> TargetTypes = new List<Type>();
                List<MethodBase> TargetMethods = new List<MethodBase>();
                foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) 
                {
                    var subclassTypes = assembly.GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(IBuildingConfig)));
                    TargetTypes.AddRange(subclassTypes);
                }
                foreach (var type in TargetTypes) 
                {
                    //MethodInfo method = type.GetMethod("CreateBuildingDef");

                    MethodInfo method = AccessTools.Method(type, "CreateBuildingDef", new Type[] { });
                    //SgtLogger.l($"Type {type.FullName}, method: {method.Name} {!method.IsGenericMethod}, {method.HasMethodBody()}"); 

                    if (method != null && !method.IsAbstract  && !method.IsGenericMethod && method.HasMethodBody() && method.DeclaringType != typeof(IBuildingConfig) )
                    {
                        TargetMethods.Add(method);
                    }
                }
                SgtLogger.l("All Methods acquired, Count: " + TargetMethods.Count());
                return TargetMethods;
            }

        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public static class Db_Init_Patch
        {

            public static void Postfix()
            {
                System.Reflection.MethodInfo postfix = AccessTools.Method(typeof(ReadAndModifyBuildingDefs), "Postfix");
                foreach(var MethodToPatch in ReadAndModifyBuildingDefs.TargetMethods())
                    Mod.haromy.Patch(MethodToPatch, null, new HarmonyMethod(postfix,Priority.VeryHigh));
            }
        }





        [HarmonyPatch(typeof(BuildingTemplates), nameof(BuildingTemplates.CreateRocketBuildingDef))]
        public static class RegisterRocketModules
        {
            [HarmonyPriority(Priority.VeryHigh)]
            public static void Prefix(BuildingDef def)
            {
                ///Apply Settings
                SgtLogger.l($"ModuleInitialisation: {def.PrefabID}, width: {def.WidthInCells}, height: {def.HeightInCells}");
                ModuleConfigManager.Instance.FinalizeRegistration(def); 
                ModuleConfigManager.Instance.LoadCustomValuesForModuleBuildingDef(def);
            }

        }

        [HarmonyPatch(typeof(BuildingTemplates), nameof(BuildingTemplates.ExtendBuildingToRocketModuleCluster))]
        public static class ReadAndApplyModuleConfigurations
        {

            [HarmonyPrefix]
            [HarmonyPriority(Priority.VeryHigh)]
            public static void Prefix(GameObject template, int burden, float enginePower = 0.0f, float fuelCostPerDistance = 0.0f)
            {
                ///Read Settings
                ///

                ModuleConfigManager.Instance.AddOriginalModuleDefinitions(template, burden, enginePower, fuelCostPerDistance);
                SgtLogger.l($"Original RocketModuleStats: {template.GetComponent<Building>().Def.PrefabID}, burden: {burden}, engine: {enginePower}, fuelPerHex: {fuelCostPerDistance}");

            }

            [HarmonyPrefix]
            [HarmonyPriority(Priority.VeryLow)]
            public static void Prefix2(GameObject template, ref int burden, ref float enginePower,ref float fuelCostPerDistance)
            {
                ///Apply Settings
                ///

                ModuleConfigManager.Instance.LoadCustomValuesForRocketModuleDefinition(template,ref burden,ref enginePower,ref   fuelCostPerDistance);
                SgtLogger.l($"New RocketModuleStats: {template.GetComponent<Building>().Def.PrefabID}, burden: {burden}, engine: {enginePower}, fuelPerHex: {fuelCostPerDistance}");

            }

        }
    }
}
