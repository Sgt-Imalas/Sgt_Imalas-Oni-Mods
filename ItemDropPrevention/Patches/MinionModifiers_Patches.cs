using HarmonyLib;

namespace ItemDropPrevention.Patches
{
	internal class MinionModifiers_Patches
	{

        [HarmonyPatch(typeof(MinionModifiers), nameof(MinionModifiers.OnBeginChore))]
        public class MinionModifiers_OnBeginChore_Patch
        {
            public static bool Prefix(MinionModifiers __instance)
			{
				return ModAssets.MarkForLaterDroppage(__instance.gameObject);
			}
        }
	}
}
