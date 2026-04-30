using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class Timelapser_Patches
	{

		[HarmonyPatch(typeof(Timelapser), nameof(Timelapser.OnNewDay))]
		public class Timelapser_OnNewDay_Patch
		{
			public static void Postfix(Timelapser __instance)
			{
				__instance.worldsToScreenshot.RemoveAll(world_id => SpaceStationManager.WorldIsSpaceStationInterior(world_id));
			}
		}
	}
}
