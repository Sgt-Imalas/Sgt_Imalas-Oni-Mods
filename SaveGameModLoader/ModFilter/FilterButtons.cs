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

namespace SaveGameModLoader.ModFilter
{
    internal class FilterButtons : KMonoBehaviour
    {
        KButton hideIncompatible_btn, hideLocal_btn, HideSteam_btn, HideActive_btn, HideInactive_btn, hidePins_btn;
        LocText hideIncompatible_lbl, hideLocal_lbl, hideSteam_lbl, HideActive_lbl, HideInactive_lbl, hidePins_lbl;
        KImage hideIncompatible_img, hideLocal_img, hideSteam_img, HideActive_img, HideInactive_img, hidePins_img;

        ColorStyleSetting normal, highlighted;
        System.Action RefreshModList = null;

        internal void Init(System.Action _refresh)
        {
            var buttonPrefab = transform.Find("CloseButton").gameObject;
            buttonPrefab.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            buttonPrefab.TryGetComponent<KImage>(out var referenceImg);
            var style = referenceImg.colorStyleSetting;
            normal = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            normal.inactiveColor = style.inactiveColor;
            normal.activeColor = style.activeColor;
            normal.disabledColor = style.disabledColor;
            normal.hoverColor = style.hoverColor;


            highlighted = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            highlighted.inactiveColor = UIUtils.Lerp( style.inactiveColor, Color.red, 33f);
            highlighted.activeColor = UIUtils.Lerp(style.activeColor, Color.red, 33f);
            highlighted.disabledColor = UIUtils.Lerp(style.disabledColor, Color.red, 33f);
            highlighted.hoverColor = UIUtils.Lerp(style.hoverColor, Color.red, 33f);

            buttonPrefab.SetActive(false);

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



            var incompatibleButton = Util.KInstantiateUI(buttonPrefab,gameObject,true);
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


            var localButton = Util.KInstantiateUI(buttonPrefab, gameObject, true);
            hideLocal_lbl = localButton.transform.Find("Text").GetComponent<LocText>();
            hideLocal_btn = localButton.GetComponent<KButton>();
            hideLocal_btn.onClick += () =>
            {
                MPM_Config.Instance.hideLocal ^= true;
                if (MPM_Config.Instance.hideLocal)
                    MPM_Config.Instance.hidePlatform = false;
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
                if (MPM_Config.Instance.hidePlatform)
                    MPM_Config.Instance.hideLocal=false;
                
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
            RefreshModList = _refresh;


            RefreshModList = _refresh;

            RefreshUIState(true);
        }

        void RefreshUIState(bool init = false)
        {
            hideIncompatible_lbl.SetText(MPM_Config.Instance.hideIncompatible ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_INCOMPATIBLE : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INCOMPATIBLE);
            hideLocal_lbl.SetText(MPM_Config.Instance.hideLocal ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_LOCAL : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_LOCAL);
            hideSteam_lbl.SetText(MPM_Config.Instance.hidePlatform ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_PLATFORM : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PLATFORM);
            HideActive_lbl.SetText(MPM_Config.Instance.hideActive ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_ACTIVE : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_ACTIVE);
            HideInactive_lbl.SetText(MPM_Config.Instance.hideInactive ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_INACTIVE : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_INACTIVE);
            hidePins_lbl.SetText(MPM_Config.Instance.hidePins ? STRINGS.UI.FRONTEND.FILTERSTRINGS.UNHIDE_PINS : STRINGS.UI.FRONTEND.FILTERSTRINGS.HIDE_PINS);

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

            if (RefreshModList != null && !init)
                RefreshModList();
        }
    }
}
