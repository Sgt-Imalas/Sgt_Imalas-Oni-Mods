using Database;
using HarmonyLib;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace MinnowStoryTrait.Patches
{
    class Stories_Patches
    {
		public static Story CGM_Minnow;
		[HarmonyPatch(typeof(Stories), MethodType.Constructor, [typeof(ResourceSet)])]
		public class Stories_Constructor_Patch
		{
			public static void Postfix(Stories __instance)
			{
				CGM_Minnow = new Story(
				CGMWorldGenUtils.CGM_Minnow_StoryTrait,
				"storytraits/CGM_MinnowTrait",
				9,
				-1,
				59,
				"poi/imperative/CGM_minnowPOI_A");
				__instance.AddStoryMod(CGM_Minnow);
			}
		}
	}
}
