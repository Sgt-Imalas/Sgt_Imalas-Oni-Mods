using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static UtilLibs.UIUtils;
using static SaveGameModLoader.STRINGS.UI.FRONTEND.SINGLEMODLIST;

namespace SaveGameModLoader
{
    class SingleModListView : KModalScreen
    {

        bool ViewSingleEntry = false;
        bool IsMissingModsOnly = false;
        List<KMod.Label> MissingModsList = new List<KMod.Label>();
        string Title = string.Empty;
        SaveGameModList Mods;
        List<GameObject> Entries = new List<GameObject>();

        Transform EntryPrefab;
        Transform InsertLocation;

        //GameObject ReturnButton;
        GameObject SyncButton;
        KModalScreen ParentWindow;

        public void SetAdditionalButtons(bool active)
        {
            ViewSingleEntry = active;

            if(SyncButton != null)
                SyncButton.SetActive(active);
            //SyncButton.SetActive(active);
            //Debug.Log(IsMissingModsOnly + "<-Missing only  IsActive? " + active);
            if (!active) RebuildList();
        }

        protected override void OnActivate()
        {

            var SingleFileModlists = this.transform;
            ///Set Title of Mod Sync Screen.
            if (Mods != null)
            {
                TryChangeText(
                    SingleFileModlists,
                    "Panel/Title/Title",
                    Mods.IsModPack ?
                        string.Format(TITLE, Title)
                        : string.Format(TITLESAVEGAMELIST, Title));
            }
            else
            {
                TryChangeText(
                    SingleFileModlists,
                    "Panel/Title/Title", 
                    STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSTITLE
                   );
            }

            var DetailsView = SingleFileModlists.Find("Panel/DetailsView").gameObject;

            ///Disable unneeded buttons
            FindAndDisable(SingleFileModlists, "Panel/DetailsView/ToggleAllButton");
            //FindAndDisable(SingleFileModlists, "Panel/DetailsView/WorkshopButton");



            /////WorkshopButton Button for single list view sync
            SyncButton = transform.Find("Panel/DetailsView/WorkshopButton").gameObject;
            SyncButton.SetActive(false);

            TryChangeText(
                SingleFileModlists,
                "Panel/DetailsView/WorkshopButton/Text",
                SYNCLIST);


            TryChangeText(
                SingleFileModlists,
                "Panel/DetailsView/CloseButton/Text",
                RETURN);

            ///Make Close(s) functional
            AddActionToButton(
                SingleFileModlists,
                "Panel/DetailsView/CloseButton",
                new System.Action(this.Deactivate),
                true);

            AddActionToButton(
                transform,
                "Panel/Title/CloseButton",
                new System.Action(this.Deactivate),
                true);

            ///get and edit entry prefab
            EntryPrefab = SingleFileModlists.Find("Panel/ListView/Files/Viewport/Entry");
            FindAndDisable(EntryPrefab, "DragReorderIndicator");
            FindAndDisable(EntryPrefab, "EnabledToggle");
            FindAndRemove<DragMe>(EntryPrefab);
            InsertLocation = SingleFileModlists.Find("Panel/ListView/Files/Viewport/Content");
            SetAdditionalButtons(false);
        }



        void RebuildList(List<KMod.Label> modsForSingleView = null)
        {

            if (Entries.Count > 0) 
            { 
                foreach (var entry in Entries)
                UnityEngine.Object.Destroy(entry);
                Entries.Clear();
            }
            var missingModColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            missingModColorStyle.inactiveColor = new Color(0.7f, 0.25f, 0.25f);
            missingModColorStyle.activeColor = new Color(0.7f, 0.25f, 0.25f);
            missingModColorStyle.disabledColor = new Color(0.7f, 0.25f, 0.25f);
            missingModColorStyle.hoverColor = new Color(0.8f, 0.25f, 0.25f);

            if (IsMissingModsOnly)
            {
                //Debug.Log("AddedMissingToView");
                foreach (var mod in MissingModsList)
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                    TryChangeText(entry, "Title", mod.title);
                    TryChangeText(entry, "Version", "internal mod Version: " + mod.version.ToString());
                    
                    var bgColor = TryFindComponent<KImage>(entry, "BG");
                    bgColor.defaultState = KImage.ColorSelector.Disabled;
                    bgColor.ColorState = KImage.ColorSelector.Disabled;
                    var infoBt = TryFindComponent<KButton>(entry, "ManageButton");

                    bool isSteamMod = mod.distribution_platform == KMod.Label.DistributionPlatform.Steam;
                    infoBt.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(isSteamMod ? WORKSHOPFINDTOOLTIP : NOSTEAMMOD);
                    infoBt.isInteractable = isSteamMod;
                    AddActionToButton(
                            infoBt.transform,
                            "",
                            () => { App.OpenWebURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + mod.id); }, true);

                        var bgColorImage = infoBt.bgImage;
                        bgColorImage.colorStyleSetting = missingModColorStyle;
                        bgColorImage.ApplyColorStyleSetting();

                        TryChangeText(entry, "ManageButton/Text", "<!> "+MISSING);
                    Entries.Add(entry.gameObject);

                }
                return;
            }

            if (ViewSingleEntry)
            {
                
                if (modsForSingleView == null|| modsForSingleView.Count == 0)
                    return;
                KMod.Manager modManager = Global.Instance.modManager;

                var allMods = modManager.mods.Select(mod => mod.label).ToList();
                var MissingMods = modsForSingleView.Except(allMods, new ModlistManager.ModDifferencesByIdComparer()).ToList();


                foreach (var mod in modsForSingleView)
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                    TryChangeText(entry, "Title", mod.title);
                    TryChangeText(entry, "Version", "internal mod Version: " + mod.version.ToString());

                    if (MissingMods.Contains(mod, new ModlistManager.ModDifferencesByIdComparer()))
                    {
                        var bgColor = TryFindComponent<KImage>(entry,"BG");
                        bgColor.defaultState = KImage.ColorSelector.Disabled;
                        bgColor.ColorState = KImage.ColorSelector.Disabled;

                        var infoBt = TryFindComponent<KButton>(entry, "ManageButton");
                        bool isSteamMod = mod.distribution_platform == KMod.Label.DistributionPlatform.Steam;
                        infoBt.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(isSteamMod?WORKSHOPFINDTOOLTIP: NOSTEAMMOD);
                        infoBt.isInteractable = isSteamMod;
                        AddActionToButton(
                            infoBt.transform, 
                            "",
                            () => { App.OpenWebURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + mod.id); }, true);

                        var bgColorImage = infoBt.bgImage;
                        bgColorImage.colorStyleSetting = missingModColorStyle;
                        bgColorImage.ApplyColorStyleSetting();

                        TryChangeText(entry, "ManageButton/Text", "<!> Missing");
                    }
                    else
                    {
                        FindAndDisable(entry, "ManageButton");
                    }
                    Entries.Add(entry.gameObject);

                }
                TryFindComponent<KButton>(SyncButton.transform).isInteractable = modsForSingleView.Count > MissingMods.Count;
                AddActionToButton(
                       SyncButton.transform,
                       "",
                       () => SyncSingleList(modsForSingleView),
                       true);

                AddActionToButton(
                    transform,
                    "Panel/DetailsView/CloseButton",
                    () => SetAdditionalButtons(false),
                    true);
            }
            else
            {
                //Debug.Log("All View");
                if (Mods == null) return;
                foreach (var mod in Mods.SavePoints.Reverse())
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                    TryChangeText(entry, "Title", mod.Key);
                    TryChangeText(entry, "Version", string.Format(MODLISTCOUNT, mod.Value.Count));
                    TryChangeText(entry, "ManageButton/Text", LOADLIST);
                    AddActionToButton(
                        entry,
                        "ManageButton",
                         () => ViewSingleList(mod.Value)
                        , true);
                    var syncbt = TryInsertNamedCopy(entry, "ManageButton", "SyncButton");
                    TryChangeText(syncbt.transform, "Text", SYNCLIST);

                    AddActionToButton(
                        syncbt,
                        "",
                        () => SyncSingleList(mod.Value)
                        , true);
                    Entries.Add(entry.gameObject);
                }
                
                AddActionToButton(
                transform,
                "Panel/DetailsView/CloseButton",
                new System.Action(this.Deactivate),
                true);
            }
        }
        
        internal void ViewSingleList(List<KMod.Label> modsInList)
        {
            SetAdditionalButtons(true);
            RebuildList(modsInList);
        }

        internal void SyncSingleList(List<KMod.Label> modsInList)
        {
            ModlistManager.Instance.SyncFromModListWithoutAutoLoad(modsInList);
            KMod.Manager.Dialog(Global.Instance.globalCanvas,
               POPUPSYNCEDTITLE,
               POPUPSYNCEDTEXT,
               RETURNTWO,
               ()=> { 
                   this.Deactivate();
                   if(ParentWindow!=null)
                       ParentWindow.Deactivate();
                    }               
               );
        }
        internal void InstantiateParams(KeyValuePair<string, SaveGameModList> exportedList, KModalScreen parent)
        {
            Title = exportedList.Key;
            Mods = exportedList.Value;
            ParentWindow = parent;
        }
        public void InstantiateMissing(List<KMod.Label> missingMods)
        {
            Title = STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSTITLE;
            IsMissingModsOnly = true; 
            MissingModsList.Clear();
            MissingModsList.AddRange(missingMods);
        }
    }
}
