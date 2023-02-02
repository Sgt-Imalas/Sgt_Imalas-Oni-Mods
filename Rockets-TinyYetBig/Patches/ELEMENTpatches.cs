using HarmonyLib;
using ONITwitchLib.Utils;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static LegacyModMain;

namespace Rockets_TinyYetBig.Patches
{
    internal class ELEMENTpatches
    {
        //[HarmonyPatch(typeof(ElementLoader), "Load")]
        //public class ElementLoader_Load_Patch
        //{
        //    public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
        //    {
        //        // Add my new elements
        //        var list = substanceTablesByDlc[DlcManager.EXPANSION1_ID].GetList();
        //        var alloy = ElementInfo.Solid("ZincOre", ModAssets.Colors.zinc)
        //        Elements.RegisterSubstances(list);
        //    }

        //}


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
    }
}
