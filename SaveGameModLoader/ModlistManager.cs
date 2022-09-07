using HarmonyLib;
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
        public Dictionary<string,SaveGameModList> Modlists = new();
        public Dictionary<string, SaveGameModList> ModPacks = new();
        private static readonly Lazy<ModlistManager> _instance = new Lazy<ModlistManager>(() => new ModlistManager());

        public static ModlistManager Instance { get { return _instance.Value; } }

        public GameObject ParentObjectRef;
        Dictionary<KMod.Label,bool> ModListDifferences = new Dictionary<KMod.Label,bool>();
        List<KMod.Label> MissingMods = new List<KMod.Label>();
        public bool IsSyncing { get; set; }
        public string ActiveSave = string.Empty;

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

        public void InstantiateModViewFromGridView(List<KMod.Label> mods, GameObject parent)
        {
            InstantiateModView(mods, parent);
        }

        /// <summary>
        /// Create a modified Modview for syncing
        /// </summary>
        /// <param name="mods"></param>
        public void InstantiateModView(List<KMod.Label> mods, GameObject parent = null)
        {
            IsSyncing = true;
            AssignModDifferences(mods);
            var ParentGO = parent == null ? ParentObjectRef : parent;

            var modScreen = Util.KInstantiateUI<ModsScreen>(ScreenPrefabs.Instance.modsMenu.gameObject, ParentGO).transform;
#if DEBUG
            UIUtils.ListAllChildren(modScreen);
#endif
            modScreen.gameObject.name = "SYNCSCREEN";
            ///Set Title of Mod Sync Screen.
            modScreen.Find("Panel/Title/Title").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODSYNCING.MODDIFFS;

            var DetailsView = modScreen.Find("Panel/DetailsView").gameObject;
            var workShopButton = modScreen.Find("Panel/DetailsView/WorkshopButton");
            if (workShopButton == null)
            {
                Debug.LogError("Couldnt add buttons to Sync Menu");
                return;
            }
            ///Disable toggle all button if no mods are in the list
            var ToggleAll = modScreen.Find("Panel/DetailsView/ToggleAllButton");
            var ToggleAllButton = ToggleAll.GetComponent<KButton>();
            ToggleAllButton.isInteractable = ModListDifferences.Count > 0;
            ToggleAll.gameObject.SetActive(ModListDifferences.Count > 0);

            //var rmRecursionBtn = modScreen.Find("Panel/DetailsView/ModListsButton");
            //rmRecursionBtn.gameObject.SetActive(false);

            //UnityEngine.Object.Destroy(togglebtn);
            ///Make Close button to "SyncSelected"-button
            var closeBtObj = modScreen.Find("Panel/DetailsView/CloseButton");
            var closeBt = closeBtObj.GetComponent<KButton>();
            closeBt.isInteractable = ModListDifferences.Count > 0 && ModListDifferences.Count > MissingMods.Count;
            closeBt.onClick += () => { AutoRestart(modScreen.GetComponent<ModsScreen>()); };
            closeBtObj.Find("Text").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODSYNCING.SYNCSELECTED;
            closeBtObj.name = "SyncSelectedButton";

            ///Sync all mods button
            var SyncAllButtonObject = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, DetailsView, true);
            SyncAllButtonObject.name = "SyncAllModsButton";
            SyncAllButtonObject.Find("Text").GetComponent<LocText>().text = STRINGS.UI.FRONTEND.MODSYNCING.SYNCALL;
            var SyncAllButton = SyncAllButtonObject.GetComponentInChildren<KButton>(true);
            SyncAllButton.ClearOnClick();
            SyncAllButton.isInteractable = ModListDifferences.Count > 0;
            SyncAllButton.onClick += () => { SyncAllMods(modScreen.GetComponent<ModsScreen>(), null); };



            ///new Close button
            var NewCloseButtonObject = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, DetailsView, true);
            NewCloseButtonObject.name = "newCloseButton";
            NewCloseButtonObject.Find("Text").GetComponent<LocText>().text = global::STRINGS.UI.CREDITSSCREEN.CLOSEBUTTON; 
            NewCloseButtonObject.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 100);
            var NewCloseButton = NewCloseButtonObject.GetComponentInChildren<KButton>(true);
            NewCloseButton.ClearOnClick();

            var methodInfo = typeof(ModsScreen).GetMethod("Exit", BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
                

            NewCloseButton.onClick += ()=> methodInfo.Invoke(modScreen.GetComponent<ModsScreen>(), null); 

            var EntryPos2 = modScreen.Find("Panel").gameObject;

            var missingModListEntry = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, EntryPos2, true);
            missingModListEntry.name = "infoButton";
            var BtnText = missingModListEntry.Find("Text").GetComponent<LocText>();
            var bgColorImage = missingModListEntry.GetComponent<KImage>();
            var Btn = missingModListEntry.GetComponent<KButton>();


            workShopButton.gameObject.SetActive(false);
            if (MissingMods.Count == 0 && ModListDifferences.Count == 0)
            {
                BtnText.text = STRINGS.UI.FRONTEND.MODSYNCING.ALLSYNCED;
                var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
                ColorStyle.inactiveColor = new Color(0.25f, 0.8f, 0.25f);
                ColorStyle.hoverColor = new Color(0.35f, 0.8f, 0.35f);
                bgColorImage.colorStyleSetting = ColorStyle;
                bgColorImage.ApplyColorStyleSetting();
                Btn.ClearOnClick();
                Btn.onClick += () =>
                {
                    ModsScreen screen = modScreen.GetComponent<ModsScreen>();
                    var method = typeof(ModsScreen).GetMethod("Exit", BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(screen, null);
                };
            }
            else if (MissingMods.Count > 0)
            {
                var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
                ColorStyle.inactiveColor = new Color(1f, 0.25f, 0.25f);
                ColorStyle.hoverColor = new Color(1f, 0.35f, 0.35f);
                bgColorImage.colorStyleSetting = ColorStyle;
                bgColorImage.ApplyColorStyleSetting();
                BtnText.text = STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMOD;
                Btn.ClearOnClick();
                Btn.onClick += () =>
                {
                    ShowMissingMods();
                };
            }
            else
                UnityEngine.Object.Destroy(missingModListEntry.gameObject);

            // var infoHeader = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, ListView, true);
        }
        public void ShowMissingMods()
        {
            Manager.Dialog(Global.Instance.globalCanvas, 
                STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSTITLE, 
                string.Format(STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSDESC,
                ModListDifferences.Count,
                MissingMods.Count,
                ListMissingMods()));
        }
        public string ListMissingMods()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var SortedNames = MissingMods.Select(mod => mod.title).ToList();
            SortedNames.Sort();

            stringBuilder.AppendLine();
            Console.WriteLine("------Mod Sync------");
            Console.WriteLine("---[Missing Mods]---");

            for (int i = 0; i< SortedNames.Count; i++)
            {
                if (i < 35)
                {
                    stringBuilder.AppendLine(" • " + SortedNames[i]);
                }
                Console.WriteLine(SortedNames[i]);
            }
            if (SortedNames.Count > 35)
            {
                stringBuilder.AppendLine(String.Format(STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSDESCEND, SortedNames.Count - 35));
            }

            Console.WriteLine("-----[List End]-----");
            Console.WriteLine("------Mod Sync------");
            return stringBuilder.ToString();
        }

        public void AutoRestart(ModsScreen screen)
        {
            var methodInfo = typeof(ModsScreen).GetMethod("Exit", BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo != null)
                methodInfo.Invoke(screen, null);
            if (ModListDifferences.Count > 0)
            {
                ModListDifferences.Clear();
                MissingMods.Clear();
                AutoLoadOnRestart();
            }
        }

        public void SyncAllMods(ModsScreen modScreen, bool? enableAll)
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

            AutoRestart(modScreen);
        }

        public void AssignModDifferences(List<KMod.Label> modList)
        {
            KMod.Manager modManager = Global.Instance.modManager;

            var thisMod = modManager.mods.Find(mod => mod.label.id == ModAssets.ModID).label;


            var allMods = modManager.mods.Select(mod => mod.label).ToList();
            var enabledModLabels = modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();

            ///Workaroundarea for ONY mods
            bool ONYModActivated = false;
            foreach(var mod in enabledModLabels)
            {
                if (mod.title.Contains("by @Ony"))
                {
                    ONYModActivated = true;
                    break;
                }
            }
            if (!ONYModActivated) { 
            for (int i = 0; i < modList.Count; i++)
            {
                if (modList[i].title.Contains("by @Ony ") && !modList[i].title.Contains("👾"))
                {
                    var replaceModLabel = modList[i];
#if DEBUG
                        Debug.LogWarning("Tell @Ony to remove the stupid Emoji from the mod title of: " +replaceModLabel.title);
#endif
                        replaceModLabel.title = replaceModLabel.title+ "👾";
                    modList[i] = replaceModLabel;
                }
            }
            }
            ///End Workaroundarea
            
            var enabledButNotSavedMods = enabledModLabels.Except(modList).ToList();
            var savedButNotEnabledMods = modList.Except(enabledModLabels).ToList();

            MissingMods = modList.Except(allMods).ToList();
#if DEBUG
            Debug.Log("MissingMods start");
            foreach (var m in MissingMods) Debug.Log(m.title);
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


            Debug.Log("The Following mods deviate from the config:");
            foreach (var modDif in ModListDifferences)
            {
                string status = modDif.Value ? "enabled" : "disabled";
                Debug.Log(modDif.Key.id + ": "+ modDif.Key + " -> should be " + status);
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
