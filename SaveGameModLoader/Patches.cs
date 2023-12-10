using Database;
using HarmonyLib;
using Klei.AI;
using KSerialization;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static SaveGameModLoader.ModAssets;
using Ionic.Zlib;
using Klei;
using YamlDotNet;
using static TextureAtlas.AtlasData;
using KMod;
using static Database.MonumentPartResource;
using System.Text;
using UnityEngine.UI;
using PeterHan.PLib.Core;
using System.ComponentModel;
using SaveGameModLoader.Patches;
using PeterHan.PLib.UI;
using static UnityEngine.UI.CanvasScaler;
using static ModsScreen;
using SaveGameModLoader.FastTrack_VirtualScroll;
using SaveGameModLoader.ModsFilter;
using TMPro;
using SaveGameModLoader.ModFilter;
using System.Globalization;
using static MotdBox_ImageButtonLayoutElement;
using Steamworks;

namespace SaveGameModLoader
{
    class AllPatches
    {
        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public static class OnASsetPrefabPatch
        {
            public static void Postfix()
            {
                LoadModConfigPatch.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
            }
        }




        [HarmonyPatch(typeof(LoadScreen), "ShowColony")]
        public static class AddModSyncButtonLogic
        {
            public static void InsertModButtonCode(
                  RectTransform entry
                , object FileDetails
                )
            {

                var ContainerOpener = FileDetails.GetType().GetField("save").GetValue(FileDetails);
                string baseName = (string)ContainerOpener.GetType().GetField("BaseName").GetValue(ContainerOpener);
                string fileName = (string)ContainerOpener.GetType().GetField("FileName").GetValue(ContainerOpener);

                var btn = entry.Find("SyncButton").GetComponent<KButton>();

                if (btn != null)
                {
                    var colonyList = ModlistManager.Instance.TryGetColonyModlist(baseName);
                    var saveGameEntry = colonyList != null ? colonyList.TryGetModListEntry(fileName) : null;

                    btn.isInteractable = colonyList != null && saveGameEntry != null && App.GetCurrentSceneName() == "frontend";
                    btn.onClick += (() =>
                    {
                        ModlistManager.Instance.InstantiateModViewForPathOnly(fileName);
                    });
                }
            }
            public static readonly MethodInfo ButtonLogic = AccessTools.Method(
               typeof(AddModSyncButtonLogic),
               nameof(InsertModButtonCode));

            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(KButton),
                    nameof(KButton.ClearOnClick));

            private static readonly MethodInfo SaveGameFileIndexFinder = AccessTools.Method(
                    typeof(System.IO.Path),
                    nameof(System.IO.Path.GetFileNameWithoutExtension));

            //private static MethodInfo TransformIndexFinder =
            //    AccessTools.Method(
            //    typeof(UnityEngine.Object),
            //    nameof(UnityEngine.Object.Instantiate),
            //    new System.Type[] { typeof(UnityEngine.Object), typeof(UnityEngine.Transform)}); 


            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                ///
                var Methods = typeof(UnityEngine.Object).GetPublicStaticMethods();

                var GenericMethodInfo = Methods.FirstOrDefault(meth =>
                meth.Name.Contains("Instantiate")
                && meth.GetParameters().Length == 2
                && meth.GetParameters().Last().ParameterType == typeof(UnityEngine.Transform)
                && meth.GetParameters().First().ParameterType.IsGenericParameter
                ).MakeGenericMethod(typeof(RectTransform));


                //SgtLogger.l(GenericMethodInfo.Name + "::" + GenericMethodInfo, "postselect");
                ///


                var code = instructions.ToList();
                var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SuitableMethodInfo);


                var SaveGameFileIndexFinderStart = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == SaveGameFileIndexFinder);
                var saveFileRootIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, SaveGameFileIndexFinderStart);

                var TransformIndexFinderStart = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == GenericMethodInfo);// code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand.ToString().Contains("Instantiate"));


                var TransformIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, TransformIndexFinderStart, false);
                //SgtLogger.l(TransformIndex + "", "TRANSFORMINDEX");

                //foreach (var v in code) { SgtLogger.log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    insertionIndex += 1;
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, TransformIndex));//7
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, saveFileRootIndex));//6
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ButtonLogic));

                    //TranspilerHelper.PrintInstructions(code,true);
                }
                //foreach (var v in code) { SgtLogger.log(v.opcode + " -> " + v.operand); };

                return code;
            }

            public struct SaveGameFileDetails
            {
                public string BaseName;
                public string FileName;
                public string UniqueID;
                public System.DateTime FileDate;
                public SaveGame.Header FileHeader;
                public SaveGame.GameInfo FileInfo;
                public long Size;
            }
        }



        //[HarmonyPatch(typeof(ModsScreen), nameof(ModsScreen.BuildDisplay))]
        //[HarmonyPriority(Priority.HigherThanNormal)]
        public static class ModsScreen_BuildDisplay_Patch_Pin_Button
        {
            public static void ExecutePatch(Harmony harmony)
            {
                var m_TargetMethod = AccessTools.Method("ModsScreen, Assembly-CSharp:BuildDisplay");
                var m_Postfix = AccessTools.Method(typeof(ModsScreen_BuildDisplay_Patch_Pin_Button), "Postfix");
                harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix, Priority.LowerThanNormal), null);
            }


            static ColorStyleSetting blue = null;
            public static Dictionary<RectTransform, int> OriginalOrder = new Dictionary<RectTransform, int>();
            /// <summary>
            /// Applied after BuildDisplay runs.
            /// </summary>
            /// 



            internal static void Postfix(ModsScreen __instance, KButton ___closeButton, List<DisplayedMod> ___displayedMods)
            {
                var allMods = Global.Instance.modManager.mods;
                OriginalOrder.Clear();
                foreach (DisplayedMod displayedMod in ___displayedMods)
                {
                    if (blue == null)
                    {

                        displayedMod.rect_transform.Find("ManageButton").TryGetComponent<KImage>(out var mngButtonImage);
                        var defaultStyle = mngButtonImage.colorStyleSetting;
                        blue = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
                        blue.inactiveColor = UIUtils.HSVShift(defaultStyle.inactiveColor, 70f);
                        blue.activeColor = UIUtils.HSVShift(defaultStyle.activeColor, 70f);
                        blue.disabledColor = UIUtils.HSVShift(defaultStyle.disabledColor, 70f);
                        blue.hoverColor = UIUtils.HSVShift(defaultStyle.hoverColor, 70f);
                    }
                    var mod = allMods[displayedMod.mod_index];
                    var go = displayedMod.rect_transform.gameObject;
                    var transf = go.transform;

                    if (mod.IsLocal)
                    {
                        displayedMod.rect_transform.Find("ManageButton").TryGetComponent<KImage>(out var mngButtonImage);
                        mngButtonImage.colorStyleSetting = blue;
                        mngButtonImage.ApplyColorStyleSetting();
                    }

                    var btn = Util.KInstantiateUI(FilterPatches._buttonPrefab, go, true);
                    var tr = btn.transform;
                    tr.SetSiblingIndex(2);
                    tr.Find("GameObject").TryGetComponent<Image>(out var img);
                    transf.Find("BG").TryGetComponent<Image>(out var bgImg);
                    HandleListEntry(displayedMod, mod, btn, img, bgImg);

                    if (btn.TryGetComponent<KButton>(out var button))
                    {
                        button.ClearOnClick();
                        button.onClick += () =>
                        {
                            MPM_Config.Instance.TogglePinnedMod(mod.label.defaultStaticID);
                            __instance.RebuildDisplay("pinned mod changed");
                        };
                    }
                }
                if (FilterButtons.Instance != null)
                {
                    FilterButtons.Instance.RefreshUIState(false);
                    FilterButtons.Instance.ReorderVisualModState(___displayedMods, allMods);
                }

            }
            static async Task DoWithDelay(System.Action task, int ms)
            {
                await Task.Delay(ms);
                task.Invoke();
            }
            static Color normal = UIUtils.rgb(62, 67, 87), pinnedBg = UIUtils.Darken(normal, 15), incompatibleBg = UIUtils.rgb(26, 28, 33), pinnedActive = UIUtils.Lighten(Color.red, 50), pinnedInactive = Color.grey;
            static void HandleListEntry(DisplayedMod mod, KMod.Mod modData, GameObject btn, Image img, Image BgImg)
            {
                bool isPinned = MPM_Config.Instance.ModPinned(modData.label.defaultStaticID);
                if (modData.contentCompatability == ModContentCompatability.OK)
                    BgImg.color = isPinned ? pinnedBg : normal;
                img.color = isPinned ? pinnedActive : pinnedInactive;

                if (isPinned)
                {                  
                    mod.rect_transform.SetAsFirstSibling();                    
                }
            }
        }




        [HarmonyPatch(typeof(KMod.Manager), nameof(KMod.Manager.NotifyDialog))]
        public static class OnLoad_BetterModDifferenceScreen_Patch
        {
            public static bool Prefix(KMod.Manager __instance, string title, string message_format, GameObject parent)
            {
                if (title == global::STRINGS.UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.TITLE && __instance.events.Count > 0)
                {

                    var eventList = __instance.events.OrderBy(entry => entry.mod.title).Distinct().ToList();

                    ConfirmDialogScreen popUpGO =
                        ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas));
                    var newlyEnabled = new StringBuilder();
                    var newlyDisabled = new StringBuilder();


                    Event.GetUIStrings(EventType.ExpectedInactive, out var expectedInactive, out _);
                    Event.GetUIStrings(EventType.ExpectedActive, out var expectedActive, out _);

                    bool hadNewlyEnabled = false, hadNewlyDisabled = false;

                    //for(int i = 0; i < 99; ++i)
                    //{

                    //    newlyEnabled.AppendLine(" s "+i);
                    //}

                    newlyEnabled.AppendLine();
                    newlyDisabled.AppendLine();

                    newlyEnabled.AppendLine(UIUtils.ColorText("<b>" + expectedInactive + ":</b>", GlobalAssets.Instance.colorSet.logicOnSidescreen));
                    newlyDisabled.AppendLine(UIUtils.ColorText("<b>" + expectedActive + ":</b>", GlobalAssets.Instance.colorSet.logicOffSidescreen));

                    int changesOverLimit = 30;
                    int changesOverLimitExAc = 0;
                    int changesOverLimitExIn = 0;

                    foreach (Event @event in eventList)
                    {
                        if (@event.event_type == EventType.ExpectedInactive)
                        {
                            hadNewlyEnabled = true;
                            if (changesOverLimit >= 0)
                            {
                                changesOverLimit--;
                                newlyEnabled.AppendLine("• " + @event.mod.title);
                            }
                            else
                            {
                                changesOverLimitExIn++;
                            }
                        }
                        else if (@event.event_type == EventType.ExpectedActive)
                        {
                            hadNewlyDisabled = true;
                            if (changesOverLimit >= 0)
                            {
                                changesOverLimit--;
                                newlyDisabled.AppendLine("• " + @event.mod.title);
                            }
                            else
                            {
                                changesOverLimitExAc++;
                            }
                        }
                    }

                    __instance.events.Clear();

                    if (!hadNewlyDisabled && !hadNewlyEnabled)
                        return false;

                    string allMods =
                        (hadNewlyEnabled ? newlyEnabled.ToString() : string.Empty)
                        + (changesOverLimitExIn > 0 ? global::STRINGS.UI.FRONTEND.MOD_DIALOGS.ADDITIONAL_MOD_EVENTS.Replace("(", "(" + changesOverLimitExIn + " ").Replace("...", string.Empty) : string.Empty)
                        + (hadNewlyDisabled ? newlyDisabled.ToString() : string.Empty)
                        + (changesOverLimitExAc > 0 ? global::STRINGS.UI.FRONTEND.MOD_DIALOGS.ADDITIONAL_MOD_EVENTS.Replace("(", "(" + changesOverLimitExAc + " ").Replace("...", string.Empty) : string.Empty);
                    ;

                    string text = string.Format(global::STRINGS.UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.MESSAGE, allMods);


                    popUpGO.popupMessage.alignment = TMPro.TextAlignmentOptions.TopLeft;

                    //var ScrollContainer = Util.KInstantiateUI(new GameObject("ScrollContainer"), popUpGO.transform.Find("GameObject").gameObject, true);
                    //var ScrollRectt = Util.KInstantiateUI(new GameObject("ScrollRect"), ScrollContainer, true);
                    //var textGo = popUpGO.popupMessage.gameObject;

                    //var mask = ScrollContainer.AddOrGet<RectMask2D>();

                    //ScrollContainer.transform.SetSiblingIndex(2);
                    //var LE = ScrollContainer.AddOrGet<LayoutElement>();
                    //LE.minHeight = 200;
                    //LE.preferredHeight = 300;
                    //LE.minWidth = 300;


                    //var scroll = ScrollContainer.AddOrGet<ScrollRect>();
                    //scroll.content = ScrollRectt.transform.rectTransform();
                    //scroll.horizontal = false;
                    //scroll.scrollSensitivity = 60;
                    ////scroll.movementType = ScrollRect.MovementType.Clamped;
                    //scroll.movementType = ScrollRect.MovementType.Elastic;
                    //scroll.inertia = false;
                    ////scroll.viewport = scroll.rectTransform();


                    //var csf = ScrollRectt.AddOrGet<ContentSizeFitter>();
                    //csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    //csf.SetLayoutVertical();
                    //csf.enabled = true;
                    ////csf.rectTransform().anchorMin = new(0, 0);
                    ////csf.rectTransform().anchorMax = new(1, 1);

                    ////var LE2 = ScrollRectt.AddOrGet<LayoutElement>();
                    ////LE2.minWidth = 350;

                    //var LG = ScrollRectt.AddOrGet<VerticalLayoutGroup>();
                    //LG.childControlWidth = true;
                    //LG.childForceExpandWidth = true;
                    //LG.childAlignment = TextAnchor.UpperLeft;
                    //LG.padding = new RectOffset(3, 3, 3, 3);



                    //textGo.SetParent(ScrollRectt);
                    //textGo.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 350);


                    /////setting start pos
                    //ScrollRectt.transform.rectTransform().pivot = new Vector2(0.5f, 0.99f);



                    popUpGO.PopupConfirmDialog(text, null, null, null, null, global::STRINGS.UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.TITLE, null, null, null);


                    //SgtLogger.l("POPUPLISTING");
                    //UIUtils.ListAllChildren(popUpGO.gameObject.transform);
                    //SgtLogger.l("POPUPLISTING2");
                    //UIUtils.ListAllChildrenWithComponents(popUpGO.gameObject.transform);
                    //UtilMethods.ListAllPropertyValues(popUpGO.popupMessage);
                    return false;
                }
                return true;
            }
        }

        // [HarmonyPatch(typeof(Steam), nameof(Steam.MakeMod))]
        public static class Steam_MakeMod
        {
            public static void TryPatchingSteam(Harmony harmony)
            {
                var type = AccessTools.TypeByName("Steam");
                if (type == null)
                {
                    SgtLogger.warning("steam.makemod was null");
                    return;
                }
                SgtLogger.l("patching steam.makemod");
                var m_TargetMethod = AccessTools.Method(type, "MakeMod");
                //var m_Transpiler = AccessTools.Method(typeof(LoadModConfigPatch), "Transpiler");
                // var m_Prefix = AccessTools.Method(typeof(Steam_MakeMod), "Prefix");
                var m_Postfix = AccessTools.Method(typeof(Steam_MakeMod), "Postfix");

                harmony.Patch(m_TargetMethod,
                    null, //new HarmonyMethod(m_Prefix),
                    new HarmonyMethod(m_Postfix)
                    );
            }

            public static void Postfix(KMod.Mod __result)
            {
                if (__result == null || __result.staticID == null || __result.label.id == null)
                    return;

                __result.on_managed = () =>
                {
                    if (UseSteamOverlay && SteamUtils.IsOverlayEnabled())
                        SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/sharedfiles/filedetails/?id=" + __result.label.id);
                    else
                        App.OpenWebURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + __result.label.id);
                };
            }
        }
        [HarmonyPatch(typeof(KMod.Manager), nameof(KMod.Manager.Install))]
        public static class ModManager_Subscribe
        {
            public static void Postfix(KMod.Mod mod)
            {
                if (ulong.TryParse(mod.label.id, out var steamID))
                    SteamInfoQuery.FindMissingModsQuery(new List<ulong>() { steamID });
            }
        }


        [HarmonyPatch(typeof(ModsScreen), "OnDeactivate")]
        public static class ModsScreen_SyncModeOff
        {
            public static void Postfix()
            {
                ModlistManager.Instance.IsSyncing = false;
            }
        }


        [HarmonyPatch(typeof(ModsScreen), "OnActivate")]
        public static class ModsScreen_AddModListButton
        {
            public static void Postfix(ModsScreen __instance)
            {
                if(ModAssets.ModsFilterActive)
                {
                    var modsFilterGO = __instance.transform.Find("Panel/Search/LocTextInputField");
                    if(modsFilterGO != null)
                    {
                        modsFilterGO.gameObject.TryGetComponent(out FilterManager.ModFilterText);
                    }
                }


                __instance.workshopButton.ClearOnClick();
                __instance.workshopButton.onClick += () =>
                {

                    if (UseSteamOverlay && SteamUtils.IsOverlayEnabled())
                        SteamFriends.ActivateGameOverlayToWebPage("http://steamcommunity.com/workshop/browse/?appid=457140");
                    else
                        App.OpenWebURL("http://steamcommunity.com/workshop/browse/?appid=457140");
                };
                ///Add Modlist Button
                var workShopButton = __instance.transform.Find("Panel/DetailsView/WorkshopButton");
                var DetailsView = __instance.transform.Find("Panel/DetailsView").gameObject;



                var panel = __instance.transform.Find("Panel").rectTransform();

                var totalWindowHeight = __instance.rectTransform().rect.height;
                float goldenHeigh = totalWindowHeight / 1.309f;
                float paddingSize = (totalWindowHeight - goldenHeigh) / 2f;
                panel.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, paddingSize, goldenHeigh);


                if (__instance.gameObject.name == "SYNCSCREEN")
                    return;

                __instance.closeButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
                AddCopyButton(__instance.workshopButton.transform.parent.gameObject, () => ModAssets.PutCurrentToClipboard(false), () => PutCurrentToClipboard(true), __instance.workshopButton.bgImage.colorStyleSetting);


                var modlistButtonGO = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, DetailsView, true);
                modlistButtonGO.name = "ModListsButton";
                var modlistButtonText = modlistButtonGO.Find("Text").GetComponent<LocText>();
                modlistButtonText.text = STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSBUTTON;
                modlistButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSBUTTONINFO);
                var modlistButton = modlistButtonGO.GetComponent<KButton>();
                modlistButton.ClearOnClick();

#if DEBUG
                // UIUtils.ListAllChildren(__instance.transform);
#endif

                modlistButton.onClick += () =>
                {
                    ///Util.KInstantiateUI(ScreenPrefabs.Instance.RailModUploadMenu.gameObject, modScreen.gameObject, true); ///HMMM; great if modified for modpack creation
                    GameObject window = Util.KInstantiateUI(ScreenPrefabs.Instance.languageOptionsScreen.gameObject);
                    window.SetActive(false);
                    var copy = window.transform;
                    UnityEngine.Object.Destroy(window);
                    var newScreen = Util.KInstantiateUI(copy.gameObject, __instance.gameObject, true);
                    newScreen.name = "ModListView";
                    newScreen.AddComponent(typeof(ModListScreen));

                };
                var closeButton = __instance.transform.Find("Panel/DetailsView/CloseButton");
                //UIUtils.ListAllChildrenWithComponents(closeButton.transform);
                closeButton.SetAsLastSibling();
            }
        }

        [HarmonyPatch(typeof(LoadScreen), "OnActivate")]
        public static class AddModSyncButtonToLoadscreen
        {
            public static bool Prefix(LoadScreen __instance)
            {
                if (__instance.name == "NODONTDOTHAT") return false;

                ModlistManager.Instance.ParentObjectRef = __instance.transform.parent.gameObject;
                ModlistManager.Instance.GetAllStoredModlists();


                var ViewRootFinder = typeof(LoadScreen).GetField("colonyViewRoot", BindingFlags.NonPublic | BindingFlags.Instance);

                GameObject viewRoot = (GameObject)ViewRootFinder.GetValue(__instance);

                HierarchyReferences references = viewRoot.GetComponent<HierarchyReferences>();
                ///get ListEntryTemplate 
                RectTransform template = references.GetReference<RectTransform>("SaveTemplate");

                ///Get LoadButton and move it to the left 
                HierarchyReferences TemplateRefs = template.GetComponent<HierarchyReferences>();
                var SyncTemplate = TemplateRefs.GetReference<RectTransform>("LoadButton");
                SyncTemplate.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 65f, SyncTemplate.rect.width);
                ///Move Time&Date text to the left
                var date = TemplateRefs.GetReference<RectTransform>("DateText");
                date.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, SyncTemplate.rect.width + 85f, date.rect.width);


                ///Instantiate SyncButton
                RectTransform kbutton = Util.KInstantiateUI<RectTransform>(SyncTemplate.gameObject, template.gameObject, true);
                kbutton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 50);

                ///Add SyncButton to template and set Params
                kbutton.name = "SyncButton";
                var syncText = kbutton.GetComponentInChildren<LocText>(true);
                var btn = kbutton.GetComponentInChildren<KButton>(true);
                syncText.key = "STRINGS.UI.FRONTEND.MODSYNCING.SYNCMODSBUTTONBG";
                btn.bgImage.sprite = Assets.GetSprite("icon_thermal_conductivity");
                return true;
            }
        }

        /// <summary>
        /// Add a "Sync and Continue"-Button to the main menu prefab
        /// </summary>
        [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
        public static class MainMenu_OnPrefabInit_Patch
        {
            public static void Prefix(MainMenu __instance)
            {
                string path;
                if (KPlayerPrefs.HasKey("AutoResumeSaveFile"))
                {
                    path = KPlayerPrefs.GetString("AutoResumeSaveFile");
                }
                else
                    path = string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveForCurrentDLC() : GenericGameSettings.instance.performanceCapture.saveGame;
#if DEBUG
                //UIUtils.ListAllChildren(__instance.transform);
#endif
                if (path == null || path == string.Empty)
                {
                    return;
                }

                Transform parentBar;
                Transform contButton;

                if (DlcManager.IsExpansion1Active())
                {
                    parentBar = __instance.transform.Find("MainMenuMenubar/MainMenuButtons");
                    contButton = __instance.transform.Find("MainMenuMenubar/MainMenuButtons/Button_ResumeGame");
                }
                else
                {
                    parentBar = __instance.transform.Find("UI Group/TopRight/MainMenuButtons");
                    contButton = __instance.transform.Find("UI Group/TopRight/MainMenuButtons/Button_ResumeGame");
                }

                var bt = Util.KInstantiateUI(contButton.gameObject, parentBar.gameObject, true);

                string colonyName = SaveGameModList.GetModListFileName(path);

                var colony = ModlistManager.Instance.TryGetColonyModlist(colonyName);
                bool interactable = colony != null ? colony.TryGetModListEntry(path) != null : false;

                var button = bt.GetComponent<KButton>();
                bt.name = "SyncAndContinue";
                var internalText = bt.transform.Find("ResumeText").GetComponent<LocText>();

                internalText.text = STRINGS.UI.FRONTEND.MODSYNCING.CONTINUEANDSYNC;

                button.isInteractable = interactable;
                button.ClearOnClick();
                button.onClick +=
                () =>
                {
                    ModlistManager.Instance.InstantiateModViewForPathOnly(path);
                };

                ModlistManager.Instance.ParentObjectRef = __instance.gameObject;
                var SaveGameName = button.transform.Find("SaveNameText").gameObject;
                UnityEngine.Object.Destroy(SaveGameName);


                ///SteamAuthorInfoFetching:
                ///

                if (SteamManager.Initialized)
                {
                    var steamMods = Global.Instance.modManager.mods
                        .Where(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Steam)
                        .Select(mod => mod.label.id)
                        .ToList();
                    if(steamMods.Count> 0) 
                        SteamInfoQuery.InitModAuthorQuery(steamMods);
                }
            }
        }


        /// <summary>
        /// On loading a savegame, store the mod config in the modlist.
        /// </summary>
        //[HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.Load), new System.Type[] { typeof(IReader) })]
        public static class LoadModConfigPatch
        {
            //Manual patch required here, otherwise it will break translations of game settings as those get their assets initialized earlier than translation
            public static void AssetOnPrefabInitPostfix(Harmony harmony)
            {
                var m_TargetMethod = AccessTools.Method("SaveLoader, Assembly-CSharp:Load", new System.Type[] { typeof(IReader) });
                var m_Transpiler = AccessTools.Method(typeof(LoadModConfigPatch), "Transpiler");
                var m_Prefix = AccessTools.Method(typeof(LoadModConfigPatch), "Prefix");
                //var m_Postfix = AccessTools.Method(typeof(MainMenuSearchBarInit), "Postfix");

                harmony.Patch(m_TargetMethod,
                    new HarmonyMethod(m_Prefix),
                    null,//new HarmonyMethod(m_Postfix),
                    new HarmonyMethod(m_Transpiler)
                    );
            }



            internal class SaveFileRoot
            {
                public int WidthInCells;
                public int HeightInCells;
                public Dictionary<string, byte[]> streamed;
                public string clusterID;
                public List<ModInfo> requiredMods;
                public List<KMod.Label> active_mods;
                public SaveFileRoot() => this.streamed = new Dictionary<string, byte[]>();
            }
            public static SaveLoader instance;
            public static void Prefix(SaveLoader __instance)
            {
                instance = __instance;
            }
            public static void WriteModlistPatch(SaveFileRoot saveFileRoot)
            {
                var savedButNotEnabledMods = saveFileRoot.active_mods;
                savedButNotEnabledMods.Remove(savedButNotEnabledMods.Find(mod => mod.title == "SaveGameModLoader"));


                bool init = ModlistManager.Instance.CreateOrAddToModLists(
                    SaveLoader.GetActiveSaveFilePath(),
                    saveFileRoot.active_mods
                    );

                KPlayerPrefs.DeleteKey("AutoResumeSaveFile");
            }

            public static readonly MethodInfo ScreenCreator = AccessTools.Method(
               typeof(LoadModConfigPatch),
               nameof(WriteModlistPatch));


            public static readonly MethodInfo SaveFileRootCallLocation = AccessTools.Method(
               typeof(Deserializer),
               nameof(Deserializer.Deserialize),
                new System.Type[] { typeof(object) });

            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(KMod.Manager),
                    "MatchFootprint");

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SuitableMethodInfo);
                var deserializerSearchIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SaveFileRootCallLocation);

                var saveFileRootIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, deserializerSearchIndex);

                //foreach (var v in code) { SgtLogger.log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1 && saveFileRootIndex != -1)
                {
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, saveFileRootIndex));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ScreenCreator));
                }

                return code;
            }
        }

        /// <summary>
        /// On Saving, store the mod config to the modlist
        /// </summary>
        [HarmonyPatch(typeof(SaveLoader))]
        [HarmonyPatch(nameof(SaveLoader.Save))]
        [HarmonyPatch(new System.Type[] { typeof(string), typeof(bool), typeof(bool) })]
        public static class SaveModConfigOnSave
        {
            public static void Postfix(string filename, bool isAutoSave = false, bool updateSavePointer = true)
            {
                //SgtLogger.log(filename + isAutoSave + updateSavePointer);
                KMod.Manager modManager = Global.Instance.modManager;

                var enabledModLabels = modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();

                bool init = ModlistManager.Instance.CreateOrAddToModLists(
                    filename,
                    enabledModLabels
                    );
            }
        }

        ///// <summary>
        ///// Since the Mod patches the load method, it will recieve blame on ANY load crash.
        ///// This Patch keeps it enabled on a crash, so you dont need to reenable it for syncing,
        ///// </summary>
        //[HarmonyPatch(typeof(KMod.Mod))]
        //[HarmonyPatch(nameof(KMod.Mod.SetCrashed))]
        //public static class DontDisableModOnCrash
        //{
        //    public static bool Prefix(KMod.Mod __instance)
        //    {
        //        if (__instance.label.title == ModAssets.ThisModName)
        //            return false;
        //        return true;
        //    }
        //}



    }
}
