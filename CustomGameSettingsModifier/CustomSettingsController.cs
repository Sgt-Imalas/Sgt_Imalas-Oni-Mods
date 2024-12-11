using Klei.CustomSettings;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static CustomGameSettings;

namespace CustomGameSettingsModifier
{
	internal class CustomSettingsController : FScreen
	{
#pragma warning disable IDE0051 // Remove unused private members
		new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
		public static CustomSettingsController Instance = null;



        GameObject CustomGameSettingsContainer;


        Dictionary<string, FToggle> CustomGameSettingsToggleConfigs = new();
        Dictionary<string, FCycle> CustomGameSettingsCycleConfigs = new();

        public FButton CloseButton;
		public FButton CloseButton2;

		public bool CurrentlyActive;
		public static void ShowWindow()
		{
			if (Instance == null)
			{
				var screen = Util.KInstantiateUI(ModAssets.CustomGameSettings, PauseScreen.Instance.transform.parent.gameObject, true);
				Instance = screen.AddOrGet<CustomSettingsController>();
				Instance.Init();
			}

			Instance.Show(true);
			Instance.ConsumeMouseScroll = true;
			Instance.transform.SetAsLastSibling();
			Instance.LoadGameSettings();
		}


		private bool init;




		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
			{
				this.Show(false);
			}

			base.OnKeyDown(e);
		}

		private void AddMissingCustomGameSetting(SettingConfig type)
		{

			SgtLogger.warning(type.GetType().ToString() + " value not found, defaulting");
			CustomGameSettings.Instance.QualitySettings[type.id] = type;
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

        private void SetCustomGameSettings(SettingConfig ConfigToSet, object valueId)
		{
			string valueToSet = valueId.ToString();
			if (valueId is bool)
			{
				var toggle = ConfigToSet as ToggleSettingConfig;
				valueToSet = ((bool)valueId) ? toggle.on_level.id : toggle.off_level.id;
			}
			SgtLogger.l("changing " + ConfigToSet.id.ToString() + " from " + CustomGameSettings.Instance.GetCurrentQualitySetting(ConfigToSet).id + " to " + valueToSet.ToString());
			CustomGameSettings.Instance.SetQualitySetting(ConfigToSet, valueToSet);
		}

		private void Init()
		{
			CloseButton = transform.Find("Title/CloseButton").FindOrAddComponent<FButton>();
			CloseButton2 = transform.Find("Close").FindOrAddComponent<FButton>();

			CloseButton.OnClick += () => this.Show(false);
			CloseButton2.OnClick += () => this.Show(false);

            UIUtils.AddSimpleTooltipToObject(transform.Find("WarningIcon"), STRINGS.UI.CUSTOMGAMESETTINGSCHANGER.CHANGEWARNINGTOOLTIP);
            UIUtils.AddSimpleTooltipToObject(transform.Find("ChangeWarning"), STRINGS.UI.CUSTOMGAMESETTINGSCHANGER.CHANGEWARNINGTOOLTIP);
            CustomGameSettingsContainer = transform.Find("Content/ScrollRectContainer").gameObject;
            InitializeGameSettings();
            init = true;
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
            SgtLogger.l("initializing custom game settings");
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
        public override void OnShow(bool show)
		{
			base.OnShow(show);
			if (!init)
			{
				Init();
			}
			CurrentlyActive = show;
		}
	}
}
