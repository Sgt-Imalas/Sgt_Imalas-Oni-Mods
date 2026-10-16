using Database;

namespace BionicBoostersPlus.Content.ModDb
{
	internal class BB_Skills
	{
		public static Skill
			Bionics_C3_Watts, //decreased wattage, perma stress relief
			 Bionics_D3_Rads //RadiationResistance
			;

		public static void Register(Skills __instance)
		{
			__instance.BionicsC2.perks.Add(BB_SkillPerks.BB_CanCraftOverclocks);
			__instance.Engineering1.perks.Add(BB_SkillPerks.BB_CanCraftOverclocks);

			Bionics_C3_Watts =
				__instance.AddSkill(new Skill("Bionics_C3_Watts",
				STRINGS.DUPLICANTS.ROLES.BIONICS_C3_WATTS.NAME,
				STRINGS.DUPLICANTS.ROLES.BIONICS_C3_WATTS.TOOLTIP,
				2, "", "skillbadge_bionic_schematics3",
				Db.Get().SkillGroups.BionicSkills.Id,
				[BB_SkillPerks.BB_Circuits_WattageReduction, BB_SkillPerks.BB_Circuits_StressRelief],
				[__instance.BionicsC2.Id, __instance.BionicsD2.Id],
				 GameTags.Minions.Models.Bionic.Name,
				 [DlcManager.DLC3_ID]
				));
			Bionics_D3_Rads =
				__instance.AddSkill(new Skill("Bionics_D3_Rads",
				STRINGS.DUPLICANTS.ROLES.BIONICS_D3_RADS.NAME,
				STRINGS.DUPLICANTS.ROLES.BIONICS_D3_RADS.TOOLTIP,
				2, "", "skillbadge_bionic_hardware3",
				Db.Get().SkillGroups.BionicSkills.Id,
				DlcManager.IsExpansion1Active() ? [BB_SkillPerks.BB_Plating_RadResistance, BB_SkillPerks.BB_Plating_Health] : [BB_SkillPerks.BB_Plating_Health], //no rad resistence in base game
				[__instance.BionicsD2.Id],
				 GameTags.Minions.Models.Bionic.Name,
				 [DlcManager.DLC3_ID]
				));
		}
	}
}
