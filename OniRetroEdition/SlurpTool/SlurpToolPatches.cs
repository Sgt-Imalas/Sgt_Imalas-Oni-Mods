using HarmonyLib;
using PeterHan.PLib.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.SlurpTool
{
    public class SlurpToolPatches
    {       /// <summary>
            /// Applied to PlayerController to load the change settings tool into the available
            /// tool list.
            /// </summary>
        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerController_OnPrefabInit_Patch
        {
            /// <summary>
            /// Applied after OnPrefabInit runs.
            /// </summary>
            /// <param name="__instance">The current instance.</param>
            internal static void Postfix(PlayerController __instance)
            {
                // Create list so that new tool can be appended at the end
                var interfaceTools = new List<InterfaceTool>(__instance.tools);
                var bulkChangeTool = new GameObject(nameof(SlurpTool));
                var tool = bulkChangeTool.AddComponent<SlurpTool>();
                // Reparent tool to the player controller, then enable/disable to load it
                bulkChangeTool.transform.SetParent(__instance.gameObject.transform);
                bulkChangeTool.SetActive(true);
                bulkChangeTool.SetActive(false);
                SgtLogger.l("Created SlurpTool");
                // Add tool to tool list
                interfaceTools.Add(tool);
                __instance.tools = interfaceTools.ToArray();
            }
        }

        public static string ACTION_KEY = "ONIRETRO.ACTION.SLURPACTION";
        public static PAction SlurpAction { get; set; }
        /// <summary>
        /// Applied to ToolMenu to add the settings change tool to the tool list.
        /// </summary>
        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenu_CreateBasicTools_Patch
        {
            /// <summary>
            /// Applied after CreateBasicTools runs.
            /// </summary>
            /// <param name="__instance">The basic tool list.</param>
            internal static void Postfix(ToolMenu __instance)
            {
                


                SgtLogger.l("Adding SlurpTool to basic tools");
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.MISC.PLACERS.SLURPPLACER.TOOL_NAME
                    , "icon_action_mop", SlurpAction.GetKAction(), "SlurpTool", STRINGS.MISC.PLACERS.SLURPPLACER.SLURPBUTTON, largeIcon: false));

            }
        }
    }
}
