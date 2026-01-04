using AkisSnowThings.Content.Defs.Plants;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;

namespace AkisSnowThings.Patches
{
	internal class SubEntry_Patches
	{

        [HarmonyPatch(typeof(SubEntry), MethodType.Constructor, [typeof(string ), typeof(string), typeof(List < ContentContainer >), typeof(string)])]
        public class SubEntry_Constructor_Patch
		{
            public static void Prefix(ref string parentEntryID)
            {
                if(parentEntryID == EvergreenTreeConfig.ID)
                    parentEntryID = EvergreenTreeConfig.ID.Replace("_",string.Empty);
            }
        }
	}
}
