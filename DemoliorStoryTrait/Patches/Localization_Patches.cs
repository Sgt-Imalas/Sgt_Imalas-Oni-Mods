using HarmonyLib;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace DemoliorStoryTrait.Patches
{
    class Localization_Patches
	{
		public const string SettingNameKey = "STRINGS.CGM_DEMOLIOR_CONFIG_NAME";

		[HarmonyPatch(typeof(Localization), "Initialize")]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				string pipmolior_name;
				string pipmolior_uppercase_name;
				SgtLogger.l("LOC KEY: " + Localization.GetCurrentLanguageCode());
				switch (Localization.GetCurrentLanguageCode())
				{
					///by Flandre:
					case "zh":
					case "zh_klei":
						pipmolior_name = "鼠王星";
						pipmolior_uppercase_name = pipmolior_name;
						break;
					default:
						pipmolior_name = "Pipmolior";
						pipmolior_uppercase_name = "PIPMOLIOR";
						break;
				}


				if (Config.Instance.PipReplaceDemoliorSprite)
				{
					global::STRINGS.MISC.NOTIFICATIONS.INCOMINGPREHISTORICASTEROIDNOTIFICATION.NAME = pipmolior_uppercase_name;
					global::STRINGS.MISC.NOTIFICATIONS.LARGEIMPACTORREVEALSEQUENCE.RETICLE.LARGE_IMPACTOR_NAME = pipmolior_uppercase_name;
					global::STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.NAME = pipmolior_name;
				}
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_IMPACTOR.NAME", pipmolior_name);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_IMPACTOR.DESCRIPTION", global::STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.DESCRIPTION);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_IMPACTOR.DESCRIPTION_SHORT", global::STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS.LARGEIMACTOR.DESCRIPTION);
				Strings.Add(SettingNameKey, global::STRINGS.CREATURES.SPECIES.SQUIRREL.NAME+"-"+ pipmolior_name);

			}
		}
	}
}
