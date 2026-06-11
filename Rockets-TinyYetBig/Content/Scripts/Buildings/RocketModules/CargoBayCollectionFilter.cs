using KSerialization;
using Rockets_TinyYetBig.Content.ModDb;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class CargoBayCollectionFilter : KMonoBehaviour, IDisableableCheckboxControl
	{
		[MyCmpReq]
		Storage storage;
		[MyCmpReq]
		TreeFilterable filter;
		[MyCmpReq]
		RocketModuleUpgradeStorage upgradeStorage;

		public const string TECH_ID = "RTB_CargoBayCollectionFilterTech";
		[Serialize]
		private bool OnlyCollectFilteredItems = false;

		private static Dictionary<Storage, CargoBayCollectionFilter> map = [];

		public override void OnSpawn()
		{
			base.OnSpawn();
			if(!Config.Instance.RocketModuleUpgrades)
			{
				Destroy(this);
				return;
			}
			map[storage] = this;
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			map.Remove(storage);
		}
		public static bool FilterOnlyActive(Storage storage)
		{
			if (storage == null)
				return false;
			if (!map.TryGetValue(storage, out var filter))
				return false;
			return filter.OnlyCollectFilteredItems;
		}
		public static bool BlockedByFilters(Storage storage, Tag ID)
		{
			if (storage == null) return false;
			if(!map.TryGetValue(storage, out var filter))
				return false;
			return !filter.InFilter(ID);
		}
		public bool InFilter(Tag ID)
		{
			return filter != null && filter.GetTags().Contains(ID);
		}

		bool UpgradeInstalled => upgradeStorage.HasUpgrade(ModuleUpgradeDatabase.CargoBayFilter);

		public string CheckboxTitleKey => "";

		public string CheckboxLabel => STRINGS.UI.RTB_CARGOBAYFILTER.LABEL;

		public string CheckboxTooltip => UpgradeInstalled ? STRINGS.UI.RTB_CARGOBAYFILTER.TOOLTIP : STRINGS.UI.RTB_CARGOBAYFILTER.TOOLTIP_NOTCONSTRUCTED;

		public bool GetCheckboxValue() => OnlyCollectFilteredItems;

		public bool GetIsCheckboxInteractable() => UpgradeInstalled;

		public void SetCheckboxValue(bool value) => OnlyCollectFilteredItems = value;

	}
}
