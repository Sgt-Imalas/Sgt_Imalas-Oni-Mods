using HarmonyLib;
using Imalas_TwitchChaosEvents.Attachments;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.ModPatches
{
    class BasePacu_Patches
    {

        [HarmonyPatch(typeof(BasePacuConfig), nameof(BasePacuConfig.CreatePrefab))]
        public class BasePacuConfig_CreatePrefab_Patch
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGet<Creature_Flipper>();
            }
        }
	}
}
