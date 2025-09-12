using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ForceFieldWallTile.Content.Scripts
{
	/// <summary>
	/// inverted AnimTileable
	/// </summary>
	internal class ConnectorTileable : KMonoBehaviour
	{

		[MyCmpGet] Rotatable rotatable;

		private HandleVector<int>.Handle partitionerEntry;
		public ObjectLayer objectLayer = ObjectLayer.Building;
		public Tag[] tags;
		private Extents extents;
		private static readonly KAnimHashedString[] leftSymbols =
		[
	new KAnimHashedString("joint_left"),
	new KAnimHashedString("joint_left_fg"),
	new KAnimHashedString("joint_left_place")
		];
		private static readonly KAnimHashedString[] rightSymbols =
		[
	new KAnimHashedString("joint_right"),
	new KAnimHashedString("joint_right_fg"),
	new KAnimHashedString("joint_right_place")
		];
		private static readonly KAnimHashedString[] topSymbols =
		[
	new KAnimHashedString("joint_top"),
	new KAnimHashedString("joint_top_fg"),
	new KAnimHashedString("joint_top_place")
		];
		private static readonly KAnimHashedString[] bottomSymbols =
		[
	new KAnimHashedString("joint_bottom"),
	new KAnimHashedString("joint_bottom_fg"),
	new KAnimHashedString("joint_bottom_place")
		];

		ForceFieldTile fft;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			if (this.tags != null && this.tags.Length != 0)
				return;

			this.tags =[this.GetComponent<KPrefabID>().PrefabTag];
		}

		public override void OnSpawn()
		{
			fft = GetComponent<ForceFieldTile>();
			OccupyArea component = this.GetComponent<OccupyArea>();
			this.extents = !(component != null) ? this.GetComponent<Building>().GetExtents() : component.GetExtents();
			this.partitionerEntry = GameScenePartitioner.Instance.Add("ConnectorTileable.OnSpawn", this.gameObject, new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2), GameScenePartitioner.Instance.objectLayers[(int)this.objectLayer], new System.Action<object>(this.OnNeighbourCellsUpdated));
			this.UpdateEndCaps();
		}

		public override void OnCleanUp()
		{
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
			base.OnCleanUp();
		}

		CellOffset GetForcefieldFlipOffset(CellOffset original)
		{
			if (fft == null || !fft.IsAltVariant)
				return original;

			return new CellOffset(-original.x, original.y);
		}

		private void UpdateEndCaps()
		{
			int cell = Grid.PosToCell(this);
			bool is_visible1 = false;
			bool is_visible2 = false;
			bool is_visible3 = false;
			bool is_visible4 = false;
			int x;
			int y;
			Grid.CellToXY(cell, out x, out y);
			CellOffset offset1 = new CellOffset(this.extents.x - x - 1, 0);
			CellOffset offset2 = new CellOffset(this.extents.x - x + this.extents.width, 0);
			CellOffset offset3 = new CellOffset(0, this.extents.y - y + this.extents.height);
			CellOffset offset4 = new CellOffset(0, this.extents.y - y - 1);
			if (rotatable != null)
			{
				offset1 = rotatable.GetRotatedCellOffset(offset1);
				offset2 = rotatable.GetRotatedCellOffset(offset2);
				offset3 = rotatable.GetRotatedCellOffset(offset3);
				offset4 = rotatable.GetRotatedCellOffset(offset4);
			}
			offset1 = GetForcefieldFlipOffset(offset1);
			offset2 = GetForcefieldFlipOffset(offset2);
			offset3 = GetForcefieldFlipOffset(offset3);
			offset4 = GetForcefieldFlipOffset(offset4);

			int num1 = Grid.OffsetCell(cell, offset1);
			int num2 = Grid.OffsetCell(cell, offset2);
			int num3 = Grid.OffsetCell(cell, offset3);
			int num4 = Grid.OffsetCell(cell, offset4);
			if (Grid.IsValidCell(num1))
				is_visible1 = this.HasTileableNeighbour(num1);
			if (Grid.IsValidCell(num2))
				is_visible2 = this.HasTileableNeighbour(num2);
			if (Grid.IsValidCell(num3))
				is_visible3 = this.HasTileableNeighbour(num3);
			if (Grid.IsValidCell(num4))
				is_visible4 = this.HasTileableNeighbour(num4);
			foreach (KBatchedAnimController componentsInChild in this.GetComponentsInChildren<KBatchedAnimController>())
			{
				foreach (KAnimHashedString leftSymbol in leftSymbols)
					componentsInChild.SetSymbolVisiblity(leftSymbol, is_visible1);
				foreach (KAnimHashedString rightSymbol in rightSymbols)
					componentsInChild.SetSymbolVisiblity(rightSymbol, is_visible2);
				foreach (KAnimHashedString topSymbol in topSymbols)
					componentsInChild.SetSymbolVisiblity(topSymbol, is_visible3);
				foreach (KAnimHashedString bottomSymbol in bottomSymbols)
					componentsInChild.SetSymbolVisiblity(bottomSymbol, is_visible4);
			}
		}

		private bool HasTileableNeighbour(int neighbour_cell)
		{
			bool flag = false;
			GameObject gameObject = Grid.Objects[neighbour_cell, (int)this.objectLayer];
			if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
			{
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.HasAnyTags(this.tags))
					flag = true;
			}
			return flag;
		}

		private void OnNeighbourCellsUpdated(object data)
		{
			if ((UnityEngine.Object)this == (UnityEngine.Object)null || (UnityEngine.Object)this.gameObject == (UnityEngine.Object)null || !this.partitionerEntry.IsValid())
				return;
			this.UpdateEndCaps();
		}
	}

}
