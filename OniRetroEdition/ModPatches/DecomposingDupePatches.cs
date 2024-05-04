using HarmonyLib;
using OniRetroEdition.Behaviors;
using OniRetroEdition.Foods;
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
        [HarmonyPatch(typeof(Grave), nameof(Grave.OnCleanUp))]
        public static class SpawnSkeletonOnDeconstruct
        {
            public static void Prefix(Grave __instance)
            {
               if(__instance.graveName != null && __instance.graveName.Length > 0)
                {
                    var go = Util.KInstantiate(Assets.GetPrefab(BonesConfig.ID));

                    go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(__instance), Grid.SceneLayer.Ore));
                    go.SetActive(true);
                }
            }
        }
    }
}
