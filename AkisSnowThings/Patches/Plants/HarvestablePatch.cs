using AkisSnowThings.Content.Scripts.Entities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
