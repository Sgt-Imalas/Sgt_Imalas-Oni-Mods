using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LogicGateVisualizer;

namespace UtilLibs.BuildingPortUtils
{
	[SkipSaveFileSerialization]
	public class PortDisplay2 : KMonoBehaviour
	{
		private GameObject portObject;

		// The cache for last location/color.
		// The default values doesn't matter and will be overwritten on first call.
		// However there is a theoredical risk that no default value can cause a crash, hence setting them to something.
		[SerializeField]
		private int lastUtilityCell = -1;

		[SerializeField]
		private Color lastColor = Color.black;

		[SerializeField]
		internal ConduitType type;

		[SerializeField]
		internal CellOffset offset;

		[SerializeField]
		internal CellOffset offsetFlipped;

		[SerializeField]
		internal bool input;

		[SerializeField]
		internal Color32 color;

		[SerializeField]
		internal Sprite sprite;

		public bool Input => input;
		public Sprite Sprite => sprite;
		public ConduitType Type => type;

		internal void AssignPort(DisplayConduitPortInfo port)
		{
			this.type = port.type;
			this.offset = port.offset;
			this.offsetFlipped = port.offsetFlipped;
			this.input = port.input;
			this.color = port.color;
			this.sprite = SharedConduitUtils.GetSprite(this.input, this.type);
		}

		internal void Draw(GameObject obj, BuildingCellVisualizer visualizer, bool force)
		{
			Building building = visualizer.building;
			int utilityCell = GetUtilityCell(building);

			// redraw if anything changed
			if (force || utilityCell != this.lastUtilityCell || color != this.lastColor)
			{
				this.lastColor = color;
				this.lastUtilityCell = utilityCell;
				visualizer.DrawUtilityIcon(utilityCell, this.sprite, ref portObject, color);
			}
		}

		void AttachTooltip(string tooltip)
		{
			if (portObject == null)
				return;
			//UIUtils.AddSimpleTooltipToObject(portObject, tooltip);
		}

		public int GetUtilityCell(Building building)
		{
			return building.GetCellWithOffset(building.Orientation == Orientation.Neutral ? this.offset : this.offsetFlipped);
		}
		public CellOffset GetUtilityCellOffset(Building building)
		{
			return (building.Orientation == Orientation.Neutral ? this.offset : this.offsetFlipped);
		}


		internal void DisableIcons()
		{
			if (this.portObject != null)
			{
				if (this.portObject != null && this.portObject.activeInHierarchy)
				{
					this.portObject.SetActive(false);
				}
			}
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			if (this.portObject != null)
			{
				UnityEngine.Object.Destroy(this.portObject);
			}
		}
	}
}
