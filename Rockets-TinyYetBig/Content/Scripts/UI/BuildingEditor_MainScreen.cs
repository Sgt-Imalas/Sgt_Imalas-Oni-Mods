using Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations;
using Rockets_TinyYetBig.Content.Scripts.UI.UIComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace Rockets_TinyYetBig.Content.Scripts.UI
{
	class BuildingEditor_MainScreen : FScreen
	{
		public static BuildingEditor_MainScreen Instance;
		public BuildingConfigurationEntry SelectedOutline = null;

		public FInputField2 FilterBar;
		public FButton ClearFilterButton;

		public GameObject OutlineEntryContainer;
		public GameObject OutlineEntryPrefab, TitleEntryPrefab;

		public LocText SelectedEntryNameDisplay;
		public LocText SelectedEntryDescriptionDisplay;
		public LocText SelectedEntryModOriginDisplay;
		public Image SelectedEntryPreviewImage;

		public FMultiSelectDropdown FilterDropDown;

		GameObject WattageContainer, StorageCapacityContainer, RangeContainer;
		LocText WattageLabel, CapacityLabel, CapacityUnits, RangeLabel, RangeUnits;
		ToolTip WattageTooltip, CapacityTooltip, RangeTooltip;

		FInputField2 WattageInput, StorageCapacityInput, RangeInput;
		FToggle BuildingEnabledToggle;

		FButton ResetSingleBuilding;

		GameObject Details;

		Dictionary<BuildingConfigurationEntry, BuildingConfigUIEntryUI> ConfigEntries = new();
		Dictionary<int, GameObject> CategoryTitleEntries = [];

		HashSet<int> FilteredMods = [];

		LocText ToggleAllButtonText;

		public static void ShowBuildingEditor(object _, int? openedFrom = null)
		{
			SgtLogger.l("Opening RTB Building Config Editor");
			ShowWindow(openedFrom);
		}
		public static void ShowWindow(int? source = null)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.BuildingEditorWindowPrefab, FrontEndManager.Instance.gameObject, true);
				Instance = screen.AddOrGet<BuildingEditor_MainScreen>();
				Instance.Init();
				Instance.name = "RTB_BuildingEditor_MainScreen";
			}
			Instance.Show(true);
			Instance.OpenedFrom(source);
		}

		public override void Show(bool show = true)
		{
			base.Show(show);
			this.ConsumeMouseScroll = show;
			if (show)
			{
				transform.SetAsLastSibling();
				SelectOutline(null);
				ClearFilter();
			}
		}

		void OpenedFrom(int? source = null)
		{
			UpdateEntryList();
			if (source == null || !source.HasValue)
			{
				ResetModFilters(false);
			}
			else
				FilteredMods = [source.Value];
			SetToggleButtonState();
			RefreshItemFilterEntries();
		}
		void ResetModFilters(bool refresh = true)
		{
			FilteredMods = [.. RTB_BuildingCategoryType.FilterCategories];
			if (refresh)
				RefreshItemFilterEntries();
		}

		public void UpdateEntryList()
		{
			//foreach (var filter in RTB_BuildingCategoryType.FilterCategories)
			//{
			//	AddOrGetCategoryUIEntry(filter);
			//}

			var configs = BuildingManager.ConfigCollection.BuildingConfigurations.OrderBy(kvp => kvp.Value.GetDisplayName()).ToList();

			foreach (var outline in configs)
			{
				var uiElement = AddOrGetBuildingConfigUIEntry(outline.Value);
				uiElement.UpdateUI();
			}
		}

		void TryResetEntries()
		{
			DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.RE_BUILDINGEDITOR.RESETALLCHANGES.TITLE, STRINGS.UI.RE_BUILDINGEDITOR.RESETALLCHANGES.TEXT, on_confirm: ResetEntries, on_cancel: () => { });
		}
		void ResetCurrentBuilding()
		{
			if (SelectedOutline == null)
				return;

			BuildingManager.ResetSingleConfig(SelectedOutline);
			OnOutlineEntryUpdated(SelectedOutline);
			RefreshDetails();
		}

		public void ResetEntries()
		{
			SelectedOutline = null;
			BuildingManager.ResetConfigChanges();
			ResetEntriesUI();
		}
		void ResetEntriesUI()
		{
			foreach (var entry in ConfigEntries)
			{
				DestroyImmediate(entry.Value.gameObject);
			}
			ConfigEntries.Clear();
			//foreach (var entry in CategoryTitleEntries)
			//{
			//	DestroyImmediate(entry.Value.gameObject);
			//}
			//CategoryTitleEntries.Clear();
			UpdateEntryList();
			ApplyBuildingFilter(FilterBar.Text);
			RefreshDetails();
		}

		private bool initialized = false;
		public void Init()
		{
			if (initialized)
				return;


			var closeButton = transform.Find("Buttons/CloseButton").gameObject.AddOrGet<FButton>();
			closeButton.OnClick += () => Show(false);
			var ResetAllButton = transform.Find("Buttons/ResetButton").gameObject.AddOrGet<FButton>();
			ResetAllButton.OnClick += TryResetEntries;

			OutlineEntryPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/PresetEntryPrefab").gameObject;
			OutlineEntryPrefab.AddOrGet<BuildingConfigUIEntryUI>();
			OutlineEntryPrefab.SetActive(false);

			TitleEntryPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/TitleEntryPrefab").gameObject;
			TitleEntryPrefab.SetActive(false);

			OutlineEntryContainer = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content").gameObject;
			Details = transform.Find("HorizontalLayout/ItemInfo").gameObject;

			FilterBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
			FilterBar.AddListener(ApplyBuildingFilter);
			FilterBar.ClearInputTextWithoutEvent();

			ClearFilterButton = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
			ClearFilterButton.OnClick += () => FilterBar.Text = string.Empty;

			UIUtils.AddSimpleTooltipToObject(ClearFilterButton.gameObject, STRINGS.UI.RE_BUILDINGEDITOR.HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);


			SelectedEntryNameDisplay = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/TitleWithIcon/Label").gameObject.GetComponent<LocText>();
			SelectedEntryPreviewImage = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/TitleWithIcon/IconContainer/Icon").gameObject.GetComponent<Image>();
			SelectedEntryDescriptionDisplay = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/DescriptionContainer").gameObject.GetComponent<LocText>();
			SelectedEntryModOriginDisplay = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ModFromContainer").gameObject.GetComponent<LocText>();

			WattageContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/WattageSettings").gameObject;
			WattageLabel = WattageContainer.GetComponent<LocText>();
			WattageTooltip = UIUtils.AddSimpleTooltipToObject(WattageContainer, string.Empty);
			WattageInput = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/WattageSettings/Input").FindOrAddComponent<FInputField2>();
			WattageInput.Text = "0";
			WattageInput.AddEndListener(UpdateItemWattage);
			transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/WattageSettings/Unit").gameObject.GetComponent<LocText>().SetText(global::STRINGS.UI.UNITSUFFIXES.ELECTRICAL.WATT);
			WattageContainer.SetActive(false);

			StorageCapacityContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/CapacitySettings").gameObject;
			CapacityLabel = StorageCapacityContainer.GetComponent<LocText>();
			CapacityTooltip = UIUtils.AddSimpleTooltipToObject(StorageCapacityContainer, string.Empty);
			CapacityUnits = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/CapacitySettings/Unit").gameObject.GetComponent<LocText>();
			StorageCapacityInput = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/CapacitySettings/Input").FindOrAddComponent<FInputField2>();
			StorageCapacityInput.Text = "0";
			StorageCapacityInput.AddEndListener(UpdateItemCapacity);
			CapacityUnits.SetText(global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM);

			RangeContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/RangeSettings").gameObject;
			RangeInput = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/RangeSettings/Input").FindOrAddComponent<FInputField2>();
			RangeInput.Text = "0";
			RangeTooltip = UIUtils.AddSimpleTooltipToObject(RangeContainer, string.Empty);
			RangeInput.AddEndListener(UpdateItemRange);
			RangeLabel = RangeContainer.GetComponent<LocText>();
			RangeUnits = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/RangeSettings/Unit").gameObject.GetComponent<LocText>();
			RangeUnits.SetText(global::STRINGS.UI.UNITSUFFIXES.TILES);

			StorageCapacityContainer.gameObject.SetActive(false);

			BuildingEnabledToggle = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/EnableBuilding/Checkbox").gameObject.AddOrGet<FToggle>();
			BuildingEnabledToggle.SetCheckmark("Checkmark");
			BuildingEnabledToggle.OnClick += (on) =>
			{
				if (SelectedOutline != null)
				{
					SetOutlineEnabled(SelectedOutline, on);
				}
			};

			ResetSingleBuilding = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ResetButton").gameObject.AddOrGet<FButton>();
			ResetSingleBuilding.OnClick += ResetCurrentBuilding;

			FilterDropDown = transform.Find("HorizontalLayout/ObjectList/Filters/FilterButton").FindOrAddComponent<FMultiSelectDropdown>();
			var entries = RTB_BuildingCategoryType.FilterCategories.Select(CreateFilterEntry).ToList();
			FilterDropDown.DropDownEntries = entries;
			FilterDropDown.InitializeDropDown();
			ToggleAllButtonText = transform.Find("HorizontalLayout/ObjectList/Filters/FilterButton/DropDownContent/ShowAllButton/Text").gameObject.GetComponent<LocText>();
			var toggleAll = transform.Find("HorizontalLayout/ObjectList/Filters/FilterButton/DropDownContent/ShowAllButton").gameObject.AddOrGet<FButton>();
			toggleAll.OnClick += ToggleAllFilters;
			ToggleAllButtonText.SetText(global::STRINGS.UI.FRONTEND.MODS.DISABLE_ALL);

			initialized = true;

			UpdateEntryList();
			SelectOutline(null);
		}

		FMultiSelectDropdown.FDropDownEntry CreateFilterEntry(int category)
		{
			RTB_BuildingCategoryType.TryGetCategoryName(category, out var categoryName);
			return new FMultiSelectDropdown.FDropDownEntry(categoryName, (_) => ToggleFilter(category), true);
		}

		void SetToggleButtonState()
		{
			bool shouldDisable = FilteredMods.Any();
			if (shouldDisable)
				ToggleAllButtonText.SetText(global::STRINGS.UI.FRONTEND.MODS.DISABLE_ALL);
			else
				ToggleAllButtonText.SetText(global::STRINGS.UI.FRONTEND.MODS.ENABLE_ALL);
		}

		void ToggleAllFilters()
		{
			bool shouldDisable = FilteredMods.Any();
			if (shouldDisable)
			{
				FilteredMods.Clear();
				ToggleAllButtonText.SetText(global::STRINGS.UI.FRONTEND.MODS.ENABLE_ALL);
				RefreshItemFilterEntries();
			}
			else
			{
				ToggleAllButtonText.SetText(global::STRINGS.UI.FRONTEND.MODS.DISABLE_ALL);
				ResetModFilters();
			}
		}

		bool ToggleFilter(int category)
		{
			if (FilteredMods.Contains(category))
				FilteredMods.Remove(category);
			else
				FilteredMods.Add(category);

			ApplyBuildingFilter(FilterBar.Text);
			SetToggleButtonState();
			return FilteredMods.Contains(category);
		}

		void UpdateItemWattage(string text)
		{
			if (SelectedOutline == null)
				return;

			if (int.TryParse(text, out int wattage))
				SelectedOutline?.SetWattage(wattage);
			WattageInput.SetTextFromData(SelectedOutline?.GetWattage().ToString());
			OnOutlineEntryUpdated();
		}
		void UpdateItemCapacity(string text)
		{
			if (SelectedOutline == null)
				return;

			if (int.TryParse(text, out int mass))
			{
				SelectedOutline?.SetMassCapacity(mass);
			}
			StorageCapacityInput.SetTextFromData(SelectedOutline?.GetStorageCapacity().ToString());
			OnOutlineEntryUpdated();
		}
		
		void UpdateItemRange(string text)
		{
			if (SelectedOutline == null)
				return;
			if (int.TryParse(text, out int range))
				SelectedOutline?.SetTileRange(range);

			RangeInput.SetTextFromData(SelectedOutline.GetTileRange().ToString());

			OnOutlineEntryUpdated();
		}

		public void ClearFilter()
		{
			FilterBar.Text = string.Empty;
			ApplyBuildingFilter();
		}

		void RefreshItemFilterEntries()
		{
			ApplyBuildingFilter(FilterBar.Text);

			var mods = RTB_BuildingCategoryType.FilterCategories;
			for (int i = 0; i < mods.Count(); i++)
			{
				var mod = mods[i];

				FilterDropDown.DropDownEntries[i].Toggle.SetOnFromCode(FilteredMods.Contains(mod));
			}
		}
		public void ApplyBuildingFilter(string filterstring = "")
		{
			foreach (var go in ConfigEntries)
			{
				bool allowedByFilter = filterstring == string.Empty || ShowInFilter(filterstring, go.Key.GetDisplayName());
				if (allowedByFilter)
				{
					allowedByFilter = go.Key.BuildingInjection?.ConfigCategories.Any(FilteredMods.Contains) ?? false;
				}

				go.Value.gameObject.SetActive(allowedByFilter);
			}

			//foreach(var filter in CategoryTitleEntries)
			//{
			//	filter.Value.gameObject.SetActive(FilteredMods.Contains(filter.Key));
			//}
		}

		bool ShowInFilter(string filtertext, string stringsToInclude)
		{
			return ShowInFilter(filtertext, [stringsToInclude]);
		}

		bool ShowInFilter(string filtertext, string[] stringsToInclude)
		{
			filtertext = filtertext.ToLowerInvariant();

			foreach (var text in stringsToInclude)
			{
				if (text != null && text.Length > 0 && text.ToLowerInvariant().Contains(filtertext))
				{
					return true;
				}
			}
			return false;
		}

		void OnOutlineEntryUpdated(BuildingConfigurationEntry target = null)
		{
			if (target == null)
				target = SelectedOutline;

			AddOrGetBuildingConfigUIEntry(target).UpdateUI();
			BuildingManager.ConfigCollection.WriteToFile();
		}

		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				Show(false);
			}
			if (e.TryConsume(Action.Escape))
			{
				Show(false);
			}
			base.OnKeyDown(e);
		}
		public BuildingConfigUIEntryUI AddOrGetBuildingConfigUIEntry(BuildingConfigurationEntry outline)
		{
			if (ConfigEntries.TryGetValue(outline, out var entry))
				return entry;

			var newEntry = Util.KInstantiateUI<BuildingConfigUIEntryUI>(OutlineEntryPrefab.gameObject, OutlineEntryContainer, true);
			newEntry.UpdateOutline(outline);
			ConfigEntries.Add(outline, newEntry);
			return newEntry;

		}
		//public GameObject AddOrGetCategoryUIEntry(int type)
		//{
		//	if (CategoryTitleEntries.TryGetValue(type, out var entry))
		//		return entry;

		//	var newEntry = Util.KInstantiateUI(TitleEntryPrefab.gameObject, OutlineEntryContainer, true);
		//	if (!RTB_BuildingCategoryType.TryGetCategoryName(type, out var categoryName))
		//		categoryName = "MISSING";
		//	UIUtils.TryChangeText(newEntry.transform, "Label", categoryName);
		//	CategoryTitleEntries.Add(type, newEntry);
		//	return newEntry;

		//}
		internal void RemoveOutlineEntry(BuildingConfigurationEntry targetOutline)
		{
			if (ConfigEntries.TryGetValue(targetOutline, out var UIEntry))
			{
				ConfigEntries.Remove(targetOutline);
				Util.KDestroyGameObject(UIEntry.gameObject);
			}

			if (SelectedOutline == targetOutline)
			{
				SelectedOutline = null;
				RefreshDetails();
			}
		}
		internal void SelectOutline(BuildingConfigurationEntry targetOutline)
		{
			if (targetOutline != SelectedOutline)
			{
				SelectedOutline = targetOutline;
				RefreshDetails();
			}
			else if (targetOutline == null)
				RefreshDetails();
		}
		public void RefreshDetails()
		{
			if (SelectedOutline == null)
			{
				Details.SetActive(false);
				return;
			}
			Details.SetActive(true);
			SelectedEntryNameDisplay.SetText(SelectedOutline.GetDisplayName());
			SelectedEntryDescriptionDisplay.SetText(SelectedOutline.GetDisplayDescription());
			SelectedEntryModOriginDisplay.SetText(SelectedOutline.GetCategoryText());
			BuildingEnabledToggle.SetOnFromCode(SelectedOutline.IsBuildingEnabled());

			if (SelectedOutline.HasStorageCapacity(out var capacity))
			{
				StorageCapacityContainer.SetActive(true); 
				if (SelectedOutline.HasStorageLabelDescriptor(out string descriptor))
				{
					CapacityLabel.SetText(descriptor);
				}
				else
				{
					CapacityLabel.SetText(STRINGS.UI.RE_BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.CAPACITYSETTINGS);
				}

				CapacityUnits.SetText(SelectedOutline.StorageUnitLabel.IsNullOrWhiteSpace() ? global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM : SelectedOutline.StorageUnitLabel);
				CapacityTooltip.SetSimpleTooltip(SelectedOutline.StorageToolTip ?? string.Empty);
				StorageCapacityInput.SetTextFromData(capacity.ToString());
			}
			else
			{
				StorageCapacityContainer.SetActive(false);
			}
			if (SelectedOutline.HasWattage(out var wattage))
			{
				WattageContainer.SetActive(true);
				WattageLabel.SetText(SelectedOutline.IsGenerator
					? STRINGS.UI.RE_BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.WATTAGESETTINGS_GENERATOR :
					 STRINGS.UI.RE_BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.WATTAGESETTINGS);
				WattageInput.SetTextFromData(wattage.ToString());
				//CapacityTooltip.SetSimpleTooltip(SelectedOutline.WattageToolTip != null ? SelectedOutline.WattageToolTip : string.Empty);
			}
			else
			{
				WattageContainer.SetActive(false);
			}
			if (SelectedOutline.HasTileRange(out int range))
			{
				RangeContainer.SetActive(true);
				if (SelectedOutline.HasTileRangeDescriptor(out string descriptor))
				{
					RangeLabel.SetText(descriptor);
				}
				else
				{
					RangeLabel.SetText(STRINGS.UI.RE_BUILDINGEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT.RANGESETTINGS);
				}
				RangeUnits.SetText(SelectedOutline.RangeUnitLabel.IsNullOrWhiteSpace() ? global::STRINGS.UI.UNITSUFFIXES.TILES : SelectedOutline.RangeUnitLabel);
				RangeTooltip.SetSimpleTooltip(SelectedOutline.RangeToolTip != null ? SelectedOutline.RangeToolTip : string.Empty);
				RangeInput.SetTextFromData(range.ToString());
			}
			else
			{
				RangeContainer.SetActive(false);
			}

			
			SelectedEntryPreviewImage.sprite = Def.GetUISprite(SelectedOutline.BuildingID)?.first ?? Assets.GetSprite("unknown");
		}

		internal void OnToggleChanged(BuildingConfigurationEntry targetOutline)
		{
			if (targetOutline == SelectedOutline)
			{
				RefreshDetails();
			}
		}

		internal void SetOutlineEnabled(BuildingConfigurationEntry targetOutline, bool on)
		{
			targetOutline.SetBuildingEnabled(on);
			if (targetOutline == SelectedOutline)
			{
				RefreshDetails();
			}
			OnOutlineEntryUpdated(targetOutline);
			if (targetOutline.IsInjected && targetOutline.BuildingInjection.TryGetSyncedEntries(out var entries))
				foreach (var entry in entries)
					if(entry.Source != null)
						OnOutlineEntryUpdated(entry.Source);
		}
	}
}
