using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static UtilLibs.UIUtils;

namespace SetStartDupes
{
    class StatSelector : KModalScreen
    {
        public HoldMyReferences references;
        string Title = string.Empty;
        List<GameObject> Entries = new List<GameObject>();

        Transform EntryPrefab;
        Transform InsertLocation;

        //GameObject ReturnButton;
        GameObject SyncButton;
        KModalScreen ParentWindow;

        protected override void OnActivate()
        {

            var SingleFileModlists = this.transform;
            ///Set Title of Mod Sync Screen.
            TryChangeText(
                    SingleFileModlists,
                    "Panel/Title/Title",
                    "Stat Selector");
            

            var DetailsView = SingleFileModlists.Find("Panel/DetailsView").gameObject;

            ///Disable unneeded buttons
            FindAndDisable(SingleFileModlists, "Panel/DetailsView/ToggleAllButton");
            FindAndDisable(SingleFileModlists, "Panel/DetailsView/WorkshopButton");



            /////WorkshopButton Button for single list view sync
            //SyncButton = transform.Find("Panel/DetailsView/WorkshopButton").gameObject;
            //SyncButton.SetActive(false);

            //TryChangeText(
            //    SingleFileModlists,
            //    "Panel/DetailsView/WorkshopButton/Text",
            //    SYNCLIST);


            TryChangeText(
                SingleFileModlists,
                "Panel/DetailsView/CloseButton/Text",
                "CANCEL");

            ///Make Close(s) functional
            AddActionToButton(
                SingleFileModlists,
                "Panel/DetailsView/CloseButton",
                new System.Action(this.DeleteObject),
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
        }

        public void Build(HoldMyReferences refs)
        {
            references = refs;
            RebuildList();
        }

        void RebuildList()
        {

            if (Entries.Count > 0) 
            { 
                foreach (var entry in Entries)
                UnityEngine.Object.Destroy(entry);
                Entries.Clear();
            }
            
            foreach (var mod in DUPLICANTSTATS.ALL_ATTRIBUTES)
            {
                var entry = Util.KInstantiateUI(EntryPrefab.gameObject, InsertLocation.gameObject, true).transform;
                TryChangeText(entry, "Title", mod);
                FindAndDisable(entry, "Version");
                
                var infoBt = TryFindComponent<KButton>(entry, "ManageButton");       
                infoBt.isInteractable = !references.HasCurrentSkill(mod);
                        AddActionToButton(
                            infoBt.transform, 
                            "",
                            () => {  }, true);
                TryChangeText(entry, "ManageButton/Text", "Select");
                Entries.Add(entry.gameObject);
            }
        }
    }
}
