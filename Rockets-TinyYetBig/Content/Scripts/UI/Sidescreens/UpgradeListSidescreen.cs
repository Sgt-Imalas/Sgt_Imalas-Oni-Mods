using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using Rockets_TinyYetBig.Content.Scripts.UI.UIComponents;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.UI;

namespace Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens
{
	internal class UpgradeListSidescreen : SideScreenContent
	{
		public override void OnSpawn()
		{
			base.OnSpawn();

			ConnectReference();
		}

		protected RocketModuleUpgradeStorage target;

		protected GameObject EntryPrefab;
		protected GameObject EntryContainer;
		//[SerializeField]
		//private GameObject noChannelRow;
		private Dictionary<RocketModuleUpgrade, RocketUpgradeEntry> UpgradeEntries = new();

		private List<int> refreshHandles = new List<int>();


		public override float GetSortKey() => 15f;
		public override bool IsValidForTarget(GameObject target) => target.TryGetComponent<RocketModuleUpgradeStorage>(out var upgradeStore) && upgradeStore.HasCompatibleUpgrades();
		public override void ClearTarget()
		{
			foreach (int id in refreshHandles)
				target.Unsubscribe(id);
			refreshHandles.Clear();
			target = null;
			base.ClearTarget();
		}

		public override void SetTarget(GameObject target)
		{
			ConnectReference();

			if (target != null)
			{
				foreach (int id in refreshHandles)
					target.Unsubscribe(id);
				refreshHandles.Clear();
			}
			base.SetTarget(target);
			target.TryGetComponent(out this.target);


			//refreshHandles.Add(target.Subscribe((int)GameHashes.ClusterDestinationChanged, new Action<object>(RefreshAll)));
			//refreshHandles.Add(target.Subscribe((int)GameHashes.ClusterLocationChanged, new Action<object>(RefreshAll)));
			refreshHandles.Add(target.Subscribe((int)ModAssets.Hashes.ModuleUpgradeChanged, Refresh));
			Refresh(null);
		}

		public override string GetTitle()
		{
			return UPGRADELIST_SIDESCREEN.TITLE.TITLETEXT;
		}

		bool refsConnected = false;
		void ConnectReference()
		{
			if (refsConnected)
				return;
			refsConnected = true;

			EntryPrefab = transform.Find("Content/ScrollRectContainer/Prefab").gameObject;
			EntryContainer = transform.Find("Content/ScrollRectContainer").gameObject;
			EntryPrefab.gameObject.SetActive(false);
		}

		private void Refresh(object _)
		{
			foreach (var uirow in UpgradeEntries)
			{
				uirow.Value.gameObject.SetActive(false);
			}
			if (target == null)
			{
				return;
			}
			foreach (var upgrade in target.GetInstalledUpgrades())
			{
				AddOrGetUiEntry(upgrade).RefreshDeconstructState(false);
			}
		}
		RocketUpgradeEntry AddOrGetUiEntry(RocketModuleUpgrade upgrade)
		{
			if (UpgradeEntries.TryGetValue(upgrade, out var value))
			{
				value.gameObject.SetActive(true);
				value.transform.SetAsLastSibling();
				return value;
			}

			value = Util.KInstantiateUI(EntryPrefab, EntryContainer).gameObject.AddOrGet<RocketUpgradeEntry>();
			value.gameObject.SetActive(true);
			value.SetUpgrade(upgrade);
			value.DeconstructPending = () => target.IsDeconstructPending(upgrade);
			value.ToggleDeconstruct = () => ToggleCurrentTargetUpgradeDeconstruct(upgrade);
			UpgradeEntries[upgrade] = value;
			return value;
		}
		void ToggleCurrentTargetUpgradeDeconstruct(RocketModuleUpgrade upgrade)
		{
			if (!target.HasUpgrade(upgrade))
				return;
			target.TogglePendingUpgradeRemoval(upgrade.ID);
		}
	}
}
