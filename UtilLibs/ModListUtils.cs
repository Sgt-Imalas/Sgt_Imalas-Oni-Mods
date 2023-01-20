using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public class ModListUtils
    {
        public static bool ModIsActive(string modId)
        {
            Manager modManager = Global.Instance.modManager;
            foreach (var mod in modManager.mods)
            {
                if (!mod.IsEnabledForActiveDlc())
                    continue;
                if(mod.staticID== modId || mod.staticID.Contains(modId) || mod.staticID == WorkshopModIDs[modId])
                    return true;
            }
            return false;
        }
        public static Dictionary<string, string> WorkshopModIDs = new Dictionary<string, string>()
        {
            { "TrueTiles","2815406414" },
            { "Amorbus","2899109675" },
            
        };
    }
}
