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

namespace SaveGameModLoader
{
    class AllPatches
    {

        //public class MainMenuModSelectionPatch
        //{
        //    public static void LoadMods(IReader reader, SaveGame.GameInfo GameInfo)
        //    {
        //        Debug.Assert(reader.ReadKleiString() == "world");

        //        Deserializer deserializer = new Deserializer(reader);
        //        deserializer.Deserialize((object)saveFileRoot);
        //        if ((GameInfo.saveMajorVersion == 7 || GameInfo.saveMinorVersion < 8) && saveFileRoot.requiredMods != null)
        //        {
        //            saveFileRoot.active_mods = new List<KMod.Label>();
        //            foreach (ModInfo requiredMod in saveFileRoot.requiredMods)
        //                saveFileRoot.active_mods.Add(new KMod.Label()
        //                {
        //                    id = requiredMod.assetID,
        //                    version = (long)requiredMod.lastModifiedTime,
        //                    distribution_platform = KMod.Label.DistributionPlatform.Steam,
        //                    title = requiredMod.description
        //                });
        //            saveFileRoot.requiredMods.Clear();
        //        }
        //        KMod.Manager modManager = Global.Instance.modManager;

        //        var enabledMods = modManager.mods.FindAll(mod => mod.IsActive() == true);
        //        var enabledModLabels = modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();

        //        Debug.Log(saveFileRoot.active_mods);
        //        Debug.Log("Enabled Mods before Changes:" + enabledMods.Count);
        //        Debug.Log(0);
        //        var enabledButNotSavedMods = enabledModLabels.Except(saveFileRoot.active_mods).ToList(); Debug.Log(1);
        //        var savedButNotEnabledMods = saveFileRoot.active_mods.Except(enabledModLabels).ToList(); Debug.Log(2);
        //        List<string> enabledIds = new();
        //        List<string> disabledIds = new();
        //        Debug.Log(3);
        //        if (enabledButNotSavedMods.Count > 0)
        //        {
        //            enabledButNotSavedMods.Remove(enabledButNotSavedMods.Find(mod => mod.title == "SaveGameModLoader"));
        //            enabledIds = enabledButNotSavedMods.Select(label => label.id).ToList();
        //        }

        //        Debug.Log(4);
        //        if (savedButNotEnabledMods.Count > 0)
        //        {
        //            savedButNotEnabledMods.Remove(savedButNotEnabledMods.Find(mod => mod.title == "SaveGameModLoader"));
        //            disabledIds = savedButNotEnabledMods.Select(label => label.id).ToList();
        //        }

        //        Debug.Log(5);

        //        foreach (var id in enabledIds)
        //        {
        //            var modToDisable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
        //            {
        //                if (modToDisable != null)
        //                {
        //                    modToDisable.SetEnabledForActiveDlc(false);
        //                    Debug.Log("enabled but not stored in SaveGame, disabling: " + id + " : " + modToDisable.title);
        //                }
        //                else
        //                {
        //                    Debug.LogWarning("Mod " + id + " : " + enabledButNotSavedMods.Find(m => m.id == id).title + " is not installed, how did this happen?");
        //                }
        //            }
        //            Debug.Log(6);
        //        }
        //        foreach (var id in disabledIds)
        //        {

        //            var modToEnable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
        //            if (modToEnable != null)
        //            {
        //                modToEnable.SetEnabledForActiveDlc(true);
        //                Debug.Log("stored in SaveGame but not enabled, enabling: " + id + " : " + modToEnable.title);
        //            }
        //            else
        //            {
        //                Debug.LogWarning("Mod " + id + " : " + modToEnable.title + " is stored in this SaveGame, but not installed!");
        //            }
        //            Debug.Log(7);

        //        }


        //        if (enabledButNotSavedMods.Count > 0 || savedButNotEnabledMods.Count > 0)
        //        {
        //            Global.Instance.modManager.Save();
        //            new System.Action(App.Quit).Invoke(); //App.Instance.Restart
        //            return;
        //        }

        //    }            
        //}

        /// <summary>
        /// ButtonInfoType copy since its private in MainMenu
        /// </summary>
        private struct ButtonInfo
        {
            public LocString text;
            public System.Action action;
            public int fontSize;
            public ColorStyleSetting style;

            public ButtonInfo(LocString text, System.Action action, int font_size, ColorStyleSetting style)
            {
                this.text = text;
                this.action = action;
                this.fontSize = font_size;
                this.style = style;
            }
        }

        /// <summary>
        /// Copy of Addbutton in main menu to add a button using the copied type above
        /// </summary>
        /// <param name="info">Button information</param>
        /// <param name="instance">Main Menu instance reference</param>
        /// <returns></returns>
        /// 

        private static KButton MakeButton(KButton buttonPrefab, GameObject buttonParent, ButtonInfo info)
        {
            KButton kbutton = Util.KInstantiateUI<KButton>(buttonPrefab.gameObject, buttonParent, true);
            kbutton.onClick += info.action;
            KImage component = kbutton.GetComponent<KImage>();
            component.colorStyleSetting = info.style;
            component.ApplyColorStyleSetting();
            LocText componentInChildren = kbutton.GetComponentInChildren<LocText>();
            componentInChildren.text = (string)info.text;
            componentInChildren.fontSize = (float)info.fontSize;
            return kbutton;
        }
        private static KButton MakeButton(ButtonInfo info, MainMenu instance)
        {
            KButton buttonPrefab = (KButton)Traverse.Create(instance).Field("buttonPrefab").GetValue();
            GameObject buttonParent = (GameObject)Traverse.Create(instance).Field("buttonParent").GetValue();
            return MakeButton(buttonPrefab, buttonParent, info);
        }


        //[HarmonyPatch(typeof(MainMenu), "ResumeGame")]
        //public static class AddModSyncToResumeButton
        //{
        //    //public static bool Prefix(MainMenu __instance)
        //    //{
        //    //    string path;
        //    //    if (KPlayerPrefs.HasKey("AutoResumeSaveFile"))
        //    //    {
        //    //        path = KPlayerPrefs.GetString("AutoResumeSaveFile");
        //    //    }
        //    //    else
        //    //        path = string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveForCurrentDLC() : GenericGameSettings.instance.performanceCapture.saveGame;
        //    //    if (string.IsNullOrEmpty(path))
        //    //        return true;
        //    //    else
        //    //    {
        //    //        Debug.Log("For now, Broke button as intended :D");
        //    //        Debug.Log(path);
        //    //        return false;
        //    //    }
        //    //}
        //}

        //[HarmonyDebug]
        [HarmonyPatch(typeof(LoadScreen), "ShowColony")]
        public static class AddModSyncButtonLogic
        {
            public static void InsertModButtonCode(RectTransform entry
                , object FileDetails
                )
            {

                var ContainerOpener = FileDetails.GetType().GetField("save").GetValue(FileDetails);
                string baseName = (string)ContainerOpener.GetType().GetField("BaseName").GetValue(ContainerOpener);
                string fileName = (string)ContainerOpener.GetType().GetField("FileName").GetValue(ContainerOpener);


                //Console.WriteLine("Properties of Type are:");
                //Debug.Log("NAME: " + baseName);
                //Debug.Log("FILE: " + fileName);

                var btn = entry.Find("SyncButton").GetComponent<KButton>();
                
                if (btn != null)
                {
                    var colonyList = ModlistManager.Instance.TryGetColonyModlist(baseName);
                    var saveGameEntry = colonyList != null ? colonyList.TryGetModListEntry(fileName) : null;

                    btn.isInteractable = colonyList !=null && saveGameEntry !=null && App.GetCurrentSceneName() == "frontend";
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

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SuitableMethodInfo);



                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    insertionIndex += 1;
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, 7));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, 6));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ButtonLogic));
                }
                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };

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

        [HarmonyPatch(typeof(ModsScreen), "Exit")]
        public static class SyncModeOff
        {
            public static void Postfix( )
            {
                ModlistManager.Instance.IsSyncing = false;
            }
        }
        [HarmonyPatch(typeof(ModsScreen), "ShouldDisplayMod")]
        public static class ModsScreen_ShouldDisplayMod_Patch
        {
            public static void Postfix(KMod.Mod mod, ref bool __result)
            {
                if (__result && ModlistManager.Instance.IsSyncing)
                {
                    __result = ModlistManager.Instance.ModIsNotInSync(mod);
                }
            }
        }
        [HarmonyPatch(typeof(LoadScreen), "OnPrefabInit")]
        public static class AddModSyncButtonToLoadscreen
        {
            public static void Prefix(LoadScreen __instance)
            {
                ModlistManager.Instance.ParentObjectRef = __instance.transform.parent.gameObject;
                ModlistManager.Instance.GetAllStoredModlists();

                GameObject viewRoot = (GameObject)Traverse.Create(__instance).Field("colonyViewRoot").GetValue();

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
                syncText.key = "STRINGS.UI.FRONTEND.MODSYNCING.SYNCMODS";
                btn.bgImage.sprite = Assets.GetSprite("icon_thermal_conductivity");
            }
        }

        /// <summary>
        /// Add a "Sync and Continue"-Button to the main menu prefab
        /// </summary>
        [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
        public static class AddSyncContinueButton
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

                ColorStyleSetting style = (ColorStyleSetting)Traverse.Create(__instance).Field("topButtonStyle").GetValue();
               

                var UpdateButton = new ButtonInfo("SYNCHRONIZE MODS AND RESUME GAME",
                    () => 
                    {
                        ModlistManager.Instance.InstantiateModViewForPathOnly(path);
                    }, 
                    18, style);

                var bt = MakeButton(UpdateButton, __instance);
                string colonyName = SaveGameModList.GetModListFileName(path);

                var colony =  ModlistManager.Instance.TryGetColonyModlist(colonyName);
                bool interactable = colony != null ? colony.TryGetModListEntry(path)!=null : false;

                bt.isInteractable = interactable;
                ModlistManager.Instance.ParentObjectRef = __instance.gameObject;
            }
        }


        /// <summary>
        /// On loading a savegame, store the mod config in the modlist.
        /// </summary>
        [HarmonyPatch(typeof(SaveLoader), "Load")]
        [HarmonyPatch(new Type[] { typeof(IReader) })]
        public static class LoadModConfigPatch
        {
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

            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(KMod.Manager),
                    "MatchFootprint");

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SuitableMethodInfo);

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                     code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_1));
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
        [HarmonyPatch(new Type[] { typeof(string), typeof(bool), typeof(bool) })]
        public static class SaaveModConfigOnSave
        {
            public static void Postfix(string filename, bool isAutoSave = false, bool updateSavePointer = true)
            {
                Debug.Log(filename + isAutoSave + updateSavePointer);
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
