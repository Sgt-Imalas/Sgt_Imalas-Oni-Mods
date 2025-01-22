using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UtilLibs;

namespace SetStartDupes.Patches
{
	internal class CrashMitigationPatches
	{
		//[HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.GenerateAttributes))]
		public class FixBionicCrash
		{
			//execute manually only when beached is not running because beached breaks patching this method by writing a static readonly array which for some reason breaks the game code of that method when patched
			public static void ExecutePatch(Harmony harmony)
			{
				SgtLogger.l("adding crash prevention fix for bionic dupes");
				var m_TargetMethod = AccessTools.Method("MinionStartingStats, Assembly-CSharp:GenerateAttributes");
				var m_Prefix = AccessTools.Method(typeof(FixBionicCrash), "Prefix");
				harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix));
			}
			/// <summary>
			/// forces bionic dupes to have 0 attribute bonus, as this crashes otherwise
			/// </summary>
			/// <param name="pointsDelta"></param>
			/// <param name="__instance"></param>
			//[HarmonyPriority(Priority.HigherThanNormal)]
			public static void Prefix(ref int pointsDelta, MinionStartingStats __instance)
			{
				if (__instance.personality.model == GameTags.Minions.Models.Bionic)
				{
					SgtLogger.l("Prevented a bionic dupe from crashing due to added bonus attribute points.");
					pointsDelta = 0;
				}
			}
		}
	}
}
