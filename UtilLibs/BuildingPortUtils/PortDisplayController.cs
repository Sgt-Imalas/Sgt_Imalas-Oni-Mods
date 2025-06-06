using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs.BuildingPortUtils
{

	[SkipSaveFileSerialization]
	public class PortDisplayController : KMonoBehaviour
	{
		[SerializeField]
		private HashedString lastMode = OverlayModes.None.ID;

		[SerializeField]
		private List<PortDisplay2> gasOverlay = new List<PortDisplay2>();

		[SerializeField]
		private List<PortDisplay2> liquidOverlay = new List<PortDisplay2>();

		[SerializeField]
		private List<PortDisplay2> solidOverlay = new List<PortDisplay2>();

		public void AssignPort(GameObject go, DisplayConduitPortInfo port)
		{
			PortDisplay2 portDisplay = go.AddComponent<PortDisplay2>();
			portDisplay.AssignPort(port);

			switch (port.type)
			{
				case ConduitType.Gas:
					this.gasOverlay.Add(portDisplay);
					break;
				case ConduitType.Liquid:
					this.liquidOverlay.Add(portDisplay);
					break;
				case ConduitType.Solid:
					this.solidOverlay.Add(portDisplay);
					break;
			}
		}

		public void Init(GameObject go)
		{
			string ID = go.GetComponent<KPrefabID>().PrefabTag.Name;

			// criteria for drawing port icons on buildings
			// vanilla will only attempt to draw icons on buildings with BuildingCellVisualizer
			go.AddOrGet<BuildingCellVisualizer>();

			// when vanilla tries to draw, call this controller if the building is in the DrawPorts list
			ConduitDisplayPortPatching.AddBuilding(ID);
		}

		public bool Draw(BuildingCellVisualizer __instance, HashedString mode, GameObject go)
		{
			bool isNewMode = mode != this.lastMode;

			if (isNewMode)
			{
				this.ClearPorts();
				this.lastMode = mode;
			}

			foreach (PortDisplay2 port in this.GetPorts(mode))
			{
				port.Draw(go, __instance, isNewMode);
			}

			return true;
		}

		private void ClearPorts()
		{
			foreach (PortDisplay2 port in this.GetPorts(this.lastMode))
			{
				port.DisableIcons();
			}
		}

		private List<PortDisplay2> GetPorts(HashedString mode)
		{
			if (mode == OverlayModes.GasConduits.ID) return this.gasOverlay;
			if (mode == OverlayModes.LiquidConduits.ID) return this.liquidOverlay;
			if (mode == OverlayModes.SolidConveyor.ID) return this.solidOverlay;

			return new List<PortDisplay2>();
		}
	}

}
