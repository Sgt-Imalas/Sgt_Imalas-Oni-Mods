using Database;
using HarmonyLib;
using Klei.AI;
using MassMoveTo.Tools;
using MassMoveTo.Tools.SweepByType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static MassMoveTo.ModAssets;

namespace MassMoveTo
{
    internal class Patches
    {
        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public class PlayerController_OnPrefabInit_Patch
        {
            public static void Postfix(PlayerController __instance)
            {
                var interfaceTools = new List<InterfaceTool>(__instance.tools);

                var moveToSelectTool = new GameObject(nameof(FilteredMoveSelectTool), typeof(FilteredMoveSelectTool));
                moveToSelectTool.transform.SetParent(__instance.gameObject.transform);
                moveToSelectTool.gameObject.SetActive(true);
                moveToSelectTool.gameObject.SetActive(false);
                interfaceTools.Add(moveToSelectTool.GetComponent<FilteredMoveSelectTool>());

                var moveToTargetTool = new GameObject(typeof(TargetSelectTool).Name, typeof(TargetSelectTool));
                moveToTargetTool.transform.SetParent(__instance.gameObject.transform);
                moveToTargetTool.gameObject.SetActive(true);
                moveToTargetTool.gameObject.SetActive(false);
                interfaceTools.Add(moveToTargetTool.GetComponent<TargetSelectTool>());
                __instance.tools = interfaceTools.ToArray();
            }
        }

        /// <summary>
        /// add mass move tool to toolbox
        /// </summary>
        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenuCreateBasicTools
        {
            public static void Prefix(ToolMenu __instance)
            {
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(
                        STRINGS.UI.TOOLS.MOVETOSELECTTOOL.NAME,
                        ModAssets.MassMoveToolIcon.name,
                    Actions.MassMoveTool_Open.GetKAction(),
                        typeof(FilteredMoveSelectTool).Name,
                        string.Format(STRINGS.UI.TOOLS.MOVETOSELECTTOOL.TOOLTIP, "{Hotkey}"),
                        false
                    ));

            }
        }

        /// <summary>
        /// Add tool icon
        /// </summary>
        [HarmonyPatch(typeof(Assets), "OnPrefabInit")]
        public class Assets_OnPrefabInit_Patch
        {
            
            public static string MassMoveToolIconId = "MassMoveToolIcon";
            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Prefix(Assets __instance)
            {
                ModAssets.MassMoveToolIcon = InjectionMethods.AddSpriteToAssets(__instance, MassMoveToolIconId);
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
    }
}
