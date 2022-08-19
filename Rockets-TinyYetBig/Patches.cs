using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig
{
    class Patches
    {

        /// <summary>
        /// Extend rocket backwall to all wall buildings in rocket (glass, ports)
        /// </summary>
        [HarmonyPatch(typeof(WorldContainer))]
        [HarmonyPatch(nameof(WorldContainer.PlaceInteriorTemplate))]
        public static class RocketBackgroundGeneration_Patch
        {
            public static bool BackgroundExtension(string id, string rwtName)
            {
                return (id == rwtName 
                    || id == "RocketEnvelopeWindowTile"
                    || id == "RocketInteriorLiquidOutputPort"
                    || id == "RocketInteriorLiquidInputPort"
                    );
            }

            private static readonly MethodInfo BackgroundHelper = AccessTools.Method(
               typeof(RocketBackgroundGeneration_Patch),
               nameof(BackgroundExtension)
            );


            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(System.String),
                   "op_Equality"
               );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


                //InjectionMethods.PrintInstructions(code);
                if (insertionIndex != -1)
                {
                    //Debug.Log("FOUNDDDDDDDDDDD");
                    code[insertionIndex] = new CodeInstruction(OpCodes.Call, BackgroundHelper);
                }

                return code;
            }
        }


        [HarmonyPatch(typeof(WorldSelector), "OnPrefabInit")]
        public static class CustomSideScreenPatch_Gibinfo
        {
            public static void Postfix(WorldSelector __instance)
            {
                 // UIUtils.ListAllChildren(__instance.transform);
            }
        }

        /// <summary>
        /// Adjusts Critter storage to go with units instead of kg (would be 100kg/critter)
        /// </summary>
        [HarmonyPatch(typeof(Storage), "MassStored")]
        public static class KGToUnitsPatch
        {
            public static bool Prefix(Storage __instance, ref float __result)
            {
                if (__instance.storageFilters != null && __instance.storageFilters.Count > 0) {

                    if (__instance.storageFilters.All(CritterContainmentModuleConfig.GetCritterTags().Contains))
                    {
                        __result = __instance.UnitsStored();
                        return false;
                    }
                }
                return true;
            }
        }


        /// <summary>
        /// Adjust Scanner Module range
        /// </summary>
        [HarmonyPatch(typeof(ScannerModule.Instance), "Scan")]
        public static class BuffScannerModule
        {
            public static void Prefix(ScannerModule.Instance __instance)
            {
                __instance.def.scanRadius = Config.Instance.ScannerModuleRange;
            }
        }

        /// <summary>
        /// Add Custom Sidescreen for Nosecone
        /// </summary>
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_SatelliteCarrier
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                UIUtils.AddClonedSideScreen<NoseConeHEPHarvestSideScreen>("HarvestModuleHEPSideScreen", "HarvestModuleSideScreen", typeof(HarvestModuleSideScreen));
            }
        }

        /// <summary>
        /// add destination selection sidescreen to rad battery
        /// </summary>
        [HarmonyPatch(typeof(HighEnergyParticleDirectionSideScreen))]
        [HarmonyPatch(nameof(HighEnergyParticleDirectionSideScreen.IsValidForTarget))]
        public static class AddTargetValidityForRadBattery
        {
            public static bool Prefix(GameObject target, ref bool __result)
            {
                var targetComponent = target.GetComponent<RadiationBatteryOutputHandler>();
                //Debug.Log((target != null) + " ATLEAST ONCE TRUE");
                if(targetComponent != null)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Add new Buildings to Technologies
        /// </summary>
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                //add buildings to technology tree
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.SpaceProgram, HabitatModuleSmallExpandedConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.DurableLifeSupport, HabitatModuleMediumExpandedConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.ColonyDevelopment.CelestialDetection, HabitatModuleStargazerConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.RadiationTechnologies.RadboltContainment, HEPBatteryModuleConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Food.AnimalControl, CritterContainmentModuleConfig.ID); 
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.SolidMaterial.HighVelocityDestruction, NoseConeHEPHarvestConfig.ID); 
            }
        }

        /// <summary>
        /// Translation & String initialisation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        /// <summary>
        /// More than 16 Rockets allowed simultaniously
        /// </summary>
        [HarmonyPatch(typeof(ClusterManager))]
        [HarmonyPatch("OnPrefabInit")]
        public static class RocketCount_Patch
        {
            public static void Postfix()
            {
                ClusterManager.MAX_ROCKET_INTERIOR_COUNT = 100;
            }
        }

        /// <summary>
        /// Compact interior template for Medium Habitat
        /// </summary>
        [HarmonyPatch(typeof(HabitatModuleMediumConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class SaveSpace_HabitatMedium_Patch
        {
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/habitat_medium_compressed";
            }
        }
        /// <summary>
        /// Add launch_pst anim to normal modules
        /// </summary>
        [HarmonyPatch(typeof(RocketModuleCluster))]
        [HarmonyPatch("UpdateAnimations")]
        public static class AddPstLaunchAnim
        {
            public static bool Prefix(RocketModuleCluster __instance)
            {
                KBatchedAnimController component = __instance.GetComponent<KBatchedAnimController>();
                Clustercraft clustercraft = (UnityEngine.Object)__instance.CraftInterface == (UnityEngine.Object)null ? (Clustercraft)null : __instance.CraftInterface.GetComponent<Clustercraft>();
                if (clustercraft == null)
                    return true;
                if (clustercraft.Status == Clustercraft.CraftStatus.Landing && component.HasAnimation((HashedString)"launch") && component.HasAnimation((HashedString)"launch_pst"))
                {
                    component.ClearQueue();
                    component.initialAnim = "grounded";
                    if (component.HasAnimation((HashedString)"launch_pst"))
                        component.Play((HashedString)"launch_pst");
                    component.Queue((HashedString)"grounded", KAnim.PlayMode.Loop);
                    return false;
                }
                return true;

            }
        }

        /// <summary>
        /// Compact interior template for Small Habitat
        /// </summary>
        [HarmonyPatch(typeof(HabitatModuleSmallConfig))]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static class SaveSpace_HabitatSmall_Patch
        {
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "interiors/habitat_small_compressed";
            }
        }

        /// <summary>
        /// Adding Rocket buildings to build Screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleSmallExpandedConfig.ID, HabitatModuleSmallConfig.ID);
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleStargazerConfig.ID, NoseconeBasicConfig.ID);
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleMediumExpandedConfig.ID, HabitatModuleMediumConfig.ID);
                RocketryUtils.AddRocketModuleToBuildList(HEPBatteryModuleConfig.ID,BatteryModuleConfig.ID); 
                RocketryUtils.AddRocketModuleToBuildList(CritterContainmentModuleConfig.ID,GasCargoBayClusterConfig.ID); 
                RocketryUtils.AddRocketModuleToBuildList(NoseConeHEPHarvestConfig.ID, NoseconeHarvestConfig.ID); 
            }
        }

        /// <summary>
        /// Patch to decrease interior size from 32x32 to dynamic value per habitat template
        /// </summary>
        [HarmonyPatch(typeof(ClusterManager))]
        [HarmonyPatch("CreateRocketInteriorWorld")]
        public class ClusterManager_CreateRocketInteriorWorld_Patch
        {

            public static Vector2I ConditionForSize(Vector2I original, string templateString)
            {
                if (true) { 
                   switch (templateString)
                    {
                    case "interiors/habitat_medium_compressed":
                        original = new Vector2I(14, 13);
                        break;
                    case "interiors/habitat_small_compressed":
                        original = new Vector2I(10, 11);
                        break;
                    case "interiors/habitat_small_expanded":
                        original = new Vector2I(10, 13);
                        break;
                    case "interiors/habitat_medium_expanded":
                        original = new Vector2I(18, 17);
                        break;
                    case "interiors/habitat_medium_stargazer":
                         original = new Vector2I(14, 11);
                        break;
                    }
                }
                return original;
            }

            private static readonly MethodInfo InteriorSizeHelper = AccessTools.Method(
               typeof(ClusterManager_CreateRocketInteriorWorld_Patch),
               nameof(ConditionForSize)
            );


            private static readonly FieldInfo SizeFieldInfo = AccessTools.Field(
                typeof(TUNING.ROCKETRY),
                "ROCKET_INTERIOR_SIZE"
            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.operand is FieldInfo f && f == SizeFieldInfo);

                if (insertionIndex != -1)
                {
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldarg_2));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, InteriorSizeHelper));
                }

                return code;
            }
        }
    }
}
