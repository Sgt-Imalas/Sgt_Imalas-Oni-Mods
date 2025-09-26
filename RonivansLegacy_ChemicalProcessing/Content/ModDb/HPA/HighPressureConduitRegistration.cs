using RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations;
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
		private static bool _usingInsulatedSolidRails;
		private static bool _capInit = false;
		private static float _gasMult = -1, _liquidMult = -1, _solidMult = -1, _logisticMult = -1;
		private static float _gasCap_hp = -1, _liquidCap_hp = -1, _solidCap_hp = -1, _solidCap_logistic = -1;
		private static float _gasCap_reg = -1, _liquidCap_reg = -1, _solidCap_reg = -1;
		private static Color32 _gasFlowOverlay = new Color32(169, 209, 251, 0), _gasFlowTint = new Color32(176, 176, 176, 255),
			_liquidFlowOverlay = new Color32(92, 144, 121, 0), _liquidFlowTint = new Color32(92, 144, 121, 255),
			_solidOverlayTint = new(166, 175, 230, 0), _logisticOverlayTint = new(152, 193, 150, 0);

		public static float SolidCap_HP => _solidCap_hp;
		public static float SolidCap_Logistic => _solidCap_logistic;
		public static float SolidCap_Regular => _solidCap_reg;

		public static float GasCap_HP => _gasCap_hp;
		public static float LiquidCap_HP => _liquidCap_hp;




		public static HashSet<int>
			ValveOutputs_Liquid = [],
			ValveOutputs_Gas = []
			;

		public static HashSet<int> DynamicSolidConduitDispenserHandles = [];

		public static HashSet<int> AllHighPressureConduitGOHandles = [];

		public static HashSet<int> AllInsulatedSolidConduitCells = [];
		//public static HashSet<int> AllInsulatedSolidConduitGOHandles = [];

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
		public static void InitCache(bool force = false)
		{
			if (!_capInit || force)
			{
				if (force)
					SgtLogger.l("Re-Initializing PipeCapacity Cache");
				else
					SgtLogger.l("Initializing PipeCapacity Cache");

				_capInit = true;

				///loading vanilla values
				_gasCap_reg = ConduitFlow.MAX_GAS_MASS;
				_liquidCap_reg = ConduitFlow.MAX_LIQUID_MASS;
				_solidCap_reg = SolidConduitFlow.MAX_SOLID_MASS;

				_gasMult = Config.Instance.HPA_Capacity_Gas_Multiplier;
				_liquidMult = Config.Instance.HPA_Capacity_Liquid_Multiplier;
				_solidMult = Config.Instance.HPA_Capacity_Solid_Multiplier;
				_logisticMult = Config.Instance.Logistic_Rail_Capacity_Multiplier;

				_usingInsulatedSolidRails = Config.Instance.HPA_Rails_Insulation_Mod_Enabled;

				///Checking if CustomizeBuilding is installed and if it adds custom values
				if (CustomizeBuildings.TryGetModifiedConduitValues(out float solid, out float liquid, out float gas))
				{
					_solidCap_reg = solid;
					_liquidCap_reg = liquid;
					_gasCap_reg = gas;
				}
				///logistic rails total capacity
				_solidCap_logistic = _logisticMult * _solidCap_reg;

				///High pressure conduits total capacity
				_gasCap_hp = _gasMult * _gasCap_reg;
				_liquidCap_hp = _liquidMult * _liquidCap_reg;
				_solidCap_hp = _solidMult * _solidCap_reg;

				SgtLogger.l($"Pipe Capacity Values cached:" +
					$"\nLogistic Rails: capacity: {_solidCap_logistic}, multiplier: {_logisticMult}" +
					$"\nRegular Conduits:" +
					$"\n- Solid: capacity: {_solidCap_reg}" +
					$"\n- Liquid: capacity: {_liquidCap_reg}" +
					$"\n- Gas: capacity: {_gasCap_reg}" +
					$"\nHigh Pressure Conduits:" +
					$"\n- Solid: capacity: {_solidCap_hp}, multiplier: {_solidMult}" +
					$"\n- Liquid: capacity: {_liquidCap_hp}, multiplier: {_liquidMult}" +
					$"\n- Gas: capacity: {_gasCap_hp}, multiplier: {_gasMult}");
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
			HighPressureConduitEventHandler.CancelPendingEvents();
			AllHighPressureConduitGOHandles.Clear();
			_cachedPickupables.Clear();

			HPA_Solid.Clear();
			HPA_Liquid.Clear();
			HPA_Gas.Clear();

			HPA_SolidBridge.Clear();
			HPA_LiquidBridge.Clear();
			HPA_GasBridge.Clear();

			All_Solid.Clear();
			All_Liquid.Clear();
			All_Gas.Clear();

			All_SolidBridge.Clear();
			All_LiquidBridge.Clear();
			All_GasBridge.Clear();

			ValveOutputs_Liquid.Clear();
			ValveOutputs_Gas.Clear();

			AllInsulatedSolidConduitCells.Clear();
			DynamicSolidConduitDispenserHandles.Clear();
		}


		internal static bool IsHighPressureConduit(int currentItemHandle) => AllHighPressureConduitGOHandles.Contains(currentItemHandle);

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
				return _gasMult;
			}
			else if (currentConduitType == ConduitType.Liquid)
			{
				return _liquidMult;
			}
			else if (currentConduitType == ConduitType.Solid)
			{
				return _solidMult;
			}
			return 1;
		}
		public static float GetLogisticConduitMultiplier()
		{
			return _logisticMult;
		}

		public static float GetMaxConduitCapacityAt(int cell, ConduitType type, bool isBridge = false)
		{
			bool isHP = false;
			switch (type)
			{
				case ConduitType.Gas:
					isHP = isBridge ? HPA_GasBridge.Contains(cell) : HPA_Gas.Contains(cell);
					if (isHP)
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
					if (LogisticConduit.HasLogisticConduitAt(cell, isBridge))
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

		public static bool TryGetOutputHPACapacityAt(int cell, ConduitType type, out float capacity, bool bridge = false, bool ignoreValves = true)
		{
			if (!ignoreValves)
			{
				switch (type)
				{
					case ConduitType.Gas:
						if (ValveOutputs_Gas.Contains(cell))
						{
							capacity = CachedRegularConduitCapacity(type);
							return false;
						}
						break;
					case ConduitType.Liquid:
						if (ValveOutputs_Liquid.Contains(cell))
						{
							capacity = CachedRegularConduitCapacity(type);
							return false;
						}
						break;
				}
			}

			capacity = CachedHPAConduitCapacity(type);
			return HasHighPressureConduitAt(cell, type, bridge);
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
		public static Color32 GetColorForConduitType(ConduitType conduitType, bool overlay, bool lowCap = false)
		{
			if (conduitType == ConduitType.Gas)
				return overlay ? _gasFlowOverlay : _gasFlowTint;
			else if (conduitType == ConduitType.Liquid)
				return overlay ? _liquidFlowOverlay : _liquidFlowTint;
			else if (conduitType == ConduitType.Solid)
				return lowCap ? _logisticOverlayTint : _solidOverlayTint;
			return Color.white;
		}


		public static void RegisterConduit(GameObject conduit, ObjectLayer layer = ObjectLayer.NumLayers)
		{
			if (conduit.TryGetComponent<Building>(out var buildingComplete))
			{
				if (layer == ObjectLayer.NumLayers)
					layer = buildingComplete.Def.ObjectLayer;
				switch (layer)
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
		public static void UnregisterConduit(GameObject conduit, ObjectLayer layer = ObjectLayer.NumLayers)
		{
			if (conduit.TryGetComponent<Building>(out var buildingComplete))
			{
				if (layer == ObjectLayer.NumLayers)
					layer = buildingComplete.Def.ObjectLayer;
				switch (layer)
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
		public static void UnregisterHighPressureConduit(GameObject conduitGO, ObjectLayer layer = ObjectLayer.NumLayers)
		{
			if (conduitGO.TryGetComponent<Building>(out var buildingComplete))
			{
				if (layer == ObjectLayer.NumLayers)
					layer = buildingComplete.Def.ObjectLayer;

				AllHighPressureConduitGOHandles.Remove(conduitGO.GetInstanceID());
				switch (layer)
				{
					case ObjectLayer.SolidConduit:
						HPA_Solid.Remove(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.SolidConduitConnection:
						HPA_SolidBridge.Remove(buildingComplete.GetUtilityInputCell());
						HPA_SolidBridge.Remove(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.LiquidConduit:
						HPA_Liquid.Remove(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.LiquidConduitConnection:
						HPA_LiquidBridge.Remove(buildingComplete.GetUtilityInputCell());
						HPA_LiquidBridge.Remove(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.GasConduit:
						HPA_Gas.Remove(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.GasConduitConnection:
						HPA_GasBridge.Remove(buildingComplete.GetUtilityInputCell());
						HPA_GasBridge.Remove(buildingComplete.GetUtilityOutputCell());
						break;
				}
				if (conduitGO.TryGetComponent<HighPressureConduit>(out var hpc) && hpc.InsulateSolidContents)
				{
					//AllInsulatedSolidConduitGOHandles.Remove(conduit.gameObject.GetInstanceID());
					AllInsulatedSolidConduitCells.Remove(buildingComplete.NaturalBuildingCell());
				}
			}
			UnregisterConduit(conduitGO, layer);
		}
		public static void RegisterHighPressureConduit(GameObject conduitGO, ObjectLayer layer = ObjectLayer.NumLayers)
		{
			AllHighPressureConduitGOHandles.Add(conduitGO.gameObject.GetInstanceID());
			if (conduitGO.TryGetComponent<Building>(out var buildingComplete))
			{
				if (layer == ObjectLayer.NumLayers)
					layer = buildingComplete.Def.ObjectLayer;
				switch (layer)
				{
					case ObjectLayer.SolidConduit:
						HPA_Solid.Add(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.SolidConduitConnection:
						HPA_SolidBridge.Add(buildingComplete.GetUtilityInputCell());
						HPA_SolidBridge.Add(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.LiquidConduit:
						HPA_Liquid.Add(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.LiquidConduitConnection:
						HPA_LiquidBridge.Add(buildingComplete.GetUtilityInputCell());
						HPA_LiquidBridge.Add(buildingComplete.GetUtilityOutputCell());
						break;
					case ObjectLayer.GasConduit:
						HPA_Gas.Add(buildingComplete.NaturalBuildingCell());
						break;
					case ObjectLayer.GasConduitConnection:
						HPA_GasBridge.Add(buildingComplete.GetUtilityInputCell());
						HPA_GasBridge.Add(buildingComplete.GetUtilityOutputCell());
						break;
				}
				if (conduitGO.TryGetComponent<HighPressureConduit>(out var hpc) && hpc.InsulateSolidContents)
				{
					AllInsulatedSolidConduitCells.Add(buildingComplete.NaturalBuildingCell());
					//AllInsulatedSolidConduitGOHandles.Add(conduit.gameObject.GetInstanceID());
				}
			}
			RegisterConduit(conduitGO, layer);
		}

		public static void RegisterDecompressionValve(HPA_DecompressionOutput valve)
		{
			switch (valve.ConduitDispenser.conduitType)
			{
				case ConduitType.Gas:
					ValveOutputs_Gas.Add(valve.building.GetUtilityOutputCell());
					break;
				case ConduitType.Liquid:
					ValveOutputs_Liquid.Add(valve.building.GetUtilityOutputCell());
					break;
				default:
					SgtLogger.warning("Tried to register a decompression valve for an unsupported conduit type: " + valve.ConduitDispenser.conduitType);
					return;
			}
		}
		public static void UnregisterDecompressionValve(HPA_DecompressionOutput valve)
		{
			switch (valve.ConduitDispenser.conduitType)
			{
				case ConduitType.Gas:
					ValveOutputs_Gas.Remove(valve.building.GetUtilityOutputCell());
					break;
				case ConduitType.Liquid:
					ValveOutputs_Liquid.Remove(valve.building.GetUtilityOutputCell());
					break;
				default:
					SgtLogger.warning("Tried to register a decompression valve for an unsupported conduit type: " + valve.ConduitDispenser.conduitType);
					return;
			}
		}

		public static void RegisterDynamicSolidConduitDispenser(GameObject solidConduitDispenserGO)
		{
			SgtLogger.l("RegisterDynamicSolidConduitDispenser: Registering " + solidConduitDispenserGO.name + " at cell " + solidConduitDispenserGO.transform.position + " with instance id " + solidConduitDispenserGO.GetInstanceID());
			DynamicSolidConduitDispenserHandles.Add(solidConduitDispenserGO.GetInstanceID());
		}

		public static void UnregisterDynamicSolidConduitDispenser(GameObject solidConduitDispenserGO)
		{
			SgtLogger.l("Unregistering " + solidConduitDispenserGO.name + " at cell " + solidConduitDispenserGO.transform.position + " with instance id " + solidConduitDispenserGO.GetInstanceID());
			DynamicSolidConduitDispenserHandles.Remove(solidConduitDispenserGO.GetInstanceID());
		}
		public static bool IsDynamicSolidConduitDispenser(SolidConduitDispenser dispenser)
		{
			if (dispenser == null || dispenser.gameObject == null)
				return false;
			return DynamicSolidConduitDispenserHandles.Contains(dispenser.gameObject.GetInstanceID());
		}

		static Dictionary<Pickupable, SimTemperatureTransfer> _cachedPickupables = new(2048);

		internal static Pickupable SetInsulatedState(Pickupable pickupable, bool sealAndInsulate)
		{
			if (!_usingInsulatedSolidRails || pickupable == null || pickupable.gameObject == null)
				return pickupable;

			if (_cachedPickupables.TryGetValue(pickupable, out var cached))
			{
				cached.enabled = !sealAndInsulate;
			}
			else
			if (pickupable.TryGetComponent<SimTemperatureTransfer>(out var _tempTransfer))
			{
				_tempTransfer.enabled = !sealAndInsulate;
				_cachedPickupables[pickupable] = _tempTransfer;
			}

			if (sealAndInsulate)
				pickupable.KPrefabID.AddTag(GameTags.Sealed);
			else
				pickupable.KPrefabID.RemoveTag(GameTags.Sealed);

			return pickupable;
		}

		internal static bool IsInsulatedRail(int cell)
		{
			return _usingInsulatedSolidRails && AllInsulatedSolidConduitCells.Contains(cell);
		}
		internal static void RegisterPickupable(Pickupable instance)
		{
			if (!_usingInsulatedSolidRails || instance == null || instance.gameObject == null || instance.deleteOffGrid)
				return;
			if (_cachedPickupables.ContainsKey(instance))
				return;
			if (instance.TryGetComponent<SimTemperatureTransfer>(out var _tempTransfer))
			{
				_cachedPickupables[instance] = _tempTransfer;
			}
			else
			{
				SgtLogger.l($"Failed to register pickupable {instance.GetProperName()} at cell {Grid.PosToCell(instance)}");
			}
		}

		internal static void CleanupPickupable(Pickupable instance)
		{
			_cachedPickupables.Remove(instance);
		}
		public static Pickupable DumpItem(Pickupable pickupable, float mass, float targetMass, int dumpCell, GameObject target)
		{
			float amountToDump = (mass - targetMass);
			var droppedExcess = pickupable.Take(amountToDump);
			///drop excess mass
			SolidConduit.GetFlowManager().DumpPickupable(droppedExcess);
			droppedExcess.transform.SetPosition(Grid.CellToPosCCC(dumpCell, Grid.SceneLayer.Ore));
			HighPressureConduitEventHandler.ScheduleDropNotification(target, (int)mass, (int)targetMass);
			return pickupable;
		}
	}
}
