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
    class SelectModuleSideScreenPatches
    {
        [HarmonyPatch(typeof(SelectModuleSideScreen))]
        [HarmonyPatch(nameof(SelectModuleSideScreen.SpawnButtons))]
        public static class CategoryPatchTest
        {


            private static void ClearButtons(SelectModuleSideScreen _this)
            {
                foreach (KeyValuePair<BuildingDef, GameObject> button in _this.buttons)
                    Util.KDestroyGameObject(button.Value);
                for (int index = _this.categories.Count - 1; index >= 0; --index)
                    Util.KDestroyGameObject(_this.categories[index]);
                _this.categories.Clear();
                _this.buttons.Clear();
            }
            public static bool Prefix(SelectModuleSideScreen __instance)
            {
                ClearButtons(__instance);
                foreach (var category in RocketModuleList.GetRocketModuleList())
                {
                    if (
                        //category.Key != (int)RocketCategory.uncategorized && 
                        category.Value.Count>0){
                    GameObject categoryGO = Util.KInstantiateUI(__instance.categoryPrefab, __instance.categoryContent, true);
                    categoryGO.name = category.Key.ToString();
                        HierarchyReferences component = categoryGO.GetComponent<HierarchyReferences>();
#if DEBUG
                        Debug.Log("Category:");
                         UIUtils.ListAllChildren(categoryGO.transform);
                        Debug.Log("HR:");
                        UIUtils.ListChildrenHR(component);

#endif
                        __instance.categories.Add(categoryGO);
                        var header = component.GetReference<LocText>("label");
                        Debug.Log(header.key);
                        Debug.Log(header.text);
                        Debug.Log(header.textStyleSetting.fontSize + " ->"+ header.textStyleSetting.textColor);
                        header.text=("AAAAAAAAA");
                        Debug.Log(header.text);
                        var copy = header.textStyleSetting;
                        copy.textColor = Color.black;
                        header.textStyleSetting = copy;
                        //header.key = ((RocketCategory)category.Key).ToString();
                        //header.enabled = true;
                        var headergo = categoryGO.transform.Find("Header").gameObject;
                        headergo.SetActive(true);
                        headergo.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 100);
                        


                        var CategoryTextGO = headergo.transform.Find("Label").GetComponent<LocText>();
                        CategoryTextGO.text = ((RocketCategory)category.Key).ToString().ToUpperInvariant();

                        Transform reference = component.GetReference<Transform>("content");
                    List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<RocketModuleCluster>();
                        foreach (string str in category.Value)
                        {
                            string id = str;
                            GameObject part = prefabsWithComponent.Find((Predicate<GameObject>)(p => p.PrefabID().Name == id));
                            if ((UnityEngine.Object)part == (UnityEngine.Object)null)
                            {
                                Debug.LogWarning((object)("Found an id [" + id + "] in moduleButtonSortOrder in SelectModuleSideScreen.cs that doesn't have a corresponding rocket part!"));
                            }
                            else
                            {
                                GameObject gameObject2 = Util.KInstantiateUI(__instance.moduleButtonPrefab, reference.gameObject, true);
                                gameObject2.GetComponentsInChildren<Image>()[1].sprite = Def.GetUISprite((object)part).first;
                                LocText componentInChildren = gameObject2.GetComponentInChildren<LocText>();
                                componentInChildren.text = part.GetProperName();
                                componentInChildren.alignment = TextAlignmentOptions.Bottom;
                                componentInChildren.enableWordWrapping = true;
                                gameObject2.GetComponent<MultiToggle>().onClick += (System.Action)(() => __instance.SelectModule(part.GetComponent<Building>().Def));
                                __instance.buttons.Add(part.GetComponent<Building>().Def, gameObject2);

                                BuildingDef selectedModuleReflec = (BuildingDef)typeof(SelectModuleSideScreen).GetField("selectedModuleDef", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                                if (selectedModuleReflec != (UnityEngine.Object)null)
                                    __instance.SelectModule(selectedModuleReflec);
                            }
                        }
                    }
                }
                var updateMethod = typeof(SelectModuleSideScreen).GetMethod("UpdateBuildableStates", BindingFlags.NonPublic | BindingFlags.Instance);
                    updateMethod.Invoke(__instance, new[] { (System.Object)null });

                return false;
            }
        }
    }
}
