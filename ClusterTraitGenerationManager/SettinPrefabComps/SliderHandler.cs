using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UtilLibs;

namespace ClusterTraitGenerationManager.SettinPrefabComps
{
    internal class SliderHandler : KMonoBehaviour, ICustomPlanetoidSetting
    {
        Slider slider;
        LocText infoLabel;
        LocText percentLabel;
        string LabelInfoText;
        bool usesMapSize = false;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            slider = UIUtils.TryFindComponent<Slider>(transform, "Slider");
            infoLabel = UIUtils.TryFindComponent<LocText>(transform, "Label");
            percentLabel = UIUtils.TryFindComponent<LocText>(transform, "PercentLabel");
            percentLabel.gameObject.SetActive(true);
        }

        public void SetupSlider(float min,float current, float max, bool fullNumbers, string infoText, System.Action<float> handleOutput, bool usesMap = false)
        {
            SgtLogger.l(min+", "+ current+", "+ max+", "+ fullNumbers);
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = fullNumbers;
            slider.onValueChanged.AddListener(new UnityAction<float>(handleOutput));
            slider.onValueChanged.AddListener(new UnityAction<float>((value)=>HandleData(value)));

            slider.value = (current);
            LabelInfoText = infoText;
            usesMapSize = usesMap;

            HandleData(current);
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        public void HandleData(object data)
        {
            infoLabel.text = LabelInfoText + data.ToString(); 
            slider.value = (float)data;
            if(usesMapSize)
            {
                slider.maxValue = CGSMClusterManager.CustomCluster.Rings;
                if(slider.value>slider.maxValue) 
                    slider.value = slider.maxValue;
            }
        }

        public void ToggleInteractable(bool interactable)
        {
            slider.interactable= interactable;
        }
    }
}
