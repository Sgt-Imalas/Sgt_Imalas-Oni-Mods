using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using UnityEngine.UI;
using static ClusterTraitGenerationManager.ModAssets;
using static ClusterTraitGenerationManager.STRINGS.UI;
using ClusterTraitGenerationManager.ClusterData;

namespace ClusterTraitGenerationManager.UI.Screens
{
    internal class VanillaPOISelectorScreen : FScreen
    {
        public static VanillaPOISelectorScreen Instance { get; private set; }

        Dictionary<string, GameObject> StarmapItems = new Dictionary<string, GameObject>();
        public Action<string> SelectAction;

        public bool IsCurrentlyActive = false;
        public int CurrentBand;
        public StarmapItem CurrentPOIGroup;

        public static void InitializeView(StarmapItem poiGroup, Action<string> _selectAction)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(TraitPopup, FrontEndManager.Instance.gameObject, true);
                Instance = screen.AddOrGet<VanillaPOISelectorScreen>();
                Instance.Init();
            }
            Instance.SelectAction = _selectAction;
            Instance.CurrentBand = -1;
            Instance.CurrentPOIGroup = poiGroup;
            Instance.Show(true);
            Instance.FilterItems();
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
        }

        void FilterItems()
        {
            foreach (var item in StarmapItems.Values)
            {
                item.SetActive(true);
            }

            if (StarmapItems.ContainsKey(TemporalTearId))
            {
                StarmapItems[TemporalTearId].SetActive(CustomCluster != null && !CustomCluster.HasTear);
            }
            if (StarmapItems.ContainsKey(TeapotId))
            {
                StarmapItems[TeapotId].SetActive(CustomCluster != null && !CustomCluster.HasTeapot);
            }
            if (CurrentPOIGroup != null && CurrentPOIGroup.placementPOI != null && CurrentPOIGroup.placementPOI.pois != null)
            {
                if (StarmapItems.ContainsKey(RandomPOIId))
                {
                    StarmapItems[RandomPOIId].SetActive(CurrentPOIGroup.placementPOI.pois.Count == 0);
                }
                if (CurrentPOIGroup.placementPOI.pois.Any(id => id == RandomPOIId))
                {
                    foreach (var item in StarmapItems.Values)
                    {
                        item.SetActive(false);
                    }
                }
                else
                {
                    foreach (var id in CurrentPOIGroup.placementPOI.pois)
                    {
                        if (StarmapItems.ContainsKey(id))
                        {
                            StarmapItems[id].SetActive(false);
                        }
                    }
                }
            }

        }

        public static void InitializeView(int band, Action<string> _selectAction)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(TraitPopup, FrontEndManager.Instance.gameObject, true);
                Instance = screen.AddOrGet<VanillaPOISelectorScreen>();
                Instance.Init();
            }
            Instance.SelectAction = _selectAction;
            Instance.CurrentBand = band;
            Instance.CurrentPOIGroup = null;
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
        }

        private GameObject SeasonPrefab;
        private GameObject PossibleSeasonContainer;
        private bool init = false;
        private void Init()
        {
            if (init) return;
            init = true;
            SeasonPrefab = transform.Find("ScrollArea/Content/ListViewEntryPrefab").gameObject;
            PossibleSeasonContainer = transform.Find("ScrollArea/Content").gameObject;

            UIUtils.TryChangeText(transform, "Text", STARMAPITEMDESCRIPTOR.POIPLURAL);
            UIUtils.TryChangeText(PossibleSeasonContainer.transform, "NoTraitAvailable/Label", STARMAPITEMDESCRIPTOR.NOPOISAVAILABLE);

            var closeButton = transform.Find("CancelButton").FindOrAddComponent<FButton>();
            closeButton.OnClick += () =>
            {
                //OnCloseAction.Invoke();
                Show(false);
            };

            InitializeTraitContainer();
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ConsumeMouseScroll = true;

            Init();
        }

        void InitializeTraitContainer()
        {
            if (DlcManager.IsExpansion1Active())
            {
                foreach (POI_Data poiType in SO_POIs.Values)
                {
                    //if (poiType.Id == "Wormhole")
                    //    continue;

                    var poiInstanceHolder = Util.KInstantiateUI(SeasonPrefab, PossibleSeasonContainer, true);


                    UIUtils.TryChangeText(poiInstanceHolder.transform, "Label", poiType.Name);
                    UIUtils.AddSimpleTooltipToObject(poiInstanceHolder.transform, poiType.Description);

                    var icon = poiInstanceHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                    icon.gameObject.SetActive(true);
                    icon.sprite = poiType.Sprite;

                    var AddPOIButton = poiInstanceHolder.gameObject.AddOrGet<FButton>();

                    AddPOIButton.OnClick += () =>
                    {
                        SelectAction.Invoke(poiType.Id);
                        CloseThis();
                    };

                    StarmapItems[poiType.Id] = poiInstanceHolder;
                }
            }
            else
            {
                foreach (var poiType in Db.Get().SpaceDestinationTypes.resources)
                {
                    if (poiType.Id == "Wormhole")
                        continue;

                    var poiInstanceHolder = Util.KInstantiateUI(SeasonPrefab, PossibleSeasonContainer, true);


                    string name = poiType.Name;
                    string description = poiType.description;

                    UIUtils.AddSimpleTooltipToObject(poiInstanceHolder.transform, description);

                    var icon = poiInstanceHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                    icon.gameObject.SetActive(true);
                    icon.sprite = Assets.GetSprite(poiType.spriteName);

                    UIUtils.TryChangeText(poiInstanceHolder.transform, "Label", name);

                    var AddTraitButton = poiInstanceHolder.gameObject.AddOrGet<FButton>();

                    AddTraitButton.OnClick += () =>
                    {
                        SelectAction.Invoke(poiType.Id);
                        CloseThis();
                    };
                    StarmapItems[poiType.Id] = poiInstanceHolder;
                }
            }
        }


        public override void Show(bool show = true)
        {
            base.Show(show);
            IsCurrentlyActive = show;
        }
        void CloseThis()
        {
            //OnCloseAction.Invoke();
            Show(false);
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                //SgtLogger.l("CONSUMING 3");
                CloseThis();
            }

            base.OnKeyDown(e);
        }
    }
}
