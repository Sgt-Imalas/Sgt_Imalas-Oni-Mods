using Database;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using static STRINGS.UI.UISIDESCREENS.AUTOPLUMBERSIDESCREEN.BUTTONS;

namespace Rockets_TinyYetBig.UI_Unity
{
    internal class SpaceConstructionTargetScreen : KScreen
    {

        GameObject TargetDupeContainer;


        AssignmentGroupController MyAssignmentGroup;
        AssignmentGroupController TargetAssignmentGroup;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Init();
        }

        void Init()
        {
            UIUtils.ListAllChildrenPath(this.transform);
            //UIUtils.ListAllChildrenPath(this.transform);
            //OwnDupeContainer = transform.Find("OwnDupesContainer/ScrollRectContainer").gameObject;
            //TargetDupeContainer = transform.Find("TargetDupesContainer/ScrollRectContainer").gameObject;

            //OwnDupePreset = transform.Find("OwnDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;
            //TargetDupePreset = transform.Find("TargetDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;


            //OwnDupePreset.SetActive(false);
            //TargetDupePreset.SetActive(false);

            //foreach (MinionIdentity dupe in Components.LiveMinionIdentities)
            //{
            //    AddDupeEntry(dupe, true);
            //    AddDupeEntry(dupe, false);
            //}

            //Components.LiveMinionIdentities.OnAdd += (minion) =>
            //{
            //    AddDupeEntry(minion, true);
            //    AddDupeEntry(minion, false);
            //};
        }

        public override void OnShow(bool show)
        {

            base.OnShow(show);
            if (show)
            {
                //foreach (var dupe in OwnDupePresets)
                //{
                //    UpdateName(dupe.Key, dupe.Value);
                //}
                //foreach (var dupe in TargetDupePresets)
                //{
                //    UpdateName(dupe.Key, dupe.Value);
                //}
            }
        }



        void AddDupeEntry(MinionIdentity minionIdentity, bool ownTrueTargetFalse)
        {
            //var DoopEntry = Util.KInstantiateUI(
            //    ownTrueTargetFalse ? OwnDupePreset : TargetDupePreset,
            //    ownTrueTargetFalse ? OwnDupeContainer : TargetDupeContainer,true);
            //DoopEntry.transform.Find("DupeHeadContainer/Image").GetComponent<Image>().sprite = Db.Get().Personalities.Get(minionIdentity.personalityResourceId).GetMiniIcon();
            //UpdateName(minionIdentity, DoopEntry);
            //UIUtils.TryChangeText(DoopEntry.transform, "AssignButtonContainer/AssignButton/Label", STRINGS.UI.DOCKINGTRANSFERSCREEN.ASSIGNTOOTHERBUTTONTEXT);
            //var btn = DoopEntry.transform.Find("AssignButtonContainer/AssignButton").gameObject.AddOrGet<FButton>();

            //if (ownTrueTargetFalse)
            //{
            //    btn.OnClick += () =>
            //    {
            //        AssignDupeFromTo(minionIdentity, MyAssignmentGroup, TargetAssignmentGroup);
            //        TargetDupePresets[minionIdentity].SetActive(true);
            //        OwnDupePresets[minionIdentity].SetActive(false);
            //    };
            //    OwnDupePresets.Add(minionIdentity, DoopEntry);
            //}
            //else
            //{
            //    btn.OnClick += () =>
            //    {
            //        AssignDupeFromTo(minionIdentity, TargetAssignmentGroup, MyAssignmentGroup);
            //        TargetDupePresets[minionIdentity].SetActive(false);
            //        OwnDupePresets[minionIdentity].SetActive(true);

            //    };
            //    TargetDupePresets.Add(minionIdentity, DoopEntry);
            //}

        }
    }
}
