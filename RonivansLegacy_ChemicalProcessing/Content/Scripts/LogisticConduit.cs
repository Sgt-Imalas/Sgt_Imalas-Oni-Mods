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
		static Dictionary<int, GameObject> ConduitGOs = [];
		static Dictionary<int, GameObject> BridgeGOs = [];
		static HashSet<LogisticConduit> AllLogisticConduits = [];
		static HashSet<GameObject> AllLogisticConduitGOs = [];
		public override void OnSpawn()
		{
			base.OnSpawn();
			RegisterLogisticConduit();
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			gameObject.AddOrGet<ConduitCapacityDescriptor>();
		}

		void RegisterLogisticConduit()
		{
			if (buildingComplete.Def.ObjectLayer == ObjectLayer.SolidConduit)
			{
				Conduits[Grid.PosToCell(this)] = this;
				ConduitGOs[Grid.PosToCell(this)] = gameObject;
			}
			else
			{
				Bridges[buildingComplete.PlacementCells.Min()] = this;
				Bridges[buildingComplete.PlacementCells.Max()] = this;
				BridgeGOs[buildingComplete.PlacementCells.Min()] = gameObject;
				BridgeGOs[buildingComplete.PlacementCells.Max()] = gameObject;
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
				ConduitGOs.Remove(Grid.PosToCell(this));
			}
			else
			{
				Bridges.Remove(buildingComplete.PlacementCells.Min());
				Bridges.Remove(buildingComplete.PlacementCells.Max());
				BridgeGOs.Remove(buildingComplete.PlacementCells.Min());
				BridgeGOs.Remove(buildingComplete.PlacementCells.Max());
			}
			AllLogisticConduitGOs.Remove(gameObject);
			AllLogisticConduits.Add(this);
			base.OnCleanUp();
		}
		public override void OnCleanUp()
		{
			UnregisterLogisticConduit();
			base.OnCleanUp();
		}
		public static void ClearEverything()
		{
			Conduits.Clear();
			Bridges.Clear();
			ConduitGOs.Clear();
			BridgeGOs.Clear();
			AllLogisticConduitGOs.Clear();
			AllLogisticConduits.Clear();
		}
		public static bool IsLogisticConduit(GameObject go ) => go == null ? false : AllLogisticConduitGOs.Contains(go);

		public static bool HasLogisticConduitAt(int cell, bool bridge = false)
		{			
			if(bridge)
				return Bridges.ContainsKey(cell);
			return Conduits.ContainsKey(cell);
		}
		public static bool TryGetLogisticConduitAt(int cell, bool bridge, out LogisticConduit conduit)
		{
			if(bridge)
				return Bridges.TryGetValue(cell, out conduit);
			return Conduits.TryGetValue(cell, out conduit);
		}
		public static bool TryGetLogisticConduitAt(int cell, bool bridge, out GameObject conduit)
		{
			if (bridge)
				return BridgeGOs.TryGetValue(cell, out conduit);
			return ConduitGOs.TryGetValue(cell, out conduit);
		}
	}
}
