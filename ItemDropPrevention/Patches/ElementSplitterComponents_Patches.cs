using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemDropPrevention.Patches
{
	internal class ElementSplitterComponents_Patches
	{

        [HarmonyPatch(typeof(ElementSplitterComponents), nameof(ElementSplitterComponents.CanFirstAbsorbSecond))]
        public class ElementSplitterComponents_CanFirstAbsorbSecond_Patch
        {
            public static void Postfix(HandleVector<int>.Handle first,HandleVector<int>.Handle second, ref bool __result)
            {
                if (!__result)
                    return;

				if (first == HandleVector<int>.InvalidHandle || second == HandleVector<int>.InvalidHandle)
					return;

				ElementSplitter data1 = GameComps.ElementSplitters.GetData(first);
				ElementSplitter data2 = GameComps.ElementSplitters.GetData(second);

				if(data1.kPrefabID.HasTag(ModAssets.BlockedFromDoingStuff) || data2.kPrefabID.HasTag(ModAssets.BlockedFromDoingStuff))
				{
					__result = false;
				}
			}
        }
	}
}
