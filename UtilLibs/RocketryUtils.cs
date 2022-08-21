using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TUNING;

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
        public static Vector2I GetCustomInteriorSize(string templateString)
        {
            Regex getSize = new Regex(@"\(([0-9]*?)[,]([0-9]*?)\)");
            MatchCollection matches = getSize.Matches(templateString);
            if (matches.Count == 1)
            {
                Debug.Log(matches[0] +" "+ matches[0].Groups.Count.ToString() + " " + matches[0].Groups[0].Value + " " + matches[0].Groups[1].Value);
                if (matches[0].Groups.Count == 3)
                {
                    Debug.Log("reachedGroups");
                    var x = int.Parse(matches[0].Groups[1].Value);
                    var y = int.Parse(matches[0].Groups[2].Value); 
                    return new Vector2I(x, y);
                }
            }
            return ROCKETRY.ROCKET_INTERIOR_SIZE;
        }
    }
}
