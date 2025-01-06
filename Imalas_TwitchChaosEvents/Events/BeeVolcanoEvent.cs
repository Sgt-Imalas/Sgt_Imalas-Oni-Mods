using Imalas_TwitchChaosEvents.BeeGeyser;
using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Util_TwitchIntegrationLib;
using UtilLibs;
using static Grid;

namespace Imalas_TwitchChaosEvents.Events
{
	internal class BeeVolcanoEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_BEEVolcano";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.BEEVOLCANO.NAME;

		public EventWeight EventWeight => EventWeight.WEIGHT_COMMON;


		List<CellOffset> beezerOffsets = new List<CellOffset>()
		{
			new(0,0),
			new(0,-1),
			new(1,-1),
			new(0,-2),
			new(1,-2),
			new(0,-3),
			new(1,-3),
		};
		bool ValidateBeezerOffset(int sourceCell)
		{
			foreach (var o in beezerOffsets)
			{
				int offsetCell = OffsetCell(sourceCell, o);

				if (!Grid.IsValidCell(offsetCell)
				|| Grid.Solid[offsetCell]
				|| Grid.Objects[offsetCell, (int)Grid.SceneLayer.Building] != null
					)
					return false;
			}
			return true;
		}


		private OccupyArea prefabOccupyArea;
		public Action<object> EventAction => (_) =>
		{
			prefabOccupyArea = Assets.GetPrefab(BeeGeyserConfig.ID).GetComponent<OccupyArea>();


			var worlds = new List<WorldContainer>(ClusterManager.Instance.WorldContainers)
				.Where((world) => world.IsDiscovered && !world.isModuleInterior)
				.OrderByDescending((world) => Components.LiveMinionIdentities.GetWorldItems(world.id).Count);

			WorldContainer targetWorld;

			if (worlds.Count() > 0)
			{
				targetWorld = worlds.First();
			}
			else
			{
				targetWorld = ClusterManager.Instance.activeWorld.IsModuleInterior ? ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorld.ParentWorldId) : ClusterManager.Instance.activeWorld;
			}


			SgtLogger.l("world found for beeser: " + targetWorld.name);

			var worlddupes = Components.LiveMinionIdentities.GetWorldItems(targetWorld.id);
			worlddupes.Shuffle();
			foreach (var minion in worlddupes)
			{
				var dupeCellUp = Grid.PosToCell(minion);
				while (!Grid.Solid[CellAbove(dupeCellUp)] && Grid.IsValidCell(CellAbove(dupeCellUp)))
				{
					dupeCellUp = CellAbove(dupeCellUp);
				}

				if (ValidateBeezerOffset(dupeCellUp))
				{
					SpawnBeeGeyserAt(Grid.OffsetCell(dupeCellUp, new(1, -3)));
					return;
				}
			}



			List<Room> rooms = new List<Room>();
			List<CavityInfo> backupCavities = new List<CavityInfo>();
			foreach (Room room in Game.Instance.roomProber.rooms)
			{
				var middle = Grid.PosToCell(room.cavity.GetCenter());

				if (!Grid.IsVisible(middle) || !Grid.IsValidCellInWorld(middle, targetWorld.id))
				{
					continue;
				}

				rooms.Add(room);
			}

			foreach (var RoomCavity in rooms)
			{
				SgtLogger.l("checking room: " + (RoomCavity.cavity.maxX - RoomCavity.cavity.minX) + "," + (RoomCavity.cavity.maxY - RoomCavity.cavity.minY));
				var cavity = RoomCavity.cavity;

				if (cavity.maxX - cavity.minX < 2)
				{
					SgtLogger.l("too thin");
					continue;
				}

				var height = cavity.maxY - cavity.minY;

				if (height < 4)
				{
					SgtLogger.l("too small");
					continue;
				}

				var cell = GetValidPlacementInCavity(cavity);

				if (cell == -1)
				{
					SgtLogger.l("no cell found");
					continue;
				}


				var go = GameUtil.KInstantiate(global::Assets.GetPrefab(BeeGeyserConfig.ID), CellToPos(cell), Grid.SceneLayer.Building);
				go.SetActive(true);

				ONITwitchLib.ToastManager.InstantiateToastWithGoTarget(
					STRINGS.CHAOSEVENTS.BEEVOLCANO.TOAST,
					STRINGS.CHAOSEVENTS.BEEVOLCANO.TOASTTEXT,
					go);


				return;
			}
			//if no room was found
			foreach (var cavity in Game.Instance.roomProber.cavityInfos)
			{
				var middle = Grid.PosToCell(cavity.GetCenter());

				if (!Grid.IsVisible(middle) || !Grid.IsValidCellInWorld(middle, targetWorld.id))
				{
					continue;
				}

				backupCavities.Add(cavity);
			}

			foreach (var cavity in backupCavities)
			{
				SgtLogger.l("checking cavity: " + cavity.room.GetProperName());

				if (cavity.maxX - cavity.minX < 2)
				{
					SgtLogger.l("too thin");
					continue;
				}

				var height = cavity.maxY - cavity.minY;

				if (height < 4)
				{
					SgtLogger.l("too small");
					continue;
				}

				var cell = GetValidPlacementInCavity(cavity);

				if (cell == -1)
				{
					SgtLogger.l("no cell found");
					continue;
				}
				SpawnBeeGeyserAt(cell);
				return;
			}


		};

		void SpawnBeeGeyserAt(int cell)
		{
			var go = GameUtil.KInstantiate(global::Assets.GetPrefab(BeeGeyserConfig.ID), CellToPos(cell), Grid.SceneLayer.Building);
			go.SetActive(true);

			ONITwitchLib.ToastManager.InstantiateToastWithGoTarget(
				STRINGS.CHAOSEVENTS.BEEVOLCANO.TOAST,
				STRINGS.CHAOSEVENTS.BEEVOLCANO.TOASTTEXT,
				go);
		}


		private int GetValidPlacementInCavity(CavityInfo cavity)
		{
			var minX = cavity.minX; // no need to check up against wall
			var maxX = cavity.maxX - 1;
			var minY = cavity.maxY - 3;
			var maxY = cavity.maxY - 3;

			for (var x = minX; x <= maxX; x++)
			{
				for (var y = maxY; y >= minY; y--)
				{
					var cell = Grid.XYToCell(x, y);
					if (prefabOccupyArea.TestArea(cell, null, (cell, _) => Grid.IsValidCell(cell) && !Grid.Solid[cell])
						&& prefabOccupyArea.CanOccupyArea(cell, ObjectLayer.Building))
					{
						return cell;
					}
				}
			}

			return -1;
		}

		public Func<object, bool> Condition => (_) => GameClock.Instance.GetCycle() > 50;

		public Danger EventDanger => Danger.High;
	}
}
