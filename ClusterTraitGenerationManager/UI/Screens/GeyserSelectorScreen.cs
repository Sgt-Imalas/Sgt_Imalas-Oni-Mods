using ProcGen;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using UnityEngine.UI;
using static ClusterTraitGenerationManager.STRINGS.UI.CGMEXPORT_SIDEMENUS.TRAITPOPUP.SCROLLAREA.CONTENT.LISTVIEWENTRYPREFAB;
using ClusterTraitGenerationManager.ClusterData;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.UI.Screens.TraitSelectorScreen;
using ClusterTraitGenerationManager.GeyserExperiments;
using static ClusterTraitGenerationManager.STRINGS.UI.CGM_MAINSCREENEXPORT.DETAILS.CONTENT.SCROLLRECTCONTAINER;

namespace ClusterTraitGenerationManager.UI.Screens
{
    internal class GeyserSelectorScreen : FScreen
    {
        //public class BlacklistGeyser : MonoBehaviour
        //{
        //    public LocText buttonDescription;
        //    public Image backgroundImage;
        //    public Color originalColor;
        //    public FButton ToggleBlacklistTrait;
        //    public string referencedGeyserId;

        //    public void Init(GeyserDataEntry geyserData)
        //    {

        //        gameObject.transform.Find("AddThisTraitButton").gameObject.SetActive(true);
        //        buttonDescription = gameObject.transform.Find("AddThisTraitButton/Text").GetComponent<LocText>();
        //        ToggleBlacklistTrait = gameObject.transform
        //            .Find("AddThisTraitButton")
        //            .FindOrAddComponent<FButton>();
        //        backgroundImage = gameObject.transform.Find("Background").GetComponent<Image>();
        //        originalColor = backgroundImage.color;
        //        referencedGeyserId = geyserData.ID;
        //        ToggleBlacklistTrait.OnClick +=
        //            () =>
        //            {
        //                bool isBlacklisted = CGSMClusterManager.ToggleRandomGeyserBlacklist(referencedGeyserId);
        //                UpdateState(isBlacklisted);
        //            };

        //        RefreshState();
        //    }

        //    public void RefreshState()
        //    {
        //        UpdateState(CGSMClusterManager.RandomGeyserInBlacklist(referencedGeyserId));
        //    }
        //    public void UpdateState(bool isInBlacklist)
        //    {
        //        Color logicColour = isInBlacklist ? GlobalAssets.Instance.colorSet.logicOff : GlobalAssets.Instance.colorSet.logicOn;
        //        logicColour.a = 1f;
        //        backgroundImage.color = Color.Lerp(logicColour, originalColor, 0.8f);
        //        buttonDescription.text = isInBlacklist ? TOGGLETRAITBUTTON.REMOVEFROMBLACKLIST : TOGGLETRAITBUTTON.ADDTOBLACKLIST;
        //        UIUtils.AddSimpleTooltipToObject(ToggleBlacklistTrait.transform, isInBlacklist ? TOGGLETRAITBUTTON.REMOVEFROMBLACKLISTTOOLTIP : TOGGLETRAITBUTTON.ADDTOBLACKLISTTOOLTIP);
        //    }
        //}

        //Dictionary<string, BlacklistGeyser> BlacklistedRandomGeyserUIEntries = new Dictionary<string, BlacklistGeyser>();

        public static GeyserSelectorScreen Instance { get; private set; }

        Dictionary<string, GameObject> GeyserUIEntries = new Dictionary<string, GameObject>();
        public StarmapItem SelectedPlanet;
        public static System.Action OnCloseAction;

        public bool IsCurrentlyActive = false;
        public bool BlacklistMode = false;

        public static void InitializeView(StarmapItem _planet, System.Action onclose, bool blackListMode = false)
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.TraitPopup, FrontEndManager.Instance.gameObject, true);
                Instance = screen.AddOrGet<GeyserSelectorScreen>();
                Instance.Init();
            }
            OnCloseAction = onclose;
            Instance.BlacklistMode = blackListMode;
            Instance.Show(true);
            Instance.SelectedPlanet = _planet;
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();


            if (_planet != null && CustomCluster.HasStarmapItem(_planet.id, out var item))
            {
                foreach (var geyserContainer in Instance.GeyserUIEntries)
                {
                    if (blackListMode)
                    {
                        geyserContainer.Value.SetActive(!item.HasGeyserBlacklisted(geyserContainer.Key));
                        
                    }
                    else
                        geyserContainer.Value.SetActive(true);
                }
            }
        }

        private GameObject GeyserEntryPrefab;
        private GameObject PossibleGeyserUIEntriesContainer;
        private bool init = false;


        private void Init()
        {
            if (init) return;
            init = true;
            GeyserEntryPrefab = transform.Find("ScrollArea/Content/ListViewEntryPrefab").gameObject;
            GeyserEntryPrefab.SetActive(false);
            PossibleGeyserUIEntriesContainer = transform.Find("ScrollArea/Content").gameObject;

            UIUtils.TryChangeText(transform, "Text", ASTEROIDGEYSERS.DESCRIPTOR.LABEL);
            UIUtils.TryChangeText(PossibleGeyserUIEntriesContainer.transform, "NoTraitAvailable/Label", ASTEROIDGEYSERS.DESCRIPTOR.NONE);
            var closeButton = transform.Find("CancelButton").FindOrAddComponent<FButton>();
            closeButton.OnClick += () =>
            {
                OnCloseAction.Invoke();
                Show(false);
            };


            InitializeGeyserContainer();
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ConsumeMouseScroll = true;

            Init();
        }
        bool IsInBlacklistMode() => BlacklistMode;
        void InitializeGeyserContainer()
        {
            foreach (var kvp in ModAssets.AllGeysers)
            {
                //SgtLogger.l(kvp.Key, "INIT");
                var geyserEntry  = kvp.Value;
                var TraitHolder = Util.KInstantiateUI(GeyserEntryPrefab, PossibleGeyserUIEntriesContainer, true);
                //UIUtils.ListAllChildrenWithComponents(TraitHolder.transform);
                var AddTraitButton = TraitHolder.FindOrAddComponent<FButton>();
                Strings.TryGet(geyserEntry.Name, out var name);
                Strings.TryGet(geyserEntry.Description, out var description);

                var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
                icon.preserveAspect = true;
                icon.sprite = geyserEntry.Sprite;
                icon.color = Color.white;
                UIUtils.TryChangeText(TraitHolder.transform, "Label", kvp.Value.Name);
                UIUtils.AddSimpleTooltipToObject(TraitHolder.transform, kvp.Value.Description);

                AddTraitButton.OnClick += () =>
                {
                    if (CustomCluster.HasStarmapItem(SelectedPlanet.id, out var item))
                    {
                        if (IsInBlacklistMode())
                            item.AddGeyserBlacklist(kvp.Value.ID);
                        else
                            item.AddGeyserOverride(kvp.Value.ID);
                    }
                    CloseThis();
                };
                GeyserUIEntries[kvp.Value.ID] = TraitHolder;
            }
            

            //foreach (var kvp in ModAssets.AllGeysers)
            //{
            //    var geyserData = kvp.Value;
            //    if(!geyserData.Generic)
            //        continue;

            //    var TraitHolder = Util.KInstantiateUI(GeyserEntryPrefab, PossibleGeyserUIEntriesContainer, true);
            //    var blacklistContainer = TraitHolder.AddOrGet<BlacklistGeyser>();
            //    blacklistContainer.Init(geyserData);

            //    var icon = TraitHolder.transform.Find("Label/TraitImage").GetComponent<Image>();
            //    icon.sprite = geyserData.Sprite;
            //    icon.color = Color.white;

            //    UIUtils.TryChangeText(TraitHolder.transform, "Label", geyserData.Name);
            //    UIUtils.AddSimpleTooltipToObject(TraitHolder.transform.Find("Label"), geyserData.Description);

            //    BlacklistedRandomGeyserUIEntries[geyserData.ID] = blacklistContainer;
            //}
        }


        public override void Show(bool show = true)
        {
            base.Show(show);
            IsCurrentlyActive = show;
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
