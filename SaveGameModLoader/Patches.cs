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
        private static KButton MakeButton(ButtonInfo info, MainMenu instance)
        {
            KButton buttonPrefab = (KButton)Traverse.Create(instance).Field("buttonPrefab").GetValue();
            GameObject buttonParent = (GameObject)Traverse.Create(instance).Field("buttonParent").GetValue();

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


        [HarmonyPatch(typeof(MainMenu), "ResumeGame")]
        public static class AddModSyncToResumeButton
        {
            public static bool Prefix(MainMenu __instance)
            {
                string path;
                if (KPlayerPrefs.HasKey("AutoResumeSaveFile"))
                {
                    path = KPlayerPrefs.GetString("AutoResumeSaveFile");
                }
                else
                    path = string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveForCurrentDLC() : GenericGameSettings.instance.performanceCapture.saveGame;
                if (string.IsNullOrEmpty(path))
                    return true;
                else
                {
                    Debug.Log("For now, Broke button as intended :D");
                    Debug.Log(path);
                    return false;
                }
            }
        }

        //[HarmonyDebug]
        [HarmonyPatch(typeof(LoadScreen), "ShowColony")]
        public static class AddModSyncButtonLogic
        {
            public static void InsertModButtonCode(RectTransform entry
                , object FileDetails
                )
            {

                //var one = FileDetails.GetType();
                //Debug.Log("STEP 1:") ;
                //Debug.Log(one);

                //var two = one.GetField("save", BindingFlags.Public | BindingFlags.Instance);
                //Debug.Log("STEP 2:");
                //Debug.Log(two);

                //var three = two.GetValue(FileDetails);

                //Debug.Log("STEP 3:");

                //Debug.Log(three);


                //var four = three.GetType().GetField("FileName").GetValue(three);

                //Debug.Log("STEP 4:");
                //Debug.Log(four);

                var ContainerOpener = FileDetails.GetType().GetField("save").GetValue(FileDetails);
                string baseName = (string)ContainerOpener.GetType().GetField("BaseName").GetValue(ContainerOpener);
                string fileName = (string)ContainerOpener.GetType().GetField("FileName").GetValue(ContainerOpener);

                //string fileName = (string)___field.GetType().GetField("FileName").GetValue(___field);

                Console.WriteLine("Properties of Type are:");
                Debug.Log("NAME: " + baseName);
                Debug.Log("FILE: " + fileName);

                //var Converted = (SaveGameFileDetails)___field;
                var btn = entry.Find("SyncButton").GetComponent<KButton>();
               //Debug.Log(Converted.FileName + " Eeeeeeeeeeeeeeeee");


                if (btn != null)
                {
                    btn.isInteractable = false;
                    btn.onClick += (() =>
                    {
                        Debug.Log("BUTTONNNNNNNNNNN");
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
                foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };

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


        [HarmonyPatch(typeof(LoadScreen), "OnPrefabInit")]
        public static class AddModSyncButtonToLoadscreen
        {
            public static void Test()
            {
                var active = SaveLoader.GetActiveSaveFilePath();
                ModlistManager.Instance.DoesModlistExist(active);
                Debug.Log("yep this works ---------------");
            }
            public static void Prefix(LoadScreen __instance)
            {

                //Debug.Log("Start Logging LoadScreen parts:");
                //UIUtils.ListAllChildren(__instance.gameObject.transform);
                //Debug.Log("End Logging LoadScreen parts");

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


        [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
        public static class GiveUI
        {

            public static void Test()
            {
                var active = SaveLoader.GetActiveSaveFilePath();
                ModlistManager.Instance.DoesModlistExist(active);
                Debug.Log("yep this works ---------------");
            }
            public static void Prefix(MainMenu __instance)
            {
                Debug.Log("START");
                var prefabGo = ScreenPrefabs.Instance.modsMenu.gameObject;
                UIUtils.ListAllChildren(prefabGo.transform);
                Debug.Log("END");

                Debug.Log("START2");
                UIUtils.ListAllChildren(__instance.gameObject.transform);

                Debug.Log("END2");


                ColorStyleSetting style = (ColorStyleSetting)Traverse.Create(__instance).Field("topButtonStyle").GetValue();

                var UpdateButton = new ButtonInfo("SYNCHRONIZE MODS AND RESUME GAME", new System.Action(GiveUI.Test), 18, style);

                MakeButton(UpdateButton, __instance);
            }
        }


        /// <summary>
        /// On loading a savegame, store the mod config in the modlist file.
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
                Debug.Log("Writing Mod Config to File..");
                //ModlistManager.Instance.CreateOrAddToModLists(instance.GameInfo.originalSaveName, instance.GameInfo.colonyGuid, saveFileRoot.active_mods);
                bool init = ModlistManager.Instance.CreateOrAddToModLists(
                    SaveLoader.GetActiveSaveFilePath(), 
                    SaveGame.GetSaveUniqueID(instance.GameInfo),
                    saveFileRoot.active_mods
                    );
                if (init)
                    Debug.Log("Save game mod config created.");
                else 
                    Debug.Log("Save game mod config overwritten.");
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
    }
}
