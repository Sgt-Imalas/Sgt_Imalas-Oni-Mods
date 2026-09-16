using BionicBoostersPlus.Content.Defs.Buildings;
using HarmonyLib;
using UtilLibs;

namespace BionicBoostersPlus.Patches
{
	internal class KAnimGroupFile_Patches
	{
		[HarmonyPatch(typeof(KAnimGroupFile), nameof(KAnimGroupFile.Load))]
		public class KAnimGroupFile_Load_Patch
		{
			public static void Prefix(KAnimGroupFile __instance)
			{
				InjectionMethods.RegisterCustomInteractAnim(__instance, Mk3BoosterMakerConfig.INTERACT_MK3_BOOSTERMAKER);
			}
		}
	}
}
