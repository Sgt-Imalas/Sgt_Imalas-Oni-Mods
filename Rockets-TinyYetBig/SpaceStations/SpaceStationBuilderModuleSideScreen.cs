using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.CREATURES.STATS;
using UnityEngine;
using UtilLibs;
using static KAnim;
using UnityEngine.UI;
using YamlDotNet.Core.Tokens;
using static Rockets_TinyYetBig.ModAssets;
using static STRINGS.DUPLICANTS.STATUSITEMS;

namespace Rockets_TinyYetBig.SpaceStations
{
    internal class SpaceStationBuilderModuleSideScreen : SideScreenContent
    {
        [SerializeField]
        private RectTransform buttonContainer;

        private GameObject stateButtonPrefab;
        private GameObject PlaceStationButton;
        private GameObject flipButton;
        private Dictionary<KeyValuePair<int, SpaceStationWithStats>, MultiToggle> buttons = new Dictionary<KeyValuePair<int, SpaceStationWithStats>, MultiToggle>();
        //private Dictionary<SpaceStationWithStats, MultiToggle> buttons = new Dictionary<SpaceStationWithStats, MultiToggle>();

        private Clustercraft targetCraft;
        private SpaceStationBuilder targetBuilder;

        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Clustercraft>() != null && this.HasConstructor(target.GetComponent<Clustercraft>());
        private bool HasConstructor(Clustercraft craft)
        {
            foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                if (clusterModule.Get().GetComponent<SpaceStationBuilder>() != null)
                    return true;
            }
            return false;
        }
        protected override void OnSpawn()
        {
            base.OnSpawn();
            // the monument screen used here has 2 extra buttons that are not needed, disabling them
            //flipButton.SetActive(false);
            //PlaceStationButton.SetActive(false);
            UIUtils.TryChangeText(PlaceStationButton.transform, "Label", "MakeOrBreakSpaceStation");
            RefreshButtons();
            UIUtils.AddActionToButton(PlaceStationButton.transform, "", () => { targetBuilder.ConstructButtonPressed(); RefreshButtons(); });
            UIUtils.AddActionToButton(flipButton.transform, "", () => { targetBuilder.DemolishButtonPressed(); RefreshButtons(); });
        }

        protected override void OnPrefabInit()
        {

            UIUtils.ListAllChildren(this.transform);
            base.OnPrefabInit();
            titleKey = "STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONBUILDERMODULESIDESCREEN.TITLE";
            stateButtonPrefab = transform.Find("ButtonPrefab").gameObject;
            buttonContainer = transform.Find("Content/Scroll/Grid").GetComponent<RectTransform>();
            PlaceStationButton = transform.Find("Butttons/ApplyButton").gameObject;
            flipButton = transform.Find("Butttons/ClearStyleButton").gameObject;
            GenerateStateButtons();

            PlaceStationButton.GetComponent<ToolTip>().enabled = false;
            Destroy(flipButton.GetComponent<ToolTip>());
            UIUtils.AddSimpleTooltipToObject(flipButton.transform.Find("FG"), "", true);
        }

        private List<int> refreshHandle = new List<int>();
        public override void SetTarget(GameObject target)
        {
            if (target != null)
            {
                foreach (int id in this.refreshHandle)
                    target.Unsubscribe(id);
                refreshHandle.Clear();
            }
            base.SetTarget(target);

            targetCraft = target.GetComponent<Clustercraft>();

            foreach (Ref<RocketModuleCluster> clusterModule in targetCraft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                if (clusterModule.Get().TryGetComponent<SpaceStationBuilder>(out var targetb))
                {
                    targetBuilder = targetb;
                    break;
                }
            }
            //GetPrefabStrings();
            refreshHandle.Add(target.Subscribe((int)GameHashes.ResearchComplete, RefreshAll)); 
            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.RefreshAll)));
            refreshHandle.Add(targetCraft.gameObject.Subscribe((int)GameHashes.JettisonedLander, new System.Action<object>(this.RefreshWithNotice)));
            refreshHandle.Add(targetBuilder.gameObject.Subscribe((int)GameHashes.JettisonedLander, RefreshWithNotice));
            //BuildModules();
        }

        private void RefreshWithNotice(object data = null)
        {
            //Debug.Log("HashEvent Triggered");
            this.RefreshButtons();
        }
        private void RefreshAll(object data = null) => this.RefreshButtons(); 


        // Creates clickable card buttons for all the lamp types + a randomizer button

        private void GenerateStateButtons()
        {
            ClearButtons();
            foreach (var stationType in ModAssets.SpaceStationTypes)
            {
                AddButton(stationType,
                    () =>
                    {
                        targetBuilder.SetStationType(stationType.Key);
                        RefreshButtons();
                    }
                    );
            }
            RefreshButtons();
        }

        private void AddButton(KeyValuePair<int, SpaceStationWithStats> type, System.Action onClick)
        {
            var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);

            //Debug.Log("ButtonPrefab_");
            //UIUtils.ListAllChildrenWithComponents(stateButtonPrefab.transform);

            if (gameObject.TryGetComponent(out MultiToggle button))
            {
                //Assets.TryGetAnim((HashedString)animName, out var anim);
                button.onClick += onClick;
                button.ChangeState(type.Key == targetBuilder.CurrentSpaceStationTypeInt ? 1 : 0);
                //Debug.Log(Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Kanim)));
                //Debug.Log("anim");
                UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Value.Kanim));
                UIUtils.AddSimpleTooltipToObject(gameObject.transform, "<b>" + type.Value.Name + "</b>\n" + type.Value.Description, true);
                buttons.Add(type, button);
            }

        }
        //private void AddButton(SpaceStationWithStats type, System.Action onClick)
        //{
        //    var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);

        //    //Debug.Log("ButtonPrefab_");
        //    //UIUtils.ListAllChildrenWithComponents(stateButtonPrefab.transform);

        //    if (gameObject.TryGetComponent(out MultiToggle button))
        //    {
        //        //Assets.TryGetAnim((HashedString)animName, out var anim);
        //        button.onClick += onClick;
        //        button.ChangeState(ModAssets.GetStationIndex(type) == targetBuilder.CurrentSpaceStationTypeInt ? 1 : 0);
        //        //Debug.Log(Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Kanim)));
        //        //Debug.Log("anim");
        //        UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Kanim));
        //        UIUtils.AddSimpleTooltipToObject(gameObject.transform, type.Name+"\n"+type.Description, true);
        //        buttons.Add(type, button);
        //    }

        //}

        void RefreshButtons()
        {
            int CurrentStationType = 0;
            CurrentStationType = targetBuilder.CurrentSpaceStationTypeInt;

            foreach (var button in buttons)
            {

                var tech = Db.Get().Techs.Get(button.Key.Value.requiredTechID);

                if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || tech == null || (tech != null && tech.IsComplete()))
                {
                    button.Value.gameObject.SetActive(true);
                    button.Value.ChangeState((button.Key.Key) == CurrentStationType ? 1 : 0);
                }
                else
                {
                    button.Value.gameObject.SetActive(false);
                }
            }
            if (PlaceStationButton != null)
            {
                //UIUtils.ListAllChildren(flipButton.transform);
                var img = flipButton.transform.Find("FG").GetComponent<Image>();
                img.sprite = Assets.GetSprite(targetBuilder.Constructing() && targetBuilder.Demolishing() ? "action_cancel" : "action_deconstruct");

                UIUtils.TryChangeText(PlaceStationButton.transform, "Label", targetBuilder.Constructing() && !targetBuilder.Demolishing() 
                    ? STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONBUILDERMODULESIDESCREEN.CANCELCONSTRUCTION
                    : STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONBUILDERMODULESIDESCREEN.STARTCONSTRUCTION);
                //UIUtils.AddSimpleTooltipToObject(PlaceStationButton.transform, targetSatelliteCarrier.HoldingSatellite() ? (ModAssets.SatelliteConfigurations[CurrentStationType].DESC) : (string)STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLELABEL_HASSAT_FALSE, true);

                img.sprite = Assets.GetSprite(targetBuilder.Demolishing() ? "action_cancel" : "action_deconstruct");
                bool canDeconstruct = targetBuilder.IsStationAtCurrentLocation();
                flipButton.GetComponent<KButton>().isInteractable = canDeconstruct;
                PlaceStationButton.GetComponent<KButton>().isInteractable = !canDeconstruct&&SpaceStationManager.Instance.CanConstructSpaceStation();
            }
        }

        //void RefreshButtons()
        //{
        //    foreach (var button in buttons)
        //    {
        //        //Debug.Log(targetBuilder.CurrentSpaceStationType + " <- current type, Button int -> " + button.Key);


        //        var tech = button.Key.requiredTech;
        //        if (tech == null || (tech != null && tech.IsComplete()))
        //        {
        //            button.Value.gameObject.SetActive(true);
        //            button.Value.ChangeState(ModAssets.GetStationIndex(button.Key) == targetBuilder.CurrentSpaceStationTypeInt ? 1 : 0);
        //        }
        //        else
        //        {

        //            button.Value.gameObject.SetActive(false);
        //        }
        //    }
        //    //UIUtils.ListAllChildren(transform);

        //    //UIUtils.ListAllChildrenWithComponents(flipButton.transform);
        //    //Debug.Log("AAAAAAAAAAAAAAAAAAAAA");
        //    var img = flipButton.transform.Find("FG").GetComponent<Image>();
        //    img.sprite = Assets.GetSprite(targetBuilder.Constructing() && targetBuilder.Demolishing() ? "action_cancel" : "action_deconstruct");
        //    var text = targetBuilder.Constructing() && !targetBuilder.Demolishing() ? "Cancel Construction" : "Start Station Construction";
        //    UIUtils.TryChangeText(PlaceStationButton.transform, "Label", text);

        //    bool canDeconstruct = targetBuilder.IsStationAtCurrentLocation();
        //    flipButton.GetComponent<KButton>().isInteractable = canDeconstruct;
        //    PlaceStationButton.GetComponent<KButton>().isInteractable = !canDeconstruct;
        //}

        private void ClearButtons()
        {
            foreach (var button in buttons)
            {
                Util.KDestroyGameObject(button.Value.gameObject);
            }

            buttons.Clear();

            //flipButton.SetActive(false);
            //PlaceStationButton.SetActive(false);
        }
    }
}
