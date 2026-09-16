using AkisSnowThings.Content.Defs.Plants;
using HarmonyLib;
using System.Collections.Generic;

namespace AkisSnowThings.Patches
{
	internal class SubEntry_Patches
	{

        [HarmonyPatch(typeof(SubEntry), MethodType.Constructor, [typeof(string ), typeof(string), typeof(List<ContentContainer>), typeof(string)])]
        public class SubEntry_Constructor_Patch
		{
            public static void Prefix(ref string parentEntryID)
            {
                if (parentEntryID == EvergreenTreeConfig.ID)
                    parentEntryID = CodexCache.FormatLinkID(parentEntryID);
            }
        }
	}
}
