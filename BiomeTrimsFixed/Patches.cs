using HarmonyLib;

namespace BiomeTrimsFixed
{
    internal class Patches
    {
        [HarmonyPatch(typeof(GroundRenderer.WorldChunk))]
        [HarmonyPatch(nameof(GroundRenderer.WorldChunk.GetBiomeIdx))]
        public class DB_Init
        {
            public static void Postfix(GroundRenderer __instance, int cell, ref int __result)
            {
                if (World.Instance != null && World.Instance.zoneRenderData != null && __result == 3)
                {
                    __result = (int)World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
                }
            }
        }        
    }
}
