using AkisSnowThings.Content.Scripts.Entities;
using HarmonyLib;

namespace AkisSnowThings.Patches.Plants
{
	internal class HarvestablePatch
	{
		[HarmonyPatch(typeof(HarvestDesignatable), nameof(HarvestDesignatable.SetHarvestWhenReady))]
		public class HarvestDesignatable_SetHarvestWhenReady_Patch
		{
			public static void Postfix(HarvestDesignatable __instance)
			{
				if (__instance is EvergreenHarvestDesignatable)
				{
					__instance.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest);
				}
			}
		}
	}
}
