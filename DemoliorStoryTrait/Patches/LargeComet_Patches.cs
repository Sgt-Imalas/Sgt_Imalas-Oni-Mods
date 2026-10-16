using HarmonyLib;

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
