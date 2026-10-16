using HarmonyLib;
using SettingsSyncGroups.Scripts;

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
