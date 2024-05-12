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
        public string Text;
        LocText label;
        FSlider slider;
        public bool wholeNumbers = true;
        public System.Action<float> OnSliderValueChanged;
        public float min, max, current;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            label = transform.Find("Descriptor/Label").GetComponent<LocText>();
            label.SetText(Text);

            slider = transform.Find("Slider").FindOrAddComponent<FSlider>();

            slider.SetWholeNumbers(wholeNumbers);
            slider.AttachOutputField(transform.Find("Descriptor/Output").GetComponent<LocText>());
            slider.OnChange += OnSliderValueChanged;

            slider.SetMinMaxCurrent(min, max, current);
        }
        public void SetMinMaxCurrent(float min, float max, float current)
        {
            this.min  = min;
            this.max = max;
            this.current = current;
            slider?.SetMinMaxCurrent(min, max, current);
        }
        public void SetCurrent(float current)
        {
            slider?.SetMinMaxCurrent(min, max, current);
        }
    }
}

