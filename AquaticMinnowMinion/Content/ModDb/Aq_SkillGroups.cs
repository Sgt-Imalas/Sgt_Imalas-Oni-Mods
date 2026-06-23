using Database;
using System;
using System.Collections.Generic;
using System.Text;
using TUNING;
using UtilLibs;
using static STRINGS.DUPLICANTS;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_SkillGroups
	{
		public static SkillGroup AdaptationSkills;
		public static string ADAPTATION_ID = "Aq_Skillgroup_Adaptation";

		public static void Register(SkillGroups __instance)
		{
			AdaptationSkills = new SkillGroup(ADAPTATION_ID, STRINGS.DUPLICANTS.CHOREGROUPS.AQ_ADAPTATIONSKILLS.NAME, STRINGS.DUPLICANTS.CHOREGROUPS.AQ_ADAPTATIONSKILLS.NAME, "", "");
			AdaptationSkills.relevantAttributes = [];
			AdaptationSkills.requiredChoreGroups =[];
			AdaptationSkills.allowAsAptitude = false;

			//AdaptationSkills = __instance.Add(AdaptationSkills);
			var index = __instance.resources.FindIndex(s => s.Id == __instance.SwimmingSkills.Id);

			if (index == -1)
			{
				SgtLogger.warning("SwimmingSkills skillgroup not found");
				AdaptationSkills = __instance.Add(AdaptationSkills);
			}
			else
			{
				__instance.resources.Insert(index+1, AdaptationSkills);
			}
		}
	}
}
