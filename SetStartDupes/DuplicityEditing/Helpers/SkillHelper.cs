using Database;
using System.Collections.Generic;

namespace SetStartDupes.DuplicityEditing.Helpers
{
	internal static class SkillHelper
	{
		public static List<Skill> GetAllSkills(string duplicantModel = null)
		{
			var skl = Db.Get().Skills;
			var skillList = new List<Skill>(skl.resources);
			skillList.RemoveAll(skill => skill.deprecated);

			if(duplicantModel != null)
				skillList.RemoveAll(skill => skill.requiredDuplicantModel != duplicantModel);

			return skillList;
		}
	}
}
