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

namespace Rockets_TinyYetBig.SpaceStations
{
    internal class SpaceStationBuilderModuleSideScreen : SideScreenContent
    {
        [SerializeField]
        private RectTransform buttonContainer;

        private GameObject stateButtonPrefab;
        private GameObject PlaceStationButton;
        private GameObject flipButton;
        private Dictionary<SpaceStationWithStats, MultiToggle> buttons = new Dictionary<SpaceStationWithStats, MultiToggle>();

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

            UIUtils.AddActionToButton(PlaceStationButton.transform, "", () => targetBuilder.OnSidescreenButtonPressed());
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
                if (clusterModule.Get().TryGetComponent<SpaceStationBuilder>(out var targetb))
                {
                    targetBuilder = targetb;
                    break;
                }
            }
            //GetPrefabStrings();
            refreshHandle.Add(this.targetCraft.gameObject.Subscribe((int)GameHashes.ClusterLocationChanged, new System.Action<object>(this.RefreshAll)));
            //BuildModules();
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
                        targetBuilder.SetStationType(stationType);
                        RefreshButtons();
                        //Debug.Log("Bt pressed");

                    }
                    );
            }
            RefreshButtons();
        }


        private void AddButton(SpaceStationWithStats type, System.Action onClick)
        {
            var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);

            //Debug.Log("ButtonPrefab_");
            //UIUtils.ListAllChildrenWithComponents(stateButtonPrefab.transform);

            if (gameObject.TryGetComponent(out MultiToggle button))
            {
                //Assets.TryGetAnim((HashedString)animName, out var anim);
                button.onClick += onClick;
                button.ChangeState(ModAssets.GetStationIndex(type) == targetBuilder.CurrentSpaceStationTypeInt ? 1 : 0);
                Debug.Log(Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Kanim)));
                Debug.Log("anim");
                UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(type.Kanim));
                UIUtils.AddSimpleTooltipToObject(gameObject.transform, type.Name+"\n"+type.Description, true);
                buttons.Add(type, button);
            }

        }

        void RefreshButtons()
        {
            foreach (var button in buttons)
            {
                //Debug.Log(targetBuilder.CurrentSpaceStationType + " <- current type, Button int -> " + button.Key);


                var tech = button.Key.requiredTech;
                if (tech == null || (tech != null && tech.IsComplete()))
                {
                    button.Value.gameObject.SetActive(true);
                    button.Value.ChangeState(ModAssets.GetStationIndex(button.Key) == targetBuilder.CurrentSpaceStationTypeInt ? 1 : 0);
                }
                else
                {

                    button.Value.gameObject.SetActive(false);
                }
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
        }
    }
}
