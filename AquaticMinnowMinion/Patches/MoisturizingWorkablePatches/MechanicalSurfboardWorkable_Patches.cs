using HarmonyLib;

namespace AquaticMinnowMinion.Patches.MoisturizingWorkablePatches
{
	internal class MechanicalSurfboardWorkable_Patches
	{

        [HarmonyPatch(typeof(MechanicalSurfboardWorkable), nameof(MechanicalSurfboardWorkable.OnStartWork))]
        public class MechanicalSurfboardWorkable_OnStartWork_Patch
		{
			public static void Postfix(WorkerBase worker) => ModAssets.StartMoisturizing(worker);
		}
	}
}
