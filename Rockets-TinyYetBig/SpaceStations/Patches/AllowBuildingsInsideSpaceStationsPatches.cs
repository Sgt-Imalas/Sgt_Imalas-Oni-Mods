using HarmonyLib;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Patches
{
	public class AllowBuildingsInsideSpaceStationsPatches
	{
		[HarmonyPatch(typeof(LaunchPadConfig), "ConfigureBuildingTemplate")]
		public static class AllowLaunchpadInSpaceStation
		{
			public static void Postfix(GameObject go)
			{
				KPrefabID component = go.GetComponent<KPrefabID>();
				component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
			}
		}
		[HarmonyPatch(typeof(BaseModularLaunchpadPortConfig), "ConfigureBuildingTemplate")]
		public static class AllowPortLoadersInSpaceStation
		{
			public static void Postfix(GameObject go)
			{
				KPrefabID component = go.GetComponent<KPrefabID>();
				component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
			}
		}

		//TODO: manual patch to avoid breaking translations
		//[HarmonyPatch(typeof(ExobaseHeadquartersConfig), "ConfigureBuildingTemplate")]
		public static class AllowSmallPrintingPodInSpaceStation
		{
			public static void Postfix(GameObject go)
			{
				KPrefabID component = go.GetComponent<KPrefabID>();
				component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
			}
		}


		[HarmonyPatch(typeof(ClusterUtil), "ActiveWorldHasPrinter")]
		public static class CLusterUtil_AllowTelepads
		{
			public static bool Prefix(ref bool __result)
			{
				__result = SpaceStationManager.ActiveWorldIsRocketInterior() || Components.Telepads.GetWorldItems(ClusterManager.Instance.activeWorldId).Count > 0;
				return false;
			}
		}
	}
}
