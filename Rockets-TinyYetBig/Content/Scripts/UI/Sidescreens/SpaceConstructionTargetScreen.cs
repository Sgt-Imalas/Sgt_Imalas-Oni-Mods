using Rockets_TinyYetBig.SpaceStations.Construction;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens
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
            AxialI Location;

            public void UpdateLocation(AxialI location)
            {
                Location = location;
            }

            public void Init(ConstructionProjectAssembly referencedProject)
            {
                // UIUtils.ListAllChildrenPath(transform);

                Reference = referencedProject;
                StartProjectBtn = transform.Find("Row1/ConstructBtn").gameObject.AddOrGet<FButton>();
                ProjectPreview = transform.Find("Row1/SpaceCraftIcon/Image").GetComponent<Image>();
                ProjectLabel = transform.Find("Row1/TitleText").GetComponent<LocText>();
                ProjectDesc = transform.Find("Desc/TitleText").GetComponent<LocText>();
                PartContainer = gameObject;
                PartListEntryPrefab = transform.Find("CostContainer").gameObject;
                PartListEntryPrefab.gameObject.SetActive(false);


                ProjectLabel.SetText(referencedProject.ProjectName);
                ProjectDesc.SetText(referencedProject.ProjectDescription);
                ProjectPreview.sprite = referencedProject.PreviewSprite != null ? referencedProject.PreviewSprite : Assets.GetSprite("unknown");
                SgtLogger.l("AAAA");

                Dictionary<string, int> partCount = new Dictionary<string, int>();
                foreach (PartProject part in referencedProject.Parts)
                {
                    if (partCount.ContainsKey(part.ResourceTag.ToString()))
                        partCount[part.ResourceTag.ToString()]++;
                    else
                    {
                        partCount[part.ResourceTag.ToString()] = 1;
                    }
                }

                foreach (var item in partCount)
                {
                    SgtLogger.l("EE");
                    var entry = Util.KInstantiateUI(PartListEntryPrefab, PartContainer, true);
                    SgtLogger.l("EEqqq");
                    entry.transform.Find("TitleText").GetComponent<LocText>().SetText(item.Key);
                    SgtLogger.l("EEaa");
                    entry.transform.Find("PartCount").GetComponent<LocText>().SetText("x" + item.Value.ToString());
                    SgtLogger.l("EEaaewe");

                }
                StartProjectBtn.OnClick += () =>
                {
                    InstantiateNewConstructionSite(Reference, Location);
                    DetailsScreen.Instance.ClearSecondarySideScreen();
                };
            }
        }
        public static GameObject InstantiateNewConstructionSite(ConstructionProjectAssembly project, AxialI TargetLocation)
        {
            Vector3 position = new Vector3(-1f, -1f, 0.0f);
            GameObject sat = Util.KInstantiate(Assets.GetPrefab(SpaceConstructionSiteConfig.ID), position);
            sat.name = project.ProjectName;
            var site = sat.GetComponent<SpaceConstructionSite>();
            site.Location = TargetLocation;
            site.SetItemName(project.ProjectName);


            sat.GetComponent<SpaceConstructable>().AssignProject(project);
            sat.SetActive(true);
            return sat;
        }


        GameObject ProjectsContainer;
        GameObject ProjectPrefab;

        Dictionary<ConstructionProjectAssembly, ProjectListUIEntry> Projects = new Dictionary<ConstructionProjectAssembly, ProjectListUIEntry>();

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Init();
        }

        void Init()
        {
            //UIUtils.ListAllChildrenPath(this.transform);


            transform.Find("Title").gameObject.SetActive(false);
            //UIUtils.ListAllChildrenPath(this.transform);
            ProjectsContainer = transform.Find("ProjectsContainer/ScrollRectContainer").gameObject;
            ProjectPrefab = transform.Find("ProjectsContainer/ScrollRectContainer/PartContainerPrefab").gameObject;
            ProjectPrefab.SetActive(false);
            foreach (var project in ConstructionProjects.AllProjects)
            {
                var entry = Util.KInstantiateUI(ProjectPrefab, ProjectsContainer, true);
                var logic = entry.AddComponent<ProjectListUIEntry>();
                logic.Init(project);
                Projects[project] = logic;
            }
        }

        internal void UpdatePositions(AxialI location)
        {
            foreach (var project in Projects.Values)
            {
                project.UpdateLocation(location);
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
