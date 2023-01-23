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
using static LogicSatellites.STRINGS.ITEMS.SATELLITE;

namespace LogicSatellites
{
    internal class SatelliteTypeSelectionSidescreen : SideScreenContent
    {
        [SerializeField]
        private RectTransform buttonContainer;

        private GameObject stateButtonPrefab;
        private GameObject PlaceStationButton;
        private GameObject flipButton;
        private Dictionary<KeyValuePair<int, SatelliteConfiguration>, MultiToggle> buttons = new Dictionary<KeyValuePair<int, SatelliteConfiguration>, MultiToggle>();

        private ISatelliteCarrier targetSatelliteCarrier;

        public override bool IsValidForTarget(GameObject target) => target.GetSMI<ISatelliteCarrier>() != null;
        private bool HasConstructor(Clustercraft craft)
        {
            foreach (Ref<RocketModuleCluster> clusterModule in craft.GetComponent<CraftModuleInterface>().ClusterModules)
            {
                if (clusterModule.Get().GetSMI<ISatelliteCarrier>() != null)
                    return true;
            }
            return false;
        }
        public override int GetSideScreenSortOrder() => 21;
        public override void OnSpawn()
        {
            base.OnSpawn();
            // the monument screen used here has 2 extra buttons that are not needed, disabling them
            //flipButton.SetActive(false);
            //PlaceStationButton.SetActive(false);
            UIUtils.TryChangeText(PlaceStationButton.transform, "Label", "Display Current Satellite Type");
            RefreshButtons();
            UIUtils.AddActionToButton(PlaceStationButton.transform, "", () => { OnConstructionButtonClicked(); RefreshButtons(); });
            UIUtils.AddActionToButton(flipButton.transform, "", () => { OnDemolishButtonClicked(); RefreshButtons(); });
            Game.Instance.Subscribe((int)GameHashes.ResearchComplete, this.RefreshAll);
            Game.Instance.Subscribe((int)GameHashes.ToggleSandbox, this.RefreshAll);
        }

        void OnConstructionButtonClicked()
        {
            RefreshButtons();
        }
        void OnDemolishButtonClicked()
        {
            targetSatelliteCarrier.EjectParts();
            RefreshButtons();
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            titleKey = "STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLE";
            stateButtonPrefab = transform.Find("ButtonPrefab").gameObject;
            buttonContainer = transform.Find("Content/Scroll/Grid").GetComponent<RectTransform>();
            PlaceStationButton = transform.Find("Butttons/ApplyButton").gameObject;
            flipButton = transform.Find("Butttons/ClearStyleButton").gameObject;
            GenerateStateButtons();
            var img = flipButton.transform.Find("FG").GetComponent<Image>();
            img.sprite = Assets.GetSprite("action_deconstruct");

            //UIUtils.ListAllChildrenWithComponents(transform);

            PlaceStationButton.GetComponent<ToolTip>().enabled = false;
            Destroy(flipButton.GetComponent<ToolTip>());
            UIUtils.AddSimpleTooltipToObject(flipButton.transform.Find("FG"), (string)STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.SCRAPSATELLITETOOLTIP, true);

            RefreshButtons();
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

            targetSatelliteCarrier = target.GetSMI<ISatelliteCarrier>();
            refreshHandle.Add(target.Subscribe((int)GameHashes.ResearchComplete, RefreshAll)); 
            RefreshButtons();
        }

        private void RefreshAll(object data = null) => this.RefreshButtons(); 



        private void GenerateStateButtons()
        {
            ClearButtons();

            foreach (var satType in ModAssets.SatelliteConfigurations)
            {
                if(satType.Key==0)
                AddButton(satType,
                    () =>
                    {
                        targetSatelliteCarrier.SetSatelliteType(satType.Key);
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
                button.ChangeState(type.Key == targetSatelliteCarrier.SatelliteType() ? 1 : 0);
                //Debug.Log(Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Kanim)));
                //Debug.Log("anim");
                UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = Def.GetUISprite(Assets.GetPrefab((Tag)type.Value.GridID)).first;
                UIUtils.AddSimpleTooltipToObject(gameObject.transform,"<b>"+ type.Value.NAME+"</b>\n"+type.Value.DESC, true);
                buttons.Add(type, button);
            }

        }

        void RefreshButtons()
        {
            int SatType = 0;
            SatType = targetSatelliteCarrier.SatelliteType();

            foreach (var button in buttons)
            {
                //Debug.Log(targetBuilder.CurrentSpaceStationType + " <- current type, Button int -> " + button.Key);

                var tech = Db.Get().Techs.Get(button.Key.Value.TechId);

                //Debug.Log("Tech -> " + tech+", researched? "+tech.IsComplete());
                if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || tech == null || (tech != null && tech.IsComplete()))
                {
                    button.Value.gameObject.SetActive(true);
                    button.Value.ChangeState( (button.Key.Key) == SatType ? 1 : 0);
                }
                else
                {
                    button.Value.gameObject.SetActive(false);
                }
            }
            //UIUtils.ListAllChildren(transform);

            //UIUtils.ListAllChildrenWithComponents(flipButton.transform);
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAA");
            //var text = "Deploy Satellite";
            if (PlaceStationButton != null)
            {
                UIUtils.ListAllChildren(PlaceStationButton.transform);
                UIUtils.TryChangeText(PlaceStationButton.transform, "Label", targetSatelliteCarrier.HoldingSatellite() ? (ModAssets.SatelliteConfigurations[SatType].NAME) : (string)STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLELABEL_HASSAT_FALSE);
                //UIUtils.AddSimpleTooltipToObject(PlaceStationButton.transform, targetSatelliteCarrier.HoldingSatellite() ? (ModAssets.SatelliteConfigurations[SatType].DESC) : (string)STRINGS.UI.UISIDESCREENS.SATELLITECARRIER_SIDESCREEN.TITLELABEL_HASSAT_FALSE, true);
                flipButton.GetComponent<KButton>().isInteractable = targetSatelliteCarrier.HoldingSatellite();
            }

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
            PlaceStationButton.GetComponent<KButton>().isInteractable = false;
        }
    }
}
