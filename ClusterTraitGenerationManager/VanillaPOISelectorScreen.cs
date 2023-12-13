using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using UnityEngine.UI;
using Klei.AI;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;
using TemplateClasses;
using static ClusterTraitGenerationManager.ModAssets;
using Satsuma;

namespace ClusterTraitGenerationManager
{
    internal class VanillaPOISelectorScreen : FScreen
    {
        public static VanillaPOISelectorScreen Instance { get; private set; }

        Dictionary<string, GameObject> VanillaStarmapItems = new Dictionary<string, GameObject>();
        public Action<string> SelectAction;

        public bool IsCurrentlyActive = false;
        public int CurrentBand;
        public StarmapItem CurrentPOIGroup;

        public static void InitializeView(StarmapItem poiGroup, Action<string> _selectAction)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitPopup, FrontEndManager.Instance.gameObject, true);
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
            foreach (var item in VanillaStarmapItems.Values)
            {
                item.SetActive(true);
            }

            if (VanillaStarmapItems.ContainsKey(ModAssets.TemporalTearId))
            {
                VanillaStarmapItems[ModAssets.TemporalTearId].SetActive(CustomCluster!=null && !CustomCluster.HasTear);
            }
            if (VanillaStarmapItems.ContainsKey(ModAssets.TeapotId))
            {
                VanillaStarmapItems[ModAssets.TeapotId].SetActive(CustomCluster != null && !CustomCluster.HasTeapot);
            }
            if (CurrentPOIGroup!= null&& CurrentPOIGroup.placementPOI != null && CurrentPOIGroup.placementPOI.pois!=null)
            {
                foreach (var id in CurrentPOIGroup.placementPOI.pois)
                {
                    if(VanillaStarmapItems.ContainsKey(id))
                    {
                        VanillaStarmapItems[id].SetActive(false);
                    }
                }
            }
            
        }

        public static void InitializeView(int band, Action<string> _selectAction)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitPopup, FrontEndManager.Instance.gameObject, true);
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
            this.ConsumeMouseScroll = true;

            Init();
        }

        void InitializeTraitContainer()
        {
            if (DlcManager.IsExpansion1Active())
            {
                foreach (POI_Data poiType in ModAssets.SO_POIs.Values)
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

                    VanillaStarmapItems[poiType.Id] = poiInstanceHolder;
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
                    VanillaStarmapItems[poiType.Id] = poiInstanceHolder;
                }
            }
        }


        public override void Show(bool show = true)
        {
            base.Show(show);
            this.IsCurrentlyActive = show;
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
