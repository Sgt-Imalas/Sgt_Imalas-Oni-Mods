using Database;
using HarmonyLib;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Content.ModDb
{
	internal class ModSkillPerks
	{

		public static SkillPerk
			Can_ReconstructFossil,
			Can_ReconstructFossil_Ugly,
			Can_ReconstructFossil_Okay,
			Can_ReconstructFossil_Great;

		public static void Register(Db db)
		{
			var skillPerks = db.SkillPerks;
			Can_ReconstructFossil = skillPerks.Add(new SimpleSkillPerk(nameof(Can_ReconstructFossil), STRINGS.UI.ROLES_SCREEN.PERKS.CAN_RECONSTRUCTFOSSIL.DESCRIPTION));
			Can_ReconstructFossil_Ugly = skillPerks.Add(new SimpleSkillPerk(nameof(Can_ReconstructFossil_Ugly), STRINGS.UI.ROLES_SCREEN.PERKS.CAN_RECONSTRUCTFOSSIL_UGLY.DESCRIPTION));
			Can_ReconstructFossil_Okay = skillPerks.Add(new SimpleSkillPerk(nameof(Can_ReconstructFossil_Okay), STRINGS.UI.ROLES_SCREEN.PERKS.CAN_RECONSTRUCTFOSSIL_OKAY.DESCRIPTION));
			Can_ReconstructFossil_Great = skillPerks.Add(new SimpleSkillPerk(nameof(Can_ReconstructFossil_Great), STRINGS.UI.ROLES_SCREEN.PERKS.CAN_RECONSTRUCTFOSSIL_GREAT.DESCRIPTION));

			foreach (var skill in db.Skills.resources)
			{
				AddExtraPerks(skill.perks);
			}
		}
		public static IEnumerable<SkillPerk> AddExtraPerks(List<SkillPerk> activePerks)
		{
			var skillPerks = Db.Get().SkillPerks;

			if (activePerks == null || !activePerks.Any())
				return activePerks;
			if (activePerks.Contains(skillPerks.AllowAdvancedResearch))
			{
				activePerks.Add(Can_ReconstructFossil);
				activePerks.Add(Can_ReconstructFossil_Ugly);
			}
			if (activePerks.Contains(skillPerks.CanStudyWorldObjects))
			{
				activePerks.Add(Can_ReconstructFossil_Okay);
			}
			var greatPerkRef = DlcManager.IsPureVanilla() ? skillPerks.AllowInterstellarResearch : skillPerks.AllowNuclearResearch;
			if (activePerks.Contains(greatPerkRef))
			{
				activePerks.Add(Can_ReconstructFossil_Great);
			}
			return activePerks;
		}

	}
}
