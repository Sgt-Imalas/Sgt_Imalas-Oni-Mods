using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static HoverTextDrawer;
using static KAnim.Anim;

namespace BawoonFwiend
{
    internal class BalloonStationSkinSelectorSidescreen : SideScreenContent
    {
        [SerializeField]
        private RectTransform buttonContainer;

        private GameObject stateButtonPrefab;
        private GameObject PlaceStationButton;
        private GameObject flipButton;
        private Dictionary<BalloonOverrideSymbol,MultiToggle> buttons = new Dictionary<BalloonOverrideSymbol,MultiToggle>();
        //private Dictionary<SpaceStationWithStats, MultiToggle> buttons = new Dictionary<SpaceStationWithStats, MultiToggle>();

        public Bawoongiver TargetBalloonStand;
        Image RandomButtonImage;


        public override bool IsValidForTarget(GameObject target) => target.GetComponent<Bawoongiver>() != null;
       
        public override void OnSpawn()
        {
            base.OnSpawn();
            // the monument screen used here has 2 extra buttons that are not needed, disabling them
            //flipButton.SetActive(false);
            //PlaceStationButton.SetActive(false);
            //UIUtils.TryChangeText(PlaceStationButton.transform, "Label", "MakeOrBreakSpaceStation");
            RandomButtonImage = flipButton.transform.Find("FG").GetComponent<Image>();


            if(PlaceStationButton.TryGetComponent<KImage>(out var sourceBtn) && flipButton.TryGetComponent<KImage>(out var targetButton))
            {
                targetButton.colorStyleSetting = (sourceBtn.colorStyleSetting);
                targetButton.ApplyColorStyleSetting();
            }

            UIUtils.AddActionToButton(PlaceStationButton.transform, "", () => { TargetBalloonStand.ToggleAll(); RefreshButtons(); });
            UIUtils.AddActionToButton(flipButton.transform, "", () => { TargetBalloonStand.ToggleFullyRandom(); RefreshButtons(); });
            //Game.Instance.Subscribe((int)GameHashes.ResearchComplete, RefreshAll);
            //Game.Instance.Subscribe((int)GameHashes.ToggleSandbox, RefreshAll);
            RefreshButtons();
        }

        public override void OnPrefabInit()
        {

            //UIUtils.ListAllChildren(this.transform);

            base.OnPrefabInit();
            titleKey = "STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TITLE";
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

            if(target.TryGetComponent<Bawoongiver>(out var bloon))
            {
                TargetBalloonStand= bloon;
            }
        }

        // Creates clickable card buttons for all the lamp types + a randomizer button

        private void GenerateStateButtons()
        {
            ClearButtons();
            foreach (var BallonSkin in TargetBalloonStand.EnabledBalloonSkins)
            {
                AddButton(BallonSkin.Key, BallonSkin.Value,
                () =>
                    {
                        TargetBalloonStand.ToggleSkin(BallonSkin.Key);
                        RefreshButtons();
                    }
                );
            }
        }

        void RefreshButtons()
        {

            foreach (var skin in TargetBalloonStand.EnabledBalloonSkins)
            {
                if (buttons.ContainsKey(skin.Key))
                {
                    buttons[skin.Key].ChangeState(skin.Value ? 1 : 0);
                }
            }
            UIUtils.TryChangeText(PlaceStationButton.transform, "Label", TargetBalloonStand.ToggleAllBtnOn
                ? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TOGGLEALLOFF
                : STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.TOGGLEALLON);

            RandomButtonImage.sprite = TargetBalloonStand.AllRandom 
                ? Assets.GetSprite("icon_balloon_toggle_random_icon")
                :Assets.GetSprite("icon_archetype_random");

            
            UIUtils.AddSimpleTooltipToObject(flipButton.transform.Find("FG"), TargetBalloonStand.AllRandom
                ? STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ALLRANDOMYES
                : STRINGS.UI.UISIDESCREENS.BF_BALLOONSTAND.ALLRANDOMNO, true);
        }

        private void AddButton(BalloonOverrideSymbol BallonSkin, bool enabled,System.Action onClick)
        {
            var gameObject = Util.KInstantiateUI(stateButtonPrefab, buttonContainer.gameObject, true);


            if (gameObject.TryGetComponent(out MultiToggle button))
            {
                //Assets.TryGetAnim((HashedString)animName, out var anim);
                button.onClick += onClick;
                button.ChangeState(enabled ? 1 : 0);
                var BallonSprite = GetSpriteFrom(BallonSkin.animFile.Unwrap(), BallonSkin.symbol.Unwrap());
                UIUtils.TryFindComponent<Image>(gameObject.transform, "FG").sprite = BallonSprite;
                buttons.Add(BallonSkin, button);
            }
        }

        //public static Sprite GetBloonUISprite(string animationID, string spriteID)
        //{
        //    return Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(Db.GetBuildingFacades().Get(animationID).AnimFile));
        //}

        Dictionary<KeyValuePair<KAnimFile, KAnim.Build.Symbol>, Sprite> BalloonSprites 
            = new Dictionary<KeyValuePair<KAnimFile, KAnim.Build.Symbol>, Sprite>();

        Sprite GetSpriteFrom(KAnimFile animFile, KAnim.Build.Symbol symbol)
        {
            var SpriteKey = new KeyValuePair<KAnimFile, KAnim.Build.Symbol>(animFile, symbol);
            if (BalloonSprites.TryGetValue(SpriteKey, out var spriteFound))
                return spriteFound;


            KAnimFileData data = animFile.GetData();
            int frame2 = default(KAnim.Anim.FrameElement).frame; 
            KAnim.Build.SymbolFrame symbolFrame = symbol.GetFrame(frame2).symbolFrame;
            if (symbolFrame == null)
            {
                SgtLogger.l("SymbolFrame ["+ frame2+"] is missing");
                return Assets.GetSprite("unknown");
            }

            Texture2D texture = data.build.GetTexture(0);
            Debug.Assert(texture != null, "Invalid texture on " + animFile.name);
            float x = symbolFrame.uvMin.x;
            float x2 = symbolFrame.uvMax.x;
            float y = symbolFrame.uvMax.y;
            float y2 = symbolFrame.uvMin.y;
            int num = (int)((float)texture.width * Mathf.Abs(x2 - x));
            int num2 = (int)((float)texture.height * Mathf.Abs(y2 - y));
            float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
            Rect rect = default(Rect);
            rect.width = num;
            rect.height = num2;
            rect.x = (int)((float)texture.width * x);
            rect.y = (int)((float)texture.height * y);
            float pixelsPerUnit = 100f;
            if (num != 0)
            {
                pixelsPerUnit = 100f / (num3 / (float)num);
            }

            Sprite sprite = Sprite.Create(texture, rect, false ? new Vector2(0.5f, 0.5f) : Vector2.zero, pixelsPerUnit, 0u, SpriteMeshType.FullRect);
            BalloonSprites.Add(SpriteKey,sprite);
            return sprite;
        }
        private void ClearButtons()
        {
            foreach (var button in buttons)
            {
                Util.KDestroyGameObject(button.Value.gameObject);
            }

            buttons.Clear();
        }
    }
}