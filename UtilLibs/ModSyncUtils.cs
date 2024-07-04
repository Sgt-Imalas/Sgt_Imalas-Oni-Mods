using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public static class ModSyncUtils
    {
        public static bool IsModSyncMod(string defaultStaticModID)
        {
            return (defaultStaticModID == "SaveGameModLoader" || defaultStaticModID == "ModProfileManager_Addon");
        }
        public static bool IsModSyncMod(KMod.Mod mod) => IsModSyncMod(mod.staticID);
        
    }
}
