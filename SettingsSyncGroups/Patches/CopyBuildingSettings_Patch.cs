using HarmonyLib;
using SettingsSyncGroups.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SettingsSyncGroups.Patches
{
	internal class CopyBuildingSettings_Patch
	{

        [HarmonyPatch(typeof(CopyBuildingSettings), nameof(CopyBuildingSettings.OnPrefabInit))]
        public class CopyBuildingSettings_OnPrefabInit_Patch
        {
            public static void Postfix(CopyBuildingSettings __instance)
            {
                //SgtLogger.l("adding SyncGroupCarrier to " + __instance.GetProperName());
                __instance.gameObject.AddOrGet<SyncGroupCarrier>();
            }
        }
	}
}
