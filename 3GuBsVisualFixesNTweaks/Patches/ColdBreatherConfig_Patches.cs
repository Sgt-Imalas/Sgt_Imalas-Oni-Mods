using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class ColdBreatherConfig_Patches
    {

        [HarmonyPatch(typeof(ColdBreatherConfig), nameof(ColdBreatherConfig.CreatePrefab))]
        public class ColdBreatherConfig_CreatePrefab_Patch
        {
            [HarmonyPrepare]
            public static bool Prepare()=> DlcManager.IsExpansion1Active();

            public static void Postfix(GameObject __result)
            {
                if(__result.TryGetComponent<KBatchedAnimController>(out var kbac))
                {
                    var anim = Assets.GetAnim("coldbreather_cluster_kanim");

                    if (anim == null)
                        return;

                    kbac.animFiles = [anim];
					SoundUtils.CopySoundsToAnim("coldbreather_cluster_kanim", "coldbreather_kanim"); //anim has no sound, cloning them from original wheezeworth anim
				}
            }
        }
    }
}
