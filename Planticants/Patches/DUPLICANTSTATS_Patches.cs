using HarmonyLib;
using Planticants.Content.ModDb;
using TUNING;

namespace Planticants.Patches
{
    class DUPLICANTSTATS_Patches
    {

        [HarmonyPatch(typeof(DUPLICANTSTATS), nameof(DUPLICANTSTATS.GetStatsFor), [typeof(Tag)])]
        public class TargetType_TargetMethod_Patch
        {
            public static bool Prefix(Tag type, ref DUPLICANTSTATS __result)
            {
				bool isPlant = type == ModTags.PlantMinion;
                if (isPlant)
                {
					__result = Content.ModDb.PLANT_TUNING.PLANTMINIONSTATS;
				}

                return !isPlant;
			}
        }
		[HarmonyPatch(typeof(DUPLICANTSTATS), nameof(DUPLICANTSTATS.GetStatsFor), [typeof(KPrefabID)])]
		public class TargetType_TargetMethod_Patch2
		{
			public static bool Prefix(KPrefabID prefabID, ref DUPLICANTSTATS __result)
			{
				bool isPlant = prefabID.HasTag(ModTags.PlantMinion);
				if (isPlant)
				{
					__result = Content.ModDb.PLANT_TUNING.PLANTMINIONSTATS;
				}

				return !isPlant;
			}
		}
	}
}
