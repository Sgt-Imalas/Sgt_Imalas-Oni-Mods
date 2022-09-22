using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static UtilLibs.UIUtils;

namespace Rockets_TinyYetBig.Docking
{
    class DockingSidescreen : SideScreenContent
    {
        public override bool IsValidForTarget(GameObject target)
        {
            var manager = target.GetComponent<DockingManager>();
            return manager != null && manager.HasDoors();
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            ListAllChildren(this.transform);
        }

        [SerializeField]
        private Dictionary<DockingDoor, GameObject> dockingPorts = new Dictionary<DockingDoor, GameObject>();
        [SerializeField]
        private Clustercraft targetCraft;
        private DockingManager targetManager;

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
        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);

            if (target != null)
            {
                foreach (int id in this.refreshHandle)
                    target.Unsubscribe(id);
                refreshHandle.Clear();
            }
            base.SetTarget(target);



            targetManager = target.GetComponent<DockingManager>();
            targetCraft = target.GetComponent<Clustercraft>();
            if (targetCraft == null && target.GetComponent<RocketControlStation>() != null) 
            { 
                targetCraft = target.GetMyWorld().GetComponent<Clustercraft>();
                targetManager = targetCraft.GetComponent<DockingManager>();
            }
            ConnectReference();
            Build();
            refreshHandle.Add(this.targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.RefreshAll)));
        }

        private void RefreshAll(object data = null) => this.Build();
        private void ClearRows()
        {
            foreach (KeyValuePair<DockingManager, GameObject> broadcasterRow in this.DockingTargets)
                Util.KDestroyGameObject(broadcasterRow.Value);
            this.DockingTargets.Clear();
        }
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        void ConnectReference()
        {
            if (rowPrefab == null)
            {
                rowPrefab=transform.Find("Content/ContentScrollRect/RowContainer/Rows/RowPrefab").gameObject;
                listContainer = transform.Find("Content/ContentScrollRect/RowContainer/Rows").gameObject;
                headerLabel = TryFindComponent<LocText>(transform.Find("Content/Header"));
                noChannelRow = transform.Find("Content/ContentScrollRect/RowContainer/Rows/NoChannelRow").gameObject;

                TryChangeText(noChannelRow.transform, "Labels/Label", "Label");
                TryChangeText(noChannelRow.transform, "Labels/DescLabel", "No Dockables Found");
            }
        }

        private void Build()
        {
#if DEBUG
           // UIUtils.ListAllChildren(this.transform);
#endif
            this.headerLabel.SetText((string)"Docking Ports");
            this.ClearRows();
            var AllDockerObjects = ClusterGrid.Instance.GetVisibleEntitiesAtCell(this.targetCraft.Location).FindAll(e => e.TryGetComponent<DockingManager>(out DockingManager manager));
            var AllDockers = AllDockerObjects.Select(e => e.GetComponent<DockingManager>()).Where(man => man.HasDoors()).ToList();
            AllDockers.Remove(targetManager);

            foreach (DockingManager targetManager in AllDockers)
            {
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
            foreach (KeyValuePair<DockingManager, GameObject> broadcasterRow in this.DockingTargets)
            {
                KeyValuePair<DockingManager, GameObject> kvp = broadcasterRow;
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(kvp.Key.gameObject.GetProperName());
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("DistanceLabel").SetText("DistanceLabel");
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").gameObject.SetActive(false);
                //kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite((object)kvp.Key.gameObject).first;
                //kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color = Def.GetUISprite((object)kvp.Key.gameObject).second;
                WorldContainer myWorld = kvp.Key.GetMyWorld();
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").sprite = targetManager.GetDockingIcon();
                //kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("WorldIcon").color = Color.black ;
                var toggle = kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle");
                toggle.onClick = (System.Action)(() =>
                {
                    targetManager.HandleUiDocking(toggle.CurrentState,kvp.Key.GetWorldId());
                    this.Refresh();
                });
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(targetManager.IsDockedTo(kvp.Key) ? 1 : 0);
            }
        }
    }
}
