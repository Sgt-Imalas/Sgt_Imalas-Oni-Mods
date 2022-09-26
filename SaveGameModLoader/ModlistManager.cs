﻿using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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

        public static Dictionary<KMod.Label, bool>  ModListDifferencesPublic
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
            //Debug.Log("ModList found for this savegame");
            return result;
        }

        public void InstantiateSyncViewWithoutRestart(List<KMod.Label> mods, GameObject parent)
        {
            InstantiateModView(mods, parent, false);
        }

        /// <summary>
        /// Create a modified Modview for syncing
        /// </summary>
        /// <param name="mods"></param>
        public void InstantiateModView(List<KMod.Label> mods, GameObject parent = null, bool LoadOnCLose = true)
        {
            IsSyncing = true;
            AssignModDifferences(mods);
            var ParentGO = parent == null ? ParentObjectRef : parent;

            var modScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, ParentGO);
            modScreen.gameObject.name = "SYNCSCREEN";
#if DEBUG
           // UIUtils.ListAllChildren(modScreen.transform);
#endif

            var screen =(SyncViewScreen)modScreen.AddComponent(typeof(SyncViewScreen));
            screen.LoadOnClose = LoadOnCLose;

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

        public void AutoRestart()
        {
            if (ModListDifferences.Count > 0)
            {
                Global.Instance.modManager.Save();
                ModListDifferences.Clear();
                MissingMods.Clear();
                AutoLoadOnRestart();
            }
        }
        public void SyncFromModListWithoutAutoLoad(List<KMod.Label> modList)
        {
            AssignModDifferences(modList);
            SyncAllMods(null, false);
        }

        public void SyncAllMods(bool? enableAll, bool restartAfter = true)
        {
            Manager modManager = Global.Instance.modManager;

            foreach (var mod in this.ModListDifferences.Keys)
            {
                if (modManager.FindMod(mod) == null)
                {
                    Debug.LogWarning("Mod not found: " + mod.title);
                    continue;
                }

                bool enabled = enableAll == null? ModListDifferences[mod] : (bool)enableAll;
                if (mod.id == ModAssets.ModID)
                    enabled = true;

                modManager.EnableMod(mod, enabled, null);
            }
            if(restartAfter)
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
            Debug.Log("MissingMods start");
            foreach (var m in MissingMods) Debug.Log(m.id+": "+m.title);
            Debug.Log("MissingMods end");
#endif
            ModListDifferences.Clear();
            foreach(var toDisable in enabledButNotSavedMods)
            {
                ModListDifferences.Add(toDisable, false);
            }
            foreach (var toEnable in savedButNotEnabledMods)
            {
                ModListDifferences.Add(toEnable, true);
            }
            ModListDifferences.Remove(thisMod);

#if DEBUG
            Debug.Log("The Following mods deviate from the config:");
            foreach (var modDif in ModListDifferences)
            {
                string status = modDif.Value ? "enabled" : "disabled";
                Debug.Log(modDif.Key.id + ": "+ modDif.Key + " -> should be " + status);
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
                //Debug.Log(obj.id + ": "+obj.title);
                return obj.id.GetHashCode();
            }
        }

        void AutoLoadOnRestart()
        {
            if(ActiveSave!=string.Empty)
                KPlayerPrefs.SetString("AutoResumeSaveFile", ActiveSave);
            ActiveSave = string.Empty;
            Global.Instance.modManager.Save(); 
            App.instance.Restart();
        }


        public void InstantiateModViewForPathOnly(string referencedPath)
        {
            ActiveSave = referencedPath;
            var mods = TryGetColonyModlist(SaveGameModList.GetModListFileName(referencedPath));
            if (mods == null)
            {
                Debug.LogError("No Modlist found for " + SaveGameModList.GetModListFileName(referencedPath));
                return;
            }
            var list = mods.TryGetModListEntry(referencedPath);

            if(list==null)
            {
                Debug.LogError("No ModConfig found for " + referencedPath);
                return;
            }
            InstantiateModView(list);
        }

        public void GetAllStoredModlists()
        {
            Modlists.Clear();
            MissingMods.Clear();
            var files = Directory.GetFiles(ModAssets.ModPath);
            foreach(var modlist in files)
            {
                try
                {
                   //Debug.Log("Trying to load: " + modlist);
                   var list = SaveGameModList.ReadModlistListFromFile(modlist);
                    Modlists.Add(list.ReferencedColonySaveName, list);
                }
                catch(Exception e)
                {
                    Debug.LogError("Couln't load savegamemod list from: " + modlist + ", Error: "+e);
                }
            }
            //Debug.Log("Found Mod Configs for " + files.Count() + " Colonies");
        }
        public void GetAllModPacks()
        {
            ModPacks.Clear();
            MissingMods.Clear();
            var files = Directory.GetFiles(ModAssets.ModPacksPath);
            foreach (var modlist in files)
            {
                try
                {
                    //Debug.Log("Trying to load: " + modlist);
                    var list = SaveGameModList.ReadModlistListFromFile(modlist);
                    ModPacks.Add(list.ReferencedColonySaveName, list);
                }
                catch (Exception e)
                {
                    Debug.LogError("Couln't load Mod list from: " + modlist + ", Error: " + e);
                }
            }
            //Debug.Log("Found Mod Configs for " + files.Count() + " Colonies");
        }

        public bool CreateOrAddToModLists(string savePath,List<KMod.Label> list)
        {
            bool hasBeenInitialized = false;

            Modlists.TryGetValue(SaveGameModList.GetModListFileName(savePath),out SaveGameModList colonyModSave);

            if (colonyModSave == null)
            {
                hasBeenInitialized = true;
                   colonyModSave = new SaveGameModList(savePath);
            }
            
            colonyModSave.Type = DlcManager.IsExpansion1Active() ? SaveGameModList.DLCType.spacedOut : SaveGameModList.DLCType.baseGame;

            bool subListInitialized = colonyModSave.AddOrUpdateEntryToModList(savePath, list);
            Modlists[SaveGameModList.GetModListFileName(savePath)] = colonyModSave;
            if (hasBeenInitialized)
                Debug.Log("New mod config file created for: "+ SaveGameModList.GetModListFileName(savePath));
            if(subListInitialized)
                Debug.Log("New mod list added for: " + savePath);
            else
                Debug.Log("mod list overwritten for: "+ savePath);

            return hasBeenInitialized | subListInitialized;
            
        }


        public bool CreateOrAddToModPacks(string savePath, List<KMod.Label> list)
        {
            bool hasBeenInitialized = false;

            ModPacks.TryGetValue(savePath, out SaveGameModList ModPackFile);

            if (ModPackFile == null)
            {
                hasBeenInitialized = true;
                ModPackFile = new SaveGameModList(savePath,true);
            }

            int versionNumber = ModPackFile.SavePoints.Count + 1;

            var VersionString = "Version " + versionNumber.ToString();

            bool subListInitialized = ModPackFile.AddOrUpdateEntryToModList(VersionString, list,true);
#if DEBUG
            Debug.Log(savePath + "<>"+ VersionString);
#endif

            ModPacks[(savePath)] = ModPackFile;
            if (hasBeenInitialized)
                Debug.Log("New mod pack file created: " + savePath);
            if (subListInitialized)
                Debug.Log("New mod pack added for: " + savePath);

            return hasBeenInitialized | subListInitialized;

        }
    }
}