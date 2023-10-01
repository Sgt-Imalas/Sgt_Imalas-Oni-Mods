using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static DebugButton.ModAssets;

namespace DebugButton
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(TopLeftControlScreen))]
        [HarmonyPatch(nameof(TopLeftControlScreen.OnActivate))]
        public static class Add_Debug_Button
        {
            static MultiToggle DebugInstaBuildButton = null;
            static ToolTip DebugInstaBuildButtonTooltip = null;
          
            public static void UpdateDebugToggleState()
            {
                //SgtLogger.l("instantbuildmodechanged");

                if (DebugInstaBuildButton != null)
                {
                    if (!DebugHandler.enabled)
                    {
                        DebugInstaBuildButton.ChangeState(0);
                        DebugInstaBuildButtonTooltip.SetSimpleTooltip(STRINGS.UI.TOOLS.DEBUG_INSTABUILD_TOGGLE.TOOLTIP_LOCKED);
                    }
                    else
                    {
                        DebugInstaBuildButtonTooltip.SetSimpleTooltip(GameUtil.ReplaceHotkeyString(STRINGS.UI.TOOLS.DEBUG_INSTABUILD_TOGGLE.TOOLTIP_TOGGLE, Action.DebugInstantBuildMode));
                        
                        if (DebugHandler.InstantBuildMode)
                        {
                            DebugInstaBuildButton.ChangeState(2);
                        }
                        else
                        {
                            DebugInstaBuildButton.ChangeState(1);
                        }
                    }

                    DebugInstaBuildButton.gameObject.SetActive(DebugHandler.enabled);
                }
            }

            public static void OnClickDebugToggle()
            {
                if (!DebugHandler.enabled)
                {
                    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
                }
                else
                {
                    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));

                    DebugHandler.InstantBuildMode = !DebugHandler.InstantBuildMode;
                    InterfaceTool.ToggleConfig(Action.DebugInstantBuildMode);
                    if (Game.Instance == null)
                        return;
                    Game.Instance.Trigger(1557339983, null);
                    if (PlanScreen.Instance != null)
                        PlanScreen.Instance.Refresh();
                    if (BuildMenu.Instance != null)
                        BuildMenu.Instance.Refresh();
                    if (OverlayMenu.Instance != null)
                        OverlayMenu.Instance.Refresh();
                    if (ConsumerManager.instance != null)
                        ConsumerManager.instance.RefreshDiscovered();
                    if (ManagementMenu.Instance != null)
                    {
                        ManagementMenu.Instance.CheckResearch(null);
                        ManagementMenu.Instance.CheckSkills();
                        ManagementMenu.Instance.CheckStarmap();
                    }
                    if (SelectTool.Instance.selected != null)
                        DetailsScreen.Instance.Refresh(SelectTool.Instance.selected.gameObject);
                    Game.Instance.Trigger(1594320620, "all_the_things");
                }
                UpdateDebugToggleState();
            }

            public static void Postfix(TopLeftControlScreen __instance)
            {
                if (DebugHandler.enabled)
                {
                    var debugButton = Util.KInstantiateUI(__instance.sandboxToggle.gameObject, __instance.sandboxToggle.transform.parent.gameObject, true).transform;
                    //UIUtils.ListAllChildrenWithComponents(debugButton);
                    debugButton.SetSiblingIndex(__instance.sandboxToggle.transform.GetSiblingIndex() + 1);
                    debugButton.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("icon_archetype_build");
                    debugButton.Find("Label").GetComponent<LocText>().text = STRINGS.UI.TOOLS.DEBUG_INSTABUILD_TOGGLE.NAME;
                    debugButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 120f);
                    debugButton.TryGetComponent<MultiToggle>(out DebugInstaBuildButton);
                    debugButton.TryGetComponent<ToolTip>(out DebugInstaBuildButtonTooltip);

                    DebugInstaBuildButton.onClick = (System.Action)Delegate.Combine(DebugInstaBuildButton.onClick, new System.Action(OnClickDebugToggle));
                    
                    Game.Instance.Subscribe((int)GameHashes.DebugInsantBuildModeChanged, delegate
                    {
                        UpdateDebugToggleState();
                    });
                    UpdateDebugToggleState();

                }
            }
        }

        [HarmonyPatch(typeof(TopLeftControlScreen))]
        [HarmonyPatch(nameof(TopLeftControlScreen.OnForcedCleanUp))]
        public static class RemoveListenerForDebugButton
        {
            public static void Postfix(TopLeftControlScreen __instance)
            {
                KInputManager.InputChange.RemoveListener(Add_Debug_Button.UpdateDebugToggleState);
            }
        }

        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        //public static string bugIconName = "no_bugs_instabuild_icon";

        //[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        //public class Assets_OnPrefabInit_Patch
        //{
        //    public static void Prefix(Assets __instance)
        //    {
        //        InjectionMethods.AddSpriteToAssets(__instance, bugIconName);
        //    }
        //}
    }
}
