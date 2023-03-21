using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UtilLibs;

namespace DailyRoutine
{
    class DailyResetSidescreen : SideScreenContent, IRender200ms
    {
        public DR_ResetComponent targetComponent;
       

        public Image imageInactiveZone;
        public Image imageActiveZone;

        public KSlider startTime;
        public KToggle bt1Toggle;
        public KToggle RecipeMode;
        //public RectTransform endIndicator;

        public LocText labelHeaderDuration;

        public LocText ActiveRecipesCountInfo;


        public LocText timeResetInfo;

        public RectTransform currentTimeMarker;
        public KNumberInputField TimeInput;

        const float width = 0.014f;
        List<GameObject> ElementsToDisable = new();


        public override bool IsValidForTarget(GameObject target) => (target.GetComponent<DR_ResetComponent>() != null);

        public override void SetTarget(GameObject target)
        {
            //Debug.Log("Setting Target");
            targetComponent = target.GetComponent<DR_ResetComponent>();
            targetComponent.fabricator = target.GetComponent<ComplexFabricator>();
            //Debug.Log("Target: " + targetComponent);
            if (this.IsInitialized())
            {
                UpdateUI(targetComponent.UseCustomTime);
                this.ChangeSetting(targetComponent.timeToReset);
                UpdateButtons();
            }
        }

        private void ChangeSetting(float newVal = 0)
        {
            targetComponent.timeToReset = newVal;
            this.imageActiveZone.rectTransform.rotation = Quaternion.identity;
            this.imageActiveZone.rectTransform.Rotate(0.0f, 0.0f, this.NormalizedValueToDegrees((targetComponent.timeToReset/600) + width));
            TimeInput.SetAmount(newVal);
            startTime.value = newVal;
        }



        public override void OnPrefabInit()
        {
            Debug.Log("Initiating Sidescreen Prefab");
#if DEBUG
            //UIUtils.ListAllChildren(gameObject.transform);
#endif   
            base.OnPrefabInit();

            imageInactiveZone = transform.Find("Contents/ClockContainer/Clock/RedRange").GetComponent<Image>();
            imageActiveZone = transform.Find("Contents/ClockContainer/Clock/GreenRange").GetComponent<Image>(); 
            startTime = transform.Find("Contents/GreenDurationSliderContainer/Slider").GetComponent<KSlider>();
            startTime.minValue = 0;
            startTime.maxValue = 599;
            currentTimeMarker = transform.Find("Contents/ClockContainer/Clock/CurrentTimeIndicator").rectTransform();
           
            labelHeaderDuration = transform.Find("Contents/GreenDurationSliderContainer/NumberInputField/Text Area/Text").GetComponent<LocText>();

            UIUtils.FindAndDisable(transform, "Contents/TimeLeftText");
            //ActiveRecipesCountInfo = transform.Find("Contents/TimeLeftText").GetComponent<LocText>();

            timeResetInfo= transform.Find("Contents/GreenDurationLabel").GetComponent<LocText>();
            timeResetInfo.text = STRINGS.UISTRINGS.TimeLeftText;


            TimeInput = transform.Find("Contents/GreenDurationSliderContainer/NumberInputField").GetComponent<KNumberInputField>();
            TimeInput.minValue = 0f;
            TimeInput.maxValue = 599f;
            TimeInput.currentValue = targetComponent.timeToReset;
            TimeInput.decimalPlaces = 0;
            TimeInput.onEndEdit += (System.Action)(() =>
            {
                float result = TimeInput.currentValue;
                this.ChangeSetting(result);
                this.startTime.value = result;
            });
                
            transform.Find("Contents/RedDurationSliderContainer").gameObject.SetActive(false);
            transform.Find("Contents/RedDurationLabel").gameObject.SetActive(false);
            transform.Find("Contents/TimeLeftText").gameObject.SetActive(false);

            var clock = transform.Find("Contents/ClockContainer/Clock").gameObject;
            var clockbg = Util.KInstantiateUI(new GameObject(), clock, true);

            var imgbg = (KImage)clockbg.AddOrGet<KImage>();
            imgbg.sprite = Assets.GetSprite("asteroid_underlay");
            imgbg.transform.localScale = new Vector3(1.2f, 1.2f);

            var img = (KImage)clock.AddOrGet<KImage>();
            img.sprite = Assets.GetSprite("asteroid_underlay");

            imgbg.type = Image.Type.Filled;
            imgbg.fillMethod = Image.FillMethod.Radial360;
            imgbg.fillAmount = 0.125f;
            imgbg.fillOrigin = (int)Image.Origin360.Top;
            imgbg.fillClockwise = true;
            imgbg.color = GlobalAssets.Instance.colorSet.powerBuildingDisabled;


            this.imageActiveZone.color = (Color)GlobalAssets.Instance.colorSet.logicOnSidescreen;
            this.imageInactiveZone.color = (Color)GlobalAssets.Instance.colorSet.logicOffSidescreen;

            this.startTime.onValueChanged.RemoveAllListeners(); 
            this.imageActiveZone.fillAmount = width;

            this.startTime.value = this.targetComponent.timeToReset;
            this.startTime.onValueChanged.AddListener((UnityAction<float>)(value => this.ChangeSetting(value)));



            
            bt1Toggle = UIUtils.TryFindComponent<KToggle>(transform, "Contents/Buttons/ModeButton");

            bt1Toggle.onClick += () =>
            {
                targetComponent.UseCustomTime = !targetComponent.UseCustomTime;

                UpdateUI(targetComponent.UseCustomTime);
                UpdateButtons();
            };
            UIUtils.TryChangeText(transform, "Contents/Buttons/ModeButton/Label", "Toggle Custom Time");
            UIUtils.AddSimpleTooltipToObject(bt1Toggle.transform, "By default, the recipes reset at\nthe start of each cycle.\nToggle to adjust the time of that reset.");



            var bt2 = UIUtils.TryFindComponent<KButton>(transform, "Contents/Buttons/ResetButton");

            bt2.onClick += ()=>
            {
                targetComponent.IsActive = !targetComponent.IsActive;                
                targetComponent.ChangeStoredRecipes();

                if (!targetComponent.IsActive)
                {
                    targetComponent.UseCustomTime = false;
                }
                UpdateButtons();
            };

            var buttonContainer = Util.KInstantiateUI(transform.Find("Contents/Buttons").gameObject, transform.Find("Contents").gameObject, true);
            buttonContainer.name = "Container2";

            UIUtils.AddActionToButton(buttonContainer.transform, "ResetButton", () =>
            {

                targetComponent.ChangeStoredRecipes();
                UpdateButtons();
                //Manager.Dialog(Global.Instance.globalCanvas,"Recipes for Daily Routine",targetComponent.GetFormattedRecipes());
            }, 
            true);

            ActiveRecipesCountInfo = UIUtils.TryFindComponent<LocText>(buttonContainer.transform, "ResetButton/Label");

            RecipeMode = UIUtils.TryFindComponent<KToggle>(buttonContainer.transform, "ModeButton");

            RecipeMode.onClick += () =>
            {
                targetComponent.QueueRecipes = !targetComponent.QueueRecipes;
            };
            UIUtils.TryChangeText(buttonContainer.transform, "ModeButton/Label", "Toggle Recipe Queueing");
            UIUtils.AddSimpleTooltipToObject(RecipeMode.transform, "When disabled, the recipe count for each recipe will be set to the stored value regardless of remaining recipe count.\nEnabling this option instead adds the amount to the existing count instead.");



            //transform.Find("Contents/ClockContainer").localScale = new(0.5f, 0.5f);

            ElementsToDisable.Add(transform.Find("Contents/ClockContainer").gameObject);
            ElementsToDisable.Add(transform.Find("Contents/GreenDurationLabel").gameObject);
            ElementsToDisable.Add(transform.Find("Contents/GreenDurationSliderContainer").gameObject);

            transform.Find("Contents/Buttons").SetAsLastSibling();

            UpdateButtons();
            UpdateUI(false);
            var vlg = UIUtils.TryFindComponent<VerticalLayoutGroup>(transform);
            vlg.childForceExpandHeight = false;

        }
        void UpdateButtons()
        {
            UIUtils.TryChangeText(transform, "Contents/Buttons/ResetButton/Label", targetComponent.IsActive ? "Disable Daily Routine" : "Enable Daily Routine");

            bool enabled = targetComponent.IsActive;
            //ActiveRecipesCountInfo.gameObject.GetComponent<KButton>().isInteractable = enabled;
            UIUtils.TryChangeText(ActiveRecipesCountInfo.transform, "", "Update Daily Tasks\n(Hover to see current)");
            UIUtils.AddSimpleTooltipToObject(ActiveRecipesCountInfo.transform, targetComponent.GetFormattedRecipes());

            //Debug.Log("isactive: "+ enabled + ", toggleison: "+ bt1Toggle.isOn)
            if (enabled)
            {
                bt1Toggle.isOn = targetComponent.UseCustomTime;
                RecipeMode.isOn = targetComponent.QueueRecipes;
                
            }
            else
            {
                bt1Toggle.isOn = false;
                RecipeMode.isOn = false;

                targetComponent.UseCustomTime = false;
                targetComponent.QueueRecipes = false;

                UpdateUI(false);
            }
            Debug.Log("hide? " + RecipeMode.isOn);
            RecipeMode.interactable = enabled;
            bt1Toggle.interactable = enabled;

        }

        public void UpdateUI(bool enableElements)
        {
            foreach (var go in ElementsToDisable)
                go.SetActive(enableElements);
        }


        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        public void Render200ms(float dt)
        {
            if (targetComponent != null && targetComponent.IsActive) 
            { 
                this.currentTimeMarker.rotation = Quaternion.identity;
                this.currentTimeMarker.Rotate(0.0f, 0.0f, this.NormalizedValueToDegrees(GameClock.Instance.GetCurrentCycleAsPercentage()));
               
            }
        }
        private float NormalizedValueToDegrees(float value) => 360f * value;
       
    }
}
