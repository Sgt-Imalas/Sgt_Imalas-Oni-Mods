using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public class RocketryUtils
    {
        public static void AddRocketModuleToBuildList(string moduleId, string placebehind = "")
        {
            if (!SelectModuleSideScreen.moduleButtonSortOrder.Contains(moduleId)) { 
            int i = -1;
            if (placebehind != "")
            {
                 i = SelectModuleSideScreen.moduleButtonSortOrder.IndexOf(placebehind);
            }
            int j = (i == -1) ? SelectModuleSideScreen.moduleButtonSortOrder.Count : ++i;
            SelectModuleSideScreen.moduleButtonSortOrder.Insert(j, moduleId);
        }
        }
    }
}
