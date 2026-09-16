using HarmonyLib;
using SettingsSyncGroups.UI;
using UtilLibs;

namespace SettingsSyncGroups.Patches
{
	public class DetailsScreenPatch
	{
		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class DetailsScreen_OnPrefabInit_Patch
		{
			public static void Postfix()
			{
				UIUtils.AddCustomSideScreen<SyncGroupCarrier_Sidescreen>("Building Settings Group Sidescreen", ModAssets.CurrentGroupSidescreenGO);
			}
		}
	}
}
