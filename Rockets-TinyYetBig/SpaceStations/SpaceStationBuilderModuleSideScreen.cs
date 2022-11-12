using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.CREATURES.STATS;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.SpaceStations
{
    internal class SpaceStationBuilderModuleSideScreen : SideScreenContent
    {
        [SerializeField]
        private RectTransform buttonContainer;

        private GameObject stateButtonPrefab;
        private GameObject debugVictoryButton;
        private GameObject flipButton;
        private readonly List<GameObject> buttons = new List<GameObject>();

        private Clustercraft targetCraft;

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
            //debugVictoryButton.SetActive(false);
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            titleKey = "STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONBUILDERSIDESCREEN.TITLE";
            stateButtonPrefab = transform.Find("ButtonPrefab").gameObject;
            buttonContainer = transform.Find("Content/Scroll/Grid").GetComponent<RectTransform>();
            debugVictoryButton = transform.Find("Butttons/Button").gameObject;
            flipButton = transform.Find("Butttons/FlipButton").gameObject;
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
            //GetPrefabStrings();
            //refreshHandle.Add(this.targetCraft.gameObject.Subscribe(1792516731, new System.Action<object>(this.RefreshAll)));
            //BuildModules();
        }



        // Creates clickable card buttons for all the lamp types + a randomizer button
            
        private void GenerateStateButtons()
        {
            //ClearButtons();
            //var animFile = SpaceStationInConstruction.GetComponent<KBatchedAnimController>().AnimFiles[0];

            //// random button
            //AddButton(animFile, "random_ui", STRINGS.BUILDINGS.PREFABS.DECORPACKA_MOODLAMP.VARIANT.RANDOM, () => target.SetRandom());

            //foreach (var lamp in ModDb.lampVariants.resources)
            //{
            //    AddButton(animFile, lamp.Id + "_ui", lamp.Name, 
            //        () =>
            //        Debug.Log("Bt pressed")
            //        //target.SetVariant(lamp.Id)
            //        );
            //}
        }


        private void AddButton(KAnimFile animFile, string animName, LocString tooltip, System.Action onClick)
        {
            var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);

            if (gameObject.TryGetComponent(out KButton button))
            {
                button.onClick += onClick;
                button.fgImage.sprite = Def.GetUISpriteFromMultiObjectAnim(animFile, animName);
            }

            UIUtils.AddSimpleTooltipToObject(gameObject.transform, tooltip, true);
            buttons.Add(gameObject);
        }

        private void ClearButtons()
        {
            foreach (var button in buttons)
            {
                Util.KDestroyGameObject(button);
            }

            buttons.Clear();

            //flipButton.SetActive(false);
            //debugVictoryButton.SetActive(false);
        }
    }
}
