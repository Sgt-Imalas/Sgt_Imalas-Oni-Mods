using AkisSnowThings.Content.UI;
using HarmonyLib;
using UtilLibs;

namespace AkisSnowThings.Patches
{
	public class DetailsScreenPatch
	{
		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class DetailsScreen_OnPrefabInit_Patch
		{
			public static void Postfix()
			{
				UIUtils.AddCustomSideScreen<SnowMachineSideScreen>("Snoe machine Sidescreen", ModAssets.Prefabs.snowmachineSidescreenPrefab);
			}
		}
	}
}
