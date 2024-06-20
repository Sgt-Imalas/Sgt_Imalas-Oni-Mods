using BlueprintsV2.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintsV2.Patches
{
    internal class CleanupPatch
    {
        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        public static class GameDestroyInstances
        {
            public static void Postfix()
            {
                CreateBlueprintTool.DestroyInstance();
                UseBlueprintTool.DestroyInstance();
                SnapshotTool.DestroyInstance();
                MultiToolParameterMenu.DestroyInstance();

                ModAssets.BLUEPRINTS_AUTOFILE_WATCHER.Dispose();
            }
        }
    }
}
