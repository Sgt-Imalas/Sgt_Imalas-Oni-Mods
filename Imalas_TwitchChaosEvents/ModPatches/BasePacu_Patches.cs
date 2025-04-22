using HarmonyLib;
using Imalas_TwitchChaosEvents.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
