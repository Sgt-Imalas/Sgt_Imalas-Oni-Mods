using System;
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
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			pickupable = gameObject.GetComponent<Pickupable>();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.EntombedChanged, OnEntombedChanged);
			OnEntombedChanged(null);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.EntombedChanged, OnEntombedChanged);
			base.OnCleanUp();
		}

		void OnEntombedChanged(object _)
		{
			if(MigrateAwayFromNeutronium(out var newCell))
			{
				var position = this.transform.GetPosition();
				var newPosition = Grid.CellToPos(newCell);
				newPosition.z = position.z;
				newPosition.x += 0.5f;
				newPosition.y += 0.5f;
				transform.SetPosition(newPosition);
			}
		}

		bool InvalidCell(int cell)
		{
			return Grid.Element[cell].id == SimHashes.Unobtanium || !Grid.IsValidCellInWorld(cell, this.GetMyWorldId());
		}

		bool MigrateAwayFromNeutronium(out int newCell)
		{
			newCell = -1;
			if (!pickupable.IsEntombed)
				return false;

			int ownCell = Grid.PosToCell(this);
			newCell = FindValidNewCell(ownCell);
			return newCell != ownCell;
		}

		int FindValidNewCell(int startCell)
		{
			///iterate the adjacent cells, favoring those above.
			if (!InvalidCell(startCell))
				return startCell;

			var u = Grid.CellAbove(startCell);
			if (!InvalidCell(u))
				return u;

			var ul = Grid.CellUpLeft(startCell);
			if (!InvalidCell(ul))
				return ul;

			var ur = Grid.CellUpRight(startCell);
			if (!InvalidCell(ur))
				return ur;

			var l = Grid.CellLeft(startCell);
			if (!InvalidCell(l))
				return l;

			var r = Grid.CellRight(startCell);
			if (!InvalidCell(r))
				return r;

			var dr = Grid.CellDownRight(startCell);
			if (!InvalidCell(dr))
				return dr;

			var dl = Grid.CellDownLeft(startCell);
			if (!InvalidCell(dl))
				return dl;

			var d = Grid.CellBelow(startCell);
			if (!InvalidCell(d))
				return d;

			///fallback: check for valic cells in surrounding circles
			if (CheckCircleCells(2, out var rad2))
				return rad2;
			if (CheckCircleCells(3, out var rad3))
				return rad3;
			if (CheckCircleCells(4, out var rad4))
				return rad4;

			///this should never ever happen... consider this edge case to be stuck for good
			return startCell;
		}

		bool CheckCircleCells(int radius, out int foundCell)
		{
			foundCell = -1;
			foreach (var pos in ProcGen.Util.GetCircle(transform.position, radius))
			{
				int posCell = Grid.PosToCell(pos);
				if (!InvalidCell(posCell))
				{
					foundCell = posCell;
					break;
				}
			}
			return foundCell >= 0;
		}
	}
}
