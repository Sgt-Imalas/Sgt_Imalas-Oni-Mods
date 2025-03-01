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
        }

        

    }
}
