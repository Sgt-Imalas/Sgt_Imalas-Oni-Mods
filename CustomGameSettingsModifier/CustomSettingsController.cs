using Database;
using Epic.OnlineServices.Sessions;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static CustomGameSettings;
using static SandboxSettings;

namespace CustomGameSettingsModifier
{
    internal class CustomSettingsController : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore IDE0051 // Remove unused private members
        public static CustomSettingsController Instance = null;

        private FCycle ImmuneSystem;
        private FCycle CalorieBurn;
        private FCycle Morale;
        private FCycle Durability;
        private FCycle MeteorShowers;
        private FCycle Radiation;
        private FCycle Stress;
        private FToggle2 StressBreaks;
        private FToggle2 CarePackages;
        //private FToggle2 SandboxMode;
        private FToggle2 FastWorkersMode;

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

            ///ImmuneSystem
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.ImmuneSystem.id))
            {
                ImmuneSystem.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ImmuneSystem).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.ImmuneSystem);
                ImmuneSystem.Value = isNoSweat ? CustomGameSettingConfigs.ImmuneSystem.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.ImmuneSystem.GetDefaultLevelId();
            }

            ///CalorieBurn
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.CalorieBurn.id))
            {
                CalorieBurn.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.CalorieBurn);
                CalorieBurn.Value = isNoSweat ? CustomGameSettingConfigs.CalorieBurn.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.CalorieBurn.GetDefaultLevelId();
            }

            ///Morale
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Morale.id))
            {
                Morale.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Morale).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.Morale);
                Morale.Value = isNoSweat ? CustomGameSettingConfigs.Morale.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Morale.GetDefaultLevelId();
            }

            ///Durability (suits)
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Durability.id))
            {
                Durability.Value= instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Durability).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.Durability);
                Durability.Value = isNoSweat ? CustomGameSettingConfigs.Durability.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Durability.GetDefaultLevelId();
            }

            ///MeteorShowers
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.MeteorShowers.id))
            {
                MeteorShowers.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.MeteorShowers).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.MeteorShowers);
                MeteorShowers.Value = isNoSweat ? CustomGameSettingConfigs.MeteorShowers.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.MeteorShowers.GetDefaultLevelId();
            }

            ///Radiation
            if (DlcManager.IsExpansion1Active())
            {
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Radiation.id))
                {
                    Radiation.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Radiation).id;
                }
                else
                {
                    AddMissingCustomGameSetting(CustomGameSettingConfigs.Radiation);
                    Radiation.Value = isNoSweat ? CustomGameSettingConfigs.Radiation.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Radiation.GetDefaultLevelId();
                }
            }

            ///Stress
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Stress.id))
            {
                Stress.Value = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Stress).id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.Stress);
                Stress.Value = isNoSweat ? CustomGameSettingConfigs.Stress.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Stress.GetDefaultLevelId();
            }

            ///StressBreaks
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.StressBreaks.id))
            {
                StressBreaks.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.StressBreaks).id == (CustomGameSettingConfigs.StressBreaks as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.StressBreaks);
                StressBreaks.On = isNoSweat 
                    ? CustomGameSettingConfigs.StressBreaks.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.StressBreaks as ToggleSettingConfig).on_level.id 
                    : CustomGameSettingConfigs.StressBreaks.GetDefaultLevelId() == (CustomGameSettingConfigs.StressBreaks as ToggleSettingConfig).on_level.id;
            }

            ///CarePackages
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.CarePackages.id))
            {
                CarePackages.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CarePackages).id == (CustomGameSettingConfigs.CarePackages as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.CarePackages);
                CarePackages.On = isNoSweat
                    ? CustomGameSettingConfigs.CarePackages.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.CarePackages as ToggleSettingConfig).on_level.id
                    : CustomGameSettingConfigs.CarePackages.GetDefaultLevelId() == (CustomGameSettingConfigs.CarePackages as ToggleSettingConfig).on_level.id;
            }

            ///Fast Workers
            if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.FastWorkersMode.id))
            {
                FastWorkersMode.On = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.FastWorkersMode).id == (CustomGameSettingConfigs.FastWorkersMode as ToggleSettingConfig).on_level.id;
            }
            else
            {
                AddMissingCustomGameSetting(CustomGameSettingConfigs.FastWorkersMode);
                FastWorkersMode.On = isNoSweat
                    ? CustomGameSettingConfigs.FastWorkersMode.GetNoSweatDefaultLevelId() == (CustomGameSettingConfigs.FastWorkersMode as ToggleSettingConfig).on_level.id
                    : CustomGameSettingConfigs.FastWorkersMode.GetDefaultLevelId() == (CustomGameSettingConfigs.FastWorkersMode as ToggleSettingConfig).on_level.id;
            }

        }

        private void SetCustomGameSettings(SettingConfig ConfigToSet, object valueId)
        {
            string valueToSet = valueId.ToString();
            if(valueId is bool)
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

            UIUtils.AddSimpleTooltipToObject(transform.Find("Content/Warning"), STRINGS.UI.CUSTOMGAMESETTINGSCHANGER.CHANGEWARNINGTOOLTIP);

            StressBreaks = transform.Find("Content/StressReactions").FindOrAddComponent<FToggle2>();

            var StressBreaksLabel = StressBreaks.transform.Find("Label").gameObject.AddOrGet<LocText>();
            StressBreaksLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.NAME;
            UIUtils.AddSimpleTooltipToObject(StressBreaksLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.TOOLTIP);

            StressBreaks.SetCheckmark("Background/Checkmark");
            StressBreaks.OnClick += (v) =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.StressBreaks, StressBreaks.On);
            };
            
            CarePackages = transform.Find("Content/CarePackages").FindOrAddComponent<FToggle2>();

            var CarePackagesLabel = CarePackages.transform.Find("Label").gameObject.AddOrGet<LocText>();
            CarePackagesLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.NAME;
            UIUtils.AddSimpleTooltipToObject(CarePackagesLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.TOOLTIP);

            CarePackages.SetCheckmark("Background/Checkmark");
            CarePackages.OnClick += (v) =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.CarePackages, CarePackages.On);
            };

            
            /**
            SandboxMode = transform.Find("Content/SandboxMode").FindOrAddComponent<FToggle2>();

            var SandboxModeLabel = SandboxMode.transform.Find("Label").gameObject.AddOrGet<LocText>();
            SandboxModeLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.NAME;
            UIUtils.AddSimpleTooltipToObject(SandboxModeLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.TOOLTIP);

            SandboxMode.SetCheckmark("Background/Checkmark");
            SandboxMode.OnClick += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.SandboxMode, SandboxMode.On);
            };
            **/
            FastWorkersMode = transform.Find("Content/FastWorkers").FindOrAddComponent<FToggle2>();

            var FastWorkersModeLabel = FastWorkersMode.transform.Find("Label").gameObject.AddOrGet<LocText>();
            FastWorkersModeLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.NAME;
            UIUtils.AddSimpleTooltipToObject(FastWorkersModeLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.TOOLTIP);

            FastWorkersMode.SetCheckmark("Background/Checkmark");
            FastWorkersMode.OnClick += (v) =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.FastWorkersMode, FastWorkersMode.On);
            };

            ///Immune System Strength
            ImmuneSystem = transform.Find("Content/ImmuneSystem").gameObject.AddOrGet<FCycle>();
            var ImmuneLabel = ImmuneSystem.transform.Find("Label").gameObject.AddOrGet<LocText>();
            ImmuneLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.NAME;
            UIUtils.AddSimpleTooltipToObject(ImmuneLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.TOOLTIP);

            ImmuneSystem.Initialize(
                ImmuneSystem.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                ImmuneSystem.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                ImmuneSystem.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                ImmuneSystem.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            ImmuneSystem.Options = new List<FCycle.Option>();
            foreach(var config in CustomGameSettingConfigs.ImmuneSystem.GetLevels())
            {
                ImmuneSystem.Options.Add(new FCycle.Option(config.id,config.label,config.tooltip));
            }
            ImmuneSystem.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.ImmuneSystem, ImmuneSystem.Value);
            };

            ///Calorie Usage
            CalorieBurn = transform.Find("Content/Calories").gameObject.AddOrGet<FCycle>();

            var CalorieBurnLabel = CalorieBurn.transform.Find("Label").gameObject.AddOrGet<LocText>();
            CalorieBurnLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.NAME;
            UIUtils.AddSimpleTooltipToObject(CalorieBurnLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.TOOLTIP);

            CalorieBurn.Initialize(
                CalorieBurn.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                CalorieBurn.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                CalorieBurn.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                CalorieBurn.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            CalorieBurn.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.CalorieBurn.GetLevels())
            {
                CalorieBurn.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            CalorieBurn.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.CalorieBurn, CalorieBurn.Value);
            };

            ///Morale Requirements
            Morale = transform.Find("Content/Morale").gameObject.AddOrGet<FCycle>();

            var MoraleLabel = Morale.transform.Find("Label").gameObject.AddOrGet<LocText>();
            MoraleLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.NAME;
            UIUtils.AddSimpleTooltipToObject(MoraleLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.TOOLTIP);

            Morale.Initialize(
                Morale.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                Morale.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                Morale.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                Morale.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            Morale.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.Morale.GetLevels())
            {
                Morale.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            Morale.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.Morale, Morale.Value);
            };

            ///Suit Durability Settings
            Durability = transform.Find("Content/Suits").gameObject.AddOrGet<FCycle>();

            var DurabilityLabel = Durability.transform.Find("Label").gameObject.AddOrGet<LocText>();
            DurabilityLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.DURABILITY.NAME;
            UIUtils.AddSimpleTooltipToObject(DurabilityLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.DURABILITY.TOOLTIP);

            Durability.Initialize(
                Durability.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                Durability.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                Durability.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                Durability.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            Durability.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.Durability.GetLevels())
            {
                Durability.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            Durability.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.Durability, Durability.Value);
            };

            ///Meteors
            MeteorShowers = transform.Find("Content/MeteorShowers").gameObject.AddOrGet<FCycle>();

            var MeteorLabel = MeteorShowers.transform.Find("Label").gameObject.AddOrGet<LocText>();
            MeteorLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.METEORSHOWERS.NAME;
            UIUtils.AddSimpleTooltipToObject(MeteorLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.METEORSHOWERS.TOOLTIP);

            MeteorShowers.Initialize(
                MeteorShowers.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                MeteorShowers.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                MeteorShowers.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                MeteorShowers.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            MeteorShowers.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.MeteorShowers.GetLevels())
            {
                MeteorShowers.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            MeteorShowers.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.MeteorShowers, MeteorShowers.Value);
            };

            ///Meteors
            Radiation = transform.Find("Content/Rads").gameObject.AddOrGet<FCycle>();
            if (DlcManager.IsExpansion1Active())
            {
                Radiation.Initialize(
                    Radiation.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                    Radiation.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                    Radiation.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                    Radiation.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());


                var RadiationLabel = Radiation.transform.Find("Label").gameObject.AddOrGet<LocText>();
                RadiationLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.RADIATION.NAME;
                UIUtils.AddSimpleTooltipToObject(RadiationLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.RADIATION.TOOLTIP);

                Radiation.Options = new List<FCycle.Option>();
                foreach (var config in CustomGameSettingConfigs.Radiation.GetLevels())
                {
                    Radiation.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
                }
                Radiation.OnChange += () =>
                {
                    SetCustomGameSettings(CustomGameSettingConfigs.Radiation, Radiation.Value);
                };
            }
            else
            {
                Radiation.gameObject.SetActive(false);
            }

            ///Stress
            Stress = transform.Find("Content/Stress").gameObject.AddOrGet<FCycle>();

            var STRESSLabel = Stress.transform.Find("Label").gameObject.AddOrGet<LocText>();
            STRESSLabel.text = global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.NAME;
            UIUtils.AddSimpleTooltipToObject(STRESSLabel.transform, global::STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.TOOLTIP);

            Stress.Initialize(
                Stress.transform.Find("Left").gameObject.AddOrGet<FButton>(),
                Stress.transform.Find("Right").gameObject.AddOrGet<FButton>(),
                Stress.transform.Find("ChoiceLabel").gameObject.AddOrGet<LocText>(),
                Stress.transform.Find("ChoiceLabel/Description").gameObject.AddOrGet<LocText>());

            Stress.Options = new List<FCycle.Option>();
            foreach (var config in CustomGameSettingConfigs.Stress.GetLevels())
            {
                Stress.Options.Add(new FCycle.Option(config.id, config.label, config.tooltip));
            }
            Stress.OnChange += () =>
            {
                SetCustomGameSettings(CustomGameSettingConfigs.Stress, Stress.Value);
            };


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
