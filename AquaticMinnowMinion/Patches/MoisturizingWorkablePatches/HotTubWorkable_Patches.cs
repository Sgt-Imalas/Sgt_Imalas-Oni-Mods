using HarmonyLib;

namespace AquaticMinnowMinion.Patches.MoisturizingWorkablePatches
{
	internal class HotTubWorkable_Patches
	{

        [HarmonyPatch(typeof(HotTubWorkable), nameof(HotTubWorkable.OnStartWork))]
        public class HotTubWorkable_OnStartWork_Patch
        {
            public static void Postfix(WorkerBase worker) => ModAssets.StartMoisturizing(worker);
		}
	}
}
