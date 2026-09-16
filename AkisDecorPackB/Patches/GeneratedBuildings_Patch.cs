using HarmonyLib;

namespace AkisDecorPackB.Patches
{
	internal class GeneratedBuildings_Patch
	{

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
			{
			}
		}
	}
}
