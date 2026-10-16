using HarmonyLib;

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
