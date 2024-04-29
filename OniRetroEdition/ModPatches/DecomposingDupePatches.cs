using HarmonyLib;
using OniRetroEdition.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
    internal class DecomposingDupePatches
    {     
        [HarmonyPatch(typeof(MinionConfig),nameof(MinionConfig.CreatePrefab))]
        public static class AddRot
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddComponent<Retro_RottingMinion>();   
            }
        }
    }
}
