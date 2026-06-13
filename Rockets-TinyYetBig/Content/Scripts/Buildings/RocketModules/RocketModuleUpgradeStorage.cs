using KSerialization;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class RocketModuleUpgradeStorage : KMonoBehaviour
	{
		[MyCmpAdd] BuildingAttachPoint attachPoint;
		[MyCmpReq] Building building;

		[Serialize]
		public List<RocketModuleUpgradeInstance> StoredModuleUpgrades = [];
		HashSet<RocketModuleUpgrade> srcUpgrades = [];
		HashSet<RocketModuleUpgrade> allowedUpgrades = [];
		static Dictionary<BuildingAttachPoint, RocketModuleUpgradeStorage> storages = [];

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		void DetachOtherModulesFromUpgradeSlot()
		{
			for (int i = 0; i < attachPoint.points.Length; i++)
			{
				var hardPoint = attachPoint.points[i];

				if (hardPoint.attachableType != ModAssets.Tags.AttachmentSlotRocketModuleUpgrades)
					continue;
				if (hardPoint.attachedBuilding != null && hardPoint.attachedBuilding.gameObject.TryGetComponent<RocketModuleCluster>(out var rocket))
				{
					SgtLogger.warning("Module upgrade slot on " + gameObject.name + " tried to connect to other rocket module: " + rocket.name);
					attachPoint.points[i].attachedBuilding = null;
				}
			}
		}
		void AddAttachmentSlot()
		{
			if (!attachPoint.points.Any(point => point.attachableType == ModAssets.Tags.AttachmentSlotRocketModuleUpgrades))
			{
				int middle = Mathf.FloorToInt(Mathf.Max(building.Def.HeightInCells - 1, 1) / 2f);
				attachPoint.points = attachPoint.points.Append(new BuildingAttachPoint.HardPoint(new CellOffset(0, middle), ModAssets.Tags.AttachmentSlotRocketModuleUpgrades, null));
			}
			attachPoint.TryAttachEmptyHardpoints();
			DetachOtherModulesFromUpgradeSlot();
		}

		public override void OnSpawn()
		{
			if(Config.Instance.RocketModuleUpgrades)
				AddAttachmentSlot();
			storages[attachPoint] = this;
			base.OnSpawn();
			DetermineAllowedUpgrades();
			LoadUpgrades();
		}
		public override void OnCleanUp()
		{
			storages.Remove(attachPoint);
			base.OnCleanUp();
		}

		void DetermineAllowedUpgrades()
		{
			if (gameObject.TryGetComponent<CargoBayCluster>(out var cargobay) && cargobay.storageType != CargoBay.CargoType.Entities)
			{
				allowedUpgrades.Add(ModuleUpgradeDatabase.CargoBayFilter);
			}
		}

		public bool UpgradeAllowed(string upgradeId, out string failure)
		{
			failure = string.Empty;
			if (!ModuleUpgradeDatabase.TryGetUpgrade(upgradeId, out var upgrade))
			{
				failure = STRINGS.UI.RTB_ROCKET_UPGRADES.FAILURE_REASONS.INVALID;
				return false;
			}
			return UpgradeAllowed(upgrade, out failure);
		}
		public bool UpgradeAllowed(RocketModuleUpgrade upgrade, out string failure)
		{
			if (srcUpgrades.Contains(upgrade))
			{
				failure = STRINGS.UI.RTB_ROCKET_UPGRADES.FAILURE_REASONS.DUPLICATE;
				return false;
			}
			if (!allowedUpgrades.Contains(upgrade))
			{
				failure = STRINGS.UI.RTB_ROCKET_UPGRADES.FAILURE_REASONS.INCOMPATIBLE;
				return false;
			}
			failure = string.Empty;
			return true;
		}

		void LoadUpgrades()
		{
			SgtLogger.l("Upgrade Count: " + StoredModuleUpgrades.Count);
			if (!Config.Instance.RocketModuleUpgrades)
				return;
			foreach (RocketModuleUpgradeInstance upgrade in StoredModuleUpgrades)
			{

				SgtLogger.l("Stored: " + upgrade.UpgradeId + " cost: " + string.Join(',', upgrade.SerializedMaterials) + " -> " + string.Join(',', upgrade.SerializedAmounts));
				var src = upgrade.GetSource();
				srcUpgrades.Add(upgrade);
				src.OnUpgradeAdded(gameObject);
			}
		}

		internal bool HasUpgrade(RocketModuleUpgrade cargoBayFilter)
		{
			return srcUpgrades.Contains(cargoBayFilter);
		}

		internal void DismantleAllUpgrades()
		{
			var pos = Grid.CellToPosCCC(this.NaturalBuildingCell(), Grid.SceneLayer.Ore);
			foreach (var upgrade in StoredModuleUpgrades)
			{
				upgrade.GetSource().OnUpgradeRemoved(gameObject);
				upgrade.Dismantle(pos);
			}
			StoredModuleUpgrades.Clear();
		}

		internal void AddUpgrade(RocketModuleUpgradeInstance upgradeInstance)
		{
			StoredModuleUpgrades.Add(upgradeInstance);
			srcUpgrades.Add(upgradeInstance.GetSource());
		}

		internal static bool GetFromAttachable(BuildingAttachPoint attachable, out RocketModuleUpgradeStorage storage) => storages.TryGetValue(attachable, out storage);
	}
}
