using RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
    class HighPressureConduitRegistration
	{
		//cache this to avoid calling config.instance each time
		private static bool _capInit = false;
		private static float _gasCap_hp = -1, _liquidCap_hp = -1, _solidCap_hp = -1, _solidCap_logistic = -1;
		private static float _gasCap_reg = -1, _liquidCap_reg = -1, _solidCap_reg = -1;
		private static Color32 _gasFlowOverlay = new Color32(169, 209, 251, 0), _gasFlowTint = new Color32(176, 176, 176, 255),
			_liquidFlowOverlay = new Color32(92, 144, 121, 0), _liquidFlowTint = new Color32(92, 144, 121, 255),
			_solidOverlayTint = new(154, 255, 167, 0);

		public static float SolidCap_HP => _solidCap_hp;
		public static float SolidCap_Logistic => _solidCap_logistic;
		public static float SolidCap_Regular => _solidCap_reg;
		
		#region brokenRails
		///experiment to handle broken rails not transporting anymore, currently unused
		public static bool IsSolidConduitBroken(int cell) => BrokenRails.Contains(cell);
		static HashSet<int> BrokenRails = [];
		public static void RegisterRailBrokenState(SolidConduit conduit, bool broken)
		{
			if (broken)
				BrokenRails.Add(conduit.NaturalBuildingCell());
			else
				BrokenRails.Remove(conduit.NaturalBuildingCell());
		}
		#endregion


		public static HashSet<HighPressureConduit> AllConduits = [];
		public static HashSet<GameObject> AllConduitGOs = [];

		public static Dictionary<int, Dictionary<int, HighPressureConduit>> ConduitsByLayer;

		static HighPressureConduitRegistration()
		{
			ClearEverything();
			InitCache();
		}
		private static void InitCache()
		{
			if (!_capInit)
			{
				_capInit = true;
				_gasCap_hp = Config.Instance.HPA_Capacity_Gas;
				_liquidCap_hp = Config.Instance.HPA_Capacity_Liquid;
				_solidCap_hp = Config.Instance.Rail_Capacity_HPA;
				_solidCap_logistic = Config.Instance.Rail_Capacity_Logistic;

				_gasCap_reg = ConduitFlow.MAX_GAS_MASS;
				_liquidCap_reg = ConduitFlow.MAX_LIQUID_MASS;
				_solidCap_reg = SolidConduitFlow.MAX_SOLID_MASS;
			}
		}

		public static float CachedHPAConduitCapacity(ConduitType type)
		{
			switch (type)
			{
				case ConduitType.Gas:
					return _gasCap_hp;
				case ConduitType.Liquid:
					return _liquidCap_hp;
				case ConduitType.Solid:
					return _solidCap_hp;
			}
			return 1;
		}
		public static float CachedRegularConduitCapacity(ConduitType type)
		{
			switch (type)
			{
				case ConduitType.Gas:
					return _gasCap_reg;
				case ConduitType.Liquid:
					return _liquidCap_reg;
				case ConduitType.Solid:
					return _solidCap_reg;
			}
			return 1;
		}

		public static void ClearEverything()
		{
			BrokenRails.Clear();
			HighPressureConduitEventHandler.CancelPendingEvents();
			AllConduits.Clear();
			AllConduitGOs.Clear();
			ConduitsByLayer = new()
			{
			{ (int)ObjectLayer.GasConduit,new Dictionary<int, HighPressureConduit>() },
			{ (int)ObjectLayer.LiquidConduit,new Dictionary<int, HighPressureConduit>() },
			{ (int)ObjectLayer.GasConduitConnection,new Dictionary<int, HighPressureConduit>() },
			{ (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, HighPressureConduit>() },
			{ (int)ObjectLayer.SolidConduit,new Dictionary<int, HighPressureConduit>() },
			{ (int)ObjectLayer.SolidConduitConnection,new Dictionary<int, HighPressureConduit>() },
			};
		}


		internal static bool IsHighPressureConduit(GameObject currentItem) => currentItem == null ? false : AllConduitGOs.Contains(currentItem);

		/// <summary>
		/// returns the mass a regular pipe would have if it had the same fill state as the high pressure pipe of this type
		/// example: liquid capacity of 40, half filled with 20kg; 20kg/s -> 20 / (40/10) -> 20kg / 4 -> 5kg
		/// normal pipe half filled is 5kg
		/// </summary>
		/// <param name="absPipeMass"></param>
		/// <param name="currentConduitType"></param>
		/// <returns></returns>
		internal static float GetNormalizedPercentageMass(float absPipeMass, ConduitType currentConduitType)
		{
			return absPipeMass / GetConduitMultiplier(currentConduitType);
		}

		public static float GetMaxConduitCapacity(ConduitType type, bool highPressure)
		{
			if (highPressure)
			{
				switch (type)
				{
					case ConduitType.Gas:
						return _gasCap_hp;
					case ConduitType.Liquid:
						return _liquidCap_hp;
					case ConduitType.Solid:
						return _solidCap_hp;
				}
			}
			else
			{
				switch (type)
				{
					case ConduitType.Gas:
						return _gasCap_reg;
					case ConduitType.Liquid:
						return _liquidCap_reg;
					case ConduitType.Solid:
						return _solidCap_reg;
				}
			}
			return 1;
		}

		public static float GetConduitMultiplier(ConduitType currentConduitType)
		{
			if (currentConduitType == ConduitType.Gas)
			{
				return (_gasCap_hp / _gasCap_reg);
			}
			else if (currentConduitType == ConduitType.Liquid)
			{
				return (_liquidCap_hp / _liquidCap_reg);
			}
			else if (currentConduitType == ConduitType.Solid)
			{
				return (_solidCap_hp / _solidCap_reg);
			}
			return 1;
		}
		public static float GetLogisticConduitMultiplier()
		{
			return _solidCap_logistic / _solidCap_reg;
		}
		public static float GetMaxConduitCapacityWithConduitGOAt(int cell, ConduitType type, out GameObject go, bool isBridge = false)
		{
			int targetLayer = -1;
			switch (type)
			{
				case ConduitType.Gas:
					targetLayer = isBridge ? (int)ObjectLayer.GasConduitConnection : (int)ObjectLayer.GasConduit; break;
				case ConduitType.Liquid:
					targetLayer = isBridge ? (int)ObjectLayer.LiquidConduitConnection : (int)ObjectLayer.LiquidConduit; break;
				case ConduitType.Solid:
					targetLayer = isBridge ? (int)ObjectLayer.SolidConduitConnection : (int)ObjectLayer.SolidConduitConnection; break;
				default:
					throw new NotImplementedException("Invalid conduit target type");
			}
			if (ConduitsByLayer[targetLayer].TryGetValue(cell, out var cmp))
			{
				go = cmp.gameObject;
				return CachedHPAConduitCapacity(type);
			}
			else
			{
				go = GetConduitAt(cell, type);
				if (LogisticConduit.HasLogisticConduitAt(cell, isBridge))
					return SolidCap_Logistic;
				return CachedRegularConduitCapacity(type);
			}
		}
		public static GameObject GetConduitAt(int cell, ConduitType type, bool isBridge = false)
		{
			if (type == ConduitType.Gas)
			{
				return Grid.Objects[cell, isBridge ? (int)ObjectLayer.GasConduitConnection : (int)ObjectLayer.GasConduit];
			}
			else if (type == ConduitType.Liquid)
			{
				return Grid.Objects[cell, isBridge ? (int)ObjectLayer.LiquidConduitConnection : (int)ObjectLayer.LiquidConduit];
			}
			else if (type == ConduitType.Solid)
			{
				return Grid.Objects[cell, isBridge ? (int)ObjectLayer.SolidConduitConnection : (int)ObjectLayer.SolidConduit];
			}
			throw new NotImplementedException("Tried getting invalid conduit type");
		}

		public static bool TryGetHPACapacityAt(int cell, ConduitType type, out float capacity, bool bridge = false)
		{
			capacity = CachedHPAConduitCapacity(type);
			return HasHighPressureConduitAt(cell, type, bridge);
		}
		public static bool HasHighPressureConduitAt(int cell, ConduitType type, bool bridge = false)
		{
			if (type == ConduitType.Gas)
			{
				return ConduitsByLayer[bridge ? (int)ObjectLayer.GasConduitConnection : (int)ObjectLayer.GasConduit].ContainsKey(cell);
			}
			else if (type == ConduitType.Liquid)
			{
				return ConduitsByLayer[bridge ? (int)ObjectLayer.LiquidConduitConnection : (int)ObjectLayer.LiquidConduit].ContainsKey(cell);
			}
			else if (type == ConduitType.Solid)
			{
				return ConduitsByLayer[bridge ? (int)ObjectLayer.SolidConduitConnection : (int)ObjectLayer.SolidConduit].ContainsKey(cell);
			}
			return false;
		}
		public static bool HasHighPressureConduitAt(int cell, ConduitType type, bool showContents, out Color32 tint)
		{
			if (HasHighPressureConduitAt(cell, type, false))
			{
				tint = GetColorForConduitType(type, showContents);
				return true;
			}
			tint = Color.white;
			return false;
		}
		public static Color32 GetColorForConduitType(ConduitType conduitType, bool overlay)
		{
			if (conduitType == ConduitType.Gas)
				return overlay ? _gasFlowOverlay : _gasFlowTint;
			else if (conduitType == ConduitType.Liquid)
				return overlay ? _liquidFlowOverlay : _liquidFlowTint;
			else if (conduitType == ConduitType.Solid)
				return _solidOverlayTint;
			return Color.white;
		}

		public static void RegisterHighPressureConduit(HighPressureConduit conduit)
		{
			if (!ConduitsByLayer.ContainsKey((int)conduit.buildingComplete.Def.ObjectLayer))
				ConduitsByLayer.Add((int)conduit.buildingComplete.Def.ObjectLayer, new());


			ConduitsByLayer[(int)conduit.buildingComplete.Def.ObjectLayer][conduit.buildingComplete.PlacementCells.Min()] = conduit;
			ConduitsByLayer[(int)conduit.buildingComplete.Def.ObjectLayer][conduit.buildingComplete.PlacementCells.Max()] = conduit;
			AllConduitGOs.Add(conduit.gameObject);
			AllConduits.Add(conduit);
		}
		public static void UnregisterHighPressureConduit(HighPressureConduit conduit)
		{
			ConduitsByLayer[(int)conduit.buildingComplete.Def.ObjectLayer].Remove(conduit.buildingComplete.PlacementCells.Min());
			ConduitsByLayer[(int)conduit.buildingComplete.Def.ObjectLayer].Remove(conduit.buildingComplete.PlacementCells.Max());
			AllConduitGOs.Remove(conduit.gameObject);
			AllConduits.Remove(conduit);
		}
	}
}
