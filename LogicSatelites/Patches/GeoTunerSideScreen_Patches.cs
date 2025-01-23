using HarmonyLib;
using LogicSatellites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites.Patches
{
	internal class GeoTunerSideScreen_Patches
	{
		[HarmonyPatch(typeof(GeoTunerSideScreen), nameof(GeoTunerSideScreen.RefreshOptions))]
		public class RemoteWorkTerminalSidescreen_RefreshOptions_Patch
		{
			public static bool Prefix(GeoTunerSideScreen __instance)
			{
				if (DlcManager.IsExpansion1Active() == false)
				{
					return true;
				}

				int idx = 0;
				int num = idx + 1;
				__instance.SetRow(idx, global::STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.NOTHING, Assets.GetSprite((HashedString)"action_building_disabled"), null,true);

				int currentGeotunerWorld = __instance.targetGeotuner.GetMyWorldId();

				List<Geyser> availableTargetItems = new();

				var clusterManager = ClusterManager.Instance;
				var ownWorld = clusterManager.GetWorld(currentGeotunerWorld);
				var targetItems = Components.Geysers;

				foreach (var targetWorldId in targetItems.GetWorldsIds())
				{
					if (targetWorldId == currentGeotunerWorld)
					{
						availableTargetItems.AddRange(targetItems.GetItems(targetWorldId));
						continue;
					}
					var targetWorld = clusterManager.GetWorld(targetWorldId);
					bool HasSatelliteConnection = ModAssets.FindConnectionViaAdjacencyMatrix(ownWorld.GetMyWorldLocation(), targetWorld.GetMyWorldLocation(), out _, 1);
					if (HasSatelliteConnection)
					{
						availableTargetItems.AddRange(targetItems.GetItems(targetWorldId));
					}
				}

				foreach (Geyser geyser in availableTargetItems)
				{
					if (geyser.GetComponent<Studyable>().Studied)
						__instance.SetRow(num++, global::STRINGS.UI.StripLinkFormatting(geyser.GetProperName()), Def.GetUISprite(geyser.gameObject).first, geyser, true);
				}
				foreach (Geyser geyser in availableTargetItems)
				{
					if (!geyser.GetComponent<Studyable>().Studied && Grid.Visible[Grid.PosToCell(geyser)] > 0 && geyser.GetComponent<Uncoverable>().IsUncovered)
						__instance.SetRow(num++, global::STRINGS.UI.StripLinkFormatting(geyser.GetProperName()), Def.GetUISprite(geyser.gameObject).first, geyser, false);
				}

				for (int index = num; index < __instance.rowContainer.childCount; ++index)
					__instance.rowContainer.GetChild(index).gameObject.SetActive(false);

				return false;
			}
		}
	}
}
