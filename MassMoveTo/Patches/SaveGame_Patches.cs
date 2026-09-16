using HarmonyLib;
using MassMoveTo.Tools.SweepByType;

namespace MassMoveTo.Patches
{
	internal class SaveGame_Patches
	{

        [HarmonyPatch(typeof(SaveGame), nameof(SaveGame.OnPrefabInit))]
        public class SaveGame_OnPrefabInit_Patch
        {
            public static void Postfix(SaveGame __instance)
			{
				__instance.gameObject.AddOrGet<SavedTypeSelections>();
			}
        }
	}
}
