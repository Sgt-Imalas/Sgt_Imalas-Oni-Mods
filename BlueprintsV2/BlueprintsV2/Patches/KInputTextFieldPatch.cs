using HarmonyLib;
using UnityEngine;

namespace BlueprintsV2.Patches
{
    [HarmonyPatch(typeof(KInputTextField))]
    [HarmonyPatch("LateUpdate")]
    public static class KInputTextFieldPatch
    {
        public static void Postfix()
        {
            // 在 LateUpdate 执行完毕后强制全局 IME 开启
            UnityEngine.Input.imeCompositionMode = UnityEngine.IMECompositionMode.On;
        }
    }
}