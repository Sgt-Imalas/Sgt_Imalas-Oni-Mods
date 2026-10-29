using KSerialization;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.ModAssets;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{
	internal class AutomaticGantry : KMonoBehaviour
	{
		[MyCmpReq] Gantry gantry;
		[MyCmpReq] OccupyArea area;
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] Operational operational;
		[MyCmpReq] BuildingHP hp;
		readonly HashSet<int> columns = [];
		private int highestGantryRow = int.MinValue;
		private readonly Dictionary<int, bool> _pendingRockets = [];
		private readonly List<int> _gameSubs = [];
		private int _worldId;
		private bool _forcedRetracted;
		private bool _breakFloors;
		int _turnOffActiveHandle = -1;

		bool HasPendingExtends => _pendingRockets.Count > 0;

		public override void OnSpawn()
		{
			_worldId = this.GetMyWorldId();
			base.OnSpawn();
			CacheGantryOccupyingColumns();

			_breakFloors = Config.Instance.GantryNoFloorWhenBroken;
			bool autoGantries = Config.Instance.AutoGantries;

			if(_breakFloors ||  autoGantries)
			{
				this.Sub(GameHashes.TagsChanged, OnTagChanged);
			}
			if (_breakFloors)
			{
				this.Sub(GameHashes.BuildingBroken, OnBroken);
				this.Sub(GameHashes.BuildingFullyRepaired, OnRepaired);
			}
			if (autoGantries)
			{
				_gameSubs.Add(Game.Instance.Subscribe(Hashes.RocketLandingInitialized, AutoRetract_Landing_Start));
				_gameSubs.Add(Game.Instance.Subscribe((int)GameHashes.RocketLanded, AutoRetract_Landing_End));
				_gameSubs.Add(Game.Instance.Subscribe((int)GameHashes.RocketLaunched, AutoRetract_Launch_Start));
				_gameSubs.Add(Game.Instance.Subscribe(Hashes.RocketLaunchFinalized, AutoRetract_Launch_End));
				this.Sub(GameHashes.OperationalChanged, AutoRetract_PowerResume);
			}
			RecoverGantryState(_breakFloors);
		}
		public override void OnCleanUp()
		{
			foreach (int i in _gameSubs)
				Game.Instance.Unsubscribe(i);
			_gameSubs.Clear();
			this.UnsubAll();
			if (_turnOffActiveHandle != -1)
				Unsubscribe(_turnOffActiveHandle);
			base.OnCleanUp();
		}
		void RecoverGantryState(bool brokenSettingActive)
		{
			var smi = gantry.smi;
			if (smi == null)
				return;

			if (smi.GetCurrentState() == smi.sm.root)
			{
				_forcedRetracted = true;
				ToggleGantrySMI(true);
			}
			else if (brokenSettingActive && hp.IsBroken && smi.IsInsideState(smi.sm.extended))
			{
				gantry.SetWalkable(false);
			}
		}
		void CacheGantryOccupyingColumns()
		{
			columns.Clear();

			int src = this.NaturalBuildingCell();
			for (int j = 0; j < area.OccupiedCellsOffsets.Length; j++)
			{
				CellOffset offset = area.OccupiedCellsOffsets[j];
				int cell = Grid.OffsetCell(src, offset);
				columns.Add(Grid.CellColumn(cell));
				int row = Grid.CellRow(cell);
				if (highestGantryRow < row)
					highestGantryRow = row;
			}
		}
		void AutoRetract_PowerResume(object _)
		{
			if (HasPendingExtends && operational.IsOperational)
				ToggleGantrySMI(false);
		}
		void AutoRetract_Launch_Start(object data)
		{
			//SgtLogger.l("Launching Start");
			StartAutoRetraction(data, true);
		}
		void AutoRetract_Launch_End(object data)
		{
			//SgtLogger.l("Launching End");
			FinishAutoRetraction(data, true);
		}
		void AutoRetract_Landing_Start(object data)
		{
			//SgtLogger.l("Landing Start");
			StartAutoRetraction(data, false);
		}
		void AutoRetract_Landing_End(object data)
		{
			//SgtLogger.l("Landing End");
			FinishAutoRetraction(data, false);
		}

		void StartAutoRetraction(object spacefarer, bool isLaunching)
		{
			if (spacefarer is not GameObject spacefarerGO || !FlightPathOverlapsGantry(spacefarerGO))
				return;

			int instanceID = spacefarerGO.GetInstanceID();
			_pendingRockets[instanceID] = isLaunching;

			if (operational.IsOperational)
				ToggleGantrySMI(false);
		}

		void FinishAutoRetraction(object spacefarer, bool isLaunching)
		{
			if (spacefarer is not GameObject spacefarerGO)
				return;

			int instanceID = spacefarerGO.GetInstanceID();
			if (!_pendingRockets.TryGetValue(instanceID, out bool wasLaunching) || wasLaunching != isLaunching)
				return;
			_pendingRockets.Remove(instanceID);
			ToggleGantrySMI(true);
		}


		bool FlightPathOverlapsGantry(GameObject spacefarerGO)
		{
			if (_worldId != spacefarerGO.GetMyWorldId())
				return false;

			if (!spacefarerGO.TryGetComponent<OccupyArea>(out var moduleArea) || !spacefarerGO.TryGetComponent<RocketModuleCluster>(out var module))
				return false;

			bool hasOverlap = false;
			int src = moduleArea.NaturalBuildingCell();
			int lowestModuleRow = Grid.CellRow(module.CraftInterface.bottomModule.Get().NaturalBuildingCell());

			for (int j = 0; j < moduleArea.OccupiedCellsOffsets.Length; j++)
			{
				CellOffset offset = moduleArea.OccupiedCellsOffsets[j];
				int cell = Grid.OffsetCell(src, offset);
				if (columns.Contains(Grid.CellColumn(cell)) && lowestModuleRow <= highestGantryRow)
				{
					hasOverlap = true;
					break;
				}
			}
			return hasOverlap;
		}

		void ToggleGantrySMI(bool on)
		{
			var smi = gantry.smi;
			if (!on)
			{
				if (_forcedRetracted || !GantryStateExtended())
					return;
				_forcedRetracted = true;
				smi.GoTo(smi.sm.root);
				kbac.Play("off_pre");
				kbac.Queue("off");
				smi.SetActive(true);
				ClearTurnOffHandle();
				_turnOffActiveHandle = Subscribe((int)GameHashes.AnimQueueComplete, TurnActiveOff);
			}
			else
			{
				if (!_forcedRetracted || HasPendingExtends)
					return;
				_forcedRetracted = false;
				ClearTurnOffHandle();
				smi.SetActive(false);
				smi.GoTo(smi.sm.should_extend.Get(smi) ? smi.sm.extended_pre : smi.sm.retracted);
			}
		}
		void ClearTurnOffHandle()
		{
			if (_turnOffActiveHandle == -1)
				return;
			Unsubscribe(_turnOffActiveHandle);
			_turnOffActiveHandle = -1;
		}
		void TurnActiveOff(object _)
		{
			ClearTurnOffHandle();
			gantry.smi?.SetActive(false);
		}

		bool GantryStateExtended()
		{
			var smi = gantry.smi;
			if (smi == null)
				return false;

			return smi.IsInsideState(smi.sm.extended) || smi.IsInsideState(smi.sm.extended_pre);
		}

		void OnRepaired(object _)
		{
			ToggleGantryFloor(true);
		}
		void OnBroken(object _)
		{
			ToggleGantryFloor(false);
		}
		void OnTagChanged(object _)
		{
			var smi = gantry.smi;
			if (smi == null || !smi.IsInsideState(smi.sm.extended))
				return;

			if (HasPendingExtends && operational.IsOperational)
			{
				ToggleGantrySMI(false);
				if (_forcedRetracted)
					return;
			}

			if (hp.IsBroken && _breakFloors)
				gantry.SetWalkable(false);
		}
		void ToggleGantryFloor(bool on)
		{
			var smi = gantry.smi;
			if (smi != null && smi.IsInsideState(smi.sm.extended))
				gantry.SetWalkable(on);
		}

	}
}
