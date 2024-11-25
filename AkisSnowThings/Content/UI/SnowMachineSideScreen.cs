using AkisSnowThings.Content.Scripts.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;

namespace AkisSnowThings.Content.UI
{
    internal class SnowMachineSideScreen : SideScreenContent
    {
        private FSlider density;
        private FSlider speed;
        private FSlider lifeTime;
        private FSlider turbulence;

        private SnowMachine target;

        public override bool IsValidForTarget(GameObject target) => target.TryGetComponent<SnowMachine>(out _);
        

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);

            this.target = target.GetComponent<SnowMachine>();
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (target == null)
            {
                return;
            }

            density.Value = target.density;
            speed.Value = target.speed;
            turbulence.Value = target.turbulence;
            lifeTime.Value = target.lifeTime;
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

            density = transform.Find("Contents/Density/Slider").gameObject.AddComponent<FSlider>();
            speed = transform.Find("Contents/Speed/Slider").gameObject.AddComponent<FSlider>();
            lifeTime = transform.Find("Contents/Lifetime/Slider").gameObject.AddComponent<FSlider>();
            turbulence = transform.Find("Contents/Turbulence/Slider").gameObject.AddComponent<FSlider>();

            UIUtils.AddSimpleTooltipToObject(density.gameObject, STRINGS.UI.SNOWMACHINESIDESCREEN.CONTENTS.DENSITY.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(speed.gameObject, STRINGS.UI.SNOWMACHINESIDESCREEN.CONTENTS.SPEED.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(lifeTime.gameObject, STRINGS.UI.SNOWMACHINESIDESCREEN.CONTENTS.LIFETIME.TOOLTIP);
            UIUtils.AddSimpleTooltipToObject(turbulence.gameObject, STRINGS.UI.SNOWMACHINESIDESCREEN.CONTENTS.TURBULENCE.TOOLTIP);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            density.OnChange += OnSettingsChanged;
            speed.OnChange += OnSettingsChanged;
            lifeTime.OnChange += OnSettingsChanged;
            turbulence.OnChange += OnSettingsChanged;
        }

        private void OnSettingsChanged(float _)
        {
            if (target != null)
            {
                target.speed = speed.Value;
                target.turbulence = turbulence.Value;
                target.lifeTime = lifeTime.Value;
                target.density = density.Value;

                target.UpdateValues();
            }
        }
    }
}
