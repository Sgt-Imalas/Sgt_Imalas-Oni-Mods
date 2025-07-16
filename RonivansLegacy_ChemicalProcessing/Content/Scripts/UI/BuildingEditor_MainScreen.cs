using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.ReorderableList;
using UtilLibs.UIcmp;
using UtilLibs;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.UI
{
	class BuildingEditor_MainScreen : FScreen
	{
		public static BuildingEditor_MainScreen Instance;
		public BuildingConfigurationEntry SelectedOutline = null;


		public FInputField2 FilterBar;
		public FButton ClearFilterButton;

		public GameObject OutlineEntryContainer;
		public GameObject OutlineEntryPrefab;

		public LocText SelectedEntryNameDisplay;
		public LocText SelectedEntryDescriptionDisplay;
		public LocText SelectedEntryModOriginDisplay;
		public Image SelectedEntryPreviewImage;

		GameObject WattageContainer;
		GameObject StorageCapacityContainer;

		FInputField2 WattageInput, StorageCapacityInput;
		FToggle BuildingEnabledToggle;

		GameObject Details;

		Dictionary<BuildingConfigurationEntry, BuildingConfigUIEntryUI> ConfigEntries = new();

		public static void ShowBuildingEditor(object obj)
		{
			SgtLogger.l("Opening AIO Building Config Editor");
			ShowWindow();
		}
		public static void ShowWindow()
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.BuildingEditorWindowPrefab, FrontEndManager.Instance.gameObject, true);
				Instance = screen.AddOrGet<BuildingEditor_MainScreen>();
				Instance.Init();
				Instance.name = "AIO_BuildingEditor_MainScreen";
			}
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.ClearFilter();
			Instance.SelectOutline(null);
		}
		public void UpdateEntryList()
		{
			foreach (var outline in BuildingManager.ConfigCollection.BuildingConfigurations)
			{
				var uiElement = AddOrGetBuildingConfigUIEntry(outline.Value);
				uiElement.UpdateUI();
			}
			ClearFilter();
		}

		void TryResetEntries()
		{
			DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.BUILDINGEDITOR.RESETALLCHANGES.TITLE, STRINGS.UI.BUILDINGEDITOR.RESETALLCHANGES.TEXT, on_confirm: ResetEntries, on_cancel: () => { });
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
				UnityEngine.Object.DestroyImmediate(entry.Value.gameObject);
			}
			ConfigEntries.Clear();
			UpdateEntryList();
			Instance.ApplyCarePackageFilter(FilterBar.Text);
			RefreshDetails();
		}

		private bool initialized = false;
		public void Init()
		{
			if (initialized)
				return;
			initialized = true;


			var closeButton = transform.Find("Buttons/CloseButton").gameObject.AddOrGet<FButton>();
			closeButton.OnClick += () => Show(false);
			var ResetAllButton = transform.Find("Buttons/ResetButton").gameObject.AddOrGet<FButton>();
			ResetAllButton.OnClick += TryResetEntries;

			OutlineEntryPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/PresetEntryPrefab").gameObject;
			OutlineEntryPrefab.AddOrGet<BuildingConfigUIEntryUI>();
			OutlineEntryPrefab.SetActive(false);

			OutlineEntryContainer = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content").gameObject;
			Details = transform.Find("HorizontalLayout/ItemInfo").gameObject;

			FilterBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
			FilterBar.OnValueChanged.AddListener(ApplyCarePackageFilter);
			FilterBar.Text = string.Empty;

			ClearFilterButton = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
			ClearFilterButton.OnClick += () => FilterBar.Text = string.Empty;
			UIUtils.AddSimpleTooltipToObject(ClearFilterButton.gameObject, STRINGS.UI.BUILDINGEDITOR.HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);


			SelectedEntryNameDisplay = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/TitleWithIcon/Label").gameObject.GetComponent<LocText>();
			SelectedEntryPreviewImage = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/TitleWithIcon/IconContainer/Icon").gameObject.GetComponent<Image>();
			SelectedEntryDescriptionDisplay = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/DescriptionContainer").gameObject.GetComponent<LocText>();
			SelectedEntryModOriginDisplay = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ModFromContainer").gameObject.GetComponent<LocText>();

			WattageContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/WattageSettings").gameObject;
			WattageInput = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/WattageSettings/Input").FindOrAddComponent<FInputField2>();
			WattageInput.Text = "0";
			WattageInput.OnValueChanged.AddListener(UpdateItemWattage);
			transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/WattageSettings/Unit").gameObject.GetComponent<LocText>().SetText(global::STRINGS.UI.UNITSUFFIXES.ELECTRICAL.WATT);
			WattageContainer.SetActive(false);

			StorageCapacityContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/CapacitySettings").gameObject;
			StorageCapacityInput = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/CapacitySettings/Input").FindOrAddComponent<FInputField2>();
			StorageCapacityInput.Text = "0";
			StorageCapacityInput.OnValueChanged.AddListener(UpdateItemCapacity);
			transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/CapacitySettings/Unit").gameObject.GetComponent<LocText>().SetText(global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM);
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


			UpdateEntryList();
			SelectOutline(null);
		}
		void UpdateItemWattage(string text)
		{
			if (SelectedOutline == null)
				return;

			if (int.TryParse(text, out int wattage))
				SelectedOutline?.SetWattage(wattage);
			OnOutlineEntryUpdated();
		}
		void UpdateItemCapacity(string text)
		{
			if (SelectedOutline == null)
				return;

			if (int.TryParse(text, out int wattage))
				SelectedOutline?.SetMassCapacity(wattage);
			OnOutlineEntryUpdated();
		}
		public void ClearFilter()
		{
			FilterBar.Text = string.Empty;
			ApplyCarePackageFilter();
		}

		public void ApplyCarePackageFilter(string filterstring = "")
		{
			foreach (var go in ConfigEntries)
			{
				go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Key.GetDisplayName()));
			}
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
				this.Show(false);
			}
			if (e.TryConsume(Action.Escape))
			{
				this.Show(false);
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
			SelectedEntryModOriginDisplay.SetText(SelectedOutline.GetModOriginText());
			BuildingEnabledToggle.SetOnFromCode(SelectedOutline.IsBuildingEnabled());

			if (SelectedOutline.HasStorageCapacity(out var capacity))
			{
				StorageCapacityContainer.SetActive(true);
				StorageCapacityInput.SetTextFromData(capacity.ToString());
			}
			else
			{
				StorageCapacityContainer.SetActive(false);
			}
			if(SelectedOutline.HasWattage(out var wattage))
			{
				WattageContainer.SetActive(true);
				WattageInput.SetTextFromData(wattage.ToString());
			}
			else
			{
				WattageContainer.SetActive(false);
			}
			SelectedEntryPreviewImage.sprite = Def.GetUISprite(SelectedOutline.BuildingID).first;
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

				OnOutlineEntryUpdated();
			}
			OnOutlineEntryUpdated(targetOutline);
		}
	}
}
