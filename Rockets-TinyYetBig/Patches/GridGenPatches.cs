using HarmonyLib;
using System;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	class GridGenPatches
	{
		/// <summary>
		/// More free grid space at world gen
		/// </summary>
		[HarmonyPatch(typeof(BestFit))]
		[HarmonyPatch(nameof(BestFit.BestFitWorlds))]
		public static class IncreaseFreeGridSpace
		{
			public static void Postfix(ref Vector2I __result)
			{
				if (DlcManager.FeatureClusterSpaceEnabled())
				{
					__result.x += 312;
					__result.y = Math.Max(__result.y, 324);
					SgtLogger.debuglog("RocketryExpanded: Increased free grid space allocation");
				}
			}
		}
		//[HarmonyPatch(typeof(Cluster))]
		//[HarmonyPatch(nameof(Cluster.Save))]
		//public static class IncreaseFreeGridSpaceOnSaving
		//{
		//    public static void Postfix()
		//    {
		//        if (DlcManager.FeatureClusterSpaceEnabled())
		//        {
		//            BestFit.GetGridOffset(ClusterManager.Instance.WorldContainers, size, out offset);
		//        }
		//    }
		//}
		//[HarmonyPatch(typeof(Sim))]
		//[HarmonyPatch(nameof(Sim.Start))]
		//public static class IncreaseFreeGridSpaceOnSaving
		//{
		//    public static void Prefix()
		//    {
		//        GridSettings.Reset(Grid.WidthInCells, Grid.HeightInCells+1000);
		//        if (UnityEngine.Application.isPlaying)
		//            Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();

		//        Sim.AllocateCells(Grid.WidthInCells, Grid.HeightInCells);
		//    }
		//}
	}
}
