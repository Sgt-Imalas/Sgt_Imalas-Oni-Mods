
// This code took inspiration from https://github.com/peterhaneve/ONIMods

using BlueprintsV2.BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.ModAPI;
using HarmonyLib;
using ModFramework;
using PeterHan.PLib.Actions;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using PeterHan.PLib.PatchManager;
using Rendering;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Blueprints {
    public class Integration {

        public static PAction BlueprintsCreateAction { get; set; }
        public static PAction BlueprintsUseAction { get; set; }
        public static PAction BlueprintsCreateFolderAction { get; set; }
        public static PAction BlueprintsRenameAction { get; set; }
        public static PAction BlueprintsCycleFoldersNextAction { get; set; }
        public static PAction BlueprintsCycleFoldersPrevAction { get; set; }
        public static PAction BlueprintsCycleBlueprintsNextAction { get; set; }
        public static PAction BlueprintsCycleBlueprintsPrevAction { get; set; }
        public static PAction BlueprintsSnapshotAction { get; set; }
        public static PAction BlueprintsDeleteAction { get; set; }

       // [PLibMethod(RunAt.BeforeDbInit)]
        //internal static void BeforeDbInit()
        //{
        //    //Добавляем необходимые спрайты в глобальную коллекцию спрайтов
        //    BlueprintsAssets.AddSpriteToCollection(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE);
        //    BlueprintsAssets.AddSpriteToCollection(BlueprintsAssets.BLUEPRINTS_USE_ICON_SPRITE);
        //    BlueprintsAssets.AddSpriteToCollection(BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE);
        //}


        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerControllerOnPrefabInitPatch {
            public static void Postfix(PlayerController __instance) {
                //Добавляем инструменты (не конопки): Создать, Использовать, Снимок.
                var interfaceTools = new List<InterfaceTool>(__instance.tools);
                var createBlueprintTool = new GameObject(nameof(CreateBlueprintTool), typeof(CreateBlueprintTool));
                createBlueprintTool.transform.SetParent(__instance.gameObject.transform);
                createBlueprintTool.gameObject.SetActive(true);
                createBlueprintTool.gameObject.SetActive(false);

                interfaceTools.Add(createBlueprintTool.GetComponent<InterfaceTool>());

                var useBlueprintTool = new GameObject(typeof(UseBlueprintTool).Name, typeof(UseBlueprintTool));
                useBlueprintTool.transform.SetParent(__instance.gameObject.transform);
                useBlueprintTool.gameObject.SetActive(true);
                useBlueprintTool.gameObject.SetActive(false);

                interfaceTools.Add(useBlueprintTool.GetComponent<InterfaceTool>());

                var snapshotTool = new GameObject(typeof(SnapshotTool).Name, typeof(SnapshotTool));
                snapshotTool.transform.SetParent(__instance.gameObject.transform);
                snapshotTool.gameObject.SetActive(true);
                snapshotTool.gameObject.SetActive(false);

                interfaceTools.Add(snapshotTool.GetComponent<InterfaceTool>());

                __instance.tools = interfaceTools.ToArray();

                //Настройки всего мода
                BlueprintsAssets.Options = POptions.ReadSettings<BlueprintsOptions>() ?? new BlueprintsOptions();
                //Настройки синхронизации слоев при использовании инструмента Создать
                CreateBlueprintTool.Instance.OverlaySynced = BlueprintsAssets.Options.CreateBlueprintToolSync;
                //Настройки синхронизации слоев при использовании инструмента Снимок
                SnapshotTool.Instance.OverlaySynced = BlueprintsAssets.Options.SnapshotToolSync;
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "OnPrefabInit")]
        public static class ToolMenuOnPrefabInit {
            public static void Postfix()
            {
                MultiToolParameterMenu.CreateInstance();
                ToolParameterMenu.ToggleState defaultSelection = BlueprintsAssets.Options.DefaultMenuSelections == DefaultSelections.All ? ToolParameterMenu.ToggleState.On : ToolParameterMenu.ToggleState.Off;

                SnapshotTool.Instance.DefaultParameters = 
                CreateBlueprintTool.Instance.DefaultParameters = new Dictionary<string, ToolParameterMenu.ToggleState> {
                    { ToolParameterMenu.FILTERLAYERS.WIRES, defaultSelection },
                    { ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, defaultSelection },
                    { ToolParameterMenu.FILTERLAYERS.GASCONDUIT, defaultSelection },
                    { ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, defaultSelection },
                    { ToolParameterMenu.FILTERLAYERS.BUILDINGS, defaultSelection },
                    { ToolParameterMenu.FILTERLAYERS.LOGIC, defaultSelection },
                    { ToolParameterMenu.FILTERLAYERS.BACKWALL, defaultSelection },
                    { ToolParameterMenu.FILTERLAYERS.DIGPLACER, defaultSelection},
                    { SolidTileFiltering.StoreNonSolidsOptionID, defaultSelection },
                };
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenuCreateBasicTools {
            public static void Prefix(ToolMenu __instance) {
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(
                        BlueprintsStrings.STRING_BLUEPRINTS_CREATE_NAME,
                        BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE.name,
                        BlueprintsCreateAction.GetKAction(),
                        nameof(CreateBlueprintTool),
                        string.Format(BlueprintsStrings.STRING_BLUEPRINTS_CREATE_TOOLTIP, "{Hotkey}"),
                        true
                    ));
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(
                        BlueprintsStrings.STRING_BLUEPRINTS_USE_NAME,
                        BlueprintsAssets.BLUEPRINTS_USE_ICON_SPRITE.name,
                        BlueprintsUseAction.GetKAction(),
                        typeof(UseBlueprintTool).Name,
                        string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_TOOLTIP, "{Hotkey}"),
                        true
                    ));
                __instance.basicTools.Add(ToolMenu.CreateToolCollection(
                        BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_NAME,
                        BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE.name,
                        BlueprintsSnapshotAction.GetKAction(),
                        typeof(SnapshotTool).Name,
                        string.Format(BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP, "{Hotkey}"),
                        false
                    ));

                Utilities.ReloadBlueprints(false);
            }
        }

        [HarmonyPatch(typeof(FileNameDialog), "OnSpawn")]
        public static class FileNameDialogOnSpawn {
            public static void Postfix(FileNameDialog __instance, TMP_InputField ___inputField) {
                if (__instance.name.StartsWith("BlueprintsMod_")) {
                    ___inputField.onValueChanged.RemoveAllListeners();
                    ___inputField.onEndEdit.RemoveAllListeners();

                    if (__instance.name.StartsWith("BlueprintsMod_FolderDialog_")) {
                        ___inputField.onValueChanged.AddListener(delegate (string text) {
                            for (int i = text.Length - 1; i >= 0; --i) {
                                if (i < text.Length && BlueprintsAssets.BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Contains(text[i])) {
                                    text = text.Remove(i, 1);
                                }
                            }

                            ___inputField.text = text;
                        });
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        public static class GameDestroyInstances {
            public static void Postfix() {
                CreateBlueprintTool.DestroyInstance();
                UseBlueprintTool.DestroyInstance();
                SnapshotTool.DestroyInstance();
                MultiToolParameterMenu.DestroyInstance();

                BlueprintsAssets.BLUEPRINTS_AUTOFILE_WATCHER.Dispose();
            }
        }

        [HarmonyPatch(typeof(BlockTileRenderer), "GetCellColour")]
        public static class BlockTileRendererGetCellColour {
            public static void Postfix(int cell, SimHashes element, ref Color __result) {
                if (__result != Color.red && element == SimHashes.Void && BlueprintsState.ColoredCells.ContainsKey(cell)) {
                    __result = BlueprintsState.ColoredCells[cell].Color;
                }
            }
        }
    }
}