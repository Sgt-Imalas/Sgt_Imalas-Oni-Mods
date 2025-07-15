using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class LogisticConduit : KMonoBehaviour
	{
		[MyCmpReq] BuildingComplete buildingComplete;

		static Dictionary<int, LogisticConduit> Conduits = [];
		static Dictionary<int, LogisticConduit> Bridges = [];
		static HashSet<LogisticConduit> AllLogisticConduits = [];
		static HashSet<GameObject> AllLogisticConduitGOs = [];
		public override void OnSpawn()
		{
			base.OnSpawn();
			RegisterLogisticConduit();
		}

		void RegisterLogisticConduit()
		{
			if (buildingComplete.Def.ObjectLayer == ObjectLayer.SolidConduit)
			{
				Conduits[Grid.PosToCell(this)] = this;
			}
			else
			{
				Bridges[buildingComplete.PlacementCells.Min()] = this;
				Bridges[buildingComplete.PlacementCells.Max()] = this;
			}
			//SgtLogger.l($"Registering high pressure conduit for layer {buildingComplete.Def.ObjectLayer} for cells {buildingComplete.PlacementCells.Min()} and {buildingComplete.PlacementCells.Max()}");
			AllLogisticConduitGOs.Add(gameObject);
			AllLogisticConduits.Add(this);
		}
		void UnregisterLogisticConduit()
		{
			if (buildingComplete.Def.ObjectLayer == ObjectLayer.SolidConduit)
			{
				Conduits.Remove(Grid.PosToCell(this));
			}
			else
			{
				Bridges.Remove(buildingComplete.PlacementCells.Min());
				Bridges.Remove(buildingComplete.PlacementCells.Max());
			}
			AllLogisticConduitGOs.Remove(gameObject);
			AllLogisticConduits.Add(this);
			base.OnCleanUp();
		}
		public override void OnCleanUp()
		{
			UnregisterLogisticConduit();
			base .OnCleanUp();
		}
		public static void ClearEverything()
		{
			Conduits.Clear();
			Bridges.Clear();
			AllLogisticConduitGOs.Clear();
			AllLogisticConduits.Clear();
		}
	}
}
