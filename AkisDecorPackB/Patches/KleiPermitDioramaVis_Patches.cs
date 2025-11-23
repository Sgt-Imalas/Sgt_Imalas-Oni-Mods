using AkisDecorPackB.Content.ModDb;
using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KleiItems;
using static TUNING.METEORS;

namespace AkisDecorPackB.Patches
{
	internal class KleiPermitDioramaVis_Patches
	{

        [HarmonyPatch(typeof(KleiPermitDioramaVis), nameof(KleiPermitDioramaVis.Init))]
        public class KleiPermitDioramaVis_Init_Patch
        {
            public static void Postfix(KleiPermitDioramaVis __instance)
			{
				ModPermitDioramas.Init(__instance);
			}
        }

        [HarmonyPatch(typeof(KleiPermitDioramaVis), nameof(KleiPermitDioramaVis.GetPermitVisTarget))]
        public class KleiPermitDioramaVis_GetPermitVisTarget_Patch
        {
            public static bool Prefix(KleiPermitDioramaVis __instance, PermitResource permit, ref IKleiPermitDioramaVisTarget __result)
			{
				if (permit == null || permit.Category != PermitCategory.Artwork)
					return true;

				var def = KleiPermitVisUtil.GetBuildingDef(permit);

				if (ModSkins.useMuseumDefs.Contains(def.PrefabID))
				{
					__result = ModPermitDioramas.museum;
					KleiPermitDioramaVis.lastRenderedPermit = permit;
					return false;
				}

				return true;
			}
        }
	}
}
