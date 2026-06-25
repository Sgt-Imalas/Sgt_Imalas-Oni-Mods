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

			//List<SkillGroup> aquaticSpacerVariants = [];
			//foreach(var group in __instance.resources)
			//{
			//	aquaticSpacerVariants.Add(
			//		new SkillGroup("Aq_" + group.Id, group.choreGroupID, group.Name, group.choreGroupIcon, group.archetypeIcon) 
			//		{
			//			requiredChoreGroups = group.requiredChoreGroups,
			//			relevantAttributes = group.relevantAttributes,
			//			allowAsAptitude = false 
			//		}
			//	);
			//}

			//var index = __instance.resources.FindIndex(s => s.Id == __instance.SwimmingSkills.Id);
			//foreach (var spacer in aquaticSpacerVariants)
			//	__instance.resources.Insert(++index, spacer);

			AdaptationSkills = new SkillGroup(ADAPTATION_ID, STRINGS.DUPLICANTS.CHOREGROUPS.AQ_ADAPTATIONSKILLS.NAME, STRINGS.DUPLICANTS.CHOREGROUPS.AQ_ADAPTATIONSKILLS.NAME, "", "");
			AdaptationSkills.relevantAttributes = [];
			AdaptationSkills.requiredChoreGroups =[];
			AdaptationSkills.allowAsAptitude = false;
			//__instance.resources.Insert(++index, AdaptationSkills);

			var index = __instance.resources.FindIndex(s => s.Id == __instance.SwimmingSkills.Id);

			if (index == -1)
			{
				SgtLogger.warning("SwimmingSkills skillgroup not found");
				AdaptationSkills = __instance.Add(AdaptationSkills);
			}
			else
			{
				__instance.resources.Insert(index + 1, AdaptationSkills);
			}
		}
	}
}
