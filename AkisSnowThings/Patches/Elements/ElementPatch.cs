using AkisSnowThings.Content.Scripts.Elements;
using HarmonyLib;
using System.Collections.Generic;

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
