using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class TagManager_Patches
    {

        [HarmonyPatch(typeof(TagManager), nameof(TagManager.GetProperName))]
        public class TagManager_GetProperName_Patch
        {
            public static void Postfix(Tag tag, ref string __result)
            {
                if(__result.Contains("MISSING"))
				{
					//if the tag is missing a proper name, use the tag name instead
					__result = tag.Name;
				}
			}
        }
    }
}
