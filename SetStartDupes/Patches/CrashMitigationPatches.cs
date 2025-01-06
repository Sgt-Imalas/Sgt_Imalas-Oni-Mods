using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.Patches
{
	internal class CrashMitigationPatches
	{
		[HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.GenerateAttributes))]
		public class FixBionicCrash
		{
			/// <summary>
			/// forces bionic dupes to have 0 attribute bonus, as this crashes otherwise
			/// </summary>
			/// <param name="pointsDelta"></param>
			/// <param name="__instance"></param>
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(ref int pointsDelta, MinionStartingStats __instance)
			{
				if(__instance.personality.model == GameTags.Minions.Models.Bionic)
				{
					pointsDelta = 0;
				}
			}
		}
	}
}
