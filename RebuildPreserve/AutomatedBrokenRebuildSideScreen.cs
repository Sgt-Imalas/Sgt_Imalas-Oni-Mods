using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RebuildPreserve
{
    internal class AutomatedBrokenRebuildSideScreen : SideScreenContent
    {
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class AutomatedBrokenRebuildSideScreen_AddToDetailsScreen
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                //foreach(var _sideScreen in ___sideScreens)
                //    SgtLogger.l(_sideScreen.GetType().ToString(), _sideScreen.name);

                UIUtils.AddClonedSideScreen<AutomatedBrokenRebuildSideScreen>
                   ("AutomatedBrokenRebuildSideScreen", "Automatable Side Screen", typeof(AutomatableSideScreen));

            }
        }
        public KToggle allowManualToggle;

        public KImage allowManualToggleCheckMark;

        public GameObject content;

        public GameObject target;

        public LocText DescriptionText;

        public AutomatedBrokenRebuild targetCmp;

        public override int GetSideScreenSortOrder() => -20;

        public override bool IsValidForTarget(GameObject target)
        {
            return target.TryGetComponent<AutomatedBrokenRebuild>(out _);
        }
        public override void SetTarget(GameObject target)
        {
            Init();
            base.SetTarget(target);
            if (target == null)
            {
                Debug.LogError("The target object provided was null");
                return;
            }

            if (!target.TryGetComponent<AutomatedBrokenRebuild>(out targetCmp))
            {
                Debug.LogError("The target provided does not have an AutomatedBrokenRebuild component");
                return;
            }

            allowManualToggle.isOn = targetCmp.RebuildOnBreaking;
            allowManualToggleCheckMark.enabled = allowManualToggle.isOn;
        }

        public void OnAllowManualChanged(bool value)
        {
            targetCmp.RebuildOnBreaking = value;
            allowManualToggleCheckMark.enabled = value;
        }
        public override void OnSpawn()
        {
            Init();
            base.OnSpawn();
        }

        bool init = false;
        void Init()
        {
            if (init) return;
            init = true;

            this.titleKey = STRINGS.PRESERVESETTINGSSIDESCREEN.HEADER.key.String; 
            transform.Find("Contents/AllowManualCheckbox").GetComponent<ToolTip>().SetSimpleTooltip(STRINGS.PRESERVESETTINGSSIDESCREEN.TOOLTIP);

            allowManualToggle = transform.Find("Contents/AllowManualCheckbox/CheckBox").GetComponent<KToggle>() ;

            allowManualToggleCheckMark = transform.Find("Contents/AllowManualCheckbox/CheckBox/CheckMark").GetComponent<KImage>();

            content = transform.Find("Contents").gameObject;

            DescriptionText = transform.Find("Contents/AllowManualCheckbox/Label").GetComponent<LocText>();
            DescriptionText.key =STRINGS.PRESERVESETTINGSSIDESCREEN.LABEL.key.String;
            allowManualToggle.onValueChanged += OnAllowManualChanged;
        }
    }
}
