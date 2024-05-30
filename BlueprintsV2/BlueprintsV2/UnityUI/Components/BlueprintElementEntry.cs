using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
    internal class BlueprintElementEntry : KMonoBehaviour
    {
        LocText ElementName;
        LocText ElementAmount;
        LocText ReplaceElementName;
        GameObject warningIndicator, severeWarningIndicator;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ElementName = transform.Find("Label").gameObject.GetComponent<LocText>();
            ElementAmount = transform.Find("MassLabel").gameObject.GetComponent<LocText>();
            ReplaceElementName = transform.Find("ReplacementElement").gameObject.GetComponent<LocText>();
            warningIndicator = transform.Find("Warning").gameObject;
            severeWarningIndicator = transform.Find("WarningSevere").gameObject;

        }
        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public void SetWarningIndicatorLevel(int level)
        {
            switch (level)
            {
                case 0:
                    warningIndicator.SetActive(false);
                    severeWarningIndicator.SetActive(false);
                    break;
                case 1:
                    warningIndicator.SetActive(true);
                    severeWarningIndicator.SetActive(false);
                    break;
                case 2:
                    warningIndicator.SetActive(false);
                    severeWarningIndicator.SetActive(true);
                    break;
            }
        }
    }
}
