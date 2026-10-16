using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Patches
{
	internal class GridRestrictionSerializer_Patches
	{

        [HarmonyPatch(typeof(GridRestrictionSerializer), nameof(GridRestrictionSerializer.OnPrefabInit))]
        public class GridRestrictionSerializer_OnPrefabInit_Patch
        {
            public static void Postfix(GridRestrictionSerializer __instance)
            {
                if (__instance.tagToId.Any(e => e.Key == Tags.AquaticMinion))
                    return;

                int lowest = 0;
                foreach(var entry in  __instance.tagToId)
                    if(entry.Value < lowest)
                        lowest = entry.Value;

                lowest--;

                __instance.tagToId.Add(new KeyValuePair<Tag, int>(Tags.AquaticMinion, lowest));
            }
        }
	}
}
