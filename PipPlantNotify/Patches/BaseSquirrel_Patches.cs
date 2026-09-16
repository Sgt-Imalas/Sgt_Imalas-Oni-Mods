using HarmonyLib;
using PipPlantNotify.Content.Scripts;
using UnityEngine;

namespace PipPlantNotify.Patches
{
	internal class BaseSquirrel_Patches
	{

        [HarmonyPatch(typeof(BaseSquirrelConfig), nameof(BaseSquirrelConfig.BaseSquirrel))]
        public class BaseSquirrelConfig_BaseSquirrel_Patch
        {
            public static void Postfix(GameObject __result, bool is_baby)
            {
                if (!is_baby)
                    __result.AddOrGet<PipNotificator>();
            }
        }
	}
}
