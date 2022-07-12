using HarmonyLib;
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
        [HarmonyPatch(typeof(WorldSelector), "OnPrefabInit")]
        public static class CustomSideScreenPatch_Gibinfo
        {
            public static void Postfix(WorldSelector __instance)
            {
                UIUtils.ListAllChildren(__instance.transform);
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
                RocketryUtils.AddRocketModuleToBuildList(HabitatModuleMediumExpandedConfig.ID, HabitatModuleMediumConfig.ID);
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
                switch (templateString)
                {
                    case "interiors/habitat_medium_compressed":
                        original = new Vector2I(14, 12);
                        break;
                    case "interiors/habitat_small_compressed":
                        original = new Vector2I(10, 10);
                        break;
                    case "interiors/habitat_small_expanded":
                        original = new Vector2I(10, 12);
                        break;
                    case "interiors/habitat_medium_expanded":
                        original = new Vector2I(14, 16);
                        break;
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
