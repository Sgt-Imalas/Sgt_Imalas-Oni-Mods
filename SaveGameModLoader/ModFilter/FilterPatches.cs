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
                            _buttonPrefab = Util.KInstantiateUI(btn.gameObject);
                            var bgImage = _buttonPrefab.transform.Find("GameObject").GetComponent<Image>();
                            bgImage.sprite = Assets.GetSprite(SpritePatch.pinSymbol);
                            bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
                            bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);
                            UnityEngine.Object.Destroy(_buttonPrefab.GetComponent<ToolTip>());
                        }

                        UnityEngine.Object.Destroy(clone);
                        prefabGo.SetActive(true);

                        return;
                    }
                }

                // ERROR!
                Debug.Log("[ModProfileManager] Error creating search prefab!  The searchbar will not function!");
            }
        }

        [HarmonyPatch(typeof(ModsScreen), "ShouldDisplayMod")]
        public static class ModsScreen_ShouldDisplayMod_Patch
        {
            public static void Postfix(KMod.Mod mod, ref bool __result)
            {
                if (__result && ModlistManager.Instance.IsSyncing)
                {
                    __result = ModlistManager.Instance.ModIsNotInSync(mod);
                }
                if (__result && _filterManager != null)
                {
                    var text = _filterManager.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        __result = CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                                       mod.label.title,
                                       text,
                                       CompareOptions.IgnoreCase
                                   ) >= 0;
                    }
                }
            }
        }

        //[HarmonyPatch(typeof(ModsScreen), "OnActivate")]

        public static GameObject _buttonPrefab;
        public static GameObject _prefab;
        public static FilterManager _filterManager;
        public static ModsScreen _modsScreen;
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

                UIUtils.ListAllChildren(__instance.transform);

                var filterButtons = Util.KInstantiateUI(buttons.gameObject);
                filterButtons.name = "FilterButtons";
                UIUtils.ListAllChildrenWithComponents(filterButtons.transform);
                var LE = filterButtons.GetComponent<LayoutElement>();
                LE.preferredHeight = 30;

                UtilMethods.ListAllPropertyValues(LE);
                HorizontalLayoutGroup HLG = filterButtons.GetComponent<HorizontalLayoutGroup>();
                UtilMethods.ListAllPropertyValues(HLG);

                UnityEngine.Object.Destroy(filterButtons.transform.Find("ToggleAllButton").gameObject);
                UnityEngine.Object.Destroy(filterButtons.transform.Find("WorkshopButton").gameObject);
                var buttonPrefab = filterButtons.transform.Find("CloseButton").gameObject;
                buttonPrefab.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);

                UIUtils.TryChangeText(buttonPrefab.transform, "Text", "Hide Incompatible");





                HLG.padding = new RectOffset(2,2,2,2);
                HLG.spacing = 3;
                var trans = filterButtons.transform;
                trans.SetParent(panel, false);
                trans.SetSiblingIndex(1);
                filterButtons.SetActive(true);
            }
        }


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
