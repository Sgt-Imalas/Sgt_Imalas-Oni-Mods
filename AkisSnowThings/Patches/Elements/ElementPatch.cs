using AkisSnowThings.Content.Scripts.Elements;
using ElementUtilNamespace;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Patches.Elements
{
    internal class ElementPatch
    {
        /// <summary>
        /// Credit: akis beached 
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public class ElementLoader_Load_Patch
        {
            public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
            {
                var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
                SnowModElements.RegisterSubstances(list);
            }
            public static void Postfix(ElementLoader __instance)
            {
                SgtElementUtil.FixTags();
            }
        }

        // Credit: Heinermann (Blood mod)
        public static class EnumPatch
        {
            [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
            public class SimHashes_ToString_Patch
            {
                public static bool Prefix(ref Enum __instance, ref string __result)
                {
                    if (__instance is SimHashes hashes)
                    {
                        return !SgtElementUtil.SimHashNameLookup.TryGetValue(hashes, out __result);
                    }

                    return true;
                }
            }
        }

    }
}
