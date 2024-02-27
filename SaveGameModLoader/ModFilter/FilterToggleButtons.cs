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
using YamlDotNet.Core;

namespace SaveGameModLoader.ModFilter
{
    internal class FilterToggleButtons : KMonoBehaviour
    {
        FToggle2 hideIncompatible_btn, hideDev_btn, hideLocal_btn, HideSteam_btn, HideActive_btn, HideInactive_btn, hidePins_btn;
        //LocText hideIncompatible_lbl, hideDev_lbl, hideLocal_lbl, hideSteam_lbl, HideActive_lbl, HideInactive_lbl, hidePins_lbl;
        KImage hideIncompatible_img, hideDev_img, hideLocal_img, hideSteam_img, HideActive_img, HideInactive_img, hidePins_img;

        GameObject devFiller, localFiller, pinFiller, steamFiller;

        ColorStyleSetting normal, highlighted;
        System.Action RefreshModList = null;

        Dropdown PlatformDropDown;

        public static FilterToggleButtons Instance;
        public static bool hasDev, hasLocal, hasSteam, hasPins;

        public static GameObject togglePrefab;

        internal void Init(System.Action _refresh)
        {
            Instance = this;
            var buttonPrefab = transform.Find("CloseButton").gameObject;
            buttonPrefab.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 96);
            //buttonPrefab.TryGetComponent<KImage>(out var referenceImg);

            UIUtils.ListAllChildren(transform.parent);

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

            #region buttons


            hidePins_btn = GenerateCheckbox(togglePrefab, gameObject, STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.PINNED, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PINS_TOOLTIP);
            hidePins_btn.On = !MPM_Config.Instance.hidePins;
            hidePins_btn.OnClick += () =>
            {
                MPM_Config.Instance.hidePins = !hidePins_btn.On;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };

            hideDev_btn = GenerateCheckbox(togglePrefab, gameObject, STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.DEV, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_DEV_TOOLTIP);
            hideDev_btn.On = !MPM_Config.Instance.hideDev;
            hideDev_btn.OnClick += () =>
            {
                MPM_Config.Instance.hideDev = !hideDev_btn.On;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };

            hideLocal_btn = GenerateCheckbox(togglePrefab, gameObject, STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.LOCAL, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_LOCAL_TOOLTIP);
            hideLocal_btn.On = !MPM_Config.Instance.hideLocal;
            hideLocal_btn.OnClick += () =>
            {
                MPM_Config.Instance.hideLocal = !hideLocal_btn.On;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };
            
            HideSteam_btn = GenerateCheckbox(togglePrefab, gameObject, STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.STEAM, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PLATFORM_TOOLTIP);
            HideSteam_btn.On = !MPM_Config.Instance.hidePlatform;
            HideSteam_btn.OnClick += () =>
            {
                MPM_Config.Instance.hidePlatform = !HideSteam_btn.On;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };

            HideActive_btn = GenerateCheckbox(togglePrefab, gameObject, STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.ACTIVE, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_ACTIVE_TOOLTIP);
            HideActive_btn.On = !MPM_Config.Instance.hideActive;
            HideActive_btn.OnClick += () =>
            {
                MPM_Config.Instance.hideActive = !HideActive_btn.On;
                if (MPM_Config.Instance.hideActive)
                    MPM_Config.Instance.hideInactive = false;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };

            HideInactive_btn = GenerateCheckbox(togglePrefab, gameObject, STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.INACTIVE, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INACTIVE_TOOLTIP);
            HideInactive_btn.On = !MPM_Config.Instance.hideInactive;
            HideInactive_btn.OnClick += () =>
            {
                MPM_Config.Instance.hideInactive = !HideInactive_btn.On;
                if (MPM_Config.Instance.hideInactive)
                    MPM_Config.Instance.hideActive = false;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };

            hideIncompatible_btn = GenerateCheckbox(togglePrefab, gameObject, STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.INCOMPATIBLE, STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INCOMPATIBLE_TOOLTIP);
            hideIncompatible_btn.On = !MPM_Config.Instance.hideIncompatible;
            hideIncompatible_btn.OnClick += () =>
            {
                MPM_Config.Instance.hideIncompatible = !hideIncompatible_btn.On;
                MPM_Config.Instance.SaveToFile();
                RefreshUIState();
            };


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
        FToggle2 GenerateCheckbox(GameObject checkboxcontainer, GameObject parent,string LabelText, string tooltip)
        {
            var buttonGO = Util.KInstantiateUI(togglePrefab, parent, true);
            UIUtils.TryChangeText(buttonGO.transform, "Label", LabelText);
            //buttonGO.TryGetComponent<RectTransform>(out var RT);
            
            buttonGO.TryGetComponent<RectTransform>(out var RT);
            RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 96);

            var toggle = buttonGO.transform.Find("Checkbox").gameObject.AddComponent<FToggle2>();
            //buttonGO.transform.Find("Label").TryGetComponent<LayoutElement>(out LE);
            //LE.minWidth = 30;


            toggle.mark = buttonGO.transform.Find("Checkbox/CheckMark").GetComponent<Image>();

            UIUtils.AddSimpleTooltipToObject(buttonGO, tooltip);
            return toggle;
        }


        public void RefreshUIState(bool rebuildAfter = true)
        {
            hasLocal = Global.Instance.modManager.mods.Any(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Local);
            hasDev = Global.Instance.modManager.mods.Any(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Dev);
            hasSteam = Global.Instance.modManager.mods.Any(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Steam);
            hasPins = MPM_Config.Instance.HasPinned;


            //hidePins_img.gameObject.SetActive(hasPins);
            //hideLocal_img.gameObject.SetActive(hasLocal);
            //hideSteam_img.gameObject.SetActive(hasSteam);
            //hideDev_img.gameObject.SetActive(hasDev);

            steamFiller.SetActive(!hasSteam);
            localFiller.SetActive(!hasLocal);
            devFiller.SetActive(!hasDev);
            pinFiller.SetActive(!hasPins);

            hideIncompatible_btn.SetOn(!MPM_Config.Instance.hideIncompatible);
            HideActive_btn.SetOn(!MPM_Config.Instance.hideActive);
            HideInactive_btn.SetOn(!MPM_Config.Instance.hideInactive);
            hideLocal_btn.SetOn(!MPM_Config.Instance.hideLocal);
            HideSteam_btn.SetOn(!MPM_Config.Instance.hidePlatform);
            hideDev_btn.SetOn(!MPM_Config.Instance.hideDev);
            hidePins_btn.SetOn(!MPM_Config.Instance.hidePins);


            if (RefreshModList != null && rebuildAfter)
                RefreshModList();
        }
    }
}
