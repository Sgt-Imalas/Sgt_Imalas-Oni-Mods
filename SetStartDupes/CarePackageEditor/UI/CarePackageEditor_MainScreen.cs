using HarmonyLib;
using UnlockConditions;
using SetStartDupes.DuplicityEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static SetStartDupes.STRINGS.UI.CAREPACKAGEEDITOR.HORIZONTALLAYOUT.ITEMINFO.SCROLLAREA.CONTENT;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS;
using static SetStartDupes.STRINGS.UI.CAREPACKAGEEDITOR.HORIZONTALLAYOUT.OBJECTLIST;

namespace SetStartDupes.CarePackageEditor.UI
{
	public class CarePackageEditor_MainScreen : FScreen
	{
		public static CarePackageEditor_MainScreen Instance;
		public CarePackageOutline SelectedOutline = null;

		public FToggle DisplayVanillaPackagesToggle;
		bool VanillaCarePackagesShown = false;

		public FInputField2 FilterBar;
		public FButton ClearFilterButton;


		public FInputField2 AddCarePackageInput;
		public FButton AddCarePackageConfirm;

		public GameObject OutlineEntryContainer;
		public GameObject OutlineEntryPrefab;
		public GameObject VanillaOutlineEntryPrefab;

		public LocText SelectedEntryNameDisplay;
		public Image SelectedEntryPreviewImage;
		public FInputField2 AmountInput;
		public LocText AmountUnitLabel;
		public LocText RequiredDlcsText;


		FInputField2 UnlockAtCycleNumberInput;
		FToggle UnlockAtCycleEnabled;
		FToggle UnlockDiscoveredEnabled;


		GameObject Details;


		Dictionary<CarePackageOutline, CarePackageOutlineEntry> OutlineEntries = new();
		Dictionary<CarePackageOutline, VanillaCarePackageOutlineEntry> VanillaOutlineEntries = new();

		public static void ShowCarePackageEditor(object obj)
		{
			SgtLogger.l("Opening Care Package Editor");
			ShowWindow();
		}
		public static void ShowWindow()
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.CarePackageEditorWindowPrefab, FrontEndManager.Instance.gameObject, true);
				Instance = screen.AddOrGet<CarePackageEditor_MainScreen>();
				Instance.Init();
				Instance.name = "DSS_CarePackageEditor_MainScreen";
			}
			//Instance.SetOpenedType(currentGroup, currentTrait, DupeTraitManager, openedFrom);
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.ClearFilter();
			Instance.SelectOutline(null);
		}

		public void UpdateEntryList()
		{
			foreach (var outline in CarePackageOutlineManager.GetVanillaCarePackageOutlines())
			{
				var uiElement = AddOrGetVanillaCarePackageOutlineUIEntry(outline);
				uiElement.UpdateUI();
			}
			foreach (var outline in CarePackageOutlineManager.GetExtraCarePackageOutlines())
			{
				var uiElement = AddOrGetCarePackageOutlineUIEntry(outline);
				uiElement.UpdateUI();
			}
			SortEntryList();
		}
		public void SortEntryList()
		{
			var editorOutlines = CarePackageOutlineManager.GetExtraCarePackageOutlines().OrderBy((outline) => outline.GetItemName()).ToHashSet();
			foreach (var outline in editorOutlines)
			{
				var uiElement = AddOrGetCarePackageOutlineUIEntry(outline);
				uiElement.transform.SetAsLastSibling();
			}
		}

		void TryResetEntries()
		{
			DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.CAREPACKAGEEDITOR.RESETALLPACKAGES.TITLE, STRINGS.UI.CAREPACKAGEEDITOR.RESETALLPACKAGES.TEXT,on_confirm: ResetEntries, on_cancel: () => { });
		}
		public void ResetEntries()
		{
			SelectedOutline = null;
			CarePackageOutlineManager.ResetExtraCarePackages();
			ResetEntriesUI();
		}
		void ResetEntriesUI()
		{
			foreach (var entry in OutlineEntries)
			{
				Util.KDestroyGameObject(entry.Value.gameObject);
			}
			OutlineEntries.Clear();
			foreach (var entry in VanillaOutlineEntries)
			{
				Util.KDestroyGameObject(entry.Value.gameObject);
			}
			VanillaOutlineEntries.Clear();
			UpdateEntryList();
			Instance.ApplyCarePackageFilter(FilterBar.Text);
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

			OutlineEntryPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content/PresetEntryPrefab").gameObject;
			OutlineEntryPrefab.AddOrGet<CarePackageOutlineEntry>();
			OutlineEntryPrefab.SetActive(false);

			VanillaOutlineEntryPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content/PresetEntryUneditablePrefab").gameObject;
			VanillaOutlineEntryPrefab.AddOrGet<VanillaCarePackageOutlineEntry>();
			VanillaOutlineEntryPrefab.SetActive(false);


			OutlineEntryContainer = OutlineEntryPrefab.transform.parent.gameObject;



			UIUtils.AddSimpleTooltipToObject(transform.Find("HorizontalLayout/ObjectList/ShowVanilla/Label").gameObject, SHOWVANILLA.TOOLTIP);
			DisplayVanillaPackagesToggle = transform.Find("HorizontalLayout/ObjectList/ShowVanilla/Checkbox").gameObject.AddOrGet<FToggle>();
			DisplayVanillaPackagesToggle.SetCheckmark("Checkmark");
			DisplayVanillaPackagesToggle.OnClick += SetVanillaCarePackagesEnabled;
			DisplayVanillaPackagesToggle.SetOn(VanillaCarePackagesShown);

			Details = transform.Find("HorizontalLayout/ItemInfo").gameObject;


			UnlockDiscoveredEnabled = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ItemDiscovered/Checkbox").gameObject.AddOrGet<FToggle>();
			UnlockDiscoveredEnabled.SetCheckmark("Checkmark");
			UnlockDiscoveredEnabled.OnClick += ToggleItemDiscoveredCondition;

			UnlockAtCycleNumberInput = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/UnlockAtCycle/Input").FindOrAddComponent<FInputField2>();
			UnlockAtCycleNumberInput.Text = "1";
			UnlockAtCycleNumberInput.OnValueChanged.AddListener(UpdateItemCycleUnlockNumber);
			UnlockAtCycleEnabled = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/UnlockAtCycle/Checkbox").gameObject.AddOrGet<FToggle>();
			UnlockAtCycleEnabled.SetCheckmark("Checkmark");
			UnlockAtCycleEnabled.OnClick += ToggleItemCycleCondition;

			AmountInput = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/AmountInput/Input").FindOrAddComponent<FInputField2>();
			AmountInput.OnValueChanged.AddListener(UpdateItemCount);
			AmountUnitLabel = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/AmountInput/AmountLabel").FindOrAddComponent<LocText>();

			FilterBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
			FilterBar.OnValueChanged.AddListener(ApplyCarePackageFilter);
			FilterBar.Text = string.Empty;

			AddCarePackageInput = transform.Find("HorizontalLayout/ObjectList/CarePackageItemId").FindOrAddComponent<FInputField2>();
			AddCarePackageInput.Text = string.Empty;
			AddCarePackageInput.OnValueChanged.AddListener(RefreshAddButton);
			AddCarePackageConfirm = transform.Find("HorizontalLayout/ObjectList/AddCarePackageBtn").FindOrAddComponent<FButton>();
			AddCarePackageConfirm.OnClick += () => TryCreateNewOutline();

			ClearFilterButton = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
			ClearFilterButton.OnClick += () => FilterBar.Text = string.Empty;

			SelectedEntryNameDisplay = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/TitleWithIcon/Label").gameObject.GetComponent<LocText>();
			SelectedEntryPreviewImage = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/TitleWithIcon/IconContainer/Icon").gameObject.GetComponent<Image>();
			RequiredDlcsText = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/RequiredDlcs/Value").gameObject.GetComponent<LocText>();

			UpdateEntryList();
			SelectOutline(null);
			RefreshAddButton(null);
		}
		void RefreshAddButton(object _)
		{
			AddCarePackageConfirm.SetInteractable(AddCarePackageInput.Text.Length > 0);
		}

		void TryCreateNewOutline()
		{
			string itemID = AddCarePackageInput.Text;
			if (itemID.IsNullOrWhiteSpace())
				return;

			if (Assets.GetPrefab(itemID))
			{
				CarePackageOutlineManager.AddNewCarePackage(itemID);
				DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.CAREPACKAGEEDITOR.CREATECAREPACKAGEPOPUP.TITLE, string.Format(STRINGS.UI.CAREPACKAGEEDITOR.CREATECAREPACKAGEPOPUP.SUCCESS, CarePackageItemHelper.GetSpawnableName(itemID)));
			}
			else if(NameIdHelper.TryGetIdFromName(itemID, out var id) && Assets.GetPrefab(id))
			{
				if (Assets.GetPrefab(id))
				{
					CarePackageOutlineManager.AddNewCarePackage(id);
					DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.CAREPACKAGEEDITOR.CREATECAREPACKAGEPOPUP.TITLE, string.Format(STRINGS.UI.CAREPACKAGEEDITOR.CREATECAREPACKAGEPOPUP.SUCCESS, CarePackageItemHelper.GetSpawnableName(id)));
				}
			}
			else
			{
				DialogUtil.CreateConfirmDialogFrontend(STRINGS.UI.CAREPACKAGEEDITOR.CREATECAREPACKAGEPOPUP.TITLE, string.Format(STRINGS.UI.CAREPACKAGEEDITOR.CREATECAREPACKAGEPOPUP.INVALIDID, itemID));
			}
			AddCarePackageInput.Text = (string.Empty);
		}
		public void ClearFilter()
		{
			FilterBar.Text = string.Empty;
			ApplyCarePackageFilter();
		}

		public void ApplyCarePackageFilter(string filterstring = "")
		{
			foreach (var go in OutlineEntries)
			{
				go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Key.GetDescriptionString()));
			}
			foreach (var go in VanillaOutlineEntries)
			{
				if(!VanillaCarePackagesShown)
					go.Value.gameObject.SetActive(false);
				else
					go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Key.GetDescriptionString()));
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

		void OnOutlineEntryUpdated(CarePackageOutline target = null)
		{
			if (target == null)
				target = SelectedOutline;

			AddOrGetCarePackageOutlineUIEntry(target).UpdateUI();
			CarePackageOutlineManager.SaveCarePackagesToFile();
		}
		public void ToggleItemDiscoveredCondition(bool enabled)
		{
			if (enabled)
			{
				SelectedOutline?.AddOrUpdateUIDiscoveredCondition();
			}
			else
			{
				SelectedOutline?.RemoveUIDiscoveredCondition();
			}
			OnOutlineEntryUpdated();
		}
		void UpdateItemCount (string text)
		{
			if (SelectedOutline == null)
				return;
			if (!int.TryParse(text, out var amount))
				return;
			SelectedOutline.Amount = amount;
			OnOutlineEntryUpdated();
		}
		void UpdateItemCycleUnlockNumber(string text)
		{
			if (SelectedOutline == null || !SelectedOutline.HasUICycleCondition(out _))
				return;

			if (int.TryParse(UnlockAtCycleNumberInput.Text, out int cycle))
				SelectedOutline?.AddOrUpdateUICycleCondition(cycle);
			OnOutlineEntryUpdated();
		}
		public void ToggleItemCycleCondition(bool enabled)
		{
			if (enabled)
			{
				if (int.TryParse(UnlockAtCycleNumberInput.Text, out int cycle))
					SelectedOutline?.AddOrUpdateUICycleCondition(cycle);
				else
					UnlockAtCycleEnabled.SetOnFromCode(false);
			}
			else
			{
				SelectedOutline?.RemoveUICycleCondition();
			}
			OnOutlineEntryUpdated();
		}

		public void SetVanillaCarePackagesEnabled(bool enabled)
		{
			VanillaCarePackagesShown = enabled;
			ApplyCarePackageFilter(FilterBar.Text);//ui refresh
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
		public VanillaCarePackageOutlineEntry AddOrGetVanillaCarePackageOutlineUIEntry(CarePackageOutline outline)
		{
			if (VanillaOutlineEntries.TryGetValue(outline, out var entry))
				return entry;

			var newEntry = Util.KInstantiateUI<VanillaCarePackageOutlineEntry>(VanillaOutlineEntryPrefab.gameObject, OutlineEntryContainer, true);
			newEntry.UpdateOutline(outline);
			VanillaOutlineEntries.Add(outline, newEntry);
			return newEntry;

		}
		public CarePackageOutlineEntry AddOrGetCarePackageOutlineUIEntry(CarePackageOutline outline)
		{
			if (OutlineEntries.TryGetValue(outline, out var entry))
				return entry;

			var newEntry = Util.KInstantiateUI<CarePackageOutlineEntry>(OutlineEntryPrefab.gameObject, OutlineEntryContainer, true);
			newEntry.UpdateOutline(outline);
			OutlineEntries.Add(outline, newEntry);
			return newEntry;

		}
		internal void RemoveOutlineEntry(CarePackageOutline targetOutline)
		{
			if(OutlineEntries.TryGetValue(targetOutline, out var UIEntry))
			{
				OutlineEntries.Remove(targetOutline);
				Util.KDestroyGameObject(UIEntry.gameObject);
			}

			if (SelectedOutline == targetOutline)
			{
				SelectedOutline = null;
				RefreshDetails();
			}
		}
		internal void SelectOutline(CarePackageOutline targetOutline)
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
			SelectedEntryNameDisplay.SetText(SelectedOutline.Name);
			var img = SelectedOutline.GetImageWithColor();
			SelectedEntryPreviewImage.sprite = img.first;
			SelectedEntryPreviewImage.color = img.second;
			AmountInput.SetTextFromData(SelectedOutline.Amount.ToString());
			AmountUnitLabel.SetText((ElementLoader.GetElement(SelectedOutline.ItemId.ToTag()) != null) ? global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM : global::STRINGS.UI.UNITSUFFIXES.UNITS);
			RequiredDlcsText.SetText(SelectedOutline.RequiredDlcs != null ? string.Join(", ", SelectedOutline.RequiredDlcs.Select(dlcId => DlcManager.GetDlcTitle(dlcId))) : "-");


			UnlockDiscoveredEnabled.SetOnFromCode(SelectedOutline.HasUIDiscoveredCondition());

			bool unlockAtCycle = SelectedOutline.HasUICycleCondition(out var condition);
			UnlockAtCycleEnabled.SetOnFromCode(unlockAtCycle);
			UnlockAtCycleNumberInput.SetTextFromData(unlockAtCycle ? condition.CycleUnlock.ToString() : "1");
		}
	}
}
