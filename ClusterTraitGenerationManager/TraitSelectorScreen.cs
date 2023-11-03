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
using UnityEngine.UI;
using STRINGS;
using static STRINGS.BUILDINGS.PREFABS.DOOR.CONTROL_STATE;
using static ClusterTraitGenerationManager.STRINGS.UI.CGMEXPORT_SIDEMENUS.TRAITPOPUP.SCROLLAREA.CONTENT.LISTVIEWENTRYPREFAB;

namespace ClusterTraitGenerationManager
{
    internal class TraitSelectorScreen : FScreen
    {
        public class BlacklistTrait : MonoBehaviour
        {
            public LocText buttonDescription;
            public Image backgroundImage;
            public Color originalColor;
            public FButton ToggleBlacklistTrait;
            public string referencedTraitId;

            public void Init(string traitID)
            {

                gameObject.transform.Find("AddThisTraitButton").gameObject.SetActive(true);
                buttonDescription = gameObject.transform.Find("AddThisTraitButton/Text").GetComponent<LocText>();
                ToggleBlacklistTrait = gameObject.transform
                    .Find("AddThisTraitButton")
                    .FindOrAddComponent<FButton>();
                backgroundImage = gameObject.transform.Find("Background").GetComponent<Image>();
                originalColor = backgroundImage.color;
                referencedTraitId = traitID;
                ToggleBlacklistTrait.OnClick +=
                    () =>
                    {
                        bool isBlacklisted = CGSMClusterManager.ToggleRandomTraitBlacklist(referencedTraitId);
                        UpdateState(isBlacklisted);
                    };

                RefreshState();
            }

            public void RefreshState()
            {
                UpdateState(CGSMClusterManager.RandomTraitInBlacklist(referencedTraitId));
            }
            public void UpdateState(bool isInBlacklist)
            {
                Color logicColour = isInBlacklist ? GlobalAssets.Instance.colorSet.logicOff : GlobalAssets.Instance.colorSet.logicOn;
                logicColour.a = 1f;
                backgroundImage.color = Color.Lerp(logicColour, originalColor, 0.8f);
                buttonDescription.text = isInBlacklist ? TOGGLETRAITBUTTON.REMOVEFROMBLACKLIST : TOGGLETRAITBUTTON.ADDTOBLACKLIST;
                UIUtils.AddSimpleTooltipToObject(ToggleBlacklistTrait.transform, isInBlacklist ? TOGGLETRAITBUTTON.REMOVEFROMBLACKLISTTOOLTIP : TOGGLETRAITBUTTON.ADDTOBLACKLISTTOOLTIP);
            }
        }


        public static TraitSelectorScreen Instance { get; private set; }

        Dictionary<string, BlacklistTrait> BlacklistedRandomTraits = new Dictionary<string, BlacklistTrait>();

        Dictionary<string, GameObject> Traits = new Dictionary<string, GameObject>();
        public StarmapItem SelectedPlanet;
        public static System.Action OnCloseAction;

        public bool IsCurrentlyActive = false;

        public static void InitializeView(StarmapItem _planet, System.Action onclose, bool editingRandomBlacklist = false)
        {
            if (Instance == null)
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


            if (_planet != null && CustomCluster.HasStarmapItem(_planet.id, out var item))
            {
                foreach (var traitContainer in Instance.BlacklistedRandomTraits.Values)
                {
                    traitContainer.gameObject.SetActive(false);
                }
                foreach (var traitContainer in Instance.Traits.Values)
                {
                    traitContainer.SetActive(false);
                }
                foreach (var activeTrait in item.AllowedPlanetTraits)
                {
                    Instance.Traits[activeTrait.filePath].SetActive(true);
                }
            }
            if(editingRandomBlacklist)
            {
                foreach (var traitContainer in Instance.Traits.Values)
                {
                    traitContainer.SetActive(false);
                }
                foreach (var traitContainer in Instance.BlacklistedRandomTraits.Values)
                {
                    traitContainer.gameObject.SetActive(true);
                    traitContainer.RefreshState();
                }
            }

        }

        private GameObject TraitPrefab;
        private GameObject PossibleTraitsContainer;
        private bool init = false;


        private void Init()
        {
            if (init) return;
            init = true;
            TraitPrefab = transform.Find("ScrollArea/Content/ListViewEntryPrefab").gameObject;
            TraitPrefab.SetActive(false);
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
                var AddTraitButton = TraitHolder
                    //.transform.Find("AddThisTraitButton").gameObject
                    .FindOrAddComponent<FButton>();
                Strings.TryGet(kvp.Value.name, out var name);
                Strings.TryGet(kvp.Value.description, out var description);
                var combined = "<color=#" + kvp.Value.colorHex + ">" + name.ToString() + "</color>";

                var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.sprite = ModAssets.GetTraitSprite(kvp.Value);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);

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


            foreach (var kvp in SettingsCache.worldTraits)
            {
                var TraitHolder = Util.KInstantiateUI(TraitPrefab, PossibleTraitsContainer, true);
                var blacklistContainer = TraitHolder.AddOrGet<BlacklistTrait>();
                blacklistContainer.Init(kvp.Value.filePath);

                Strings.TryGet(kvp.Value.name, out var name);
                Strings.TryGet(kvp.Value.description, out var description);
                var combined = "<color=#" + kvp.Value.colorHex + ">" + name.ToString() + "</color>";

                var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.sprite = ModAssets.GetTraitSprite(kvp.Value);
                icon.color = Util.ColorFromHex(kvp.Value.colorHex);

                UIUtils.TryChangeText(TraitHolder.transform, "Label", combined);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform.Find("Label"), description);

                BlacklistedRandomTraits[kvp.Value.filePath] = blacklistContainer;
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
                CloseThis();
            }

            base.OnKeyDown(e);
        }
    }
}
