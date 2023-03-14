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


            var l = infoLabel.GetComponent<LayoutElement>();
            l.preferredWidth = 250;
            l.minWidth = 200;
            var s = slider.GetComponent<LayoutElement>();
            s.minWidth = 500;
            s.preferredWidth = 700;
            //foreach (var p in s.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            //    {
            //        //Console.WriteLine(p+": " +p.GetValue(s, null));
            //    }

            //this.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = 500;
            //infoLabel.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 800);
            //slider.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 800);

            //percentLabel.gameObject.SetActive(true);
        }

        public void SetupSlider(float min,float current, float max, bool fullNumbers, string infoText,string desc, System.Action<float> handleOutput, bool usesMap = false)
        {
            //SgtLogger.l(min+", "+ current+", "+ max+", "+ fullNumbers);
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = fullNumbers;
            slider.onValueChanged.AddListener(new UnityAction<float>(handleOutput));
            slider.onValueChanged.AddListener(new UnityAction<float>((value)=>HandleData(value)));

            slider.value = (current);
            LabelInfoText = infoText;
            UIUtils.AddSimpleTooltipToObject(infoLabel.transform, desc,true);
            usesMapSize = usesMap;

            HandleData(current);
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        public void HandleData(object data)
        {
            float first=0,second=-1;
            if(data is float[])
            {
                var casted = (float[])data;
                first = casted[0];
                second = casted[1];
            }
            if(data is float) {
                first = (float)data;
            }
            if (second > 0)
            {
                slider.maxValue = second;
            }

            infoLabel.text = LabelInfoText + first.ToString(); 
            slider.value = first;
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
