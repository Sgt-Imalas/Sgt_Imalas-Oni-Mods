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
        //List<string> MissingModsIdList = new List<string>();
        string Title = string.Empty;
        SaveGameModList Mods;
        List<GameObject> Entries = new List<GameObject>();

        Transform EntryPrefab;
        Transform InsertLocation;

        //GameObject ReturnButton;
        GameObject SyncButton;
        GameObject AddSyncButton;
        GameObject RefreshViewBtn;
        GameObject CopyToClipboardBtn;
        List<KMod.Label> currentMods = null;
        GameObject WorkShopSubBtn;
        KModalScreen ParentWindow;


        public void SetAdditionalButtons(bool active)
        {
            ViewSingleEntry = active;

            if (SyncButton != null)
                SyncButton.SetActive(active);
            if (AddSyncButton != null)
                AddSyncButton.SetActive(active);
            if (RefreshViewBtn != null)
                RefreshViewBtn.SetActive(active);
            if (WorkShopSubBtn != null)
                WorkShopSubBtn.SetActive(active || IsMissingModsOnly);

            if (CopyToClipboardBtn != null)
                CopyToClipboardBtn.SetActive(active && ViewSingleEntry);
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
                    MODSYNCING.MISSINGMODSTITLE
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
            RefreshViewBtn.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
            RefreshViewBtn.SetActive(false);
            RefreshViewBtn.TryGetComponent<KButton>(out var refreshviewbuttonforstyle);
            SyncButton = transform.Find("Panel/DetailsView/WorkshopButton").gameObject;
            AddSimpleTooltipToObject(SyncButton.transform, SYNCLISTTOOLTIP);

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

            AddSyncButton = TryInsertNamedCopy(DetailsView.transform, "ToggleAllButton", "AddSyncButton").gameObject;
            AddSyncButton.transform.SetAsLastSibling();
            AddSyncButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);

            SingleFileModlists.Find("Panel/DetailsView/CloseButton").SetAsLastSibling();

            TryChangeText(AddSyncButton.transform, "Text", SYNCLISTADDITIVE);
            AddSimpleTooltipToObject(AddSyncButton.transform, SYNCLISTADDITIVETOOLTIP);
            
            transform.Find("Panel/Title/CloseButton").transform.SetAsLastSibling();

            WorkShopSubBtn = TryInsertNamedCopy(DetailsView.transform, "ToggleAllButton", "SubAllButton").gameObject;
            WorkShopSubBtn.transform.SetAsFirstSibling();
            TryChangeText(WorkShopSubBtn.transform, "Text", WORKSHOPACTIONS.BUTTON);

            ///get and edit entry prefab
            EntryPrefab = SingleFileModlists.Find("Panel/ListView/Files/Viewport/Entry");
            FindAndDisable(EntryPrefab, "DragReorderIndicator");
            FindAndDisable(EntryPrefab, "EnabledToggle");
            FindAndRemove<DragMe>(EntryPrefab);
            InsertLocation = SingleFileModlists.Find("Panel/ListView/Files/Viewport/Content");
            CopyToClipboardBtn = ModAssets.AddCopyButton(DetailsView, () => ModAssets.PutToClipboard(currentMods, false), () => ModAssets.PutToClipboard(currentMods, true), refreshviewbuttonforstyle.bgImage.colorStyleSetting);
            SetAdditionalButtons(false);

            //UIUtils.ListAllChildren(SingleFileModlists);
        }


        void SubActionToAllMissing(List<string> allMissings)
        {
            if (allMissings.Count == 0)
                return;
            int msInterval = 1000;

            SgtLogger.log("SubToAllCalled , missing mods count: " + allMissings.Count);
            for (int i = 0; i < allMissings.Count; ++i)
            {
                var mod = allMissings[i];
                SgtLogger.log("nr: " + i+ ", mod: "+ mod);
                string modID = mod.Replace(".Steam",string.Empty);
                if (true
                    //mod.distribution_platform == KMod.Label.DistributionPlatform.Steam
                    )
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


        void MissingModWorkshopHandler(string singleMod) => MissingModWorkshopHandler(new List<string> { singleMod });
        void MissingModWorkshopHandler(List<string> missingMods)
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
                    SubToLoneMissing(missingMods[0]);
                },
                WORKSHOPACTIONS.VISIT,
                () =>
                {
                    OpenMissingMod(missingMods[0]);
                },
                global::STRINGS.UI.FRONTEND.NEWGAMESETTINGS.BUTTONS.CANCEL,
                () => { });
            }
            

        }
        static void UnSubMod(string modId)
        {
            modId = modId.Replace(".Steam", string.Empty);
            if (ulong.TryParse(modId, out var mod))
                SteamUGC.UnsubscribeItem(new PublishedFileId_t(mod));
            else
                SgtLogger.warning("unsubbing from " + modId + " failed!");
        }

        static void SubToMissingMod(string modId)
        {
            modId = modId.Replace(".Steam", string.Empty);

            if (ulong.TryParse(modId, out var mod))
                SteamUGC.SubscribeItem(new PublishedFileId_t(mod));
            else
                SgtLogger.warning("subscribing to " + modId + " failed!");
        }

        void OpenMissingMod(string modId)
        {
            modId = modId.Replace(".Steam", string.Empty);

            if (ModAssets.UseSteamOverlay && SteamUtils.IsOverlayEnabled())
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/sharedfiles/filedetails/?id=" + modId);
            else
                App.OpenWebURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + modId);
        }

        void MarkEntryAsMissing(Transform entry, string modId)
        {
            var bgColor = TryFindComponent<KImage>(entry, "BG");
            bgColor.defaultState = KImage.ColorSelector.Disabled;
            bgColor.ColorState = KImage.ColorSelector.Disabled;

            var infoBt = TryFindComponent<KButton>(entry, "ManageButton");
            bool isSteamMod = modId.Contains(".Steam");// mod.distribution_platform == KMod.Label.DistributionPlatform.Steam;
            infoBt.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(isSteamMod ? WORKSHOPFINDTOOLTIP : NOSTEAMMOD);
            infoBt.isInteractable = isSteamMod;
            AddActionToButton(
                infoBt.transform,
                "",
                () =>
                {
                    MissingModWorkshopHandler(modId);
                }, true);

            var bgColorImage = infoBt.bgImage;
            bgColorImage.colorStyleSetting = missingModColorStyle;
            bgColorImage.ApplyColorStyleSetting();

            TryChangeText(entry, "ManageButton/Text", "<!> Missing");
        }

        void QueueGetMissingModName(Transform entry,string mod)
        {
            SteamInfoQuery.AddModIdToQuery(mod,
                            (name)
                            =>
                            {
                                var platform = KMod.Label.DistributionPlatform.Local;
                                string ID = mod;
                                if (ulong.TryParse(mod.Replace(".Steam", string.Empty), out var modId))
                                {
                                    ID = modId.ToString();
                                    platform = KMod.Label.DistributionPlatform.Steam;
                                }

                                var tmpLabel = new KMod.Label()
                                {
                                    id = modId.ToString(),
                                    distribution_platform = platform,
                                    title = name,
                                    version = 404
                                };
                                TryChangeText(entry, "Title", name);
                                TryChangeText(entry, "Version", "404");
                            }
                          );
        }

        void RebuildList(List<string> modsForSingleView = null)
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

                foreach (var mod in ModlistManager.MissingModsPublic)
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                    TryChangeText(entry, "Title", mod);
                    //TryChangeText(entry, "Version", "internal mod Version: " + mod..ToString());

                    MarkEntryAsMissing(entry, mod);
                    QueueGetMissingModName(entry,mod);
                    Entries.Add(entry.gameObject);
                }
                SteamInfoQuery.InstantiateMissingModQuery();

                AddActionToButton(
                    transform,
                    "Panel/DetailsView/ToggleAllButton",
                    () => RebuildList(ModlistManager.MissingModsPublic.ToList()),
                    true);

                AddActionToButton(
                    WorkShopSubBtn.transform,
                    "",
                    () => MissingModWorkshopHandler(ModlistManager.MissingModsPublic.ToList())
                    , true);
                return;
            }
            currentMods = new List<KMod.Label>();
            if (ViewSingleEntry)
            {

                if (modsForSingleView == null || modsForSingleView.Count == 0)
                    return;
                KMod.Manager modManager = Global.Instance.modManager;

                var allMods = modManager.mods.Select(mod => mod.label).ToList();

                ModlistManager.Instance.AssignModDifferences(modsForSingleView);
                //MissingModsList = modsForSingleView.Except(allMods, new ModlistManager.ModDifferencesByIdComparer()).ToList();

                foreach (var mod in modsForSingleView)
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;

                    if (ModlistManager.MissingModsPublic.Contains(mod))
                    {
                        TryChangeText(entry, "Title", mod);
                        TryChangeText(entry, "Version", "404");
                        MarkEntryAsMissing(entry, mod);
                        QueueGetMissingModName(entry, mod);
                    }
                    else
                    {
                       var locMod = modManager.mods.First(mode => mode.label.defaultStaticID == mod);
                        if(locMod != null)
                        {
                            currentMods.Add(locMod.label);
                            TryChangeText(entry, "Title", locMod.label.title);
                            TryChangeText(entry, "Version", "internal mod Version: " + locMod.label.version.ToString());
                            FindAndDisable(entry, "ManageButton");
                        }

                    }
                    Entries.Add(entry.gameObject);

                }
                SteamInfoQuery.InstantiateMissingModQuery();

                TryFindComponent<KButton>(SyncButton.transform).isInteractable = modsForSingleView.Count > ModlistManager.MissingModsPublic.Count;
                TryFindComponent<KButton>(AddSyncButton.transform).isInteractable = modsForSingleView.Count > ModlistManager.MissingModsPublic.Count;

                AddActionToButton(
                       SyncButton.transform,
                       "",
                       () => SyncSingleList(modsForSingleView),
                       true);

                AddActionToButton(
                       AddSyncButton.transform,
                       "",
                        () => AddListMods(modsForSingleView)
                       , true);


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

                TryFindComponent<KButton>(WorkShopSubBtn.transform).isInteractable = ModlistManager.MissingModsPublic.Count > 0;

                AddActionToButton(
                    WorkShopSubBtn.transform,
                    "",
                    () => MissingModWorkshopHandler(ModlistManager.MissingModsPublic.ToList())
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
                         () => ViewSingleList(mod.Value.Select(i => i.defaultStaticID).ToList())
                        , true);

                    var syncbt = TryInsertNamedCopy(entry, "ManageButton", "SyncButton");
                    TryChangeText(syncbt.transform, "Text", SYNCLIST);
                    UIUtils.AddSimpleTooltipToObject(syncbt.transform, SYNCLISTTOOLTIP);


                    AddActionToButton(
                        syncbt,
                        "",
                         () => SyncSingleList(mod.Value.Select(i => i.defaultStaticID).ToList()));


                    var syncbtAdditive = TryInsertNamedCopy(entry, "ManageButton", "SyncButtonAdditive");
                    TryChangeText(syncbtAdditive.transform, "Text", SYNCLISTADDITIVE);
                    UIUtils.AddSimpleTooltipToObject(syncbtAdditive.transform, SYNCLISTADDITIVETOOLTIP);


                    AddActionToButton(
                        syncbtAdditive,
                        "",
                        () => AddListMods(mod.Value.Select(i=>i.defaultStaticID).ToList()));


                    Entries.Add(entry.gameObject);
                }

                AddActionToButton(
                transform,
                "Panel/DetailsView/CloseButton",
                new System.Action(this.Deactivate),
                true);
            }
        }

        internal void ViewSingleList(List<string> modsInList)
        {
            SetAdditionalButtons(true);
            RebuildList(modsInList);
        }

        internal void AddListMods(List<string> modsInList)
        {
            ModlistManager.Instance.SyncFromModListWithoutAutoLoad(modsInList,
                (() =>
                {
                    this.Deactivate();
                    if (ParentWindow != null)
                        ParentWindow.Deactivate();
                }
                ),true);
        }
        internal void SyncSingleList(List<string> modsInList)
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
        public void InstantiateMissing(List<string> missingMods, System.Action onclose =null)
        {
            //missingMods.ForEach(i => SgtLogger.l(i, "missing mod:"));

            Title = STRINGS.UI.FRONTEND.MODSYNCING.MISSINGMODSTITLE;
            IsMissingModsOnly = true;
            //MissingModsIdList.Clear();
            //MissingModsIdList.AddRange(missingMods);
            onClose = onclose;
        }
        System.Action onClose = null;
    }
}
