using KSerialization;
using UnityEngine;
using UtilLibs;

namespace AkisDecorPackB.Content.Scripts.BigFossil
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class AnchorMonitor : KMonoBehaviour, IRenderEveryTick, ISim200ms
	{
		private const int MAX_CABLE_LENGTH = 16;

		[MyCmpReq] private Building building;
		[MyCmpGet] private Operational operational;
		[MyCmpReq] private KSelectable kSelectable;

		public CellOffset[] monitorCells = new CellOffset[7];
		public Extents[] hangingAreas;

		public int previousPosition;

		public bool isGrounded;
		public bool isCeilingInReach;

		[Serialize] public Grounding grounding;

		public static readonly Operational.Flag foundationFlag = new("decorpackb_requires_foundation", Operational.Flag.Type.Functional);

		private HandleVector<int>.Handle[] partitionerEntries;

		public float[] ceilingHeights = new float[7];
		private int position;

		public bool dirty;
		public bool dirtyFoundation;

		public enum Grounding
		{
			Floor,
			Hanging,
			Either
		}

		public void SetGrounding(Grounding grounding)
		{
			Debug.Log($"setting grounding to {grounding}");
			if (this.grounding == grounding)
				return;

			this.grounding = grounding;
			monitorCells = [];

			if (grounding == Grounding.Either)
				return;

			switch (grounding)
			{
				case Grounding.Floor:
					monitorCells = GetFoundationCells();
					SetFloorFoundation();
					break;
				case Grounding.Hanging:
					break;
			}
		}

		private void SetFloorFoundation()
		{
			Debug.Log("set floor foundation");
			if (partitionerEntries != null)
			{
				for (int i = 0; i < partitionerEntries.Length; i++)
				{
					if (partitionerEntries[i].IsValid())
						GameScenePartitioner.Instance.Free(ref partitionerEntries[i]);
				}
			}

			var def = building.Def;

			int x1 = -(def.WidthInCells - 1) / 2;
			int x2 = def.WidthInCells / 2;

			int cell = Grid.PosToCell(this);

			var left = Grid.OffsetCell(cell, x1, -1);
			var right = Grid.OffsetCell(cell, x2, -1);

			Grid.CellToXY(left, out var x, out var y);

			partitionerEntries =
			[
				GameScenePartitioner.Instance.Add(
					"DecorPackB.AnchorMonitor.Add",
					gameObject,
					x,
					y,
					7,
					1,
					GameScenePartitioner.Instance.solidChangedLayer,
					OnSolidChanged)
			];

			for (int i = 0; i < 7; i++)
			{
				var color = Color.Lerp(Color.red, Color.green, i / 7f);

				//FUtility.ModDebug.Square(FUtility.ModDebug.AddSimpleLineRenderer(transform, color, color), Grid.CellToPos(Grid.OffsetCell(cell, i, -1)), 1);
			}
		}

		private void OnSolidChanged(object obj)
		{
			Debug.Log("on solid changed");
			dirtyFoundation = true;
			SetDirty();
		}


		private CellOffset[] GetFoundationCells()
		{
			var width = building.Def.WidthInCells;
			var cells = new CellOffset[width];
			for (int i = 0; i < width; i++)
			{
				var x = i - (width / 2);
				var rotatedCellOffset = Rotatable.GetRotatedCellOffset(new CellOffset(x, -1), building.Orientation);
				cells[i] = rotatedCellOffset;
			}
			return cells;
		}
		public AnchorMonitor()
		{
			previousPosition = -1;
			grounding = Grounding.Either;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			position = Grid.PosToCell(this);
			Recheck();
			GroundingChanged();
		}
		private bool IsSecure(bool forceRecheck)
		{
			if (forceRecheck)
				Recheck();

			return grounding switch
			{
				Grounding.Floor => isGrounded,
				Grounding.Hanging => isCeilingInReach,
				_ => isGrounded || isCeilingInReach,
			};
		}

		private void UpdateStatusItem()
		{
			//kSelectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MissingFoundation, !IsSecure(false), this);
			kSelectable.ToggleStatusItem(new StatusItem("test", "Test", "", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID), !IsSecure(false), this);
		}

		private float GetCeilingDistance(int x)
		{
			Grid.PosToXY(transform.position, out var xo, out var yo);
			for (var y = building.Def.HeightInCells; y < MAX_CABLE_LENGTH; y++)
			{
				var cell = Grid.XYToCell(x + xo, y + yo);
				if (!Grid.IsValidCell(cell) || Grid.IsSolidCell(cell))
					return y;
			}

			return -1;
		}

		private bool IsGrounded()
		{
			return BuildingDef.CheckFoundation(
				Grid.PosToCell(this),
				building.Orientation,
				BuildLocationRule.OnFloor,
				building.Def.WidthInCells,
				building.Def.HeightInCells);
		}

		private bool IsCeilingInReach()
		{
			var inReach = true;
			for (int i = 0; i < building.Def.WidthInCells; i++)
			{
				ceilingHeights[i] = GetCeilingDistance(Mathf.FloorToInt(i - building.Def.WidthInCells / 2));
				if (ceilingHeights[i] < 0 || ceilingHeights[i] > MAX_CABLE_LENGTH)
					inReach = false;
			}

			return inReach;
		}

		public void Sim4000ms(float dt)
		{
			// Recheck();
		}


		public void Sim200ms(float dt)
		{
			if (dirty)
			{
				Recheck();
				dirty = false;
			}
		}

		public void SetDirty()
		{
			dirty = true;
		}

		public void Recheck()
		{
			int cell = Grid.PosToCell(this);
			if (cell != previousPosition || dirtyFoundation)
			{
				var hasGroundingChanged = false;

				var checkGround = grounding != Grounding.Hanging;
				var checkCeiling = grounding != Grounding.Floor;

				var grounded = checkGround && IsGrounded();
				var ceilingInReach = checkCeiling && IsCeilingInReach();

				if (grounded != isGrounded)
					hasGroundingChanged |= true;

				if (ceilingInReach != isCeilingInReach)
					hasGroundingChanged |= true;

				isGrounded = grounded;
				isCeilingInReach = ceilingInReach;

				if (hasGroundingChanged)
				{
					GroundingChanged();
					if (!isGrounded)
					{

					}
					//UpdatePartitionarEntries();
				}

				dirtyFoundation = false;
				previousPosition = cell;
			}
		}

		private void GroundingChanged()
		{
			var isSecure = IsSecure(false);
			if (operational != null)
				operational.SetFlag(foundationFlag, isSecure);
			BoxingTrigger(ModAssets.Hashes.GoundingChanged, isSecure);
			UpdateStatusItem();
		}

		/*		private void UpdatePartitionarEntries()
				{
					if (partitionerEntries != null)
					{
						for (int i = 0; i < partitionerEntries.Count; i++)
						{
							var partitionerEntry = partitionerEntries[i];
							GameScenePartitioner.Instance.Free(ref partitionerEntry);
						}
					}

					monitorCells = [];
					for (int i = 0; i < monitorCells.Length; i++)
					{
						var cell = Grid.OffsetCell(position, monitorCells[i]);
						if (Grid.IsValidCell(position) && Grid.IsValidCell(cell))
							partitionerEntries.Add(
								GameScenePartitioner.Instance.Add("FoundationOrAnchorMonitor.UpdateCells",
								gameObject,
								cell,
								GameScenePartitioner.Instance.solidChangedLayer,
								OnGroundChanged));

					}
				}
		*/
		private void OnGroundChanged(object obj)
		{
			Recheck();
		}

		public void RenderEveryTick(float dt)
		{
			Recheck();
		}
	}
}
