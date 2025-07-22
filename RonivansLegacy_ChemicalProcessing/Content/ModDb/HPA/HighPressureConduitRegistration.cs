using RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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


		public static HashSet<GameObject> AllHighPressureConduitGOs = [];

		public static HashSet<int> AllInsulatedSolidConduitCells = [];

		static HashSet<int>
			HPA_Solid = [],
			HPA_Liquid = [],
			HPA_Gas = [],

			HPA_SolidBridge = [],
			HPA_LiquidBridge = [],
			HPA_GasBridge = [],

			All_Solid = [],
			All_Liquid = [],
			All_Gas = [],

			All_SolidBridge = [],
			All_LiquidBridge = [],
			All_GasBridge = [];

		//public static Dictionary<int, HashSet<int>> HighPressureConduitsByLayer;
		//public static Dictionary<int, HashSet<int>> AllConduitsByLayer;

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
			AllHighPressureConduitGOs.Clear();
			BrokenRails.Clear();
			HighPressureConduitEventHandler.CancelPendingEvents();
			//HighPressureConduitsByLayer = new()
			//{
			//{ (int)ObjectLayer.GasConduit,[] },
			//{ (int)ObjectLayer.LiquidConduit,[] },
			//{ (int)ObjectLayer.GasConduitConnection,[] },
			//{ (int)ObjectLayer.LiquidConduitConnection,[] },
			//{ (int)ObjectLayer.SolidConduit,[] },
			//{ (int)ObjectLayer.SolidConduitConnection,[] },
			//}; 
			//AllConduitsByLayer = new()
			//{
			//{ (int)ObjectLayer.GasConduit,[] },
			//{ (int)ObjectLayer.LiquidConduit,[] },
			//{ (int)ObjectLayer.GasConduitConnection,[] },
			//{ (int)ObjectLayer.LiquidConduitConnection,[] },
			//{ (int)ObjectLayer.SolidConduit,[] },
			//{ (int)ObjectLayer.SolidConduitConnection,[] },
			//};
		}


		internal static bool IsHighPressureConduit(GameObject currentItem) => currentItem == null ? false : AllHighPressureConduitGOs.Contains(currentItem);

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

		public static float GetMaxConduitCapacityAt(int cell, ConduitType type, bool isBridge = false)
		{
			int targetLayer = -1;
			bool isHP = false;
			switch (type)
			{
				case ConduitType.Gas:
					isHP = isBridge ? HPA_GasBridge.Contains(cell) : HPA_Gas.Contains(cell);
					if(isHP)
						return CachedHPAConduitCapacity(type);
					else
						return CachedRegularConduitCapacity(type);
				case ConduitType.Liquid:
					isHP = isBridge ? HPA_LiquidBridge.Contains(cell) : HPA_Liquid.Contains(cell);
					if (isHP)
						return CachedHPAConduitCapacity(type);
					else
						return CachedRegularConduitCapacity(type);
				case ConduitType.Solid:
					if(LogisticConduit.HasLogisticConduitAt(cell, isBridge))
						return SolidCap_Logistic;
					isHP = isBridge ? HPA_SolidBridge.Contains(cell) : HPA_Solid.Contains(cell);
					if (isHP)
						return CachedHPAConduitCapacity(type);
					else
						return CachedRegularConduitCapacity(type);
				default:
					throw new NotImplementedException("Invalid conduit target type");
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
			switch (type)
			{
				case ConduitType.Gas:
					return bridge ? HPA_GasBridge.Contains(cell) : HPA_Gas.Contains(cell);
				case ConduitType.Liquid:
					return bridge ? HPA_LiquidBridge.Contains(cell) : HPA_Liquid.Contains(cell);
				case ConduitType.Solid:
					return bridge ? HPA_SolidBridge.Contains(cell) : HPA_Solid.Contains(cell);
				default:
					return false;
			}
		}
		public static bool HasConduitAt(int cell, ConduitType type, bool bridge = false)
		{
			switch (type)
			{
				case ConduitType.Gas:
					return bridge ? All_GasBridge.Contains(cell) : All_Gas.Contains(cell);
				case ConduitType.Liquid:
					return bridge ? All_LiquidBridge.Contains(cell) : All_Liquid.Contains(cell);
				case ConduitType.Solid:
					return bridge ? All_SolidBridge.Contains(cell) : All_Solid.Contains(cell);
				default:
					return false;
			}
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

		public static void RegisterConduit(GameObject conduit)
		{
			if(conduit.TryGetComponent<Building>(out var buildingComplete))
			{
				switch (buildingComplete.Def.ObjectLayer)
				{
					case ObjectLayer.SolidConduit:
						All_Solid.Add(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.SolidConduitConnection:
						All_SolidBridge.Add(buildingComplete.GetUtilityInputCell());
						All_SolidBridge.Add(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.LiquidConduit:
						All_Liquid.Add(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.LiquidConduitConnection:
						All_LiquidBridge.Add(buildingComplete.GetUtilityInputCell());
						All_LiquidBridge.Add(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.GasConduit:
						All_Gas.Add(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.GasConduitConnection:
						All_GasBridge.Add(buildingComplete.GetUtilityInputCell());
						All_GasBridge.Add(buildingComplete.GetUtilityOutputCell());
						break;
				}
			}
		}
		public static void UnregisterConduit(GameObject conduit)
		{
			if (conduit.TryGetComponent<Building>(out var buildingComplete))
			{
				switch (buildingComplete.Def.ObjectLayer)
				{
					case ObjectLayer.SolidConduit:
						All_Solid.Remove(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.SolidConduitConnection:
						All_SolidBridge.Remove(buildingComplete.GetUtilityInputCell());
						All_SolidBridge.Remove(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.LiquidConduit:
						All_Liquid.Remove(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.LiquidConduitConnection:
						All_LiquidBridge.Remove(buildingComplete.GetUtilityInputCell());
						All_LiquidBridge.Remove(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.GasConduit:
						All_Gas.Remove(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.GasConduitConnection:
						All_GasBridge.Remove(buildingComplete.GetUtilityInputCell());
						All_GasBridge.Remove(buildingComplete.GetUtilityOutputCell());
						break;
				}
			}
		}
		public static void UnregisterHighPressureConduit(HighPressureConduit conduit)
		{
			AllHighPressureConduitGOs.Remove(conduit.gameObject);
			switch (conduit.buildingComplete.Def.ObjectLayer)
			{
				case ObjectLayer.SolidConduit:
					HPA_Solid.Remove(conduit.buildingComplete.NaturalBuildingCell());
					break;
				case ObjectLayer.SolidConduitConnection:
					HPA_SolidBridge.Remove(conduit.buildingComplete.GetUtilityInputCell());
					HPA_SolidBridge.Remove(conduit.buildingComplete.GetUtilityOutputCell());
					break;
				case ObjectLayer.LiquidConduit:
					HPA_Liquid.Remove(conduit.buildingComplete.NaturalBuildingCell());
					break;
				case ObjectLayer.LiquidConduitConnection:
					HPA_LiquidBridge.Remove(conduit.buildingComplete.GetUtilityInputCell());
					HPA_LiquidBridge.Remove(conduit.buildingComplete.GetUtilityOutputCell());
					break;
				case ObjectLayer.GasConduit:
					HPA_Gas.Remove(conduit.buildingComplete.NaturalBuildingCell());
					break;
				case ObjectLayer.GasConduitConnection:
					HPA_GasBridge.Remove(conduit.buildingComplete.GetUtilityInputCell());
					HPA_GasBridge.Remove(conduit.buildingComplete.GetUtilityOutputCell());
					break;
			}
			if (conduit.InsulateSolidContents)
			{
				AllInsulatedSolidConduitCells.Remove(conduit.NaturalBuildingCell());
			}
			UnregisterConduit(conduit.gameObject);
		}
		public static void RegisterHighPressureConduit(HighPressureConduit conduit)
		{
			AllHighPressureConduitGOs.Add(conduit.gameObject);

			switch (conduit.buildingComplete.Def.ObjectLayer)
			{
				case ObjectLayer.SolidConduit:
					HPA_Solid.Add(conduit.buildingComplete.NaturalBuildingCell());
					break;
				case ObjectLayer.SolidConduitConnection:
					HPA_SolidBridge.Add(conduit.buildingComplete.GetUtilityInputCell());
					HPA_SolidBridge.Add(conduit.buildingComplete.GetUtilityOutputCell());
					break;
				case ObjectLayer.LiquidConduit:
					HPA_Liquid.Add(conduit.buildingComplete.NaturalBuildingCell());
					break;
				case ObjectLayer.LiquidConduitConnection:
					HPA_LiquidBridge.Add(conduit.buildingComplete.GetUtilityInputCell());
					HPA_LiquidBridge.Add(conduit.buildingComplete.GetUtilityOutputCell());
					break;
				case ObjectLayer.GasConduit:
					HPA_Gas.Add(conduit.buildingComplete.NaturalBuildingCell());
					break;
				case ObjectLayer.GasConduitConnection:
					HPA_GasBridge.Add(conduit.buildingComplete.GetUtilityInputCell());
					HPA_GasBridge.Add(conduit.buildingComplete.GetUtilityOutputCell());
					break;
			}
			if (conduit.InsulateSolidContents)
			{
				AllInsulatedSolidConduitCells.Add(conduit.buildingComplete.PlacementCells.Min());
			}
			RegisterConduit(conduit.gameObject);
		}

		static Tuple<SimTemperatureTransfer, KPrefabID> cached = null;
		static Dictionary<Pickupable,Tuple<SimTemperatureTransfer, KPrefabID>> _insulatedPickupables = new(2048);
		internal static Pickupable SetInsulatedState(Pickupable pickupable, bool sealAndInsulate)
		{
			if (pickupable == null || pickupable.gameObject == null)
				return pickupable;

			if(_insulatedPickupables.TryGetValue(pickupable, out cached))
			{
				if (cached.first != null)
					cached.first.enabled = !sealAndInsulate;
				if (cached.second != null)
				{
					if (sealAndInsulate)
						cached.second.AddTag(GameTags.Sealed);
					else
						cached.second.RemoveTag(GameTags.Sealed);
				}
				return pickupable;
			}
			if (pickupable.TryGetComponent<SimTemperatureTransfer>(out var _tempTransfer) && pickupable.TryGetComponent<KPrefabID>(out var _kPrefab))
			{
				_insulatedPickupables[pickupable] = new Tuple<SimTemperatureTransfer, KPrefabID>(_tempTransfer, _kPrefab);
				return SetInsulatedState(pickupable, sealAndInsulate);
			}
			return pickupable;
		}

		internal static bool IsInsulatedRail(int cell)
		{
			if (cell < 0)
				return false;

			return AllInsulatedSolidConduitCells.Contains(cell);
		}
	}
}
