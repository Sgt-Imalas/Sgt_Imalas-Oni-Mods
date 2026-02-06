using DupePrioPresetManager.Serializables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static DupePrioPresetManager.STRINGS.UI.PRESETWINDOWDUPEPRIOS;
using static STRINGS.UI.FRONTEND;

namespace DupePrioPresetManager
{
	internal class UnityPresetScreen_ResearchQueue : FScreen
	{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
		public static UnityPresetScreen_ResearchQueue Instance = null;


		public FButton GeneratePresetButton;
		public FButton CloseButton;
		public FButton ApplyButton;

		public GameObject InfoHeaderPrefab;
		public GameObject InfoRowPrefab;
		public GameObject InfoSpacer;
		public GameObject InfoScreenContainer;

		public GameObject PresetListContainer;
		public GameObject PresetListPrefab;

		public FButton OpenPresetFolder;
		public FButton ClearSearchBar;
		public FInputField2 Searchbar;

		public bool CurrentlyActive = false;
		private bool HoveringPrio = false;

		///Preset
		ResearchQueuePreset CurrentlySelected;
		///Referenced Stats to apply presets to.

		Dictionary<ResearchQueuePreset, GameObject> Presets = new Dictionary<ResearchQueuePreset, GameObject>();
		//List<GameObject> InformationObjects = new List<GameObject>();

		Dictionary<string, GameObject> Techs = [];
		LocText TitleHolder = null;

		string RefName;

		public static void ShowWindow(System.Action onClose, string refName = "")
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, ModAssets.ParentScreen, true);
				Instance = screen.AddOrGet<UnityPresetScreen_ResearchQueue>();
				Instance.Init();
			}
			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.LoadAllPresets();
			Instance.RefName = refName;
			Instance.LoadTemporalPreset();
			Instance.OnCloseAction = onClose;
			Instance.Searchbar.Text = string.Empty;
		}

		private bool init;
		private System.Action OnCloseAction;

		public void LoadTemporalPreset()
		{
			ResearchQueuePreset tempStats = ResearchQueuePreset.CreatePreset(RefName);
			SetAsCurrent(tempStats);
		}

		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				//if (!HoveringPrio)
				//{
				//    this.Show(false);
				//}
			}
			if (e.TryConsume(Action.Escape))
			{
				this.Show(false);
			}
			if (e.TryConsume(Action.Find))
			{
				Searchbar.ExternalStartEditing();
			}
			base.OnKeyDown(e);
		}

		void LoadAllPresets()
		{
			foreach (var existing in Presets.Values)
			{
				Destroy(existing.gameObject);
			}
			Presets.Clear();
			foreach (var loadedPreset in LoadPresets())
			{
				AddUiElementForPreset(loadedPreset);
			}
		}

		List<ResearchQueuePreset> LoadPresets()
		{
			List<ResearchQueuePreset> minionStatConfigs = new List<ResearchQueuePreset>();
			var files = new DirectoryInfo(ModAssets.ResearchTemplatePath).GetFiles();


			for (int i = 0; i < files.Count(); i++)
			{
				var File = files[i];
				try
				{
					if (IO_Utils.ReadFromFile(File, out ResearchQueuePreset preset) && preset.IsValidForCurrentDlc())
					{
						minionStatConfigs.Add(preset);
					}
				}
				catch (Exception e)
				{
					SgtLogger.logError("Couln't load preset from: " + File.FullName + ",\nError: " + e);
				}
			}
			return minionStatConfigs;
		}

		private bool AddUiElementForPreset(ResearchQueuePreset config)
		{
			if (!Presets.ContainsKey(config))
			{
				var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);
				PresetHolder.transform.Find("TraitImage").gameObject.SetActive(false);

				UIUtils.TryChangeText(PresetHolder.transform, "Label", config.ConfigName);
				PresetHolder.transform.Find("RenameButton").FindOrAddComponent<FButton>().OnClick +=
					() => config.OpenPopUpToChangeName(
						() =>
						{
							UIUtils.TryChangeText(PresetHolder.transform, "Label", config.ConfigName);
							RebuildInformationPanel();
						}
						);

				PresetHolder.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>().OnClick += () => SetAsCurrent(config);
				PresetHolder.transform.Find("DeleteButton").FindOrAddComponent<FButton>().OnClick += () => DeletePreset(config);

				UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("RenameButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.RENAMEPRESETTOOLTIP);
				UIUtils.AddSimpleTooltipToObject(PresetHolder.transform.Find("DeleteButton"), STRINGS.UI.PRESETWINDOWDUPEPRIOS.HORIZONTALLAYOUT.OBJECTLIST.SCROLLAREA.CONTENT.PRESETENTRYPREFAB.DELETEPRESETTOOLTIP);
				Presets[config] = PresetHolder;
				return true;
			}
			return false;
		}

		void DeletePreset(ResearchQueuePreset config)
		{
			System.Action Delete = () =>
			{
				if (Presets.ContainsKey(config))
				{
					Destroy(Presets[config]);
					Presets.Remove(config);
					config.DeleteFile();
				}
				RebuildInformationPanel();
			};
			System.Action nothing = () =>
			{ };

			DialogUtil.CreateConfirmDialog(
				string.Format(STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.TITLE, config.ConfigName),
				string.Format(STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.DESC, config.ConfigName),
				STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.YES,
				Delete,
				STRINGS.UI.PRESETWINDOWDUPEPRIOS.DELETEWINDOW.CANCEL
				, nothing
		   , parent: this.gameObject
		   );
		}

		void SetAsCurrent(ResearchQueuePreset config)
		{
			CurrentlySelected = config;
			RebuildInformationPanel();
		}
		void RebuildInformationPanel()
		{
			//for (int i = InformationObjects.Count - 1; i >= 0; i--)
			//{
			//    Destroy(InformationObjects[i]);
			//}
			if (CurrentlySelected == null)
				return;

			TitleHolder.text = CurrentlySelected.ConfigName;
			foreach (var techEntry in Techs)
			{
				techEntry.Value.SetActive(CurrentlySelected.QueuedResearchs.Contains(techEntry.Key));
			}
			GeneratePresetButton.SetInteractable(!Presets.ContainsKey(CurrentlySelected) && CurrentlySelected.QueuedResearchs.Any());
		}


		private void SetAllowedSprite(bool allowed, Image image)
		{
		}


		public string ChoreGroupName(ChoreGroup group)
		{
			return Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + group.Id.ToUpperInvariant() + ".NAME");
		}
		public string ChoreGroupTooltip(ChoreGroup group)
		{
			return Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + group.Id.ToUpperInvariant() + ".DESC");
		}

		private void Init()
		{

			UIUtils.TryChangeText(transform, "Title", TITLERESEARCHQUEUE);

			GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrent").FindOrAddComponent<FButton>();
			CloseButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").FindOrAddComponent<FButton>();
			ApplyButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/ApplyPresetButton").FindOrAddComponent<FButton>();

			OpenPresetFolder = transform.Find("HorizontalLayout/ObjectList/SearchBar/FolderButton").FindOrAddComponent<FButton>();
			OpenPresetFolder.OnClick += () => Process.Start(new ProcessStartInfo(ModAssets.ResearchTemplatePath) { UseShellExecute = true });


			Searchbar = transform.Find("HorizontalLayout/ObjectList/SearchBar/Input").FindOrAddComponent<FInputField2>();
			Searchbar.OnValueChanged.AddListener(ApplyFilter);
			Searchbar.Text = string.Empty;


			ClearSearchBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();
			ClearSearchBar.OnClick += () => Searchbar.Text = string.Empty;

			ApplyButton.OnClick += () =>
			{
				CurrentlySelected.ApplyPreset();
				this.OnCloseAction.Invoke();
				this.Show(false);
			};
			///OpenFolder

			CloseButton.OnClick += () => this.Show(false);
			GeneratePresetButton.OnClick += () =>
			{
				if (!Presets.ContainsKey(CurrentlySelected))
				{
					CurrentlySelected.OpenPopUpToChangeName(
							() =>
							{
								AddUiElementForPreset(CurrentlySelected);
								if (this.CurrentlyActive && Presets[CurrentlySelected] != null)
								{
									UIUtils.TryChangeText(Presets[CurrentlySelected].transform, "Label", CurrentlySelected.ConfigName);
									RebuildInformationPanel();
								}
							}
							);
					RebuildInformationPanel();
				}
			};


			UIUtils.AddSimpleTooltipToObject(GeneratePresetButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.GENERATEFROMCURRENT.TOOLTIP);
			UIUtils.AddSimpleTooltipToObject(CloseButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.CLOSEBUTTON.TOOLTIP);
			UIUtils.AddSimpleTooltipToObject(ApplyButton.transform, HORIZONTALLAYOUT.ITEMINFO.BUTTONS.APPLYPRESETBUTTON.TOOLTIP);

			UIUtils.AddSimpleTooltipToObject(ClearSearchBar.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.CLEARTOOLTIP);
			UIUtils.AddSimpleTooltipToObject(OpenPresetFolder.transform, HORIZONTALLAYOUT.OBJECTLIST.SEARCHBAR.OPENFOLDERTOOLTIP);

			InfoHeaderPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/HeaderPrefab").gameObject;
			InfoRowPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ListViewEntryPrefab").gameObject;
			InfoSpacer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ItemPrefab").gameObject;
			UIUtils.FindAndDestroy(InfoSpacer.transform, "Label");
			InfoScreenContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content").gameObject;
			PresetListContainer = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content").gameObject;
			PresetListPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content/PresetEntryPrefab").gameObject;
			///Filling the items
			var Name = Util.KInstantiateUI(InfoHeaderPrefab, InfoScreenContainer, true);
			//UIUtils.TryChangeText(Name.transform, "Label", "\"" + CurrentlySelected.ConfigName + "\"");
			TitleHolder = Name.transform.Find("Label").GetComponent<LocText>();

			//InformationObjects.Add(Name);

			foreach (var tech in Db.Get().Techs.resources)
			{
				var techEntry = Util.KInstantiateUI(InfoRowPrefab, InfoScreenContainer, true);
				
				UIUtils.TryChangeText(techEntry.transform, "Label", tech.Name);
				UIUtils.AddSimpleTooltipToObject(techEntry.transform.Find("Label"), tech.desc, true);

				if (techEntry.transform.Find("Label/TraitImage").TryGetComponent<Image>(out var image))
				{
					image.gameObject.SetActive(false);
				}

				if (techEntry.transform.Find("AddThisTraitButton"))
				{
					techEntry.transform.Find("AddThisTraitButton").gameObject.SetActive(false);
				}

				Techs[tech.Id] = techEntry;

				//InformationObjects.Add(ConsumableAllowedItem);

			}

			init = true;
		}

		public void ApplyFilter(string filterstring = "")
		{
			foreach (var go in Presets)
			{
				go.Value.SetActive(filterstring == string.Empty ? true : go.Key.ConfigName.ToLowerInvariant().Contains(filterstring.ToLowerInvariant()));
			}
		}

		public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!init)
			{
				Init();
			}

			if (show)
			{
				CurrentlyActive = show;
			}
			else
			{
				DeactivateStatusWithDelay(600);
			}
		}
		async Task DeactivateStatusWithDelay(int ms)
		{
			await Task.Delay(ms);
			CurrentlyActive = false;
		}
	}
}
