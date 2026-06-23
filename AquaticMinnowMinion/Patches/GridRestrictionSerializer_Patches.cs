using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                if (__instance.tagToId.Any(e => e.Key == ModTags.AquaticMinion))
                    return;

                int lowest = 0;
                foreach(var entry in  __instance.tagToId)
                    if(entry.Value < lowest)
                        lowest = entry.Value;

                lowest--;

                __instance.tagToId.Add(new KeyValuePair<Tag, int>(ModTags.AquaticMinion, lowest));
            }
        }
	}
}
