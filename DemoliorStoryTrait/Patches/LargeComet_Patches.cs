using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoliorStoryTrait.Patches
{
    class LargeComet_Patches
    {

        [HarmonyPatch(typeof(LargeComet), nameof(LargeComet.InitializeMaterial))]
        public class LargeComet_InitializeMaterial_Patch
        {
            public static void Postfix(LargeComet __instance)
            {
                if(Config.Instance.PipReplaceDemoliorSprite)
                {
                    __instance.largeCometTexture = Assets.GetSprite("ImpactorPip");
                }
            }
        }
    }
}
