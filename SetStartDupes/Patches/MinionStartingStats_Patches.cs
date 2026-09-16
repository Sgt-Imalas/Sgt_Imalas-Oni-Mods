using HarmonyLib;
using UnityEngine;

namespace SetStartDupes.Patches
{
	internal class MinionStartingStats_Patches
	{

        [HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.Apply))]
        public class MinionStartingStats_Apply_Patch
        {
            public static void Postfix(MinionStartingStats __instance, GameObject go)
			{
				ModAssets.PostProcessMinion(go, __instance);
            }
        }
	}
}
