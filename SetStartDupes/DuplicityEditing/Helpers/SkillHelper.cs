using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.DuplicityEditing.Helpers
{
    internal static class SkillHelper
    {
        public static List<Skill> GetAllSkills()
        {
            var skl = Db.Get().Skills;
            var skillList = new List<Skill>(skl.resources);
            skillList.RemoveAll(skill => skill.deprecated);

            return skillList;
        }
    }
}
