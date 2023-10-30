using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using Rockets_TinyYetBig.SpaceStations;
using Rockets_TinyYetBig.SpaceStations.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UI.FUI;
using UtilLibs.UIcmp;
using YamlDotNet.Core.Tokens;
using static ModInfo;
using static Rockets_TinyYetBig.STRINGS.UI;
using static Rockets_TinyYetBig.STRINGS.UI.DOCKINGSCREEN.OWNDUPESCONTAINER.SCROLLRECTCONTAINER.ITEMPREFAB.ROW2;
using static UnityEngine.GraphicsBuffer;
using static UtilLibs.UIUtils;

namespace Rockets_TinyYetBig.UI_Unity
{
    class SpaceConstructionSidescreen : SideScreenContent
    {

        private SpaceConstructionTargetScreen buildTargetScreen;
        public override void OnSpawn()
        {
            base.OnSpawn();
            ConnectReference();

        }

        protected Clustercraft targetCraft;
        protected SpaceStationBuilder stationBuilder;




        private class PartListUIEntry : KMonoBehaviour
        {
            public FButton ToggleConstruction;
            public LocText ToggleConstructionLabel;
            public LocText PartLabel;
            public Image PartPreview;

            public Slider ConstructionProgress;
            public LocText ConstructionProgressLabel;
            public PartProject Reference;


            public void Init(PartProject referencedProject)
            {
                Reference = referencedProject;
                ToggleConstruction = transform.Find("Row1/ConstructBtn").gameObject.AddOrGet<FButton>();
                ToggleConstructionLabel = transform.Find("Row1/ConstructBtn/Label").GetComponent<LocText>();
                PartLabel = transform.Find("Row1/TitleText").GetComponent<LocText>();
                PartPreview = transform.Find("Row1/SpaceCraftIcon").GetComponent<Image>();
                ConstructionProgress = transform.Find("CostContainer/Slider").GetComponent<Slider>();
                ConstructionProgressLabel = transform.Find("CostContainer/Slider/DurationText").GetComponent<LocText>();

            }
            public void Refresh()
            {

            }
        }

        protected GameObject PartEntryPrefab;
        protected GameObject PartListContainer;

        protected LocText headerLabel;
        protected FButton ToggleConstructionButton;
        protected LocText ToggleConstructionButtonLabel;
        protected Slider TotalProgressSlider;
        protected LocText TotalProgressLabel;
        protected LocText CurrentProjectTitleLabel;
        protected Image CurrentProjectPreviewImage;



        private List<int> refreshHandle = new List<int>();


        public override float GetSortKey() => 25f;
        public override bool IsValidForTarget(GameObject target)
        {
            return
                target.TryGetComponent<CraftModuleInterface>(out var ModuleInterface)
                && ModuleInterface.clusterModules.Any( module => module.Get().TryGetComponent<SpaceStationBuilder>(out _))               
                && ModuleInterface.m_clustercraft.status == Clustercraft.CraftStatus.InFlight
                && !RocketryUtils.IsRocketInFlight(ModuleInterface.m_clustercraft);
        }
        public override void ClearTarget()
        {
            foreach (int id in refreshHandle)
                targetCraft.Unsubscribe(id);
            refreshHandle.Clear();

            stationBuilder = null;
            targetCraft = null;
            base.ClearTarget();
        }
        public override void SetTarget(GameObject target)
        {

            if (target != null)
            {
                foreach (int id in refreshHandle)
                    target.Unsubscribe(id);
                refreshHandle.Clear();
            }

            //SgtLogger.l("setting Target");
            base.SetTarget(target);
            target.TryGetComponent(out targetCraft);
            targetCraft.ModuleInterface.clusterModules.First(module => module.Get().TryGetComponent<SpaceStationBuilder>(out stationBuilder));

            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new Action<object>(Refresh)));
            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new Action<object>(Refresh)));

            Refresh();
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //titleKey = "STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.TITLE";
        }

        void ConnectReference()
        {

            UIUtils.ListAllChildrenPath(this.transform);

            PartEntryPrefab = transform.Find("PartProgressContainer/ScrollRectContainer/PartContainerPrefab").gameObject;
            PartListContainer = transform.Find("PartProgressContainer/ScrollRectContainer").gameObject;
            PartEntryPrefab.SetActive(false);

            headerLabel = transform.Find("CurrentLocationHeader/TitleText").GetComponent<LocText>();
            
            TotalProgressSlider = transform.Find("ProjectHeader/CostContainer/Slider").GetComponent<Slider>();
            TotalProgressLabel = transform.Find("ProjectHeader/CostContainer/PartCount").GetComponent<LocText>(); 
            CurrentProjectTitleLabel = transform.Find("ProjectHeader/Row1/TitleText").GetComponent<LocText>();
            CurrentProjectPreviewImage = transform.Find("ProjectHeader/Row1/TargetIcon/Image").GetComponent<Image>();
            ToggleConstructionButton = transform.Find("ProjectHeader/Row1/CreateNew").gameObject.AddOrGet<FButton>(); 
            ToggleConstructionButtonLabel = transform.Find("ProjectHeader/Row1/CreateNew/Label").GetComponent<LocText>();

            ToggleConstructionButton.OnClick += () =>
            {
                ToggleBuildTargetScreen();
            };
        }


        void ToggleBuildTargetScreen()
        {
            if (buildTargetScreen == null)
            {
                buildTargetScreen = (SpaceConstructionTargetScreen)DetailsScreen.Instance.SetSecondarySideScreen(ModAssets.SpaceConstructionTargetSecondarySideScreen, CONSTRUCTIONSELECTOR_SECONDARYSIDESCREEN.TITLE.TITLETEXT);
                //buildTargetScreen.UpdateForConnection(targetManager.GetAssignmentGroupControllerIfExisting(), targetManager.WorldId, target.GetAssignmentGroupControllerIfExisting(), target.WorldId);
            }
            else
            {
                ClearSecondarySideScreen();
            }
        }
        public override void OnShow(bool show)
        {
            base.OnShow(show);

            if (!show)
            {
                ClearSecondarySideScreen();
            }
        }
        void ClearSecondarySideScreen()
        {
            DetailsScreen.Instance.ClearSecondarySideScreen();
            this.buildTargetScreen = null;
        }

        private void AddRowEntry(DockingManager referencedManager, bool startActive = true)
        {
            //int ReferenceWorldId = referencedManager.WorldId;
            //GameObject RowEntry = Util.KInstantiateUI(rowPrefab, listContainer);
            /////ListAllChildren(RowEntry.transform);
            //RowEntry.name = referencedManager.GetProperName();
            //RowEntry.transform.Find("Row1/TitleText").gameObject.GetComponent<LocText>().SetText(referencedManager.GetProperName());
            //RowEntry.transform.Find("Row1/SpaceCraftIcon/Image").GetComponent<Image>().sprite = referencedManager.GetDockingIcon();


            //Debug.Assert(!DockingTargets.ContainsKey(referencedManager), "Adding two of the same DockingManager to DockingSideScreen UI: " + referencedManager.gameObject.GetProperName());

            //var DockButton = RowEntry.transform.Find("Row1/Dock").gameObject.AddComponent<FButton>();
            //var UndockButton = RowEntry.transform.Find("Row1/Undock").gameObject.AddComponent<FButton>();
            //var TransferButton = RowEntry.transform.Find("Row2/TransferButton").gameObject.AddComponent<FButton>();
            //var ViewDockedButton = RowEntry.transform.Find("Row2/ViewDockedButton").gameObject.AddComponent<FButton>();

            //var SpaceShipIconRotatabe = RowEntry.transform.Find("Row1/WaitContainer/loading").rectTransform();
            //SpaceShipIconRotatabe.gameObject.SetActive(false);

            //DockButton.OnClick += () =>
            //{
            //    ClearSecondarySideScreen();
            //    this.targetManager.DockToTargetWorld(ReferenceWorldId, targetDoor);
            //    Refresh();
            //};

            //UndockButton.OnClick += () =>
            //{
            //    ClearSecondarySideScreen();
            //    rotatings.Add(SpaceShipIconRotatabe);
            //    SpaceShipIconRotatabe.gameObject.SetActive(true);

            //    targetManager.UnDockFromTargetWorld(ReferenceWorldId, false,
            //        () =>
            //        {
            //            rotatings.Remove(SpaceShipIconRotatabe);
            //            SpaceShipIconRotatabe.gameObject.SetActive(false);
            //            Refresh();
            //        });
            //    Refresh();
            //};
            //TransferButton.OnClick += () =>
            //{
            //    ToggleCrewScreen(referencedManager);
            //};

            //ViewDockedButton.OnClick += () =>
            //{
            //    if (referencedManager != null && targetManager.IsDockedTo(ReferenceWorldId))
            //    {
            //        if (targetManager.DockedToDoor(ReferenceWorldId) != null)
            //            ViewDockedButton.SetInteractable(targetManager.DockedToDoor(ReferenceWorldId).HasDupeTeleporter);
            //        {
            //            ClusterManager.Instance.SetActiveWorld(ReferenceWorldId);
            //            SelectTool.Instance.Activate();
            //        }
            //    }
            //};

            //DockingTargets.Add(referencedManager, RowEntry);
            //RowEntry.SetActive(startActive);
        }

        private void Refresh(object _ = null)
        {
            //if (targetManager == null)
            //{
            //    SgtLogger.l("Skipping refresh");
            //    return;
            //}
            //headerLabel.SetText(string.Format(STRINGS.UI.DOCKINGSCREEN.DOCKINGBRIDGES.TITLETEXT, targetManager.AvailableConnections(), targetManager.TotatConnections()));


            //foreach (var kvp in DockingTargets)
            //{
            //    //if (!kvp.Value.activeInHierarchy)
            //    //    continue;

            //    // SgtLogger.l("refreshing " + kvp.Key.ToString());

            //    var manager = kvp.Key;
            //    int ReferenceWorldId = manager.WorldId;
            //    var DockButton = kvp.Value.transform.Find("Row1/Dock").gameObject.GetComponent<FButton>();
            //    var UndockButton = kvp.Value.transform.Find("Row1/Undock").gameObject.GetComponent<FButton>();
            //    var TransferButton = kvp.Value.transform.Find("Row2/TransferButton").gameObject.GetComponent<FButton>();
            //    var ViewDockedButton = kvp.Value.transform.Find("Row2/ViewDockedButton").gameObject.GetComponent<FButton>();


            //    bool CanDock = targetManager.AvailableConnections() > 0 && manager.AvailableConnections() > 0 && !targetManager.IsDockedTo(ReferenceWorldId);
            //    DockButton.SetInteractable(CanDock);

            //    bool CanUnDock = targetManager.IsDockedTo(ReferenceWorldId) && !targetManager.HasPendingUndocks(ReferenceWorldId) && !manager.HasPendingUndocks(targetManager.WorldId);

            //    UndockButton.SetInteractable(CanUnDock);

            //    bool canViewInterior =
            //         CanUnDock
            //        && manager.DockedToDoor(targetManager.WorldId).HasDupeTeleporter
            //        ;

            //    ViewDockedButton.SetInteractable(canViewInterior);

            //    TransferButton.SetInteractable(canViewInterior);
            //}
        }

    }
}
