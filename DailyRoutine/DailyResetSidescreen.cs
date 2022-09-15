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
        //public RectTransform endIndicator;

        public LocText labelHeaderDuration;

        public LocText ActiveRecipes;

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



        protected override void OnPrefabInit()
        {
            Debug.Log("Initiating Sidescreen Prefab");

            UIUtils.ListAllChildren(gameObject.transform);
            base.OnPrefabInit();

            imageInactiveZone = transform.Find("Contents/ClockContainer/Clock/RedRange").GetComponent<Image>();
            imageActiveZone = transform.Find("Contents/ClockContainer/Clock/GreenRange").GetComponent<Image>(); 
            startTime = transform.Find("Contents/GreenDurationSliderContainer/Slider").GetComponent<KSlider>();
            startTime.minValue = 0;
            startTime.maxValue = 599;
            currentTimeMarker = transform.Find("Contents/ClockContainer/Clock/CurrentTimeIndicator").rectTransform();
           
            labelHeaderDuration = transform.Find("Contents/GreenDurationSliderContainer/NumberInputField/Text Area/Text").GetComponent<LocText>();

            ActiveRecipes = transform.Find("Contents/TimeLeftText").GetComponent<LocText>();

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
                if (!targetComponent.UseCustomTime)
                {
                    targetComponent.timeToReset = 0f;
                    ChangeSetting(); 
                }
                UpdateUI(targetComponent.UseCustomTime);
            };
            UIUtils.TryChangeText(transform, "Contents/Buttons/ModeButton/Label", "Toggle Custom Time");

            var bt2 = UIUtils.TryFindComponent<KButton>(transform, "Contents/Buttons/ResetButton");
            bt2.onClick += ()=>
            {
                targetComponent.IsActive = !targetComponent.IsActive;                
                targetComponent.ChangeStoredRecipes(); 
                UpdateButtons();
            };

            ElementsToDisable.Add(transform.Find("Contents/ClockContainer").gameObject);
            ElementsToDisable.Add(transform.Find("Contents/GreenDurationLabel").gameObject);
            ElementsToDisable.Add(transform.Find("Contents/GreenDurationSliderContainer").gameObject);
            
            UpdateButtons();
            UpdateUI(false);

        }
        void UpdateButtons()
        {
            UIUtils.TryChangeText(transform, "Contents/Buttons/ResetButton/Label", targetComponent.IsActive ? "Disable Daily Routine" : "Enable Daily Routine");

            bool enabled = targetComponent.IsActive;
            ActiveRecipes.gameObject.SetActive(enabled);
            UIUtils.TryChangeText(ActiveRecipes.transform, "", targetComponent.GetFormattedRecipes());
            //Debug.Log("isactive: "+ enabled + ", toggleison: "+ bt1Toggle.isOn)
            if (enabled)
            {
                bt1Toggle.isOn = targetComponent.UseCustomTime;
            }
            else
            {
                bt1Toggle.isOn = false;
                targetComponent.UseCustomTime = false;
                bt1Toggle.Select();
                UpdateUI(false);
            }
            bt1Toggle.interactable = enabled;

        }

        public void UpdateUI(bool enableElements)
        {
            foreach (var go in ElementsToDisable)
                go.SetActive(enableElements);
        }


        protected override void OnCleanUp()
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
