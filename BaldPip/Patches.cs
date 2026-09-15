using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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
