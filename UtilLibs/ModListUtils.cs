using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    internal class ModListUtils
    {
        public static bool ModIsActive(string modId)
        {
            Manager modManager = Global.Instance.modManager;
            foreach (var mod in modManager.mods)
            {
                if (!mod.IsEnabledForActiveDlc())
                    continue;
                if(mod.staticID== modId)
                    return true;
            }
            return false;
        }
    }
}
