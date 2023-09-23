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
    internal class CrewAssignmentSidescreen : KScreen
    {
        GameObject OwnDupeContainer;

        GameObject TargetDupeContainer;

        GameObject OwnDupePreset;
        GameObject TargetDupePreset;

        Dictionary<MinionIdentity, GameObject> OwnDupePresets = new Dictionary<MinionIdentity, GameObject>();
        Dictionary<MinionIdentity, GameObject> TargetDupePresets = new Dictionary<MinionIdentity, GameObject>();

        AssignmentGroupController MyAssignmentGroup;
        AssignmentGroupController TargetAssignmentGroup;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Init();
        }

        void Init()
        {
            //UIUtils.ListAllChildrenPath(this.transform);
            OwnDupeContainer = transform.Find("OwnDupesContainer/ScrollRectContainer").gameObject;
            TargetDupeContainer = transform.Find("TargetDupesContainer/ScrollRectContainer").gameObject;

            OwnDupePreset = transform.Find("OwnDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;
            TargetDupePreset = transform.Find("TargetDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;
            

            OwnDupePreset.SetActive(false);
            TargetDupePreset.SetActive(false);

            foreach (MinionIdentity dupe in Components.LiveMinionIdentities)
            {
                AddDupeEntry(dupe, true);
                AddDupeEntry(dupe, false);
            }

            Components.LiveMinionIdentities.OnAdd += (minion) =>
            {
                AddDupeEntry(minion, true);
                AddDupeEntry(minion, false);
            };
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            if (show)
            {
                foreach (var dupe in OwnDupePresets)
                {
                    UpdateName(dupe.Key, dupe.Value);
                }
                foreach (var dupe in TargetDupePresets)
                {
                    UpdateName(dupe.Key, dupe.Value);
                }
            }
        }

        public void UpdateForConnection(AssignmentGroupController myAssignmentGroup, int MyWorldID, AssignmentGroupController targetAssignmentGroup, int TargetWorldId)
        {
            MyAssignmentGroup = myAssignmentGroup;
            TargetAssignmentGroup = targetAssignmentGroup;


            var AllDupes = Components.LiveMinionIdentities.GetWorldItems(MyWorldID).Concat(Components.LiveMinionIdentities.GetWorldItems(TargetWorldId)).ToList();


            foreach (var kvp in OwnDupePresets)
            {
                kvp.Value.SetActive(false);
            }
            foreach (var kvp in TargetDupePresets)
            {
                kvp.Value.SetActive(false);
            }

            foreach(var Duplicant in AllDupes)
            {
                if (!OwnDupePresets.ContainsKey(Duplicant))
                {
                    AddDupeEntry(Duplicant, true);
                    AddDupeEntry(Duplicant, false);
                }
                if(MyAssignmentGroup != null )
                {
                    if (MyAssignmentGroup.CheckMinionIsMember(Duplicant.assignableProxy.Get()))
                    {
                        TargetDupePresets[Duplicant].SetActive(false);
                        OwnDupePresets[Duplicant].SetActive(true);
                    }
                    else
                    {
                        TargetDupePresets[Duplicant].SetActive(true);
                        OwnDupePresets[Duplicant].SetActive(false);
                    }
                    continue;
                }
                if (TargetAssignmentGroup != null)
                {
                    if (TargetAssignmentGroup.CheckMinionIsMember(Duplicant.assignableProxy.Get()))
                    {
                        TargetDupePresets[Duplicant].SetActive(true);
                        OwnDupePresets[Duplicant].SetActive(false);
                    }
                    else
                    {
                        TargetDupePresets[Duplicant].SetActive(false);
                        OwnDupePresets[Duplicant].SetActive(true);
                    }
                    continue;
                }
            }


            
        }


        void UpdateName(MinionIdentity identity, GameObject target)
        {
            UIUtils.TryChangeText(target.transform,("TitleText"),identity.name);
        }

        void AssignDupeFromTo(MinionIdentity minion, AssignmentGroupController source, AssignmentGroupController target)
        {

            var minionIdentity = minion.assignableProxy.Get();
            if (source != null && Game.Instance.assignmentManager.assignment_groups[source.AssignmentGroupID].HasMember(minionIdentity))
            {
                SgtLogger.l("REMOVING DOOP " + source.name);
                Game.Instance.assignmentManager.assignment_groups[source.AssignmentGroupID].RemoveMember(minionIdentity);
            }
            if (target != null && !Game.Instance.assignmentManager.assignment_groups[target.AssignmentGroupID].HasMember(minionIdentity))
            {
                SgtLogger.l("Adding DOOP " + target.name);
                Game.Instance.assignmentManager.assignment_groups[target.AssignmentGroupID].AddMember(minionIdentity);
            }
        }

        void AddDupeEntry(MinionIdentity minionIdentity, bool ownTrueTargetFalse)
        {
            var DoopEntry = Util.KInstantiateUI(
                ownTrueTargetFalse ? OwnDupePreset : TargetDupePreset,
                ownTrueTargetFalse ? OwnDupeContainer : TargetDupeContainer,true);
            DoopEntry.transform.Find("DupeHeadContainer/Image").GetComponent<Image>().sprite = Db.Get().Personalities.Get(minionIdentity.personalityResourceId).GetMiniIcon();
            UpdateName(minionIdentity, DoopEntry);
            UIUtils.TryChangeText(DoopEntry.transform, "AssignButtonContainer/AssignButton/Label", STRINGS.UI.DOCKINGTRANSFERSCREEN.ASSIGNTOOTHERBUTTONTEXT);
            var btn = DoopEntry.transform.Find("AssignButtonContainer/AssignButton").gameObject.AddOrGet<FButton>();

            if (ownTrueTargetFalse)
            {
                btn.OnClick += () =>
                {
                    AssignDupeFromTo(minionIdentity, MyAssignmentGroup, TargetAssignmentGroup);
                    TargetDupePresets[minionIdentity].SetActive(true);
                    OwnDupePresets[minionIdentity].SetActive(false);
                };
                OwnDupePresets.Add(minionIdentity, DoopEntry);
            }
            else
            {
                btn.OnClick += () =>
                {
                    AssignDupeFromTo(minionIdentity, TargetAssignmentGroup, MyAssignmentGroup);
                    TargetDupePresets[minionIdentity].SetActive(false);
                    OwnDupePresets[minionIdentity].SetActive(true);

                };
                TargetDupePresets.Add(minionIdentity, DoopEntry);
            }

        }
    }
}
