using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
    public class FileHierarchyEntry:KMonoBehaviour
    {
        public enum HierarchyEntryType
        {
            none = 0,
            folder = 1,
            blueprint = 2,
            goUp = 3,
        }
        public HierarchyEntryType Type;
    }
}
