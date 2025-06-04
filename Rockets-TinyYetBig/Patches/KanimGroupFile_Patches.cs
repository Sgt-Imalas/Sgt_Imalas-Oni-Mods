using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    class KanimGroupFile_Patches
	{
		[HarmonyPatch(typeof(KAnimGroupFile), nameof(KAnimGroupFile.Load))]
		public class KAnimGroupFile_Load_Patch
		{
			private const string INTERACT_ANALYZER= "anim_interacts_temporal_tear_analyzer_mod_kanim";
			public static void Prefix(KAnimGroupFile __instance)
			{
				InjectionMethods.RegisterCustomInteractAnim(__instance, INTERACT_ANALYZER);
			}
		}
	}
}
