using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class HPA_InsulationCapHandler : KMonoBehaviour
	{
		[MyCmpReq]
		KAnimGraphTileVisualizer animTileVisualizer;

		int cell, cell_u, cell_d, cell_l, cell_r;
		readonly string symbol_u = "cap_u", symbol_d = "cap_d", symbol_l = "cap_l", symbol_r = "cap_r";

		GameObject CapVisualizer;
		KBatchedAnimController VisualizerKbac;

		public override void OnSpawn()
		{
			cell = Grid.PosToCell(this);
			cell_u = Grid.CellAbove(cell);
			cell_d = Grid.CellBelow(cell);
			cell_l = Grid.CellLeft(cell);
			cell_r = Grid.CellRight(cell);
			Subscribe((int)GameHashes.ConnectionsChanged, OnUtilityConnectionsChanged);
			base.OnSpawn();

			string name = this.name + "_CapVisualizer";
			CapVisualizer = new GameObject(name);
			CapVisualizer.SetActive(false);
			CapVisualizer.transform.SetParent(this.transform);
			CapVisualizer.AddComponent<KPrefabID>().PrefabTag = new Tag(name);
			VisualizerKbac = CapVisualizer.AddComponent<KBatchedAnimController>();
			VisualizerKbac.AnimFiles = [Assets.GetAnim("hpa_rail_insulation_caps_kanim")];
			VisualizerKbac.initialAnim = "caps";
			VisualizerKbac.isMovable = true;

			var pos = this.transform.position;
			pos.y += 0.5f;
			CapVisualizer.transform.SetPosition(pos);

			VisualizerKbac.SetSceneLayer(Grid.SceneLayer.Wires); //1 above insulated hpa conduit window fg
			CapVisualizer.SetActive(true);
			OnUtilityConnectionsChanged(null);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.ConnectionsChanged, OnUtilityConnectionsChanged);
			base.OnCleanUp();
			UnityEngine.Object.Destroy(CapVisualizer);
		}
		IEnumerator DelayedRefresh()
		{
			yield return new WaitForEndOfFrame(); 
			RefreshConnection();
			OnUtilityConnectionsChanged(null);
		}
		void RefreshConnection()
		{
			if (animTileVisualizer.connectionManager == null || animTileVisualizer.skipRefresh)
			{
				return;
			}

			UtilityConnections connections = animTileVisualizer.connectionManager.GetConnections(cell, true);
			VisualizerKbac.SetSymbolVisiblity(symbol_r, (connections & UtilityConnections.Right) != 0 && !HighPressureConduitRegistration.IsInsulatedRail(cell_r));
			VisualizerKbac.SetSymbolVisiblity(symbol_l, (connections & UtilityConnections.Left) != 0 && !HighPressureConduitRegistration.IsInsulatedRail(cell_l));
			VisualizerKbac.SetSymbolVisiblity(symbol_u, (connections & UtilityConnections.Up) != 0 && !HighPressureConduitRegistration.IsInsulatedRail(cell_u));
			VisualizerKbac.SetSymbolVisiblity(symbol_d, (connections & UtilityConnections.Down) != 0 && !HighPressureConduitRegistration.IsInsulatedRail(cell_d));
		}

		void OnUtilityConnectionsChanged(object data)
		{
			StartCoroutine(DelayedRefresh());
		}
	}
}
