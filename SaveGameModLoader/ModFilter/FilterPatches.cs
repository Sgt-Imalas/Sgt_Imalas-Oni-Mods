using HarmonyLib;
using SaveGameModLoader.ModsFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SaveGameModLoader.AllPatches;
using TMPro;
using SaveGameModLoader.Patches;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using UtilLibs;
using KMod;

namespace SaveGameModLoader.ModFilter
{
    internal class FilterPatches
    {
        /// <summary>
        /// Original Code written by Asquared31415, reused with her permission
        /// </summary>
        [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
        public static class MainMenuSearchBarInit
        {
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix()
            {
                var prefabGo = ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject;
                prefabGo.SetActive(false);
                var clone = Util.KInstantiateUI(prefabGo);
                if (clone != null)
                {
                    var go = clone.transform.Find("Content/ColonyData/Colonies and Achievements/Colonies/Search");
                    if (go != null)
                    {
                        _prefab = Util.KInstantiateUI(go.gameObject);
                        var btn = go.transform.Find("ClearButton");
                        if (btn != null)
                        {
                            _copyToClipboardPrefab = Util.KInstantiateUI(btn.gameObject);

                            _buttonPrefab = Util.KInstantiateUI(btn.gameObject);
                            var bgImage = _buttonPrefab.transform.Find("GameObject").GetComponent<Image>();
                            bgImage.sprite = Assets.GetSprite(SpritePatch.pinSymbol);
                            bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
                            bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);


                            UnityEngine.Object.Destroy(_buttonPrefab.GetComponent<Image>());
                            UnityEngine.Object.Destroy(_buttonPrefab.GetComponent<ToolTip>());


                        }

                        UnityEngine.Object.Destroy(clone);
                        prefabGo.SetActive(true);
                    }
                }
                // ERROR!
                else
                    Debug.Log("[ModProfileManager] Error creating search prefab!  The searchbar will not function!");


                if (Config.Instance.ButtonStyle == Config.FilterbuttonStyle.Checkbox)
                {
                    var options = Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject);
                    SgtLogger.Assert("options", options);
                    var optionscreenclone = Util.KInstantiateUI<GameOptionsScreen>(options.gameOptionsScreenPrefab.gameObject);
                    SgtLogger.Assert("optionscreenclone", optionscreenclone);
                    FilterToggleButtons.togglePrefab = Util.KInstantiateUI(optionscreenclone.unitConfiguration.toggleUnitPrefab.gameObject);
                    SgtLogger.Assert("togglePrefab", FilterToggleButtons.togglePrefab);
                    UnityEngine.Object.Destroy(options.gameObject);
                    UnityEngine.Object.Destroy(optionscreenclone.gameObject);
                }
            }
        }

        [HarmonyPatch(typeof(ModsScreen), nameof(ModsScreen.ShouldDisplayMod))]
        public static class ModsScreen_ShouldDisplayMod_Patch
        {

            [HarmonyPostfix]
            [HarmonyPriority(Priority.Low)]
            public static void Postfix(KMod.Mod mod, ref bool __result)
            {
                var label = mod.label;

                ///Modsfilter is active
                if (FilterManager.ModFilterTextCmp != null && !__result)
                {
                    var text = FilterManager.ModFilterText;
                    if (!string.IsNullOrEmpty(text))
                    {
                        __result = ModAssets.ModAuthorFilter(text, mod);
                    }
                }

                if (__result && ModlistManager.Instance.IsSyncing)
                {
                    __result = ModlistManager.Instance.ModIsNotInSync(mod);
                }

                if (__result && MPM_Config.Instance.hidePins)
                    __result = !MPM_Config.Instance.PinnedMods.Contains(mod.label.defaultStaticID);


                if (__result && _filterManager != null)
                {
                    var text = _filterManager.Text;

                    if (!string.IsNullOrEmpty(text))
                    {
                        __result = ModAssets.ModWithinTextFilter(text, mod);
                    }
                }



                if (__result && MPM_Config.Instance.hideLocal)
                    __result = label.distribution_platform != Label.DistributionPlatform.Local;

                if (__result && MPM_Config.Instance.hideDev)
                    __result = label.distribution_platform != Label.DistributionPlatform.Dev;

                if (__result && MPM_Config.Instance.hidePlatform)
                    __result = label.distribution_platform == Label.DistributionPlatform.Dev || label.distribution_platform == Label.DistributionPlatform.Local;

                if (__result && MPM_Config.Instance.hideIncompatible)
                    __result = mod.contentCompatability == ModContentCompatability.OK;

                if (__result && MPM_Config.Instance.hideActive)
                    __result = !mod.IsActive();

                if (__result && MPM_Config.Instance.hideInactive)
                    __result = mod.IsActive();

            }
        }

        //[HarmonyPatch(typeof(ModsScreen), "OnActivate")]

        public static GameObject _copyToClipboardPrefab;
        public static GameObject _buttonPrefab;
        public static GameObject _prefab;
        public static FilterManager _filterManager;
        public static ModsScreen _modsScreen;
        //public static GameObject _dropDownPrefab;
        public static class ModsScreen_OnActivate_SearchBar_Patch
        {
            public static void ExecutePatch(Harmony harmony)
            {
                var m_TargetMethod = AccessTools.Method("ModsScreen, Assembly-CSharp:OnActivate");
                var m_Prefix = AccessTools.Method(typeof(ModsScreen_OnActivate_SearchBar_Patch), "Prefix");
                harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix), null);
            }
            public static void Prefix(ModsScreen __instance)
            {
                if (_prefab == null)
                {
                    return;
                }

                _modsScreen = __instance;

                var local = Util.KInstantiateUI(_prefab);
                var panel = __instance.transform.Find("Panel");
                if (panel != null)
                {
                    var trans = local.transform;
                    trans.SetParent(panel, false);
                    trans.SetSiblingIndex(1);
                    local.SetActive(true);
                    local.GetComponent<HorizontalLayoutGroup>().spacing = 3;
                    var LA = local.GetComponent<LayoutElement>();
                    LA.minHeight = 42;
                    LA.preferredHeight = 42;

                    //side padding
                    var leftPadding = Util.KInstantiateUI(trans.Find("ClearButton/GameObject").gameObject, local, true);
                    var rightPadding = Util.KInstantiateUI(trans.Find("ClearButton/GameObject").gameObject, local, true);
                    UnityEngine.Object.Destroy(leftPadding.GetComponent<Image>());
                    UnityEngine.Object.Destroy(rightPadding.GetComponent<Image>());
                    leftPadding.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 4);
                    rightPadding.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 4);

                    leftPadding.transform.SetAsFirstSibling();
                    rightPadding.transform.SetAsLastSibling();

                    //del img
                    var symbolTr = trans.Find("ClearButton/GameObject");
                    symbolTr.GetComponent<Image>().sprite = Assets.GetSprite(SpritePatch.delSymbol);
                    symbolTr.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 28);
                    symbolTr.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 28);

                    _filterManager = new FilterManager(
                        trans.Find("LocTextInputField").GetComponent<TMP_InputField>(),
                        trans.Find("ClearButton").GetComponent<KButton>()
                    );

                    _filterManager.ConfigureButtons(_modsScreen);
                }
                else
                {
                    Debug.Log("[ModFilter] Error adding search bar to mods screen!");
                }
            }
        }

        [HarmonyPatch(typeof(ModsScreen), "OnActivate")]
        public static class ModsScreen_OnActivate_FilterCategories_Patch
        {
            public static void Prefix(ModsScreen __instance)
            {
                if (_prefab == null)
                {
                    return;
                }

                _modsScreen = __instance;

                var panel = __instance.transform.Find("Panel");
                var buttons = panel.Find("DetailsView");

                var filterButtons = Util.KInstantiateUI(buttons.gameObject);
                var trans = filterButtons.transform;
                trans.SetParent(panel, false);
                trans.SetSiblingIndex(1);

                filterButtons.name = "FilterButtons";
                var LE = filterButtons.GetComponent<LayoutElement>();
                LE.preferredHeight = 30;

                HorizontalLayoutGroup HLG = filterButtons.GetComponent<HorizontalLayoutGroup>();

                UnityEngine.Object.Destroy(filterButtons.transform.Find("ToggleAllButton").gameObject);
                UnityEngine.Object.Destroy(filterButtons.transform.Find("WorkshopButton").gameObject);

                if (Config.Instance.ButtonStyle == Config.FilterbuttonStyle.Button)
                    filterButtons.AddOrGet<FilterButtons>().Init(() => __instance.RebuildDisplay(null));
                else if (Config.Instance.ButtonStyle == Config.FilterbuttonStyle.Checkbox)
                    filterButtons.AddOrGet<FilterToggleButtons>().Init(() => __instance.RebuildDisplay(null));



                HLG.padding = new RectOffset(2, 2, 2, 2);
                HLG.spacing = 3;

                filterButtons.SetActive(true);
            }
        }
        //[HarmonyPatch(typeof(UnityEngine.UI.Dropdown), nameof(Dropdown.Show))]
        //public static class Dropdown_Show_Patch
        //{
        //    public static string TargetName = "ModProfileManager_Dropdown";

        //    public static void Postfix(Dropdown __instance)
        //    {
        //        SgtLogger.l("DROPDOWN PATCH 1");
        //        if (__instance.gameObject.name != TargetName)
        //            return;

        //        SgtLogger.l("DROPDOWN PATCH 2");
        //        //if (!__instance.IsActive() || !__instance.IsInteractable() || __instance.m_Dropdown != null)
        //        //{
        //        //    return;
        //        //}

        //        if (__instance.m_Items != null && __instance.m_Items.Count> 0)
        //        {
        //            foreach( var item in __instance.m_Items)
        //            {
        //                item.toggle.isOn = true;
        //            }
        //        }
        //        SgtLogger.l("DROPDOWN PATCH 3");
        //    }
        //}

        [HarmonyPatch(typeof(ModsScreen), "OnDeactivate")]
        public static class ModsScreen_OnDeactivate_Patch
        {
            public static void Prefix()
            {
                _modsScreen = null;
                _filterManager = null;
            }
        }
    }
}
