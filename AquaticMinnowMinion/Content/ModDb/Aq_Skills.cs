using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static AquaticMinnowMinion.ModAssets;
using static AquaticMinnowMinion.STRINGS.DUPLICANTS.ROLES;
using static AquaticMinnowMinion.STRINGS.UI.ROLES_SCREEN.PERKS;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Skills
	{
		public static Skill
			 Adaptation_EyeProtection //Double Eyelids
			, Adaptation_GillProtection //Mucus Glands
			, Adaptation_Insulation //Blubber/Fat Layer
			, Adaptation_WaterBreathingRateReduction //idk, sth about rebreathing/slowing down heartrate when diving
			, Adaptation_SlimySkin
			;

		public static void Register(Skills __instance)
		{

			///In case the main menu tries spawning an aquatic dupe with a skill hat, it needs at least one skill in that group.
			///as we are hackingly reusing the default minion skills, that crashes when theres no skill
			var dummy = new Skill("Adaptation_DummySkill",
				"DummySkill",
				"does nothing except preventing a main menu crash",
				1, "hat_role_suits1", "skillbadge_role_suits2",
				Aq_SkillGroups.ADAPTATION_ID,
				null,
				null,
				ModAssets.Tags.AquaticMinion.ToString()
				);
			dummy.deprecated = true;
			__instance.AddSkill(dummy);

			string requiredDuplicantModel = Tags.AquaticMinion.ToString();
			string[] dlc = [DlcManager.DLC5_ID];

			List<Skill> aq_skills = new List<Skill>();
			foreach (var original in __instance.resources)
			{
				if (original.deprecated)
					continue;

				//clone all default skills except the two swimming skills from aquatic, they are innate
				bool isDlc5Content = original.requiredDlcIds != null && original.requiredDlcIds.Contains(DlcManager.DLC5_ID);


				if (original.requiredDuplicantModel == GameTags.Minions.Models.Standard && !isDlc5Content)
				{
					aq_skills.Add(new Skill("Aq_" + original.Id, original.Name, original.description, original.tier, original.hat, original.badge, original.skillGroup, original.perks, original.priorSkills?.Select(s => "Aq_" + s).ToList(), Tags.AquaticMinion.ToString(), original.requiredDlcIds, original.forbiddenDlcIds));
				}
			}
			//foreach (var skill in aq_skills)
			//	__instance.AddSkill(skill);
						
			Adaptation_WaterBreathingRateReduction =
				__instance.AddSkill(new Skill("Adaptation_WaterBreathingRateReduction",
				ADAPTATION_WATERBREATHINGRATEREDUCTION.NAME,
				ADAPTATION_WATERBREATHINGRATEREDUCTION.TOOLTIP,
				0, "", "skillbadge_role_adaptation_gills",
				Aq_SkillGroups.ADAPTATION_ID,
				[Aq_SkillPerks.Adapt_WaterbreathingEfficiency, Db.Get().SkillPerks.IncreasedLungCapacity],
				null
				//, requiredDuplicantModel, dlc
				));

			Adaptation_EyeProtection =
				__instance.AddSkill(new Skill("Adaptation_EyeProtection",
				ADAPTATION_EYEPROTECTION.NAME,
				ADAPTATION_EYEPROTECTION.TOOLTIP,
				0, "", "skillbadge_role_adaptation_eye_protection",
				Aq_SkillGroups.ADAPTATION_ID,
				[Aq_SkillPerks.Adapt_EyeProtectionMinor, Aq_SkillPerks.Adapt_EyeProtectionMajor],
				null
				//, requiredDuplicantModel, dlc
				));

			Adaptation_Insulation =
				__instance.AddSkill(new Skill("Adaptation_Insulation",
				ADAPTATION_INSULATION.NAME,
				ADAPTATION_INSULATION.TOOLTIP,
				0, "", "skillbadge_role_adaptation_insulation",
				Aq_SkillGroups.ADAPTATION_ID,
				[Aq_SkillPerks.Adapt_ColdImmunity, Aq_SkillPerks.Adapt_FatLayer],
				null
				//, requiredDuplicantModel, dlc
				));

			Adaptation_GillProtection =
				__instance.AddSkill(new Skill("Adaptation_GillProtection",
				ADAPTATION_GILLPROTECTION.NAME,
				ADAPTATION_GILLPROTECTION.TOOLTIP,
				1, "", "skillbadge_role_adaptation_mucus",
				Aq_SkillGroups.ADAPTATION_ID,
				[Aq_SkillPerks.Adapt_SuitAirImmunity, Aq_SkillPerks.Adapt_ItchyGillsImmunity, Aq_SkillPerks.Adapt_GillMoisturizing, Aq_SkillPerks.Adapt_HeatImmunity],
				[Adaptation_WaterBreathingRateReduction.Id, Adaptation_EyeProtection.Id, Adaptation_Insulation.Id]
				//, requiredDuplicantModel, dlc
				));
		}
	}
}
