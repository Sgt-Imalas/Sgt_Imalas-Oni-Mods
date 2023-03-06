using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static UtilLibs.UIUtils;
using static SaveGameModLoader.STRINGS.UI.FRONTEND.SINGLEMODLIST;
using Steamworks;
using static SaveGameModLoader.STRINGS.UI.FRONTEND;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;

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
        GameObject RefreshViewBtn;
        GameObject WorkShopSubBtn;
        KModalScreen ParentWindow;

        public void SetAdditionalButtons(bool active)
        {
            ViewSingleEntry = active;

            if (SyncButton != null)
                SyncButton.SetActive(active);
            if (RefreshViewBtn != null)
                RefreshViewBtn.SetActive(active);
            if (WorkShopSubBtn != null)
                WorkShopSubBtn.SetActive(active || IsMissingModsOnly);
            //SyncButton.SetActive(active);
            //SgtLogger.log(IsMissingModsOnly + "<-Missing only  IsActive? " + active);
            if (!active) RebuildList();
        }

        ColorStyleSetting missingModColorStyle;
        public override void OnActivate()
        {
            missingModColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            missingModColorStyle.inactiveColor = new Color(0.7f, 0.25f, 0.25f);
            missingModColorStyle.activeColor = new Color(0.7f, 0.25f, 0.25f);
            missingModColorStyle.disabledColor = new Color(0.7f, 0.25f, 0.25f);
            missingModColorStyle.hoverColor = new Color(0.8f, 0.25f, 0.25f);

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
            //FindAndDisable(SingleFileModlists, "Panel/DetailsView/ToggleAllButton");
            //FindAndDisable(SingleFileModlists, "Panel/DetailsView/WorkshopButton");

            ///SubToAll missing mods
            TryChangeText(
                SingleFileModlists,
                "Panel/DetailsView/ToggleAllButton/Text",
                REFRESH);



            ///WorkshopButton Button for single list view sync

            RefreshViewBtn = transform.Find("Panel/DetailsView/ToggleAllButton").gameObject;
            RefreshViewBtn.transform.SetAsFirstSibling();
            RefreshViewBtn.SetActive(false);

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


            WorkShopSubBtn = TryInsertNamedCopy(DetailsView.transform, "ToggleAllButton", "SubAllButton").gameObject;
            WorkShopSubBtn.transform.SetAsFirstSibling();
            TryChangeText(WorkShopSubBtn.transform, "Text", WORKSHOPACTIONS.BUTTON);

            ///get and edit entry prefab
            EntryPrefab = SingleFileModlists.Find("Panel/ListView/Files/Viewport/Entry");
            FindAndDisable(EntryPrefab, "DragReorderIndicator");
            FindAndDisable(EntryPrefab, "EnabledToggle");
            FindAndRemove<DragMe>(EntryPrefab);
            InsertLocation = SingleFileModlists.Find("Panel/ListView/Files/Viewport/Content");
            SetAdditionalButtons(false);
        }


        void SubActionToAllMissing(List<KMod.Label> allMissings)
        {
            if (allMissings.Count == 0)
                return;
            int msInterval = 1000;

            SgtLogger.log("SubToAllCalled , missing mods count: " + allMissings.Count);
            for (int i = 0; i < allMissings.Count; ++i)
            {
                var mod = allMissings[i];
                SgtLogger.log("nr: " + i+ ", mod: "+ mod.id);
                string modID = mod.id;
                if (mod.distribution_platform == KMod.Label.DistributionPlatform.Steam)
                {
                    SubActionWithDelay(modID, msInterval * i);
                }
            }
            int refreshDelay = allMissings.Count * msInterval + 1000;
            RefreshViewWithDelay(refreshDelay);
        }
        void SubToLoneMissing(string modID)
        {
            SubToMissingMod(modID);
            RefreshViewWithDelay(2000);
        }


        async Task RefreshViewWithDelay(int ms)
        {
            await Task.Delay(ms);
            if(RefreshViewBtn.TryGetComponent<KButton>(out var button))
            {
                button.SignalClick(KKeyCode.None);
            }
            if(onClose!=null)
                onClose.Invoke(); 
        }

        async Task SubActionWithDelay(string modID, int ms, bool unsubscribe = false)
        {
            await Task.Delay(ms);
            if(unsubscribe)
                UnSubMod(modID);
            else
                SubToMissingMod(modID);
            //SgtLogger.log(modID + "<- Subbed!");
        }


        void MissingModWorkshopHandler(KMod.Label singleMod) => MissingModWorkshopHandler(new List<KMod.Label> { singleMod });
        void MissingModWorkshopHandler(List<KMod.Label> missingMods)
        {
            bool isList = missingMods.Count > 1;

            if(isList)
            {
                KMod.Manager.Dialog(Global.Instance.globalCanvas,
                WORKSHOPACTIONS.TITLE,
                string.Format(WORKSHOPACTIONS.INFOLIST, missingMods.Count),
                WORKSHOPACTIONS.SUBLIST,
                () =>
                {
                    SubActionToAllMissing(missingMods);
                },
                global::STRINGS.UI.FRONTEND.NEWGAMESETTINGS.BUTTONS.CANCEL,
                () => { });
            }
            else
            {
                KMod.Manager.Dialog(Global.Instance.globalCanvas,
                WORKSHOPACTIONS.TITLE,
                WORKSHOPACTIONS.INFO,
                WORKSHOPACTIONS.SUB,
                () =>
                {
                    SubToLoneMissing(missingMods[0].id);
                },
                WORKSHOPACTIONS.VISIT,
                () =>
                {
                    OpenMissingMod(missingMods[0].id);
                },
                global::STRINGS.UI.FRONTEND.NEWGAMESETTINGS.BUTTONS.CANCEL,
                () => { });
            }
            

        }
        static void UnSubMod(string modId)
        {
            SteamUGC.UnsubscribeItem(new PublishedFileId_t(ulong.Parse(modId)));
        }

        static void SubToMissingMod(string modId)
        {
            SteamUGC.SubscribeItem(new PublishedFileId_t(ulong.Parse(modId)));
        }

        void OpenMissingMod(string modId)
        {
            App.OpenWebURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + modId);
        }

        void MarkEntryAsMissing(Transform entry, KMod.Label mod)
        {
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
                () =>
                {
                    MissingModWorkshopHandler(mod);
                }, true);

            var bgColorImage = infoBt.bgImage;
            bgColorImage.colorStyleSetting = missingModColorStyle;
            bgColorImage.ApplyColorStyleSetting();

            TryChangeText(entry, "ManageButton/Text", "<!> Missing");
        }


        void RebuildList(List<KMod.Label> modsForSingleView = null)
        {

            if (Entries.Count > 0)
            {
                foreach (var entry in Entries)
                    UnityEngine.Object.Destroy(entry);
                Entries.Clear();
            }

            if (IsMissingModsOnly)
            {
                //SgtLogger.log("AddedMissingToView");

                foreach (var mod in MissingModsList)
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                    TryChangeText(entry, "Title", mod.title);
                    TryChangeText(entry, "Version", "internal mod Version: " + mod.version.ToString());

                    MarkEntryAsMissing(entry, mod);

                    Entries.Add(entry.gameObject);
                }
                AddActionToButton(
                    transform,
                    "Panel/DetailsView/ToggleAllButton",
                    () => RebuildList(MissingModsList),
                    true);

                AddActionToButton(
                    WorkShopSubBtn.transform,
                    "",
                    () => MissingModWorkshopHandler(MissingModsList)
                    , true);
                return;
            }

            if (ViewSingleEntry)
            {

                if (modsForSingleView == null || modsForSingleView.Count == 0)
                    return;
                KMod.Manager modManager = Global.Instance.modManager;

                var allMods = modManager.mods.Select(mod => mod.label).ToList();
                MissingModsList = modsForSingleView.Except(allMods, new ModlistManager.ModDifferencesByIdComparer()).ToList();


                foreach (var mod in modsForSingleView)
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                    TryChangeText(entry, "Title", mod.title);
                    TryChangeText(entry, "Version", "internal mod Version: " + mod.version.ToString());

                    if (MissingModsList.Contains(mod, new ModlistManager.ModDifferencesByIdComparer()))
                    {
                        MarkEntryAsMissing(entry, mod);
                    }
                    else
                    {
                        FindAndDisable(entry, "ManageButton");
                    }
                    Entries.Add(entry.gameObject);

                }
                TryFindComponent<KButton>(SyncButton.transform).isInteractable = modsForSingleView.Count > MissingModsList.Count;
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

                AddActionToButton(
                    transform,
                    "Panel/DetailsView/ToggleAllButton",
                    () => RebuildList(modsForSingleView),
                    true);

                TryFindComponent<KButton>(WorkShopSubBtn.transform).isInteractable = MissingModsList.Count > 0;

                AddActionToButton(
                    WorkShopSubBtn.transform,
                    "",
                    () => MissingModWorkshopHandler(MissingModsList)
                    , true);
            }
            else
            {
                //SgtLogger.log("All View");
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
            ModlistManager.Instance.SyncFromModListWithoutAutoLoad(modsInList,
                (() =>
                {
                    this.Deactivate();
                    if (ParentWindow != null)
                        ParentWindow.Deactivate();
                }
                ));
        }
        internal void InstantiateParams(KeyValuePair<string, SaveGameModList> exportedList, KModalScreen parent)
        {
            Title = exportedList.Key;
            Mods = exportedList.Value;
            ParentWindow = parent;
            onClose = null;
        }
        public void InstantiateMissing(List<KMod.Label> missingMods, System.Action onclose =null)
        {
            Title = STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSTITLE;
            IsMissingModsOnly = true;
            MissingModsList.Clear();
            MissingModsList.AddRange(missingMods);
            onClose = onclose;
        }
        System.Action onClose = null;
    }
}
