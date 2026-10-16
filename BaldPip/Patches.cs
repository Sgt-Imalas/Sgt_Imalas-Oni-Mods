using HarmonyLib;
using UnityEngine;

namespace BaldPip
{
	internal class Patches
	{

        [HarmonyPatch(typeof(BaseSquirrelConfig), nameof(BaseSquirrelConfig.BaseSquirrel))]
        public class BaseSquirrelConfig_BaseSquirrel_Patch
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGet<PipBarber>();
            }
        }
	}
}
