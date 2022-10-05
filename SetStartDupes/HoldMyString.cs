using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes
{
    class HoldMyString:KMonoBehaviour
    {
        public string NAME => Group == null ? "" : Group.Name;
        [Serialize]
        public SkillGroup Group = null;

    }
}
