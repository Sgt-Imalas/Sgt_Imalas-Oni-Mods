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
using static STRINGS.BUILDING.STATUSITEMS;

namespace ClusterTraitGenerationManager.SettinPrefabComps
{
    internal class SliderReusedAsInfo : KMonoBehaviour, ICustomPlanetoidSetting
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
            slider.gameObject.SetActive(false);

            var l = infoLabel.GetComponent<LayoutElement>();
            l.preferredWidth = 250;
            l.minWidth = 200;
        }

        public void SetupInfo(string infoText,string desc, Vector2I data)
        {
            LabelInfoText = infoText;
            UIUtils.AddSimpleTooltipToObject(infoLabel.transform, desc,true);
            HandleData(data);
        }
        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        public void HandleData(object data)
        {
            int first =0,second=0;
            if (data is Vector2I)
            {
                var casted = (Vector2I)data;
                first = casted.X;
                second = casted.Y;
            }
            infoLabel.text = string.Format(LabelInfoText, first.ToString(), second.ToString()); 
        }

        public void ToggleInteractable(bool interactable)
        {
            slider.interactable= interactable;            
        }
    }
}
