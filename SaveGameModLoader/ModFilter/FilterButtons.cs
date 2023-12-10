using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI.RESEARCHSCREEN;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using UnityEngine.UI;
using HarmonyLib;
using static SaveGameModLoader.ModFilter.FilterPatches;
using System.Runtime.CompilerServices;
using static ModsScreen;

namespace SaveGameModLoader.ModFilter
{
    internal class FilterButtons : KMonoBehaviour
    {
        KButton hideIncompatible_btn, hideDev_btn, hideLocal_btn, HideSteam_btn, HideActive_btn, HideInactive_btn, hidePins_btn;
        LocText hideIncompatible_lbl, hideDev_lbl, hideLocal_lbl, hideSteam_lbl, HideActive_lbl, HideInactive_lbl, hidePins_lbl;
        KImage hideIncompatible_img, hideDev_img, hideLocal_img, hideSteam_img, HideActive_img, HideInactive_img, hidePins_img;

        GameObject devFiller, localFiller, pinFiller, steamFiller;

        ColorStyleSetting normal, highlighted;
        System.Action RefreshModList = null;

        Dropdown PlatformDropDown;

        public static FilterButtons Instance;
        public static bool hasDev, hasLocal, hasSteam, hasPins;

        internal void Init(System.Action _refresh)
        {
            Instance = this;
            var buttonPrefab = transform.Find("CloseButton").gameObject;
            buttonPrefab.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 96);
            //buttonPrefab.TryGetComponent<KImage>(out var referenceImg);

            //UIUtils.ListAllChildren(transform.parent);

            ///normal styling from X button in the title bar
            transform.parent.Find("Title/CloseButton").TryGetComponent<KImage>(out var referenceImg);

            var style = referenceImg.colorStyleSetting;
            normal = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            normal.inactiveColor = style.inactiveColor;
            normal.activeColor = style.activeColor;
            normal.disabledColor = style.disabledColor;
            normal.hoverColor = style.hoverColor;


            highlighted = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            highlighted.inactiveColor = UIUtils.Lerp(style.inactiveColor, Color.red, 40f);
            highlighted.activeColor = UIUtils.Lerp(style.activeColor, Color.red, 40f);
            highlighted.disabledColor = UIUtils.Lerp(style.disabledColor, Color.red, 40f);
            highlighted.hoverColor = UIUtils.Lerp(style.hoverColor, Color.red, 40f);

            buttonPrefab.SetActive(false);

            pinFiller = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            Destroy(pinFiller.transform.Find("Text").gameObject);
            Destroy(pinFiller.GetComponent<Image>());


            devFiller = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            Destroy(devFiller.transform.Find("Text").gameObject);
            Destroy(devFiller.GetComponent<Image>());

            localFiller = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            Destroy(localFiller.transform.Find("Text").gameObject);
            Destroy(localFiller.GetComponent<Image>());

            steamFiller = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            Destroy(steamFiller.transform.Find("Text").gameObject);
            Destroy(steamFiller.GetComponent<Image>());
            //var dropDownContainer = Util.KInstantiateUI(FilterPatches._dropDownPrefab, gameObject, true);
            //UIUtils.ListAllChildrenPath(dropDownContainer.transform);
            //UIUtils.ListAllChildrenWithComponents(dropDownContainer.transform);
            //dropDownContainer.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 5, 330);
            //dropDownContainer.transform.Find("Label").TryGetComponent<LocText>(out var dropDownLabel);
            //if (!dropDownContainer.transform.Find("Dropdown").TryGetComponent<Dropdown>(out var drop))
            //    SgtLogger.error("DropDownNotFoun");
            //drop.gameObject.name = Dropdown_Show_Patch.TargetName;

            //drop.ClearOptions();
            //List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            //options.Add(new Dropdown.OptionData(STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.DEV));
            //options.Add(new Dropdown.OptionData(STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.LOCAL));
            //options.Add(new Dropdown.OptionData(STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.STEAM));
            //drop.AddOptions(options);
            //UtilMethods.ListAllPropertyValues(drop);

            //drop.onValueChanged.AddListener(v => 
            //{
            //    HandleStateInput(v);
            //}
            //);
            //drop.value = 0;
            //drop.gameObject.SetActive(true);
            //PlatformDropDown = drop;

            #region buttons

            var pinButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            hidePins_lbl = pinButton.transform.Find("Text").GetComponent<LocText>();
            hidePins_btn = pinButton.GetComponent<KButton>();
            hidePins_btn.onClick += () =>
            {
                MPM_Config.Instance.hidePins ^= true;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            //HideInactive_btn.onDoubleClick += () =>
            //{
            //    MPM_Config.Instance.ToggleAll();
            //    MPM_Config.Instance.hidePins = false;
            //    MPM_Config.Instance.SaveToFile();
            //    RefreshUIState();
            //};
            UIUtils.AddSimpleTooltipToObject(pinButton, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PINS_TOOLTIP);
            pinButton.TryGetComponent<KImage>(out hidePins_img);

            var devButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            hideDev_lbl = devButton.transform.Find("Text").GetComponent<LocText>();
            hideDev_btn = devButton.GetComponent<KButton>();
            hideDev_btn.onClick += () =>
            {
                MPM_Config.Instance.hideDev ^= true;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            //hideLocal_btn.onDoubleClick += () =>
            //{
            //    MPM_Config.Instance.ToggleAll();
            //    MPM_Config.Instance.hideLocal = false;
            //    MPM_Config.Instance.SaveToFile();
            //    RefreshUIState();
            //};
            UIUtils.AddSimpleTooltipToObject(devButton, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_DEV_TOOLTIP);
            devButton.TryGetComponent<KImage>(out hideDev_img);


            var localButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            hideLocal_lbl = localButton.transform.Find("Text").GetComponent<LocText>();
            hideLocal_btn = localButton.GetComponent<KButton>();
            hideLocal_btn.onClick += () =>
            {
                MPM_Config.Instance.hideLocal ^= true;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            //hideLocal_btn.onDoubleClick += () =>
            //{
            //    MPM_Config.Instance.ToggleAll();
            //    MPM_Config.Instance.hideLocal = false;
            //    MPM_Config.Instance.SaveToFile();
            //    RefreshUIState();
            //};
            UIUtils.AddSimpleTooltipToObject(localButton, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_LOCAL_TOOLTIP);
            localButton.TryGetComponent<KImage>(out hideLocal_img);


            var steamButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            hideSteam_lbl = steamButton.transform.Find("Text").GetComponent<LocText>();
            HideSteam_btn = steamButton.GetComponent<KButton>();
            HideSteam_btn.onClick += () =>
            {
                MPM_Config.Instance.hidePlatform ^= true;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            //HideSteam_btn.onDoubleClick += () =>
            //{
            //    MPM_Config.Instance.ToggleAll();
            //    MPM_Config.Instance.hideLocal = false;
            //    MPM_Config.Instance.SaveToFile();
            //    RefreshUIState();
            //};
            UIUtils.AddSimpleTooltipToObject(steamButton, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PLATFORM_TOOLTIP);
            steamButton.TryGetComponent<KImage>(out hideSteam_img);



            var activeButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            HideActive_lbl = activeButton.transform.Find("Text").GetComponent<LocText>();
            HideActive_btn = activeButton.GetComponent<KButton>();
            HideActive_btn.onClick += () =>
            {
                MPM_Config.Instance.hideActive ^= true;
                if (MPM_Config.Instance.hideActive)
                    MPM_Config.Instance.hideInactive = false;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            //HideActive_btn.onDoubleClick += () =>
            //{
            //    MPM_Config.Instance.ToggleAll();
            //    MPM_Config.Instance.hideActive = false;
            //    MPM_Config.Instance.SaveToFile();
            //    RefreshUIState();
            //};
            UIUtils.AddSimpleTooltipToObject(activeButton, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_ACTIVE_TOOLTIP);
            activeButton.TryGetComponent<KImage>(out HideActive_img);

            var inactiveButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            HideInactive_lbl = inactiveButton.transform.Find("Text").GetComponent<LocText>();
            HideInactive_btn = inactiveButton.GetComponent<KButton>();
            HideInactive_btn.onClick += () =>
            {
                MPM_Config.Instance.hideInactive ^= true;
                if (MPM_Config.Instance.hideInactive)
                    MPM_Config.Instance.hideActive = false;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            //HideInactive_btn.onDoubleClick += () =>
            //{
            //    MPM_Config.Instance.ToggleAll();
            //    MPM_Config.Instance.hideInactive = false;
            //    MPM_Config.Instance.SaveToFile();
            //    RefreshUIState();
            //};
            UIUtils.AddSimpleTooltipToObject(inactiveButton, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INACTIVE_TOOLTIP);
            inactiveButton.TryGetComponent<KImage>(out HideInactive_img);


            var incompatibleButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            hideIncompatible_lbl = incompatibleButton.transform.Find("Text").GetComponent<LocText>();
            hideIncompatible_btn = incompatibleButton.GetComponent<KButton>();
            hideIncompatible_btn.onClick += () =>
            {
                MPM_Config.Instance.hideIncompatible ^= true;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            //hideIncompatible_btn.onDoubleClick += () =>
            //{
            //    MPM_Config.Instance.ToggleAll();
            //    MPM_Config.Instance.hideIncompatible = false;
            //    MPM_Config.Instance.SaveToFile();
            //    RefreshUIState();
            //};
            incompatibleButton.TryGetComponent<KImage>(out hideIncompatible_img);
            UIUtils.AddSimpleTooltipToObject(incompatibleButton, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INCOMPATIBLE_TOOLTIP);


            #endregion

            steamFiller.transform.SetSiblingIndex(9);
            localFiller.transform.SetSiblingIndex(7);
            devFiller.transform.SetSiblingIndex(5);
            pinFiller.transform.SetSiblingIndex(3);
            RefreshModList = _refresh;
            RefreshUIState(false);


            devFiller.name = "Dev_Spacer";
            localFiller.name = "Local_Spacer";
            pinFiller.name = "Pinned_Spacer";
            steamFiller.name = "Steam_Spacer";
            hideIncompatible_btn.gameObject.name = "Incompatible_Filter";
            hideDev_btn.gameObject.name = "Dev_Filter";
            hideLocal_btn.gameObject.name = "Local_Filter";
            HideSteam_btn.gameObject.name = "Steam_Filter";
            HideActive_btn.gameObject.name = "Active_Filter";
            HideInactive_btn.gameObject.name = "Inactive_Filter";
            hidePins_btn.gameObject.name = "Pinned_Filter";
        }



        public void RefreshUIState(bool rebuildAfter = true)
        {
            hasLocal = Global.Instance.modManager.mods.Any(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Local);
            hasDev = Global.Instance.modManager.mods.Any(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Dev);
            hasSteam = Global.Instance.modManager.mods.Any(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Steam);
            hasPins = MPM_Config.Instance.HasPinned;


            hidePins_img.gameObject.SetActive(hasPins);
            hideLocal_img.gameObject.SetActive(hasLocal);
            hideSteam_img.gameObject.SetActive(hasSteam);
            hideDev_img.gameObject.SetActive(hasDev);

            steamFiller.SetActive(!hasSteam);
            localFiller.SetActive(!hasLocal);
            devFiller.SetActive(!hasDev);
            pinFiller.SetActive(!hasPins);



            HideActive_lbl.SetText(MPM_Config.Instance.hideActive ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_ACTIVE : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_ACTIVE);
            HideInactive_lbl.SetText(MPM_Config.Instance.hideInactive ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_INACTIVE : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INACTIVE);
            hideIncompatible_lbl.SetText(MPM_Config.Instance.hideIncompatible ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_INCOMPATIBLE : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INCOMPATIBLE);
            
            hideLocal_lbl.SetText(MPM_Config.Instance.hideLocal ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_LOCAL : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_LOCAL);
            hideLocal_btn.interactable = hasLocal;
            
            hideSteam_lbl.SetText(MPM_Config.Instance.hidePlatform ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_PLATFORM : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PLATFORM);
            HideSteam_btn.interactable = hasSteam;
            
            hidePins_lbl.SetText(MPM_Config.Instance.hidePins ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_PINS : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PINS);
            hidePins_btn.interactable = hasPins;
            
            hideDev_lbl.SetText(MPM_Config.Instance.hideDev ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_DEV : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_DEV);
            hideDev_btn.interactable = hasDev;

            hidePins_img.colorStyleSetting = (MPM_Config.Instance.hidePins ? highlighted : normal);
            hidePins_img.ApplyColorStyleSetting();

            hideIncompatible_img.colorStyleSetting = (MPM_Config.Instance.hideIncompatible ? highlighted : normal);
            hideIncompatible_img.ApplyColorStyleSetting();

            hideLocal_img.colorStyleSetting = (MPM_Config.Instance.hideLocal ? highlighted : normal);
            hideLocal_img.ApplyColorStyleSetting();

            hideSteam_img.colorStyleSetting = (MPM_Config.Instance.hidePlatform ? highlighted : normal);
            hideSteam_img.ApplyColorStyleSetting();

            HideActive_img.colorStyleSetting = (MPM_Config.Instance.hideActive ? highlighted : normal);
            HideActive_img.ApplyColorStyleSetting();

            HideInactive_img.colorStyleSetting = (MPM_Config.Instance.hideInactive ? highlighted : normal);
            HideInactive_img.ApplyColorStyleSetting();

            hideDev_img.colorStyleSetting = (MPM_Config.Instance.hideDev ? highlighted : normal);
            hideDev_img.ApplyColorStyleSetting();
           // hideDev_img.gameObject.SetActive(hasDev);





            if (RefreshModList != null && rebuildAfter)
                RefreshModList();
        }

        internal void ReorderVisualModState(List<ModsScreen.DisplayedMod> displayedMods, List<KMod.Mod> mods)
        {
            return;//Todo for later;
            Dictionary<KMod.Mod, RectTransform> originalPos = new();
            for(int i= 0; i<displayedMods.Count; i++)
            {
                var displayedMod = displayedMods[i];
                var mod = mods[displayedMod.mod_index];
                originalPos.Add(mod, displayedMod.rect_transform);
            }


            var sorted =
                mods
                .OrderBy(mod => MPM_Config.Instance.ModPinned(mod.label.defaultStaticID))
                .ThenByDescending(mod => mod.label.title);


            foreach (var mod in sorted)
            {
                if (originalPos.ContainsKey(mod))
                    originalPos[mod].SetAsFirstSibling();
                else
                    SgtLogger.l(mod.label.title + " not found in dictionary");
            }

        }
    }
}
