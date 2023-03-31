using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static SaveGameModLoader.STRINGS.UI.FRONTEND;

namespace SaveGameModLoader
{
    public class ModlistManager
    {
        public Dictionary<string, SaveGameModList> Modlists = new();
        public Dictionary<string, SaveGameModList> ModPacks = new();
        private static readonly Lazy<ModlistManager> _instance = new Lazy<ModlistManager>(() => new ModlistManager());

        public static ModlistManager Instance { get { return _instance.Value; } }

        public GameObject ParentObjectRef;
        Dictionary<KMod.Label, bool> ModListDifferences = new Dictionary<KMod.Label, bool>();
        List<KMod.Label> MissingMods = new List<KMod.Label>();
        public bool IsSyncing { get; set; }
        public string ActiveSave = string.Empty;

        public static Dictionary<KMod.Label, bool> ModListDifferencesPublic
        {
            get { return Instance.ModListDifferences; }
        }
        public static List<KMod.Label> MissingModsPublic
        {
            get { return Instance.MissingMods; }
        }

        public bool ModIsNotInSync(KMod.Mod mod)
        {
            if (mod.label.id == ModAssets.ModID)
                return false;
            return ModListDifferences.Keys.Contains(mod.label);
        }

        public SaveGameModList TryGetColonyModlist(string colonyName)
        {
            //GetAllStoredModlists();
            Modlists.TryGetValue(colonyName, out SaveGameModList result);
            //SgtLogger.log("ModList found for this savegame");
            return result;
        }

        public void InstantiateSyncViewWithoutRestart(List<KMod.Label> mods, GameObject parent)
        {
            InstantiateModView(mods, "",parent, false);
        }

        /// <summary>
        /// Create a modified Modview for syncing
        /// </summary>
        /// <param name="mods"></param>
        public void InstantiateModView(List<KMod.Label> mods,string activeSaveToLoad = "", GameObject parent = null, bool LoadOnCLose = true)
        {
            ActiveSave = activeSaveToLoad;
            IsSyncing = true;

            var assignAction = () => { AssignModDifferences(mods); };
            assignAction.Invoke();
            var ParentGO = parent == null ? ParentObjectRef : parent;

            var modScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, ParentGO);
            modScreen.gameObject.name = "SYNCSCREEN";
#if DEBUG
            // UIUtils.ListAllChildren(modScreen.transform);
#endif

            var screen = (SyncViewScreen)modScreen.AddComponent(typeof(SyncViewScreen));
            screen.LoadOnClose = LoadOnCLose;
            screen.RefreshAction= assignAction;
        }
        //public void ShowMissingMods()
        //{

        //    Manager.Dialog(Global.Instance.globalCanvas, 
        //        STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSTITLE, 
        //        string.Format(STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSDESC,
        //        ModListDifferences.Count,
        //        MissingMods.Count,
        //        ListMissingMods()));
        //}
        //public string ListMissingMods()
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    var SortedNames = MissingMods.Select(mod => mod.title).ToList();
        //    SortedNames.Sort();

        //    stringBuilder.AppendLine();
        //    Console.WriteLine("------Mod Sync------");
        //    Console.WriteLine("---[Missing Mods]---");

        //    for (int i = 0; i< SortedNames.Count; i++)
        //    {
        //        if (i < 35)
        //        {
        //            stringBuilder.AppendLine(" • " + SortedNames[i]);
        //        }
        //        Console.WriteLine(SortedNames[i]);
        //    }
        //    if (SortedNames.Count > 35)
        //    {
        //        stringBuilder.AppendLine(String.Format(STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSDESCEND, SortedNames.Count - 35));
        //    }

        //    Console.WriteLine("-----[List End]-----");
        //    Console.WriteLine("------Mod Sync------");
        //    return stringBuilder.ToString();
        //}

        public void AutoRestart(bool save = true)
        {
            if (ModListDifferences.Count > 0)
            {
                if (save)
                    Global.Instance.modManager.Save();
                ModListDifferences.Clear();
                MissingMods.Clear();
                AutoLoadOnRestart();
            }
        }
        public enum Status
        {
            NotInstalled,
            Installed,
            UninstallPending,
            ReinstallPending,
        }
        public class file_Mod
        {
            public Label label;
            public Status status;
            public bool enabled;
            public List<string> enabledForDlc;
            public int crash_count;
            public string reinstall_path;
        }
        public class modsJSON
        {
            public int version = 1;
            public List<file_Mod> mods;
        }

        static string ModsFolder { get { return System.IO.Directory.GetParent(System.IO.Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName).ToString() + "\\"; } }

        public static modsJSON ReadGameMods()
        {
            var path = Path.Combine(ModsFolder, "mods.json");
            SgtLogger.l(path);
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists || fileInfo.Extension != ".json")
            {
                SgtLogger.logwarning("no valid file found.");
                return null;
            }
            else
            {
                FileStream filestream = fileInfo.OpenRead();
                using (var sr = new StreamReader(filestream))
                {
                    string jsonString = sr.ReadToEnd();
                    modsJSON modlist = JsonConvert.DeserializeObject<modsJSON>(jsonString);
                    return modlist;
                }
            }
        }

        public void OverwriteGameMods(modsJSON modlist)
        {
            try
            {
                var path = Path.Combine(ModsFolder, "mods.json");
                SgtLogger.l("WRITING TO: " + path);
                var fileInfo = new FileInfo(path);

                FileStream fcreate = fileInfo.Open(FileMode.Create);//(path, FileMode.Create);

                var JsonString = JsonConvert.SerializeObject(modlist, Formatting.Indented);
                SgtLogger.l(JsonString);   
                using (var streamWriter = new StreamWriter(fcreate))
                {
                    SgtLogger.log("Overwriting mods.json");
                    streamWriter.Write(JsonString);
                }
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not write file, Exception: " + e);
            }
        }

        public void SyncFromModListWithoutAutoLoad(List<KMod.Label> modList, System.Action OnFinishAction = null)
        {
            AssignModDifferences(modList);
            SyncAllMods(null, false, OnFinishAction);
        }

        public void SyncAllMods(bool? enableAll, bool restartAfter = true, System.Action OnFinishAction = null)
        {
            if (restartAfter)
                RestartSyncing(enableAll, restartAfter);
            else
            {
                if (ModListDifferences.Count > 20)
                {
                    KMod.Manager.Dialog(Global.Instance.globalCanvas,
                        SINGLEMODLIST.WARNINGMANYMODS,
                        SINGLEMODLIST.WARNINGMANYMODSQUESTION,
                        SINGLEMODLIST.USEALTERNATIVEMODE,
                        () =>
                        {
                            RestartSyncing(enableAll, restartAfter);
                        },
                        SINGLEMODLIST.USENORMALMETHOD,
                        () =>
                        {
                            NormalSyncing(enableAll, restartAfter);
                        },
                        global::STRINGS.UI.FRONTEND.NEWGAMESETTINGS.BUTTONS.CANCEL,
                        () => { }
                  );
                }
                else
                {
                    NormalSyncing(enableAll, restartAfter);
                    KMod.Manager.Dialog(Global.Instance.globalCanvas,
                   SINGLEMODLIST.POPUPSYNCEDTITLE,
                   SINGLEMODLIST.POPUPSYNCEDTEXT,
                   SINGLEMODLIST.RETURNTWO,
                   OnFinishAction
                   );
                }
            }
        }
        public void RestartSyncing(bool? enableAll, bool restartAfter = true)
        {
            var ModFileDeserialized = ReadGameMods();
            if (ModFileDeserialized == null)
                return;


            foreach (var mod in ModFileDeserialized.mods)
                SgtLogger.l(mod.enabledForDlc.FirstOrDefault());


            string dlcId = DlcManager.IsExpansion1Active() ? DlcManager.EXPANSION1_ID : DlcManager.VANILLA_ID;

            foreach (var mod in ModListDifferences.Keys)
            {
                var TargetModIndex = ModFileDeserialized.mods.FindIndex(tmod => tmod.label.id == mod.id || tmod.label.title == mod.title);
                if (TargetModIndex != -1)
                {
                    bool enabled = enableAll == null ? ModListDifferences[mod] : (bool)enableAll;



                    if (ModFileDeserialized.mods[TargetModIndex].enabledForDlc == null)
                    {
                        ModFileDeserialized.mods[TargetModIndex].enabledForDlc = new List<string>();
                    }

                    if (enabled)
                    {
                        SgtLogger.l("ENABLE: " + ModFileDeserialized.mods[TargetModIndex].label);
                        ModFileDeserialized.mods[TargetModIndex].enabledForDlc.Add(dlcId);
                    }  //new List<string> { DlcManager.EXPANSION1_ID }; VANILLA_ID
                    else
                    {
                        SgtLogger.l("DISABLE: " + ModFileDeserialized.mods[TargetModIndex].label);
                        ModFileDeserialized.mods[TargetModIndex].enabledForDlc.Remove(dlcId);
                    }
                }
                else SgtLogger.error(mod.id);
            }


            foreach (var mod in ModFileDeserialized.mods)
                SgtLogger.l(mod.enabledForDlc.FirstOrDefault());

            OverwriteGameMods(ModFileDeserialized);
            
            AutoLoadOnRestart();
        }


        public void NormalSyncing(bool? enableAll, bool restartAfter = true)
        {
            Manager modManager = Global.Instance.modManager;
            foreach (var mod in this.ModListDifferences.Keys)
            {
                if (modManager.FindMod(mod) == null)
                {
                    SgtLogger.warning("Mod not found: " + mod.title);
                    continue;
                }

                bool enabled = enableAll == null ? ModListDifferences[mod] : (bool)enableAll;
                if (mod.id == ModAssets.ModID)
                    enabled = true;

                modManager.EnableMod(mod, enabled, null);
            }
            if (restartAfter)
                AutoRestart();
        }


        public void AssignModDifferences(List<KMod.Label> modList)
        {
            KMod.Manager modManager = Global.Instance.modManager;

            var thisMod = modManager.mods.Find(mod => mod.label.id == ModAssets.ModID).label;


            var allMods = modManager.mods.Select(mod => mod.label).ToList();
            var enabledModLabels = modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();

            var comparer = new ModDifferencesByIdComparer();
            var enabledButNotSavedMods = enabledModLabels.Except(modList, comparer).ToList();
            var savedButNotEnabledMods = modList.Except(enabledModLabels, comparer).ToList();

            MissingMods = modList.Except(allMods, comparer).ToList();
#if DEBUG
            SgtLogger.log("MissingMods start");
            foreach (var m in MissingMods) SgtLogger.log(m.id + ": " + m.title);
            SgtLogger.log("MissingMods end");
#endif
            ModListDifferences.Clear();
            foreach (var toDisable in enabledButNotSavedMods)
            {
                ModListDifferences.Add(toDisable, false);
            }
            foreach (var toEnable in savedButNotEnabledMods)
            {
                ModListDifferences.Add(toEnable, true);
            }
            ModListDifferences.Remove(thisMod);

#if DEBUG
            SgtLogger.log("The Following mods deviate from the config:");
            foreach (var modDif in ModListDifferences)
            {
                string status = modDif.Value ? "enabled" : "disabled";
                SgtLogger.log(modDif.Key.id + ": " + modDif.Key + " -> should be " + status);
            }
#endif

        }

        public class ModDifferencesByIdComparer : IEqualityComparer<Label>
        {
            public bool Equals(Label l1, Label l2)
            {
                return l1.id == l2.id || l1.title == l2.title;
            }

            public int GetHashCode(Label obj)
            {
                //SgtLogger.log(obj.id + ": "+obj.title);
                return obj.id.GetHashCode();
            }

        }

        void AutoLoadOnRestart()
        {

            if (ActiveSave != string.Empty)
                KPlayerPrefs.SetString("AutoResumeSaveFile", ActiveSave);
            ActiveSave = string.Empty;
            App.instance.Restart();
        }


        public void InstantiateModViewForPathOnly(string referencedPath)
        {
            var mods = TryGetColonyModlist(SaveGameModList.GetModListFileName(referencedPath));
            if (mods == null)
            {
                SgtLogger.logError("No Modlist found for " + SaveGameModList.GetModListFileName(referencedPath));
                return;
            }
            var list = mods.TryGetModListEntry(referencedPath);

            if (list == null)
            {
                SgtLogger.logError("No ModConfig found for " + referencedPath);
                return;
            }
            InstantiateModView(list, referencedPath);
        }

        public void GetAllStoredModlists()
        {
            Modlists.Clear();
            MissingMods.Clear();
            var files = new DirectoryInfo(ModAssets.ModPath).GetFiles();

            
            foreach (FileInfo modlist in files)
            {
                try
                {
                    //SgtLogger.log("Trying to load: " + modlist);
                    var list = SaveGameModList.ReadModlistListFromFile(modlist);
                    if(list != null)
                    {
                        Modlists.Add(list.ReferencedColonySaveName, list);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load savegamemod list from: " + modlist.FullName + ", Error: " + e);
                }
            }
            SgtLogger.log("Found Mod Configs for " + files.Count() + " Colonies");
        }
        public void GetAllModPacks()
        {
            ModPacks.Clear();
            MissingMods.Clear();
            var files = new DirectoryInfo(ModAssets.ModPacksPath).GetFiles();
            foreach (FileInfo modlist in files)
            {
                SgtLogger.l(modlist.ToString(), "FilePathModProfile");
            }
            foreach (FileInfo modlist in files)
            {
                try
                {
                    //SgtLogger.log("Trying to load: " + modlist);
                    var list = SaveGameModList.ReadModlistListFromFile(modlist);
                    if (list != null)
                    {
                        ModPacks.Add(list.ReferencedColonySaveName, list);
                    }
                }
                catch (Exception e)
                {
                    SgtLogger.logError("Couln't load Mod list from: " + modlist.FullName + ", Error: " + e);
                }
            }
            SgtLogger.log("Found " + files.Count() + " custom profiles");
        }

        public bool CreateOrAddToModLists(string savePath, List<KMod.Label> list)
        {
            bool hasBeenInitialized = false;

            Modlists.TryGetValue(SaveGameModList.GetModListFileName(savePath), out SaveGameModList colonyModSave);

            if (colonyModSave == null)
            {
                hasBeenInitialized = true;
                colonyModSave = new SaveGameModList(savePath);
            }

            colonyModSave.Type = DlcManager.IsExpansion1Active() ? SaveGameModList.DLCType.spacedOut : SaveGameModList.DLCType.baseGame;

            bool subListInitialized = colonyModSave.AddOrUpdateEntryToModList(savePath, list);
            Modlists[SaveGameModList.GetModListFileName(savePath)] = colonyModSave;
            if (hasBeenInitialized)
                SgtLogger.log("New mod config file created for: " + SaveGameModList.GetModListFileName(savePath));
            if (subListInitialized)
                SgtLogger.log("New mod list added for: " + savePath);
            else
                SgtLogger.log("mod list overwritten for: " + savePath);

            return hasBeenInitialized | subListInitialized;

        }


        public bool CreateOrAddToModPacks(string savePath, List<KMod.Label> list)
        {
            bool hasBeenInitialized = false;

            ModPacks.TryGetValue(savePath, out SaveGameModList ModPackFile);

            if (ModPackFile == null)
            {
                hasBeenInitialized = true;
                ModPackFile = new SaveGameModList(savePath, true);
            }

            int versionNumber = ModPackFile.SavePoints.Count + 1;

            var VersionString = "Version " + versionNumber.ToString();

            bool subListInitialized = ModPackFile.AddOrUpdateEntryToModList(VersionString, list, true);
#if DEBUG
            SgtLogger.log(savePath + "<>" + VersionString);
#endif

            ModPacks[(savePath)] = ModPackFile;
            if (hasBeenInitialized)
                SgtLogger.log("New mod pack file created: " + savePath);
            if (subListInitialized)
                SgtLogger.log("New mod pack added for: " + savePath);

            return hasBeenInitialized | subListInitialized;

        }
    }
}
