using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace UndigYourself.Content.Scripts
{
	internal class NeutroniumMole : KMonoBehaviour
	{
		[MyCmpGet] Pickupable pickupable;
		[MyCmpGet] KBatchedAnimController kbac;
		[MyCmpGet] KPrefabID kpref;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			pickupable = gameObject.GetComponent<Pickupable>();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.EntombedChanged, OnEntombedChanged);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.EntombedChanged, OnEntombedChanged);
			base.OnCleanUp();
		}

		void OnEntombedChanged(object d)
		{
			if (ShouldMigrateAwayFromNeutronium(out var newCell))
			{
				var position = this.transform.GetPosition();
				var newPosition = Grid.CellToPos(newCell);
				newPosition.z = position.z;
				newPosition.x += 0.5f;
				newPosition.y += 0.5f;
				transform.SetPosition(newPosition);
			}
		}

		public static bool InvalidCell(int cell, int worldIdx)
		{
			if (cell <= 0)
				return true;

			return !Grid.IsValidCellInWorld(cell, worldIdx) || Grid.Element[cell].id == SimHashes.Unobtanium;
		}

		public static bool InvalidTargetCell(int cell, int worldIdx)
		{
			return InvalidCell(cell, worldIdx) || Grid.Objects[cell, (int)ObjectLayer.FoundationTile] != null;
		}

		bool ShouldMigrateAwayFromNeutronium(out int newCell)
		{
			newCell = -1;
			int ownCell = Grid.PosToCell(this);
			var world = Grid.WorldIdx[ownCell];

			if (!pickupable.IsEntombed || !InvalidCell(ownCell, world)) //stuck in the tile for all the items that go off the map, e.g. rocket contents or equippables
				return false;

			newCell = FindValidNewCell(ownCell, world);
			bool canMigrate = newCell != ownCell;
			SgtLogger.l(this.GetProperName() + "Found valid migration cell? " + canMigrate + " old cell: " + ownCell + ", new cell: " + newCell);
			return canMigrate;
		}

		public static int FindValidNewCell(int startCell, int worldIdx)
		{
			///iterate the adjacent cells, favoring those above.
			if (!InvalidTargetCell(startCell, worldIdx))
				return startCell;

			var u = Grid.CellAbove(startCell);
			if (!InvalidTargetCell(u, worldIdx))
				return u;

			var ul = Grid.CellUpLeft(startCell);
			if (!InvalidTargetCell(ul, worldIdx))
				return ul;

			var ur = Grid.CellUpRight(startCell);
			if (!InvalidTargetCell(ur, worldIdx))
				return ur;

			var l = Grid.CellLeft(startCell);
			if (!InvalidTargetCell(l, worldIdx))
				return l;

			var r = Grid.CellRight(startCell);
			if (!InvalidTargetCell(r, worldIdx))
				return r;

			var dr = Grid.CellDownRight(startCell);
			if (!InvalidTargetCell(dr, worldIdx))
				return dr;

			var dl = Grid.CellDownLeft(startCell);
			if (!InvalidTargetCell(dl, worldIdx))
				return dl;

			var d = Grid.CellBelow(startCell);
			if (!InvalidTargetCell(d, worldIdx))
				return d;

			///fallback: check for valic cells in surrounding circles
			if (CheckCircleCells(startCell, worldIdx, 2, out var rad2))
				return rad2;
			if (CheckCircleCells(startCell, worldIdx, 3, out var rad3))
				return rad3;
			if (CheckCircleCells(startCell, worldIdx, 4, out var rad4))
				return rad4;

			///this should never ever happen... consider this edge case to be stuck for good
			return startCell;
		}

		static bool CheckCircleCells(int centerCell, int worldIdx, int radius, out int foundCell)
		{
			foundCell = -1;
			foreach (var pos in ProcGen.Util.GetCircle(Grid.CellToPos(centerCell), radius))
			{
				int posCell = Grid.PosToCell(pos);
				if (!InvalidTargetCell(posCell, worldIdx))
				{
					foundCell = posCell;
					break;
				}
			}
			return foundCell >= 0;
		}
	}
}
