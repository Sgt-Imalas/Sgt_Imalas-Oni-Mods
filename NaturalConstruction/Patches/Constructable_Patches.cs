using HarmonyLib;
using NaturalConstruction.Content.Scripts;

namespace NaturalConstruction.Patches
{
	internal class Constructable_Patches
	{

        [HarmonyPatch(typeof(Constructable), nameof(Constructable.FinishConstruction))]
        public class Constructable_FinishConstruction_Patch
        {
            public static bool Prefix(Constructable __instance, WorkerBase workerForGameplayEvent)
            {
                if(__instance is ConstructableNaturalSpawner natTile)
                {
                    natTile.SpawnNaturalTile(workerForGameplayEvent);
					return false;
                }
                return true;
            }
        }
	}
}
