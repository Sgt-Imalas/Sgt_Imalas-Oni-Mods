
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace ModProfileManager_Addon.UnityUI.Components
{
    internal class ModScreenEntry : KMonoBehaviour
    {
        FToggle2 ModEnabled;
        public KMod.Mod TargetMod;
        LocText ModName;

        GameObject PlibConfigHighlight;
        ToolTip plibTooltip;

        public string Name=string.Empty;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ModName = transform.Find("Label").gameObject.GetComponent<LocText>();
            ModEnabled = transform.Find("Background").gameObject.AddComponent<FToggle2>();
            ModEnabled.SetCheckmark("Checkmark");
            PlibConfigHighlight = transform.Find("HasPLibData").gameObject;
            plibTooltip = UIUtils.AddSimpleTooltipToObject(PlibConfigHighlight, "");

            if (TargetMod != null)
            {
                Name = TargetMod.label.title;
                this.gameObject.name = Name;


                ModName?.SetText(Name);
                var label = transform.Find("ModType/Label").gameObject.GetComponent<LocText>();
                if (TargetMod.IsLocal)
                {
                    transform.Find("ModType").gameObject.GetComponent<Image>().color = ModAssets.Colors.Blue;
                    label.SetText(STRINGS.UI.LOCAL_MOD);
                }
                else
                {
                    label.SetText(global::STRINGS.UI.PLATFORMS.STEAM);
                }
                ModEnabled.OnClick += (active) =>
                {
                    ModAssets.ToggleModActive(TargetMod.label, active);
                };
            }
        }
        public void Refresh(bool enabled, bool hasPlibConfig, string plibData)
        {
            PlibConfigHighlight.SetActive(hasPlibConfig);
            if(hasPlibConfig)
            {
                plibTooltip.SetSimpleTooltip(STRINGS.UI.PLIB_CONFIG_FOUND+"\n"+plibData);
            }
            ModEnabled.SetOnFromCode(enabled);
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
    }
}
