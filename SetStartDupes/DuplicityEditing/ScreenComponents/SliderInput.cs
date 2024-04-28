using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
    internal class SliderInput : KMonoBehaviour
    {
        public string TextLeft, TextRight;
        LocText label;
        FSlider slider;
        public bool wholeNumbers = true;
        public System.Action<float> OnSliderValueChanged;

        public override void OnPrefabInit()
        {
            base.OnSpawn();
            label = transform.Find("Descriptor/Label").GetComponent<LocText>();
            label.SetText(TextLeft);

            slider = transform.Find("Slider").FindOrAddComponent<FSlider>();

            slider.SetWholeNumbers(wholeNumbers);
            slider.AttachOutputField(transform.Find("Descriptor/Output").GetComponent<LocText>());
            slider.OnChange += OnSliderValueChanged;
        }
        public void SetMinMaxCurrent(float min, float max, float current)
        {
            slider?.SetMinMaxCurrent(min, max, current); 
        }
    }
}

