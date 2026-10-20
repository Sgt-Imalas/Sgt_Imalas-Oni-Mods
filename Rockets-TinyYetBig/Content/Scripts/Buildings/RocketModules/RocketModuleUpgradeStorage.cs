using KSerialization;
using Rockets_TinyYetBig.Content.ModDb;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class RocketModuleUpgradeStorage : Workable
	{
		[MyCmpReq] Building building;
		[MyCmpReq] KSelectable selectable;

		[Serialize]
		public List<RocketModuleUpgradeInstance> StoredModuleUpgrades = [];


		[Serialize]
		List<string> PendingDeconstructs = new List<string>();
		[Serialize]
		string currentDeconstruct = null;
		public Chore deconstructChore;

		HashSet<RocketModuleUpgrade> srcUpgrades = [];
		HashSet<RocketModuleUpgrade> allowedUpgrades = [];

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.showProgressBar = true;
			InitWorkable();
		}
		void InitWorkable()
		{
			this.faceTargetWhenWorking = true;
			this.synchronizeAnims = false;
			this.workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
			this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
			this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			this.minimumAttributeMultiplier = 0.75f;
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
			this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			this.multitoolContext = (HashedString)"build";
			this.multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
			this.workingPstComplete = null;
			this.workingPstFailed = null;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.SetOffsetTable(OffsetGroups.InvertedWideTable);
			DetermineAllowedUpgrades();
			LoadUpgrades();
			AdvanceDeconstructQueue();
		}

		void DetermineAllowedUpgrades()
		{
			allowedUpgrades.Clear();
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

		public bool HasCompatibleUpgrades() => allowedUpgrades.Count > 0;
		public HashSet<RocketModuleUpgrade> GetInstalledUpgrades() => srcUpgrades;

		void LoadUpgrades()
		{
			SgtLogger.l("Upgrade Count: " + StoredModuleUpgrades.Count);
			if (!Config.Instance.EnableRocketModuleUpgrades)
				return;
			foreach (RocketModuleUpgradeInstance upgrade in StoredModuleUpgrades)
			{

				SgtLogger.l("Stored: " + upgrade.UpgradeId + " cost: " + string.Join(',', upgrade.SerializedMaterials) + " -> " + string.Join(',', upgrade.SerializedAmounts));
				RocketModuleUpgrade src = upgrade.GetSource();
				srcUpgrades.Add(src);
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
			srcUpgrades.Clear();
		}
		internal void RemoveUpgrade(string upgradeId)
		{
			var existingUpgrade = StoredModuleUpgrades.FirstOrDefault(u => u.UpgradeId == upgradeId);
			StoredModuleUpgrades.RemoveAll(u => u.UpgradeId == upgradeId);
			if (!ModuleUpgradeDatabase.TryGetUpgrade(upgradeId, out var upgrade))
			{
				SgtLogger.error("Tried removing nonexisting upgrade: " + upgradeId);
				return;
			}
			var pos = Grid.CellToPosCCC(this.NaturalBuildingCell(), Grid.SceneLayer.Ore);
			upgrade.OnUpgradeRemoved(gameObject);
			existingUpgrade.Dismantle(pos);
			srcUpgrades.Remove(existingUpgrade.GetSource());
			this.Trigger((int)ModAssets.Hashes.ModuleUpgradeChanged, upgrade);
			this.Trigger((int)ModAssets.Hashes.ModuleUpgradeRemoved, upgrade);
		}
		internal void AddUpgrade(RocketModuleUpgradeInstance upgradeInstance)
		{
			StoredModuleUpgrades.Add(upgradeInstance);
			var src = upgradeInstance.GetSource();
			srcUpgrades.Add(src);
			this.Trigger((int)ModAssets.Hashes.ModuleUpgradeChanged, src);
			this.Trigger((int)ModAssets.Hashes.ModuleUpgradeAdded, src);
		}

		#region deconstructUpgrade

		public bool IsDeconstructPending(RocketModuleUpgrade upgrade) => PendingDeconstructs.Contains(upgrade.ID);
		public void TogglePendingUpgradeRemoval(string upgradeId)
		{
			if (PendingDeconstructs.Contains(upgradeId))
			{
				CancelUpgradeRemoval(upgradeId);
			}
			else
			{
				PendingDeconstructs.Add(upgradeId);
				AdvanceDeconstructQueue();
			}
		}
		void AdvanceDeconstructQueue()
		{
			if (PendingDeconstructs.Count == 0)
				return;

			string upgradeId = PendingDeconstructs[0];
			if (!ModuleUpgradeDatabase.TryGetUpgrade(upgradeId, out var upgrade))
			{
				SgtLogger.error(upgradeId + " not a valid module upgrade!");
				PendingDeconstructs.Remove(upgradeId);
				return;
			}
			if (this.deconstructChore != null)
				return;
			this.SetWorkTime(upgrade.WorkTime * 0.5f);

			Prioritizable.AddRef(this.gameObject);
			this.deconstructChore = new WorkChore<RocketModuleUpgradeStorage>(Db.Get().ChoreTypes.Deconstruct, this, only_when_operational: false, is_preemptable: true, ignore_building_assignment: true);
			selectable.AddStatusItem(ModStatusItems.RTB_PendingUpgradeRemoval, upgrade);
			this.currentDeconstruct = upgradeId;

			if (DebugHandler.InstantBuildMode)
				this.OnCompleteWork(null);
		}

		void CancelUpgradeRemoval(string upgradeId)
		{
			PendingDeconstructs.Remove(upgradeId);
			if (currentDeconstruct == upgradeId)
			{
				ClearChore();
			}
			selectable.RemoveStatusItem(ModStatusItems.RTB_PendingUpgradeRemoval);
			this.ShowProgressBar(false);
			AdvanceDeconstructQueue();
		}
		public override void OnCompleteWork(WorkerBase worker)
		{
			PlaySound();
			base.OnCompleteWork(worker);
			selectable.RemoveStatusItem(ModStatusItems.RTB_PendingUpgradeRemoval);
			RemoveUpgrade(currentDeconstruct);
			PendingDeconstructs.Remove(currentDeconstruct);
			ClearChore();
			AdvanceDeconstructQueue();
		}

		void ClearChore()
		{
			if (deconstructChore != null)
				deconstructChore.Cancel("deconstruct canceled");
			deconstructChore = null;
			currentDeconstruct = null;
		}
		public override void OnStartWork(WorkerBase worker)
		{
			this.ShowProgressBar(true);
			base.OnStartWork(worker);
		}
		void PlaySound()
		{
			string sound = GlobalAssets.GetSound("Finish_Deconstruction_small");
			if (sound == null)
				return;
			KMonoBehaviour.PlaySound3DAtLocation(sound, this.gameObject.transform.GetPosition());
		}


		#endregion
		//internal static bool GetFromAttachable(BuildingAttachPoint attachable, out RocketModuleUpgradeStorage storage) => storages.TryGetValue(attachable, out storage);
	}
}
