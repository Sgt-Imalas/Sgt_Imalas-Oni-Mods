using AkisSnowThings.Content.Defs.Plants;
using HarmonyLib;

namespace AkisSnowThings.Patches
{
	internal class ImmigrationPatch
	{
		[HarmonyPatch(typeof(Immigration), nameof(Immigration.ConfigureCarePackages))]
		public class AdditionalCarePackages
		{
			public static void Postfix(Immigration __instance)
			{
				__instance.carePackages.Add(new CarePackageInfo(EvergreenTreeConfig.SEED_ID, 3f, null));

			}
		}
	}
}
