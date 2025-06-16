using HarmonyLib;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoliorStoryTrait.Patches
{
    class Localization_Patches
	{
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				if (Config.Instance.PipReplaceDemoliorSprite)
				{
					global::STRINGS.MISC.NOTIFICATIONS.INCOMINGPREHISTORICASTEROIDNOTIFICATION.NAME = "PIPMOLIOR";
					global::STRINGS.MISC.NOTIFICATIONS.LARGEIMPACTORREVEALSEQUENCE.RETICLE.LARGE_IMPACTOR_NAME = "PIPMOLIOR";
					global::STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.NAME = "Pipmolior";
				}
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_IMPACTOR.NAME", global::STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.NAME);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_IMPACTOR.DESCRIPTION", global::STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.DESCRIPTION);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_IMPACTOR.DESCRIPTION_SHORT", global::STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.DESCRIPTION);
				
			}
		}
	}
}
