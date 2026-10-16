using HarmonyLib;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
    class BuildingPatch
    {
        /// <summary>
        /// sets the visualisation element to an override element if applicable
        /// </summary>
        [HarmonyPatch(typeof(Building), nameof(Building.GetVisualizationElementID))]
        public class Building_GetVisualizationElementID_Patch
        {
            public static void Postfix(Building __instance, ref SimHashes __result)
            {
                int cell = Grid.PosToCell(__instance);
                if (__result == SimHashes.Void)
                    return;
                if(TrueTiles_OverrideStorage.TryGetElement(cell, __instance.Def, out var overrideId))
                {
					__result = overrideId;
				}
            }
        }
    }
}
