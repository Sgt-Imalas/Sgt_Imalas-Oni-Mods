using HarmonyLib;
using UnityEngine;
using UtilLibs;  // 引用 DialogUtil

namespace BlueprintsV2.Patches
{
    [HarmonyPatch(typeof(KInputTextField))]
    [HarmonyPatch("LateUpdate")]
    public static class KInputTextFieldPatch
    {
        public static void Postfix()
        {
            // 根据对话框激活状态决定 IME 模式
            if (DialogUtil.IsInputDialogActive)
            {
                UnityEngine.Input.imeCompositionMode = UnityEngine.IMECompositionMode.On;
            }
            else
            {
                // 无对话框时恢复为 Auto（让系统自动处理）
                UnityEngine.Input.imeCompositionMode = UnityEngine.IMECompositionMode.Auto;
            }
        }
    }
}