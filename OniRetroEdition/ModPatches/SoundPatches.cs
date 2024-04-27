using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using PeterHan.PLib.Actions;
using PeterHan.PLib.Core;

namespace OniRetroEdition.ModPatches
{
    internal class SoundPatches
    {
        [HarmonyPatch(typeof(OverlayScreen), "RegisterModes")]
        public static class OverlayScreen_RegisterModes_Patch
        {
            public static void Postfix(OverlayScreen __instance)
            {
                __instance.RegisterMode(new OverlayModes.Sound());
            }
        }
        [HarmonyPatch(typeof(NoisePolluter), nameof(NoisePolluter.OnActiveChanged))]
        public static class NoisePolluter_RegisterModes_Patch
        {
            public static bool Prefix(NoisePolluter __instance, object data)
            {
                bool _isActive = false;
                if (data == null)
                {
                    return false;
                }
                if (data is bool b)
                {
                    _isActive = b;
                }
                else if (data is Operational operational
                    )
                {
                    _isActive = operational.IsActive;
                }
                else return false;


                bool isActive = _isActive;
                __instance.SetActive(isActive);
                __instance.Refresh();
                return false;
            }
        }
        /// <summary>
        /// Initialize spatial splats as soon as they are needed
        /// </summary>
        [HarmonyPatch(typeof(AudioEventManager), nameof(AudioEventManager.AddSplat))]
        public static class CrashDetect
        {
            public static void Prefix(AudioEventManager __instance)
            {
                if (__instance.spatialSplats.cells == null)
                {
                    SgtLogger.l("Resetting noise splat grid");

                    __instance.spatialSplats.Reset(Grid.WidthInCells, Grid.HeightInCells, 16, 16);
                }
            }
        }

        /// <summary>
        /// skip the method as it gets initialized in AddSplat prefix instead
        /// </summary>
        [HarmonyPatch(typeof(AudioEventManager), nameof(AudioEventManager.OnSpawn))]
        public static class CrashDetect2
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        /// <summary>
        /// Exclude items from the overlay that dont have a noise component to prevent crashes
        /// </summary>
        [HarmonyPatch(typeof(OverlayModes.Sound), nameof(OverlayModes.Sound.OnSaveLoadRootUnregistered))]

        public static class CrashDetect3
        {
            public static bool Prefix(SaveLoadRoot item)
            {
                return item != null && item.gameObject != null && item.TryGetComponent<NoisePolluter>(out _);
            }
        }
        
        /// <summary>
         /// Sound Overlay
         /// </summary>
        [HarmonyPatch(typeof(SimDebugView), "OnPrefabInit")]
        public static class SimDebugView_OnPrefabInit_Patch
        {
            public static void Postfix(Dictionary<HashedString, Func<SimDebugView, int, Color>> ___getColourFuncs)
            {
                //sound overlay
                ___getColourFuncs.Add(OverlayModes.Sound.ID, GetCellColor);

                //old light color
                GlobalAssets.Instance.colorSet.lightOverlay = UIUtils.rgb(255, 226, 141);

            }

            private static Color GetCellColor(SimDebugView instance, int cell)
            {
                var db = AudioEventManager.Get().GetDecibelsAtCell(cell);
                return Color.Lerp(SoundColors[0], SoundColors[1], Mathf.Clamp(db, 0, 200f) / 200f);
            }
        }
        public static Color32[] SoundColors = new Color32[2]
        {
            Color.black, //No Sound
            new Color(0.75f, 0, 0)  //Very Loud Sound
        }; 
        
        
        /// <summary>
        /// filter out regular elements in sound overlay, like decor overlay
        /// </summary>
        [HarmonyPatch(typeof(SelectToolHoverTextCard), nameof(SelectToolHoverTextCard.OnSpawn))]

        public static class AddFilterForSoundOverlay
        {
            public static void Postfix(SelectToolHoverTextCard __instance)
            {
                __instance.overlayFilterMap.Add(OverlayModes.Sound.ID, () => false);
            }
        }

        [HarmonyPatch(typeof(SelectToolHoverTextCard), nameof(SelectToolHoverTextCard.UpdateHoverElements))]
        public static class SelectToolHoverTextCard_UpdateHoverElements_Patch
        {
            public static bool Prefix(SelectToolHoverTextCard __instance, List<KSelectable> hoverObjects)
            {
                if (SimDebugView.Instance.GetMode()!= OverlayModes.Sound.ID)
                    return true;
                if (__instance.iconWarning == null)
                {
                    __instance.ConfigureHoverScreen();
                }

                int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
                if (OverlayScreen.Instance == null || !Grid.IsValidCell(num))
                {
                    return true;
                }

                HoverTextDrawer hoverTextDrawer = HoverTextScreen.Instance.BeginDrawing();
                __instance.overlayValidHoverObjects.Clear();
                foreach (KSelectable hoverObject in hoverObjects)
                {
                    if (__instance.ShouldShowSelectableInCurrentOverlay(hoverObject))
                    {
                        __instance.overlayValidHoverObjects.Add(hoverObject);
                    }
                }

                __instance.currentSelectedSelectableIndex = -1;
                if (SelectToolHoverTextCard.highlightedObjects.Count > 0)
                {
                    SelectToolHoverTextCard.highlightedObjects.Clear();
                }

                DrawerHelper(__instance, num, hoverTextDrawer);
                hoverTextDrawer.EndDrawing();
                return false;
            }


            private static readonly FieldInfo InfoId = AccessTools.Field(typeof(OverlayModes.Sound), nameof(OverlayModes.Sound.ID));

            private static readonly FieldInfo LogicId = AccessTools.Field(
                typeof(OverlayModes.Logic),
                nameof(OverlayModes.Logic.ID)
            );

            private static readonly MethodInfo HashEq = AccessTools.Method(
                typeof(HashedString),
                "op_Equality",
                new[] { typeof(HashedString), typeof(HashedString) }
            );

            private static readonly MethodInfo Helper = AccessTools.Method(
                typeof(SelectToolHoverTextCard_UpdateHoverElements_Patch),
                nameof(DrawerHelper)
            );

          //  public static IEnumerable<CodeInstruction> Transpiler(
          //      IEnumerable<CodeInstruction> orig,
          //      ILGenerator generator
          //  )
          //  {
          //      List<CodeInstruction> list = orig.ToList<CodeInstruction>();
          //      int index1 = list.FindIndex((Predicate<CodeInstruction>)(ci =>
          //      {
          //          FieldInfo operand = ci.operand as FieldInfo;
          //          return (object)operand != null && operand == SelectToolHoverTextCard_UpdateHoverElements_Patch.LogicId;
          //      }));
          //      System.Reflection.Emit.Label label = generator.DefineLabel();
          //      list[index1 + 2].operand = (object)label;
          //      int index2 = list.FindIndex(index1, (Predicate<CodeInstruction>)(ci => ci.opcode == OpCodes.Endfinally)) + 1;
          //      System.Reflection.Emit.Label operand1 = generator.DefineLabel();
          //      list[index2].labels.Add(operand1);
          //      int index3 = index2;
          //      int num1 = index3 + 1;
          //      list.Insert(index3, new CodeInstruction(OpCodes.Ldloc_2)
          //      {
          //          labels = {
          //  label
          //}
          //      });
          //      int index4 = num1;
          //      int num2 = index4 + 1;
          //      list.Insert(index4, new CodeInstruction(OpCodes.Ldsfld, (object)SelectToolHoverTextCard_UpdateHoverElements_Patch.InfoId));
          //      int index5 = num2;
          //      int num3 = index5 + 1;
          //      list.Insert(index5, new CodeInstruction(OpCodes.Call, (object)SelectToolHoverTextCard_UpdateHoverElements_Patch.HashEq));
          //      int index6 = num3;
          //      int num4 = index6 + 1;
          //      list.Insert(index6, new CodeInstruction(OpCodes.Brfalse, (object)operand1));
          //      int index7 = num4;
          //      int num5 = index7 + 1;
          //      list.Insert(index7, new CodeInstruction(OpCodes.Ldarg_0));
          //      int index8 = num5;
          //      int num6 = index8 + 1;
          //      list.Insert(index8, new CodeInstruction(OpCodes.Ldloc_0));
          //      int index9 = num6;
          //      int num7 = index9 + 1;
          //      list.Insert(index9, new CodeInstruction(OpCodes.Ldloc_1));
          //      int index10 = num7;
          //      int num8 = index10 + 1;
          //      list.Insert(index10, new CodeInstruction(OpCodes.Call, (object)SelectToolHoverTextCard_UpdateHoverElements_Patch.Helper));
          //      int index11 = num8;
          //      int num9 = index11 + 1;
          //      list.Insert(index11, new CodeInstruction(OpCodes.Br, (object)operand1));
          //      return (IEnumerable<CodeInstruction>)list;

          //  }

            private static void DrawerHelper(SelectToolHoverTextCard inst, int cell, HoverTextDrawer drawer)
            {
                if (AudioEventManager.Get() == null) { return; }

                // Cell position info
                drawer.BeginShadowBar();
                var db = AudioEventManager.Get().GetDecibelsAtCell(cell);
                drawer.DrawText(STRINGS.UI.RETRO_OVERLAY.SOUND.OVERLAYNAME, inst.Styles_Title.Standard);
                drawer.NewLine();
                drawer.DrawText(string.Format(STRINGS.UI.RETRO_OVERLAY.SOUND.TOOLTIP1, db), inst.Styles_BodyText.Standard);
                SelectToolHoverTextCard.highlightedObjects.Clear();
                if (db > 0)
                {
                    drawer.NewLine();
                    drawer.NewLine();
                    drawer.DrawText(STRINGS.UI.RETRO_OVERLAY.SOUND.TOOLTIP2, inst.Styles_BodyText.Standard);
                    foreach (AudioEventManager.PolluterDisplay source in AudioEventManager.Get().GetPollutersForCell(cell))
                    {
                        drawer.NewLine();
                        drawer.DrawText($" - {source.name}: {source.value} dB.", inst.Styles_BodyText.Standard);
                        SelectToolHoverTextCard.highlightedObjects.Add(source.provider.GetGameObject());
                    }
                }
                drawer.EndShadowBar();
            }
        }

        /// <summary>
        /// Applied to OverlayMenu to add a button for our overlay.
        /// </summary>
        [HarmonyPatch(typeof(OverlayMenu), "InitializeToggles")]
        public static class OverlayMenu_InitializeToggles_Patch
        {
            private const BindingFlags INSTANCE_ALL = PPatchTools.BASE_FLAGS | BindingFlags.
                Instance;
            private static readonly Type OVERLAY_TYPE = typeof(OverlayMenu).GetNestedType(
                "OverlayToggleInfo", INSTANCE_ALL);

            /// <summary>
            /// Applied after InitializeToggles runs.
            /// </summary>
            internal static void Postfix(ICollection<KIconToggleMenu.ToggleInfo> ___overlayToggleInfos)
            {
                var action =
                    //(OpenOverlay == null) ? 
                    PAction.MaxAction
                    //: OpenOverlay.GetKAction()
                    ;
                var info = CreateOverlayInfo("Sound Overlay", "overlay_sound", OverlayModes.Sound.ID, action,
                    global::STRINGS.UI.TOOLTIPS.NOISE_POLLUTION_OVERLAY_STRING);
                if (info != null)
                    ___overlayToggleInfos?.Add(info);
            }


            private static KIconToggleMenu.ToggleInfo CreateOverlayInfo(string text,
                    string icon_name, HashedString sim_view, Action openKey,
                    string tooltip)
            {
                const int KNOWN_PARAMS = 7;
                KIconToggleMenu.ToggleInfo info = null;
                ConstructorInfo[] cs;
                if (OVERLAY_TYPE == null || (cs = OVERLAY_TYPE.GetConstructors(INSTANCE_ALL)).
                        Length != 1)
                    PUtil.LogWarning("Unable to add TileOfInterest - missing constructor");
                else
                {
                    var cons = cs[0];
                    var toggleParams = cons.GetParameters();
                    int paramCount = toggleParams.Length;
                    // Manually plug in the knowns
                    if (paramCount < KNOWN_PARAMS)
                        PUtil.LogWarning("Unable to add TileOfInterest - parameters missing");
                    else
                    {
                        object[] args = new object[paramCount];
                        args[0] = text;
                        args[1] = icon_name;
                        args[2] = sim_view;
                        args[3] = "";
                        args[4] = openKey;
                        args[5] = tooltip;
                        args[6] = text;
                        // 3 and further (if existing) get new optional values
                        for (int i = KNOWN_PARAMS; i < paramCount; i++)
                        {
                            var op = toggleParams[i];
                            if (op.IsOptional)
                                args[i] = op.DefaultValue;
                            else
                            {
                                PUtil.LogWarning("Unable to add TileOfInterest - new parameters");
                                args[i] = null;
                            }
                        }
                        info = cons.Invoke(args) as KIconToggleMenu.ToggleInfo;
                    }
                }
                return info;
            }
        }

    }
}
