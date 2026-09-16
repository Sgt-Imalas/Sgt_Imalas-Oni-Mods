using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;

namespace RonivansLegacy_ChemicalProcessing.Patches.HPA
{
	internal class SolidLogicValve_Patches
	{

        [HarmonyPatch(typeof(SolidLogicValve), nameof(SolidLogicValve.OnSpawn))]
        public class SolidLogicValve_OnSpawn_Patch
        {
            public static void Postfix(SolidLogicValve __instance)
			{
				HighPressureConduitRegistration.RegisterHighPressureConduit(__instance.gameObject, ObjectLayer.SolidConduitConnection);
			}
        }

        [HarmonyPatch(typeof(SolidLogicValve), nameof(SolidLogicValve.OnCleanUp))]
        public class SolidLogicValve_OnCleanUp_Patch
        {
            public static void Prefix(SolidLogicValve __instance)
			{
				HighPressureConduitRegistration.UnregisterHighPressureConduit(__instance.gameObject, ObjectLayer.SolidConduitConnection);
			}
        }
	}
}
