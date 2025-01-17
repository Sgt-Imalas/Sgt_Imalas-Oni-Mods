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

		public FInputField FilterBar;
		public FButton ClearFilterButton;

		public GameObject OutlineEntryContainer;
		public GameObject OutlineEntryPrefab;

		public LocText SelectedEntryNameDisplay;
		public Image SelectedEntryPreviewImage;
		public FInputField AmountInput;
		public LocText RequiredDlcsText;


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
			Instance.UpdateEntryList();
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

			var newEntry = Util.KInstantiateUI<CarePackageOutlineEntry>(OutlineEntryPrefab.gameObject, OutlineEntryContainer);
			newEntry.TargetOutline = outline;
			OutlineEntries.Add(outline,newEntry);
			return newEntry;

		}

		internal void SelectOutline(CarePackageOutline targetOutline)
		{

		}
	}
}
