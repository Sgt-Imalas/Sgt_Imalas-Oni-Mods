using Database;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class GameplayEvents_Patches
    {

        [HarmonyPatch(typeof(GameplayEvents), MethodType.Constructor, [typeof(ResourceSet)])]
        public class GameplayEvents_Constructor_Patch
		{
            public static void Postfix(GameplayEvents __instance)
            {
                MeteorShowerAdjustments.AddModdedComets(__instance);
			}
        }
    }
}
