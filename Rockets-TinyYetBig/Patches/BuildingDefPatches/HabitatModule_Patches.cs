using HarmonyLib;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.RocketModulePatches
{
    /// <summary>
    /// Compact and properly centered habitat interior templates
    /// </summary>
    class HabitatModule_Patches
    {
        #region HabitatMedium
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
                foreach (var storage in go.GetComponents<Storage>())
                {
                    storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
                }
            }
        }


        /// <summary>
        /// Adding Power Plug to module part 1
        /// </summary>
        [HarmonyPatch(typeof(HabitatModuleMediumConfig))]
        [HarmonyPatch("CreateBuildingDef")]
        public static class AddPowerPlug1_HabitatMedium_Patch
        {
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix(BuildingDef __result)
            {
                if (Config.Instance.HabitatPowerPlug)
                    RocketryUtils.AddPowerPlugToModule(__result, ModAssets.PLUG_OFFSET_MEDIUM);
            }
        }

        /// <summary>
        /// Adding Power Plug to module part 2
        /// </summary>
        [HarmonyPatch(typeof(HabitatModuleMediumConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public static class AddPowerPlug2_HabitatMedium_Patch
        {

            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix(GameObject go)
            {
                if (Config.Instance.HabitatPowerPlug)
                {
                    WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
                    virtualNetworkLink.link1 = ModAssets.PLUG_OFFSET_MEDIUM;
                    virtualNetworkLink.visualizeOnly = true;
                }

            }
        }

        #endregion
        #region HabitatNoseconeSmall


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

                foreach (var storage in go.GetComponents<Storage>())
                {
                    storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
                }
            }
        }

        /// <summary>
        /// Adding Power Plug to module part 1
        /// </summary>
        [HarmonyPatch(typeof(HabitatModuleSmallConfig))]
        [HarmonyPatch("CreateBuildingDef")]
        public static class AddPowerPlug1_HabitatSmall_Patch
        {
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix(BuildingDef __result)
            {
                if (Config.Instance.HabitatPowerPlug)
                    RocketryUtils.AddPowerPlugToModule(__result, ModAssets.PLUG_OFFSET_SMALL);
            }
        }

        /// <summary>
        /// Adding Power Plug to module part 2
        /// </summary>
        [HarmonyPatch(typeof(HabitatModuleSmallConfig))]
        [HarmonyPatch("DoPostConfigureComplete")]
        public static class AddPowerPlug2_HabitatSmall_Patch
        {
            [HarmonyPrepare]
            public static bool Prepare() => Config.Instance.HabitatPowerPlug;
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix(GameObject go)
            {
                if (Config.Instance.HabitatPowerPlug)
                {
                    WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
                    virtualNetworkLink.link1 = ModAssets.PLUG_OFFSET_SMALL;
                    virtualNetworkLink.visualizeOnly = true;
                }
            }
        }
        #endregion
    }
}
