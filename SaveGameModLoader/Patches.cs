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

namespace SaveGameModLoader
{
    class Patches
    {
        [HarmonyPatch(typeof(MainMenu), "ResumeGame")]
        public class MainMenuModSelectionPatch
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


            private static byte[] DecompressContents(byte[] compressed) => ZlibStream.UncompressBuffer(compressed);

            public static void LoadMods(IReader reader, SaveGame.GameInfo GameInfo)
            {
                Debug.Assert(reader.ReadKleiString() == "world");

                Deserializer deserializer = new Deserializer(reader);
                SaveFileRoot saveFileRoot = new SaveFileRoot();
                deserializer.Deserialize((object)saveFileRoot);
                if ((GameInfo.saveMajorVersion == 7 || GameInfo.saveMinorVersion < 8) && saveFileRoot.requiredMods != null)
                {
                    saveFileRoot.active_mods = new List<KMod.Label>();
                    foreach (ModInfo requiredMod in saveFileRoot.requiredMods)
                        saveFileRoot.active_mods.Add(new KMod.Label()
                        {
                            id = requiredMod.assetID,
                            version = (long)requiredMod.lastModifiedTime,
                            distribution_platform = KMod.Label.DistributionPlatform.Steam,
                            title = requiredMod.description
                        });
                    saveFileRoot.requiredMods.Clear();
                }
                KMod.Manager modManager = Global.Instance.modManager;

                var enabledMods = modManager.mods.FindAll(mod => mod.IsActive() == true);
                var enabledModLabels = modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();

                Debug.Log(saveFileRoot.active_mods);
                Debug.Log("Enabled Mods before Changes:" + enabledMods.Count);
                Debug.Log(0);
                var enabledButNotSavedMods = enabledModLabels.Except(saveFileRoot.active_mods).ToList(); Debug.Log(1);
                var savedButNotEnabledMods = saveFileRoot.active_mods.Except(enabledModLabels).ToList(); Debug.Log(2);
                List<string> enabledIds = new();
                List<string> disabledIds = new();
                Debug.Log(3);
                if (enabledButNotSavedMods.Count > 0)
                {
                    enabledButNotSavedMods.Remove(enabledButNotSavedMods.Find(mod => mod.title == "SaveGameModLoader"));
                    enabledIds = enabledButNotSavedMods.Select(label => label.id).ToList();
                }

                Debug.Log(4);
                if (savedButNotEnabledMods.Count > 0)
                {
                    savedButNotEnabledMods.Remove(savedButNotEnabledMods.Find(mod => mod.title == "SaveGameModLoader"));
                    disabledIds = savedButNotEnabledMods.Select(label => label.id).ToList();
                }

                Debug.Log(5);

                foreach (var id in enabledIds)
                {
                    var modToDisable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
                    {
                        if (modToDisable != null)
                        {
                            modToDisable.SetEnabledForActiveDlc(false);
                            Debug.Log("enabled but not stored in SaveGame, disabling: " + id + " : " + modToDisable.title);
                        }
                        else
                        {
                            Debug.LogWarning("Mod " + id + " : " + enabledButNotSavedMods.Find(m => m.id == id).title + " is not installed, how did this happen?");
                        }
                    }
                    Debug.Log(6);
                }
                foreach (var id in disabledIds)
                {

                    var modToEnable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
                    if (modToEnable != null)
                    {
                        modToEnable.SetEnabledForActiveDlc(true);
                        Debug.Log("stored in SaveGame but not enabled, enabling: " + id + " : " + modToEnable.title);
                    }
                    else
                    {
                        Debug.LogWarning("Mod " + id + " : " + modToEnable.title + " is stored in this SaveGame, but not installed!");
                    }
                    Debug.Log(7);

                }


                if (enabledButNotSavedMods.Count > 0 || savedButNotEnabledMods.Count > 0)
                {
                    Global.Instance.modManager.Save();
                    new System.Action(App.Quit).Invoke(); //App.Instance.Restart
                    return;
                }

            }
            public static void DebugMethod(string filename)
            {
                Debug.Log("Path of save: " + filename + ", path is null?? "+ filename.IsNullOrDestroyed());
                if (WorldGen.CanLoad(filename))
                {
                    try
                    {
                        KSerialization.Manager.Clear();
                        byte[] numArray1 = File.ReadAllBytes(filename);
                        IReader reader = (IReader)new FastReader(numArray1);
                        SaveGame.Header header ;
                        var GameInfo = SaveGame.GetHeader(reader, out header, filename);
                        DebugUtil.LogArgs((object)string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", (object)header.headerVersion, (object)header.buildVersion, (object)header.headerSize, (object)header.IsCompressed, (object)filename));
                        DebugUtil.LogArgs((object)string.Format("GameInfo loaded from save header:\n  numberOfCycles:{0},\n  numberOfDuplicants:{1},\n  baseName:{2},\n  isAutoSave:{3},\n  originalSaveName:{4},\n  clusterId:{5},\n  worldTraits:{6},\n  colonyGuid:{7},\n  saveVersion:{8}.{9}", (object)GameInfo.numberOfCycles, (object)GameInfo.numberOfDuplicants, (object)GameInfo.baseName, (object)GameInfo.isAutoSave, (object)GameInfo.originalSaveName, (object)GameInfo.clusterId, GameInfo.worldTraits == null || GameInfo.worldTraits.Length == 0 ? (object)"<i>none</i>" : (object)string.Join(", ", GameInfo.worldTraits), (object)GameInfo.colonyGuid, (object)GameInfo.saveMajorVersion, (object)GameInfo.saveMinorVersion));

                        if (GameInfo.saveMajorVersion == 7 && GameInfo.saveMinorVersion < 4)
                            Helper.SetTypeInfoMask(SerializationTypeInfo.VALUE_MASK | SerializationTypeInfo.IS_GENERIC_TYPE);
                        KSerialization.Manager.DeserializeDirectory(reader);

                        if (header.IsCompressed)
                        {
                            int length = numArray1.Length - reader.Position;
                            byte[] numArray2 = new byte[length];
                            Array.Copy((Array)numArray1, reader.Position, (Array)numArray2, 0, length);
                            byte[] bytes = DecompressContents(numArray2);
                            LoadMods((IReader)new FastReader(bytes), GameInfo);
                        }
                        else
                        {
                            LoadMods(reader, GameInfo);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        DebugUtil.LogWarningArgs((object)("\n--- Error loading mods from Save ---\n" + ex.Message + "\n" + ex.StackTrace));
                        Sim.Shutdown();
                        SaveLoader.SetActiveSaveFilePath((string)null);
                    }
                }

                    new System.Action(App.Quit).Invoke();
            }

            public static readonly MethodInfo ResumeGame = AccessTools.Method(
               typeof(MainMenuModSelectionPatch),
               nameof(DebugMethod));

            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(System.String),
                    "IsNullOrEmpty");

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
              
                var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == SuitableMethodInfo);


                if (insertionIndex != -1)
                {
                    Debug.Log("foundIT!");
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_1));
                    //code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_0));
                    //code.Insert(insertionIndex, new CodeInstruction(OpCodes.Call, ResumeGame));
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_0));
                }
                //Debug.Log("-------------------");
                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                return code;
            }
        }

        
        [HarmonyPatch(typeof(SaveLoader), "Load")]
        [HarmonyPatch(new Type[] { typeof(IReader) })]

        public class LoadModConfigPatch
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


            public static void WriteSaveGameModSettings(SaveFileRoot saveFileRoot)
            {

            }
           
            public static void WriteModlistPatch(SaveFileRoot saveFileRoot)
            {
                //foreach(var label in saveFileRoot.active_mods)
                //{
                //   Debug.Log("TRANSPILER PATCH; Modname 1 :" + label.id + " : " + label.title);
                //}
                var enabledMods = Global.Instance.modManager.mods.FindAll(mod => mod.IsActive() == true);//.Select(mod => mod.label).ToList();
                var enabledModLabels = Global.Instance.modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();
                
                Debug.Log("Enabled Mods count:" + enabledMods.Count);
                //foreach (var mod in enabledMods)
                //{
                //    Debug.Log("TRANSPILER PATCH; Enabled Mod:" + mod.label.id + " : "+ mod.label.title);
                //}



                var enabledButNotSavedMods = enabledModLabels.Except(saveFileRoot.active_mods).ToList(); 
                var savedButNotEnabledMods = saveFileRoot.active_mods.Except(enabledModLabels).ToList();

                enabledButNotSavedMods.Remove(enabledButNotSavedMods.Find(mod => mod.title == "SaveGameModLoader"));
                savedButNotEnabledMods.Remove(savedButNotEnabledMods.Find(mod => mod.title == "SaveGameModLoader"));

                var enabledIds = enabledButNotSavedMods.Select(label => label.id);
                var disabledIds = savedButNotEnabledMods.Select(label => label.id);

                
                
                foreach (var id in enabledIds)
                {
                    var modToDisable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
                    {
                        if (modToDisable != null)
                        {
                            modToDisable.SetEnabledForActiveDlc(false);
                            Debug.Log("enabled but not stored in SaveGame, disabling: " + id + " : " + modToDisable.title);
                        }
                        else
                        {
                            Debug.LogWarning("Mod " + id + " : " + enabledButNotSavedMods.Find(m => m.id==id).title + " is not installed, how did this happen?");
                        }
                    }
                }
                foreach (var id in disabledIds)
                {

                    var modToEnable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
                    if (modToEnable != null)
                    {
                        modToEnable.SetEnabledForActiveDlc(true);
                        Debug.Log("stored in SaveGame but not enabled, enabling: " + id + " : " + modToEnable.title);
                    }
                    else
                    {
                        Debug.LogWarning("Mod " + id + " : " + modToEnable.title + " is stored in this SaveGame, but not installed!");
                    }

                }


                if(enabledButNotSavedMods.Count>0 || savedButNotEnabledMods.Count > 0)
                {
                    Global.Instance.modManager.Save();
                    new System.Action(App.Quit).Invoke(); //App.Instance.Restart
                    return;
                }

            }

            public static readonly MethodInfo ScreenCreator = AccessTools.Method(
               typeof(LoadModConfigPatch),
               nameof(WriteModlistPatch)

            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldstr && (string)ci.operand == "Mod footprint of save file doesn't match current mod configuration");

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                     //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_1));
                    // code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ScreenCreator));
                }

                return code;
            }
        }
    }
}
