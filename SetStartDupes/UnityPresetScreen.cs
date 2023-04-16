using Database;
using Epic.OnlineServices.Sessions;
using FMOD;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static SandboxSettings;

namespace SetStartDupes
{
    internal class UnityPresetScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
        public static UnityPresetScreen Instance = null;

        public FButton GeneratePresetButton;
        public FButton CloseButton;
        public FButton ApplyButton;

        public GameObject InfoHeaderPrefab;
        public GameObject InfoRowPrefab;
        public GameObject InfoScreenContainer;

        public GameObject PresetListContainer;
        public GameObject PresetListPrefab;

        public FButton OpenPresetFolder;
        public FButton ClearSearchBar;

        public bool CurrentlyActive;

        MinionStatConfig CurrentlySelected;



        Dictionary<MinionStatConfig, GameObject> Presets = new Dictionary<MinionStatConfig, GameObject>();

        public static void ShowWindow()
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.PresetWindowPrefab, PauseScreen.Instance.transform.parent.gameObject, true);
                Instance = screen.AddOrGet<UnityPresetScreen>(); 
                Instance.Init();
            }
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling(); 
            Instance.LoadAllPresets(); 
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

        List<MinionStatConfig> LoadPresets()
        {
            List<MinionStatConfig> minionStatConfigs = new List<MinionStatConfig>();
            var files = new DirectoryInfo(ModAssets.DupeTemplatePath).GetFiles();


            for (int i = 0; i < files.Count(); i++)
            {
                var File = files[i];
                try
                {
                    var preset = MinionStatConfig.ReadFromFile(File);
                    if (preset != null)
                    {
                        minionStatConfigs.Add(preset);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load minion preset from: " + File.FullName + ", Error: " + e);
                }
            }
            return minionStatConfigs;
        }

        private void AddUiElementForPreset(MinionStatConfig config)
        {
            if (!Presets.ContainsKey(config))
            {
                var PresetHolder = Util.KInstantiateUI(PresetListPrefab, PresetListContainer, true);
                UIUtils.TryChangeText(PresetHolder.transform, "Label", config.ConfigName);
                PresetHolder.transform.Find("RenameButton").FindOrAddComponent<FButton>().OnClick += () => config.OpenPopUpToChangeName( ()=> UIUtils.TryChangeText(PresetHolder.transform, "Label", config.ConfigName));
                PresetHolder.transform.Find("AddThisTraitButton").FindOrAddComponent<FButton>().OnClick += () => SetAsCurrent(config);
                PresetHolder.transform.Find("DeleteButton").FindOrAddComponent<FButton>().OnClick += () => DeletePreset(config);
                Presets[config] = PresetHolder;
            }
        }

        void DeletePreset(MinionStatConfig config)
        {
            if (Presets.ContainsKey(config))
            {
                Destroy(Presets[config]);
                Presets.Remove(config);
                config.DeleteFile();
            }
        }

        void SetAsCurrent(MinionStatConfig config)
        {
            CurrentlySelected = config;
            //TopBarLabel.text = config.ConfigName;
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
            GeneratePresetButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/GenerateFromCurrentButton").FindOrAddComponent<FButton>();
            CloseButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/CloseButton").FindOrAddComponent<FButton>();
            ApplyButton = transform.Find("HorizontalLayout/ItemInfo/Buttons/ApplyPresetButton").FindOrAddComponent<FButton>() ;

            OpenPresetFolder = transform.Find("HorizontalLayout/ObjectList/SearchBar/FolderButton").FindOrAddComponent<FButton>();
            ClearSearchBar = transform.Find("HorizontalLayout/ObjectList/SearchBar/DeleteButton").FindOrAddComponent<FButton>();

            CloseButton.OnClick += () => this.Show(false);
            
            InfoHeaderPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/HeaderPrefab").gameObject; ;
            InfoRowPrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/ItemPrefab").gameObject; ;

            InfoScreenContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content").gameObject; 

            PresetListContainer = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content").gameObject;
            PresetListPrefab = transform.Find("HorizontalLayout/ObjectList/ScrollArea/Content/PresetEntryPrefab").gameObject;


        init = true;
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
