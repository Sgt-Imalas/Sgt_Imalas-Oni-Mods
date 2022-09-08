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

        string Title = string.Empty;
        SaveGameModList Mods;
        List<GameObject> Entries = new List<GameObject>();

        Transform EntryPrefab;
        Transform InsertLocation;

        GameObject ReturnButton;
        //GameObject SyncButton;

        internal void SetAdditionalButtons(bool active)
        {
            ViewSingleEntry = active;

            if(ReturnButton!=null)
                ReturnButton.SetActive(active);
            //SyncButton.SetActive(active);
            if (!active) RebuildList();
        }

        protected override void OnActivate()
        {

            var SingleFileModlists = this.transform;
            ///Set Title of Mod Sync Screen.
            TryChangeText(
                SingleFileModlists, 
                "Panel/Title/Title", 
                Mods.IsModPack ? 
                    string.Format(TITLE, Title) 
                    : string.Format(TITLESAVEGAMELIST, Title));

            var DetailsView = SingleFileModlists.Find("Panel/DetailsView").gameObject;

            ///Disable unneeded buttons
            FindAndDisable(SingleFileModlists, "Panel/DetailsView/ToggleAllButton");
            FindAndDisable(SingleFileModlists, "Panel/DetailsView/WorkshopButton");


            /////Return Button for single list view
            //ReturnButton = transform.Find("Panel/DetailsView/WorkshopButton").gameObject;

            TryChangeText(
                SingleFileModlists,
                "Panel/DetailsView/CloseButton/Text",
                RETURN);

            ///Make Close functional
            AddActionToButton(
                SingleFileModlists,
                "Panel/DetailsView/CloseButton",
                new System.Action(this.Deactivate),
                true);

            ///get and edit entry prefab
            EntryPrefab = SingleFileModlists.Find("Panel/ListView/Files/Viewport/Entry");
            FindAndDisable(EntryPrefab, "DragReorderIndicator");
            FindAndDisable(EntryPrefab, "EnabledToggle");

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

            if (ViewSingleEntry)
            {
                if (modsForSingleView == null)
                    return;
                KMod.Manager modManager = Global.Instance.modManager;

                var allMods = modManager.mods.Select(mod => mod.label).ToList();
                var MissingMods = modsForSingleView.Except(allMods, new ModlistManager.ModDifferencesByIdComparer()).ToList();

                var missingModColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
                missingModColorStyle.inactiveColor = new Color(0.7f, 0.25f, 0.25f);
                missingModColorStyle.activeColor = new Color(0.7f, 0.25f, 0.25f);
                missingModColorStyle.disabledColor = new Color(0.7f, 0.25f, 0.25f);

                foreach (var mod in modsForSingleView)
                {
                    var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                    TryChangeText(entry, "Title", mod.title);
                    TryChangeText(entry, "Version", "internal mod Version: " + mod.version.ToString());
                    if (MissingMods.Contains(mod))
                    {
                        var bgColor = TryFindComponent<KImage>(entry,"BG");
                        bgColor.defaultState = KImage.ColorSelector.Disabled;
                        bgColor.ColorState = KImage.ColorSelector.Disabled;

                        var infoBt = TryFindComponent<KButton>(entry, "ManageButton");
                        infoBt.isInteractable = false;

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

            }
            else
            {
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
            }

            ///Extra if for buttons; Readability
            if (ViewSingleEntry)
            {
                AddActionToButton(
                    transform,
                    "Panel/DetailsView/CloseButton",
                    () => SetAdditionalButtons(false),
                    true);
            }
            else
            {
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
            
        }



        internal void InstantiateParams(KeyValuePair<string, SaveGameModList> exportedList)
        {
            Title = exportedList.Key;
            Mods = exportedList.Value;
        }
    }
}
