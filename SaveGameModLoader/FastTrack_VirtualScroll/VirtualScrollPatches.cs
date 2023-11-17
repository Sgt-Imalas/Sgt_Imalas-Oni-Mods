/*
 * Copyright 2023 Peter Han
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


using HarmonyLib;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace SaveGameModLoader.FastTrack_VirtualScroll
{
    /// <summary>
    /// Applied to DragMe to set it as always visible when dragged off screen.
    /// </summary>
    
    //patch manually only if fast track is not active
    //[HarmonyPatch(typeof(DragMe), nameof(DragMe.OnBeginDrag))]
    public static class DragMe_OnBeginDrag_Patch
    {
        public static void ExecutePatch(Harmony harmony)
        {
            var m_TargetMethod = AccessTools.Method("DragMe, Assembly-CSharp:OnBeginDrag");
            var m_Postfix = AccessTools.Method(typeof(DragMe_OnBeginDrag_Patch), "Postfix");
            harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix), null);
        }



        /// <summary>
        /// Applied after OnBeginDrag runs.
        /// </summary>
        internal static void Postfix(DragMe __instance)
        {
            GameObject go;
            if (__instance != null && (go = __instance.gameObject) != null)
            {
                var vs = go.GetComponentInParent<VirtualScroll>();
                if (vs != null)
                    vs.SetForceShow(go);
            }
        }
    }

    /// <summary>
    /// Applied to DragMe to clear it from always visible after dragging is complete.
    /// </summary>
    
    //[HarmonyPatch(typeof(DragMe), nameof(DragMe.OnEndDrag))]
    public static class DragMe_OnEndDrag_Patch
    {
        public static void ExecutePatch(Harmony harmony)
        {
            var m_TargetMethod = AccessTools.Method("DragMe, Assembly-CSharp:OnEndDrag");
            var m_Postfix = AccessTools.Method(typeof(DragMe_OnEndDrag_Patch), "Postfix");
            harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix), null);
        }

        /// <summary>
        /// Applied after OnEndDrag runs.
        /// </summary>
        internal static void Postfix(DragMe __instance)
        {
            GameObject go;
            if (__instance != null && (go = __instance.gameObject) != null)
            {
                var vs = go.GetComponentInParent<VirtualScroll>();
                if (vs != null)
                    vs.ClearForceShow(go);
            }
        }
    }

    /// <summary>
    /// Applied to ModsScreen to update the scroll pane whenever the list changes.
    /// </summary>
    
    //[HarmonyPatch(typeof(ModsScreen), nameof(ModsScreen.BuildDisplay))]
    public static class ModsScreen_BuildDisplay_Patch
    {
        public static void ExecutePatch(Harmony harmony)
        {
            var m_TargetMethod = AccessTools.Method("ModsScreen, Assembly-CSharp:BuildDisplay");
            var m_Prefix = AccessTools.Method(typeof(ModsScreen_BuildDisplay_Patch), "Prefix");
            var m_Postfix = AccessTools.Method(typeof(ModsScreen_BuildDisplay_Patch), "Postfix");
            harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix,Priority.High), new HarmonyMethod(m_Postfix, Priority.VeryLow), null);
        }


        /// <summary>
        /// Applied before BuildDisplay runs.
        /// </summary>
        [HarmonyPriority(Priority.High)]
        internal static void Prefix(ModsScreen __instance, ref VirtualScroll __state)
        {
            var entryList = __instance.entryParent;
            if (entryList != null && entryList.TryGetComponent(out VirtualScroll vs))
            {
                vs.OnBuild();
                __state = vs;
            }
            else
                __state = null;
        }

        /// <summary>
        /// Applied after BuildDisplay runs.
        /// </summary>
        [HarmonyPriority(Priority.VeryLow)]
        internal static void Postfix(VirtualScroll __state)
        {
            if (__state != null)
                __state.Rebuild();
        }
    }

    /// <summary>
    /// Applied to ModsScreen to set up listeners and state for virtual scroll.
    /// </summary>
    
    
    //[HarmonyPatch(typeof(ModsScreen), nameof(ModsScreen.OnActivate))]
    public static class ModsScreen_OnActivate_Patch
    {
        public static void ExecutePatch(Harmony harmony)
        {
            var m_TargetMethod = AccessTools.Method("ModsScreen, Assembly-CSharp:OnActivate");
            var m_Postfix = AccessTools.Method(typeof(ModsScreen_OnActivate_Patch), "Postfix");
            harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix), null);
        }

        /// <summary>
        /// Applied after OnActivate runs.
        /// </summary>
        internal static void Postfix(ModsScreen __instance)
        {
            var entryList = __instance.entryParent;
            GameObject go;
            if (entryList != null && (go = entryList.gameObject) != null)
            {
                var vs = go.AddOrGet<VirtualScroll>();
                vs.freezeLayout = true;
                vs.Initialize();
            }
        }
    }

}

