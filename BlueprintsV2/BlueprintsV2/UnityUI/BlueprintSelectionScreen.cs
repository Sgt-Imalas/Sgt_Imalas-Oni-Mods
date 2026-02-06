using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using BlueprintsV2.Tools;
using BlueprintsV2.UnityUI.Components;
using rail;
using STRINGS;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.BLUEPRINTINFO.STATS;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.BUILDINGLIST.SCROLLAREA.CONTENT;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.MATERIALSWITCH;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.MATERIALSWITCH.BUTTONS;
using static BlueprintsV2.STRINGS.UI.DIALOGUE;

namespace BlueprintsV2.UnityUI
{
	internal class BlueprintSelectionScreen : FScreen, IRender1000ms
	{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
		public static BlueprintSelectionScreen Instance = null;

		enum OrderBy
		{
			Invalid = 0,
			CreationDateAscending = 1,
			CreationDateDescending = 2,
			NameAscending = 3, 
			NameDescending = 4,
		}
		const string SortStateKey = "BlueprintsV2_BlueprintSortState";
		OrderBy SortBlueprintsBy
		{
			get
			{
				var val = (OrderBy)KPlayerPrefs.GetInt(SortStateKey);
				if (val == OrderBy.Invalid)
					val = OrderBy.CreationDateDescending;
				return val;
			}
			set
			{
				KPlayerPrefs.SetInt(SortStateKey,(int)value);
			}
		}

		//Main Areas
		public GameObject BlueprintsList;
		public GameObject BlueprintsElements;
		public GameObject ReplaceBlueprintElements;
		public GameObject BlueprintInfo;
		public GameObject BlueprintInfoBuildingList;
		public FButton CloseBtn;

		//BlueprintList
		public FInputField2 BlueprintSearchbar;
		public FButton ClearBlueprintSearchbar;
		public FButton OpenBlueprintFolder;
		public FButton ImportBlueprintButton;
		public FButton FolderUpBtn;
		public GameObject HierarchyContainer;
		public FileHierarchyEntry HierarchyEntryPrefab;
		public FolderHierarchyEntry HierarchyFolderPrefab;
		public Dictionary<BlueprintFolder, FolderHierarchyEntry> FolderEntries = new();
		public Dictionary<Blueprint, FileHierarchyEntry> BlueprintEntries = new();

		public FOrdeByParamToggle OrderByName, OrderByDate;

		//Blueprint Info Screen
		public LocText BlueprintName;
		public FInputField2 DescriptionInput;
		public FButton ResetText, ApplyText;
		public LocText DimensionInfo, BuildingCount, DigCount, NoteCount;

		//Blueprint info building list
		public FInputField2 BuildingListSearchbar;
		public FButton ClearBuildingListSearchbar;
		public GameObject BuildingInfoContainer;
		public BuildingInfoEntry BuildingInfoEntryPrefab;
		public Dictionary<string, BuildingInfoEntry> BuildingInfoEntries = new();
		public Image BlueprintIconDisplay;
		public FButton EditBlueprintIconBtn, ClearBlueprintIconBtn;
		public FColorPickerArray ColorPicker;


		//MaterialList
		public Dictionary<BlueprintSelectedMaterial, BlueprintElementEntry> ElementEntries = new();
		public GameObject ElementEntryContainer;
		public GameObject WarningGO, ErrorGO;
		public ToolTip SevereErrorTooltip, ErrorTooltip;
		public BlueprintElementEntry ElementEntryPrefab;
		public FButton ClearOverrides, PlaceBlueprint, CreateNewBlueprintFromOverrides;
		public LocText MaterialHeaderTitle;
		public FToggle AdvancedReplacementToggle;


		//ReplacementList
		public FInputField2 ReplacementElementSearchbar;
		public Dictionary<Tag, ReplaceElementEntry> ReplacementElementEntries = new();
		public FButton ClearReplacementElementSearchbar;
		public GameObject ReplacementElementsContainer;
		public ReplaceElementEntry ReplacementElementsPrefab;
		public LocText ToReplaceName;
		public GameObject NoItems;

		System.Action<Blueprint> onCloseAction;

		public bool CurrentlyActive;
		public bool DialogueCurrentlyOpen;

		public bool ShowInfoScreen;
		public Blueprint TargetBlueprint, InfoBlueprint;

		private bool _openedFromSnapshot;
		public bool OpenedFromSnapshot
		{
			get
			{
				return _openedFromSnapshot;
			}
			set
			{
				_openedFromSnapshot = value;
				if (TargetBlueprint != null)
				{
					string materialLabel = value ? STRINGS.UI.USEBLUEPRINTSTATECONTAINER.INFOITEMSCONTAINER.MATERIALREPLACEMENT.LABEL : string.Format(MATERIALSWITCH.MATERIALSHEADER.LABEL, TargetBlueprint.FriendlyName);
					MaterialHeaderTitle.SetText(materialLabel);
				}
			}
		}
		public override float GetSortKey()
		{
			return base.GetSortKey() + 1;
		}

		private void Init()
		{
			if (init) { return; }
			SgtLogger.l("Initializing BlueprintWindow");

			BlueprintsList = transform.Find("FileHierarchy").gameObject;
			BlueprintsElements = transform.Find("MaterialSwitch").gameObject;
			ReplaceBlueprintElements = transform.Find("MaterialReplacer").gameObject;
			BlueprintInfo = transform.Find("BlueprintInfo").gameObject;
			BlueprintInfoBuildingList = transform.Find("BuildingList").gameObject;

			CloseBtn = transform.Find("CloseButton").gameObject.AddOrGet<FButton>();
			CloseBtn.OnClick += OnCloseClicked;

			//blueprint files
			BlueprintSearchbar = transform.Find("FileHierarchy/SearchBar/Input").gameObject.AddOrGet<FInputField2>();
			BlueprintSearchbar.OnValueChanged.AddListener(ApplyBlueprintFilter);
			BlueprintSearchbar.Text = string.Empty;

			ImportBlueprintButton = transform.Find("FileHierarchy/ImportButton").gameObject.AddOrGet<FButton>();
			ImportBlueprintButton.OnClick += TryImportBlueprint;
			UIUtils.AddSimpleTooltipToObject(ImportBlueprintButton.gameObject, BLUEPRINTSELECTOR.FILEHIERARCHY.IMPORTBUTTON.TOOLTIP);

			OpenBlueprintFolder = transform.Find("FileHierarchy/SearchBar/FolderButton").gameObject.AddOrGet<FButton>();
			OpenBlueprintFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.BlueprintFileHandling.GetBlueprintDirectory()) { UseShellExecute = true });
			UIUtils.AddSimpleTooltipToObject(OpenBlueprintFolder.gameObject, BLUEPRINTSELECTOR.FILEHIERARCHY.SEARCHBAR.OPENFOLDERTOOLTIP);

			ClearBlueprintSearchbar = transform.Find("FileHierarchy/SearchBar/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearBlueprintSearchbar.OnClick += () => BlueprintSearchbar.Text = string.Empty;
			UIUtils.AddSimpleTooltipToObject(ClearBlueprintSearchbar.gameObject, BLUEPRINTSELECTOR.FILEHIERARCHY.SEARCHBAR.CLEARTOOLTIP);

			FolderUpBtn = transform.Find("FileHierarchy/ScrollArea/Content/FolderUp").gameObject.AddOrGet<FButton>();
			FolderUpBtn.gameObject.SetActive(true);
			FolderUpBtn.OnClick += () => SelectFolder(null);

			HierarchyContainer = transform.Find("FileHierarchy/ScrollArea/Content").gameObject;

			ClearOverrides = transform.Find("MaterialSwitch/Buttons/ResetButton").gameObject.AddOrGet<FButton>();
			ClearOverrides.OnClick += OnClearOverrides;

			PlaceBlueprint = transform.Find("MaterialSwitch/Buttons/PlaceBPbtn").gameObject.AddOrGet<FButton>();
			PlaceBlueprint.OnClick += OnPlaceBlueprint;

			CreateNewBlueprintFromOverrides = transform.Find("MaterialSwitch/Buttons/CreateModifiedBtn").gameObject.AddOrGet<FButton>();
			CreateNewBlueprintFromOverrides.OnClick += OnCreateFromOverrides;
			UIUtils.AddSimpleTooltipToObject(CreateNewBlueprintFromOverrides.gameObject, CREATEMODIFIED.TOOLTIP);

			OrderByName = transform.Find("FileHierarchy/Filters/NameSort").gameObject.AddOrGet<FOrdeByParamToggle>();
			OrderByName.SetActions(() => ChangeSortBy(OrderBy.NameAscending), () => ChangeSortBy(OrderBy.NameDescending));
			OrderByDate = transform.Find("FileHierarchy/Filters/DateSort").gameObject.AddOrGet<FOrdeByParamToggle>();
			OrderByDate.SetActions(() => ChangeSortBy(OrderBy.CreationDateAscending), () => ChangeSortBy(OrderBy.CreationDateDescending));
			OrderByDate.StartDescending = true;

			switch (SortBlueprintsBy)
			{
				case OrderBy.CreationDateAscending:
					OrderByDate.ActivateToggle(1);break;
				case OrderBy.CreationDateDescending:
					OrderByDate.ActivateToggle(2); break;
				case OrderBy.NameAscending:
					OrderByName.ActivateToggle(1); break;
				case OrderBy.NameDescending:
					OrderByName.ActivateToggle(2); break;
			}



			var hierarchyEntryGO = transform.Find("FileHierarchy/ScrollArea/Content/BlueprintEntryPrefab").gameObject;
			hierarchyEntryGO.SetActive(false);
			HierarchyEntryPrefab = hierarchyEntryGO.AddOrGet<FileHierarchyEntry>();


			var hierarchyFolderGO = transform.Find("FileHierarchy/ScrollArea/Content/FolderPrefab").gameObject;
			hierarchyFolderGO.SetActive(false);
			HierarchyFolderPrefab = hierarchyFolderGO.AddOrGet<FolderHierarchyEntry>();

			//material overrides
			ElementEntryContainer = transform.Find("MaterialSwitch/ScrollArea/Content").gameObject;
			MaterialHeaderTitle = transform.Find("MaterialSwitch/MaterialsHeader/Label").gameObject.AddOrGet<LocText>();

			AdvancedReplacementToggle = transform.Find("MaterialSwitch/PerBuildingOverrides").gameObject.AddOrGet<FToggle>();
			AdvancedReplacementToggle.SetCheckmark("Checkbox/Checkmark");
			AdvancedReplacementToggle.SetOnFromCode(BlueprintState.AdvancedMaterialReplacement);
			UIUtils.AddSimpleTooltipToObject(transform.Find("MaterialSwitch/PerBuildingOverrides/Label").gameObject, PERBUILDINGOVERRIDES.TOOLTIP);
			AdvancedReplacementToggle.OnChange += OnAdvancedReplacementToggleChanged;

			WarningGO = transform.Find("MaterialSwitch/MaterialsHeader/WarningSevere").gameObject;
			SevereErrorTooltip = UIUtils.AddSimpleTooltipToObject(WarningGO.transform, MATERIALSWITCH.WARNINGSEVERE);
			WarningGO.SetActive(false);


			ErrorGO = transform.Find("MaterialSwitch/MaterialsHeader/Warning").gameObject;
			ErrorTooltip = UIUtils.AddSimpleTooltipToObject(ErrorGO.transform, MATERIALSWITCH.WARNING);
			ErrorGO.SetActive(false);

			var ElementEntryPrefabGo = transform.Find("MaterialSwitch/ScrollArea/Content/PresetEntryPrefab").gameObject;
			ElementEntryPrefabGo.SetActive(false);
			ElementEntryPrefab = ElementEntryPrefabGo.AddOrGet<BlueprintElementEntry>();

			///material override selection
			ReplacementElementSearchbar = transform.Find("MaterialReplacer/SearchBar/Input").gameObject.AddOrGet<FInputField2>();
			ReplacementElementSearchbar.OnValueChanged.AddListener(ApplyElementsFilter);
			ReplacementElementSearchbar.Text = string.Empty;

			ClearReplacementElementSearchbar = transform.Find("MaterialReplacer/SearchBar/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearReplacementElementSearchbar.OnClick += () => ReplacementElementSearchbar.Text = string.Empty;
			UIUtils.AddSimpleTooltipToObject(ClearReplacementElementSearchbar.gameObject, BLUEPRINTSELECTOR.FILEHIERARCHY.SEARCHBAR.CLEARTOOLTIP);

			ReplacementElementsContainer = transform.Find("MaterialReplacer/ScrollArea/Content").gameObject;
			ToReplaceName = transform.Find("MaterialReplacer/ToReplace/CurrentlyActive/Label").gameObject.GetComponent<LocText>();
			NoItems = transform.Find("MaterialSwitch/ScrollArea/Content/NoElementsInBlueprint")?.gameObject;

			var ReplaceElementEntryGo = transform.Find("MaterialReplacer/ScrollArea/Content/CarePackagePrefab").gameObject;
			ReplaceElementEntryGo.SetActive(false);
			ReplacementElementsPrefab = ReplaceElementEntryGo.AddComponent<ReplaceElementEntry>();

			//blueprintInfo

			BlueprintName = transform.Find("BlueprintInfo/Header/Label").gameObject.GetComponent<LocText>();

			DescriptionInput = transform.Find("BlueprintInfo/Description/Input").gameObject.AddOrGet<FInputField2>();
			DescriptionInput.Text = string.Empty;

			ResetText = transform.Find("BlueprintInfo/Buttons/ResetButton").gameObject.AddOrGet<FButton>();
			ResetText.OnClick += LoadBlueprintDescription;
			ApplyText = transform.Find("BlueprintInfo/Buttons/ApplyButton").gameObject.AddOrGet<FButton>();
			ApplyText.OnClick += SaveBlueprintDescription;


			DimensionInfo = transform.Find("BlueprintInfo/Stats/Dimension/Descriptor/Output").gameObject.GetComponent<LocText>();
			BuildingCount = transform.Find("BlueprintInfo/Stats/BuildingCount/Descriptor/Output").gameObject.GetComponent<LocText>();
			DigCount = transform.Find("BlueprintInfo/Stats/DigCount/Descriptor/Output").gameObject.GetComponent<LocText>();
			NoteCount = transform.Find("BlueprintInfo/Stats/LiquidCount/Descriptor/Output").gameObject.GetComponent<LocText>();

			//blueprint info building list

			BuildingListSearchbar = transform.Find("BuildingList/SearchBar/Input").gameObject.AddOrGet<FInputField2>();
			BuildingListSearchbar.OnValueChanged.AddListener(ApplyBuildingsFilter);
			BuildingListSearchbar.Text = string.Empty;

			ClearBuildingListSearchbar = transform.Find("BuildingList/SearchBar/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearBuildingListSearchbar.OnClick += () => BuildingListSearchbar.Text = string.Empty;

			BuildingInfoContainer = transform.Find("BuildingList/ScrollArea/Content").gameObject;
			BuildingInfoEntryPrefab = BuildingInfoContainer.transform.Find("BuildingEntryPrefab").gameObject.AddOrGet<BuildingInfoEntry>();
			BuildingInfoEntryPrefab.gameObject.SetActive(false);

			EditBlueprintIconBtn = transform.Find("BlueprintInfo/Stats/IconContainer/EditButton").gameObject.AddOrGet<FButton>();
			EditBlueprintIconBtn.OnClick += ShowSpriteSelectionScreen;
			ClearBlueprintIconBtn = transform.Find("BlueprintInfo/Stats/IconContainer/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearBlueprintIconBtn.OnClick += ClearCurrentInfoBlueprintIcon;
			BlueprintIconDisplay = transform.Find("BlueprintInfo/Stats/IconContainer/IconBG/Icon").gameObject.GetComponent<Image>();

			ColorPicker = transform.Find("BlueprintInfo/Stats/IconContainer/ColorPicker").gameObject.AddOrGet<FColorPickerArray>();
			ColorPicker.OnColorChange += SetCurrentInfoBlueprintTint;

			init = true;
		}
		void ChangeSortBy(OrderBy orderBy)
		{
			SortBlueprintsBy = orderBy;
			switch (orderBy)
			{
				case OrderBy.NameDescending:
				case OrderBy.NameAscending:
					OrderByDate.DeactivateToggle();
					break;
				case OrderBy.CreationDateDescending: 
				case OrderBy.CreationDateAscending:
					OrderByName.DeactivateToggle(); 
					break;
			}
			UpdateBlueprintButtons();
		}

		void ShowSpriteSelectionScreen()
		{
			DialogueCurrentlyOpen = true;
			SpriteSelectorScreen.ShowScreen(true, UpdateBlueprintIcon, ()=>DialogueOpen(false));
		}
		void UpdateBlueprintIcon(string iconId, Color tint)
		{
			SetCurrentInfoBlueprintIcon(iconId);
			tint.a = 1;
			SetCurrentInfoBlueprintTint(tint);
			ColorPicker.SetSelected(tint);

			DialogueCurrentlyOpen = false;
		}
		void SetCurrentInfoBlueprintTint(Color tint)
		{
			if (tint == Color.white)
				InfoBlueprint.IconTintHex = null;
			else
				InfoBlueprint.IconTintHex = tint.ToHexString();
			InfoBlueprint.Write();
			RefreshInfoIcon();
		}
		void SetCurrentInfoBlueprintIcon(string spriteId)
		{
			InfoBlueprint.IconId = spriteId;
			InfoBlueprint.Write();
			RefreshInfoIcon();
		}
		void ClearCurrentInfoBlueprintIcon()
		{
			InfoBlueprint.IconId = null;
			InfoBlueprint.Write();
			RefreshInfoIcon();
		}

		void CreateConfirmDialogue(string title, string text)
		{
			DialogueOpen(true);
			DialogUtil.CreateConfirmDialog(title, text, on_confirm: () => DialogueOpen(false));
		}

		void TryImportBlueprint()
		{
			if (ModAssets.ImportFromClipboard(out Blueprint bp))
			{
				CreateConfirmDialogue(BASE64_IMPORT_SUCCESS.TITLE, string.Format(BASE64_IMPORT_SUCCESS.TEXT, bp.FriendlyName));
			}
			else
				CreateConfirmDialogue(BASE64_IMPORT_FAIL.TITLE, BASE64_IMPORT_FAIL.TEXT);
		}

		private void OnClearOverrides()
		{
			ModAssets.ClearReplacementTags();
			TargetBlueprint?.CacheCost();
			SetMaterialState();
		}

		public static void ShowWindow(System.Action<Blueprint> OnClose, Blueprint targetBlueprint, bool showBlueprintList)
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.BlueprintSelectionScreenGO, ModAssets.ParentScreen, true);
				Instance = screen.AddOrGet<BlueprintSelectionScreen>();
				Instance.Init();
			}
			Instance.TargetBlueprint = targetBlueprint;
			Instance.onCloseAction = OnClose;
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.ClearUIState();
			Instance.OpenedFromSnapshot = !showBlueprintList;
			Instance.CreateNewBlueprintFromOverrides.SetInteractable(showBlueprintList);
			Instance.BlueprintsList.gameObject.SetActive(showBlueprintList);
		}
		private void ClearSearchbars()
		{
			BlueprintSearchbar.Text = string.Empty;
			ReplacementElementSearchbar.Text = string.Empty;
		}

		private bool init;

		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				SgtLogger.l("BlueprintSelectionScreen consume esc.");
				if (!DialogueCurrentlyOpen)
				{
					this.Show(false);
				}
			}
			if (e.TryConsume(Action.Find))
			{
				BlueprintSearchbar.ExternalStartEditing();
			}
			base.OnKeyDown(e);
		}

		public void ClearUIState()
		{
			ClearSearchbars();
			UpdateBlueprintButtons();
			ToReplaceTag = null;
			SetMaterialState();
		}

		void ShowElements(bool show)
		{
			if (BlueprintInfo.activeInHierarchy && show)
				ShowInfo(false);

			BlueprintsElements.SetActive(show);
		}
		void ShowInfo(bool show)
		{
			if (BlueprintsElements.activeInHierarchy && show)
				ShowElements(false);


			BlueprintInfo.SetActive(show);
			BlueprintInfoBuildingList.SetActive(show);
			if (show)
			{
				BuildingListSearchbar.Text = string.Empty;
				UpdateBuildingButtons();
			}
		}

		void RefreshInfoIcon()
		{
			if (InfoBlueprint == null || BlueprintIconDisplay == null)
				return;

			if (!InfoBlueprint.IconId.IsNullOrWhiteSpace())
			{
				BlueprintIconDisplay.sprite = ModAssets.GetBlueprintIconSprite(InfoBlueprint.IconId);

				if (InfoBlueprint.IconTintHex.IsNullOrWhiteSpace())
				{
					BlueprintIconDisplay.color = Color.white;
				}
				else
				{
					BlueprintIconDisplay.color = Util.ColorFromHex(InfoBlueprint.IconTintHex);
				}
			}
			else
			{
				BlueprintIconDisplay.sprite = null;
				BlueprintIconDisplay.color = Color.white;
			}

			if (BlueprintEntries.TryGetValue(InfoBlueprint, out var uiCmp))
				uiCmp.RefreshIcon();
		}

		void LoadBlueprintDescription()
		{
			DescriptionInput.SetTextFromData(InfoBlueprint?.UserDescription);
		}
		void SaveBlueprintDescription()
		{
			if (InfoBlueprint == null)
				return;

			InfoBlueprint.UserDescription = DescriptionInput.Text;
			InfoBlueprint.Write();
			if (BlueprintEntries.TryGetValue(InfoBlueprint, out var uiCmp))
				uiCmp.RefreshTooltip();
		}

		void SetMaterialState()
		{
			int allMaterialsState = 0;
			ShowReplacementItems(false);
			bool targetSet = TargetBlueprint != null && !ShowInfoScreen;
			ShowInfo(InfoBlueprint != null && ShowInfoScreen);
			ShowElements(targetSet);
			if (ShowInfoScreen)
			{
				BlueprintName.SetText(InfoBlueprint.FriendlyName);
				var dimensions = InfoBlueprint.VisibleDimensions;
				DimensionInfo.SetText($"{dimensions.X} x {dimensions.Y}");
				BuildingCount.SetText(InfoBlueprint.BuildingConfigurations.Count.ToString());
				DigCount.SetText(InfoBlueprint.DigLocations.Count.ToString());
				NoteCount.SetText(InfoBlueprint.WorldNotes.Count.ToString());

				RefreshInfoIcon();
				LoadBlueprintDescription();
			}
			else if (targetSet)
			{
				MaterialHeaderTitle.SetText(string.Format(MATERIALSWITCH.MATERIALSHEADER.LABEL, TargetBlueprint.FriendlyName));

				foreach (var prev in ElementEntries)
				{
					prev.Value.gameObject.SetActive(false);
				}

				var blueprintMaterials = TargetBlueprint.BlueprintCost.OrderByDescending(kvp => kvp.Value).ToList();

				NoItems.SetActive(blueprintMaterials.Count() == 0);
				foreach (var kvp in blueprintMaterials)
				{
					var selectedAndCategory = kvp.Key;

					Tag replacementTag = null;
					var uiEntry = AddOrGetBlueprintElementEntry(kvp.Key);
					uiEntry.gameObject.SetActive(true);
					int materialState = uiEntry.Refresh(TargetBlueprint);
					if (materialState > allMaterialsState)
						allMaterialsState = materialState;
					uiEntry.SetTotalAmount(kvp.Value);
					uiEntry.transform.SetAsLastSibling();

				}
			}
			switch (allMaterialsState)
			{
				case 0:
					WarningGO.SetActive(false);
					ErrorGO.SetActive(false);
					break;
				case 1:
					WarningGO.SetActive(false);
					ErrorGO.SetActive(true);
					break;
				case 2:
					WarningGO.SetActive(true);
					ErrorGO.SetActive(false);
					break;
			}
		}
		void UpdateBuildingButtons()
		{
			foreach (var kvp in BuildingInfoEntries)
			{
				if (kvp.Value != null)
				{
					kvp.Value.gameObject.SetActive(false);
				}
			}

			if (InfoBlueprint == null)
				return;

			var buildingIds = InfoBlueprint.GetBuildingCounts().OrderByDescending(b => b.Value);
			foreach (var buildingWithCount in buildingIds)
			{
				var uiEntry = AddOrGetBuildingInfoEntry(buildingWithCount.Key);
				uiEntry.transform.SetAsLastSibling();
				uiEntry.gameObject.SetActive(true);
				uiEntry.SetBuildingCount(buildingWithCount.Value);
			}
		}

		void UpdateBlueprintButtons()
		{
			foreach (var kvp in BlueprintEntries)
			{
				if (kvp.Value != null)
				{
					kvp.Value.gameObject.SetActive(false);
				}

			}
			foreach (var kvp in FolderEntries)
			{
				if (kvp.Value != null)
					kvp.Value.gameObject.SetActive(false);
			}

			var targetFolder = ModAssets.SelectedFolder;
			bool root = targetFolder == null;
			FolderUpBtn.SetInteractable(!root);
			SgtLogger.l("rebuilding folders");
			if (root)
			{
				targetFolder = ModAssets.BlueprintFileHandling.RootFolder;

				var folders = ModAssets.BlueprintFileHandling.BlueprintFolders.OrderBy(f => f.Name);
				foreach (var folder in folders)
				{
					var uiEntry = AddOrGetFolderEntry(folder);
					uiEntry.transform.SetAsLastSibling();
					uiEntry.gameObject.SetActive(true);
				}

				//SgtLogger.l(ModAssets.BlueprintFileHandling.BlueprintFolders.Count + "", "folder count");
			}
			else
				SgtLogger.l("not root");

			//SgtLogger.l(targetFolder.BlueprintCount + "", "count");
			var bps = SortBlueprintsBy switch
			{
				OrderBy.NameAscending => targetFolder.Blueprints.OrderBy(bp => bp.FriendlyName),
				OrderBy.NameDescending => targetFolder.Blueprints.OrderByDescending(bp => bp.FriendlyName),
				OrderBy.CreationDateAscending => targetFolder.Blueprints.OrderBy(bp => targetFolder.GetBlueprintIndex(bp)),
				OrderBy.CreationDateDescending => targetFolder.Blueprints.OrderByDescending(bp => targetFolder.GetBlueprintIndex(bp)),
				_ => targetFolder.Blueprints.OrderBy(bp => bp.FriendlyName),
			};

				
			foreach (var bp in bps)
			{
				var uiEntry = AddOrGetBlueprintEntry(bp);
				uiEntry.transform.SetAsLastSibling();
				uiEntry.gameObject.SetActive(true);
				uiEntry.SetSelected(bp == TargetBlueprint);
			}
			this.ConsumeMouseScroll = true;
			StartCoroutine(ToggleCamLock(true));
		}

		public void LockCam()
		{
			Task.Run(() =>
			{
				Task.Delay(25);
				if (this.isActive)
				{
					CameraController.Instance.DisableUserCameraControl = true;
				}
			});
		}
		IEnumerator ToggleCamLock(bool lockCam)
		{
			yield return null;
			CameraController.Instance.DisableUserCameraControl = lockCam;
		}
		public void UnlockCam()
		{
			Task.Run(() =>
			{
				Task.Delay(30);
				CameraController.Instance.DisableUserCameraControl = false;
			});
		}

		private void OnAdvancedReplacementToggleChanged(bool enabled)
		{
			BlueprintState.AdvancedMaterialReplacement = enabled;
			TargetBlueprint?.CacheCost();
			ClearUIState();
		}

		public void SelectFolder(BlueprintFolder folder)
		{
			ModAssets.SelectedFolder = folder;
			UpdateBlueprintButtons();
		}
		public void ApplyReplacementMaterialUI(BlueprintSelectedMaterial original)
		{
			if (ElementEntries.TryGetValue(original, out var UIcmp))
			{
				UIcmp.Refresh(TargetBlueprint);
			}
		}
		private BlueprintElementEntry AddOrGetBlueprintElementEntry(BlueprintSelectedMaterial elementTag)
		{
			if (!ElementEntries.ContainsKey(elementTag))
			{
				var BPelementEntry = Util.KInstantiateUI<BlueprintElementEntry>(ElementEntryPrefab.gameObject, ElementEntryContainer);
				BPelementEntry.SelectedAndCategory = elementTag;
				//folderEntry.Name = folder.Name;
				BPelementEntry.OnEntryClicked = StartSelectingReplacementTag;
				ElementEntries[elementTag] = BPelementEntry;
			}
			return ElementEntries[elementTag];
		}
		private FolderHierarchyEntry AddOrGetFolderEntry(BlueprintFolder folder)
		{
			if (!FolderEntries.ContainsKey(folder))
			{
				var folderEntry = Util.KInstantiateUI<FolderHierarchyEntry>(HierarchyFolderPrefab.gameObject, HierarchyContainer);
				folderEntry.folder = folder;
				//folderEntry.Name = folder.Name;
				folderEntry.OnEntryClicked += () => SelectFolder(folder);
				FolderEntries[folder] = folderEntry;
			}
			return FolderEntries[folder];
		}

		private BuildingInfoEntry AddOrGetBuildingInfoEntry(string buildingId)
		{
			if (!BuildingInfoEntries.ContainsKey(buildingId))
			{
				BuildingInfoEntry bpEntry = Util.KInstantiateUI<BuildingInfoEntry>(BuildingInfoEntryPrefab.gameObject, BuildingInfoContainer);
				bpEntry.SetBuilding(buildingId);
				BuildingInfoEntries[buildingId] = bpEntry;
			}
			return BuildingInfoEntries[buildingId];
		}
		private FileHierarchyEntry AddOrGetBlueprintEntry(Blueprint blueprint)
		{
			if (!BlueprintEntries.ContainsKey(blueprint))
			{
				var bpEntry = Util.KInstantiateUI<FileHierarchyEntry>(HierarchyEntryPrefab.gameObject, HierarchyContainer);
				bpEntry.blueprint = blueprint;
				//folderEntry.Name = folder.Name;
				//folderEntry.OnSelectFolder = OnSelectFolder(folder);
				bpEntry.OnRenamed = (_) => UpdateBlueprintButtons();
				bpEntry.OnMoved = (_) => OnBlueprintMoved();
				bpEntry.OnDeleted = OnBlueprintDeleted;
				bpEntry.OnDialogueToggled = DialogueOpen;
				bpEntry.OnSelectBlueprint = OnSelectBlueprint;
				bpEntry.OnInfoClicked = OnShowBlueprintInfo;

				BlueprintEntries[blueprint] = bpEntry;
			}
			return BlueprintEntries[blueprint];
		}

		void OnCreateFromOverrides()
		{
			if (TargetBlueprint == null)
				return;

			var clone = TargetBlueprint.GetClone();
			clone.ApplyGlobalMaterialOverrides();
			OpenRenameDialogue(clone, true);
		}
		void OpenRenameDialogue(Blueprint blueprint, bool cloneCreation = false)
		{
			DialogueOpen(true);
			var RenameAction = (string result) =>
			{
				DialogueOpen(false);
				if (result == blueprint.FriendlyName && !cloneCreation)
					return;
				blueprint.Rename(result);
				if (cloneCreation)
					ModAssets.BlueprintFileHandling.HandleBlueprintLoading(blueprint.FilePath);
			};
			DialogUtil.CreateTextInputDialog(STRINGS.UI.DIALOGUE.RENAMEBLUEPRINT_TITLE, blueprint.FriendlyName, null, false, RenameAction, () => DialogueOpen(false), ModAssets.ParentScreen, true, false);
		}

		void OnPlaceBlueprint()
		{
			if (onCloseAction != null)
				onCloseAction(TargetBlueprint);
			TargetBlueprint = null;
			Show(false);
		}
		void OnCloseClicked()
		{
			if (onCloseAction != null)
				onCloseAction(ModAssets.SelectedBlueprint);
			TargetBlueprint = null;
			Show(false);
		}

		void OnShowBlueprintInfo(Blueprint bp)
		{
			ShowInfoScreen = true;
			if (bp != InfoBlueprint)
			{
				InfoBlueprint = bp;
				InfoBlueprint.CacheCost();
				RefreshEntryHighlight();
			}
			SetMaterialState();
		}

		void OnSelectBlueprint(Blueprint bp)
		{
			ShowInfoScreen = false;
			if (bp != TargetBlueprint)
			{
				TargetBlueprint = bp;
				TargetBlueprint.CacheCost();
				RefreshEntryHighlight();
				foreach (var prev in ElementEntries)
				{
					prev.Value.SetSelected(false);
				}
			}
			SetMaterialState();
		}

		void RefreshEntryHighlight()
		{
			foreach (var prev in BlueprintEntries)
			{
				prev.Value.SetSelected(prev.Key == TargetBlueprint || prev.Key == InfoBlueprint);
			}
		}

		private void DialogueOpen(bool isOpen)
		{
			DialogueCurrentlyOpen = isOpen;
		}

		void OnBlueprintMoved()
		{
			if (ModAssets.SelectedFolder != null && !ModAssets.SelectedFolder.HasBlueprints)
				SelectFolder(null);
			else
				UpdateBlueprintButtons();
		}
		void OnBlueprintDeleted(Blueprint bp)
		{

			if (bp != null)
			{
				if (BlueprintEntries.TryGetValue(bp, out var uientry))
				{
					UnityEngine.Object.Destroy(uientry.gameObject);
					BlueprintEntries.Remove(bp);
				}
				ModAssets.BlueprintFileHandling.DeleteBlueprint(bp);
			}

			if (bp == TargetBlueprint)
				TargetBlueprint = null;
			if (bp == InfoBlueprint)
				InfoBlueprint = null;

			ClearUIState();
		}

		public static bool HasReplacementCandidates(Tag original) => ModAssets.GetValidMaterials(original).Count() > 1;

		BlueprintSelectedMaterial ToReplaceTag = null;
		List<ReplaceElementEntry> PreviouslyActiveMaterialReplacementButtons = new();
		private void SetReplacementMaterials(BlueprintSelectedMaterial materialTypeTag, float amount)
		{
			ToReplaceTag = materialTypeTag;
			ToReplaceName.SetText(ToReplaceTag.LocalizedCategoryTag());
			foreach (var prev in PreviouslyActiveMaterialReplacementButtons)
			{
				prev.gameObject.SetActive(false);
			}
			PreviouslyActiveMaterialReplacementButtons.Clear();

			var replacementTags = ModAssets.GetValidMaterials(materialTypeTag.CategoryTag);

			foreach (var replacementTag in replacementTags)
			{
				var btn = AddOrGetReplaceMaterialContainer(replacementTag);
				if (btn == null)
				{
					SgtLogger.logError(replacementTag + " go was null!");
					continue;
				}
				PreviouslyActiveMaterialReplacementButtons.Add(btn);
				btn.gameObject.SetActive(true);

				if (ModAssets.TryGetReplacementTag(ToReplaceTag, out var cachedReplacement))
					btn.Refresh(TargetBlueprint, amount, ToReplaceTag.SelectedTag, cachedReplacement);
				else
					btn.Refresh(TargetBlueprint, amount, ToReplaceTag.SelectedTag);
			}
		}
		private ReplaceElementEntry AddOrGetReplaceMaterialContainer(Tag material)
		{
			if (!ReplacementElementEntries.ContainsKey(material))
			{
				var elementEntry = Util.KInstantiateUI<ReplaceElementEntry>(ReplacementElementsPrefab.gameObject, ReplacementElementsContainer);
				elementEntry.targetTag = material;
				elementEntry.OnSelectElement = OnSelectReplacementTag;
				ReplacementElementEntries[material] = elementEntry;
			}
			return ReplacementElementEntries[material];
		}
		private void OnSelectReplacementTag(Tag replacement)
		{
			ModAssets.AddOrSetReplacementTag(ToReplaceTag, replacement);
			TargetBlueprint.CacheCost();
			ApplyReplacementMaterialUI(ToReplaceTag);
			ToReplaceTag = null;
			ShowReplacementItems(false);
			SetMaterialState();
		}

		public void StartSelectingReplacementTag(BlueprintSelectedMaterial materialToReplace, float amount)
		{
			ShowReplacementItems(true);
			foreach (var prev in ElementEntries)
			{
				prev.Value.SetSelected(prev.Key == materialToReplace);
			}
			SetReplacementMaterials(materialToReplace, amount);
		}

		void ShowReplacementItems(bool show)
		{
			ReplaceBlueprintElements.SetActive(show);
		}

		private GameObject AddUiContainer(GameObject prefab, GameObject parent, string name, string description, System.Action onClickAction, Color overrideColor = default, Sprite placeImage = null)
		{

			var PresetHolder = Util.KInstantiateUI(prefab, parent, true);

			UIUtils.TryChangeText(PresetHolder.transform, "Label", name);
			if (description != null && description.Length > 0)
			{
				UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("Label"), description, true, onBottom: true);
			}
			if (placeImage != null)
			{
				var image = PresetHolder.transform.Find("Image").gameObject.AddOrGet<Image>();
				image.sprite = placeImage;
				UnityEngine.Rect rect = image.sprite.rect;
				if (rect.width > rect.height)
				{
					var size = (rect.height / rect.width) * 55;
					image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				}
				else
				{
					var size = (rect.width / rect.height) * 55;
					image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				}
			}

			PresetHolder.transform.gameObject.AddOrGet<FButton>().OnClick += onClickAction;
			if (overrideColor != default)
				PresetHolder.transform.Find("Background").gameObject.AddOrGet<Image>().color = overrideColor;

			return PresetHolder;
		}


		public void ApplyElementsFilter(string filterstring = "")
		{
			foreach (ReplaceElementEntry entry in PreviouslyActiveMaterialReplacementButtons)
			{
				entry.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, entry.Name));
			}
		}
		public void ApplyBuildingsFilter(string filterstring = "")
		{
			if (filterstring.Length == 0)
			{
				UpdateBuildingButtons();
				return;
			}

			foreach (var go in BuildingInfoEntries)
			{
				go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Value.BuildingName));
			}
		}

		public void ApplyBlueprintFilter(string filterstring = "")
		{
			if (filterstring.Length == 0)
			{
				UpdateBlueprintButtons();
				return;
			}

			foreach (var go in BlueprintEntries)
			{
				go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Key.FriendlyName));
			}
		}

		bool ShowInFilter(string filtertext, string stringsToInclude)
		{
			return ShowInFilter(filtertext, new string[] { stringsToInclude });
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


		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!init)
			{
				Init();
			}
			CurrentlyActive = show;
			if (!show)
			{
				UnlockCam();
			}
		}

		internal static void RefreshOnBpChanges()
		{
			if (Instance != null && Instance.CurrentlyActive)
			{
				Instance.RefreshRequested = true;
			}
		}
		public bool RefreshRequested = false;
		public void Render1000ms(float dt)
		{
			if (!RefreshRequested)
				return;

			RefreshRequested = false;
			ClearUIState();
		}

		internal static bool HasBlueprintSelected()
		{
			if (Instance == null)
				return false;
			return Instance.TargetBlueprint != null;
		}
	}
}

