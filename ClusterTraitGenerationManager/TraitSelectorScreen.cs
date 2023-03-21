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

namespace ClusterTraitGenerationManager
{
    internal class TraitSelectorScreen: FScreen
    {
        public static TraitSelectorScreen Instance { get; private set; }

        Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();
        public StarmapItem SelectedPlanet;
        public static System.Action OnCloseAction;

        public static void InitializeView(StarmapItem _planet, System.Action onclose)
        {
            if(Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitPopup, Global.Instance.globalCanvas, true);
                Instance = screen.AddOrGet<TraitSelectorScreen>();
                Instance.Init();
            }
            OnCloseAction = onclose;

            Instance.Show(true);
            Instance.SelectedPlanet = _planet;

            if (CustomCluster.HasStarmapItem(_planet.id, out var item))
            {
                foreach (var traitContainer in Instance.Traits.Values)
                {
                    traitContainer.SetActive(false);
                }
                foreach (var activeTrait in _planet.AllowedPlanetTraits)
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


        void InitializeTraitContainer()
        {
            foreach (var kvp in SettingsCache.worldTraits)
            {
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

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, description);
                

                AddTraitButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        item.AddWorldTrait(kvp.Value);
                    }
                    OnCloseAction.Invoke();
                    Show(false);
                };
                Traits[kvp.Value.filePath] = TraitHolder;
            }
        }
    }
}
