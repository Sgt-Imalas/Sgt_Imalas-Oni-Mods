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
using static STRINGS.DUPLICANTS.STATUSITEMS;
using LogicSatellites.Behaviours;
using static LogicSatellites.Behaviours.ModAssets;
using Database;

namespace LogicSatellites
{
    internal class NewSatConstructorModuleSideScreen : SideScreenContent
    {
        [SerializeField]
        private RectTransform buttonContainer;

        private int CurrentSatelliteType = 0;

        private GameObject stateButtonPrefab;
        private GameObject PlaceStationButton;
        private GameObject flipButton;
        private Dictionary<KeyValuePair<int, SatelliteConfiguration>, MultiToggle> buttons = new Dictionary<KeyValuePair<int, SatelliteConfiguration>, MultiToggle>();

        private Clustercraft targetCraft;
        private List<ISatelliteCarrier> targetBuilders = new List<ISatelliteCarrier>();

        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Clustercraft>() != null && HasConstructor(target.GetComponent<Clustercraft>());
        private bool HasConstructor(Clustercraft craft)
        {
            foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                if (clusterModule.Get().GetSMI<ISatelliteCarrier>() != null)
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
            UIUtils.AddActionToButton(PlaceStationButton.transform, "", () => { OnConstructionButtonClicked(); RefreshButtons(); });
            UIUtils.AddActionToButton(flipButton.transform, "", () => { OnDemolishButtonClicked(); RefreshButtons(); });
        }

        void OnConstructionButtonClicked()
        {

            foreach (var carrier in this.targetBuilders)
            {
                if (carrier.CanDeploySatellite(CurrentSatelliteType))
                {
                    carrier.TryDeploySatellite(CurrentSatelliteType);
                    break;
                }
            }
            RefreshButtons();
        }
        void OnDemolishButtonClicked()
        {

            foreach (var carrier in this.targetBuilders)
            {
                if (carrier.CanRetrieveSatellite())
                {

                    carrier.TryRetrieveSatellite();
                    break;
                }
            }
            RefreshButtons();
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
                if (clusterModule.Get().GetSMI<ISatelliteCarrier>()!= null  )
                {
                    //Debug.Log("AddedCarrier");
                    targetBuilders.Add(clusterModule.Get().GetSMI<ISatelliteCarrier>());
                }
            }
            //GetPrefabStrings();
            refreshHandle.Add(this.targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.RefreshAll)));
            //BuildModules();
            //RefreshButtons();
        }

        private void RefreshAll(object data = null) => this.RefreshButtons(); 



        private void GenerateStateButtons()
        {
            ClearButtons();

            foreach (var satType in ModAssets.SatelliteConfigurations)
            {

                AddButton(satType,
                    () =>
                    {
                        CurrentSatelliteType = satType.Key;
                        RefreshButtons();
                    }
                    );
            }
            RefreshButtons();
        }


        private void AddButton(KeyValuePair<int, SatelliteConfiguration> type, System.Action onClick)
        {
            var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);

            //Debug.Log("ButtonPrefab_");
            //UIUtils.ListAllChildrenWithComponents(stateButtonPrefab.transform);

            if (gameObject.TryGetComponent(out MultiToggle button))
            {
                //Assets.TryGetAnim((HashedString)animName, out var anim);
                button.onClick += onClick;
                button.ChangeState(type.Key == CurrentSatelliteType ? 1 : 0);
                //Debug.Log(Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Kanim)));
                //Debug.Log("anim");
                UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = Def.GetUISprite(Assets.GetPrefab((Tag)type.Value.GridID)).first;
                UIUtils.AddSimpleTooltipToObject(gameObject.transform,"<b>"+ type.Value.NAME+"</b>\n"+type.Value.DESC, true);
                buttons.Add(type, button);
            }

        }

        void RefreshButtons()
        {
            foreach (var button in buttons)
            {
                //Debug.Log(targetBuilder.CurrentSpaceStationType + " <- current type, Button int -> " + button.Key);

                var tech = Db.Get().Techs.Get(button.Key.Value.TechId);

                if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || tech == null || (tech != null && tech.IsComplete()))
                {
                    button.Value.gameObject.SetActive(true);
                    button.Value.ChangeState( (button.Key.Key) == CurrentSatelliteType ? 1 : 0);
                }
                else
                {
                    button.Value.gameObject.SetActive(false);
                }
            }
            //UIUtils.ListAllChildren(transform);

            //UIUtils.ListAllChildrenWithComponents(flipButton.transform);
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAA");
            var img = flipButton.transform.Find("FG").GetComponent<Image>();
            img.sprite = Assets.GetSprite("action_deconstruct");
            var text = "Deploy Satellite";
            UIUtils.TryChangeText(PlaceStationButton.transform, "Label", text);

            flipButton.GetComponent<KButton>().isInteractable = CanRetrieveSatellites();
            PlaceStationButton.GetComponent<KButton>().isInteractable = CanDeploySatellites();
        }

        bool CanDeploySatellites()
        {
            foreach(var carrier in this.targetBuilders)
            {
                //Debug.Log("Carrier "+carrier+", Can Deploy: "+carrier.CanDeploySatellite(CurrentSatelliteType));    
                if(carrier.CanDeploySatellite(CurrentSatelliteType))
                    return true;
            }
            return false;
        }
        bool CanRetrieveSatellites()
        {
            foreach (var carrier in this.targetBuilders)
            {
                //Debug.Log("Carrier " + carrier + ", Can Retrieve: " + carrier.CanRetrieveSatellite());
                if (!carrier.HoldingSatellite())
                    return true;
            }
            return false;
        }

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
