using Database;
using HarmonyLib;
using Klei.CustomSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace GeothermalStoryTrait.Patches
{
    class Stories_Patches
    {
		public static Story CGM_GeothermalPump;
		[HarmonyPatch(typeof(Stories), MethodType.Constructor, [typeof(ResourceSet)])]
		public class Stories_Constructor_Patch
		{
			public static void Postfix(Stories __instance)
			{
				CGM_GeothermalPump = new Story(
				CGMWorldGenUtils.CGM_Heatpump_StoryTrait,
				"storytraits/CGM_GeothermalHeatPump",
				7,
				-1,
				55,
				"dlc2::poi/geothermal/geothermal_controller");
				__instance.AddStoryMod(CGM_GeothermalPump);
			}
		}
	}
}
