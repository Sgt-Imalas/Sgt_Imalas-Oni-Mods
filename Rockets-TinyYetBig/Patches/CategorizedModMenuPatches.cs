using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static UtilLibs.RocketryUtils;

namespace Rockets_TinyYetBig
{


    class CategorizedModMenuPatches
    {

        //[HarmonyPatch(typeof(AdditionalDetailsPanel))]
        //[HarmonyPatch("OnPrefabInit")]
        //public static class GibCOllaps
        //{ 
        //    public static void Postfix(AdditionalDetailsPanel __instance)
        //    {
        //        var detailsPanel = (GameObject)typeof(AdditionalDetailsPanel).GetField("detailsPanel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

        //        //Debug.Log("Detailspanel:");
        //        //UIUtils.ListAllChildren(detailsPanel.transform);
        //        //Debug.Log("LabelTemplate:");
        //        //UIUtils.ListAllChildren(__instance.attributesLabelTemplate.transform);
        //    }
        //}

        /// <summary>
        /// Is only bugged in creative, aka direct replacement, thus no longer enabled as "fix"
        /// </summary>
        //[HarmonyPatch(typeof(ReorderableBuilding))]
        //[HarmonyPatch("CanChangeModule")]
        //public static class DisableSwapButtonOnHabitats_Patch ///Should be base game, is bugged in base game
        //{
        //    public static void Postfix(ReorderableBuilding __instance,ref bool __result)
        //    {
        //        if (__result)
        //        {
        //            string moduleId = !((UnityEngine.Object)__instance.GetComponent<BuildingUnderConstruction>() != (UnityEngine.Object)null) ? __instance.GetComponent<Building>().Def.PrefabID : __instance.GetComponent<BuildingUnderConstruction>().Def.PrefabID;
        //            var Habitats = RocketModuleList.GetRocketModuleList()[(int)RocketCategory.habitats];
        //            if (Habitats.Contains(moduleId))
        //            {
        //                __result = false;
        //            }
        //        }

        //        //Debug.Log("Detailspanel:");
        //        //UIUtils.ListAllChildren(detailsPanel.transform);
        //        //Debug.Log("LabelTemplate:");
        //        //UIUtils.ListAllChildren(__instance.attributesLabelTemplate.transform);
        //    }
        //}

        //[HarmonyPatch(typeof(SelectModuleSideScreen))]
        //[HarmonyPatch("UpdateBuildableStates")]
        //public static class HideEmptyCategories
        //{
        //    public static void Postfix(SelectModuleSideScreen __instance)
        //    {
        //        if (Config.Instance.EnableBuildingCategories)
        //        {
                    
        //        }
        //    }
        //}


        /// <summary>
        /// add Categories to Rocket modules
        /// </summary>
        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch(nameof(SelectModuleSideScreen.SpawnButtons))]
        public static class CategoryPatchTest
        {
            public static void ToggleCategory(int category, SelectModuleSideScreen reference, bool initializing = false)
            {
                string categoryName = category.ToString();


                if (initializing)
                {
                    foreach (var cat in reference.categories)
                        cat.transform.Find("Grid").gameObject.SetActive(false);
                    return;
                }

                var ReferencedCategory = reference.categories.Find(go => go.name == categoryName);

                bool wasActive = ReferencedCategory.transform.Find("Grid").gameObject.activeSelf;

                if (wasActive)
                {
                    ReferencedCategory.transform.Find("Grid").gameObject.SetActive(false);
                }
                else
                {
                    foreach (var cat in reference.categories)
                    {
                        if (cat == ReferencedCategory)
                            cat.transform.Find("Grid").gameObject.SetActive(true);
                        else
                            cat.transform.Find("Grid").gameObject.SetActive(false);
                    }
#if DEBUG
                    //Debug.Log(ReferencedCategory.name + " <-category to activate");
#endif
                }
            }
            private static void ClearButtons(SelectModuleSideScreen _this)
            {
                foreach (var button in ModAssets.CategorizedButtons)
                {
                    if (!button.IsNullOrDestroyed())
                        Util.KDestroyGameObject(button.Value);
                }
                for (int index = _this.categories.Count - 1; index >= 0; --index)
                {

                    if (!_this.categories[index].IsNullOrDestroyed())
                        Util.KDestroyGameObject(_this.categories[index]);
                }

                _this.categories.Clear();
                ModAssets.CategorizedButtons.Clear();
            }

            public static bool Prefix(SelectModuleSideScreen __instance)
            {
                if (Config.Instance.EnableBuildingCategories)
                {
                    ClearButtons(__instance);
                    foreach (var category in RocketModuleList.GetRocketModuleList())
                    {

#if DEBUG
                        //Debug.Log("{" + (RocketCategory)category.Key + "}");

                        //foreach (var module in category.Value)
                        //{
                        //    Debug.Log("Module In List: " + module);
                        //}
#endif



                        if (
                            //category.Key != (int)RocketCategory.uncategorized && 
                            category.Value.Count > 0)
                        {
                            GameObject categoryGO = Util.KInstantiateUI(__instance.categoryPrefab, __instance.categoryContent, true);
                            categoryGO.name = category.Key.ToString();
                            HierarchyReferences component = categoryGO.GetComponent<HierarchyReferences>();
                            //categoryGO.name = ((RocketCategory)category.Key).ToString().ToUpperInvariant();

                            __instance.categories.Add(categoryGO);

                            //header.key = ((RocketCategory)category.Key).ToString();
                            //header.enabled = true;
                            var headergo = categoryGO.transform.Find("Header").gameObject;
                            headergo.SetActive(true);
                            //headergo.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 100);

                            var buttonPrefab = __instance.transform.Find("Button").gameObject;



                            var CategoryText = headergo.transform.Find("Label").GetComponent<LocText>();
                            CategoryText.text = ((RocketCategory)category.Key).ToString().ToUpperInvariant();

                            var foldButtonGO = Util.KInstantiateUI(buttonPrefab, headergo.gameObject, true);

                            var tooltip = UIUtils.AddSimpleTooltipToObject(foldButtonGO.transform, Mod.Tooltips[category.Key]);
                            tooltip.toolTipPosition = ToolTip.TooltipPosition.Custom;
                            tooltip.parentPositionAnchor = new Vector2(0.0f, 0.5f);
                            tooltip.tooltipPivot = new Vector2(1f, 1f);
                            tooltip.tooltipPositionOffset = new Vector2(-24f, 20f);

                            var foldButton = foldButtonGO.GetComponent<KButton>();

                            var rect = headergo.rectTransform().rect;

                            foldButtonGO.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rect.width);
                            foldButtonGO.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rect.height);

                            foldButton.ClearOnClick();
                            foldButton.onClick += () =>
                            {
                                ToggleCategory(category.Key, __instance);
                            };
                            foldButton.isInteractable = true;
                            var buttonText = foldButtonGO.transform.Find("Label").GetComponent<LocText>();
                            buttonText.text = ((RocketCategory)category.Key).ToString().ToUpperInvariant();

                            headergo.transform.Find("BG").gameObject.SetActive(false);
                            CategoryText.gameObject.SetActive(false);

                            Transform reference = component.GetReference<Transform>("content");
                            List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<RocketModuleCluster>();
                            foreach (string RocketModuleID in category.Value)
                            {
                                GameObject part = prefabsWithComponent.Find((Predicate<GameObject>)(p => p.PrefabID().Name == RocketModuleID));
                                if ((UnityEngine.Object)part == (UnityEngine.Object)null)
                                {
                                    Debug.LogWarning((object)("Found an id [" + RocketModuleID + "] in moduleButtonSortOrder in SelectModuleSideScreen.cs that doesn't have a corresponding rocket part!"));
                                }
                                else
                                {
                                    GameObject ModuleButton = Util.KInstantiateUI(__instance.moduleButtonPrefab, reference.gameObject, true);
                                    ModuleButton.GetComponentsInChildren<Image>()[1].sprite = Def.GetUISprite((object)part).first;
                                    LocText componentInChildren = ModuleButton.GetComponentInChildren<LocText>();
                                    componentInChildren.text = part.GetProperName();
                                    componentInChildren.alignment = TextAlignmentOptions.Bottom;
                                    componentInChildren.enableWordWrapping = true;
                                    ModuleButton.GetComponent<MultiToggle>().onClick += (System.Action)(() => __instance.SelectModule(part.GetComponent<Building>().Def));
                                    ModAssets.CategorizedButtons.Add(new Tuple<BuildingDef, int>(part.GetComponent<Building>().Def, category.Key), ModuleButton);
                                    //if (Config.Instance.HideRocketCategoryTooltips == false)
                                    //{
                                    //    string Tooltip = part.GetComponent<Building>().GetProperName() + "\n" + part.GetComponent<Building>().Def.Effect;
                                    //    var tt = UIUtils.AddSimpleTooltipToObject(ModuleButton.transform, Tooltip);

                                    //    tt.toolTipPosition = ToolTip.TooltipPosition.Custom;
                                    //    tt.parentPositionAnchor = new Vector2(0.0f, 0.5f);
                                    //    tt.tooltipPivot = new Vector2(1f, 1f);
                                    //    tt.tooltipPositionOffset = new Vector2(-24f, 20f);
                                    //}

                                    var TooltipMethod = typeof(SelectModuleSideScreen).GetMethod("SetupBuildingTooltip", BindingFlags.NonPublic | BindingFlags.Instance);

                                    TooltipMethod.Invoke(__instance, new System.Object[] { ModuleButton.GetComponent<ToolTip>(), part.GetComponent<Building>().Def });
                                    BuildingDef selectedModuleReflec = (BuildingDef)typeof(SelectModuleSideScreen).GetField("selectedModuleDef", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                                    if (selectedModuleReflec != (UnityEngine.Object)null)
                                        __instance.SelectModule(selectedModuleReflec);
                                }
                            }
#if DEBUG
                            //Debug.Log("Category:");
                            //UIUtils.ListAllChildren(categoryGO.transform);

#endif
                        }
                    }
                    __instance.UpdateBuildableStates();
                    ToggleCategory(0, __instance, true);

                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch(nameof(SelectModuleSideScreen.SetButtonColors))]
        public static class ButtonColorPatch
        {

            public static bool Prefix(SelectModuleSideScreen __instance, ref Dictionary<BuildingDef, bool> ___moduleBuildableState, BuildingDef ___selectedModuleDef)
            {
                if (Config.Instance.EnableBuildingCategories)
                {
                    foreach (var button in ModAssets.CategorizedButtons)
                    {
                        if (button.Value.TryGetComponent<MultiToggle>(out var component1) && button.Value.TryGetComponent<HierarchyReferences>(out var component2))
                        {
                            if (!___moduleBuildableState[button.Key.first])
                            {
                                component2.GetReference<Image>("FG").material = PlanScreen.Instance.desaturatedUIMaterial;
                                if (button.Key.first == ___selectedModuleDef)
                                    component1.ChangeState(1);
                                else
                                    component1.ChangeState(0);
                            }
                            else
                            {
                                component2.GetReference<Image>("FG").material = PlanScreen.Instance.defaultUIMaterial;
                                if (button.Key.first == ___selectedModuleDef)
                                    component1.ChangeState(3);
                                else
                                    component1.ChangeState(2);
                            }
                        }
                    }
                    __instance.UpdateBuildButton();
                    return false;
                }
                return true;

            }
        }

        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch(nameof(SelectModuleSideScreen.UpdateBuildableStates))]
        public static class BuildableStatesCategoryPatch
        {
            public static bool Prefix(SelectModuleSideScreen __instance, ref Dictionary<BuildingDef, bool> ___moduleBuildableState, BuildingDef ___selectedModuleDef)
            {
                if (Config.Instance.EnableBuildingCategories)
                {
                    foreach (var button in ModAssets.CategorizedButtons)
                    {
                        if (!___moduleBuildableState.ContainsKey(button.Key.first))
                        {
                            ___moduleBuildableState.Add(button.Key.first, false);
                        }
                        TechItem techItem = Db.Get().TechItems.TryGet(button.Key.first.PrefabID);
                        if (techItem != null)
                        {
                            bool flag = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem.IsComplete();

                            if (!button.Value.IsNullOrDestroyed())
                                button.Value.SetActive(flag);
                        }
                        else
                        {
                            if (!button.Value.IsNullOrDestroyed())
                                button.Value.SetActive(true);
                        }
                        ___moduleBuildableState[button.Key.first] = __instance.TestBuildable(button.Key.first);

                    }
                    if (___selectedModuleDef != null)
                    {
                        __instance.ConfigureMaterialSelector();
                    }
                    __instance.SetButtonColors();

                    foreach (var category in RocketModuleList.GetRocketModuleList())
                    {
                        bool keepCategory = false;
                        foreach (var item in category.Value)
                        {
                            TechItem techItem = Db.Get().TechItems.TryGet(item);

                            if (techItem != null)
                            {
                                if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem.IsComplete())
                                {
                                    keepCategory = true;
                                    break;
                                }
                            }
                            else
                            {
                                keepCategory = true;
                                break;
                            }
                        }
                        var categoryToDisable = __instance.categories.Find(categ => categ.name == category.Key.ToString());
                        if (!categoryToDisable.IsNullOrDestroyed())
                        {
                            categoryToDisable.SetActive(keepCategory);
                        }
                    }
                    return false;
                }
                return true;
            }
        }

    }
}
