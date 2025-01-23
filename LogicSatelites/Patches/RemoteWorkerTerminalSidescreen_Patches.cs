using HarmonyLib;
using LogicSatellites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites.Patches
{
	internal class RemoteWorkerTerminalSidescreen_Patches
	{
		[HarmonyPatch(typeof(RemoteWorkTerminalSidescreen), nameof(RemoteWorkTerminalSidescreen.RefreshOptions))]
		public class RemoteWorkTerminalSidescreen_RefreshOptions_Patch
		{
			public static bool Prefix(RemoteWorkTerminalSidescreen __instance)
			{
				if(DlcManager.IsExpansion1Active() == false)
				{
					return true;
				}

				int idx = 0;
				int num = idx + 1;
				__instance.SetRow(idx, global::STRINGS.UI.UISIDESCREENS.REMOTE_WORK_TERMINAL_SIDE_SCREEN.NOTHING_SELECTED, Assets.GetSprite((HashedString)"action_building_disabled"), null);

				int currentTerminalWorld = __instance.targetTerminal.GetMyWorldId();
				
				List<RemoteWorkerDock> availableWorldTerminals = new();

				var clusterManager = ClusterManager.Instance;
				var ownWorld = clusterManager.GetWorld(currentTerminalWorld);

				foreach (var targetWorldId in Components.RemoteWorkerDocks.GetWorldsIds())
				{
					if (targetWorldId == currentTerminalWorld)
					{
						availableWorldTerminals.AddRange(Components.RemoteWorkerDocks.GetItems(targetWorldId));
						continue;
					}	
					var targetWorld = clusterManager.GetWorld(targetWorldId);
					bool HasSatelliteConnection = ModAssets.FindConnectionViaAdjacencyMatrix(ownWorld.GetMyWorldLocation(),targetWorld.GetMyWorldLocation(), out _, 1);
					if (HasSatelliteConnection)
					{
						availableWorldTerminals.AddRange(Components.RemoteWorkerDocks.GetItems(targetWorldId));
					}
				}

				foreach (RemoteWorkerDock remoteWorkerDock in availableWorldTerminals)
				{
					__instance.SetRow(num++, global::STRINGS.UI.StripLinkFormatting(remoteWorkerDock.GetProperName()), Def.GetUISprite(remoteWorkerDock.gameObject)?.first, remoteWorkerDock);
				}
				for (int index = num; index < __instance.rowContainer.childCount; ++index)
					__instance.rowContainer.GetChild(index).gameObject.SetActive(false);

				return false;
			}
		}
	}
}
