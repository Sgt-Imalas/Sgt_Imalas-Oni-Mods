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
using static UnityEngine.GraphicsBuffer;
using static UtilLibs.UIUtils;

namespace Rockets_TinyYetBig.UI_Unity
{
    class DockingSidescreen : SideScreenContent
    {

        public override void OnSpawn()
        {
            base.OnSpawn();
            // ListAllChildren(this.transform);
        }

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
            base.SetTarget(target);
            target.TryGetComponent(out targetManager); ///??? revisit
            target.TryGetComponent(out targetCraft);
            target.TryGetComponent(out targetDoor);
            if (targetManager == null)
            {
                targetManager = targetDoor.dManager;
            }
            targetManager.TryGetComponent(out targetCraft);
            ConnectReference();
            Build();
            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterDestinationChanged, new Action<object>(RefreshAll)));
            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new Action<object>(RefreshAll)));
        }

        private void RefreshAll(object data = null) => Build();
        private void ClearRows()
        {
            foreach (KeyValuePair<DockingManager, GameObject> broadcasterRow in DockingTargets)
                Util.KDestroyGameObject(broadcasterRow.Value);
            DockingTargets.Clear();
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            //titleKey = "STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.TITLE";
        }

        void ConnectReference()
        {
            return;

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
            //headerLabel.SetText("Docking Ports: " + targetManager.GetUiDoorInfo());
            ClearRows();
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
                if (RocketryUtils.IsRocketInFlight(targetCraft))
                    continue;
                targetManager.gameObject.TryGetComponent<Clustercraft>(out var clustercraft);
                if (clustercraft != null && RocketryUtils.IsRocketInFlight(clustercraft))
                    continue;


                if (!targetManager.IsNullOrDestroyed())
                {
                    GameObject gameObject = Util.KInstantiateUI(rowPrefab, listContainer);
                    gameObject.gameObject.name = targetManager.GetProperName();
                    Debug.Assert(!DockingTargets.ContainsKey(targetManager), "Adding two of the same DockingManager to DockingSideScreen UI: " + targetManager.gameObject.GetProperName());
                    DockingTargets.Add(targetManager, gameObject);
                    gameObject.SetActive(true);
                }
            }
            noChannelRow.SetActive(AllDockers.Count == 0);
            Refresh();
        }

        private void Refresh()
        {
            //headerLabel.SetText(string.Format(STRINGS.UI_MOD.UISIDESCREENS.DOCKINGSIDESCREEN.MORECONNECTIONS, targetManager.GetUiDoorInfo()));
            foreach (KeyValuePair<DockingManager, GameObject> kvp in DockingTargets)
            {
                kvp.Value.TryGetComponent<HierarchyReferences>(out var hr);
                hr.GetReference<LocText>("Label").SetText(kvp.Key.gameObject.GetProperName());
               // hr.GetReference<LocText>("DistanceLabel").SetText(kvp.Key.GetUiDoorInfo());
                hr.GetReference<Image>("Icon").gameObject.SetActive(false);
                //hr.GetReference<Image>("Icon").sprite = Def.GetUISprite((object)kvp.Key.gameObject).first;
                //hr.GetReference<Image>("Icon").color = Def.GetUISprite((object)kvp.Key.gameObject).second;
                WorldContainer myWorld = kvp.Key.GetMyWorld();
                hr.GetReference<Image>("WorldIcon").sprite = kvp.Key.GetDockingIcon();
                //hr.GetReference<Image>("WorldIcon").color = Color.black ;
                var toggle = hr.GetReference<MultiToggle>("Toggle");

                toggle.onClick = () =>
                {
                    targetManager.HandleUiDocking(toggle.CurrentState, kvp.Key.WorldId, targetDoor,
                        () =>
                        {
                            Refresh();
                        });
                    Refresh();
                };
                toggle.ChangeState(targetManager.GetActiveUIState(kvp.Key.WorldId) ? 1 : 0);
            }
        }
    }
}
