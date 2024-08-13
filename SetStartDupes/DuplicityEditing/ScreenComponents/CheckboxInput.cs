using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
    internal class CheckboxInput:KMonoBehaviour
    {
        public string Text;
        LocText label;
        public System.Action<bool> OnCheckboxToggled;
        FToggle checkboxToggle;

        public override void OnPrefabInit()
        {
            base.OnSpawn();
            label = transform.Find("Label").GetComponent<LocText>();
            label.SetText(Text);

            checkboxToggle = transform.Find("Background").gameObject.AddOrGet<FToggle>();
            checkboxToggle.SetCheckmark("Checkmark");
            checkboxToggle.OnClick += OnCheckboxToggled;
        }
        public void SetCheckboxValue(bool value)
        {
            checkboxToggle?.SetOn(value);
        }
    }
}
