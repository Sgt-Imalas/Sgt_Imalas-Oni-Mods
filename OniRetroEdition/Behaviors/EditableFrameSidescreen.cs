//using Database;
//using OniRetroEdition.Behaviors;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;
//using UtilLibs;
//using static HoverTextDrawer;
//using static KAnim.Anim;

//namespace BawoonFwiend
//{
//    internal class EditableFrameSidescreen : SideScreenContent
//    {
//        [SerializeField]
//        private RectTransform buttonContainer;

//        private GameObject stateButtonPrefab;
//        //private GameObject PlaceStationButton;
//        //private GameObject ToggleManualDeliveryButton;
//        //private GameObject flipButton;
//        private Dictionary<int, MultiToggle> buttons = new ();

//        public EditableFrame TargetFrame;
//        //Image RandomButtonImage;


//        public override bool IsValidForTarget(GameObject target) => target.TryGetComponent<EditableFrame>(out _);

//        public override void OnSpawn()
//        {
//            base.OnSpawn();
           
//            //RandomButtonImage = flipButton.transform.Find("FG").GetComponent<Image>();

//            //if (PlaceStationButton.TryGetComponent<KImage>(out var sourceBtn) && flipButton.TryGetComponent<KImage>(out var targetButton))
//            //{
//            //    targetButton.colorStyleSetting = (sourceBtn.colorStyleSetting);
//            //    targetButton.ApplyColorStyleSetting();
//            //}
            
//            //UIUtils.AddActionToButton(ToggleManualDeliveryButton.transform, "", () => { TargetBalloonStand.ToggleManualDelivery(); RefreshButtons(); });
//            //UIUtils.AddActionToButton(PlaceStationButton.transform, "", () => { TargetBalloonStand.ToggleAll(); RefreshButtons(); });
//            //UIUtils.AddActionToButton(flipButton.transform, "", () => { TargetBalloonStand.ToggleFullyRandom(); RefreshButtons(); });
//        }

//        public override void OnPrefabInit()
//        {
//            base.OnPrefabInit();
//            InitLinks();
//            RefreshStrings();

//        }

//        bool init = false;
//        void InitLinks()
//        {
//            if (init) return;

//            titleKey = "STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TITLE";
//            stateButtonPrefab = transform.Find("ButtonPrefab").gameObject;
//            buttonContainer = transform.Find("Content/Scroll/Grid").GetComponent<RectTransform>();

//            var buttons = transform.Find("Butttons");
//            buttons.gameObject.SetActive(false);


//            GenerateStateButtons();

//            //PlaceStationButton.GetComponent<ToolTip>().enabled = false;
//            //Destroy(flipButton.GetComponent<ToolTip>());
//            //UIUtils.AddSimpleTooltipToObject(flipButton.transform.Find("FG"), "", true);
//            init = true;
//        }

//        private List<int> refreshHandle = new List<int>();
//        public override void SetTarget(GameObject target)
//        {
//            if (target != null)
//            {
//                foreach (int id in this.refreshHandle)
//                    target.Unsubscribe(id);
//                refreshHandle.Clear();

//                base.SetTarget(target);

//                if (target.TryGetComponent(out TargetFrame))
//                {
//                    InitLinks();
//                    //GenerateStateButtons();
//                    GenerateStateButtons();
//                    RefreshButtons();
//                }
//            }
//        }

//        void RefreshStrings()
//        {
//            bool toggleAll = TargetFrame == null ? false : TargetFrame.ToggleAllBtnOn;
//            bool manualDeliveryEnabled = TargetFrame == null ? false : TargetFrame.UseManualDelivery;
//            bool toggleAllRandom = TargetFrame == null ? false : TargetFrame.AllRandom;
//            UIUtils.TryChangeText(PlaceStationButton.transform, "Label", toggleAll
//                ? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TOGGLEALLOFF
//                : STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TOGGLEALLON);

//            RandomButtonImage.sprite = toggleAllRandom
//                ? Assets.GetSprite("icon_balloon_toggle_random_icon")
//                : Assets.GetSprite("icon_archetype_random");


//            UIUtils.AddSimpleTooltipToObject(flipButton.transform.Find("FG"), toggleAllRandom
//                ? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ALLRANDOMYES
//                : STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ALLRANDOMNO);

//            UIUtils.TryChangeText(ToggleManualDeliveryButton.transform, "Label", manualDeliveryEnabled
//                ? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.DISABLEMANUALDELIVERY
//                : STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ENABLEMANUALDELIVERY);

//            UIUtils.AddSimpleTooltipToObject(ToggleManualDeliveryButton.transform, manualDeliveryEnabled
//                ? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.DISABLEMANUALDELIVERYTOOLTIP
//                : STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ENABLEMANUALDELIVERYTOOLTIP);
//        }

//        // Creates clickable card buttons for all the balloon skin types
//        private void GenerateStateButtons()
//        {
//            ClearButtons();
//            foreach (var BallonSkin in TargetFrame.EnabledBalloonSkins)
//            {
//                AddButton(BallonSkin.Key, BallonSkin.Value,
//                () =>
//                    {
//                        TargetFrame.SelectSymbol(BallonSkin.Key);
//                        RefreshButtons();
//                    }
//                );
//            }
//        }

//        /// <summary>
//        /// Refresh activation states of buttons
//        /// </summary>
//        void RefreshButtons()
//        {
//            foreach (var skin in TargetFrame.EnabledBalloonSkins)
//            {
//                if (buttons.ContainsKey(skin.Key))
//                {
//                    buttons[skin.Key].ChangeState(skin.Value ? 1 : 0);
//                }
//            }
//            RefreshStrings();
//        }

//        private void AddButton(int FrameSymbol, bool enabled, System.Action onClick)
//        {
//            var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);
//            if (gameObject.TryGetComponent(out MultiToggle button))
//            {
//                //Assets.TryGetAnim((HashedString)animName, out var anim);
//                button.onClick += onClick;
//                button.ChangeState(enabled ? 1 : 0);
//                var SkinOverride = TargetFrame.GetOverrideViaIndex(FrameSymbol);
//                var BallonSprite = ModAssets.GetSpriteFrom(SkinOverride.animFile.Unwrap(), SkinOverride.symbol.Unwrap());
//                UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = BallonSprite;
//                buttons.Add(FrameSymbol, button);
//            }
//        }

//        private void ClearButtons()
//        {
//            foreach (var button in buttons)
//            {
//                Util.KDestroyGameObject(button.Value.gameObject);
//            }
//            buttons.Clear();
//        }
//    }
//}