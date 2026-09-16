using HarmonyLib;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
	internal class SimCellOccupier_Patches
	{

        [HarmonyPatch(typeof(SimCellOccupier), nameof(SimCellOccupier.OnCleanUp))]
        public class SimCellOccupier_OnCleanUp_Patch
        {
            [HarmonyPriority(Priority.HigherThanNormal)]
            public static void Prefix(SimCellOccupier __instance)
            {
                if (!__instance.TryGetComponent<TrueTiles_OverrideStorage>(out var storage))
                    return;
                storage.ClearAll();
            }
        }
	}
}
