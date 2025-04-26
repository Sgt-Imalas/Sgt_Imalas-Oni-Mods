using ONITwitchLib.Utils;
using ProcGen;
using System;

namespace Util_TwitchIntegrationLib.Scripts
{
	public static class SpaceCheeseChecker
	{

		static bool IsSpaceCell(int cell)
		{
			if (!Grid.IsValidCell(cell))
				return false;

			bool isSpaceCell = World.Instance.zoneRenderData.worldZoneTypes[cell] == SubWorld.ZoneType.Space;
			bool notSolid = !Grid.IsSolidCell(cell) && !Grid.IsLiquid(cell);
			bool tileOrBackwall = Grid.Objects[cell, (int)ObjectLayer.Backwall] != null || Grid.Objects[cell, (int)ObjectLayer.FoundationTile] != null;

			return isSpaceCell && notSolid && !tileOrBackwall;
		}

		public static bool HasThereBeenAttemptedSpaceCheese(int sourceCell, out int targetCell, out string dupeName, int radius = 7, int checks = 6, bool instaFail = true, int thresholdFail = -1)
		{
			targetCell = sourceCell;

			Debug.Log($"World.Instance.zoneRenderData.worldZoneTypes[sourceCell]: ({Grid.CellToXY(sourceCell)}) " + World.Instance.zoneRenderData.worldZoneTypes[sourceCell]);

			if (IsSpaceCell(sourceCell))
			{
				targetCell = GetRandomLiveDupeCell(sourceCell, out dupeName);
				return true;
			}
			int fails = 0;

			for (int i = 0; i < checks; i++)
			{
				var randomCheckPos = PosUtil.ClampedMouseCellWithRange(radius);

				Debug.Log($"World.Instance.zoneRenderData.worldZoneTypes[randomCheckPos]: ({Grid.CellToXY(randomCheckPos)}) " + World.Instance.zoneRenderData.worldZoneTypes[randomCheckPos]);

				if (IsSpaceCell(randomCheckPos))
				{
					if (instaFail)
					{
						targetCell = GetRandomLiveDupeCell(sourceCell, out dupeName);
						return true;
					}
					else
					{
						fails++;
					}
				}
			}

			if (fails > thresholdFail && thresholdFail > 0)
			{
				targetCell = GetRandomLiveDupeCell(sourceCell, out dupeName);
				return true;
			}
			if (fails <= thresholdFail && fails > 0 && thresholdFail > 0)
			{
				var rando = new Random().Next(thresholdFail + 1);
				if (rando < fails)
				{
					targetCell = GetRandomLiveDupeCell(sourceCell, out dupeName);
					return true;
				}
			}


			dupeName = "";
			return false;
		}

		static int GetRandomLiveDupeCell(int fallback, out string dupeName)
		{
			if (Components.LiveMinionIdentities.Count > 0)
			{
				var randomDupe = Components.LiveMinionIdentities.GetRandom();
				dupeName = randomDupe.name;
				return Grid.PosToCell(randomDupe);

			}
			dupeName = "";
			return fallback;
		}
	}
}
