using AkisDecorPackB.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Scripts
{
	public class AutoRotateToNearestWall : KMonoBehaviour
	{
		[MyCmpReq] public Rotatable rotatable;
		private int lastCell = -1;

		public override void OnSpawn()
		{
			lastCell = -1;
			CheckRotation();
		}

		private void Update()
		{
			if (BuildTool.Instance.visualizer == null)
				return;

			CheckRotation();
		}

		private void CheckRotation()
		{
			if (this.NaturalBuildingCell() == lastCell)
				return;

			var below = rotatable.GetRotatedCellOffset(CellOffset.down);
			var cell = Grid.OffsetCell(this.NaturalBuildingCell(), below);

			if (!IsCellSolid(cell))
			{
				TryRotating();
			}

			lastCell = this.NaturalBuildingCell();
		}

		private bool TryRotating()
		{
			for (int i = 0; i < 3; i++)
			{
				KFMOD_Patches.DoMuted(BuildTool.Instance.TryRotate);

				var below = rotatable.GetRotatedCellOffset(CellOffset.down);
				var cell = Grid.OffsetCell(this.NaturalBuildingCell(), below);

				if (IsCellSolid(cell))
					return true;
			}

			return false;
		}

		private static bool IsCellSolid(int cell)
		{
			GameObject gameObject = Grid.Objects[cell, 1];

			bool isFoundationInPlan = false;
			if (gameObject != null)
			{
				var building = gameObject.GetComponent<BuildingUnderConstruction>();
				if (building != null && building.Def.IsFoundation)
				{
					isFoundationInPlan = true;
				}
			}

			return Grid.IsValidBuildingCell(cell) && (Grid.Solid[cell] || isFoundationInPlan);
		}
	}
}
