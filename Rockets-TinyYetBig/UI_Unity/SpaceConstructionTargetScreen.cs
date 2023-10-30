using Database;
using Rockets_TinyYetBig.SpaceStations.Construction;
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
        private class ProjectListUIEntry : KMonoBehaviour
        {
            public FButton StartProjectBtn;
            public LocText ProjectLabel;
            public LocText ProjectDesc;
            public Image ProjectPreview;

            public ConstructionProjectAssembly Reference;
            GameObject PartContainer;
            GameObject PartListEntryPrefab;

            public void Init(ConstructionProjectAssembly referencedProject)
            {
                Reference = referencedProject;
                StartProjectBtn = transform.Find("Row1/ConstructBtn").gameObject.AddOrGet<FButton>();
                ProjectPreview = transform.Find("Row1/SpaceCraftIcon").GetComponent<Image>();
                ProjectLabel = transform.Find("Row1/TitleText").GetComponent<LocText>();
                ProjectDesc = transform.Find("Desc/TitleText").GetComponent<LocText>();
                PartContainer = gameObject;
                PartListEntryPrefab = transform.Find("CostContainer").gameObject;
                PartListEntryPrefab.gameObject.SetActive(false);


                ProjectLabel.SetText(referencedProject.ProjectName);
                ProjectDesc.SetText(referencedProject.ProjectDescription);
                ProjectPreview.sprite = referencedProject.PreviewSprite;

                Dictionary<string, int> partCount = new Dictionary<string, int>();
                foreach(var part in referencedProject.Parts)
                {
                    if(partCount.ContainsKey(part.name)) 
                        partCount[part.name]++;
                    else
                    {
                        partCount[part.name] = 1;
                    }
                }

                foreach(var item in partCount)
                {
                    var entry = Util.KInstantiateUI(PartListEntryPrefab, PartContainer, true);
                    entry.transform.Find("TitleText").GetComponent<LocText>().SetText(item.Key);
                    entry.transform.Find("PartCount").GetComponent<LocText>().SetText("x"+item.Value.ToString());
                    
                }
                StartProjectBtn.OnClick += () =>
                {

                };
            }
        }


        GameObject ProjectsContainer;
        GameObject ProjectPrefab;

        Dictionary<ConstructionProjectAssembly, ProjectListUIEntry> Project = new Dictionary<ConstructionProjectAssembly, ProjectListUIEntry>();

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Init();
        }

        void Init()
        {
            UIUtils.ListAllChildrenPath(this.transform);


            transform.Find("Title").gameObject.SetActive(false);
            //UIUtils.ListAllChildrenPath(this.transform);
            ProjectsContainer = transform.Find("ProjectsContainer/ScrollRectContainer").gameObject;
            ProjectPrefab = transform.Find("ProjectsContainer/ScrollRectContainer/PartContainerPrefab").gameObject;

            foreach(var project in ConstructionProjects.AllProjects)
            {
                var entry = Util.KInstantiateUI(ProjectPrefab, ProjectsContainer, true);
                var logic = entry.AddComponent<ProjectListUIEntry>();
                logic.Init(project);
            }
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
