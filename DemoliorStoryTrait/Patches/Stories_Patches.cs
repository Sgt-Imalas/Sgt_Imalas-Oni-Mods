using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
    class Stories_Patches
    {
		public static Story CGM_Impactor;
		public static readonly string CGM_Impactor_Path = "storytraits/CGM_Impactor";
		[HarmonyPatch(typeof(Stories), MethodType.Constructor, [typeof(ResourceSet)])]
		public class Stories_Constructor_Patch
		{
			public static void Postfix(Stories __instance)
			{
				CGM_Impactor = new Story(
				CGMWorldGenUtils.CGM_Impactor_StoryTrait,
				CGM_Impactor_Path,
				8);
				__instance.AddStoryMod(CGM_Impactor);
			}
		}
	}
}
