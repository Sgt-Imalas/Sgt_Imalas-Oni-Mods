using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Patches
{
    class DUPLICANTSTATS_Patches
    {

        [HarmonyPatch(typeof(DUPLICANTSTATS), nameof(DUPLICANTSTATS.GetStatsFor), [typeof(Tag)])]
        public class TargetType_TargetMethod_Patch
        {
            public static bool Prefix(Tag type, ref DUPLICANTSTATS __result)
            {
				bool isPlant = type == Tags.AquaticMinion;
                if (isPlant)
                {
					__result = AQ_TUNING.AQUATICMINIONSTATS;
				}

                return !isPlant;
			}
        }
		[HarmonyPatch(typeof(DUPLICANTSTATS), nameof(DUPLICANTSTATS.GetStatsFor), [typeof(KPrefabID)])]
		public class TargetType_TargetMethod_Patch2
		{
			public static bool Prefix(KPrefabID prefabID, ref DUPLICANTSTATS __result)
			{
				bool isPlant = prefabID.HasTag(Tags.AquaticMinion);
				if (isPlant)
				{
					__result = AQ_TUNING.AQUATICMINIONSTATS;
				}

				return !isPlant;
			}
		}
	}
}
