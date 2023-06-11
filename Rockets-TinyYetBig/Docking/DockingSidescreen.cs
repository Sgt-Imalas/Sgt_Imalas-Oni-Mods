using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static UnityEngine.GraphicsBuffer;
using static UtilLibs.UIUtils;

namespace Rockets_TinyYetBig.Docking
{
    class DockingSidescreen : SideScreenContent
    {

        public override void OnSpawn()
        {
            base.OnSpawn();
            // ListAllChildren(this.transform);
        }

        //[SerializeField]
        //private Dictionary<DockingDoor, GameObject> dockingPorts = new Dictionary<DockingDoor, GameObject>();
        private DockingManager targetManager;
        private Clustercraft targetCraft;
        private DockingDoor targetDoor;

        [SerializeField]
        private GameObject rowPrefab;
        [SerializeField]
        private GameObject listContainer;
        [SerializeField]
        private LocText headerLabel;
        [SerializeField]
        private GameObject noChannelRow;
        private Dictionary<DockingManager, GameObject> DockingTargets = new Dictionary<DockingManager, GameObject>();


        private List<int> refreshHandle = new List<int>();


        public override float GetSortKey() => 21f;
        public override bool IsValidForTarget(GameObject target)
        {
            DockingDoor door = null;
            var manager = target.GetComponent<DockingManager>();
            if (manager == null)
            {
                if (target.TryGetComponent<DockingDoor>(out door))
                {
                    manager = door.dManager;
                }
            }
            if (manager == null)
                return false;

            var spaceship = manager.GetComponent<Clustercraft>();

            bool flying = spaceship != null ? spaceship.Status == Clustercraft.CraftStatus.InFlight : false;

            return manager != null && manager.HasDoors() && manager.GetCraftType != DockableType.Derelict && flying && (!RocketryUtils.IsRocketInFlight(spaceship));
        }
        public override void ClearTarget()
        {
            foreach (int id in this.refreshHandle)
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
                foreach (int id in this.refreshHandle)
                    target.Unsubscribe(id);
                refreshHandle.Clear();
            }
            base.SetTarget(target);
            target.TryGetComponent<DockingManager>(out targetManager); ///??? revisit
            target.TryGetComponent<Clustercraft>(out targetCraft);
            target.TryGetComponent<DockingDoor>(out targetDoor);
            if (targetManager == null)
            {
                targetManager = targetDoor.dManager;
            }
            targetManager.TryGetComponent<Clustercraft>(out targetCraft);
            ConnectReference();
            Build();
            refreshHandle.Add(this.targetCraft.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new System.Action<object>(this.RefreshAll)));
            refreshHandle.Add(this.targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.RefreshAll)));
        }

        private void RefreshAll(object data = null) => this.Build();
        private void ClearRows()
        {
            foreach (KeyValuePair<DockingManager, GameObject> broadcasterRow in this.DockingTargets)
                Util.KDestroyGameObject(broadcasterRow.Value);
            this.DockingTargets.Clear();
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            titleKey = "STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.TITLE";
        }

        void ConnectReference()
        {
            if (rowPrefab == null)
            {
                rowPrefab = transform.Find("Content/ContentScrollRect/RowContainer/Rows/RowPrefab").gameObject;
                listContainer = transform.Find("Content/ContentScrollRect/RowContainer/Rows").gameObject;
                //var layout = transform.Find("Content/ContentScrollRect/Scrollbar").GetComponent<LayoutElement>();
                //SgtLogger.debuglog(String.Format("{0}, {1}, {2}", layout.minHeight, layout.preferredHeight, layout.flexibleHeight));
                //layout.minHeight = 150f;

                transform.Find("Content/ContentScrollRect").GetComponent<LayoutElement>().minHeight = 150;
                transform.Find("Content/ContentScrollRect/Scrollbar").GetComponent<LayoutElement>().minHeight = 120;

                headerLabel = TryFindComponent<LocText>(transform.Find("Content/Header"));
                noChannelRow = transform.Find("Content/ContentScrollRect/RowContainer/Rows/NoChannelRow").gameObject;

                TryChangeText(noChannelRow.transform, "Labels/Label", "Nothing");
                TryChangeText(noChannelRow.transform, "Labels/DescLabel", "Nothing to dock to.");
            }
        }

        private void Build()
        {
#if DEBUG
            // SgtLogger.debuglog("------------------");
            //UIUtils.ListAllChildren(this.transform);
            // SgtLogger.debuglog("------------------");
            //UIUtils.ListAllChildrenWithComponents(this.transform);
            // SgtLogger.debuglog("------------------");
#endif
            if (targetManager == null || headerLabel == null)
                return;
            this.headerLabel.SetText("Docking Ports: " + targetManager.GetUiDoorInfo());
            this.ClearRows();
            var AllDockerObjects = ClusterGrid.Instance.GetVisibleEntitiesAtCell(this.targetCraft.Location).FindAll(e => e.TryGetComponent<DockingManager>(out DockingManager manager));
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
                if (RocketryUtils.IsRocketInFlight(targetCraft))
                    continue;
                targetManager.gameObject.TryGetComponent<Clustercraft>(out var clustercraft);
                if (clustercraft != null && RocketryUtils.IsRocketInFlight(clustercraft))
                    continue;


                if (!targetManager.IsNullOrDestroyed())
                {
                    GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.listContainer);
                    gameObject.gameObject.name = targetManager.GetProperName();
                    Debug.Assert(!this.DockingTargets.ContainsKey(targetManager), (object)("Adding two of the same DockingManager to DockingSideScreen UI: " + targetManager.gameObject.GetProperName()));
                    this.DockingTargets.Add(targetManager, gameObject);
                    gameObject.SetActive(true);
                }
            }
            this.noChannelRow.SetActive(AllDockers.Count == 0);
            this.Refresh();
        }

        private void Refresh()
        {
            this.headerLabel.SetText(string.Format(STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.MORECONNECTIONS, targetManager.GetUiDoorInfo()));
            foreach (KeyValuePair<DockingManager, GameObject> kvp in this.DockingTargets)
            {
                kvp.Value.TryGetComponent<HierarchyReferences>(out var hr);
                hr.GetReference<LocText>("Label").SetText(kvp.Key.gameObject.GetProperName());
                hr.GetReference<LocText>("DistanceLabel").SetText(kvp.Key.GetUiDoorInfo());
                hr.GetReference<Image>("Icon").gameObject.SetActive(false);
                //hr.GetReference<Image>("Icon").sprite = Def.GetUISprite((object)kvp.Key.gameObject).first;
                //hr.GetReference<Image>("Icon").color = Def.GetUISprite((object)kvp.Key.gameObject).second;
                WorldContainer myWorld = kvp.Key.GetMyWorld();
                hr.GetReference<Image>("WorldIcon").sprite = kvp.Key.GetDockingIcon();
                //hr.GetReference<Image>("WorldIcon").color = Color.black ;
                var toggle = hr.GetReference<MultiToggle>("Toggle");

                toggle.onClick = (() =>
                {
                    targetManager.HandleUiDocking(toggle.CurrentState, kvp.Key.GetWorldId(), targetDoor,
                        () =>
                        {
                            this.Refresh();
                        });
                    this.Refresh();
                });
                hr.GetReference<MultiToggle>("Toggle")
                    .ChangeState(targetManager.GetActiveUIState(kvp.Key.GetWorldId()) ? 1 : 0);
            }
        }
    }
}
