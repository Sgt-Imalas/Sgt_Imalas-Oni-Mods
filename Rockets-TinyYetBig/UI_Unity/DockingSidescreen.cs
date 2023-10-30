using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
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
    class DockingSidescreen : SideScreenContent, IRender200ms
    {

        private CrewAssignmentSidescreen crewScreen;
        public override void OnSpawn()
        {
            base.OnSpawn();

            ConnectReference();
            ClusterManager.Instance.Subscribe(ModAssets.Hashes.DockingManagerAdded, new System.Action<object>(this.ManagerAddedHandler));
            //ClusterManager.Instance.Subscribe(ModAssets.Hashes.DockingManagerRemoved , new System.Action<object>(this.ManagerRemovedHandler));

        }

        private void ManagerAddedHandler(object obj)
        {
            GameScheduler.Instance.ScheduleNextFrame("refreshUI", (oj =>
            {
                if (obj is DockingManager mng)
                {
                    if (!DockingTargets.ContainsKey(mng))
                    {
                        AddRowEntry(mng);
                        Refresh();
                    }
                    else
                    {
                        Debug.LogWarning("already had DockingManager");
                    }

                }
            })
            );

        }
        //private void ManagerRemovedHandler(object obj)
        //{
        //    if (obj is DockingManager mng)
        //    {
        //        if (!DockingTargets.ContainsKey(mng))
        //        {
        //            AddRowEntry(mng);
        //            Refresh();
        //        }
        //        else
        //        {
        //            Debug.LogWarning("already had DockingManager");
        //        }

        //    }

        //}



        protected DockingManager targetManager;
        protected Clustercraft targetCraft;
        protected IDockable targetDoor;

        protected GameObject rowPrefab;
        protected GameObject listContainer;
        protected LocText headerLabel;
        //[SerializeField]
        //private GameObject noChannelRow;
        private Dictionary<DockingManager, GameObject> DockingTargets = new Dictionary<DockingManager, GameObject>();


        private List<int> refreshHandle = new List<int>();


        public override float GetSortKey() => 21f;
        public override bool IsValidForTarget(GameObject target)
        {
            IDockable door = null;
            var manager = target.GetComponent<DockingManager>();
            if (manager == null)
            {
                if (target.TryGetComponent(out door))
                {
                    manager = door.dManager;
                }
            }
            if (manager == null)
                return false;

            var spaceship = manager.GetComponent<Clustercraft>();

            bool flying = spaceship != null ? spaceship.Status == Clustercraft.CraftStatus.InFlight : false;

            return manager != null && manager.HasDoors() && manager.GetCraftType != DockableType.Derelict && flying && !RocketryUtils.IsRocketInFlight(spaceship);
        }
        public override void ClearTarget()
        {
            //SgtLogger.l("clearing Target");
            foreach (int id in refreshHandle)
                targetCraft.Unsubscribe(id);
            refreshHandle.Clear();
            targetManager = null;
            targetDoor = null;
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

            ConnectReference();
            target.TryGetComponent(out targetManager); ///??? revisit
            target.TryGetComponent(out targetCraft);
            target.TryGetComponent(out targetDoor);
            if (targetManager == null && targetDoor != null)
            {
                targetManager = targetDoor.dManager;
            }
            targetManager.TryGetComponent(out targetCraft);

            Build();
            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new Action<object>(RefreshAll)));
            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new Action<object>(RefreshAll)));

            Refresh();
        }

        private void RefreshAll(object data = null) => DelayedRefresh();
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //titleKey = "STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.TITLE";
        }

        bool refsConnected = false;
        void ConnectReference()
        {
            if (refsConnected)
                return; 
            refsConnected = true;

            rowPrefab = transform.Find("OwnDupesContainer/ScrollRectContainer/ItemPrefab").gameObject;
            listContainer = transform.Find("OwnDupesContainer/ScrollRectContainer").gameObject;
            //var layout = transform.Find("Content/ContentScrollRect/Scrollbar").GetComponent<LayoutElement>();
            //SgtLogger.debuglog(String.Format("{0}, {1}, {2}", layout.minHeight, layout.preferredHeight, layout.flexibleHeight));
            //layout.minHeight = 150f;
            rowPrefab.gameObject.SetActive(false);
            headerLabel = TryFindComponent<LocText>(transform.Find("DockingBridges/TitleText"));
            //noChannelRow = transform.Find("Content/ContentScrollRect/RowContainer/Rows/NoChannelRow").gameObject;

            //TryChangeText(noChannelRow.transform, "Labels/Label", "Nothing");
            //TryChangeText(noChannelRow.transform, "Labels/DescLabel", "Nothing to dock to.");

        }

        private void Build()
        {
            foreach (var uirow in DockingTargets)
            {
                uirow.Value.SetActive(false);
            }

            if (targetManager == null || headerLabel == null)
                return;
            //headerLabel.SetText("Docking Ports: " + targetManager.GetUiDoorInfo());
            var AllDockerObjects = ClusterGrid.Instance.GetVisibleEntitiesAtCell(targetCraft.Location).FindAll(e => e.TryGetComponent(out DockingManager manager));
            var AllDockers = AllDockerObjects
                .Select(e => e.GetComponent<DockingManager>())
                .Where(mng => mng.HasDoors())
                .ToList();
            AllDockers.Remove(targetManager);

            //SgtLogger.debuglog("CurrentTargetType: " + targetManager.GetCraftType);

            if (targetManager.GetCraftType == DockableType.SpaceStation)
            {
                AllDockers.RemoveAll(craft => craft.GetCraftType == DockableType.SpaceStation);
            }



            foreach (DockingManager targetManager in AllDockers)
            {
                if (!DockingTargets.ContainsKey(targetManager))
                {
                    AddRowEntry(targetManager);
                }

                DockingTargets[targetManager].SetActive(true);
            }


            //noChannelRow.SetActive(AllDockers.Count == 0);

            Refresh();
            DelayedRefresh();
        }

        void DelayedRefresh()
        {
            GameScheduler.Instance.ScheduleNextFrame("dockingUiRefresh", (d) => Refresh());
        }

        List<RectTransform> rotatings = new List<RectTransform>();

        void ToggleCrewScreen(DockingManager target)
        {
            if (crewScreen == null)
            {
                crewScreen = (CrewAssignmentSidescreen)DetailsScreen.Instance.SetSecondarySideScreen(ModAssets.DupeTransferSecondarySideScreen, DOCKINGTRANSFERSCREEN.TITLE.TITLETEXT);
                crewScreen.UpdateForConnection(targetManager.GetAssignmentGroupControllerIfExisting(), targetManager.WorldId, target.GetAssignmentGroupControllerIfExisting(), target.WorldId);
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
            this.crewScreen = null;
        }

        private void AddRowEntry(DockingManager referencedManager, bool startActive = true)
        {
            int ReferenceWorldId = referencedManager.WorldId;
            GameObject RowEntry = Util.KInstantiateUI(rowPrefab, listContainer);
            ///ListAllChildren(RowEntry.transform);
            RowEntry.name = referencedManager.GetProperName();
            RowEntry.transform.Find("Row1/TitleText").gameObject.GetComponent<LocText>().SetText(referencedManager.GetProperName());
            RowEntry.transform.Find("Row1/SpaceCraftIcon/Image").GetComponent<Image>().sprite = referencedManager.GetDockingIcon();


            Debug.Assert(!DockingTargets.ContainsKey(referencedManager), "Adding two of the same DockingManager to DockingSideScreen UI: " + referencedManager.gameObject.GetProperName());

            var DockButton = RowEntry.transform.Find("Row1/Dock").gameObject.AddComponent<FButton>();
            var UndockButton = RowEntry.transform.Find("Row1/Undock").gameObject.AddComponent<FButton>();
            var TransferButton = RowEntry.transform.Find("Row2/TransferButton").gameObject.AddComponent<FButton>();
            var ViewDockedButton = RowEntry.transform.Find("Row2/ViewDockedButton").gameObject.AddComponent<FButton>();

            var SpaceShipIconRotatabe = RowEntry.transform.Find("Row1/WaitContainer/loading").rectTransform();
            SpaceShipIconRotatabe.gameObject.SetActive(false);

            DockButton.OnClick += () =>
            {
                ClearSecondarySideScreen();
                this.targetManager.DockToTargetWorld(ReferenceWorldId, targetDoor);
                Refresh();
            };

            UndockButton.OnClick += () =>
            {
                ClearSecondarySideScreen();
                rotatings.Add(SpaceShipIconRotatabe);
                SpaceShipIconRotatabe.gameObject.SetActive(true);

                targetManager.UnDockFromTargetWorld(ReferenceWorldId, false,
                    () =>
                    {
                        rotatings.Remove(SpaceShipIconRotatabe);
                        SpaceShipIconRotatabe.gameObject.SetActive(false);
                        Refresh();
                    });
                Refresh();
            };
            TransferButton.OnClick += () =>
            {
                ToggleCrewScreen(referencedManager);
            };

            ViewDockedButton.OnClick += () =>
            {
                if (referencedManager != null && targetManager.IsDockedTo(ReferenceWorldId))
                {
                    if (targetManager.DockedToDoor(ReferenceWorldId) != null)
                        ViewDockedButton.SetInteractable(targetManager.DockedToDoor(ReferenceWorldId).HasDupeTeleporter);
                    {
                        ClusterManager.Instance.SetActiveWorld(ReferenceWorldId);
                        SelectTool.Instance.Activate();
                    }
                }
            };

            DockingTargets.Add(referencedManager, RowEntry);
            RowEntry.SetActive(startActive);
        }

        private void Refresh()
        {
            Debug.Log(targetManager);

            if (targetManager == null)
            {
                SgtLogger.l("Skipping refresh");
                return;
            }
            headerLabel.SetText(string.Format(STRINGS.UI.DOCKINGSCREEN.DOCKINGBRIDGES.TITLETEXT, targetManager.AvailableConnections(), targetManager.TotatConnections()));

            foreach (var kvp in DockingTargets)
            {
                //if (!kvp.Value.activeInHierarchy)
                //    continue;

                // SgtLogger.l("refreshing " + kvp.Key.ToString());

                var manager = kvp.Key;
                int ReferenceWorldId = manager.WorldId;
                var DockButton = kvp.Value.transform.Find("Row1/Dock").gameObject.GetComponent<FButton>();
                var UndockButton = kvp.Value.transform.Find("Row1/Undock").gameObject.GetComponent<FButton>();
                var TransferButton = kvp.Value.transform.Find("Row2/TransferButton").gameObject.GetComponent<FButton>();
                var ViewDockedButton = kvp.Value.transform.Find("Row2/ViewDockedButton").gameObject.GetComponent<FButton>();


                bool CanDock = targetManager.AvailableConnections() > 0 && manager.AvailableConnections() > 0 && !targetManager.IsDockedTo(ReferenceWorldId);
                DockButton.SetInteractable(CanDock);

                bool CanUnDock = targetManager.IsDockedTo(ReferenceWorldId) && !targetManager.HasPendingUndocks(ReferenceWorldId) && !manager.HasPendingUndocks(targetManager.WorldId);

                UndockButton.SetInteractable(CanUnDock);

                bool canViewInterior =
                     CanUnDock
                    && manager.DockedToDoor(targetManager.WorldId).HasDupeTeleporter
                    ;

                ViewDockedButton.SetInteractable(canViewInterior);

                TransferButton.SetInteractable(canViewInterior);
            }
        }


        public void Render200ms(float dt)
        {
            foreach (var rotatable in rotatings)
            {
                rotatable.Rotate(new Vector3(0, 0, 25));
            }
        }
    }
}
