using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static ClusterTraitGenerationManager.STRINGS.UI;
using static ClusterTraitGenerationManager.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM.INDIVIDUALSETTINGS.ASTEROIDTRAITS;
using UnityEngine.UI;
using STRINGS;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;

namespace ClusterTraitGenerationManager
{
    internal class TraitSelectorScreen : FScreen
    {
        public static TraitSelectorScreen Instance { get; private set; }

        Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();
        public StarmapItem SelectedPlanet;
        public static System.Action OnCloseAction;

        public bool IsCurrentlyActive = false;

        public static void InitializeView(StarmapItem _planet, System.Action onclose)
        {
            if(Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitPopup, FrontEndManager.Instance.gameObject, true);
                Instance = screen.AddOrGet<TraitSelectorScreen>();
                Instance.Init();
            }
            OnCloseAction = onclose;

            Instance.Show(true);
            Instance.SelectedPlanet = _planet;
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();


            if (CustomCluster.HasStarmapItem(_planet.id, out var item))
            {
                foreach (var traitContainer in Instance.Traits.Values)
                {
                    traitContainer.SetActive(false);
                }
                foreach (var activeTrait in item.AllowedPlanetTraits)
                {
                    Instance.Traits[activeTrait.filePath].SetActive(true);
                }
            }
        }

        private GameObject TraitPrefab;
        private GameObject PossibleTraitsContainer;
        private bool init=false;
        private void Init()
        {
            if(init) return;
            init=true;
            TraitPrefab = transform.Find("ScrollArea/Content/ListViewEntryPrefab").gameObject;
            PossibleTraitsContainer = transform.Find("ScrollArea/Content").gameObject;

            var closeButton = transform.Find("CancelButton").FindOrAddComponent<FButton>();
            closeButton.OnClick += () =>
            {
                OnCloseAction.Invoke();
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
            foreach (var kvp in ModAssets.AllTraitsWithRandom)
            {
                //SgtLogger.l(kvp.Key, "INIT");

                var TraitHolder = Util.KInstantiateUI(TraitPrefab, PossibleTraitsContainer, true);
                //UIUtils.ListAllChildrenWithComponents(TraitHolder.transform);
                var AddTraitButton = TraitHolder.transform.Find("AddThisTraitButton").gameObject.FindOrAddComponent<FButton>();
                Strings.TryGet(kvp.Value.name, out var name);
                Strings.TryGet(kvp.Value.description, out var description);
                var combined = "<color=#" + kvp.Value.colorHex + ">" + name.ToString() + "</color>";


                string associatedIcon = kvp.Value.filePath.Substring(kvp.Value.filePath.LastIndexOf("/") + 1);
                
                var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.sprite = Assets.GetSprite(associatedIcon);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);

                if (kvp.Key.Contains(SpritePatch.randomTraitsTraitIcon))
                {
                    combined = UIUtils.RainbowColorText(name.ToString());
                }

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);
                

                AddTraitButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        item.AddWorldTrait(kvp.Value);
                    }
                    CloseThis();
                };
                Traits[kvp.Value.filePath] = TraitHolder;
            }
        }


        public override void Show(bool show = true)
        {
            base.Show(show);
            this.IsCurrentlyActive = show;
        }
        void CloseThis()
        {
            OnCloseAction.Invoke();
            Show(false);
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
            {
                SgtLogger.l("CONSUMING 3");
                CloseThis();
            }

            base.OnKeyDown(e);
        }
    }
}
