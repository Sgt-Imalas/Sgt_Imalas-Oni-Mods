using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace Rockets_TinyYetBig.Patches
{
    class FixForAutoRocket
    {
        [HarmonyPatch(typeof(ElementLoader), "GetElement")]
        public static class FixAutoRocket
        {
            public static void Postfix(Tag tag, ref Element __result)
            {
                if(tag== GameTags.CombustibleLiquid && __result == default(Element))
                {
                    ElementLoader.elementTagTable.TryGetValue(SimHashes.Petroleum.CreateTag(), out __result);
                }
            }

        }
    }
}
