using HarmonyLib;
using ItemDropPrevention.Content.Scripts;
using UnityEngine;

namespace ItemDropPrevention.Patches
{
	internal class BaseRoverConfig_Patches
	{

        [HarmonyPatch(typeof(BaseRoverConfig), nameof(BaseRoverConfig.BaseRover))]
        public class BaseRoverConfig_BaseRover_Patch
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGet<DroppablesHolder>();
            }
        }
	}
}
