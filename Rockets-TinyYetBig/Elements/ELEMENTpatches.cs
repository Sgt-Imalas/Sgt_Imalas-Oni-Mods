using ElementUtilNamespace;
using HarmonyLib;
using ONITwitchLib.Utils;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Elements
{

    internal class ELEMENTpatches
    {



        /// <summary>
        /// akis beached 
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public class ElementLoader_Load_Patch
        {
            public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
            {
                // Add my new elements
                var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
                ModElements.RegisterSubstances(list);

                //SgtLogger.l("ElementList length after that method; " + substanceTablesByDlc[DlcManager.VANILLA_ID].GetList().Count);
                //SgtLogger.l("ElementList SO length; " + substanceTablesByDlc[DlcManager.EXPANSION1_ID].GetList().Count);
            }
            public static void Postfix(ElementLoader __instance)
            {
                //SgtLogger.l("ElementList length in postfix; " + ElementLoader.elementTable.Count);
                SgtElementUtil.FixTags();
            }
        }


        [HarmonyPatch(typeof(ElementLoader), "CollectElementsFromYAML")]
        public class MakeNeutroniumFullyRadBlocking_Patch
        {
            public static void Postfix(ref List<ElementLoader.ElementEntry> __result)
            {
                foreach (var elem in __result)
                {
                    if (elem.elementId == SimHashes.Unobtanium.ToString())
                    {
                        elem.radiationAbsorptionFactor= 1.26f;
                    }
                }
            }
        }

        // Credit: Heinermann (Blood mod)
        public static class EnumPatch
        {
            [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
            private class SimHashes_ToString_Patch
            {
                private static bool Prefix(ref Enum __instance, ref string __result)
                {
                    if (__instance is SimHashes hashes)
                    {
                        return !SgtElementUtil.SimHashNameLookup.TryGetValue(hashes, out __result);
                    }

                    return true;
                }
            }

            [HarmonyPatch(typeof(Enum), nameof(Enum.Parse), new Type[] { typeof(Type), typeof(string), typeof(bool) })]
            private class SimHashes_Parse_Patch
            {
                private static bool Prefix(Type enumType, string value, ref object __result)
                {
                    if (enumType.Equals(typeof(SimHashes)))
                    {
                        return !SgtElementUtil.ReverseSimHashNameLookup.TryGetValue(value, out __result);
                    }

                    return true;
                }
            }
        }
    }
}
