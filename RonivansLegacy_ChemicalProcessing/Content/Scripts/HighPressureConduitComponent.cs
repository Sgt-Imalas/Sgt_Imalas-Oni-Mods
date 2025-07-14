using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static FlowUtilityNetwork;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	public class HighPressureOutput : HighPressureConduitComponent
	{
		public HighPressureOutput()
		{
			RequiresHighPressureInput = false;
			RequiresHighPressureOutput = true;
		}
	}
	public class HighPressureInput : HighPressureConduitComponent
	{
		public HighPressureInput()
		{
			RequiresHighPressureInput = true;
			RequiresHighPressureOutput = false;
		}
	}
	public class HighPressureConduit : HighPressureConduitComponent
	{
		public HighPressureConduit()
		{
			RequiresHighPressureInput = false;
			RequiresHighPressureOutput = false;
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}
		protected override void CheckRequirements()
		{
			//skip on high pressure conduits
		}
	}

	public class HighPressureConduitComponent : KMonoBehaviour, ISim200ms
	{
		public static Dictionary<int, Dictionary<int, HighPressureConduitComponent>> ConduitsByLayer = new()
		{
			{ (int)ObjectLayer.GasConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.GasConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.Building,new Dictionary<int, HighPressureConduitComponent>() },

		};
		public static void ClearEverything()
		{
			CancelPendingPressureDamage();
			AllConduits.Clear();
			AllConduitGOs.Clear();
			ConduitsByLayer = new()
			{
			{ (int)ObjectLayer.GasConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduit,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.GasConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.LiquidConduitConnection,new Dictionary<int, HighPressureConduitComponent>() },
			{ (int)ObjectLayer.Building,new Dictionary<int, HighPressureConduitComponent>() },
			};
		}


		public static HashSet<HighPressureConduitComponent> AllConduits = [];
		public static HashSet<GameObject> AllConduitGOs = [];

		private static readonly Operational.Flag highPressureInputConnected = new Operational.Flag("highPressureInputConnected", Operational.Flag.Type.Requirement);
		private static readonly Operational.Flag highPressureOutputConnected = new Operational.Flag("highPressureOutputConnected", Operational.Flag.Type.Requirement);

		[MyCmpReq]
		KPrefabID prefab;
		[MyCmpReq]
		KSelectable selectable;
		[MyCmpGet]
		public BuildingComplete buildingComplete;
		[MyCmpGet]
		public Operational operational;
		[MyCmpGet]
		public ConduitConsumer consumer;
		[MyCmpGet]
		public ConduitDispenser dispenser;

		public bool RequiresHighPressureInput = true, RequiresHighPressureOutput = true;
		private int consumerCell = -1, dispenserCell = -1;
		private ConduitType inputType = ConduitType.None, outputType = ConduitType.None;
		private bool previouslyConnectedHPInput = true, previouslyConnectedHPOutput = true;

		public override void OnSpawn()
		{
			if (!ConduitsByLayer.ContainsKey((int)buildingComplete.Def.ObjectLayer))
				ConduitsByLayer.Add((int)buildingComplete.Def.ObjectLayer, new());


			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Min()] = this;
			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer][buildingComplete.PlacementCells.Max()] = this;
			SgtLogger.l("Registering bounds: min: " + buildingComplete.PlacementCells.Min() + ", max: " + buildingComplete.PlacementCells.Max());

			AllConduitGOs.Add(this.gameObject);
			AllConduits.Add(this);
			CacheConduitCells();
			base.OnSpawn();
			CheckRequirements();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer].Remove(buildingComplete.PlacementCells.Min());
			ConduitsByLayer[(int)buildingComplete.Def.ObjectLayer].Remove(buildingComplete.PlacementCells.Max());
			AllConduitGOs.Remove(this.gameObject);
			AllConduits.Remove(this);
		}

		void CacheConduitCells()
		{
			if (RequiresHighPressureInput && consumer != null)
			{
				consumerCell = consumer.GetInputCell(consumer.conduitType);
				inputType = consumer.conduitType;
			}
			if (RequiresHighPressureOutput && dispenser != null)
			{
				dispenserCell = dispenser.GetOutputCell(dispenser.conduitType);
				outputType = dispenser.conduitType;
			}
		}
		public void Sim200ms(float dt) => CheckRequirements();

		protected virtual void CheckRequirements()
		{
			if (!RequiresHighPressureInput && !RequiresHighPressureOutput)
				return;

			if (RequiresHighPressureInput && consumerCell > 0)
			{
				bool hasHighPressureInput = !consumer.enabled || consumer.IsConnected && HasHighPressureConduitAt(consumerCell, inputType, out _);

				if (previouslyConnectedHPInput != hasHighPressureInput)
				{
					previouslyConnectedHPInput = hasHighPressureInput;
					StatusItem status_item = null;
					switch (inputType)
					{
						case ConduitType.Gas:
							status_item = StatusItemsDatabase.HPA_NeedGasIn;
							break;
						case ConduitType.Liquid:
							status_item = StatusItemsDatabase.HPA_NeedLiquidIn;
							break;
					}
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureInput);
					this.operational.SetFlag(highPressureInputConnected, hasHighPressureInput);
				}
			}
			if (RequiresHighPressureOutput && dispenserCell > 0)
			{
				bool hasHighPressureOutput = !dispenser.enabled || dispenser.IsConnected && HasHighPressureConduitAt(dispenserCell, outputType, out _);

				if (previouslyConnectedHPOutput != hasHighPressureOutput)
				{
					previouslyConnectedHPOutput = hasHighPressureOutput;
					StatusItem status_item = null;
					switch (outputType)
					{
						case ConduitType.Gas:
							status_item = StatusItemsDatabase.HPA_NeedGasOut;
							break;
						case ConduitType.Liquid:
							status_item = StatusItemsDatabase.HPA_NeedLiquidOut;
							break;
					}
					if (status_item != null)
						this.selectable.ToggleStatusItem(status_item, !hasHighPressureOutput);
					this.operational.SetFlag(highPressureOutputConnected, hasHighPressureOutput);
				}
			}
		}

		public static float GetMaxCapacityAt(int cell, ConduitType type, out GameObject go)
		{
			if (type == ConduitType.Gas)
			{
				if (ConduitsByLayer[(int)ObjectLayer.GasConduit].TryGetValue(cell, out var cmp))
				{
					go = cmp.gameObject;
					return CachedHPAConduitCapacity(type);
				}
				else
				{
					go = GetConduitAt(cell, type);
					return CachedRegularConduitCapacity(type);
				}
			}
			else if (type == ConduitType.Liquid)
			{
				if (ConduitsByLayer[(int)ObjectLayer.LiquidConduit].TryGetValue(cell, out var cmp))
				{
					go = cmp.gameObject;
					return CachedHPAConduitCapacity(type);
				}
				else
				{
					go = GetConduitAt(cell, type);
					return CachedRegularConduitCapacity(type);
				}
			}
			throw new NotImplementedException("Tried getting capacity from invalid conduit type");
		}

		static GameObject GetConduitAt(int cell, ConduitType type, bool isBridge = false)
		{
			if (type == ConduitType.Gas)
			{
				return Grid.Objects[cell, isBridge ? (int)ObjectLayer.GasConduitConnection : (int)ObjectLayer.GasConduit];
			}
			else if (type == ConduitType.Liquid)
			{
				return Grid.Objects[cell, isBridge ?  (int)ObjectLayer.LiquidConduitConnection : (int)ObjectLayer.LiquidConduit];
			}
			throw new NotImplementedException("Tried getting invalid conduit type");
		}

		public static bool HasHighPressureConduitAt(int cell, ConduitType type, out float capacity, bool bridge = false)
		{
			capacity = CachedHPAConduitCapacity(type);
			if (type == ConduitType.Gas)
			{
				return ConduitsByLayer[bridge ? (int)ObjectLayer.GasConduitConnection : (int)ObjectLayer.GasConduit].ContainsKey(cell);
			}
			else if (type == ConduitType.Liquid)
			{
				return ConduitsByLayer[bridge ? (int)ObjectLayer.LiquidConduitConnection : (int)ObjectLayer.LiquidConduit].ContainsKey(cell);
			}
			return false;
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
			return false;
		}
		public static bool HasHighPressureConduitAt(int cell, ConduitType type, bool showContents, out Color32 tint)
		{
			if (type == ConduitType.Gas && ConduitsByLayer[(int)ObjectLayer.GasConduit].ContainsKey(cell))
			{
				tint = GetColorForConduitType(type, showContents);
				return true;
			}
			else if (type == ConduitType.Liquid && ConduitsByLayer[(int)ObjectLayer.LiquidConduit].ContainsKey(cell))
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
			return Color.white;
		}

		//cache this to avoid calling config.instance each time
		private static bool _capInit = false;
		private static float _gasCap_hp = -1, _liquidCap_hp = -1;
		private static float _gasCap_reg = -1, _liquidCap_reg = -1;
		private static Color32 _gasFlowOverlay = new Color32(169, 209, 251, 0), _gasFlowTint = new Color32(176, 176, 176, 255),
			_liquidFlowOverlay = new Color32(92, 144, 121, 0), _liquidFlowTint = new Color32(92, 144, 121, 255);


		static float CachedHPAConduitCapacity(ConduitType type)
		{
			InitCache();
			switch (type)
			{
				case ConduitType.Gas:
					return _gasCap_hp;
				case ConduitType.Liquid:
					return _liquidCap_hp;
			}
			return 1;
		}
		static float CachedRegularConduitCapacity(ConduitType type)
		{
			InitCache();
			switch (type)
			{
				case ConduitType.Gas:
					return _gasCap_reg;
				case ConduitType.Liquid:
					return _liquidCap_reg;
			}
			return 1;
		}
		private static void InitCache()
		{
			if (!_capInit)
			{
				_capInit = true;
				_gasCap_hp = Config.Instance.HPA_Capacity_Gas;
				_liquidCap_hp = Config.Instance.HPA_Capacity_Liquid;
				_gasCap_reg = Conduit.GetFlowManager(ConduitType.Gas).MaxMass;
				_liquidCap_reg = Conduit.GetFlowManager(ConduitType.Liquid).MaxMass;
			}
		}


		static List<GameObject> DamageTargets = new();

		static SchedulerHandle? handle = null;
		internal static void ScheduleForDamage(GameObject receiver)
		{
			DamageTargets.Add(receiver);
			if (handle != null)
				return;

			GameScheduler.Instance.ScheduleNextFrame("DamageAccumulation", (_) => DealAccumulatedDamage());
		}

		private static BuildingHP.DamageSourceInfo? ConduitPressureDamage = null;
		public static BuildingHP.DamageSourceInfo GetPressureDamageSource()
		{
			if(ConduitPressureDamage == null)
			{
				ConduitPressureDamage = new()
				{
					damage = 1,
					source = global::STRINGS.BUILDINGS.DAMAGESOURCES.LIQUID_PRESSURE,
					popString = global::STRINGS.UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LIQUID_PRESSURE
				};
			}
			return ConduitPressureDamage.Value;
		}


		static void DealAccumulatedDamage()
		{
			SgtLogger.l(DamageTargets.Count + " items queued for damage");

			for (int i = DamageTargets.Count - 1; i >= 0; i--)
			{
				SgtLogger.l("index: " + i);
				var target = DamageTargets[i];
				SgtLogger.l("Target: "+target);
				target.Trigger((int)GameHashes.DoBuildingDamage, GetPressureDamageSource());
			}
			handle = null;
			DamageTargets.Clear();
		}

		internal static void CancelPendingPressureDamage()
		{
			DamageTargets.Clear();
		}

		internal static void PressureDamageHandling(GameObject receiver, float sentMass, float receiverMax)
		{
			//33% chance to damage the receiver when sender has double its capacity if the receiver is not a bridge
			//if receiver is a bridge, 33% to damage the bridge if the sender's contents are above the bridge's capacity at all
			if (sentMass >= receiverMax * 2f && UnityEngine.Random.Range(0f, 1f) < 0.33f)
			{
				//This damage CANNOT be dealt immediately, or it will cause the game to crash. This code execution occurs during an UpdateNetworkTask execution and does not seem to support executing Triggers
				//The damage will instead be queued and dealt by a scheduler on the next tick
				ScheduleForDamage(receiver);
			}
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

		public static float GetConduitMultiplier(ConduitType currentConduitType) 
		{
			if (currentConduitType == ConduitType.Gas)
			{
				return (Config.Instance.HPA_Capacity_Gas / ConduitFlow.MAX_GAS_MASS);
			}
			else if (currentConduitType == ConduitType.Liquid)
			{
				return (Config.Instance.HPA_Capacity_Liquid / ConduitFlow.MAX_LIQUID_MASS);
			}
			return 1;
		}
	}
}
