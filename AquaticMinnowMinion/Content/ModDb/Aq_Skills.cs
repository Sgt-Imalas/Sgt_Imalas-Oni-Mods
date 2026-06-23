using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Skills
	{

		public static void Register(Skills __instance)
		{
			List<Skill> aq_skills = new List<Skill>();
			foreach (var original in __instance.resources)
			{
				//clone all default skills except the two swimming skills from aquatic, they are innate
				bool isDlc5Content = original.requiredDlcIds != null && original.requiredDlcIds.Contains(DlcManager.DLC5_ID);

				if (original.requiredDuplicantModel == GameTags.Minions.Models.Standard && !isDlc5Content)
				{
					aq_skills.Add( new Skill("Aq_" + original.Id, original.Name, original.description, original.tier, original.hat, original.badge, original.skillGroup, original.perks, original.priorSkills?.Select(s => "Aq_" + s).ToList(), ModTags.AquaticMinion.ToString(), [DlcManager.DLC5_ID], null));
				}
			}
			foreach(var skill in aq_skills)
				__instance.AddSkill(skill);
		}
	}
}
