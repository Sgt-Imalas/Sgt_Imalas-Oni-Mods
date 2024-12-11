using ClusterTraitGenerationManager.ClusterData;
using ClusterTraitGenerationManager.UI.ItemEntryTypes;
using ClusterTraitGenerationManager.UI.SecondaryDisplayTypes;
using ClusterTraitGenerationManager.UI.SO_StarmapEditor;
using Database;
using Klei.AI;
using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.CATEGORIES.FOOTERCONTENT;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER.ASTEROIDTRAITS.CONTENT.TRAITCONTAINER.SCROLLAREA.CONTENT.LISTVIEWENTRYPREFAB;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER.VANILLAPOI_RESOURCES.VANILLAPOI_ARTIFACT;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.FOOTER;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.FOOTER.BUTTONS;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.ITEMSELECTION.VANILLASTARMAPCONTENT.VANILLASTARMAPCONTAINER;
using static CustomGameSettings;

namespace ClusterTraitGenerationManager.UI.Screens
{
	public class CGM_MainScreen_UnityScreen : KModalScreen
	{
		#region MixingSettings

		GameObject MixingSettingsContent;
		GameObject MixingSettingsContainer;
		GameObject MixingSettingsButton;

		Dictionary<string, FCycle> MixingCycleConfigs = new();
		private void SetMixingSetting(SettingConfig ConfigToSet, object valueId)
		{
			string valueToSet = valueId.ToString();
			if (valueId is bool val)
			{
				var toggle = ConfigToSet as ToggleSettingConfig;
				valueToSet = val ? toggle.on_level.id : toggle.off_level.id;
			}
			//SgtLogger.l("changing " + ConfigToSet.id.ToString() + " from " + CustomGameSettings.Instance.GetCurrentMixingSettingLevel(ConfigToSet).id + " to " + valueToSet.ToString());			
            CustomGameSettings.Instance.SetMixingSetting(ConfigToSet, valueToSet);
			RefreshCategories();
        }

		void SetMixingSettingsVisible(bool visible)
		{
			foreach (var item in MixingCycleConfigs.Values)
			{
				item.gameObject.SetActive(visible);
			}
		}

		private void LoadMixingSettings()
		{
			var instance = CustomGameSettings.Instance;
			bool isNoSweat = instance.customGameMode == CustomGameMode.Nosweat;

			foreach (var qualitySetting in instance.MixingSettings)
			{
				string id = qualitySetting.Key;

				if (qualitySetting.Value is DlcMixingSettingConfig)
					continue;

				if (!DlcManager.IsAllContentSubscribed(qualitySetting.Value.required_content))
					continue;
				if(qualitySetting.Value is not MixingSettingConfig setting)
				{
					SgtLogger.warning(qualitySetting.Value.id + " was not a mixing setting!");
					continue;
				}	

				string settingValue = instance.GetCurrentMixingSettingLevel(setting).id;
				if (MixingCycleConfigs.TryGetValue(id, out var settingsCycle))
				{
					settingsCycle.Value = settingValue;
				}
				else
				{
					SgtLogger.warning("uninitialized setting found: " + id);
				}
			}
		}
		public void InitializeMixingSettings()
		{
			SgtLogger.l("initializing mixing settings");
			SgtLogger.Assert("MixingSettingsContainer not assigned", MixingSettingsContainer);
			Debug.Log(MixingSettingsContainer);
			var transform = MixingSettingsContainer.transform;
			MixingSettingsContainer.SetActive(true);
			GameObject CyclePrefab = transform.Find("SwitchPrefab").gameObject;

			SgtLogger.Assert("CyclePrefab missing", CyclePrefab);

			CyclePrefab.SetActive(false);

			foreach (var mixingSetting in CustomGameSettings.Instance.MixingSettings)
			{


				string settingID = mixingSetting.Key;
                if (mixingSetting.Value is DlcMixingSettingConfig)
                        continue;

				if (!DlcManager.IsAllContentSubscribed(mixingSetting.Value.required_content))
					continue;

				if (mixingSetting.Value is MixingSettingConfig listSetting)
				{
					MixingCycleConfigs[settingID] = AddListMixingSettingsContainer(CyclePrefab, MixingSettingsContainer, listSetting);
				}
				else
				{
					SgtLogger.error(mixingSetting.Value.id + " was not a valid mixing setting!");
				}
			}
		}

		public FCycle AddListMixingSettingsContainer(GameObject prefab, GameObject parent, MixingSettingConfig ConfigToSet)
		{
			var cycle = Util.KInstantiateUI(prefab, parent, true).AddOrGet<FCycle>();

			var settingLabel = cycle.transform.Find("Label").gameObject.AddOrGet<LocText>();
			cycle.transform.Find("Image").GetComponent<Image>().sprite = ConfigToSet.icon;
			settingLabel.text = ConfigToSet.label;
			UIUtils.AddSimpleTooltipToObject(settingLabel.transform, ConfigToSet.tooltip, alignCenter: true, onBottom: true);
			cycle.Initialize(
				cycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
				cycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
				cycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>()
				//	,cycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>()
				);

			cycle.Options = new List<FCycle.Option>();
			foreach (var config in ConfigToSet.GetLevels())
			{
				cycle.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
			}
			cycle.OnChange += () =>
			{
				SetMixingSetting(ConfigToSet, cycle.Value);
			};
			return cycle;
		}
		#endregion

		#region CustomGameSettings

		GameObject CustomGameSettingsContent;
		GameObject GameSettingsButton;

		Dictionary<string, FToggle> CustomGameSettingsToggleConfigs = new();
		Dictionary<string, FCycle> CustomGameSettingsCycleConfigs = new();

		void SetGameSettingsVisible(bool visible)
		{
			foreach (var item in CustomGameSettingsToggleConfigs.Values)
			{
				item.gameObject.SetActive(visible);
			}
			foreach (var item in CustomGameSettingsCycleConfigs.Values)
			{
				item.gameObject.SetActive(visible);
			}
		}
		private void LoadGameSettings()
		{
			var instance = CustomGameSettings.Instance;
			bool isNoSweat = instance.customGameMode == CustomGameMode.Nosweat;

			foreach (var qualitySetting in instance.QualitySettings)
			{
				string id = qualitySetting.Key;
				if (id == CustomGameSettingConfigs.WorldgenSeed.id || id == CustomGameSettingConfigs.ClusterLayout.id)
					continue;

				if (!DlcManager.IsAllContentSubscribed(qualitySetting.Value.required_content))
					continue;
				SettingConfig setting = qualitySetting.Value;
				string settingValue = instance.GetCurrentQualitySetting(setting).id;


				if (CustomGameSettingsCycleConfigs.TryGetValue(id, out var settingsCycle))
				{
					settingsCycle.Value = settingValue;
				}
				else if (CustomGameSettingsToggleConfigs.TryGetValue(id, out var settingsToggle))
				{
					settingsToggle.SetOnFromCode(settingValue == (setting as ToggleSettingConfig).on_level.id);
				}
				else
				{
					SgtLogger.warning("uninitialized setting found: " + id);
				}
			}
		}
		private void GetNewRandomSeed() => SetSeed(UnityEngine.Random.Range(0, int.MaxValue).ToString());
		void SetSeed(string seedString)
		{
			string ExistingSeedString = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id;
			if (ExistingSeedString == seedString)
			{
				SgtLogger.l("new seed was the same as old seed, skipping setter");
				return;
			}
			if (ExistingSeedString.Count() == 0)
			{
				SgtLogger.l("seed text was empty");
				RefreshView();
				return;
			}

			if (!int.TryParse(seedString, out int seed))
				return;
			seed = Mathf.Min(seed, int.MaxValue);
			SeedInput_Main.Text = seedString;

			CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.WorldgenSeed, seed.ToString());

			if (RerollStarmapWithSeedChange)
				RebuildStarmap(true);


			RefreshView();
		}

		private void SetCustomGameSettings(SettingConfig ConfigToSet, object valueId)
		{
			string valueToSet = valueId.ToString();
			if (valueId is bool)
			{
				var toggle = ConfigToSet as ToggleSettingConfig;
				valueToSet = (bool)valueId ? toggle.on_level.id : toggle.off_level.id;
			}
			// SgtLogger.l("changing " + ConfigToSet.id.ToString() + " from " + CustomGameSettings.Instance.GetCurrentQualitySetting(ConfigToSet).id + " to " + valueToSet.ToString());
			CustomGameSettings.Instance.SetQualitySetting(ConfigToSet, valueToSet);
		}

		public FToggle AddCheckboxGameSettingsContainer(GameObject prefab, GameObject parent, SettingConfig ConfigToSet)
		{
			var toggle = Util.KInstantiateUI(prefab, parent, true).gameObject.AddOrGet<FToggle>();

			var settingLabel = toggle.transform.Find("Label").gameObject.AddOrGet<LocText>();
			settingLabel.text = ConfigToSet.label;
			UIUtils.AddSimpleTooltipToObject(settingLabel.transform, ConfigToSet.tooltip, alignCenter: true, onBottom: true);

			toggle.SetCheckmark("Background/Checkmark");
			toggle.OnClick += (v) =>
			{
				SetCustomGameSettings(ConfigToSet, v);
			};
			return toggle;
		}
		public FCycle AddListGameSettingsContainer(GameObject prefab, GameObject parent, SettingConfig ConfigToSet)
		{
			var cycle = Util.KInstantiateUI(prefab, parent, true).AddOrGet<FCycle>();

			var settingLabel = cycle.transform.Find("Label").gameObject.AddOrGet<LocText>();
			settingLabel.text = ConfigToSet.label;
			UIUtils.AddSimpleTooltipToObject(settingLabel.transform, ConfigToSet.tooltip, alignCenter: true, onBottom: true);
			cycle.Initialize(
				cycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
				cycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
				cycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
				cycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

			cycle.Options = new List<FCycle.Option>();
			foreach (var config in ConfigToSet.GetLevels())
			{
				cycle.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
			}
			cycle.OnChange += () =>
			{
				SetCustomGameSettings(ConfigToSet, cycle.Value);
			};
			return cycle;
		}

		public void InitializeGameSettings()
		{
			SgtLogger.l("initializing settings");
			SgtLogger.Assert("CustomGameSettingsContainer not assigned", CustomGameSettingsContainer);
			Debug.Log(CustomGameSettingsContainer);
			var transform = CustomGameSettingsContainer.transform;
			CustomGameSettingsContainer.SetActive(true);
			GameObject CyclePrefab = transform.Find("SwitchPrefab").gameObject;
			GameObject TogglePrefab = transform.Find("TogglePrefab").gameObject;

			SgtLogger.Assert("CyclePrefab missing", CyclePrefab);
			SgtLogger.Assert("TogglePrefab missing", TogglePrefab);

			TogglePrefab.SetActive(false);
			CyclePrefab.SetActive(false);

			foreach (var qualitySetting in CustomGameSettings.Instance.QualitySettings)
			{
				string settingID = qualitySetting.Key;
				SgtLogger.l(settingID, "initializing QualitySetting UI Item");

				if (settingID == CustomGameSettingConfigs.WorldgenSeed.id || settingID == CustomGameSettingConfigs.ClusterLayout.id)
					continue;

				if (!DlcManager.IsAllContentSubscribed(qualitySetting.Value.required_content))
					continue;

				if (qualitySetting.Value is ToggleSettingConfig toggleSetting)
				{
					CustomGameSettingsToggleConfigs[settingID] = AddCheckboxGameSettingsContainer(TogglePrefab, CustomGameSettingsContainer, toggleSetting);
				}
				else if (qualitySetting.Value is ListSettingConfig listSetting)
				{
					CustomGameSettingsCycleConfigs[settingID] = AddListGameSettingsContainer(CyclePrefab, CustomGameSettingsContainer, listSetting);
				}
			}
		}

		#endregion

		public static CGM_MainScreen_UnityScreen Instance = null;

		////GridLayouter galleryGridLayouter;
		//GridLayoutSizeAdjustment galleryGridLayouter;

		private Dictionary<StarmapItemCategory, CategoryItem> categoryToggles = new Dictionary<StarmapItemCategory, CategoryItem>();
		private Dictionary<StarmapItem, GalleryItem> planetoidGridButtons = new Dictionary<StarmapItem, GalleryItem>();

		///Gallery
		GameObject PlanetoidEntryPrefab;
		private GameObject galleryGridContainer;

		GameObject StoryTraitEntryPrefab;
		private GameObject StoryTraitGridContainer;

		GameObject CustomGameSettingsContainer;

		///GalleryContents

		GameObject StoryTraitGridContent;
		GameObject ClusterItemsContent;
		FInputField2 AsteroidFilter;
		FButton ClearAsteroidFilter;
		string AsteroidFilterText = "";
		void SetFilterText(string text)
		{
			AsteroidFilterText = text;
			RefreshGallery();
		}

		///Categories
		GameObject PlanetoidCategoryPrefab;
		public GameObject categoryListContent;

		FToggle DLC2_Toggle;
		FToggle DLC3_Toggle;

        GameObject StoryTraitButton;

		private LocText galleryHeaderLabel;
		private LocText categoryHeaderLabel;
		private LocText selectionHeaderLabel;

		private bool init = false;


		private ISecondaryDisplayData _currentlySelectedItemData;
		ISecondaryDisplayData CurrentlySelectedItemData
		{
			get { return _currentlySelectedItemData; }
			set
			{
				_currentlySelectedItemData = value;
				RefreshView();
			}
		}

		public void SetSelectedItem(ISecondaryDisplayData newItem)
		{
			CurrentlySelectedItemData = newItem;
		}

		//private StarmapItem _selectedPlanet = null;// new StarmapItem("none", StarmapItemCategory.Starter,null);
		public StarmapItem CurrentStarmapItem
		{
			get
			{
				if (CurrentlySelectedItemData != null && CurrentlySelectedItemData is SelectedGalleryStarmapItem galleryItem)
				{
					return galleryItem.StarmapItem;
				}
				else if (CurrentlySelectedItemData != null && CurrentlySelectedItemData is SelectedGalleryPlanet_HexGrid hexgridItem)
				{
					return hexgridItem.StarmapItem;
				}
				return null;
			}
		}

		/// </GameSettings>
		StarmapItemCategory SelectedCategory = StarmapItemCategory.Starter;


		Dictionary<string, GameObject> SeasonTypes = new Dictionary<string, GameObject>();

		Dictionary<string, GameObject> MeteorTypes = new Dictionary<string, GameObject>();
		Dictionary<string, ToolTip> MeteorTypeTooltips = new Dictionary<string, ToolTip>();


		Dictionary<string, GameObject> PlanetBiomes = new Dictionary<string, GameObject>();

		Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();



		private GameObject Details_StoryTraitContainer;
		private Image StoryTraitImage;
		private LocText StoryTraitDesc;
		public FToggle StoryTraitToggle;


		private FSlider ClusterSize;

		public FInputField2 SeedInput_Main;
		public FButton SeedCycleButton_Main;
		public FToggle SeedRerollsTraitsToggle_Main;
		public FToggle SeedRerollsVanillaStarmapToggle;
		public FToggle SeedRerollsMixingsToggle;


		private LocText StarmapItemEnabledText;
		private FToggle StarmapItemEnabled;

		public FInputField2 NumberToGenerateInput;


		//private FSlider NumberToGenerate;

		private FSlider NumberOfRandomClassics;
		private GameObject NumberOfRandomClassicsGO;


		private UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider MinMaxDistanceSlider;
		private LocText SpawnDistanceText;
		private FSlider BufferDistance;

		private GameObject AsteroidSky;
		private FCycle AsteroidSky_Light;
		private FCycle AsteroidSky_Radiation;
		private FCycle AsteroidSky_NorthernLights;

		private GameObject AsteroidSize;
		private LocText AsteroidSizeLabel;
		private ToolTip AsteroidSizeTooltip;

		private FInputField2 PlanetSizeWidth;
		private FInputField2 PlanetSizeHeight;

		private FCycle PlanetSizeCycle;
		private FCycle PlanetRazioCycle;

		private GameObject MeteorSelector;
		private GameObject ActiveMeteorsContainer;
		private GameObject MeteorPrefab;
		private GameObject ActiveSeasonsContainer;
		private GameObject SeasonPrefab;
		private FButton AddSeasonButton;



		private GameObject AsteroidTraits;
		private GameObject ActiveTraitsContainer;
		private GameObject TraitPrefab;
		private FButton AddTraitButton;
		private FButton RandomTraitBlacklistOpener;
		private FButton RandomTraitDeleteButton;


		private GameObject PlanetBiomesGO;
		private GameObject ActiveBiomesContainer;
		private GameObject BiomePrefab;


		private FButton ResetButton;
		private FButton ResetAllButton;
		private FButton ReturnButton;
		private FButton PresetsButton;
		private FButton ResetStarmapButton;
		//private FButton SettingsButton;
		private FButton GenerateClusterButton;
		private ToolTip StartGameTooltip;

		private StarmapToolkit SpacedOutStarmap;
		private CategoryItem SpacedOutStarmap_CategoryToggle;

		//Geysers
		private GameObject GeyserContainer;
		private GameObject ActiveGeyserOverridesContainer;
		private GameObject GeyserOverridePrefab;
		private List<GameObject> ActiveGeyserOverrides = new();
		private FButton AddGeyserOverrideButton;

		private LocText CapacityText;

		private GameObject ActiveGeyserBlacklistContainer;
		private GameObject GeyserBlacklistPrefab;
		private Dictionary<string, GameObject> ActiveGeyserBlacklists = new();
		private FButton AddBlacklistedGeysers;
		private FToggle BlacklistAffectsNonGenerics;

		//BiomeMixings
		private GameObject BiomeMixingContainer;

		//WorldMixings
		private GameObject WorldMixingContainer;


		static bool isDLC2Active => CustomGameSettings.Instance.GetCurrentMixingSettingLevel(CustomMixingSettingsConfigs.DLC2Mixing).id == (CustomMixingSettingsConfigs.DLC2Mixing as ToggleSettingConfig).on_level.id;
		static bool isDLC3Active => CustomGameSettings.Instance.GetCurrentMixingSettingLevel(CustomMixingSettingsConfigs.DLC3Mixing).id == (CustomMixingSettingsConfigs.DLC3Mixing as ToggleSettingConfig).on_level.id;

        public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Instance = this;
			canBackoutWithRightClick = true;
			ConsumeMouseScroll = true;
			SetHasFocus(true);


			///Categories
			///
			SgtLogger.l("Hooking up Categories");
			PlanetoidCategoryPrefab = transform.Find("Categories/Content/Item").gameObject;
			categoryListContent = transform.Find("Categories/Content").gameObject;
			categoryHeaderLabel = transform.Find("Categories/Header/Label").GetComponent<LocText>();

			transform.Find("Categories/DLCFooter/Title/Label").gameObject.GetComponent<LocText>().SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.MIXING_DLC_HEADER);
			transform.Find("Categories/DLCFooter/DLC2/Label").gameObject.GetComponent<LocText>().SetText(global::STRINGS.UI.DLC2.NAME);
			transform.Find("Categories/DLCFooter/DLC3/Label").gameObject.GetComponent<LocText>().SetText(global::STRINGS.UI.DLC3.NAME);

			DLC2_Toggle = transform.Find("Categories/DLCFooter/DLC2/Checkbox").gameObject.AddComponent<FToggle>();
			DLC2_Toggle.SetCheckmark("Checkmark");

			DLC2_Toggle.SetOnFromCode(isDLC2Active);
			DLC2_Toggle.OnClick += (v) =>
			{
				ToggleWorldgenAffectingDlc(v, CustomMixingSettingsConfigs.DLC2Mixing);
			};

            DLC3_Toggle = transform.Find("Categories/DLCFooter/DLC3/Checkbox").gameObject.AddComponent<FToggle>();
            DLC3_Toggle.SetCheckmark("Checkmark");

            DLC3_Toggle.SetOnFromCode(isDLC3Active);
            DLC3_Toggle.OnClick += (v) =>
            {
                ToggleNonWorldGenDlc(v, CustomMixingSettingsConfigs.DLC3Mixing);
            };


            StoryTraitButton = transform.Find("Categories/FooterContent/StoryTraits").gameObject;
			GameSettingsButton = transform.Find("Categories/FooterContent/GameSettings").gameObject;
			MixingSettingsButton = transform.Find("Categories/FooterContent/MixingSettings").gameObject;

			///Gallery
			SgtLogger.l("Hooking up Gallery");
			StoryTraitGridContainer = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer").gameObject;
			StoryTraitEntryPrefab = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer/Item").gameObject;

			VanillaStarmapItemContainer = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer").gameObject;
			VanillaStarmapItemPrefab = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer/VanillaStarmapEntryPrefab").gameObject;
			MixingSettingsContainer = transform.Find("ItemSelection/MixingSettingsSettingsContent/MixingSettingsSettingsContainer").gameObject;
			CustomGameSettingsContainer = transform.Find("ItemSelection/CustomGameSettingsContent/CustomGameSettingsContainer").gameObject;

			galleryGridContainer = transform.Find("ItemSelection/StarItemContent/StarItemContainer").gameObject;
			PlanetoidEntryPrefab = transform.Find("ItemSelection/StarItemContent/StarItemContainer/Item").gameObject;
			galleryHeaderLabel = transform.Find("ItemSelection/Header/Label").GetComponent<LocText>();


			SpacedOutStarmap = transform.Find("ItemSelection").gameObject.AddOrGet<StarmapToolkit>();
			SpacedOutStarmap.SetActive(false, true);
			///GalleryContainers
			SgtLogger.l("Hooking up GalleryContainers");

			MixingSettingsContent = transform.Find("ItemSelection/MixingSettingsSettingsContent").gameObject;
			CustomGameSettingsContent = transform.Find("ItemSelection/CustomGameSettingsContent").gameObject;
			StarmapItemContent = transform.Find("ItemSelection/VanillaStarmapContent").gameObject;
			StoryTraitGridContent = transform.Find("ItemSelection/StoryTraitsContent").gameObject;
			ClusterItemsContent = transform.Find("ItemSelection/StarItemContent").gameObject;

			AsteroidFilter = transform.Find("ItemSelection/StarItemContent/Input").gameObject.AddOrGet<FInputField2>();

			AsteroidFilter.Text = string.Empty;

			AsteroidFilter.OnValueChanged.AddListener(SetFilterText);

			ClearAsteroidFilter = transform.Find("ItemSelection/StarItemContent/Input/DeleteButton").gameObject.AddOrGet<FButton>();
			ClearAsteroidFilter.OnClick += () => AsteroidFilter.Text = string.Empty;


			///Details
			///
			SgtLogger.l("Hooking up Details");
			selectionHeaderLabel = transform.Find("Details/Header/Label").GetComponent<LocText>();
			Init();

			if (DlcManager.IsExpansion1Active())
			{
				SpacedOutStarmap.OnGridChanged = UpdateStartButton;
			}
			OnResize();
		}
		public void DoAndRefreshView(System.Action action)
		{
			action.Invoke();
			RefreshCategories();
			RefreshGallery();
			RefreshDetails();
		}
		public override float GetSortKey() => 20f;

		public override void OnActivate() => OnShow(true);

		public static bool AllowedToClose()
		{
			return
					 (TraitSelectorScreen.Instance != null ? !TraitSelectorScreen.Instance.IsCurrentlyActive : true)
					&& (SeasonSelectorScreen.Instance != null ? !SeasonSelectorScreen.Instance.IsCurrentlyActive : true)
					&& (VanillaPOISelectorScreen.Instance != null ? !VanillaPOISelectorScreen.Instance.IsCurrentlyActive : true)
					;
		}

		bool overrideToWholeNumbers = false;
		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.Controller.GetKeyDown(KKeyCode.LeftShift))
			{
				overrideToWholeNumbers = !overrideToWholeNumbers;
				e.Consumed = true;
				RefreshDetails();
			}

			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				if (AllowedToClose())
					Show(show: false);
			}

			base.OnKeyDown(e);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
		}

		public bool IsCurrentlyActive = false;

		public void OnActivateWindow()
		{
			OnResize();

			if (!init)
			{
				Init();
				init = true;
			}

			ResetSOStarmap(false);
			RefreshView();
			DoWithDelay(() => SelectCategory(StarmapItemCategory.Starter), 25);
		}

		public override void OnShow(bool show)
		{
			SgtLogger.l("SHOWING: " + show);
			//this.isActive = show;
			IsCurrentlyActive = show;

			if (!show)
				ScreenResize.Instance.OnResize -= OnResize;
			else
				ScreenResize.Instance.OnResize += OnResize;

			base.OnShow(show);
			if (!show)
				return;

			RebuildVanillaStarmapUIIfPending();
		}
		static async Task DoWithDelay(System.Action task, int ms)
		{
			await Task.Delay(ms);
			task.Invoke();
		}
		public void OnResize()
		{
			var rectMain = this.rectTransform();
			rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, UnityEngine.Screen.width * (1f / rectMain.lossyScale.x));
			rectMain.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, UnityEngine.Screen.height * (1f / rectMain.lossyScale.y));
			//if(galleryGridLayouter!=null)    
			//    this.galleryGridLayouter.RequestGridResize();
		}

		public void RefreshView()
		{
			if (!init) return;

			RefreshCategories();
			RefreshGallery();
			RefreshDetails();
		}
		private void RefreshCategories()
		{
			foreach (var categoryToggle in categoryToggles)
			{
				Sprite PlanetSprite = null;
				switch (categoryToggle.Key)
				{
					case StarmapItemCategory.Starter:
						PlanetSprite = CustomCluster.StarterPlanet != null ? CustomCluster.StarterPlanet.planetSprite : Assets.GetSprite("unknown");
						break;
					case StarmapItemCategory.Warp:
						PlanetSprite = CustomCluster.WarpPlanet != null ? CustomCluster.WarpPlanet.planetSprite : Assets.GetSprite("unknown");
						break;
					case StarmapItemCategory.Outer:
						PlanetSprite = CustomCluster.OuterPlanets.Count > 0 ? CustomCluster.OuterPlanets.First().Value.planetSprite : Assets.GetSprite("unknown");
						break;
					case StarmapItemCategory.SpacedOutStarmap:
						// PlanetSprite = Assets.GetSprite(SpritePatch.starmapIcon);
						PlanetSprite = Assets.GetSprite("hex_unknown");
						break;
					case StarmapItemCategory.POI:
					case StarmapItemCategory.VanillaStarmap:
						PlanetSprite = Assets.GetSprite(Db.Get().SpaceDestinationTypes.Wormhole.spriteName);
						break;
				}
				categoryToggle.Value.Refresh(SelectedCategory, PlanetSprite);
			}
			DLC2_Toggle.SetOnFromCode(isDLC2Active);
			DLC3_Toggle.SetOnFromCode(isDLC3Active);
            DLC2_Toggle.SetInteractable(!CustomCluster.HasCeresAsteroid);
		}


		//string SpacedOutPOIHeaderString()
		//{
		//    if (CurrentlySelectedSOStarmapItem == null || !ModAssets.SO_POIs.ContainsKey(CurrentlySelectedSOStarmapItem.first))
		//        return string.Empty;
		//    var data = ModAssets.SO_POIs[CurrentlySelectedSOStarmapItem.first];

		//        //TODO!
		//        return
		//         ModAssets.Strings.ApplyCategoryTypeToString(
		//        CurrentlySelectedSOStarmapItem != null
		//        ? string.Format(VANILLAPOI_RESOURCES.SELECTEDDISTANCE_SO,
		//                data.Name,
		//                CurrentlySelectedSOStarmapItem.second.Substring(0,8))
		//        : string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL, VANILLAPOI_RESOURCES.NONESELECTED)
		//        , StarmapItemCategory.VanillaStarmap);
		//}

		//string VanillaStarmapHeaderString()
		//{
		//    return
		//         ModAssets.Strings.ApplyCategoryTypeToString(
		//        CurrentlySelectedVanillaStarmapItem != null
		//        ? string.Format(VANILLAPOI_RESOURCES.SELECTEDDISTANCE,
		//                Db.Get().SpaceDestinationTypes.TryGet(CurrentlySelectedVanillaStarmapItem.first).Name,
		//                (CurrentlySelectedVanillaStarmapItem.second + 1) * 10000,
		//                global::STRINGS.UI.UNITSUFFIXES.DISTANCE.KILOMETER.Replace(" ", ""))
		//        : string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL, VANILLAPOI_RESOURCES.NONESELECTED)
		//        , StarmapItemCategory.VanillaStarmap);
		//}

		string ArtifactRateTooltip(ArtifactDropRate rateItem)
		{
			string artifactRates = string.Empty;
			bool first = true;
			foreach (var artifactRate in rateItem.rates)
			{
				if (!first)
				{
					artifactRates += "\n";
				}
				first = false;
				var rate = artifactRate.first;
				artifactRates += "• ";
				artifactRates += Strings.Get(rate.name_key);
				artifactRates += ": ";
				artifactRates += (artifactRate.second / rateItem.totalWeight).ToString("P");

			}
			return artifactRates;
		}


		void UpdateVanillaStarmapDetails()
		{
			if (CurrentlySelectedItemData != null && Db.Get().SpaceDestinationTypes.TryGet(CurrentlySelectedItemData.ID) != null)
			{
				selectionHeaderLabel.SetText(CurrentlySelectedItemData.LocationDescription());
				SpaceDestinationType currentDestination = Db.Get().SpaceDestinationTypes.TryGet(CurrentlySelectedItemData.ID);
				VanillaPOI_POIDesc.text = currentDestination.description;
				for (int i = VanillaPOI_Resources.Count - 1; i >= 0; i--)
				{
					Destroy(VanillaPOI_Resources[i].gameObject);
				}
				if (currentDestination.elementTable != null)
				{
					foreach (var res in currentDestination.elementTable)
					{
						var entry = Util.KInstantiateUI(VanillaPOIResourcePrefab, VanillaPOIResourceContainer, true);
						Element element = ElementLoader.GetElement(res.Key.CreateTag());

						var UISprite = Def.GetUISprite(element);
						var image = entry.transform.Find("PreviewImage").gameObject.AddOrGet<Image>();
						image.sprite = UISprite.first;
						image.color = UISprite.second;
						entry.transform.Find("Label").gameObject.AddOrGet<LocText>().text = element.name;
						entry.transform.Find("BioLabel").gameObject.SetActive(false);
						entry.transform.Find("Amount").gameObject.AddOrGet<LocText>().text = "≈" + (1f / currentDestination.elementTable.Count).ToString("P");

						VanillaPOI_Resources.Add(entry);
					}
				}
				if (currentDestination.recoverableEntities != null)
				{
					foreach (var recoverableEntity in currentDestination.recoverableEntities)
					{
						var entry = Util.KInstantiateUI(VanillaPOIResourcePrefab, VanillaPOIResourceContainer, true);
						GameObject prefab = Assets.GetPrefab(recoverableEntity.Key);

						var UISprite = Def.GetUISprite(prefab);
						var image = entry.transform.Find("PreviewImage").gameObject.AddOrGet<Image>();
						image.sprite = UISprite.first;
						image.color = UISprite.second;

						entry.transform.Find("Label").gameObject.AddOrGet<LocText>().text = prefab.GetProperName();
						entry.transform.Find("BioLabel").gameObject.SetActive(true);
						entry.transform.Find("Amount").gameObject.AddOrGet<LocText>().text = "x" + recoverableEntity.Value.ToString();

						VanillaPOI_Resources.Add(entry);
					}
				}
				VanillaPOI_ReplenishmentAmountDesc.text = currentDestination.replishmentPerCycle.ToString("0.00") + global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;
				VanillaPOI_ArtifactDesc.text = ArtifactRateToString(currentDestination.artifactDropTable);
				VanillaPOI_SizeAmountDesc.text = (currentDestination.maxiumMass - currentDestination.minimumMass).ToString() + global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;
				VanillaPOI_RemovePOIBtn.gameObject.SetActive(CurrentlySelectedItemData.ID != "Wormhole");

				VanillaPOI_ArtifactTooltip.SetSimpleTooltip(ArtifactRateTooltip(currentDestination.artifactDropTable));
			}
			else
				selectionHeaderLabel.SetText(string.Empty);
		}

		void UpdateSO_SinglePOIDetails()
		{
			selectionHeaderLabel.SetText(string.Empty);
			if (CurrentlySelectedItemData == null)
				return;

			if (ModAssets.SO_POIs.ContainsKey(CurrentlySelectedItemData.ID))
			{
				selectionHeaderLabel.SetText(CurrentlySelectedItemData.LocationDescription());
				ModAssets.POI_Data data = ModAssets.SO_POIs[CurrentlySelectedItemData.ID];

				VanillaPOI_POIDesc.text = data.Description;
				for (int i = VanillaPOI_Resources.Count - 1; i >= 0; i--)
				{
					Destroy(VanillaPOI_Resources[i].gameObject);
				}

				string replenishmentString = ARTIFACTRATES.NONE;
				string mineableMassString = ARTIFACTRATES.NONE;
				string artifactsMinableString = ARTIFACTRATES.DLC_NO;

				if (data.Mineables != null)
				{

					float totalWeight = 0;
					foreach (var entry in data.Mineables.Values)
					{
						totalWeight += entry;
					}

					foreach (KeyValuePair<SimHashes, float> res in data.Mineables)
					{
						var entry = Util.KInstantiateUI(VanillaPOIResourcePrefab, VanillaPOIResourceContainer, true);
						Element element = ElementLoader.GetElement(res.Key.CreateTag());

						var UISprite = Def.GetUISprite(element);
						var image = entry.transform.Find("PreviewImage").gameObject.AddOrGet<Image>();
						image.sprite = UISprite.first;
						image.color = UISprite.second;
						entry.transform.Find("Label").gameObject.AddOrGet<LocText>().text = element.name;
						entry.transform.Find("BioLabel").gameObject.SetActive(false);
						entry.transform.Find("Amount").gameObject.AddOrGet<LocText>().text = (res.Value / totalWeight).ToString("P");

						VanillaPOI_Resources.Add(entry);
					}
					replenishmentString = (data.CapacityMin / (data.RechargeMax / 600f)).ToString("0.0") + global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM + " - " + (data.CapacityMax / (data.RechargeMin / 600f)).ToString("0.0") + global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;
					mineableMassString = (data.CapacityMin / 1000f).ToString("0.0") + global::STRINGS.UI.UNITSUFFIXES.MASS.TONNE + " - " + (data.CapacityMax / 1000f).ToString("0.0") + global::STRINGS.UI.UNITSUFFIXES.MASS.TONNE;

				}
				if (data.HasArtifacts)
				{
					artifactsMinableString = ARTIFACTRATES.DLC_YES;
				}
				VanillaPOI_ReplenishmentAmountDesc.text = replenishmentString;
				VanillaPOI_ArtifactDesc.text = artifactsMinableString;
				VanillaPOI_SizeAmountDesc.text = mineableMassString;
				VanillaPOI_RemovePOIBtn.gameObject.SetActive(CurrentlySelectedItemData is SelectedSinglePOI_SO);
				VanillaPOI_ArtifactTooltip.SetSimpleTooltip(string.Empty);
			}
		}

		void AddSO_GroupPOIEntry_UI(string groupId, string poiId)
		{
			if (!ModAssets.SO_POIs.ContainsKey(poiId))
			{
				SgtLogger.warning(poiId, "poi not found");
				return;
			}

			var data = ModAssets.SO_POIs[poiId];

			var entry = Util.KInstantiateUI(POIGroup_EntryPrefab, POIGroup_Container, true);

			var image = entry.transform.Find("ImageContainer/POISprite").gameObject.AddOrGet<Image>();
			image.sprite = data.Sprite;
			Rect rect = image.sprite.rect;
			if (rect.width > rect.height)
			{
				var size = rect.height / rect.width * 40;
				image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			}
			else
			{
				var size = rect.width / rect.height * 40;
				image.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}



			entry.transform.Find("Label").gameObject.AddOrGet<LocText>().text = data.Name;
			UIUtils.AddSimpleTooltipToObject(entry.gameObject, data.Description);

			entry.AddOrGet<FButton>().OnClick += () =>
			{
				CurrentlySelectedItemData = new SelectedSinglePOI_SO(poiId, groupId);
				//CGM_MainScreen_UnityScreen.Instance.CurrentlySelectedSOStarmapItem = new Tuple<string, string>(poiId, groupId);
			};

			entry.transform.Find("DeleteButton").gameObject.AddOrGet<FButton>().OnClick += () =>
			{
				RemoveSOSinglePOI(groupId, poiId);
			};

			POIGroup_Entries.Add(new(poiId, entry));
		}
		void UpdateSO_POIGroup()
		{
			selectionHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL, CurrentStarmapItem.DisplayName), SelectedCategory));
			POIGroup_AllowDuplicates.SetOnFromCode(CurrentStarmapItem.placementPOI.canSpawnDuplicates);
			POIGroup_AvoidClumping.SetOnFromCode(CurrentStarmapItem.placementPOI.avoidClumping);
			POIGroup_Guarantee.SetOnFromCode(CurrentStarmapItem.placementPOI.guarantee);
			var allItems = POIGroup_Entries;

			for (int i = allItems.Count - 1; i >= 0; i--)
			{
				Destroy(POIGroup_Entries[i].second);
			}
			POIGroup_Entries.Clear();

			foreach (string poiId in CurrentStarmapItem.placementPOI.pois)
			{
				string groupId = CurrentStarmapItem.id;
				AddSO_GroupPOIEntry_UI(groupId, poiId);
			}

		}

		bool PlanetSelected() => CurrentStarmapItem != null && IsPlanetCategory(CurrentStarmapItem.category);

		public void RefreshDetails()
		{
			///State bools
			bool planetCategorySelected = PlanetSelected();
			bool POIGroupSelected = CurrentStarmapItem != null && SelectedCategory == StarmapItemCategory.POI;
			bool SinglePOISelected = CurrentlySelectedItemData is SelectedSinglePOI_Vanilla || CurrentlySelectedItemData is SelectedSinglePOI_HexGrid || CurrentlySelectedItemData is SelectedSinglePOI_SO;
			bool DlcActive = DlcManager.IsExpansion1Active();
			bool HexGridSelection = CurrentlySelectedItemData is SelectedGalleryPlanet_HexGrid || CurrentlySelectedItemData is SelectedSinglePOI_HexGrid;

			///Reset Buttons
			ResetButton?.SetInteractable((planetCategorySelected || POIGroupSelected) && !PresetApplied);
			ResetButton?.gameObject.SetActive(SelectedCategory != StarmapItemCategory.VanillaStarmap && SelectedCategory != StarmapItemCategory.SpacedOutStarmap);
			ResetStarmapButton?.gameObject.SetActive(SelectedCategory == StarmapItemCategory.VanillaStarmap || SelectedCategory == StarmapItemCategory.SpacedOutStarmap);

			///Footer Settings
			SeedInput_Main.Text = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id;
			SeedRerollsTraitsToggle_Main.On = RerollTraitsWithSeedChange;
			SeedRerollsVanillaStarmapToggle.On = RerollStarmapWithSeedChange;
			SeedRerollsMixingsToggle.On = RerollMixingsWithSeedChange;

			ClusterSize?.transform.parent.gameObject.SetActive(DlcActive);
			ClusterSize?.SetMinMaxCurrent(ringMin, ringMax, CustomCluster.Rings);
			//ClusterSize.SetInteractable(SelectedCategory != StarmapItemCategory.SpacedOutStarmap);
			//SeedRerollsVanillaStarmapToggle?.gameObject.SetActive(!DlcActive);


			///PlanetToggles
			StarmapItemEnabled?.gameObject.SetActive(planetCategorySelected && !HexGridSelection);
			NumberOfRandomClassicsGO?.SetActive(false);

			NumberToGenerateInput?.transform.parent.gameObject.SetActive(false);
			MinMaxDistanceSlider?.transform.parent.gameObject.SetActive((planetCategorySelected || POIGroupSelected) && !HexGridSelection && DlcActive);
			MinMaxDistanceSlider?.SetLimits(0, CustomCluster.Rings);

			BufferDistance?.transform.parent.gameObject.SetActive(planetCategorySelected && !HexGridSelection);
			AsteroidSize?.SetActive(planetCategorySelected);
			AsteroidSky?.SetActive(planetCategorySelected);
			MeteorSelector?.SetActive(planetCategorySelected);
			AsteroidTraits?.SetActive(planetCategorySelected);
			PlanetBiomesGO?.SetActive(planetCategorySelected);
			ActiveBiomesContainer?.SetActive(planetCategorySelected);

			GeyserContainer?.SetActive(planetCategorySelected);
			///TODO: reenable when mixing is fixed
			//BiomeMixingContainer?.SetActive(planetCategorySelected);
			//WorldMixingContainer?.SetActive(planetCategorySelected && !HexGridSelection);

			///StoryTrait Details Container
			Details_StoryTraitContainer.SetActive(SelectedCategory == StarmapItemCategory.StoryTraits);


			///Single POI Details Container
			Details_VanillaPOIContainer.SetActive(SinglePOISelected);
			selectionHeaderLabel.SetText(string.Empty);

			///SpacedOut POI Group Containers
			POIGroup_AllowDuplicates.gameObject.SetActive(POIGroupSelected);
			POIGroup_AvoidClumping.gameObject.SetActive(POIGroupSelected);
			POIGroup_Guarantee.gameObject.SetActive(POIGroupSelected);
			POIGroup_POIs.gameObject.SetActive(POIGroupSelected);
			POIGroup_AddPoiToGroup.gameObject.SetActive(POIGroupSelected);
			POIGroup_DeletePoiGroup.gameObject.SetActive(POIGroupSelected);

			if (SelectedCategory == StarmapItemCategory.MixingSettings || SelectedCategory == StarmapItemCategory.GameSettings)
			{
				selectionHeaderLabel.SetText(string.Empty);
			}

			if (CurrentlySelectedItemData != null)
			{
				if (CurrentlySelectedItemData is SelectedSinglePOI_Vanilla)
					UpdateVanillaStarmapDetails();
				else if (CurrentlySelectedItemData is SelectedGalleryStarmapItem galleryItem && galleryItem.StarmapItem.category == StarmapItemCategory.POI)
					UpdateSO_POIGroup();
				else if (CurrentlySelectedItemData is SelectedSinglePOI_SO || CurrentlySelectedItemData is SelectedSinglePOI_HexGrid)
					UpdateSO_SinglePOIDetails();
			}
			if (CurrentStarmapItem != null)
			{
				if (!HexGridSelection)
					NumberToGenerateInput.SetTextFromData(CurrentStarmapItem.InstancesToSpawn.ToString("0.00"));

				bool IsPartOfCluster = CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current);

				bool canGenerateMultiple = CurrentStarmapItem.MoreThanOnePossible;

				NumberToGenerateInput.transform.parent.gameObject.SetActive((planetCategorySelected || POIGroupSelected) && canGenerateMultiple && !HexGridSelection); ///Amount, only on poi / random planets
				NumberToGenerateInput.SetInteractable(IsPartOfCluster);

				if (!HexGridSelection)
					MinMaxDistanceSlider.SetValues(current.minRing, current.maxRing, 0, CustomCluster.Rings, false);

				MinMaxDistanceSlider.SetInteractable(IsPartOfCluster);

				if (planetCategorySelected)
				{
					selectionHeaderLabel.SetText(CurrentlySelectedItemData.LocationDescription());

					StarmapItemEnabledText.SetText(ModAssets.Strings.ApplyCategoryTypeToString(STARMAPITEMENABLED.LABEL, SelectedCategory));
					StarmapItemEnabled.SetOn(IsPartOfCluster);

					PlanetSizeWidth.SetInteractable(IsPartOfCluster && planetCategorySelected);
					PlanetSizeHeight.SetInteractable(IsPartOfCluster && planetCategorySelected);
					//if (!HexGridSelection)
					//    MinMaxDistanceSlider.SetValues(current.minRing, current.maxRing, 0, CustomCluster.Rings, false);
					//SpawnDistanceText.SetText(string.Format(MINMAXDISTANCE.DESCRIPTOR.FORMAT, (int)current.minRing, (int)current.maxRing));

					if (!HexGridSelection)
					{
						BufferDistance.SetMinMaxCurrent(0, CustomCluster.Rings, CurrentStarmapItem.buffer);
					}
					BufferDistance.transform.parent.gameObject.SetActive(!current.IsPOI && DlcActive && !HexGridSelection);
					BufferDistance.SetInteractable(IsPartOfCluster);

					RandomTraitDeleteButton.SetInteractable(!current.IsRandom);

					AddTraitButton.SetInteractable(IsPartOfCluster && !current.IsRandom && !PlanetIsMiniBase(CurrentStarmapItem));
					AddSeasonButton.SetInteractable(IsPartOfCluster && !current.IsRandom);

					//disable those on random asteroid
					AsteroidSize.SetActive(!current.IsRandom);
					AsteroidSky.SetActive(!current.IsRandom);
					MeteorSelector.SetActive(!current.IsRandom);
					PlanetBiomesGO.SetActive(!current.IsRandom);
					GeyserContainer.SetActive(!current.IsRandom);

					//no radiation in base game
					AsteroidSky_Radiation.gameObject.SetActive(DlcActive);

					if (!current.IsRandom)
					{
						UpdateSizeLabels(current);
						PlanetSizeCycle.Value = current.CurrentSizePreset.ToString();
						PlanetRazioCycle.Value = current.CurrentRatioPreset.ToString();

						if (current.GetSunlightValue() != null)
							AsteroidSky_Light.Value = current.GetSunlightValue();

						if (DlcActive && current.GetRadiationValue() != null)
							AsteroidSky_Radiation.Value = current.GetRadiationValue();

						if (current.GetNorthernLightsValue() != null)
							AsteroidSky_NorthernLights.Value = current.GetNorthernLightsValue();
					}

					RefreshMeteorLists();
					RefreshTraitList();
					RefreshPlanetBiomes();
					RefreshGeyserOverrides();
				}

				if (RandomOuterPlanetsStarmapItem != null)
				{
					NumberOfRandomClassicsGO.SetActive(current.IsRandom && !current.IsPOI);
					if (!HexGridSelection)
						NumberOfRandomClassics.SetMinMaxCurrent(0, MaxAmountRandomPlanet, MaxClassicOuterPlanets);
					NumberOfRandomClassics.SetInteractable(IsPartOfCluster);
				}

			}

		}
		string ArtifactRateToString(ArtifactDropRate rate)
		{
			var artifactRates = Db.Get().ArtifactDropRates;
			if (rate == artifactRates.None)
			{
				return ARTIFACTRATES.NONE;
			}
			if (rate == artifactRates.Bad)
			{
				return ARTIFACTRATES.BAD;
			}
			if (rate == artifactRates.Mediocre)
			{
				return ARTIFACTRATES.MEDIOCRE;
			}
			if (rate == artifactRates.Good)
			{
				return ARTIFACTRATES.GOOD;
			}
			if (rate == artifactRates.Great)
			{
				return ARTIFACTRATES.GREAT;
			}
			if (rate == artifactRates.Amazing)
			{
				return ARTIFACTRATES.AMAZING;
			}
			if (rate == artifactRates.Perfect)
			{
				return ARTIFACTRATES.PERFECT;
			}

			return string.Empty;
		}

		public void RefreshCurrentItemData()
		{
			if (CurrentlySelectedItemData != null && CurrentlySelectedItemData is SelectedGalleryStarmapItem)
			{
				CurrentlySelectedItemData = new SelectedGalleryStarmapItem((CurrentlySelectedItemData as SelectedGalleryStarmapItem).StarmapItem);
			}
		}
		public void DeselectCurrentItem()
		{
			CurrentlySelectedItemData = null;
		}


		public void SelectItem(StarmapItem planet)
		{

			if (planet != CurrentStarmapItem)
			{
				overrideToWholeNumbers = false;
			}

			if (planet != null)
				CurrentlySelectedItemData = new SelectedGalleryStarmapItem(planet);
			else
				CurrentlySelectedItemData = null;
			RefreshView();
		}
		public void SelectHexItem(string id, Tuple<int, int> location)
		{
			if (SelectedCategory != StarmapItemCategory.SpacedOutStarmap)
				return;

			if (id == null)
			{
				CurrentlySelectedItemData = null;
				return;
			}

			if (CustomCluster.HasStarmapItem(id, out var item))
			{
				CurrentlySelectedItemData = new SelectedGalleryPlanet_HexGrid(item, location);
			}
			else if (ModAssets.SO_POIs.ContainsKey(id))
			{
				var data = ModAssets.SO_POIs[id];
				CurrentlySelectedItemData = new SelectedSinglePOI_HexGrid(data.Id, data.Name, location);
			}
			RefreshView();
		}

		bool IsPlanetCategory(StarmapItemCategory category) => AvailableStarmapItemCategories.Contains(category);

		bool ShowPlanetFromFilter(StarmapItem item)
		{
			if (AsteroidFilterText == string.Empty)
				return true;
			string nameDesc = (item.DisplayName + item.DisplayDescription).ToUpperInvariant();
			return nameDesc.Contains(AsteroidFilterText.ToUpperInvariant());
		}

		private void RefreshGallery()
		{
			MixingSettingsContent.SetActive( SelectedCategory == StarmapItemCategory.MixingSettings);
			CustomGameSettingsContent.SetActive(SelectedCategory == StarmapItemCategory.GameSettings);
			StarmapItemContent.SetActive(SelectedCategory == StarmapItemCategory.VanillaStarmap || SelectedCategory == StarmapItemCategory.POI);
			StoryTraitGridContent.SetActive(SelectedCategory == StarmapItemCategory.StoryTraits);
			ClusterItemsContent.SetActive(IsPlanetCategory(SelectedCategory));

			galleryHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(STRINGS.UI.CGM_MAINSCREENEXPORT.ITEMSELECTION.HEADER.LABEL, SelectedCategory));
			if (IsPlanetCategory(SelectedCategory))
			{
				var activePlanets = GetActivePlanetsStarmapitems();

				foreach (var galleryGridButton in planetoidGridButtons)
				{
					var logicComponent = galleryGridButton.Value;
					logicComponent.Refresh(galleryGridButton.Key, false, false, false);
					galleryGridButton.Value.gameObject.SetActive(galleryGridButton.Key.category == SelectedCategory && ShowPlanetFromFilter(galleryGridButton.Key));
				}
				foreach (var activePlanet in activePlanets)
				{
					bool selected = CurrentStarmapItem == null ? false : CurrentStarmapItem == activePlanet;
					bool mixed = activePlanet.IsMixed;

					planetoidGridButtons[activePlanet].Refresh(activePlanet, true, selected, mixed);
					planetoidGridButtons[activePlanet].gameObject.SetActive(activePlanet.category == SelectedCategory && ShowPlanetFromFilter(activePlanet));
				}
			}
			else if (SelectedCategory == StarmapItemCategory.GameSettings)
			{
				galleryHeaderLabel.SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.CUSTOMIZE);
				LoadGameSettings();
				//those two share a container
				SetGameSettingsVisible(true);
				SetMixingSettingsVisible(false);
			}
			else if (SelectedCategory == StarmapItemCategory.MixingSettings)
			{
				galleryHeaderLabel.SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.MIXING_SETTINGS_HEADER);
				LoadMixingSettings();
				//those two share a container
				SetGameSettingsVisible(false);
				SetMixingSettingsVisible(true);
			}
			else if (SelectedCategory == StarmapItemCategory.StoryTraits)
			{
				galleryHeaderLabel.SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.STORY_TRAITS_HEADER);
			}
			else if (SelectedCategory == StarmapItemCategory.VanillaStarmap || SelectedCategory == StarmapItemCategory.SpacedOutStarmap)
			{
				galleryHeaderLabel.SetText(global::STRINGS.UI.CLUSTERMAP.TITLE);
			}
		}

		public void SelectCategory(StarmapItemCategory category)
		{

			if (DlcManager.IsExpansion1Active())
				SpacedOutStarmap.SetActive(category == StarmapItemCategory.SpacedOutStarmap);
			SelectedCategory = category;
			//this.categoryHeaderLabel.SetText(STRINGS.UI.CUSTOMCLUSTERUI.NAMECATEGORIES);
			SelectDefaultCategoryItem();
			RefreshView();
		}
		private void SelectDefaultCategoryItem()
		{
			if (SelectedCategory == StarmapItemCategory.StoryTraits)
			{
				SelectStoryTrait(Db.Get().Stories.resources.First().Id);
				return;
			}

			foreach (var galleryGridButton in planetoidGridButtons)
			{
				if (galleryGridButton.Key.category == SelectedCategory && CustomCluster.HasStarmapItem(galleryGridButton.Key.id, out _))
				{
					SelectItem(galleryGridButton.Key);
					return;
				}
			}
			SelectItem(null);
		}



		#region StarmapPOIs


		GameObject VanillaStarmapItemContainer;
		private GameObject VanillaStarmapItemPrefab;
		private GameObject SO_StarmapItemPrefab;

		GameObject StarmapItemContent;
		Dictionary<int, VanillaStarmapRangeBand> VanillaStarmapEntries = new Dictionary<int, VanillaStarmapRangeBand>();
		Dictionary<string, SO_StarmapRangeBand> SOStarmapEntries = new Dictionary<string, SO_StarmapRangeBand>();
		Transform AddRemoveStarmapButtons;
		GameObject AddAllMissingStarmapItemsButton;
		ToolTip MissingPlanetsTooltip;



		public FToggle POIGroup_AllowDuplicates;
		public FToggle POIGroup_Guarantee;
		public FToggle POIGroup_AvoidClumping;
		public GameObject POIGroup_POIs;
		public GameObject POIGroup_Container;
		public GameObject POIGroup_EntryPrefab;
		public List<Tuple<string, GameObject>> POIGroup_Entries = new List<Tuple<string, GameObject>>();
		public FButton POIGroup_AddPoiToGroup;
		public FButton POIGroup_DeletePoiGroup;


		private GameObject Details_VanillaPOIContainer;
		private GameObject VanillaPOIResourceContainer;
		private GameObject VanillaPOIResourcePrefab;
		private LocText VanillaPOI_SizeAmountDesc;
		private LocText VanillaPOI_POIDesc;
		private LocText VanillaPOI_ReplenishmentAmountDesc;
		private LocText VanillaPOI_ArtifactDesc;
		private ToolTip VanillaPOI_ArtifactTooltip;
		private FButton VanillaPOI_RemovePOIBtn;
		List<GameObject> VanillaPOI_Resources = new List<GameObject>();

		void MissingStarmapItemsAdding()
		{
			if (DlcManager.IsExpansion1Active())
			{
				///TODO: Open POISelector
				// CustomCluster.AddPoiGroup();
				VanillaPOISelectorScreen.InitializeView(null, (id) =>
				{
					CustomCluster.AddNewPoiGroupFromPOI(id);
					RebuildStarmap(true);
				});
				RefreshDetails();
				RefreshGallery();
			}
			else
			{
				CustomCluster.AddMissingStarmapItems();
				RebuildStarmap(false);
			}

			RefreshMissingItemsButton();
		}

		public void InitializeStarmap()
		{
			VanillaStarmapItemContainer = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer").gameObject;
			SO_StarmapItemPrefab = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer/SO_GroupPrefab").gameObject;
			SO_StarmapItemPrefab.SetActive(false);
			VanillaStarmapItemPrefab = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer/VanillaStarmapEntryPrefab").gameObject;
			VanillaStarmapItemPrefab.SetActive(false);


			AddRemoveStarmapButtons = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer/AddNewDistanceButtonContainer");
			AddAllMissingStarmapItemsButton = transform.Find("ItemSelection/VanillaStarmapContent/VanillaStarmapContainer/AddMissingPOI").gameObject;
			AddAllMissingStarmapItemsButton.AddOrGet<FButton>().OnClick += () =>
			{
				MissingStarmapItemsAdding();
			};
			MissingPlanetsTooltip = UIUtils.AddSimpleTooltipToObject(AddAllMissingStarmapItemsButton, "");

			var AddButton = AddRemoveStarmapButtons.Find("AddDistanceRow");
			AddButton.gameObject.AddOrGet<FButton>().OnClick += () =>
			{
				int newDistance = CustomCluster.AddVanillaStarmapDistance();
				AddVanillaStarmapItemBand(newDistance, new List<string>());

				RefreshTearIndex();
				AddRemoveStarmapButtons.SetAsLastSibling();
			};
			var RemoveButton = AddRemoveStarmapButtons.Find("RemoveDistanceRow").gameObject.AddOrGet<FButton>();
			RemoveButton.OnClick += () =>
			{
				CustomCluster.RemoveFurthestVanillaStarmapDistance();
				RemoveFurthestVanillaStarmapItemBand();
			};

			RebuildStarmap(true);
			RefreshTearIndex();
		}

		public void RefreshTearIndex()
		{
			foreach (KeyValuePair<int, VanillaStarmapRangeBand> item in VanillaStarmapEntries)
			{
				item.Value.IsLatestEntry = item.Key == CustomCluster.MaxStarmapDistance;
			}
		}
		GameObject AddVanillaStarmapItemBand(int band, List<string> items)
		{
			var entry = Util.KInstantiateUI(VanillaStarmapItemPrefab, VanillaStarmapItemContainer, true);
			entry.SetActive(true);
			var logic = entry.AddOrGet<VanillaStarmapRangeBand>();
			logic.Init(band);

			foreach (var item in items)
			{
				if (item != "Wormhole")
					logic.AddPoi(item);
			}
			VanillaStarmapEntries[band] = logic;
			return entry;
		}
		GameObject AddSO_POIGroup(StarmapItem item)
		{
			var entry = Util.KInstantiateUI(SO_StarmapItemPrefab, VanillaStarmapItemContainer, true);
			entry.SetActive(true);
			var logic = entry.AddOrGet<SO_StarmapRangeBand>();
			logic.Init(item);

			foreach (var poi in item.placementPOI.pois)
			{
				logic.AddPoiUI(poi);
			}
			SOStarmapEntries[item.id] = logic;
			return entry;
		}
		void RefreshPOIGroupHeader(string id)
		{
			if (SOStarmapEntries.ContainsKey(id))
				SOStarmapEntries[id].RefreshHeader();
		}


		void RemoveFurthestVanillaStarmapItemBand()
		{
			int last = VanillaStarmapEntries.Count - 1;
			Destroy(VanillaStarmapEntries[last].gameObject);
			VanillaStarmapEntries.Remove(last);

			RefreshTearIndex();
		}

		public void RebuildSoStarmap()
		{
			SgtLogger.l("rebuilding poi groups");
			bool currentlyActive = StarmapItemContent.activeSelf;
			StarmapItemContent.SetActive(true);

			var list = SOStarmapEntries.Values.ToList();
			for (int i = SOStarmapEntries.Count - 1; i >= 0; i--)
			{
				Destroy(list[i].gameObject);
			}
			SOStarmapEntries.Clear();

			foreach (var rangeEntry in CustomCluster.POIs)
			{
				AddSO_POIGroup(rangeEntry.Value);
			}
			AddRemoveStarmapButtons.gameObject.SetActive(false);

			RefreshMissingItemsButton();
			StarmapItemContent.SetActive(currentlyActive);
		}

		public void ResetSOStarmap(bool reset)
		{
			if (DlcManager.IsExpansion1Active())
			{
				if (reset)
					CustomCluster.SO_Starmap = null;
				RebuildSoStarmap();
				SpacedOutStarmap.RebuildGrid();
				UpdateStartButton();
			}
		}
		public bool GenerationPossible = true;
		void UpdateStartButton()
		{
			if (DlcManager.IsExpansion1Active() && SpacedOutStarmap != null)
			{
				bool planetEncased = CustomCluster.SO_Starmap.EncasedPlanet(out string encasedId);

				if (!CustomCluster.SO_Starmap.GenerationPossible & PlanetoidDict.TryGetValue(CustomCluster.SO_Starmap.FailedGenerationPlanetId, out var failLocation))
					StartGameTooltip.SetSimpleTooltip(string.Format(GENERATECLUSTERBUTTON.TOOLTIP_CLUSTERPLACEMENTFAILED_ASTEROID, failLocation.DisplayName));

				else if (planetEncased && PlanetoidDict.TryGetValue(encasedId, out var plt))
					StartGameTooltip.SetSimpleTooltip(string.Format(GENERATECLUSTERBUTTON.TOOLTIP_CLUSTERPLACEMENTFAILED_COMETS, plt.DisplayName));

				else if (!SpacedOutStarmap.TearOnMap)
					StartGameTooltip.SetSimpleTooltip(GENERATECLUSTERBUTTON.TOOLTIP_CLUSTERPLACEMENTFAILED_TEAR);

				else
					StartGameTooltip.SetSimpleTooltip(GENERATECLUSTERBUTTON.TOOLTIP);

				GenerationPossible = CustomCluster.SO_Starmap.GenerationPossible && SpacedOutStarmap.TearOnMap && !planetEncased;
				GenerateClusterButton.SetInteractable(GenerationPossible);

			}
		}

		public void RebuildStarmap(bool reset)
		{
			if (DlcManager.IsExpansion1Active())
			{
				ResetSOStarmap(reset);
			}
			else
				RebuildVanillaStarmap(reset);
		}


		bool pendingRebuild = false;
		public void TryRebuildStarmapUI()
		{
			if (!IsCurrentlyActive)
				pendingRebuild = true;
			else
				RebuildVanillaStarmapUI();
		}
		public void RebuildVanillaStarmapUIIfPending()
		{
			if (pendingRebuild)
			{
				pendingRebuild = false;
				RebuildVanillaStarmapUI();
			}
		}
		private void RebuildVanillaStarmapUI()
		{
			bool currentlyActive = StarmapItemContent.activeSelf;
			StarmapItemContent.SetActive(true);
			var list = VanillaStarmapEntries.Values.ToList();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Destroy(list[i].gameObject);
			}
			VanillaStarmapEntries.Clear();

			foreach (var rangeEntry in CustomCluster.VanillaStarmapItems.OrderBy(entry => entry.Key))
			{
				AddVanillaStarmapItemBand(rangeEntry.Key, rangeEntry.Value);
			}
			AddRemoveStarmapButtons.gameObject.SetActive(true);
			AddRemoveStarmapButtons.SetAsLastSibling();

			RefreshTearIndex();
			RefreshMissingItemsButton();
			StarmapItemContent.SetActive(currentlyActive);
			if (SelectedCategory == StarmapItemCategory.VanillaStarmap)
				CurrentlySelectedItemData = null;
		}

		public void RebuildVanillaStarmap(bool reset)
		{
			if (DlcManager.IsExpansion1Active())
				return;

			if (reset)
				CustomCluster.ResetVanillaStarmap();
			TryRebuildStarmapUI();
		}
		void RefreshMissingItemsButton()
		{
			if (DlcManager.IsExpansion1Active())
			{
				AddAllMissingStarmapItemsButton.SetActive(true);
			}
			else
			{
				AddAllMissingStarmapItemsButton.SetActive(CustomCluster.SomeStarmapitemsMissing(out var items));
				UIUtils.TryChangeText(AddAllMissingStarmapItemsButton.transform, "Label", string.Format(ADDMISSINGPOI.LABEL_BASEGAME, items.Count));
				string tooltipList = string.Empty;
				var db = Db.Get().SpaceDestinationTypes;
				foreach (var entry in items)
				{
					var type = db.TryGet(entry);
					tooltipList += "\n • ";
					tooltipList += type.Name;
				}
				MissingPlanetsTooltip.SetSimpleTooltip(string.Format(ADDMISSINGPOI.TOOLTIP, tooltipList));

			}
		}


		#endregion

		#region initialisation

		public void Init()
		{
			if (init) return;

			PopulateGalleryAndCategories();
			InitializeItemSettings();
			InitializeGameSettings();
			InitializeMixingSettings();
			InitializeStoryTraits();
			InitializeStarmapInfo();
			InitializeStarmap();
			init = true;
		}

		public void InitializeItemSettings()
		{
			MinMaxDistanceSlider = transform.Find("Details/Content/ScrollRectContainer/MinMaxDistance/Slider").FindOrAddComponent<UtilLibs.UI.FUI.Unity_UI_Extensions.Scripts.Controls.Sliders.MinMaxSlider>();
			SpawnDistanceText = MinMaxDistanceSlider.transform.parent.Find("Descriptor/Output").GetComponent<LocText>();

			MinMaxDistanceSlider.SliderBounds = MinMaxDistanceSlider.transform.Find("Handle Slide Area").rectTransform();
			MinMaxDistanceSlider.MinHandle = MinMaxDistanceSlider.transform.Find("Handle Slide Area/HandleMin").rectTransform();
			MinMaxDistanceSlider.MaxHandle = MinMaxDistanceSlider.transform.Find("Handle Slide Area/Handle").rectTransform();
			MinMaxDistanceSlider.MiddleGraphic = MinMaxDistanceSlider.transform.Find("Fill Area/Fill").rectTransform();
			MinMaxDistanceSlider.wholeNumbers = true;
			MinMaxDistanceSlider.MinMaxText = SpawnDistanceText;
			MinMaxDistanceSlider.MinMaxTextFormat = MINMAXDISTANCE.DESCRIPTOR.FORMAT;

			MinMaxDistanceSlider.onValueChanged.AddListener(
				(min, max) =>
				{
					if (CurrentStarmapItem != null && CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
					{
						int minInt = Mathf.RoundToInt(min);
						int maxInt = Mathf.RoundToInt(max);
						if (minInt == current.minRing && maxInt == current.maxRing)
							return;

						current.SetInnerRing(minInt);
						current.SetOuterRing(maxInt);
						SpawnDistanceText.SetText(string.Format(MINMAXDISTANCE.DESCRIPTOR.FORMAT, (int)min, (int)max));
						if (CurrentStarmapItem.IsPOI)
							RefreshPOIGroupHeader(CurrentStarmapItem.id);
						if (DlcManager.IsExpansion1Active()) ResetSOStarmap(true);
					}
				}
				);

			//MinMaxDistanceSlider.SetLimits(0, CustomCluster.Rings);
			//MinMaxDistanceSlider.SetValues(0, 0.001f, 0, CustomCluster.Rings, true);

			UIUtils.AddSimpleTooltipToObject(MinMaxDistanceSlider.transform.parent.Find("Descriptor"), MINMAXDISTANCE.DESCRIPTOR.TOOLTIP, onBottom: true, alignCenter: true);

			StarmapItemEnabledText = transform.Find("Details/Content/ScrollRectContainer/StarmapItemEnabled/Label").GetComponent<LocText>();
			StarmapItemEnabled = transform.Find("Details/Content/ScrollRectContainer/StarmapItemEnabled").FindOrAddComponent<FToggle>();
			StarmapItemEnabled.SetCheckmark("Background/Checkmark");
			StarmapItemEnabled.OnClick += (v) =>
			{
				if (CurrentStarmapItem != null)
				{
					TogglePlanetoid(CurrentStarmapItem);
					RefreshCategories();
					RefreshGallery();
					RefreshDetails();

					if (DlcManager.IsExpansion1Active()) ResetSOStarmap(true);
				}
			};
			UIUtils.AddSimpleTooltipToObject(StarmapItemEnabled.transform, STARMAPITEMENABLED.TOOLTIP, onBottom: true, alignCenter: true);

			NumberToGenerateInput = transform.Find("Details/Content/ScrollRectContainer/NumbersSpawnedInput/Input").FindOrAddComponent<FInputField2>();
			NumberToGenerateInput.inputField.onEndEdit.AddListener(TryChangingNumber);
			var label = NumberToGenerateInput.transform.parent.Find("Label");
			label.GetComponent<LocText>().SetText(AMOUNTSLIDER.DESCRIPTOR.LABEL);
			UIUtils.AddSimpleTooltipToObject(label, AMOUNTSLIDER.DESCRIPTOR.TOOLTIP, onBottom: true, alignCenter: true);
			//NumberToGenerate = transform.Find("Details/Content/ScrollRectContainer/AmountSlider/Slider").FindOrAddComponent<FSlider>();

			//NumberToGenerate.SetWholeNumbers(true);
			//NumberToGenerate.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/AmountSlider/Descriptor/Output").GetComponent<LocText>());
			//NumberToGenerate.OnChange += (value) =>
			//{
			//    if (SelectedPlanet != null)
			//    {
			//        if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var current))
			//            current.SetSpawnNumber(value);
			//        this.RefreshGallery();

			//        if (SelectedPlanet == CGSMClusterManager.RandomOuterPlanetsStarmapItem)
			//        {
			//            MaxClassicOuterPlanets = Mathf.Min(MaxClassicOuterPlanets, Mathf.RoundToInt(SelectedPlanet.InstancesToSpawn) + 2);
			//        }

			//        this.RefreshDetails();
			//    }
			//};
			//UIUtils.AddSimpleTooltipToObject(NumberToGenerate.transform.parent.Find("Descriptor"), (AMOUNTSLIDER.DESCRIPTOR.TOOLTIP), onBottom: true, alignCenter: true);
			NumberOfRandomClassicsGO = transform.Find("Details/Content/ScrollRectContainer/AmountOfClassicPlanets").gameObject;

			NumberOfRandomClassicsGO.SetActive(true);
			NumberOfRandomClassics = NumberOfRandomClassicsGO.transform.Find("Slider").FindOrAddComponent<FSlider>();
			NumberOfRandomClassicsGO.SetActive(false);

			NumberOfRandomClassics.SetWholeNumbers(true);
			NumberOfRandomClassics.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/AmountOfClassicPlanets/Descriptor/Output").GetComponent<LocText>());
			NumberOfRandomClassics.OnChange += (value) =>
			{
				int newValue = Mathf.RoundToInt(value);
				if (newValue == MaxClassicOuterPlanets)
					return;

				MaxClassicOuterPlanets = newValue;
				if (DlcManager.IsExpansion1Active()) ResetSOStarmap(true);
			};
			if (RandomOuterPlanetsStarmapItem != null)
				NumberOfRandomClassics.SetMinMaxCurrent(0, RandomOuterPlanetsStarmapItem.InstancesToSpawn, RandomOuterPlanetsStarmapItem.InstancesToSpawn);

			UIUtils.AddSimpleTooltipToObject(NumberOfRandomClassics.transform.parent.Find("Descriptor"), AMOUNTOFCLASSICPLANETS.DESCRIPTOR.TOOLTIP, onBottom: true, alignCenter: true);

			BufferDistance = transform.Find("Details/Content/ScrollRectContainer/BufferSlider/Slider").FindOrAddComponent<FSlider>();
			BufferDistance.SetWholeNumbers(true);
			BufferDistance.AttachOutputField(transform.Find("Details/Content/ScrollRectContainer/BufferSlider/Descriptor/Output").GetComponent<LocText>());
			BufferDistance.OnChange += (value) =>
			{
				if (CurrentStarmapItem != null)
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
					{
						int newBuffer = (int)value;
						if (newBuffer == current.buffer)
							return;

						current.SetBuffer(newBuffer);
						if (DlcManager.IsExpansion1Active()) ResetSOStarmap(true);
					}
				}
			};
			UIUtils.AddSimpleTooltipToObject(BufferDistance.transform.parent.Find("Descriptor"), BUFFERSLIDER.DESCRIPTOR.TOOLTIP);

			ClusterSize = transform.Find("Details/Footer/ClusterSizeSlider/Slider").FindOrAddComponent<FSlider>();
			ClusterSize.SetWholeNumbers(true);
			ClusterSize.AttachOutputField(transform.Find("Details/Footer/ClusterSizeSlider/Descriptor/Output").GetComponent<LocText>());
			ClusterSize.OnChange += (value) =>
			{
				int newRings = (int)value;
				if (newRings == CustomCluster.Rings)
					return;

				CustomCluster.SetRings(newRings);
				RefreshGallery();
				RefreshDetails();
				if (DlcManager.IsExpansion1Active())
					ResetSOStarmap(true);
			};
			UIUtils.AddSimpleTooltipToObject(ClusterSize.transform.parent.Find("Descriptor"), CLUSTERSIZESLIDER.DESCRIPTOR.TOOLTIP);
			///asteroid sky


			AsteroidSky = transform.Find("Details/Content/ScrollRectContainer/AsteroidSky").gameObject;

			AsteroidSky_Light = AsteroidSky.transform.Find("Content/SunlightCycle").gameObject.AddOrGet<FCycle>();
			AsteroidSky_Light.Initialize(
				AsteroidSky_Light.transform.Find("Left").gameObject.AddOrGet<FButton>(),
				AsteroidSky_Light.transform.Find("Right").gameObject.AddOrGet<FButton>(),
				AsteroidSky_Light.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>());

			var lightOptions = new List<FCycle.Option>();
			foreach (var lightSetting in ModAssets.SunlightFixedTraits)
			{
				lightOptions.Add(new(lightSetting.Key, lightSetting.Value.ToString()));
			}
			AsteroidSky_Light.Options = lightOptions;
			AsteroidSky_Light.OnChange += () =>
			{
				if (CurrentStarmapItem != null)
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
					{
						current.SetSunlightValue(AsteroidSky_Light.Value);
					}
				}
			};

			AsteroidSky_Radiation = AsteroidSky.transform.Find("Content/RadiationCycle").gameObject.AddOrGet<FCycle>();
			AsteroidSky_Radiation.Initialize(
				AsteroidSky_Radiation.transform.Find("Left").gameObject.AddOrGet<FButton>(),
				AsteroidSky_Radiation.transform.Find("Right").gameObject.AddOrGet<FButton>(),
				AsteroidSky_Radiation.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>());

			var radOptions = new List<FCycle.Option>();
			foreach (var radSetting in ModAssets.CosmicRadiationFixedTraits)
			{
				radOptions.Add(new(radSetting.Key, radSetting.Value.ToString()));
			}
			AsteroidSky_Radiation.Options = radOptions;
			AsteroidSky_Radiation.OnChange += () =>
			{
				if (CurrentStarmapItem != null && DlcManager.IsExpansion1Active())
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
					{
						current.SetRadiationValue(AsteroidSky_Radiation.Value);
					}
				}
			};

			AsteroidSky_NorthernLights = AsteroidSky.transform.Find("Content/NorthernLightsCycle").gameObject.AddOrGet<FCycle>();
			AsteroidSky_NorthernLights.Initialize(
				AsteroidSky_NorthernLights.transform.Find("Left").gameObject.AddOrGet<FButton>(),
				AsteroidSky_NorthernLights.transform.Find("Right").gameObject.AddOrGet<FButton>(),
				AsteroidSky_NorthernLights.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>());

			var northernLightOptions = new List<FCycle.Option>();
			foreach (var setting in ModAssets.NorthernLightsFixedTraits)
			{
				northernLightOptions.Add(new(setting.Key, setting.Value.ToString()));
			}
			AsteroidSky_NorthernLights.Options = northernLightOptions;
			AsteroidSky_NorthernLights.OnChange += () =>
			{
				if (CurrentStarmapItem != null)
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
					{
						current.SetNorthernLightsValue(AsteroidSky_NorthernLights.Value);
					}
				}
			};
			AsteroidSky_NorthernLights.IsInteractable = DlcManager.IsContentSubscribed(DlcManager.DLC2_ID);

            ///asteroid size 
            AsteroidSize = transform.Find("Details/Content/ScrollRectContainer/AsteroidSize").gameObject;
			AsteroidSizeLabel = AsteroidSize.transform.Find("Descriptor/Label").GetComponent<LocText>();
			AsteroidSizeTooltip = UIUtils.AddSimpleTooltipToObject(AsteroidSizeLabel.transform.parent, ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);

			PlanetSizeWidth = AsteroidSize.transform.Find("Content/Info/WidthLabel/Input").FindOrAddComponent<FInputField2>();
			PlanetSizeWidth.inputField.onEndEdit.AddListener((sizestring) => TryApplyingCoordinates(sizestring, false));

			PlanetSizeHeight = AsteroidSize.transform.Find("Content/Info/HeightLabel/Input").FindOrAddComponent<FInputField2>();
			PlanetSizeHeight.inputField.onEndEdit.AddListener((sizestring) => TryApplyingCoordinates(sizestring, true));

			PlanetSizeCycle = AsteroidSize.transform.Find("Content/Cycles/SizeCycle").gameObject.AddOrGet<FCycle>();
			PlanetSizeCycle.Initialize(
				PlanetSizeCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
				PlanetSizeCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
				PlanetSizeCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
				PlanetSizeCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

			PlanetSizeCycle.Options = new List<FCycle.Option>()
			{
				new FCycle.Option(WorldSizePresets.Tiny.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE0TOOLTIP),
				new FCycle.Option(WorldSizePresets.Smaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE1TOOLTIP),
				new FCycle.Option(WorldSizePresets.Small.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE2TOOLTIP),
				new FCycle.Option(WorldSizePresets.SlightlySmaller.ToString(), ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3, ASTEROIDSIZE.SIZESELECTOR.NEGSIZE3TOOLTIP),

				new FCycle.Option(WorldSizePresets.Normal.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE0, ASTEROIDSIZE.SIZESELECTOR.SIZE0TOOLTIP),
				new FCycle.Option(WorldSizePresets.SlightlyLarger.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE1, ASTEROIDSIZE.SIZESELECTOR.SIZE1TOOLTIP),
				new FCycle.Option(WorldSizePresets.Large.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE2, ASTEROIDSIZE.SIZESELECTOR.SIZE2TOOLTIP),
				new FCycle.Option(WorldSizePresets.Huge.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE3, ASTEROIDSIZE.SIZESELECTOR.SIZE3TOOLTIP),
				new FCycle.Option(WorldSizePresets.Massive.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE4, ASTEROIDSIZE.SIZESELECTOR.SIZE4TOOLTIP),
				new FCycle.Option(WorldSizePresets.Enormous.ToString(), ASTEROIDSIZE.SIZESELECTOR.SIZE5, ASTEROIDSIZE.SIZESELECTOR.SIZE5TOOLTIP),
			};

			PlanetSizeCycle.OnChange += () =>
			{
				if (CurrentStarmapItem != null)
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
					{
						WorldSizePresets setTo = Enum.TryParse<WorldSizePresets>(PlanetSizeCycle.Value, out var result) ? result : WorldSizePresets.Normal;
						current.SetPlanetSizeToPreset(setTo);
						UpdateSizeLabels(current);
					}
				}
			};

			PlanetRazioCycle = AsteroidSize.transform.Find("Content/Cycles/RazioCycle").gameObject.AddOrGet<FCycle>();
			PlanetRazioCycle.Initialize(
				PlanetRazioCycle.transform.Find("Left").gameObject.AddOrGet<FButton>(),
				PlanetRazioCycle.transform.Find("Right").gameObject.AddOrGet<FButton>(),
				PlanetRazioCycle.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
				PlanetRazioCycle.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

			PlanetRazioCycle.Options = new List<FCycle.Option>()
			{
				new FCycle.Option(WorldRatioPresets.LotWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE3, ASTEROIDSIZE.RATIOSELECTOR.WIDE3TOOLTIP),
				new FCycle.Option(WorldRatioPresets.Wider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE2, ASTEROIDSIZE.RATIOSELECTOR.WIDE2TOOLTIP),
				new FCycle.Option(WorldRatioPresets.SlightlyWider.ToString(), ASTEROIDSIZE.RATIOSELECTOR.WIDE1, ASTEROIDSIZE.RATIOSELECTOR.WIDE1TOOLTIP),
				new FCycle.Option(WorldRatioPresets.Normal.ToString(), ASTEROIDSIZE.RATIOSELECTOR.NORMAL, ASTEROIDSIZE.RATIOSELECTOR.NORMALTOOLTIP),
				new FCycle.Option(WorldRatioPresets.SlightlyTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT1TOOLTIP),
				new FCycle.Option(WorldRatioPresets.Taller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT2TOOLTIP),
				new FCycle.Option(WorldRatioPresets.LotTaller.ToString(), ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3, ASTEROIDSIZE.RATIOSELECTOR.HEIGHT3TOOLTIP),
			};
			PlanetRazioCycle.Value = WorldRatioPresets.Normal.ToString();

			PlanetRazioCycle.OnChange += () =>
			{
				if (CurrentStarmapItem != null)
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
					{
						WorldRatioPresets setTo = Enum.TryParse<WorldRatioPresets>(PlanetRazioCycle.Value, out var result) ? result : WorldRatioPresets.Normal;
						current.SetPlanetRatioToPreset(setTo);
						UpdateSizeLabels(current);
						//AsteroidSizeLabel.text = string.Format(ASTEROIDSIZEINFO.INFO, current.CustomPlanetDimensions.x, current.CustomPlanetDimensions.y);
					}
				}
			};

			MeteorSelector = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle").gameObject;
			ActiveMeteorsContainer = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Asteroids/ScrollArea/Content").gameObject;
			MeteorPrefab = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Asteroids/ScrollArea/Content/ListViewEntryPrefab").gameObject;

			ActiveSeasonsContainer = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content").gameObject;
			SeasonPrefab = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content/ListViewEntryPrefab").gameObject;

			ActiveBiomesContainer = transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes/Content/TraitContainer/ScrollArea/Content").gameObject;
			BiomePrefab = transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes/Content/TraitContainer/ScrollArea/Content/Item").gameObject;

			AddSeasonButton = transform.Find("Details/Content/ScrollRectContainer/MeteorSeasonCycle/Content/Seasons/SeasonScrollArea/Content/AddSeasonButton").FindOrAddComponent<FButton>();
			UIUtils.AddSimpleTooltipToObject(AddSeasonButton.transform, METEORSEASONCYCLE.DESCRIPTOR.TOOLTIP);
			AddSeasonButton.OnClick += () =>
			{
				SeasonSelectorScreen.InitializeView(CurrentStarmapItem, () => RefreshMeteorLists());
			};

			UIUtils.AddSimpleTooltipToObject(MeteorSelector.transform.Find("Descriptor"), METEORSEASONCYCLE.DESCRIPTOR.TOOLTIP);


			AsteroidTraits = transform.Find("Details/Content/ScrollRectContainer/AsteroidTraits").gameObject;
			ActiveTraitsContainer = AsteroidTraits.transform.Find("Content/TraitContainer/ScrollArea/Content").gameObject;
			TraitPrefab = AsteroidTraits.transform.Find("Content/TraitContainer/ScrollArea/Content/ListViewEntryPrefab").gameObject;
			TraitPrefab.SetActive(false);

			AddTraitButton = AsteroidTraits.transform.Find("Content/AddSeasonButton").FindOrAddComponent<FButton>();

			AddTraitButton.OnClick += () =>
			{
				TraitSelectorScreen.InitializeView(CurrentStarmapItem, () =>
				{
					RefreshTraitList();
					RefreshGeyserOverrides();
				});
			};

			var buttons = transform.Find("Details/Footer/Buttons");

			ReturnButton = buttons.Find("ReturnButton").FindOrAddComponent<FButton>();
			ReturnButton.OnClick += () => Show(false);

			GenerateClusterButton = buttons.Find("GenerateClusterButton").FindOrAddComponent<FButton>();
			GenerateClusterButton.OnClick += () => InitializeGeneration();

			ResetButton = buttons.Find("ResetSelectionButton").FindOrAddComponent<FButton>();
			ResetButton.OnClick += () =>
			{
				ResetPlanetFromPreset(CurrentStarmapItem.id);
				RefreshView();
			};

			ResetStarmapButton = buttons.Find("StarmapButton").FindOrAddComponent<FButton>();
			ResetStarmapButton.OnClick += () =>
			{

				RegenerateAllPOIData();
				RebuildStarmap(true);
				RefreshView();

			};

			ResetAllButton = buttons.Find("ResetClusterButton").FindOrAddComponent<FButton>();
			ResetAllButton.OnClick += () =>
			{
				ResetToLastPreset();
				RebuildStarmap(true);
				RefreshView();
			};

			PresetsButton = buttons.Find("PresetButton").FindOrAddComponent<FButton>();
			PresetsButton.OnClick += () =>
			{
				OpenPresetWindow(() =>
				{
					RebuildStarmap(false);
					RefreshView();
				}
				);
			};

			//SettingsButton = buttons.Find("SettingsButton").FindOrAddComponent<FButton>();
			//SettingsButton.OnClick += () =>
			//{
			//    CustomSettingsController.ShowWindow(() => RefreshView());
			//};

			UIUtils.AddSimpleTooltipToObject(ResetAllButton.transform, RESETCLUSTERBUTTON.TOOLTIP, true, onBottom: true);
			UIUtils.AddSimpleTooltipToObject(ResetButton.transform, RESETSELECTIONBUTTON.TOOLTIP, true, onBottom: true);
			UIUtils.AddSimpleTooltipToObject(ResetStarmapButton.transform, STARMAPBUTTON.TOOLTIP, true, onBottom: true);
			StartGameTooltip = UIUtils.AddSimpleTooltipToObject(GenerateClusterButton.transform, GENERATECLUSTERBUTTON.TOOLTIP, true, onBottom: true);
			UIUtils.AddSimpleTooltipToObject(ReturnButton.transform, RETURNBUTTON.TOOLTIP, true, onBottom: true);
			//UIUtils.AddSimpleTooltipToObject(SettingsButton.transform, SETTINGSBUTTON.TOOLTIP, true, onBottom: true);
			UIUtils.AddSimpleTooltipToObject(PresetsButton.transform, PRESETBUTTON.TOOLTIP, true, onBottom: true);

			PlanetBiomesGO = transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes").gameObject;

			SeedInput_Main = transform.Find("Details/Footer/Seed/SeedBar/Input").FindOrAddComponent<FInputField2>();
			SeedInput_Main.inputField.onEndEdit.AddListener(SetSeed);

			SeedCycleButton_Main = transform.Find("Details/Footer/Seed/SeedBar/DeleteButton").FindOrAddComponent<FButton>();
			SeedCycleButton_Main.OnClick += () => GetNewRandomSeed();

			var SeedLabel = transform.Find("Details/Footer/Seed/Label").gameObject.AddOrGet<LocText>();
			SeedLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.NAME;
			UIUtils.AddSimpleTooltipToObject(SeedLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.TOOLTIP, alignCenter: true, onBottom: true);

			SeedRerollsVanillaStarmapToggle = transform.Find("Details/Footer/Seed/SeedAfffectingStarmap").gameObject.AddOrGet<FToggle>();
			SeedRerollsVanillaStarmapToggle.SetCheckmark("Background/Checkmark");
			SeedRerollsVanillaStarmapToggle.On = RerollStarmapWithSeedChange;
			SeedRerollsVanillaStarmapToggle.OnClick += (v) =>
			{
				RerollStarmapWithSeedChange = SeedRerollsVanillaStarmapToggle.On;
			};

			var seedStarmapRerollLabel = transform.Find("Details/Footer/Seed/SeedAfffectingStarmap/Label").gameObject.AddOrGet<LocText>();
			UIUtils.TryChangeText(transform, "Details/Footer/Seed/SeedBar/Input/Text Area/Text", STRINGS.UI.SEEDLOCK.NAME_STARMAP);
			UIUtils.TryChangeText(seedStarmapRerollLabel.transform, "", STRINGS.UI.SEEDLOCK.NAME_STARMAP);
			UIUtils.AddSimpleTooltipToObject(seedStarmapRerollLabel.transform, STRINGS.UI.SEEDLOCK.TOOLTIP_STARMAP, alignCenter: true, onBottom: true);

			SeedRerollsTraitsToggle_Main = transform.Find("Details/Footer/Seed/SeedAfffectingTraits").FindOrAddComponent<FToggle>();
			SeedRerollsTraitsToggle_Main.SetCheckmark("Background/Checkmark");
			SeedRerollsTraitsToggle_Main.On = RerollTraitsWithSeedChange;
			SeedRerollsTraitsToggle_Main.OnClick += (v) =>
			{
				RerollTraitsWithSeedChange = SeedRerollsTraitsToggle_Main.On;
			};
			SeedRerollsMixingsToggle = transform.Find("Details/Footer/Seed/SeedAfffectingMixings").FindOrAddComponent<FToggle>();
			UIUtils.TryChangeText(SeedRerollsMixingsToggle.transform, "Label", STRINGS.UI.SEEDLOCK.NAME_MIXING);

			SeedRerollsMixingsToggle.SetCheckmark("Background/Checkmark");
			SeedRerollsMixingsToggle.On = RerollMixingsWithSeedChange;
			SeedRerollsMixingsToggle.OnClick += (v) =>
			{
				RerollMixingsWithSeedChange = v;
			};

			var seedRerollLabel = transform.Find("Details/Footer/Seed/SeedAfffectingTraits/Label").gameObject.AddOrGet<LocText>();
			seedRerollLabel.text = STRINGS.UI.SEEDLOCK.NAME_SHORT;
			UIUtils.AddSimpleTooltipToObject(seedRerollLabel.transform, STRINGS.UI.SEEDLOCK.TOOLTIP, alignCenter: true, onBottom: true);
			UIUtils.AddSimpleTooltipToObject(transform.Find("Details/Content/ScrollRectContainer/AsteroidBiomes/Descriptor/infoIcon"), STRINGS.UI.INFOTOOLTIPS.INFO_ONLY, alignCenter: true, onBottom: true);

			InitializeTraitContainer();
			InitializeMeteorShowerContainers();
			//UIUtils.AddSimpleTooltipToObject(SeedLabel.transform, global::STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES, alignCenter: true, onBottom: true);
			InitializePlanetBiomesContainers();
			InitializeGeyserOverrideContainer();
			InitializePlanetMixingContainer();
			InitializeBiomeMixingContainer();

		}

		private void InitializeBiomeMixingContainer()
		{
			 BiomeMixingContainer = transform.Find("Details/Content/ScrollRectContainer/BiomeMixing").gameObject;
            BiomeMixingContainer?.SetActive(false);
        }

		private void InitializePlanetMixingContainer()
        {
            //UIUtils.ListAllChildrenPath(this.transform);
            WorldMixingContainer = transform.Find("Details/Content/ScrollRectContainer/WorldMixing").gameObject;
			WorldMixingContainer?.SetActive(false);

        }

		private void InitializeGeyserOverrideContainer()
		{
			GeyserContainer = transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers").gameObject;
			ActiveGeyserOverridesContainer = transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Guaranteed/ScrollArea/Content").gameObject;
			GeyserOverridePrefab = ActiveGeyserOverridesContainer.transform.Find("ListViewEntryPrefab").gameObject;
			GeyserOverridePrefab.SetActive(false);
			CapacityText = transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Guaranteed/Descriptor/MaxLabel").gameObject.GetComponent<LocText>();
			UIUtils.AddSimpleTooltipToObject(transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Guaranteed/Descriptor/InfoImage").gameObject, ASTEROIDGEYSERS.CONTENT.GUARANTEED.DESCRIPTOR.INFOTOOLTIP);
			AddGeyserOverrideButton = transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Guaranteed/AddGeyserBtn").gameObject.AddOrGet<FButton>();
			AddGeyserOverrideButton.OnClick += () =>
			{
				GeyserSelectorScreen.InitializeView(CurrentStarmapItem, () => RefreshView());
			};

			ActiveGeyserBlacklistContainer = transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Blacklist/ScrollArea/Content").gameObject;
			GeyserBlacklistPrefab = ActiveGeyserBlacklistContainer.transform.Find("ListViewEntryPrefab").gameObject;
			GeyserBlacklistPrefab.SetActive(false);

			AddBlacklistedGeysers = transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Blacklist/BlacklistButton").gameObject.AddOrGet<FButton>();
			AddBlacklistedGeysers.OnClick += () =>
			{
				GeyserSelectorScreen.InitializeView(CurrentStarmapItem, () => RefreshView(), true);
			};

			BlacklistAffectsNonGenerics = transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Blacklist/BlacklistAffectNonGenerics").gameObject.AddOrGet<FToggle>();
			BlacklistAffectsNonGenerics.SetCheckmark("Background/Checkmark");
			BlacklistAffectsNonGenerics.OnClick += SetCurrentGeyserBlacklistEffect;
			UIUtils.AddSimpleTooltipToObject(BlacklistAffectsNonGenerics.gameObject, ASTEROIDGEYSERS.CONTENT.BLACKLIST.BLACKLISTAFFECTNONGENERICS.TOOLTIP);

			UIUtils.AddSimpleTooltipToObject(transform.Find("Details/Content/ScrollRectContainer/AsteroidGeysers/Content/Blacklist/Descriptor/InfoImage").gameObject, ASTEROIDGEYSERS.CONTENT.BLACKLIST.DESCRIPTOR.INFOTOOLTIP);

			foreach (var entry in ModAssets.AllGeysers)
			{
				AddGeyserBlacklistContainer(entry.Key).SetActive(false);
			}
		}
		void SetCurrentGeyserBlacklistEffect(bool affectsNonGenerics)
		{
			if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item))
			{
				item.SetGeyserBlacklistAffectsNonGenerics(affectsNonGenerics);
			}
		}

		GameObject AddGeyserBlacklistContainer(string geyserID)
		{
			var container = Util.KInstantiateUI(GeyserBlacklistPrefab, ActiveGeyserBlacklistContainer);
			var tr = container.transform;
			if (!ModAssets.AllGeysers.TryGetValue(geyserID, out var geyserData))
			{
				Debug.LogError("geyser " + geyserID + " not found!");
			}

			tr.Find("DeleteButton").gameObject.AddOrGet<FButton>().OnClick += () =>
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item))
				{
					item.RemoveGeyserBlacklist(geyserID);
					RefreshGeyserOverrides();
				}
			};
			tr.Find("Label").gameObject.GetComponent<LocText>().SetText(geyserData.Name);
			UIUtils.AddSimpleTooltipToObject(container, geyserData.Description);
			tr.Find("ImageContainer/Image").GetComponent<Image>().sprite = geyserData.Sprite;
			container.SetActive(true);
			ActiveGeyserBlacklists.Add(geyserID, container);
			return container;
		}
		void AddGeyserOverrideContainer(string geyserID, int index)
		{
			var container = Util.KInstantiateUI(GeyserOverridePrefab, ActiveGeyserOverridesContainer);
			var tr = container.transform;
			if (!ModAssets.AllGeysers.TryGetValue(geyserID, out var geyserData))
			{
				Debug.LogError("geyser " + geyserID + " not found!");
			}

			tr.Find("DeleteButton").gameObject.AddOrGet<FButton>().OnClick += () =>
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item))
				{
					item.RemoveGeyserOverrideAt(index);
					RefreshGeyserOverrides();
				}
			};
			tr.Find("Label").gameObject.GetComponent<LocText>().SetText(geyserData.Name);
			UIUtils.AddSimpleTooltipToObject(container, geyserData.Description);
			tr.Find("ImageContainer/Image").GetComponent<Image>().sprite = geyserData.Sprite;
			container.SetActive(true);
			ActiveGeyserOverrides.Add(container);
		}


		void RemoveSOBand_UI(string itemId)
		{
			if (SOStarmapEntries.ContainsKey(itemId))
			{
				Destroy(SOStarmapEntries[itemId].gameObject);
				SOStarmapEntries.Remove(itemId);
			}
		}


		void RemoveSOSinglePOI(string bandId, string poiID)
		{

			if (CustomCluster.HasStarmapItem(bandId, out var item))
			{
				item.placementPOI.pois.Remove(poiID);
			}
			if (CurrentlySelectedItemData != null && CurrentlySelectedItemData is SelectedSinglePOI_SO)
				CurrentlySelectedItemData = null;
			RemoveSOSinglePOI_UI(bandId, poiID);

			RefreshView();
		}

		void RemoveSOSinglePOI_UI(string bandId, string id)
		{
			if (SOStarmapEntries.ContainsKey(bandId))
			{
				var band = SOStarmapEntries[bandId];
				band.RemovePoiUI(id);
			}
			var toRemove = POIGroup_Entries.FirstOrDefault(item => item.first == id);

			if (toRemove != null)
			{
				Destroy(toRemove.second);
				POIGroup_Entries.Remove(toRemove);
			}
			RebuildStarmap(true);
		}
		void AddSOSinglePOI_UI(string bandId, string id)
		{
			if (SOStarmapEntries.ContainsKey(bandId))
			{
				var band = SOStarmapEntries[bandId];
				band.AddPoiUI(id);
			}

			AddSO_GroupPOIEntry_UI(CurrentStarmapItem.id, id);
			RebuildStarmap(true);
			//CurrentlySelectedItemData = new SelectedSinglePOI_SO(id, bandId);
		}



		public void InitializeSO_POIGroupInfo()
		{
			POIGroup_AllowDuplicates = transform.Find("Details/Content/ScrollRectContainer/POI_AllowDuplicates").FindOrAddComponent<FToggle>();
			POIGroup_AllowDuplicates.SetCheckmark("Background/Checkmark");
			POIGroup_AllowDuplicates.OnClick += (v) =>
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item) && item.category == StarmapItemCategory.POI && item.placementPOI != null)
				{
					if (item.placementPOI.canSpawnDuplicates == v)
						return;

					item.placementPOI.canSpawnDuplicates = v;
					if (DlcManager.IsExpansion1Active())
						ResetSOStarmap(true);
				}
			};
			UIUtils.AddSimpleTooltipToObject(POIGroup_AllowDuplicates.gameObject, POI_ALLOWDUPLICATES.TOOLTIP);

			POIGroup_AvoidClumping = transform.Find("Details/Content/ScrollRectContainer/POI_AvoidClumping").FindOrAddComponent<FToggle>();
			POIGroup_AvoidClumping.SetCheckmark("Background/Checkmark");
			POIGroup_AvoidClumping.OnClick += (v) =>
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item) && item.category == StarmapItemCategory.POI && item.placementPOI != null)
				{
					if (item.placementPOI.avoidClumping == v)
						return;
					item.placementPOI.avoidClumping = v;
					if (DlcManager.IsExpansion1Active())
						ResetSOStarmap(true);
				}
			};

			UIUtils.AddSimpleTooltipToObject(POIGroup_AvoidClumping.gameObject, POI_AVOIDCLUMPING.TOOLTIP);

			POIGroup_Guarantee = transform.Find("Details/Content/ScrollRectContainer/POI_Guarantee").FindOrAddComponent<FToggle>();
			POIGroup_Guarantee.SetCheckmark("Background/Checkmark");
			POIGroup_Guarantee.OnClick += (v) =>
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item) && item.category == StarmapItemCategory.POI && item.placementPOI != null)
				{
					if (item.placementPOI.guarantee == v)
						return;
					item.placementPOI.guarantee = v;
					if (DlcManager.IsExpansion1Active())
						ResetSOStarmap(true);
				}
			};

			UIUtils.AddSimpleTooltipToObject(POIGroup_Guarantee.gameObject, POI_GUARANTEE.TOOLTIP);



			POIGroup_DeletePoiGroup = transform.Find("Details/Content/ScrollRectContainer/SO_POIGroup_Remove").FindOrAddComponent<FButton>();
			POIGroup_DeletePoiGroup.OnClick += () =>
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item) && item.category == StarmapItemCategory.POI)
				{
					RemoveSOBand_UI(CurrentStarmapItem.id);
					CustomCluster.RemovePoiGroup(CurrentStarmapItem.id);
					CurrentlySelectedItemData = null;
					RebuildStarmap(true);
				}
			};
			UIUtils.AddSimpleTooltipToObject(POIGroup_DeletePoiGroup.gameObject, SO_POIGROUP_REMOVE.TOOLTIP);

			POIGroup_AddPoiToGroup = transform.Find("Details/Content/ScrollRectContainer/SO_POIGroup_Container/AddPOIButton").FindOrAddComponent<FButton>();
			POIGroup_AddPoiToGroup.OnClick += () =>
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item) && item.category == StarmapItemCategory.POI && item.placementPOI != null)
				{
					VanillaPOISelectorScreen.InitializeView(CurrentStarmapItem, (id) =>
					{
						CurrentStarmapItem.placementPOI.pois.Add(id);
						CurrentStarmapItem.RefresDuplicateState();
						POIGroup_AllowDuplicates.SetOn(CurrentStarmapItem.placementPOI.canSpawnDuplicates);
						AddSOSinglePOI_UI(CurrentStarmapItem.id, id);

						if (DlcManager.IsExpansion1Active()) ResetSOStarmap(true);
					});
				}
			};
			UIUtils.AddSimpleTooltipToObject(POIGroup_DeletePoiGroup.gameObject, SO_POIGROUP_REMOVE.TOOLTIP);

			POIGroup_POIs = transform.Find("Details/Content/ScrollRectContainer/SO_POIGroup_Container").gameObject;
			POIGroup_Container = transform.Find("Details/Content/ScrollRectContainer/SO_POIGroup_Container/POIContainer/ScrollArea/Content").gameObject;
			POIGroup_EntryPrefab = transform.Find("Details/Content/ScrollRectContainer/SO_POIGroup_Container/POIContainer/ScrollArea/Content/ListViewEntryPrefab").gameObject;
			POIGroup_EntryPrefab.SetActive(false);
		}

		public void InitializeStarmapInfo()
		{
			Details_VanillaPOIContainer = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources").gameObject;// .FindOrAddComponent<FToggle2>();
			Details_VanillaPOIContainer.SetActive(true);
			VanillaPOIResourceContainer = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Content/ResourceContainer/ScrollArea/Content").gameObject;
			VanillaPOIResourcePrefab = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Content/ResourceContainer/ScrollArea/Content/ListViewEntryPrefab").gameObject;
			VanillaPOIResourcePrefab.SetActive(false);
			VanillaPOI_SizeAmountDesc = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Capacity/Amount").gameObject.AddOrGet<LocText>();
			VanillaPOI_POIDesc = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Description").gameObject.AddOrGet<LocText>();

			VanillaPOI_ReplenishmentAmountDesc = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/Replenisment/Amount").gameObject.AddOrGet<LocText>();
			VanillaPOI_ArtifactDesc = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/VanillaPOI_Artifact/Amount").gameObject.AddOrGet<LocText>();
			VanillaPOI_ArtifactTooltip = UIUtils.AddSimpleTooltipToObject(VanillaPOI_ArtifactDesc.transform, "");
			VanillaPOI_RemovePOIBtn = transform.Find("Details/Content/ScrollRectContainer/VanillaPOI_Resources/VanillaPOI_Remove/DeletePOI").gameObject.AddOrGet<FButton>();

			VanillaPOI_RemovePOIBtn.OnClick += () =>
			{
				if (CurrentlySelectedItemData != null && CurrentlySelectedItemData is SelectedSinglePOI_Vanilla data)
				{
					VanillaStarmapEntries[data.Band].RemovePoi(data.ID);

					CurrentlySelectedItemData = null;

					RefreshMissingItemsButton();
				}
				if (CurrentlySelectedItemData != null && CurrentlySelectedItemData is SelectedSinglePOI_SO data_so)
				{
					RemoveSOSinglePOI(data_so.GroupID, data_so.ID);
					CurrentlySelectedItemData = null;
				}
			};
			Details_VanillaPOIContainer.SetActive(false);
			SgtLogger.l("vanilla pois done");
			InitializeSO_POIGroupInfo();
			SgtLogger.l("SO pois done");
		}
		public void InitializeStoryTraits()
		{

			StoryTraitGridContainer = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer").gameObject;
			StoryTraitEntryPrefab = transform.Find("ItemSelection/StoryTraitsContent/StoryTraitsContainer/Item").gameObject;
			StoryTraitEntryPrefab.SetActive(false);
			Details_StoryTraitContainer = transform.Find("Details/Content/ScrollRectContainer/StoryTrait").gameObject;// .FindOrAddComponent<FToggle2>();
			Details_StoryTraitContainer.SetActive(true);
			StoryTraitImage = transform.Find("Details/Content/ScrollRectContainer/StoryTrait/HeaderImage").gameObject.AddOrGet<Image>();
			StoryTraitDesc = transform.Find("Details/Content/ScrollRectContainer/StoryTrait/Description").gameObject.AddOrGet<LocText>();
			StoryTraitToggle = transform.Find("Details/Content/ScrollRectContainer/StoryTrait/StoryTraitEnabled").gameObject.AddOrGet<FToggle>();
			StoryTraitToggle.SetCheckmark("Background/Checkmark");
			Details_StoryTraitContainer.SetActive(false);
			StoryTraitToggle.OnClick +=
				(v) =>
				{
					if (CurrentlySelectedItemData is SelectedStoryTrait)
						ToggleStoryTrait(CurrentlySelectedItemData.ID);
				};

			foreach (Story Story in Db.Get().Stories.resources)
			{
				var entry = Util.KInstantiateUI(StoryTraitEntryPrefab, StoryTraitGridContainer);
				UIUtils.TryChangeText(entry.transform, "Label", Strings.Get(Story.StoryTrait.name));
				entry.transform.Find("Image").GetComponent<Image>().sprite = Assets.GetSprite(Story.StoryTrait.icon);
				var btn = entry.gameObject.AddOrGet<FToggleButton>();
				FToggle toggle = entry.transform.Find("Background").gameObject.AddOrGet<FToggle>();
				toggle.SetCheckmark("Checkmark");
				toggle.OnClick +=
				(v) =>
				{
					SelectStoryTrait(Story.Id);
					ToggleStoryTrait(Story.Id);
				};

				StoryTraitToggleButtons[Story.Id] = btn;
				StoryTraitToggleCheckmarks[Story.Id] = toggle;
				btn.OnClick += () =>
				{
					SelectStoryTrait(Story.Id);
				};

				entry.SetActive(true);
			}
		}

		Dictionary<string, FToggle> StoryTraitToggleCheckmarks = new Dictionary<string, FToggle>();
		Dictionary<string, FToggleButton> StoryTraitToggleButtons = new Dictionary<string, FToggleButton>();

		bool StoryTraitEnabled(string id) => CustomGameSettings.Instance.GetCurrentStoryTraitSetting(id).id == StoryContentPanel.StoryState.Guaranteed.ToString();
		public void ToggleStoryTrait(string id)
		{
			CustomGameSettings.Instance.SetStorySetting(CustomGameSettings.Instance.StorySettings[id], !StoryTraitEnabled(id));
			RefreshStoryTraitsUI();
		}
		void RefreshStoryTraitsUI()
		{
			if (CurrentlySelectedItemData is not SelectedStoryTrait)
				return;
			var data = CurrentlySelectedItemData as SelectedStoryTrait;

			StoryTraitToggle.On = StoryTraitEnabled(data.ID);

			foreach (var state in StoryTraitToggleCheckmarks)
			{
				state.Value.On = StoryTraitEnabled(state.Key);
			}

			foreach (var state in StoryTraitToggleButtons)
			{
				state.Value.ChangeSelection(state.Key == data.ID);
			}
			RefreshDetails();
		}

		public void SelectStoryTrait(string id)
		{
			WorldTrait storyTrait = Db.Get().Stories.GetStoryTrait(id, true);

			CurrentlySelectedItemData = new SelectedStoryTrait(id, Strings.Get(storyTrait.name));


			selectionHeaderLabel.SetText(ModAssets.Strings.ApplyCategoryTypeToString(string.Format(STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.HEADER.LABEL, Strings.Get(storyTrait.name)), SelectedCategory));
			StoryTraitDesc.SetText(Strings.Get(storyTrait.description));
			StoryTraitImage.sprite = Assets.GetSprite(storyTrait.icon.Replace("_icon", "_image"));
			RefreshStoryTraitsUI();
		}


		public void PopulateGalleryAndCategories()
		{
			foreach (var galleryGridButton in planetoidGridButtons)
				Destroy(galleryGridButton.Value.gameObject);
			planetoidGridButtons.Clear();

			foreach (var item in categoryToggles)
				Destroy(item.Value.gameObject);

			categoryToggles.Clear();

			foreach (var Planet in PlanetoidDict)
			{
				AddItemToGallery(Planet.Value);
			}
			foreach (StarmapItemCategory category in AvailableStarmapItemCategories)
			{
				AddCategoryItem(category);
			};
			AddCategoryItem(DlcManager.IsExpansion1Active() ? StarmapItemCategory.POI : StarmapItemCategory.VanillaStarmap);
			if (DlcManager.IsExpansion1Active())
				SpacedOutStarmap_CategoryToggle = AddCategoryItem(StarmapItemCategory.SpacedOutStarmap);


			var StoryTraitsBtn = StoryTraitButton.AddOrGet<CategoryItem>();
			StoryTraitsBtn.transform.Find("Label").GetComponent<LocText>().SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.STORY_TRAITS_HEADER);
			UIUtils.AddSimpleTooltipToObject(StoryTraitsBtn.gameObject, STORYTRAITS.TOOLTIP);
			StoryTraitsBtn.Initialize(StarmapItemCategory.StoryTraits, null);
			StoryTraitsBtn.ActiveToggle.OnClick += () => SelectCategory(StarmapItemCategory.StoryTraits);
			categoryToggles.Add(StarmapItemCategory.StoryTraits, StoryTraitsBtn);

			var MixingSettingsBtn = MixingSettingsButton.AddOrGet<CategoryItem>();
			MixingSettingsBtn.transform.Find("Label").GetComponent<LocText>().SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.MIXING_SETTINGS_HEADER);
			UIUtils.AddSimpleTooltipToObject(MixingSettingsBtn.gameObject, MIXINGSETTINGS.TOOLTIP);
			MixingSettingsBtn.Initialize(StarmapItemCategory.MixingSettings, null);
			MixingSettingsBtn.ActiveToggle.OnClick += () => SelectCategory(StarmapItemCategory.MixingSettings);
			categoryToggles.Add(StarmapItemCategory.MixingSettings, MixingSettingsBtn);

			var GameSettingsBtn = GameSettingsButton.AddOrGet<CategoryItem>();
			GameSettingsBtn.transform.Find("Label").GetComponent<LocText>().SetText(global::STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.CUSTOMIZE);
			UIUtils.AddSimpleTooltipToObject(GameSettingsBtn.gameObject, GAMESETTINGS.TOOLTIP);
			GameSettingsBtn.Initialize(StarmapItemCategory.GameSettings, null);
			GameSettingsBtn.ActiveToggle.OnClick += () => SelectCategory(StarmapItemCategory.GameSettings);
			categoryToggles.Add(StarmapItemCategory.GameSettings, GameSettingsBtn);

			//if (galleryGridLayouter != null)
			//    this.galleryGridLayouter.RequestGridResize();

			MixingSettingsContainer.SetActive(true);
			CustomGameSettingsContainer.SetActive(true);
			StarmapItemContent.SetActive(true);
			StoryTraitGridContent.SetActive(true);
			ClusterItemsContent.SetActive(true);
		}
		void InitializePlanetBiomesContainers()
		{
			UIUtils.TryChangeText(transform, "Details/Content/ScrollRectContainer/AsteroidBiomes/Descriptor/Label", global::STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES + ":");


			Regex rx = new Regex(@"subworlds[\\\/](.*)[\\\/]");
			foreach (var biomeTypePath in SettingsCache.subworlds.Keys)
			{
				var match = rx.Match(biomeTypePath);
				string biomeName = string.Empty;
				if (!match.Success || match.Groups.Count < 2)
				{
					continue;
				}
				biomeName = match.Groups[1].ToString();
				if (!PlanetBiomes.ContainsKey(biomeName))
				{
					var biomeHolder = Util.KInstantiateUI(BiomePrefab, ActiveBiomesContainer, true);
					//SgtLogger.l(biomeName, "BIOMETYPE");

					string name = Strings.Get("STRINGS.SUBWORLDS." + biomeName.ToUpperInvariant() + ".NAME");
					string description = Strings.Get("STRINGS.SUBWORLDS." + biomeName.ToUpperInvariant() + ".DESC");

					Sprite biomeSprite = GameUtil.GetBiomeSprite(biomeName);
					var icon = biomeHolder.transform.Find("Image").GetComponent<Image>();
					icon.sprite = biomeSprite;

					UIUtils.AddSimpleTooltipToObject(biomeHolder.transform, description, true, 250, true);
					var LocTextName = biomeHolder.transform.Find("Label").GetComponent<LocText>();
					LocTextName.fontSizeMax = 18f;
					LocTextName.fontSizeMin = LocTextName.fontSize - 7f;
					LocTextName.enableAutoSizing = true;
					UIUtils.TryChangeText(biomeHolder.transform, "Label", name);


					PlanetBiomes[biomeName] = biomeHolder;

				}
			}

			RefreshPlanetBiomes();
		}

		void InitializeMeteorShowerContainers()
		{
			///SeasonContainer
			foreach (var gameplaySeason in Db.Get().GameplaySeasons.resources)
			{
				if (!(gameplaySeason is MeteorShowerSeason) || gameplaySeason.Id.Contains("Fullerene") || gameplaySeason.Id.Contains("TemporalTear") || !DlcManager.IsContentSubscribed(gameplaySeason.dlcId))
					continue;

				var meteorSeason = gameplaySeason as MeteorShowerSeason;
				var seasonInstanceHolder = Util.KInstantiateUI(SeasonPrefab, ActiveSeasonsContainer, true);
				string name = meteorSeason.Name.Replace("MeteorShowers", string.Empty);
				if (name == string.Empty)
					name = METEORSEASONCYCLE.VANILLASEASON;
				string description = meteorSeason.events.Count == 0 ? METEORSEASONCYCLE.CONTENT.SEASONTYPENOMETEORSTOOLTIP : METEORSEASONCYCLE.CONTENT.SEASONTYPETOOLTIP;

				foreach (var meteorShower in meteorSeason.events)
				{
					var shower = meteorShower as MeteorShowerEvent;
					description += "\n • ";
					description += shower.Id;// Assets.GetPrefab((Tag)meteor.prefab).GetProperName();
					description += ":";
					foreach (var info in shower.GetMeteorsInfo())
					{
						var meteor = Assets.GetPrefab(info.prefab);
						if (meteor == null) continue;

						description += "\n    • ";
						description += meteor.GetProperName();
					}
				}
				UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform, description);
				var LocTextName = seasonInstanceHolder.transform.Find("Label").GetComponent<LocText>();
				LocTextName.fontSizeMax = LocTextName.fontSize;
				LocTextName.fontSizeMin = LocTextName.fontSize - 7f;
				LocTextName.enableAutoSizing = true;
				UIUtils.TryChangeText(seasonInstanceHolder.transform, "Label", name);
				UIUtils.AddSimpleTooltipToObject(seasonInstanceHolder.transform.Find("Label"), description);


				var RemoveButton = seasonInstanceHolder.transform.Find("DeleteButton").gameObject.FindOrAddComponent<FButton>();
				var SwitchButton = seasonInstanceHolder.transform.Find("SwitchButton").gameObject.FindOrAddComponent<FButton>();
				UIUtils.AddSimpleTooltipToObject(SwitchButton.transform, METEORSEASONCYCLE.SWITCHTOOTHERSEASONTOOLTIP);
				UIUtils.AddSimpleTooltipToObject(RemoveButton.transform, METEORSEASONCYCLE.REMOVESEASONTOOLTIP);


				RemoveButton.OnClick += () =>
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item))
					{
						item.RemoveMeteorSeason(meteorSeason.Id); //SeasonSelectorScreen.InitializeView(lastSelected, () => UpdateUI());
					}
					RefreshMeteorLists();
				};
				SwitchButton.OnClick += () =>
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item))
					{
						SeasonSelectorScreen.InitializeView(CurrentStarmapItem, () => RefreshMeteorLists(), meteorSeason.Id);
					}
				};
				SeasonTypes[gameplaySeason.Id] = seasonInstanceHolder;
			}

			///Shower Container
			foreach (var MeteorPreset in Assets.GetPrefabsWithComponent<Comet>())
			{
				string meteorId = MeteorPreset.GetComponent<KPrefabID>().PrefabID().ToString();
				//SgtLogger.l(meteorId, "METEOR");
				var cometInstanceHolder = Util.KInstantiateUI(MeteorPrefab, ActiveMeteorsContainer, true);

				string description = METEORSEASONCYCLE.SHOWERTOOLTIP;
				var icon = cometInstanceHolder.transform.Find("TraitImage").GetComponent<Image>();
				icon.sprite = Def.GetUISprite(MeteorPreset).first;

				UIUtils.TryChangeText(cometInstanceHolder.transform, "Label", MeteorPreset.GetProperName());
				MeteorTypeTooltips[meteorId] = UIUtils.AddSimpleTooltipToObject(cometInstanceHolder, description);
				MeteorTypes[meteorId] = cometInstanceHolder;
			}
			RefreshMeteorLists();
		}
		void InitializeTraitContainer()
		{
			foreach (var kvp in ModAssets.AllTraitsWithRandom)
			{
				var TraitHolder = Util.KInstantiateUI(TraitPrefab, ActiveTraitsContainer, true);
				var RemoveButton = TraitHolder.transform.Find("DeleteButton").FindOrAddComponent<FButton>();
				Strings.TryGet(kvp.Value.name, out var name);
				Strings.TryGet(kvp.Value.description, out var description);

				var combined = UIUtils.ColorText(name.ToString(), kvp.Value.colorHex);

				var icon = TraitHolder.transform.Find("TraitImage").GetComponent<Image>();

				icon.sprite = ModAssets.GetTraitSprite(kvp.Value);
				icon.color = Util.ColorFromHex(kvp.Value.colorHex);
				if (kvp.Key == ModAssets.CGM_RandomTrait)
				{
					combined = UIUtils.RainbowColorText(name.ToString());
					TraitHolder.transform.Find("AwailableRandomTraits").gameObject.SetActive(true);

					RandomTraitBlacklistOpener = TraitHolder.transform.Find("AwailableRandomTraits").FindOrAddComponent<FButton>();
					RandomTraitBlacklistOpener.gameObject.SetActive(true);
					UIUtils.AddSimpleTooltipToObject(RandomTraitBlacklistOpener.transform, AWAILABLERANDOMTRAITS.TOOLTIP);
					RandomTraitBlacklistOpener.OnClick += () => TraitSelectorScreen.InitializeView(null, () => RefreshTraitList(), true);
				}

				UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
				UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);



				RemoveButton.OnClick += () =>
				{
					if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var item))
					{
						item.RemoveWorldTrait(kvp.Value);
					}
					RefreshTraitList();
					RefreshGeyserOverrides();
				};

				if (kvp.Key == ModAssets.CGM_RandomTrait)
				{
					RandomTraitDeleteButton = RemoveButton;
				}

				Traits[kvp.Value.filePath] = TraitHolder;
			}
			RefreshTraitList();
		}
		public void RefreshGeyserOverrides()
		{
			for (int i = ActiveGeyserOverrides.Count - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(ActiveGeyserOverrides[i]);
			}
			ActiveGeyserOverrides.Clear();

			if (CurrentStarmapItem == null || CurrentStarmapItem.world == null)
				return;

			int maxSlots = CurrentStarmapItem.GetMaxGeyserOverrideCount();
			int currentSlots = CurrentStarmapItem.GetCurrentGeyserOverrideCount();


			GeyserContainer.SetActive(true);
			CapacityText.SetText(currentSlots + " / " + maxSlots);
			bool hasItem = CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out _);

			AddGeyserOverrideButton.SetInteractable(maxSlots - currentSlots > 0 && hasItem);
			AddBlacklistedGeysers.SetInteractable(hasItem);
			BlacklistAffectsNonGenerics.SetInteractable(hasItem);

			for (int i = 0; i < CurrentStarmapItem.GeyserOverrideIDs.Count; i++)
			{
				var id = CurrentStarmapItem.GeyserOverrideIDs[i];
				AddGeyserOverrideContainer(id, i);
			}
			foreach (var blacklistedEntry in ActiveGeyserBlacklists)
			{
				blacklistedEntry.Value.SetActive(CurrentStarmapItem.HasGeyserBlacklisted(blacklistedEntry.Key));
			}
			BlacklistAffectsNonGenerics.SetOnFromCode(CurrentStarmapItem.GeyserBlacklistAffectsNonGenerics);
		}
		public void RefreshPlanetBiomes()
		{
			if (CurrentStarmapItem == null || CurrentStarmapItem.world == null)
				return;
			foreach (var biomeHolder in PlanetBiomes.Values)
			{
				biomeHolder.SetActive(false);
			}
			Regex rx = new Regex(@"subworlds[\\\/](.*)[\\\/]");
			foreach (var subworld in CurrentStarmapItem.world.subworldFiles)
			{
				var match = rx.Match(subworld.name);
				string biomeName = string.Empty;
				if (!match.Success || match.Groups.Count < 2)
				{
					continue;
				}

				biomeName = match.Groups[1].ToString();
				if (PlanetBiomes.ContainsKey(biomeName))
					PlanetBiomes[biomeName].SetActive(true);
			}
			///world mixing:
			if (CurrentStarmapItem.IsMixed && CurrentStarmapItem.placement != null
				&& CurrentStarmapItem.placement.worldMixing != null
				&& CurrentStarmapItem.placement.worldMixing.additionalSubworldFiles != null
				)
			{
				foreach (var subworld in CurrentStarmapItem.placement.worldMixing.additionalSubworldFiles)
				{
					var match = rx.Match(subworld.name);
					string biomeName = string.Empty;
					if (!match.Success || match.Groups.Count < 2)
					{
						continue;
					}
					biomeName = match.Groups[1].ToString();
					if (PlanetBiomes.ContainsKey(biomeName))
						PlanetBiomes[biomeName].SetActive(true);
				}
			}
			///Biome Mixing (TODO!)
			///
		}
		Dictionary<string, string> CometDescriptions = new Dictionary<string, string>();

		public void RefreshMeteorLists()
		{
			if (CurrentStarmapItem == null)
				return;
			foreach (var meteorType in MeteorTypes.Values)
			{
				meteorType.SetActive(false);
			}
			CometDescriptions.Clear();
			foreach (var activeShower in CurrentStarmapItem.CurrentMeteorShowerTypes)
			{
				foreach (var cometType in activeShower.GetMeteorsInfo())
				{
					if (MeteorTypes.ContainsKey(cometType.prefab))
					{
						if (!CometDescriptions.ContainsKey(cometType.prefab))
							CometDescriptions[cometType.prefab] = string.Empty;

						CometDescriptions[cometType.prefab] += "\n • " + activeShower.Id;
						MeteorTypes[cometType.prefab].SetActive(true);
					}
				}
			}
			foreach (var entry in CometDescriptions)
			{
				MeteorTypeTooltips[entry.Key].SetSimpleTooltip(METEORSEASONCYCLE.SHOWERTOOLTIP + entry.Value);
			}

			foreach (var seasonHolder in SeasonTypes.Values)
			{
				seasonHolder.SetActive(false);
			}
			foreach (var activeSeason in CurrentStarmapItem.CurrentMeteorSeasons)
			{
				if (SeasonTypes.ContainsKey(activeSeason.Id))
					SeasonTypes[activeSeason.Id].SetActive(true);
			}
		}
		public void RefreshTraitList()
		{
			if (CurrentStarmapItem == null)
				return;

			foreach (var traitContainer in Traits.Values)
			{
				traitContainer.SetActive(false);
			}

			if (CurrentStarmapItem.IsRandom)
			{
				Traits[ModAssets.CGM_RandomTrait].SetActive(true);
			}
			else
			{
				foreach (var activeTrait in CurrentStarmapItem.CurrentTraits)
				{
					if (Traits.TryGetValue(activeTrait, out var go))
					{
						go.SetActive(true);
					}
					else
						SgtLogger.error(activeTrait + " had no gameObject!!");
				}
			}
		}

		public void SetResetButtonStates()
		{

			if (ResetButton != null)
				ResetButton.SetInteractable(!PresetApplied);

			if (ResetStarmapButton != null)
				ResetStarmapButton.SetInteractable(!PresetApplied);

			if (ResetAllButton != null)
				ResetAllButton.SetInteractable(!PresetApplied);
		}

		private bool _presetApplied = false;
		public bool PresetApplied
		{
			get
			{
				return _presetApplied;
			}
			set
			{
				_presetApplied = value;
				SetResetButtonStates();
			}
		}

		void TryChangingNumber(string msg)
		{
			if (CurrentStarmapItem != null && float.TryParse(msg, out var value))
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
				{
					if (current.InstancesToSpawn == value)
						return;

					current.SetSpawnNumber(value);
					RefreshGallery();

					if (CurrentStarmapItem == RandomOuterPlanetsStarmapItem)
					{
						MaxClassicOuterPlanets = Mathf.Min(MaxClassicOuterPlanets, Mathf.RoundToInt(CurrentStarmapItem.InstancesToSpawn) + 2);
					}
					if (CurrentStarmapItem.IsPOI)
						RefreshPOIGroupHeader(CurrentStarmapItem.id);
					RefreshDetails();
					if (DlcManager.IsExpansion1Active()) ResetSOStarmap(true);
				}
			}

		}

		void TryApplyingCoordinates(string msg, bool Height)
		{
			if (int.TryParse(msg, out var size))
			{
				if (CustomCluster.HasStarmapItem(CurrentStarmapItem.id, out var current))
				{
					if (size == (Height ? current.CustomPlanetDimensions.Y : current.CustomPlanetDimensions.X))
						return;

					current.ApplyCustomDimension(size, Height);
					UpdateSizeLabels(current);
				}
			}
		}


		string Warning3 = "EC1802";
		string Warning2 = "ff8102";
		string Warning1 = "F6D42A";

		public void UpdateSizeLabels(StarmapItem current)
		{
			PlanetSizeWidth.EditTextFromData(current.CustomPlanetDimensions.X.ToString());
			PlanetSizeHeight.EditTextFromData(current.CustomPlanetDimensions.Y.ToString());
			PercentageLargerThanTerra(current, out var Percentage);
			if (Percentage > 200)
			{
				AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning3);
				AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning3));
			}
			else if (Percentage > 100)
			{
				AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning2);
				AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning2));
			}
			else if (Percentage > 33)
			{
				AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning1);
				AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(string.Format(ASTEROIDSIZE.SIZEWARNING, Percentage), Warning1));
			}
			else
			{
				AsteroidSizeLabel.text = ASTEROIDSIZE.DESCRIPTOR.LABEL;
				AsteroidSizeTooltip.SetSimpleTooltip(ASTEROIDSIZE.DESCRIPTOR.TOOLTIP);
			}

			if (current.CurrentSizeMultiplier < 0.6f && !PlanetIsClassic(current))
			{
				AsteroidSizeLabel.text = UIUtils.ColorText(ASTEROIDSIZE.DESCRIPTOR.LABEL, Warning3);
				AsteroidSizeTooltip.SetSimpleTooltip(UIUtils.ColorText(ASTEROIDSIZE.BIOMEMISSINGWARNING, Warning3));
			}
			//geyser count scales with size
			RefreshGeyserOverrides();
		}
		void PercentageLargerThanTerra(StarmapItem current, out int dimensions)
		{
			float TerraArea = 240 * 380;
			float CustomSize = current.CustomPlanetDimensions.X * current.CustomPlanetDimensions.Y;

			dimensions = Mathf.RoundToInt(CustomSize / TerraArea * 100f);
			dimensions -= 100;
		}

		private void OnMouseOverToggle() => KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover"));

		public void AddItemToGallery(StarmapItem planet)
		{
			if (planetoidGridButtons.ContainsKey(planet))
			{
				SgtLogger.warning(planet.id + " was already in the gallery");
				return;
			}

			// PermitPresentationInfo presentationInfo = permit.GetPermitPresentationInfo();
			GameObject availableGridButton = Util.KInstantiateUI(PlanetoidEntryPrefab, galleryGridContainer);
			var itemLogic = availableGridButton.AddOrGet<GalleryItem>();
			itemLogic.Initialize(planet);


			LocText itemNameText = availableGridButton.transform.Find("Label").GetComponent<LocText>();
			itemNameText.SetText(planet.DisplayName);
			UIUtils.TryChangeText(availableGridButton.transform, "Label", planet.DisplayName);


			itemLogic.ActiveToggle.OnClick += () => SelectItem(planet);
			itemLogic.ActiveToggle.OnDoubleClick += () =>
			{
				SelectItem(planet);
				if (CurrentStarmapItem != null)
				{
					TogglePlanetoid(CurrentStarmapItem);
					RefreshView();

					if (DlcManager.IsExpansion1Active())
						ResetSOStarmap(true);
				}
			};
			planetoidGridButtons[planet] = itemLogic;
			//this.SetItemClickUISound(planet, component2);
			availableGridButton.SetActive(true);
		}


		private CategoryItem AddCategoryItem(StarmapItemCategory StarmapItemCategory)
		{
			GameObject categoryItem = Util.KInstantiateUI(PlanetoidCategoryPrefab, categoryListContent, true);

			string categoryName = ModAssets.Strings.CategoryEnumToName(StarmapItemCategory); //CATEGORYENUM

			categoryItem.transform.Find("Label").GetComponent<LocText>().SetText(categoryName);
			var item = categoryItem.AddOrGet<CategoryItem>();
			item.Initialize(StarmapItemCategory, Assets.GetSprite("unknown"));
			item.ActiveToggle.OnClick += () => SelectCategory(StarmapItemCategory);
			categoryToggles.Add(StarmapItemCategory, item);
			return item;
		}

		#endregion

	}
}
