using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class KleiPermitDioramaVis_Patches
    {

        [HarmonyPatch(typeof(KleiPermitDioramaVis), nameof(KleiPermitDioramaVis.GetPermitVisTarget))]
        public class KleiPermitDioramaVis_GetPermitVisTarget_Patch
        {
            public static void Postfix(KleiPermitDioramaVis __instance, ref IKleiPermitDioramaVisTarget __result,PermitResource permit)
            {
                if (permit.Category != PermitCategory.Building)
                    return;

                if(__result == (IKleiPermitDioramaVisTarget)__instance.fallbackVis)
                {
                    var rule = KleiPermitVisUtil.GetBuildLocationRule(permit);
                    switch (rule)
                    {
                        case BuildLocationRule.Tile:
                        case BuildLocationRule.Anywhere:
							__result = __instance.buildingOnFloorVis;
							break;
                    }
                }
            }
        }
    }
}
