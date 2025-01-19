using SetStartDupes.DuplicityEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes.CarePackageEditor.UI
{
	public class CarePackageEditor_MainScreen : FScreen
	{
		public static CarePackageEditor_MainScreen Instance;

		public FToggle DisplayVanillaPackagesToggle;
		bool VanillaCarePackagesShown = false;

		public FInputField2 FilterBar;
		public FButton ClearFilterButton;

		public GameObject OutlineEntryContainer;
		public GameObject OutlineEntryPrefab;

		public LocText SelectedEntryNameDisplay;
		public Image SelectedEntryPreviewImage;
		public FInputField2 AmountInput;
		public LocText RequiredDlcsText;

		FToggle UnlockAtCycleEnabled;
		FToggle UnlockDiscoveredEnabled;


		GameObject Details;


		Dictionary<CarePackageOutline, CarePackageOutlineEntry> OutlineEntries = new();



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
			Instance.ApplyBlueprintFilter();
		}

		public void UpdateEntryList()
		{
			foreach(var outline in CarePackageOutlineManager.GetExtraCarePackageOutlines())
			{
				CarePackageOutlineEntry uiElement = AddOrGetCarePackageOutline(outline);
				uiElement.UpdateUI();
			}
		}

		private bool initialized = false;
		public void Init()
		{
			if (initialized)
				return;
			initialized = true;


			var closeButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").gameObject.AddOrGet<FButton>();
			closeButton.OnClick += () => Show(false);

			OutlineEntryPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content/PresetEntryPrefab").gameObject;
			OutlineEntryPrefab.AddOrGet<CarePackageOutlineEntry>();
			OutlineEntryPrefab.SetActive(false);
			OutlineEntryContainer = OutlineEntryPrefab.transform.parent.gameObject;

			
			DisplayVanillaPackagesToggle = transform.Find("HorizontalLayout/ObjectList/ShowVanilla/Checkbox").gameObject.AddOrGet<FToggle>();
			DisplayVanillaPackagesToggle.SetCheckmark("Checkmark");
			DisplayVanillaPackagesToggle.OnClick += SetVanillaCarePackagesEnabled;
			DisplayVanillaPackagesToggle.SetOn(VanillaCarePackagesShown);

			Details = transform.Find("HorizontalLayout/ItemInfo").gameObject;


			UnlockDiscoveredEnabled = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ItemDiscovered/Checkbox").gameObject.AddOrGet<FToggle>();
			UnlockDiscoveredEnabled.SetCheckmark("Checkmark");
			UnlockDiscoveredEnabled.OnClick += ToggleItemDiscoveredCondition;

			UnlockAtCycleEnabled = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/UnlockAtCycle/Checkbox").gameObject.AddOrGet<FToggle>();
			UnlockAtCycleEnabled.SetCheckmark("Checkmark");
			UnlockAtCycleEnabled.OnClick += ToggleItemCycleCondition;



			FilterBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
			FilterBar.OnValueChanged.AddListener(ApplyBlueprintFilter);
			FilterBar.Text = string.Empty;

			ClearFilterButton = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
			ClearFilterButton.OnClick += () => FilterBar.Text = string.Empty;

			UpdateEntryList();
		}
		public void ApplyBlueprintFilter(string filterstring = "")
		{
			foreach (var go in OutlineEntries)
			{
				go.Value.gameObject.SetActive(filterstring == string.Empty ? true : ShowInFilter(filterstring, go.Key.GetDescriptionString()));
			}
		}

		bool ShowInFilter(string filtertext, string stringsToInclude)
		{
			return ShowInFilter(filtertext, [ stringsToInclude ]);
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


		public void ToggleItemDiscoveredCondition(bool enabled)
		{

		}
		public void ToggleItemCycleCondition(bool enabled)
		{

		}

		public void SetVanillaCarePackagesEnabled(bool enabled)
		{
			VanillaCarePackagesShown = enabled;
			//todo: refresh ui
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
		public CarePackageOutlineEntry AddOrGetCarePackageOutline(CarePackageOutline outline)
		{
			if (OutlineEntries.TryGetValue(outline, out var entry))
				return entry;

			var newEntry = Util.KInstantiateUI<CarePackageOutlineEntry>(OutlineEntryPrefab.gameObject, OutlineEntryContainer,true);
			newEntry.UpdateOutline(outline);
			OutlineEntries.Add(outline,newEntry);
			return newEntry;

		}

		internal void SelectOutline(CarePackageOutline targetOutline)
		{

		}
	}
}
